using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Lab2
{
    public class XmlFileReader : IFileReader
    {
        private XmlReader reader;

        public XmlFileReader(XmlReader xmlReader)
        {
            reader = xmlReader;
        }

        public bool EndOfFile()
        {
            while (reader.NodeType == XmlNodeType.Whitespace)
            {
                reader.Read();
            }
            return reader.EOF || (reader.NodeType == XmlNodeType.EndElement && reader.Name == "root");
        }

        public Address Next()
        {
            while (reader.Name != "item" && !reader.EOF)
            {
                reader.Read();
            }
            if (reader.EOF)
            {
                throw new EndOfStreamException();
            }
            var result = new Address
            {
                city = reader.GetAttribute("city"),
                street = reader.GetAttribute("street"),
                house = reader.GetAttribute("house"),
                floor  = reader.GetAttribute("floor")
            };
            reader.Read();
            return result;
        }
    }
}
