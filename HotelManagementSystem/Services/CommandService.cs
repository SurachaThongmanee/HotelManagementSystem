using HotelManagementSystem.Extensions;
using HotelManagementSystem.Models;
using HotelManagementSystem.Services.Interface;

namespace HotelManagementSystem.Services
{
    public class CommandService : ICommandService
    {
        public List<CommandModel> GetCommandsFromFileName(string fileName, ref string response)
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
                response += $"An error occurred while reading the file: {e.Message}";
                return new List<CommandModel>();
            }
        }
        public void BookRoom(int roomNumber, string guestName, int guestAge, ref Hotel hotel, ref List<Guest> guests, ref string response)
        {
            try
            {
                if (hotel == null)
                {
                    response += "Error hotel not created.\r\n";
                    return;
                }

                var room = HotelManagementExtension.GetRoomByNumber(roomNumber, hotel);
                if (room == null)
                {
                    response += $"Error Room {roomNumber} does not exist.\r\n";
                    return;
                }

                if (room.Occupant is not null)
                {
                    response += $"Cannot book room {roomNumber} for {guestName}, The room is currently booked by {room.Occupant.Name}.\r\n";
                    return;
                }
                var keyCardNumberList = guests.Select(keyCardNumber => keyCardNumber.KeycardNumber).ToArray();
                var keycardNumber = HotelManagementExtension.FindKeyCardNumberAvailable(keyCardNumberList);
                var guest = new Guest { KeycardNumber = keycardNumber, Name = guestName, Age = guestAge };
                guests.Add(guest);
                room.Occupant = guest;
                response += $"Room {roomNumber} is booked by {guestName} with keycard number {keycardNumber}.\r\n";
            }
            catch (Exception e)
            {
                response += $"Error: can not booked by room {e.Message}\r\n";
            }

        }
        public void ListAvailableRooms(ref Hotel hotel, ref string response)
        {
            try
            {
                if (hotel == null)
                {
                    response += "Error: Hotel not created.\r\n";
                    return;
                }
                var availableRooms = hotel.Floors
                    .SelectMany(floor => floor.Where(room => room.Occupant == null))
                    .Select(room => room.Number);
                if (availableRooms is not null)
                {
                    response += $"{string.Join(", ", availableRooms)}\r\n";
                }
                else
                {
                    response += $"The room is not available.\r\n";
                }

            }
            catch (Exception e)
            {
                response += $"Error: can not get list available room {e.Message}\r\n";
            }
        }
        public void CheckoutRoom(int keyCardNumber, string name, ref Hotel hotel, ref List<Guest> guests, ref string response)
        {
            try
            {
                var guestToCheckout = guests.FirstOrDefault(guest => guest.KeycardNumber == keyCardNumber);
                if (guestToCheckout is not null)
                {
                    if (guestToCheckout.Name == name)
                    {
                        guests.Remove(guestToCheckout);
                    }
                }
                var roomToCheckout = hotel.Floors
                                    .SelectMany(floor => floor)
                                    .Where(room => room?.Occupant?.KeycardNumber == keyCardNumber);
                if (roomToCheckout is not null)
                {
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
                }
                else
                {
                    response += $"Can not checkout room.\r\n";
                }

            }
            catch (Exception e)
            {
                response += $"Error: can not checkout room {e.Message}\r\n";
            }
        }
        public void GetListGuest(List<Guest> guests, ref string response)
        {
            try
            {
                var guestNames = guests.Select(name => name.Name).Distinct().ToList();
                if (guestNames.Count > 0)
                {
                    response += string.Join(", ", guestNames) + "\r\n";
                }
                else
                {
                    response += "Guest nothing\r\n";
                }
            }
            catch (Exception e)
            {
                response += $"Error: can not get guest name list {e.Message}\r\n";
            }
        }
        public void GetListGuestInRoomByRoomNumber(Hotel hotel, int roomNumber, ref string response)
        {
            try
            {
                var roomToFind = hotel.Floors
                                            .SelectMany(floor => floor)
                                            .FirstOrDefault(room => room.Number == roomNumber);

                if (roomToFind is not null)
                {
                    var occupantName = roomToFind.Occupant?.Name;
                    response += $"{occupantName}\r\n";
                }
                else
                {
                    response += $"Guest nothing\r\n";
                }
            }
            catch (Exception e)
            {
                response += $"Error: can not get guest list room by room number {e.Message}\r\n";
            }
        }
        public void GetListGuestByAge(string ageOperator, int ageThreshold, List<Guest> guests, ref string response)
        {
            try
            {
                if (string.IsNullOrEmpty(ageOperator))
                {
                    response += $"Error: Unable to get guest list by age operator.\r\n";
                    return;
                }

                Func<Guest, bool> condition;
                switch (ageOperator)
                {
                    case ">":
                        condition = guest => guest.Age > ageThreshold;
                        break;
                    case "<":
                        condition = guest => guest.Age < ageThreshold;
                        break;
                    case "=":
                        condition = guest => guest.Age == ageThreshold;
                        break;
                    case ">=":
                        condition = guest => guest.Age >= ageThreshold;
                        break;
                    case "<=":
                        condition = guest => guest.Age <= ageThreshold;
                        break;
                    default:
                        response += $"Error: Invalid age operator '{ageOperator}'.\r\n";
                        return;
                }
                var guestNames = guests.Where(condition).Select(guest => guest.Name).Distinct().ToList();
                if (guestNames is not null)
                {
                    response += string.Join(", ", guestNames) + "\r\n";
                }
                else
                {
                    response += $"Error: Unable to get guest list by age operator\r\n";
                }
            }
            catch (Exception e)
            {
                response += $"Error: Unable to get guest list by age. {e.Message}\r\n";
            }

        }
        public void GetListGuestByFloor(int floorNumber, Hotel hotel, ref string response)
        {
            try
            {
                if (floorNumber <= 0 || floorNumber > hotel.Floors.Count)
                {
                    response += $"Error: Invalid floor number ({floorNumber}).\r\n";
                    return;
                }

                var floorSelect = hotel.Floors[floorNumber - 1];

                if (floorSelect == null)
                {
                    response += $"Error: Unable to get guest list from floor {floorNumber}.\r\n";
                    return;
                }

                var guestNames = floorSelect
                    .Where(room => room.Occupant is not null)
                    .Select(room => room.Occupant?.Name)
                    .Distinct()
                    .ToList();

                if (guestNames.Any())
                {
                    response += string.Join(", ", guestNames) + "\r\n";
                }
                else
                {
                    response += $"Guest list is empty for floor {floorNumber}.\r\n";
                }
            }
            catch (Exception e)
            {
                response += $"Error: Unable to get guest list from floor {floorNumber}. {e.Message}\r\n";
            }
        }
        public void CheckoutGuestsByFloor(int floorNumber, ref List<Guest> guests, ref Hotel hotel, ref string response)
        {
            try
            {
                if (floorNumber <= 0 || floorNumber > hotel.Floors.Count)
                {
                    response += $"Error: Invalid floor number ({floorNumber}).\r\n";
                    return;
                }
                var roomsOnFloor = hotel.Floors[floorNumber - 1];

                if (roomsOnFloor == null)
                {
                    response += $"Error: Unable to checkout guest from floor {floorNumber}.\r\n";
                    return;
                }

                var checkedOutRooms = roomsOnFloor
                    .Where(room => room.Occupant is not null)
                    .ToList();

                if (checkedOutRooms.Count > 0)
                {
                    var roomNumbers = checkedOutRooms.Select(room => room.Number);
                    foreach (var room in checkedOutRooms)
                    {
                        var guestToCheckout = guests.FirstOrDefault(guest => guest.KeycardNumber == room?.Occupant?.KeycardNumber && guest.Name == room.Occupant.Name);
                        if (guestToCheckout != null)
                        {
                            guests.Remove(guestToCheckout);
                        }
                        room.Occupant = null;
                    }
                    response += $"Room {string.Join(", ", roomNumbers)} is checked out\r\n";
                }
                else
                {
                    response += $"Error: No guest to checkout on floor {floorNumber}.\r\n";
                }
            }
            catch (Exception e)
            {
                response += $"Error: Unable to checkout guest by floor {e.Message}\r\n";
            }
        }
        public void BookRoomByFloor(int bookFloorNumber, string bookGuestName, int bookGuestAge, ref Hotel hotel, ref List<Guest> guests, ref string response)
        {
            try
            {
                if (hotel == null)
                {
                    response += "Error: Hotel not created.\r\n";
                    return;
                }

                if (bookFloorNumber <= 0 || bookFloorNumber > hotel.Floors.Count)
                {
                    response += $"Error: Invalid floor number ({bookFloorNumber}).\r\n";
                    return;
                }

                var floorToBook = hotel.Floors[bookFloorNumber - 1];

                if (floorToBook == null)
                {
                    response += $"Error: Unable to book floor {bookFloorNumber}.\r\n";
                    return;
                }

                if (floorToBook.Any(room => room.Occupant is not null))
                {
                    response += $"Cannot book floor {bookFloorNumber} for {bookGuestName}\r\n";
                    return;
                }

                foreach (var room in floorToBook)
                {
                    var keyCardNumber = HotelManagementExtension.FindKeyCardNumberAvailable(guests.Select(guest => guest.KeycardNumber).ToArray());

                    room.Occupant = new Guest
                    {
                        KeycardNumber = keyCardNumber,
                        Name = bookGuestName,
                        Age = bookGuestAge
                    };

                    guests.Add(room.Occupant);
                }

                var bookedRooms = string.Join(", ", floorToBook.Select(room => room.Number));
                var bookedKeycardNumbers = string.Join(", ", floorToBook.Select(room => room?.Occupant?.KeycardNumber));

                response += $"Rooms {bookedRooms} are booked with keycard numbers {bookedKeycardNumbers}\r\n";
            }
            catch (Exception e)
            {
                response += $"Error: Unable to book floor {e.Message}\r\n";
            }
        }
    }
}

