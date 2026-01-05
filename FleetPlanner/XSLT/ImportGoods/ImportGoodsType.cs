using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Amcs.Tools.Xml;
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
                string.IsNullOrWhiteSpace(importData.Record.Description))
            {
                throw new InvalidDataException("Error: ITEM and DESCRIPTION must not be empty for goods type.");
            }

            InventoryType inventoryType = new InventoryType()
            {
                ForeignID = inventoryRecord.Item,
                Name = inventoryRecord.Description,
            };
            UpdateTransaction updateTransaction = new UpdateTransaction("Importing goods type" + inventoryType.ForeignID);
            updateTransaction.NewObject(inventoryType);
            return updateTransaction.ToString();
        }
    }

    public class InventoryRecord
    {
        [XmlElement("ITEM")]
        public string Item { get; set; }

        [XmlElement("DESCRIPTION")]
        public string Description { get; set; }
    }

    [XmlRoot("BODY")]
    public class InventoryImportBody
    {
        [XmlElement("InventoryRecord")]
        public InventoryRecord Record { get; set; }
    }
}


