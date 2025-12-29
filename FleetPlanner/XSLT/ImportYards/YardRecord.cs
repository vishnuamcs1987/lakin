using System;
using System.Xml.Serialization;

namespace ImportYards
{
    public class YardRecord
    {
        [XmlElement("FP_EQYID")]
        public string FpEqyid { get; set; }

        [XmlElement("EQY_TERMINAL_NO")]
        public string EqyTerminalNo { get; set; }

        [XmlElement("EQY_SERVICE_ID")]
        public string EqyServiceIdRaw { get; set; }
        public int? EqyServiceId
        {
            get
            {
                return ParseInt(EqyServiceIdRaw);
            }
        }

        [XmlElement("EQY_NAME")]
        public string EqyName { get; set; }

        [XmlElement("EQY_STREET_ADDRESS1")]
        public string EqyStreetAddress1 { get; set; }

        [XmlElement("EQY_STREET_ADDRESS2")]
        public string EqyStreetAddress2 { get; set; }

        [XmlElement("EQY_POSTAL_CODE")]
        public string EqyPostalCodeRaw { get; set; }
        public int? EqyPostalCode
        {
            get
            {
                return ParseInt(EqyPostalCodeRaw);
            }
        }

        [XmlElement("EQY_CITY")]
        public string EqyCity { get; set; }

        [XmlElement("EQY_STATE")]
        public string EqyState { get; set; }

        [XmlElement("EQY_LATITUDE")]
        public string EqyLatitudeRaw { get; set; }
        public decimal? EqyLatitude
        {
            get
            {
                return ParseDecimal(EqyLatitudeRaw);
            }
        }

        [XmlElement("EQY_LONGITUDE")]
        public string EqyLongitudeRaw { get; set; }
        public decimal? EqyLongitude
        {
            get
            {
                return ParseDecimal(EqyLongitudeRaw);
            }
        }

        [XmlElement("EQY_HOS_START")]
        public string EqyHosStart { get; set; }

        [XmlElement("EQY_HOS_END")]
        public string EqyHosEnd { get; set; }

        [XmlElement("EQY_TW1_OPEN_TIME")]
        public string EqyTw1OpenTime { get; set; }

        [XmlElement("EQY_TW1_CLOSE_TIME")]
        public string EqyTw1CloseTime { get; set; }

        [XmlElement("EQY_TW2_OPEN_TIME")]
        public string EqyTw2OpenTime { get; set; }

        [XmlElement("EQY_TW2_CLOSE_TIME")]
        public string EqyTw2CloseTime { get; set; }

        private decimal? ParseDecimal(string val)
        {
            if (string.IsNullOrEmpty(val) || val == "NULL") return null;
            decimal res; return decimal.TryParse(val, out res) ? res : (decimal?)null;
        }

        private int? ParseInt(string val)
        {
            if (string.IsNullOrEmpty(val) || val == "NULL") return null;
            return int.TryParse(val, out int res) ? res : null;
        }
    }

    [XmlRoot("BODY")]
    public class YardImportBody
    {
        [XmlElement("YardRecord")]
        public YardRecord Record { get; set; }
    }
}