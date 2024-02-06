using HotelManagementSystem.Models;

namespace HotelManagementSystem.Services.Interface
{
    public interface ICommandService
    {
        List<CommandModel> GetCommandsFromFileName(string fileName, ref string response);
        bool CreateHotel(int floor, int roomPerFloor, ref Hotel hotel, ref string response);
        void BookRoom(int roomNumber, string guestName, int guestAge, ref Hotel hotel, ref List<Guest> guests, ref string response);
        void ListAvailableRooms(ref Hotel hotel, ref string response);
        void CheckoutRoom(int keyCardNumber, string name, ref Hotel hotel, ref List<Guest> guests, ref string response);
        void GetListGuest(List<Guest> guests, ref string response);
        void GetListGuestInRoomByRoomNumber(Hotel hotel, int roomNumber, ref string response);
        void GetListGuestByAge(string ageOperator, int ageThreshold, List<Guest> guests, ref string response);
        void GetListGuestByFloor(int floorNumber, Hotel hotel, ref string response);
        void CheckoutGuestsByFloor(int floorNumber, ref List<Guest> guests, ref Hotel hotel, ref string response);
        void BookRoomByFloor(int bookFloorNumber, string bookGuestName, int bookGuestAge, ref Hotel hotel, ref List<Guest> guests, ref string response);
    }
}
