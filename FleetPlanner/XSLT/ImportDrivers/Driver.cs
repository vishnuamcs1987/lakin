using System;
using System.Xml.Serialization;

namespace ImportDrivers
{
    public class DriverRecord
    {
        [XmlElement("DR_ID")]
        public string DrId { get; set; }

        [XmlElement("DR_SUBSIDIARY")]
        public string DrSubsidiary { get; set; }

        [XmlElement("DR_NAME")]
        public string DrName { get; set; }

        [XmlElement("DR_ADDRESS1")]
        public string DrAddress1 { get; set; }

        [XmlElement("DR_POSTAL_CODE")]
        public string DrPostalCode { get; set; }

        [XmlElement("DR_CITY")]
        public string DrCity { get; set; }

        [XmlElement("DR_QUALIFICATIONID")]
        public string DrQualificationid { get; set; }

        [XmlElement("DR_TRACTOR")]
        public string DrTractor { get; set; }

    }

    [XmlRoot("BODY")]
    public class DriverImportBody
    {
        [XmlElement("DriverRecord")]
        public DriverRecord Record { get; set; }
    }
}
