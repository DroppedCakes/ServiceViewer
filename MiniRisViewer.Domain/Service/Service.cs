using System;
using System.Xml.Serialization;

namespace MiniRisViewer.Domain.Service
{
    [Serializable]
    public class Service
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("visible")]
        public string Visible { get; set; }

        [XmlElement("Caption")]
        public string Caption { get; set; }

        [XmlElement("LogPath")]
        public string LogPath { get; set; }
    }
}