using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Amcs.Tools.Xml;
using Tfp.Actions;
using Tfp.Datamodel;
using Tfp.Gateways;

namespace ImportVehicles
{
    public class ImportVehicle : IExternal
    {
        public string Run(string input)
        {
            VehicleImportBody importData = XmlSerializerCache.Deserialize<VehicleImportBody>(input, "BODY");

            if (importData == null || importData.Record == null)
            {
                throw new InvalidDataException("Error: Could not deserialize XML. Check structure.");
            }
            VehicleRecord vehicleRecord = importData.Record;

            Vehicle vehicle = new Vehicle
            {
                ForeignID = vehicleRecord.VehicleId,
                Name = vehicleRecord.VehicleNo
            };

            // Create an import TFP Transaction with a description for the Transaction Log, and fill in objects and actions
            UpdateTransaction updateTransaction = new UpdateTransaction("Importing Vehicle " + vehicle.ForeignID);
            updateTransaction.NewObject(vehicle);

            // Return the Transaction to the Import Server
            return updateTransaction.ToString();
        }
    }
}

