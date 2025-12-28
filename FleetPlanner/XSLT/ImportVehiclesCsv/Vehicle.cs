using System;
using System.Xml.Serialization;

namespace ImportVehiclesCsv
{
    //[XmlRoot("VehicleRecord")]
    public class VehicleRecord
    {
        [XmlElement("VEH_ID")]
        public string VehicleId { get; set; } // faa

        [XmlElement("VEH_VEHICLENO")]
        public string VehicleNo { get; set; } // 103059

        [XmlElement("VEH_TYPE_CATEGORY")]
        public string TypeCategory { get; set; } // BOX TRUCK

        [XmlElement("VEH_TYPE")]
        public string VehicleType { get; set; } // BOX TRUCK

        // Keep Raw logic for Numeric fields that might be "NULL"
        [XmlElement("VEH_KMCOST")]
        public string KmCostRaw { get; set; }
        public decimal? KmCost
        {
            get
            {
                return ParseDecimal(KmCostRaw);
            }
        }

        [XmlElement("VEH_VEHICLE_COST_PROFILE")]
        public string CostProfile { get; set; } // Directly as string

        [XmlElement("EQY_START_ID")]
        public string EqyStartId { get; set; }

        [XmlElement("EQY_END_ID")]
        public string EqyEndId { get; set; }

        [XmlElement("VEH_CAPACITY1")]
        public double Capacity1 { get; set; } // 2000

        [XmlElement("VEH_CAPACITY2")]
        public double Capacity2 { get; set; } // 0

        [XmlElement("VEH_CAPACITY3")]
        public double Capacity3 { get; set; } // 450

        // These are now standard strings to handle "NULL" or IDs
        [XmlElement("VEH_QUALIFICATION_ID")]
        public string QualificationId { get; set; }

        [XmlElement("QF_QUALIFICATION_ID")]
        public string QfQualificationId { get; set; }

        private decimal? ParseDecimal(string val)
        {
            // 1. Check if the text is missing or explicitly says "NULL"
            if (string.IsNullOrEmpty(val) || val == "NULL")
            {
                return null;
            }

            // 2. Otherwise, try to turn the text into a number
            return decimal.Parse(val);
        }
    }
    
    [XmlRoot("BODY")]
    public class VehicleImportBody
    {
        [XmlElement("VehicleRecord")]
        public VehicleRecord Record { get; set; }
    }
}