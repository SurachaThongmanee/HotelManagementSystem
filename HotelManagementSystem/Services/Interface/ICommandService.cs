using HotelManagementSystem.Models;

namespace HotelManagementSystem.Services.Interface
{
    public interface ICommandService
    {
        List<CommandModel> GetCommandsFromFileName(string fileName);
        void Book(int roomNumber, string guestName, int guestAge, ref Hotel hotel, ref List<Guest> guests, ref string response);
        void ListAvailableRooms(ref Hotel hotel, ref string response);
        void CheckoutRoom( int keyCardNumber, string name ,ref Hotel hotel, ref List<Guest> guests);
    }
}
