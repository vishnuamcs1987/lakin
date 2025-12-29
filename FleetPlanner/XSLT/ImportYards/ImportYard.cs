using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Amcs.Tools.Xml;
using ImportYards;
using Tfp.Actions;
using Tfp.Datamodel;
using Tfp.Gateways;

namespace ImportYard
{
    public class ImportYard : IExternal
    {
        public string Run(string input)
        {
            YardImportBody importData = XmlSerializerCache.Deserialize<YardImportBody>(input, "BODY");

            // 2. Safety check - using standard if-statement for older compilers
            if (importData == null || importData.Record == null)
            {
                // fixme
                return "Error: Could not deserialize XML. Check structure.";
            }
            YardRecord yardRecord = importData.Record;

            Terminal terminal = new Terminal();
            terminal.ForeignID = yardRecord.FpEqyid;
            terminal.Name = yardRecord.EqyTerminalNo;

            // Create an import TFP Transaction with a description for the Transaction Log, and fill in objects and actions
            UpdateTransaction updateTransaction = new UpdateTransaction("Importing Yard " + terminal.ForeignID);
            updateTransaction.UpdateObject(terminal);

            // Return the Transaction to the Import Server
            return updateTransaction.ToString();
        }
    }
}
