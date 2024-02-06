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
                return;
            }
            var keyCardNumberList = guests.Select(keyCardNumber => keyCardNumber.KeycardNumber).ToArray();
            var keycardNumber = FindKeyCardNumberAvailable(keyCardNumberList);
            var guest = new Guest { KeycardNumber = keycardNumber, Name = guestName, Age = guestAge };
            guests.Add(guest);
            room.Occupant = guest;
            response += $"Room {roomNumber} is booked by {guestName} with keycard number {keycardNumber}.\r\n";
        }
        public Room GetRoomByNumber(int roomNumber, Hotel hotel)
        {
            return hotel.Floors.SelectMany(floor => floor).FirstOrDefault(room => room.Number == roomNumber) ?? new();
        }
        public void ListAvailableRooms(ref Hotel hotel, ref string response)
        {
            if (hotel == null)
            {
                response += "Error: Hotel not created.\r\n";
                return;
            }

            var availableRooms = hotel.Floors
                .SelectMany(floor => floor.Where(room => room.Occupant == null))
                .Select(room => room.Number);

            response += $"{string.Join(", ", availableRooms)}\r\n";
        }
        public void CheckoutRoom(int keyCardNumber, string name, ref Hotel hotel, ref List<Guest> guests)
        {
            var roomToCheckout = hotel.Floors
                                .SelectMany(floor => floor)
                                .FirstOrDefault(room => room?.Occupant?.KeycardNumber == keyCardNumber && room?.Occupant?.Name == name);

            if (roomToCheckout != null && roomToCheckout.Occupant != null)
            {
                roomToCheckout.Occupant = null;
            }

            var guestToCheckout = guests.FirstOrDefault(guest => guest.KeycardNumber == keyCardNumber && guest.Name == name);
            if (guestToCheckout != null)
            {
                guests.Remove(guestToCheckout);
            }
            return;
        }
        public int FindKeyCardNumberAvailable(int[] array)
        {
            if (array == null || array.Length == 0)
            {
                return 1;
            }

            int[] positiveArray = array.Where(num => num > 0).ToArray();

            if (positiveArray.Length == 0)
            {
                return 1;
            }

            Array.Sort(positiveArray);

            for (int i = 1; i <= positiveArray.Length + 1; i++)
            {
                if (!positiveArray.Contains(i))
                {
                    return i;
                }
            }

            return positiveArray.Length + 1;
        }

    }
}
