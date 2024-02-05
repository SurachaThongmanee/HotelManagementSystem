namespace HotelManagementSystem.Models
{
    public class CommandModel
    {
        public string Name { get; }
        public List<object> Params { get; }

        public CommandModel(string name, List<object> @params)
        {
            Name = name;
            Params = @params;
        }
        class Guest
        {
            public int KeycardNumber { get; set; }
            public string? Name { get; set; }
            public int Age { get; set; }
        }

        class Room
        {
            public int Number { get; set; }
            public Guest Occupant { get; set; }
            public Room()
            {
                Occupant = new Guest();
            }
        }

        class Hotel
        {
            public List<List<Room>> Floors { get; }

            public Hotel(int floorCount, int roomsPerFloor)
            {
                Floors = new List<List<Room>>();
                for (int i = 0; i < floorCount; i++)
                {
                    var floor = new List<Room>();
                    for (int j = 0; j < roomsPerFloor; j++)
                    {
                        floor.Add(new Room { Number = (i + 1) * 100 + (j + 1) });
                    }
                    Floors.Add(floor);
                }
            }
        }
    }
}
