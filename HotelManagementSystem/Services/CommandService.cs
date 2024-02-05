using HotelManagementSystem.Models;
using HotelManagementSystem.Services.Interface;

namespace HotelManagementSystem.Services
{
    public class CommandService : ICommandService
    {
        public List<CommandModel> GetCommandsFromFileName(string fileName)
        {
            try
            {
                return System.IO.File.ReadLines(fileName)
                    .Where(line => !string.IsNullOrWhiteSpace(line))
                    .Select(line => line.Split(' ').Select(param => param.Trim()).ToArray())
                    .Select(parts => new CommandModel(
                        parts[0],
                        parts.Skip(1).Select(param => int.TryParse(param, out int parsedParam) ? (object)parsedParam : param.Trim()).ToList()
                    ))
                    .ToList();
            }
            catch (IOException e)
            {
                Console.WriteLine($"An error occurred while reading the file: {e.Message}");
                return new List<CommandModel>();
            }
        }
        public void Book(int roomNumber, string guestName, int guestAge, ref Hotel hotel, ref List<Guest> guests, ref string response)
        {
            if (hotel == null)
            {
                response += "Error hotel not created.\r\n";
                return;
            }

            var room = GetRoomByNumber(roomNumber, hotel);
            if (room == null)
            {
                response += $"Error Room {roomNumber} does not exist.\r\n";
                return;
            }

            if (room.Occupant != null)
            {
                response += $"Cannot book room {roomNumber} for {guestName}, The room is currently booked by {room.Occupant.Name}.\r\n";
            }

            var keycardNumber = guests.Count + 1;
            var guest = new Guest { KeycardNumber = keycardNumber, Name = guestName, Age = guestAge };
            guests.Add(guest);
            room.Occupant = guest;

            response += $"Room {roomNumber} is booked by {guestName} with keycard number {keycardNumber}.\r\n";
        }
        public Room GetRoomByNumber(int roomNumber, Hotel hotel)
        {
            return hotel.Floors.SelectMany(floor => floor).FirstOrDefault(room => room.Number == roomNumber) ?? new();
        }
        public void ListAvailableRooms(Hotel hotel, ref string response)
        {
            if (hotel == null)
            {
                response += "Error: Hotel not created.\r\n";
                return;
            }

            var availableRooms = hotel.Floors
                .SelectMany(floor => floor.Where(room => room.Occupant == null))
                .Select(room => room.Number);

            response += $"Available rooms: {string.Join(", ", availableRooms)}\r\n";
        }

    }
}
