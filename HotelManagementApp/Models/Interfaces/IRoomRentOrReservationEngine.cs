using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementApp.Models
{
    public interface IRoomRentOrReservationEngine
    {
        Transactions FindTransRentOrReservationForRoom();
        ResultProcess ProcessRentOrReservaiotnRegisterTrans(RoomRentOrReservation roomRentOrReservation,int idLoggingUser);
        void RentOrReservationnRegisterTrans();
        void CancellingRentOrReservation(int idLoggingUser, int idTransToCancel);
    }
}
