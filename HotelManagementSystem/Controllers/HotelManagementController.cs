using HotelManagementSystem.Models;
using HotelManagementSystem.Services.Interface;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet]
        public string Input()
        {
            const string filename = "D:\\ProgramingTest\\APEAK\\HotelManagementSystem_\\input.txt";
            List<CommandModel> commands = _commandService.GetCommandsFromFileName(filename);

            foreach (var command in commands)
            {
                switch (command.Name)
                {
                    case "create_hotel":
                        var floor = (int)command.Params[0];
                        var roomPerFloor = (int)command.Params[1];
                        var hotel = new { Floor = floor, RoomPerFloor = roomPerFloor };
                        Console.WriteLine($"Hotel created with {floor} floor(s), {roomPerFloor} room(s) per floor.");
                        break;
                    default:
                        break;
                }
            }
            return "";
        }
    }
}
