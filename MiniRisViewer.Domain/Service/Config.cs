using System.Collections.Generic;
using System.Xml.Serialization;

namespace MiniRisViewer.Domain.Service
{
    [XmlRoot("config")]
    public class Config
    {
        [XmlArray("services")]
        [XmlArrayItem("service")]
        public List<Service> Services { get; set; }
    }
}