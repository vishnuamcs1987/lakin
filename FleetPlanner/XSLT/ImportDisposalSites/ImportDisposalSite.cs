using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amcs.Tools.Xml;
using Tfp.Datamodel;
using Tfp.Gateways;

namespace ImportDisposalSites
{
    public class ImportDisposalSite : IExternal
    {
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
                Name = siteRecord.DsTerminalNo,
                Name2 = siteRecord.DsName,
                //QualificationProfiles = new List<DataReference<QualificationProfile>>(),
                //Qualifications = new QualificationEntries { Entries = new List<QualificationEntry>() },
                //IgnoreInventoryCollectionTime = true,
                //DeliverySequence = 1,
                //PickupSequence = 2,
            };

            Address address = new Address()
            {
                Street = siteRecord.EqyStreetAddress1,
                HouseNo = siteRecord.EqyStreetAddress2,
                City = siteRecord.EqyCity,
                ZipCode = siteRecord.EqyPostalCode,
                Country = "USA",
            };

            if (siteRecord.EqyLatitude.HasValue && siteRecord.EqyLongitude.HasValue)
            {
                address.HomeLngLat = new LongLatCoord()
                {
                    Lat = new TfpNullable<double>((double)siteRecord.EqyLatitude.Value),
                    Long = new TfpNullable<double>((double)siteRecord.EqyLongitude.Value),
                };
            }

            Destination dest = new Destination()
            {
                ForeignID = terminal.ForeignID,
                Name = terminal.Name,
                Address = address
            };

            terminal.Address = new Address() { DestinationReference = new DataReference<Destination>(dest) };

            try
            {
                if (!string.IsNullOrEmpty(siteRecord.EqyHosStart))
                    terminal.OpenFrom = TimeSpan.Parse(siteRecord.EqyHosStart);
                if (!string.IsNullOrEmpty(siteRecord.EqyHosEnd))
                    terminal.OpenTo = TimeSpan.Parse(siteRecord.EqyHosEnd);
            }
            catch (Exception)
            {
                throw new Exception($"EQY {terminal.ForeignID}: Opening hours is not formatted correcly [{siteRecord.EqyHosStart};{siteRecord.EqyHosEnd}]");
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
            UpdateTransaction updateTransaction = new UpdateTransaction("Importing Yard " + terminal.ForeignID);
            updateTransaction.UpdateObject(terminal);

            // Return the Transaction to the Import Server
            return updateTransaction.ToString();
        }
    }
}
