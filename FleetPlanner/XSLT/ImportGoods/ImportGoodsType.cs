using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Amcs.Tools.Xml;
using Tfp.Actions;
using Tfp.Datamodel;
using Tfp.Gateways;

namespace ImportGoods
{
    public class ImportGoodsType : IExternal
    {
        public string Run(string input)
        {
            InventoryImportBody importData = XmlSerializerCache.Deserialize<InventoryImportBody>(input, "BODY");
            if (importData == null || importData.Record == null)
            {
                throw new InvalidDataException("Error: Could not deserialize XML. Check structure.");
            }
            InventoryRecord inventoryRecord = importData.Record;


            // Check for non-empty strings
            if (string.IsNullOrWhiteSpace(importData.Record.Item) ||
                string.IsNullOrWhiteSpace(importData.Record.Description) ||
                string.IsNullOrWhiteSpace(importData.Record.Type))
            {
                throw new InvalidDataException("Error: ITEM, DESCRIPTION, TYPE must not be empty for goods type.");
            }

            InventoryType inventoryType = new InventoryType()
            {
                ForeignID = inventoryRecord.Item,
                Name = inventoryRecord.Item,
                Group = new DataReference<InventoryType>(inventoryRecord.Type),

            };
            UpdateTransaction updateTransaction = new UpdateTransaction("Importing goods type" + inventoryType.ForeignID);
            updateTransaction.NewObject(inventoryType);

            if (!string.IsNullOrEmpty(inventoryRecord.Description))
            {
                UpdateInfoTextAction infotextAction1 = new UpdateInfoTextAction("INVENTORYTYPE[" + inventoryType.ForeignID + "]", inventoryRecord.Description,
                    "Description",false);
                updateTransaction.PerformAction(infotextAction1);
            }
            return updateTransaction.ToString();
        }
    }

    public class InventoryRecord
    {
        [XmlElement("ITEM")]
        public string Item { get; set; }

        [XmlElement("DESCRIPTION")]
        public string Description { get; set; }

        [XmlElement("TYPE")]
        public string Type { get; set; }
    }

    [XmlRoot("BODY")]
    public class InventoryImportBody
    {
        [XmlElement("InventoryRecord")]
        public InventoryRecord Record { get; set; }
    }
}


