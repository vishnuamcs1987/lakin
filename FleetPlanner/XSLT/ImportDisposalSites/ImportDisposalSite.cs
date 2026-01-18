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

namespace ImportDisposalSites
{
    public class ImportDisposalSite : IExternal
    {
        public string Run(string input)
        {
            DisposalSiteImportBody importData = XmlSerializerCache.Deserialize<DisposalSiteImportBody>(input, "BODY");

            if (importData == null || importData.Record == null)
            {

                throw new InvalidDataException("Error: Could not deserialize XML. Check structure.");
            }
            DisposalSiteRecord siteRecord = importData.Record;

            Terminal terminal = new Terminal()
            {
                ForeignID = siteRecord.FpDsid,
                Name = siteRecord.DsName,
                Name2 = siteRecord.DsServiceId,
                //QualificationProfiles = new List<DataReference<QualificationProfile>>(),
                //Qualifications = new QualificationEntries { Entries = new List<QualificationEntry>() },
                //IgnoreInventoryCollectionTime = true,
                //DeliverySequence = 1,
                //PickupSequence = 2,
            };

            Address address = new Address()
            {
                HousePostfix = siteRecord.DsStreetAddress2,
                City = siteRecord.DsCity,
                ZipCode = siteRecord.DsPostalCode,
                Country = "USA",
            };
            AddressParser.ParseStreet(siteRecord.DsStreetAddress1, address);

            if (siteRecord.DsLatitude.HasValue && siteRecord.DsLongitude.HasValue)
            {
                double lat = (double)siteRecord.DsLatitude.Value;
                double lon = (double)siteRecord.DsLongitude.Value;
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
                if (!string.IsNullOrEmpty(siteRecord.DsHosStart))
                    terminal.OpenFrom = TimeSpan.Parse(siteRecord.DsHosStart);
                if (!string.IsNullOrEmpty(siteRecord.DsHosEnd))
                    terminal.OpenTo = TimeSpan.Parse(siteRecord.DsHosEnd);
            }
            catch (Exception)
            {
                throw new Exception(string.Format("EQY {0}: Opening hours is not formatted correcly [{1};{2}]",
                    terminal.ForeignID, siteRecord.DsHosStart, siteRecord.DsHosEnd));
            }


            terminal.Address = new Address() { DestinationReference = new DataReference<Destination>(dest) };
            // Infotexts
            UpdateInfoTextAction infotextAction1 = new UpdateInfoTextAction("TERMINAL[" + terminal.ForeignID + "]",
                siteRecord.DsTerminalNo, "TERMINAL_NO", false);
            updateTransaction.PerformAction(infotextAction1);

            UpdateInfoTextAction infotextAction2 = new UpdateInfoTextAction("TERMINAL[" + terminal.ForeignID + "]", siteRecord.DsServiceReference, 
                "SERVICE_REFERENCE", false);
            updateTransaction.PerformAction(infotextAction1);

            UpdateInfoTextAction infotextAction3 = new UpdateInfoTextAction("TERMINAL[" + terminal.ForeignID + "]", siteRecord.DsServiceId.
                ToString(), "Subsidiary", false);
            updateTransaction.PerformAction(infotextAction2);


            updateTransaction.UpdateObject(terminal);

            // Return the Transaction to the Import Server
            return updateTransaction.ToString();
        }
    }
}
