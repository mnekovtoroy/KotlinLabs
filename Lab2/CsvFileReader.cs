using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2
{
    public class CsvFileReader : IFileReader
    {
        private StreamReader reader;

        public CsvFileReader(StreamReader streamReader)
        {
            reader = streamReader;
        }

        public bool EndOfFile()
        {
            return reader.EndOfStream;
        }

        public Address Next()
        {
            var line = reader.ReadLine();
            if(line == "\"city\";\"street\";\"house\";\"floor\"")
            {
                line = reader.ReadLine();
            }
            var values = line.Split(';');
            var result = new Address()
            {
                city = values[0],
                street = values[1],
                house = values[2],
                floor = values[3]
            };
            return result;
        }
    }
}
