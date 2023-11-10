using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2
{
    public struct Address
    {
        public string city;
        public string street;
        public string house;
        public string floor;

        public override string ToString()
        {
            return $"{city}, улица {street}, дом {house}, этаж {floor}";
        }
    }
}
