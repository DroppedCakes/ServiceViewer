using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MiniRisViewer.Domain.Service
{
    [XmlRoot("Services")]
    public class Services
    {
        [XmlElement(ElementName ="Service")]
        public List<Service> ServicesList { get; set; }
    }
}
