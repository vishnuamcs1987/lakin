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
            XmlDocument doc = new XmlDocument(); //Make new XML document class
            doc.LoadXml(input); // Populate class with input from file
            string root = doc.SelectSingleNode("/*").Name; // get the 'root' (outer layer of the XML file) from the import
            //Order inputOrder = XmlSerializerCache.Deserialize<Order>(input, root); 
            //'Unpack' the XML file by removing the outer most bracket, in this case <Order> </Order>
            
            Vehicle vehicle = new Vehicle();
            vehicle.ForeignID = "veh123";
            vehicle.Name = "Import Vehicle 123";

            // Create an import TFP Transaction with a description for the Transaction Log, and fill in objects and actions
            UpdateTransaction updateTransaction = new UpdateTransaction("Importing Vehicle " + vehicle.ForeignID);
            updateTransaction.UpdateObject(vehicle);

            // Return the Transaction to the Import Server
            return updateTransaction.ToString();
        }
    }
}
