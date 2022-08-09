using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotelManagementApp.Models
{
    public class RoomProperitiesOperations : IRoomProperitiesOperations
    {
        private HotelManagementDbContext hotelManagementDbEntities = new HotelManagementDbContext();

        public void UpdatingRoomNowStatus()
        {
            var roomsNowToChangeStatus = from i in hotelManagementDbEntities.HotelRooms
                                         join j in hotelManagementDbEntities.Transactions
                                         on i.Id_HotelRoom equals j.Id_HotelRoom
                                         where (j.Id_TypeTrans == (int)TypeOfTrans.Rent || j.Id_TypeTrans == (int)TypeOfTrans.Reservation)
                                         && DateTime.Now >= j.FromTime && DateTime.Now <= j.ToTime && j.IsActive == true
                                         select new { i.Id_HotelRoom, j.Id_TypeTrans };

            var roomsNowToLossOfStatus = from i in hotelManagementDbEntities.HotelRooms
                                         where (i.Id_Condition == (int)ConditionOFHotelRooms.Reserved || i.Id_Condition == (int)ConditionOFHotelRooms.Rented)
                                         select i;

            int counterOfLossStatus = 0;

            foreach (var k in roomsNowToChangeStatus)
            {
                HotelRooms roomToChangeStatus = hotelManagementDbEntities.HotelRooms.Single(a => a.Id_HotelRoom == k.Id_HotelRoom);

                if (k.Id_TypeTrans == (int)TypeOfTrans.Reservation)
                {
                    roomToChangeStatus.Id_Condition = (int)ConditionOFHotelRooms.Reserved;
                }
                else
                {
                    roomToChangeStatus.Id_Condition = (int)ConditionOFHotelRooms.Rented;
                }
            }

            foreach (HotelRooms l in roomsNowToLossOfStatus)
            {
                Transactions roomToLossStatus;

                if (l.Id_Condition == (int)ConditionOFHotelRooms.Reserved)
                {
                    roomToLossStatus = hotelManagementDbEntities.Transactions.
                    FirstOrDefault(a => a.Id_HotelRoom == l.Id_HotelRoom && a.Id_TypeTrans == (int)TypeOfTrans.Reservation
                    && DateTime.Now >= a.FromTime && DateTime.Now <= a.ToTime && a.IsActive == true);
                }
                else
                {
                    roomToLossStatus = hotelManagementDbEntities.Transactions.
                    FirstOrDefault(a => a.Id_HotelRoom == l.Id_HotelRoom && a.Id_TypeTrans == (int)TypeOfTrans.Rent
                    && DateTime.Now >= a.FromTime && DateTime.Now <= a.ToTime && a.IsActive == true);
                }

                if (roomToLossStatus == null)
                {
                    l.Id_Condition = (int)ConditionOFHotelRooms.Available;
                    counterOfLossStatus++;
                }
            }

            if (roomsNowToChangeStatus.Count() > 0 || counterOfLossStatus > 0)
            {
                hotelManagementDbEntities.SaveChanges();
            }
        }


        public void RoomStatusChangeUpOrDown(int idLoggingUser, int idRoom, bool StatusChangeUp = false)
        {
            HotelRooms hotelRooms = hotelManagementDbEntities.HotelRooms.SingleOrDefault(a => a.Id_HotelRoom == idRoom);
            Transactions newTransaction = new Transactions()
            {
                Id_LoggingUser = idLoggingUser,
                Id_HotelRoom = idRoom,
                FromTime = DateTime.Now,
                ToTime = DateTime.Now,
                CreatedRecordDateTime = DateTime.Now
            };

            if (StatusChangeUp)
            {
                hotelRooms.Id_Condition = (int)ConditionOFHotelRooms.Available;
                newTransaction.Id_TypeTrans = (int)TypeOfTrans.ChangeToAvailable;
            }
            else
            {
                hotelRooms.Id_Condition = (int)ConditionOFHotelRooms.Unavailable;
                newTransaction.Id_TypeTrans = (int)TypeOfTrans.ChangeToUnavailable;
            }
            hotelManagementDbEntities.Transactions.Add(newTransaction);
            hotelManagementDbEntities.SaveChanges();
        }

        public void CreateNewRoom(RoomCreateModel roomCreateModel, int userHotel,int idLoggingUser)
        {
            HotelRooms newHotelRoom = new HotelRooms()
            {
                Id_UserHotel = userHotel,
                NumberInUserHotel = roomCreateModel.NumberInUserHotel,
                NameOfRoom = roomCreateModel.NameOfRoom,
                Id_Condition = (int)ConditionOFHotelRooms.Available,
                CostOfRent = roomCreateModel.CostOfRent,
                OverallDescription = roomCreateModel.OverallDescription,
                OverallDescriptionAdditional = roomCreateModel.OverallDescriptionAdditional,
                RoomSurface = roomCreateModel.RoomSurface,
                Floor = roomCreateModel.Floor,
                WithBadroom = roomCreateModel.WithBadroom,
                QtyOfPersonsInRoom = roomCreateModel.QtyOfPersonsInRoom
            };
            hotelManagementDbEntities.HotelRooms.Add(newHotelRoom);

            Transactions newTransaction = new Transactions()
            {
                Id_LoggingUser = idLoggingUser,
                Id_HotelRoom = newHotelRoom.Id_HotelRoom,
                Id_TypeTrans = (int)TypeOfTrans.AddingNewRoom,
                FromTime = DateTime.Now,
                ToTime = DateTime.Now,
                CreatedRecordDateTime = DateTime.Now
            };
            hotelManagementDbEntities.Transactions.Add(newTransaction);

            hotelManagementDbEntities.SaveChanges();
        }

        public void AddFeaturesRoom(string feturesDesc, int idRoom)
        {
            HotelRoomsFeatures hotelRoomsFeatures = new HotelRoomsFeatures()
            {
                Id_HotelRoom = idRoom,
                Description = feturesDesc
            };
            hotelManagementDbEntities.HotelRoomsFeatures.Add(hotelRoomsFeatures);
            hotelManagementDbEntities.SaveChanges();
        }
    }
}