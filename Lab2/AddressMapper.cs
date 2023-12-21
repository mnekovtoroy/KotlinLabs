namespace Lab2
{
    public class AddressMapper
    {
        public Dictionary<string, Dictionary<string, Dictionary<int, int>>> cities =
            new Dictionary<string, Dictionary<string, Dictionary<int, int>>>();
        public Dictionary<Address, int> duplicates = new Dictionary<Address, int>();
        public Dictionary<string, int[]> floors = new Dictionary<string, int[]>();

        public void MapBuilding(Address address)
        {
            if (cities.ContainsKey(address.city))
            {
                if (cities[address.city].ContainsKey(address.street))
                {
                    if (cities[address.city][address.street].ContainsKey(address.house))
                    {
                        if (cities[address.city][address.street][address.house] == address.floor)
                        {
                            //Дупликат адреса
                            if (duplicates.ContainsKey(address))
                            {
                                duplicates[address]++;
                            } else
                            {
                                duplicates.Add(address, 2);
                            }
                            return;
                        }
                    }
                    else
                    {
                        //Новый дом на существующей улице
                        cities[address.city][address.street].Add(address.house, address.floor);
                    }
                } 
                else
                {
                    //Новая улица в существующем городе
                    var house = new Dictionary<int, int>();
                    house.Add(address.house, address.floor);
                    cities[address.city].Add(address.street, house);
                }

            } 
            else
            {
                //Новый город
                var house = new Dictionary<int, int>();
                house.Add(address.house, address.floor);
                var street = new Dictionary<string, Dictionary<int, int>>();
                street.Add(address.street, house);
                cities.Add(address.city, street);                
            }

            if (!floors.ContainsKey(address.city))
            {
                floors.Add(address.city, new int[] { 0, 0, 0, 0, 0 });
            }
            floors[address.city][address.floor-1]++;
        }
    }
}
