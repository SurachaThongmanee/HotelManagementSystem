using HotelManagementSystem.Models;

namespace HotelManagementSystem.Services.Interface
{
    public interface ICommandService
    {
        List<CommandModel> GetCommandsFromFileName(string fileName);
    }
}
