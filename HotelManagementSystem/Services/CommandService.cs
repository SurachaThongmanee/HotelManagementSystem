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
    }
}
