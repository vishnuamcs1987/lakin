using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amcs.Tools.Xml;
using ImportYards;
using Tfp.Actions;
using Tfp.Datamodel;
using Tfp.Datamodel.Extensions;
using Tfp.Datamodel.Lookup;
using Tfp.Gateways;
using Tfp.TrpCloudServiceReference;

namespace ImportDrivers
{
    public class ImportDriver: IExternal
    {
        public string Run(string input)
        {
            DriverImportBody importData = XmlSerializerCache.Deserialize<DriverImportBody>(input, "BODY");
            if (importData == null || importData.Record == null)
            {
                throw new InvalidDataException("Error: Could not deserialize XML. Check structure.");
            }
            DriverRecord driverRecord = importData.Record;

            Driver driver = new Driver()
            {
                ForeignID = driverRecord.DrId,
                Name = driverRecord.DrName,
            };
            
            UpdateTransaction updateTransaction = new UpdateTransaction("Importing driver" + driver.ForeignID);

            if (!string.IsNullOrEmpty(driverRecord.DrAddress1) && !string.IsNullOrEmpty(driverRecord.DrCity) && !string.IsNullOrEmpty(driverRecord.DrPostalCode))
            {
                Tfp.Datamodel.Address address = new Tfp.Datamodel.Address()
                {
                    City = driverRecord.DrCity,
                    ZipCode = driverRecord.DrPostalCode,
                    Country = "USA",
                };
                AddressParser.ParseStreet(driverRecord.DrAddress1, address);

                Tfp.TfpRequests.GeocodeRequest geocodeRequest = new Tfp.TfpRequests.GeocodeRequest()
                {
                    Address = address,
                    FailOnError = true,
                };
                geocodeRequest = Tfp.TfpRequests.TFPRequestInterface.PerformRequest(geocodeRequest);
                if (!string.IsNullOrEmpty(geocodeRequest.ErrorMessage))
                {
                    // Geocoding failed
                    throw new InvalidOperationException("Geocoding driver failed: " + address.Street + ", " + address.HouseNo + ", "
                        + address.ZipCode + ", " + address.City + ". Error: " + geocodeRequest.ErrorMessage);
                }
                else
                {
                    address = geocodeRequest.Address;
                }

                Destination dest = new Destination()
                {
                    ForeignID = driver.ForeignID,
                    Name = driver.Name,
                    Address = address
                };

                updateTransaction.UpdateObject(dest);
                driver.Address = new Tfp.Datamodel.Address() { DestinationReference = new DataReference<Destination>(dest) };
            }
            
            if (!string.IsNullOrEmpty(driverRecord.DrQualificationid))
            {
                Qualification qualification = DataMap<Qualification>.GetById(driverRecord.DrQualificationid);
                if (qualification == null)
                {
                    qualification = new Qualification()
                    {
                       ForeignID = driverRecord.DrQualificationid,
                       Name = driverRecord.DrQualificationid,
                       TypeFrom = Qualification.QualificationType.Vehicle,
                       TypeTo = Qualification.QualificationType.Driver,
                    };
                    updateTransaction.NewObject(qualification);
                    driver.QualificationEntries = new QualificationEntries()
                    {
                        new QualificationEntry()
                        {
                            Qualification = new DataReference<Qualification>(qualification),
                        },
                    };
                }
            }

            updateTransaction.NewObject(driver);

            // Infotexts
            UpdateInfoTextAction infotextAction1 = new UpdateInfoTextAction("DRIVER[" + driver.ForeignID + "]", driverRecord.DrSubsidiary, "SUBSIDIARY", false);
            updateTransaction.PerformAction(infotextAction1);

            UpdateInfoTextAction infotextAction2 = new UpdateInfoTextAction("DRIVER[" + driver.ForeignID + "]", driverRecord.DrTractor, "TRACTOR", false);
            updateTransaction.PerformAction(infotextAction2);

            // Return the Transaction to the Import Server
            return updateTransaction.ToString();
        }
    }
}
