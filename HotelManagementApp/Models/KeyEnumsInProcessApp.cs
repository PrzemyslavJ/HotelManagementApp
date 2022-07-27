using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotelManagementApp.Models
{
    public enum TypeOfTrans
    {
        Reservation = 1,
        Rent,
        ChangeToAvailable,
        ChangeToUnavailable,
        CancellingReservation,
        CancellinfOfent,
        CleaningTheRoom,
        RoomRenovation,
        AddingEquipment,
        RepairEquipment,
        AddingFeatureRoom
    }

    public enum ConditionOFHotelRooms
    {
        Unavailable = 1,
        Available,
        Reserved,
        Rented
    }
}