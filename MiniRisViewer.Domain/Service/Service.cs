using System.Xml.Serialization;

namespace MiniRisViewer.Domain.Service
{
    public class Service
    {
        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "Visible")]
        public bool Visible { get; set; }

        [XmlElement(ElementName = "Caption")]
        public string Caption { get; set; }

        [XmlElement(ElementName = "LogPath")]
        public string LogPath { get; set; }
    }
}