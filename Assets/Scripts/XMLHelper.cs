using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;
using System.IO;

class XMLHelper
{
    public void ParseXML(string filepath)
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(filepath);
        doc.SelectSingleNode("what"); 
    }    
}
