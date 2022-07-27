using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HotelManagementApp.Models
{
    public class RoomRentOrReservation : Clients
    {
        public int Id_HotelRoom { get; set; }

        public int Id_loggingUSer { get; set; }

        [Display(Name = "Kwota")]
        [Range(0, 10000, ErrorMessage = "Kwota nie może być mniejsza od 0")]
        public int CostOfOperation { get; set; }

        [Required(ErrorMessage = "Proszę podać datę początkową! ")]

        public DateTime FromDateTime { get; set; }

        [Required(ErrorMessage = "Proszę podać datę końcową! ")]
        public DateTime ToDateTime { get; set; }

        public string Comment { get; set; }

        public int Id_TypeOfTrans { get; set; }

        public bool OverWriteClientData { get; set; }

        // powinna być lista klientów przyporządkowanych do operacji rezerwacji lub wynajecia pokoju
        //public List<Clients> clientsInReservation { get; set; }
        // narazie robie do pojedyńczego klienta poprzez dziedziczenie pol z klasy Clients

        public Transactions FindTransRentOrReservationForRoom()
        {
            HotelManagementDbContext hotelManagementDbEntities = new HotelManagementDbContext();
            Transactions existTrans;

            existTrans = hotelManagementDbEntities.Transactions.
            FirstOrDefault(a => a.Id_HotelRoom == this.Id_HotelRoom && (a.Id_TypeTrans == (int)TypeOfTrans.Rent || a.Id_TypeTrans == (int)TypeOfTrans.Reservation)
            && ((this.FromDateTime >= a.FromTime && this.FromDateTime <= a.ToTime) || (this.ToDateTime >= a.FromTime && this.ToDateTime <= a.ToTime)) && a.IsActive == true);

            return existTrans;
        }

        public ResultProcess ProcessRentOrReservaiotnRegisterTrans(int IdLoggingUser)
        {
            ResultProcess resultProcess = new ResultProcess();

            string communicate;
            bool ResultOfExecute;

            if (this.FromDateTime < DateTime.Now || this.ToDateTime < DateTime.Now)
            {
                communicate = "Nie można wykonać rezerwacji/ wynajęcia z datą przeszłą!";
                ResultOfExecute = false;
            }
            else if (this.ToDateTime <= this.FromDateTime)
            {
                communicate = "Data końcowa nie może być mniejsza niż data początkowa!";
                ResultOfExecute = false;
            }
            else if (this.Id_TypeOfTrans == (int)TypeOfTrans.Rent && this.CostOfOperation == 0)
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
                        this.RentOrReservaiotnRegisterTrans(IdLoggingUser);
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

        public void RentOrReservaiotnRegisterTrans(int IdLoggingUser)
        {
            HotelManagementDbContext hotelManagementDbEntities = new HotelManagementDbContext();
            Transactions newTransaction = new Transactions();
            int? idClient = 0;

            Clients existClient = hotelManagementDbEntities.Clients.FirstOrDefault(a => a.PersonalNum == this.PersonalNum);

            if(existClient != null)
            {
                idClient = existClient.Id_Client;
            }

            if (existClient == null || this.OverWriteClientData == true)
            {
                if (existClient == null)
                {
                    Clients newClient = new Clients();

                    newClient.Surname = this.Surname;
                    newClient.Name = this.Name;
                    newClient.PersonalNum = this.PersonalNum;
                    newClient.City = this.City;
                    newClient.PostalCode = this.PostalCode;
                    newClient.Street = this.Street;
                    newClient.BuildingNumber = this.BuildingNumber;
                    newClient.FlatNumber = this.FlatNumber;
                    newClient.Telephone = this.Telephone;
                    newClient.Email = this.Email;

                    hotelManagementDbEntities.Clients.Add(newClient);

                    idClient = newClient.Id_Client;
                }
                else
                {
                    existClient.Surname = this.Surname;
                    existClient.Name = this.Name;
                    existClient.PersonalNum = this.PersonalNum;
                    existClient.City = this.City;
                    existClient.PostalCode = this.PostalCode;
                    existClient.Street = this.Street;
                    existClient.BuildingNumber = this.BuildingNumber;
                    existClient.FlatNumber = this.FlatNumber;
                    existClient.Telephone = this.Telephone;
                    existClient.Email = this.Email;
                }
                hotelManagementDbEntities.SaveChanges();
            }

            newTransaction.Id_HotelRoom = this.Id_HotelRoom;
            newTransaction.Id_LoggingUser = IdLoggingUser; 
            newTransaction.Id_Client = (int)idClient;
            newTransaction.Id_TypeTrans = this.Id_TypeOfTrans;
            newTransaction.Description = this.Comment;
            newTransaction.Cost = this.CostOfOperation;
            newTransaction.FromTime = this.FromDateTime;
            newTransaction.ToTime = this.ToDateTime;

            newTransaction.CreatedRecordDateTime = DateTime.Now;
            newTransaction.IsActive = true;

            hotelManagementDbEntities.Transactions.Add(newTransaction);
            hotelManagementDbEntities.SaveChanges();
        }
    }
}