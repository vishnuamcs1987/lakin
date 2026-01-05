using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amcs.Tools.Xml;
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
            UpdateTransaction updateTransaction = new UpdateTransaction("Importing driver address" + driver.ForeignID);

            if (!string.IsNullOrEmpty(driverRecord.DrAddress1) && !string.IsNullOrEmpty(driverRecord.DrCity) && !string.IsNullOrEmpty(driverRecord.DrPostalCode))
            {
                Tfp.Datamodel.Address address = new Tfp.Datamodel.Address()
                {
                    Street = driverRecord.DrAddress1,
                    HouseNo = "",
                    City = driverRecord.DrCity,
                    ZipCode = driverRecord.DrPostalCode,
                    Country = "USA",
                };
                Destination dest = new Destination()
                {
                    ForeignID = driver.ForeignID,
                    Name = driver.Name,
                    Address = address
                };
                updateTransaction.NewObject(dest); 
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

            updateTransaction.NewObject(driver); //fixme new vs update? how do I completely over-write driver object

            // Infotexts
            if (!string.IsNullOrEmpty(driverRecord.DrSubsidiary))
            {
                UpdateInfoTextAction infotextAction1 = new UpdateInfoTextAction("DRIVER[" + driver.ForeignID + "]", driverRecord.DrSubsidiary, "Subsidiary", false);
                updateTransaction.PerformAction(infotextAction1);
            }
            if (!string.IsNullOrEmpty(driverRecord.DrTractor)) //fixme askPeder 
            {
                UpdateInfoTextAction infotextAction2 = new UpdateInfoTextAction("DRIVER[" + driver.ForeignID + "]", driverRecord.DrTractor, "Tractor", false);
                updateTransaction.PerformAction(infotextAction2);
            }

            // Return the Transaction to the Import Server
            return updateTransaction.ToString();
        }
    }
}
