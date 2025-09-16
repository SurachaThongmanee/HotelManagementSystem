using HotelManagementSystem.Models;
using HotelManagementSystem.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagementSystem.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HotelManagementController : ControllerBase
    {
        private readonly ICommandService _commandService;
        public HotelManagementController(ICommandService commandService)
        {
            _commandService = commandService;
        }

        [HttpGet("Commands")]
        public string Commands()
        {
        //test
        //Test 2
            const string filename = "D:\\ProgramingTest\\APEAK\\HotelManagementSystem_\\input.txt";
            string response = string.Empty;
            List<CommandModel> commands = _commandService.GetCommandsFromFileName(filename, ref response);
            Hotel hotel = new Hotel(null, null);
            List<Guest> guests = new List<Guest>();
            foreach (var command in commands)
            {
                switch (command.Name)
                {
                    case "create_hotel":
                        var floor = (int)command.Params[0];
                        var roomPerFloor = (int)command.Params[1];
                        hotel = new Hotel(floor, roomPerFloor);
                        response = $"Hotel created with {floor} floor(s), {roomPerFloor} room(s) per floor.\r\n";
                        break;
                    case "book":
                        var roomNumber = (int)command.Params[0];
                        var guestName = (string)command.Params[1];
                        var guestAge = (int)command.Params[2];
                        _commandService.BookRoom(roomNumber, guestName, guestAge, ref hotel, ref guests, ref response);
                        break;
                    case "list_available_rooms":
                        _commandService.ListAvailableRooms(ref hotel, ref response);
                        break;
                    case "checkout":
                        var keyCardNumber = (int)command.Params[0];
                        var name = (string)command.Params[1];
                        _commandService.CheckoutRoom(keyCardNumber, name, ref hotel, ref guests, ref response);
                        break;
                    case "list_guest":
                        _commandService.GetListGuest(guests, ref response);
                        break;
                    case "get_guest_in_room":
                        var getGuestRoomNumber = (int)command.Params[0];
                        _commandService.GetListGuestInRoomByRoomNumber(hotel, getGuestRoomNumber, ref response);
                        break;
                    case "list_guest_by_age":
                        var ageOperator = command.Params[0].ToString() ?? string.Empty;
                        var ageThreshold = (int)command.Params[1];
                        _commandService.GetListGuestByAge(ageOperator, ageThreshold, guests, ref response);
                        break;
                    case "list_guest_by_floor":
                        var floorNumber = (int)command.Params[0];
                        _commandService.GetListGuestByFloor(floorNumber, hotel, ref response);
                        break;
                    case "checkout_guest_by_floor":
                        var checkoutFloorNumber = (int)command.Params[0];
                        _commandService.CheckoutGuestsByFloor(checkoutFloorNumber, ref guests, ref hotel, ref response);
                        break;
                    case "book_by_floor":
                        var bookFloorNumber = (int)command.Params[0];
                        var bookGuestName = (string)command.Params[1];
                        var bookGuestAge = (int)command.Params[2];
                        _commandService.BookRoomByFloor(bookFloorNumber, bookGuestName, bookGuestAge, ref hotel, ref guests, ref response);
                        break;
                    default:
                        break;
                }
            }
            return response;
        }
    }
}
