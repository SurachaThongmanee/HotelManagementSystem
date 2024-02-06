using HotelManagementSystem.Models;
using HotelManagementSystem.Services.Interface;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Xml.Linq;

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
        public void CheckoutRoom(int keyCardNumber, string name, ref Hotel hotel, ref List<Guest> guests, ref string response)
        {
            var guestToCheckout = guests.FirstOrDefault(guest => guest.KeycardNumber == keyCardNumber);
            if (guestToCheckout != null)
            {
                if (guestToCheckout.Name == name)
                {
                    guests.Remove(guestToCheckout);
                }
            }
            var roomToCheckout = hotel.Floors
                                .SelectMany(floor => floor)
                                .Where(room => room?.Occupant?.KeycardNumber == keyCardNumber);

            foreach (var room in roomToCheckout)
            {
                if (room?.Occupant?.Name == name)
                {
                    room.Occupant = null;
                    response += $"Room {room.Number} is checkout.\r\n";
                }
                else
                {
                    response += $"Only {room?.Occupant?.Name} can checkout with keycard number {room?.Occupant?.KeycardNumber}.\r\n";
                }
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
        public void GetListGuest(List<Guest> guests, ref string response)
        {
            var guestNames = guests.Select(name => name.Name).Distinct().ToList();
            response += string.Join(", ", guestNames) + "\r\n";
        }
        public void GetListGuestInRoomByRoomNumber(Hotel hotel, int roomNumber, ref string response)
        {
            var roomToFind = hotel.Floors
                            .SelectMany(floor => floor)
                            .FirstOrDefault(room => room.Number == roomNumber);

            if (roomToFind != null)
            {
                var occupantName = roomToFind.Occupant?.Name;
                response += $"{occupantName}\r\n";
            }
        }
        public void GetListGuestByAge(string ageOperator, int ageThreshold, List<Guest> guests, ref string response)
        {
            if (!string.IsNullOrEmpty(ageOperator))
            {
                if (ageOperator == ">")
                {
                    var guestNames = guests.Where(w => w.Age > ageThreshold).Select(name => name.Name).Distinct().ToList();
                    response += string.Join(", ", guestNames) + "\r\n";
                }
                else if (ageOperator == "<")
                {
                    var guestNames = guests.Where(w => w.Age < ageThreshold).Select(name => name.Name).Distinct().ToList();
                    response += string.Join(", ", guestNames) + "\r\n";
                }
                else if (ageOperator == "=")
                {
                    var guestNames = guests.Where(w => w.Age == ageThreshold).Select(name => name.Name).Distinct().ToList();
                    response += string.Join(", ", guestNames) + "\r\n";
                }
                else if (ageOperator == ">=")
                {
                    var guestNames = guests.Where(w => w.Age >= ageThreshold).Select(name => name.Name).Distinct().ToList();
                    response += string.Join(", ", guestNames) + "\r\n";
                }
                else if (ageOperator == "<=")
                {
                    var guestNames = guests.Where(w => w.Age <= ageThreshold).Select(name => name.Name).Distinct().ToList();
                    response += string.Join(", ", guestNames) + "\r\n";
                }
            }
            else
            {
                return;
            }
        }
        public void GetListGuestByFloor(int floorNumber, Hotel hotel, ref string response)
        {
            if (floorNumber > 0 && floorNumber <= hotel.Floors.Count)
            {
                var floorSelect = hotel.Floors[floorNumber - 1];
                var guestNames = floorSelect.Where(w => w.Occupant is not null).Select(s => s.Occupant?.Name).Distinct().ToList();
                response += string.Join(", ", guestNames) + "\r\n";
            }
            else
            {
                return;
            }

        }
        public void CheckoutGuestsByFloor(int floorNumber, ref List<Guest> guests, ref Hotel hotel, ref string response)
        {

            if (floorNumber > 0 && floorNumber <= hotel.Floors.Count)
            {
                var roomsOnFloor = hotel.Floors[floorNumber - 1];

                var checkedOutRooms = roomsOnFloor
                    .Where(room => room.Occupant is not null)
                    .ToList();

                var roomNumbers = checkedOutRooms.Select(room => room.Number).ToList();
                response += $"Room {string.Join(", ", roomNumbers)} is checked out\r\n";

                foreach (var room in checkedOutRooms)
                {
                    var guestToCheckout = guests.FirstOrDefault(guest => guest.KeycardNumber == room?.Occupant?.KeycardNumber && guest.Name == room.Occupant.Name);
                    if (guestToCheckout != null)
                    {
                        guests.Remove(guestToCheckout);
                    }

                    room.Occupant = null;
                }
            }
        }
        public void BookRoomByFloor(int bookFloorNumber, string bookGuestName, int bookGuestAge, ref Hotel hotel, ref List<Guest> guests, ref string response)
        {
            if (hotel == null)
            {
                response += "Error: Hotel not created.\r\n";
                return;
            }

            if (bookFloorNumber > 0 && bookFloorNumber <= hotel.Floors.Count)
            {
                var floorToBook = hotel.Floors[bookFloorNumber - 1];

                if (floorToBook.Any(room => room.Occupant != null))
                {
                    response += $"Cannot book floor {bookFloorNumber} for {bookGuestName}\r\n";
                    return;
                }

                foreach (var room in floorToBook)
                {
                    var keyCardNumber = FindKeyCardNumberAvailable(guests.Select(guest => guest.KeycardNumber).ToArray());

                    room.Occupant = new Guest
                    {
                        KeycardNumber = keyCardNumber,
                        Name = bookGuestName,
                        Age = bookGuestAge
                    };

                    guests.Add(room.Occupant);
                }

                response += $"Rooms {string.Join(", ", floorToBook.Select(room => room.Number))} are booked with keycard numbers {string.Join(", ", floorToBook.Select(room => room?.Occupant?.KeycardNumber))}\r\n";
            }
            else
            {
                response += $"Error: Invalid floor number ({bookFloorNumber}).\r\n";
            }
        }
    }
}

