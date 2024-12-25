using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FA.Automation.MessageBus
{
    public class MyXMLEntity
    {
        public string NodeName { get; set; }
        public Dictionary<string, string> Attrs = new Dictionary<string, string>();
        public string InnerText { get; set; }
        public string InnerXml { get; set; }
        public List<MyXMLEntity> SubNodes = new List<MyXMLEntity>();
    }
}
