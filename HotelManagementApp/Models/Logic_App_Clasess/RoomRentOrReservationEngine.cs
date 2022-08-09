using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotelManagementApp.Models
{
    public class RoomRentOrReservationEngine : IRoomRentOrReservationEngine
    {
        private HotelManagementDbContext hotelManagementDbEntities = new HotelManagementDbContext();

        private RoomRentOrReservation roomRentOrReservation;

        private int idLoggingUser;

        public Transactions FindTransRentOrReservationForRoom()
        {
            Transactions existTrans;

            existTrans = hotelManagementDbEntities.Transactions.
            FirstOrDefault(a => a.Id_HotelRoom == roomRentOrReservation.Id_HotelRoom && (a.Id_TypeTrans == (int)TypeOfTrans.Rent || a.Id_TypeTrans == (int)TypeOfTrans.Reservation)
            && ((roomRentOrReservation.FromDateTime >= a.FromTime && roomRentOrReservation.FromDateTime <= a.ToTime) || (roomRentOrReservation.ToDateTime >= a.FromTime && roomRentOrReservation.ToDateTime <= a.ToTime)) && a.IsActive == true);

            return existTrans;
        }

        public ResultProcess ProcessRentOrReservaiotnRegisterTrans(RoomRentOrReservation roomRentOrReservation, int idLoggingUser)
        {
            this.roomRentOrReservation = roomRentOrReservation;
            this.idLoggingUser = idLoggingUser;

            ResultProcess resultProcess = new ResultProcess();

            string communicate;
            bool ResultOfExecute;

            if (roomRentOrReservation.FromDateTime < DateTime.Now || roomRentOrReservation.ToDateTime < DateTime.Now)
            {
                communicate = "Nie można wykonać rezerwacji/ wynajęcia z datą przeszłą!";
                ResultOfExecute = false;
            }
            else if (roomRentOrReservation.ToDateTime <= roomRentOrReservation.FromDateTime)
            {
                communicate = "Data końcowa nie może być mniejsza niż data początkowa!";
                ResultOfExecute = false;
            }
            else if (roomRentOrReservation.Id_TypeOfTrans == (int)TypeOfTrans.Rent && roomRentOrReservation.CostOfOperation == 0)
            {
                communicate = "Dla wynajęcia zarejestrowana kwota musi być większa od 0 !";
                ResultOfExecute = false;
            }
            else
            {
                Transactions rentOrReservationTrans = this.FindTransRentOrReservationForRoom();

                if (rentOrReservationTrans == null)
                {
                    try
                    {
                        this.RentOrReservationnRegisterTrans();
                        communicate = "Operacja została pomyślnie wykonana !";
                        ResultOfExecute = true;
                    }
                    catch
                    {
                        communicate = "coś poszło nie tak. Transakcja nie została pomyślnie przetworzona, spróbój ponownie !";
                        ResultOfExecute = false;
                    }
                }
                else
                {
                    communicate = "W podanym okresie już istnieje transakcja rezerwacji lub wynajęcia dla tego pokoju !";
                    ResultOfExecute = false;
                }
            }

            resultProcess.TextCommunicate = communicate;
            resultProcess.ResultOfProcess = ResultOfExecute;

            return resultProcess;
        }

        public void RentOrReservationnRegisterTrans()
        {
            int? idClient = 0;

            Clients existClient = hotelManagementDbEntities.Clients.FirstOrDefault(a => a.PersonalNum == roomRentOrReservation.PersonalNum);

            if (existClient != null)
            {
                idClient = existClient.Id_Client;
            }

            if (existClient == null || roomRentOrReservation.OverWriteClientData == true)
            {
                if (existClient == null)
                {
                    Clients newClient = new Clients()
                    {
                        Surname = roomRentOrReservation.Surname,
                        Name = roomRentOrReservation.Name,
                        PersonalNum = roomRentOrReservation.PersonalNum,
                        City = roomRentOrReservation.City,
                        PostalCode = roomRentOrReservation.PostalCode,
                        Street = roomRentOrReservation.Street,
                        BuildingNumber = roomRentOrReservation.BuildingNumber,
                        FlatNumber = roomRentOrReservation.FlatNumber,
                        Telephone = roomRentOrReservation.Telephone,
                        Email = roomRentOrReservation.Email
                    };

                    hotelManagementDbEntities.Clients.Add(newClient);

                    idClient = newClient.Id_Client;
                }
                else
                {
                    existClient.Surname = roomRentOrReservation.Surname;
                    existClient.Name = roomRentOrReservation.Name;
                    existClient.PersonalNum = roomRentOrReservation.PersonalNum;
                    existClient.City = roomRentOrReservation.City;
                    existClient.PostalCode = roomRentOrReservation.PostalCode;
                    existClient.Street = roomRentOrReservation.Street;
                    existClient.BuildingNumber = roomRentOrReservation.BuildingNumber;
                    existClient.FlatNumber = roomRentOrReservation.FlatNumber;
                    existClient.Telephone = roomRentOrReservation.Telephone;
                    existClient.Email = roomRentOrReservation.Email;
                }
                hotelManagementDbEntities.SaveChanges();
            }

            Transactions newTransaction = new Transactions()
            {
                Id_HotelRoom = roomRentOrReservation.Id_HotelRoom,
                Id_LoggingUser = idLoggingUser,
                Id_Client = (int)idClient,
                Id_TypeTrans = roomRentOrReservation.Id_TypeOfTrans,
                Description = roomRentOrReservation.Comment,
                Cost = roomRentOrReservation.CostOfOperation,
                FromTime = roomRentOrReservation.FromDateTime,
                ToTime = roomRentOrReservation.ToDateTime,
                CreatedRecordDateTime = DateTime.Now,
                IsActive = true
            };

            hotelManagementDbEntities.Transactions.Add(newTransaction);
            hotelManagementDbEntities.SaveChanges();
        }

        public void CancellingRentOrReservation(int idLoggingUser, int idTransToCancel)
        {
            Transactions transToCancel = hotelManagementDbEntities.Transactions.FirstOrDefault(a => a.Id_Trans == idTransToCancel);

            transToCancel.IsActive = false;
            hotelManagementDbEntities.SaveChanges();

            Transactions newTransaction = new Transactions()
            {
                Id_HotelRoom = transToCancel.Id_HotelRoom,
                CreatedRecordDateTime = DateTime.Now,
                Id_thisRef = transToCancel.Id_Trans,
                Id_LoggingUser = idLoggingUser,
                Cost = 0,
                FromTime = transToCancel.FromTime,
                ToTime = transToCancel.ToTime
            };

            if (transToCancel.Id_TypeTrans == (int)TypeOfTrans.Reservation)
            {
                newTransaction.Id_TypeTrans = (int)TypeOfTrans.CancellingReservation;
                newTransaction.Description = "Anulowanie rezerwacji o numerze Id: " + newTransaction.Id_thisRef;
            }
            else if (transToCancel.Id_TypeTrans == (int)TypeOfTrans.Rent)
            {
                newTransaction.Id_TypeTrans = (int)TypeOfTrans.CancellinfOfent;
                newTransaction.Description = "Anulowanie wynajęcia o numerze Id: " + newTransaction.Id_thisRef;
            }
            hotelManagementDbEntities.Transactions.Add(newTransaction);
            hotelManagementDbEntities.SaveChanges();
        }
    }
}
