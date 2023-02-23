using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HotelManagementApp.Models
{
    public class GetHotelAdvancedData : IGetHotelAdvancedData
    {
        private HotelManagementDbContext hotelManagementDbEntities = new HotelManagementDbContext();

        public HotelManagementDbContext getHotelDb()
        {
            return hotelManagementDbEntities;
        }

        public IEnumerable<Transactions> GetAllHotelTransactions(int idHotel)
        {
            var hotelTrans = from i in hotelManagementDbEntities.Transactions
                             join j in hotelManagementDbEntities.HotelRooms
                             on i.Id_HotelRoom equals j.Id_HotelRoom
                             where j.Id_UserHotel == idHotel
                             orderby i.Id_Trans descending
                             select i;

            return hotelTrans;
        }

        public IEnumerable<Clients> GetAllHotelClients(int idHotel)
        {
            var hotelClients = from i in hotelManagementDbEntities.Clients
                               join j in hotelManagementDbEntities.Transactions
                               on i.Id_Client equals j.Id_Client
                               join k in hotelManagementDbEntities.HotelRooms
                               on j.Id_HotelRoom equals k.Id_HotelRoom
                               where k.Id_UserHotel == idHotel
                               orderby i.Id_Client descending
                               select i;

            hotelClients = hotelClients.Distinct();

            return hotelClients;
        }

        public double[] GetTransRevenueAndIssue(IEnumerable<Transactions> trans)
        {
            double[] RevenueAndIssue = new double[2] { 0, 0 };


            var revenueTransactions = from i in trans
                               join j in hotelManagementDbEntities.TypesTrans on i.Id_TypeTrans equals j.Id_TypeTrans
                               where j.IsRevenue == true
                               select new { i.Cost };

            var issueTransactions = from i in trans
                             join j in hotelManagementDbEntities.TypesTrans on i.Id_TypeTrans equals j.Id_TypeTrans
                             where j.IsRevenue == false
                             select new { i.Cost };

            foreach (var i in revenueTransactions)
            {
                if(i.Cost != null)
                {
                    RevenueAndIssue[0] += (double)i.Cost;
                }
            }

            foreach (var i in issueTransactions)
            {
                if (i.Cost != null)
                {
                    RevenueAndIssue[1] += (double)i.Cost;
                }
            }

            return RevenueAndIssue;
        }

        public IEnumerable<HotelRooms> ConditionOfRoomsAsOfDay(DateTime dateTimeToResearch, int idHotel)
        {
            var RoomsInUserHotel = hotelManagementDbEntities.HotelRooms.Where(a => a.Id_UserHotel == idHotel);

            List<HotelRooms> allRoomWithCondition = new List<HotelRooms>();

            foreach (HotelRooms i in RoomsInUserHotel)
            {
                var reserveTransForDay = hotelManagementDbEntities.Transactions.FirstOrDefault(a => a.Id_HotelRoom == i.Id_HotelRoom && a.Id_TypeTrans == (int)TypeOfTrans.Reservation
                    && dateTimeToResearch >= a.FromTime && dateTimeToResearch <= a.ToTime && a.IsActive == true);

                if (reserveTransForDay != null)
                {
                    allRoomWithCondition.Add(new HotelRooms() { Id_HotelRoom = i.Id_HotelRoom, NumberInUserHotel = i.NumberInUserHotel, Id_Condition = (int)ConditionOFHotelRooms.Reserved, NameOfRoom = i.NameOfRoom, Id_UserHotel = i.Id_UserHotel, CostOfRent = i.CostOfRent, OverallDescription = i.OverallDescription, OverallDescriptionAdditional = i.OverallDescriptionAdditional });
                }
                else
                {
                    var rentTransForDay = hotelManagementDbEntities.Transactions.
                    FirstOrDefault(a => a.Id_HotelRoom == i.Id_HotelRoom && a.Id_TypeTrans == (int)TypeOfTrans.Rent
                    && dateTimeToResearch >= a.FromTime && dateTimeToResearch <= a.ToTime && a.IsActive == true);

                    if (rentTransForDay != null)
                    {
                        allRoomWithCondition.Add(new HotelRooms() { Id_HotelRoom = i.Id_HotelRoom, NumberInUserHotel = i.NumberInUserHotel, Id_Condition = (int)ConditionOFHotelRooms.Rented, NameOfRoom = i.NameOfRoom, Id_UserHotel = i.Id_UserHotel, CostOfRent = i.CostOfRent, OverallDescription = i.OverallDescription, OverallDescriptionAdditional = i.OverallDescriptionAdditional });
                    }
                    else
                    {
                        if (i.Id_Condition == (int)ConditionOFHotelRooms.Unavailable)
                        {
                            allRoomWithCondition.Add(new HotelRooms() { Id_HotelRoom = i.Id_HotelRoom, NumberInUserHotel = i.NumberInUserHotel, Id_Condition = (int)ConditionOFHotelRooms.Unavailable, NameOfRoom = i.NameOfRoom, Id_UserHotel = i.Id_UserHotel, CostOfRent = i.CostOfRent, OverallDescription = i.OverallDescription, OverallDescriptionAdditional = i.OverallDescriptionAdditional });
                        }
                        else
                        {
                            allRoomWithCondition.Add(new HotelRooms() { Id_HotelRoom = i.Id_HotelRoom, NumberInUserHotel = i.NumberInUserHotel, Id_Condition = (int)ConditionOFHotelRooms.Available, NameOfRoom = i.NameOfRoom, Id_UserHotel = i.Id_UserHotel, CostOfRent = i.CostOfRent, OverallDescription = i.OverallDescription, OverallDescriptionAdditional = i.OverallDescriptionAdditional });
                        }
                    }
                }
            }

            return allRoomWithCondition;

        }

        public IEnumerable<Transactions> GetTransInSpecifiedPeriod(IEnumerable<Transactions> specifiedTrans, DateTime startTime,DateTime endTime)
        {
            var transInPeriodRaw = specifiedTrans.Where(a =>
                 ((a.FromTime >= startTime && a.FromTime <= endTime) || (a.ToTime >= startTime && a.ToTime <= endTime))).OrderByDescending(a => a.Id_Trans);

            return transInPeriodRaw;
        }

        public IEnumerable<TransUserViewModel> GetTransactionsUserView(IEnumerable<Transactions> transactions)
        {
            var transView = from i in transactions
                            join j in hotelManagementDbEntities.LoggingUsers
                             on i.Id_LoggingUser equals j.Id_LoggingUser
                            join k in hotelManagementDbEntities.TypesTrans
                            on i.Id_TypeTrans equals k.Id_TypeTrans
                            join l in hotelManagementDbEntities.HotelRooms
                            on i.Id_HotelRoom equals l.Id_HotelRoom
                            select (new TransUserViewModel
                            {
                                Id_Trans = i.Id_Trans,
                                Id_HotelRoom = i.Id_HotelRoom,
                                NumberInUserHotel = l.NumberInUserHotel,
                                Id_TypeTrans = i.Id_TypeTrans,
                                TypeTransDesc = k.Description,
                                Cost = i.Cost,
                                TransDesc = i.Description,
                                FromTime = i.FromTime,
                                ToTime = i.ToTime,
                                SurnameLoggingUser = j.Surname,
                                CreatedRecordDateTime = i.CreatedRecordDateTime,
                                IsActive = i.IsActive
                            });

            return transView;
        }

        public ClientData GetSelectClientDataFromJson(string jsonInputPersonalNumber,int idHotel)
        {
            String PersonalNumber = JsonConvert.DeserializeObject(jsonInputPersonalNumber).ToString();
            Clients existClient = GetAllHotelClients(idHotel).FirstOrDefault(a => a.PersonalNum == PersonalNumber);
            ClientData clientData = new ClientData();

            if (existClient != null)
            {
                clientData = new ClientData()
                {
                    Surname = existClient.Surname,
                    Name = existClient.Name,
                    City = existClient.City,
                    PostalCode = existClient.PostalCode,
                    Street = existClient.Street,
                    BuildingNumber = existClient.BuildingNumber,
                    FlatNumber = existClient.FlatNumber,
                    Telephone = existClient.Telephone,
                    Email = existClient.Email,
                    PersonalNum = existClient.PersonalNum
                };
            }
            return clientData;
        }


    }
}