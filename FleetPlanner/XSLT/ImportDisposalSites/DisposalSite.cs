using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ImportDisposalSites
{
    public class DisposalSiteRecord
    {
        [XmlElement("FP_DSID")]
        public string FpDsid { get; set; }

        [XmlElement("DS_TERMINAL_NO")]
        public string DsTerminalNo { get; set; }

        [XmlElement("DS_SERVICE_ID")]
        public string DsServiceId { get; set; }

        [XmlElement("DS_NAME")]
        public string DsName { get; set; }

        [XmlElement("DS_STREET_ADDRESS1")]
        public string DsStreetAddress1 { get; set; }

        [XmlElement("DS_STREET_ADDRESS2")]
        public string DsStreetAddress2 { get; set; }

        [XmlElement("DS_POSTAL_CODE")]
        public string DsPostalCode { get; set; }

        [XmlElement("DS_CITY")]
        public string DsCity { get; set; }

        [XmlElement("DS_STATE")]
        public string DsState { get; set; }

        [XmlElement("DS_SERVICE_REFERENCE")]
        public string DsServiceReference { get; set; }

        [XmlElement("DS_SERVICE_DAILY_EXPECTED_QTY")]
        public string DsServiceDailyExpectedQtyRaw { get; set; }
        public decimal? DsServiceDailyExpectedQty
        {
            get
            {
                return ParseDecimal(DsServiceDailyExpectedQtyRaw);
            }
        }

        [XmlElement("DS_SERVICE_CAPACITY")]
        public string DsServiceCapacityRaw { get; set; }
        public decimal? DsServiceCapacity
        {
            get
            {
                return ParseDecimal(DsServiceCapacityRaw);
            }
        }

        [XmlElement("DS_SERVICE_FREQUENCY")]
        public string DsServiceFrequencyRaw { get; set; }
        public decimal? DsServiceFrequency
        {
            get
            {
                return ParseDecimal(DsServiceFrequencyRaw);
            }
        }

        [XmlElement("DS_SERVICE_TRAILER_SIZE")]
        public string DsServiceTrailerSize { get; set; }

        [XmlElement("DS_LATITUDE")]
        public string DsLatitudeRaw { get; set; }
        public decimal? DsLatitude
        {
            get
            {
                return ParseDecimal(DsLatitudeRaw);
            }
        }

        [XmlElement("DS_LONGITUDE")]
        public string DsLongitudeRaw { get; set; }
        public decimal? DsLongitude
        {
            get
            {
                return ParseDecimal(DsLongitudeRaw);
            }
        }

        [XmlElement("DS_DELIVERY_DAYS")]
        public string DsDeliveryDays { get; set; }

        [XmlElement("DS_HOS_START")]
        public string DsHosStart { get; set; }

        [XmlElement("DS_HOS_END")]
        public string DsHosEnd { get; set; }

        [XmlElement("DS_TW1_OPEN_TIME")]
        public string DsTw1OpenTime { get; set; }

        [XmlElement("DS_TW1_CLOSE_TIME")]
        public string DsTw1CloseTime { get; set; }

        [XmlElement("DS_TW2_OPEN_TIME")]
        public string DsTw2OpenTimeRaw { get; set; }
        public decimal? DsTw2OpenTime
        {
            get
            {
                return ParseDecimal(DsTw2OpenTimeRaw);
            }
        }

        [XmlElement("DS_TW2_CLOSE_TIME")]
        public string DsTw2CloseTimeRaw { get; set; }
        public decimal? DsTw2CloseTime
        {
            get
            {
                return ParseDecimal(DsTw2CloseTimeRaw);
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
    }

                                                                                                                                                                [XmlRoot("BODY")]
                                                                                                                                                                public class DisposalSiteImportBody
                                                                                                                                                                {
                                                                                                                                                                    [XmlElement("DisposalSiteRecord")]
                                                                                                                                                                    public DisposalSiteRecord Record { get; set; }
                                                                                                                                                                }
}