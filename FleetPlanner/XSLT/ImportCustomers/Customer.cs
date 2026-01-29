using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ImportCustomers
{
    public class CustomerRecord
    {
        [XmlElement("FP_CSID")]
        public string FpCsid { get; set; }

        [XmlElement("CS_TERMINAL_NO")]
        public string CsTerminalNo { get; set; }

        [XmlElement("CS_SERVICE_ID")]
        public string CsServiceId { get; set; }

        [XmlElement("CS_NAME")]
        public string CsName { get; set; }

        [XmlElement("CS_STREET_ADDRESS1")]
        public string CsStreetAddress1 { get; set; }

        [XmlElement("CS_STREET_ADDRESS2")]
        public string CsStreetAddress2Raw { get; set; }
        public decimal? CsStreetAddress2
        {
            get
            {
                return ParseDecimal(CsStreetAddress2Raw);
            }
        }

        [XmlElement("CS_POSTAL_CODE")]
        public string CsPostalCode { get; set; }

        [XmlElement("CS_CITY")]
        public string CsCity { get; set; }

        [XmlElement("CS_STATE")]
        public string CsState { get; set; }

        [XmlElement("CS_SITE")]
        public string CsSite { get; set; }

        [XmlElement("CS_SERVICE_DAILY_EXPECTED_QTY")]
        public string CsServiceDailyExpectedQtyRaw { get; set; }
        public decimal? CsServiceDailyExpectedQty
        {
            get
            {
                return ParseDecimal(CsServiceDailyExpectedQtyRaw);
            }
        }

        [XmlElement("CS_SERVICE_CAPACITY")]
        public string CsServiceCapacityRaw { get; set; }
        public decimal? CsServiceCapacity
        {
            get
            {
                return ParseDecimal(CsServiceCapacityRaw);
            }
        }

        [XmlElement("CS_SERVICE_FREQUENCY")]
        public string CsServiceFrequencyRaw { get; set; }
        public decimal? CsServiceFrequency
        {
            get
            {
                return ParseDecimal(CsServiceFrequencyRaw);
            }
        }

        [XmlElement("CS_SERVICE_TRAILER_SIZE")]
        public string CsServiceTrailerSize { get; set; }

        [XmlElement("CS_SERVICE_DAYS")]
        public string CsServiceDays { get; set; }

        [XmlElement("CS_LATITUDE")]
        public string CsLatitudeRaw { get; set; }
        public decimal? CsLatitude
        {
            get
            {
                return ParseDecimal(CsLatitudeRaw);
            }
        }

        [XmlElement("CS_LONGITUDE")]
        public string CsLongitudeRaw { get; set; }
        public decimal? CsLongitude
        {
            get
            {
                return ParseDecimal(CsLongitudeRaw);
            }
        }

        [XmlElement("CS_HOS_START")]
        public string CsHosStart { get; set; }

        [XmlElement("CS_HOS_END")]
        public string CsHosEnd { get; set; }

        [XmlElement("CS_TW1_OPEN_TIME")]
        public string CsTw1OpenTime { get; set; }

        [XmlElement("CS_TW1_CLOSE_TIME")]
        public string CsTw1CloseTime { get; set; }

        [XmlElement("CS_TW2_OPEN_TIME")]
        public string CsTw2OpenTimeRaw { get; set; }
        public decimal? CsTw2OpenTime
        {
            get
            {
                return ParseDecimal(CsTw2OpenTimeRaw);
            }
        }

        [XmlElement("CS_TW2_CLOSE_TIME")]
        public string CsTw2CloseTimeRaw { get; set; }
        public decimal? CsTw2CloseTime
        {
            get
            {
                return ParseDecimal(CsTw2CloseTimeRaw);
            }
        }

        private decimal? ParseDecimal(string val)
        {
            if (string.IsNullOrEmpty(val) || val == "NULL") return null;
            decimal res; return decimal.TryParse(val, out res) ? res : (decimal?)null;
        }

        private int? ParseInt(string val)
        {
            if (string.IsNullOrEmpty(val) || val == "NULL") return null;
            int res; return int.TryParse(val, out res) ? res : (int?)null;
        }

        [XmlRoot("BODY")]
        public class CustomerImportBody
        {
            [XmlElement("CustomerRecord")]
            public CustomerRecord Record { get; set; }
        }
    }
}
