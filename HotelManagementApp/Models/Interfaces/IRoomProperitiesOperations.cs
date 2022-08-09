using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementApp.Models
{
    public interface IRoomProperitiesOperations
    {
        void UpdatingRoomNowStatus();
        void RoomStatusChangeUpOrDown(int idLoggingUser, int idRoom, bool StatusChangeUp = false);
        void CreateNewRoom(RoomCreateModel roomCreateModel, int userHotel,int idLoggingUser);
        void AddFeaturesRoom(string FeturesDesc, int IdRoom);
    }
}
