using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amcs.Tools.Xml;
using ImportYards;
using Tfp.Actions;
using Tfp.Datamodel;
using Tfp.Gateways;
using static ImportCustomers.CustomerRecord;

namespace ImportCustomers
{
    public class ImportCustomer : IExternal
    {
        public string Run(string input)
        {
            CustomerImportBody importData = XmlSerializerCache.Deserialize<CustomerImportBody>(input, "BODY");

            if (importData == null || importData.Record == null)
            {

                throw new InvalidDataException("Error: Could not deserialize XML. Check structure.");
            }
            CustomerRecord customerRecord = importData.Record;

            Terminal terminal = new Terminal()
            {
                ForeignID = customerRecord.FpCsid,
                Name = customerRecord.CsName,
                Name2 = customerRecord.CsServiceId,
                //QualificationProfiles = new List<DataReference<QualificationProfile>>(),
                //Qualifications = new QualificationEntries { Entries = new List<QualificationEntry>() },
                //IgnoreInventoryCollectionTime = true,
                //DeliverySequence = 1,
                //PickupSequence = 2,
            };

            Address address = new Address()
            {
                HousePostfix = customerRecord.DsStreetAddress2,
                City = customerRecord.DsCity,
                ZipCode = customerRecord.DsPostalCode,
                Country = "USA",
            };
            AddressParser.ParseStreet(customerRecord.DsStreetAddress1, address);

            if (customerRecord.DsLatitude.HasValue && customerRecord.DsLongitude.HasValue)
            {
                double lat = (double)customerRecord.DsLatitude.Value;
                double lon = (double)customerRecord.DsLongitude.Value;
                LongLatCoord lnglat = new LongLatCoord(lon, lat);
                UTMCoord utmCoord = lnglat.ToUTMCoord();
                address.HomeUTM = utmCoord;
                address.RoadUTM = utmCoord;
            }
            else
            {
                Tfp.TfpRequests.GeocodeRequest geocodeRequest = new Tfp.TfpRequests.GeocodeRequest()
                {
                    Address = address,
                    FailOnError = true,
                };
                geocodeRequest = Tfp.TfpRequests.TFPRequestInterface.PerformRequest(geocodeRequest);
                if (!string.IsNullOrEmpty(geocodeRequest.ErrorMessage))
                {
                    // Geocoding failed
                    throw new InvalidOperationException("Geocoding yard failed: " + address.Street + ", " + address.HouseNo + ", "
                        + address.ZipCode + ", " + address.City + ". Error: " + geocodeRequest.ErrorMessage);
                }
                else
                {
                    address = geocodeRequest.Address;
                }
            }

            Destination dest = new Destination()
            {
                ForeignID = terminal.ForeignID,
                Name = terminal.Name,
                Address = address
            };

            UpdateTransaction updateTransaction = new UpdateTransaction("Importing Yard " + terminal.ForeignID);
            updateTransaction.NewObject(dest);
            terminal.Address = new Address() { DestinationReference = new DataReference<Destination>(dest) };

            try
            {
                if (!string.IsNullOrEmpty(customerRecord.DsHosStart))
                    terminal.OpenFrom = TimeSpan.Parse(customerRecord.DsHosStart);
                if (!string.IsNullOrEmpty(customerRecord.DsHosEnd))
                    terminal.OpenTo = TimeSpan.Parse(customerRecord.DsHosEnd);
            }
            catch (Exception)
            {
                throw new Exception(string.Format("EQY {0}: Opening hours is not formatted correcly [{1};{2}]",
                    terminal.ForeignID, customerRecord.DsHosStart, customerRecord.DsHosEnd));
            }

            if (!string.IsNullOrEmpty(customerRecord.DsDeliveryDays))
            {
                //fixme create method to check or create dutypatterns and assign terminal to it
            }



            // Infotexts
            TFPList<InfoTextKeyValue> activityInfotextsToUpdate = new List<InfoTextKeyValue>();


            updateTransaction.UpdateObject(terminal);

            // Return the Transaction to the Import Server
            return updateTransaction.ToString();
        }
    }
}
