namespace Lab2
{
    public struct Address
    {
        public string city;
        public string street;
        public int house;
        public int floor;

        public override string ToString()
        {
            return $"{city}, улица {street}, дом {house}, этаж {floor}";
        }
    }
}
