using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Amcs.Tools.Xml;
using Tfp.Actions;
using Tfp.Datamodel;
using Tfp.Gateways;

namespace ImportVehiclesCsv
{
    public class Class1 : IExternal
    {
        public string Run(string input)
        {
            VehicleImportBody importData = XmlSerializerCache.Deserialize<VehicleImportBody>(input, "BODY");

            // 2. Safety check - using standard if-statement for older compilers
            if (importData == null || importData.Record == null)
            {
                // fixme
                return "Error: Could not deserialize XML. Check structure.";
            }
            VehicleRecord vehicleRecord = importData.Record;

            Vehicle vehicle = new Vehicle();
            vehicle.ForeignID = vehicleRecord.VehicleId;
            vehicle.Name = vehicleRecord.VehicleNo;

            // Create an import TFP Transaction with a description for the Transaction Log, and fill in objects and actions
            UpdateTransaction updateTransaction = new UpdateTransaction("Importing Vehicle " + vehicle.ForeignID);
            updateTransaction.UpdateObject(vehicle);

            // Return the Transaction to the Import Server
            return updateTransaction.ToString();
        }
    }
}

