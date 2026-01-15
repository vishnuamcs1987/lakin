using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Amcs.Tools.Xml;
using ImportYards;
using Tfp.Actions;
using Tfp.Datamodel;
using Tfp.Gateways;

namespace ImportYards
{
    public class ImportYard : IExternal
    {
        public string Run(string input)
        {
            YardImportBody importData = XmlSerializerCache.Deserialize<YardImportBody>(input, "BODY");

            if (importData == null || importData.Record == null)
            {

                throw new InvalidDataException("Error: Could not deserialize XML. Check structure.");
            }
            YardRecord yardRecord = importData.Record;

            Terminal terminal = new Terminal()
            {
                ForeignID = yardRecord.FpEqyid,
                Name = yardRecord.EqyName,
                Name2 = yardRecord.EqyTerminalNo,
                //QualificationProfiles = new List<DataReference<QualificationProfile>>(),
                //Qualifications = new QualificationEntries { Entries = new List<QualificationEntry>() },
                //IgnoreInventoryCollectionTime = true,
                //DeliverySequence = 1,
                //PickupSequence = 2,
            };

            Address address = new Address()
            {
                HousePostfix = yardRecord.EqyStreetAddress2,
                City = yardRecord.EqyCity,
                ZipCode = yardRecord.EqyPostalCode,
                Country = "USA",
            };
            AddressParser.ParseStreet(yardRecord.EqyStreetAddress1, address);

            if (yardRecord.EqyLatitude.HasValue && yardRecord.EqyLongitude.HasValue)
            {
                double lat = (double)yardRecord.EqyLatitude.Value;
                double lon = (double)yardRecord.EqyLongitude.Value;
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
                    FailOnError = false,
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
                if (!string.IsNullOrEmpty(yardRecord.EqyHosStart))
                    terminal.OpenFrom = TimeSpan.Parse(yardRecord.EqyHosStart);
                if (!string.IsNullOrEmpty(yardRecord.EqyHosEnd))
                    terminal.OpenTo = TimeSpan.Parse(yardRecord.EqyHosEnd);
            }
            catch (Exception)
            {
                throw new Exception(string.Format("EQY {0}: Opening hours is not formatted correcly [{1};{2}]",
                    terminal.ForeignID, yardRecord.EqyHosStart, yardRecord.EqyHosEnd));
            }

            //Infotexts
            //if (updateWasteDisposal.Infotexts != null)
            //{
            //    foreach (Infotext infoText in updateWasteDisposal.Infotexts)
            //    {
            //        dataBucket.Actions.Add(new UpdateInfoTextAction("TERMINAL[" + updateWasteDisposal.Id + "]", infoText.Value, infoText.Key, false));
            //    }
            //}

            // Create an import TFP Transaction with a description for the Transaction Log, and fill in objects and actions
            updateTransaction.NewObject(terminal);

            // Return the Transaction to the Import Server
            return updateTransaction.ToString();
        }
    }
}
