using HotelManagementSystem.Models;

namespace HotelManagementSystem.Extensions
{
    public static class HotelManagementExtension
    {
        public static Room GetRoomByNumber(int roomNumber, Hotel hotel)
        {
            return hotel.Floors.SelectMany(floor => floor).FirstOrDefault(room => room.Number == roomNumber) ?? new();
        }
        public static int FindKeyCardNumberAvailable(int[] array)
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
