using HotelManagementSystem.Models;
using HotelManagementSystem.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;

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
            const string filename = "D:\\ProgramingTest\\APEAK\\HotelManagementSystem_\\input.txt";
            List<CommandModel> commands = _commandService.GetCommandsFromFileName(filename);
            Hotel hotel = new Hotel(null, null);
            List<Guest> guests = new List<Guest>();
            string response = string.Empty;
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
                        _commandService.Book(roomNumber, guestName, guestAge, ref hotel, ref guests, ref response);
                        break;
                    case "list_available_rooms":
                        _commandService.ListAvailableRooms(ref hotel, ref response);
                        break;
                    case "checkout":
                        var keyCardNumber = (int)command.Params[0];
                        var name = (string)command.Params[1];
                        _commandService.CheckoutRoom(keyCardNumber, name ,ref hotel, ref guests);
                        break;
                    default:
                        break;
                }
            }
            return response;
        }
    }
}
