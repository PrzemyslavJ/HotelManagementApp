using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotelManagementApp.Models
{
    public class RegisterServiceOrOperation
    {
        public int Id_HotelRoom { get; set; }
        public int Id_TypeTrans { get; set; }

        public float? Cost { get; set; }

        public System.DateTime StartTime { get; set; }

        public System.DateTime EndTime { get; set; }

        public string Comment { get; set; }


        public ResultProcess ProcessRegisterOperationInTransations(int IdLoggingUser)
        {
            ResultProcess resultProcess = new ResultProcess();

            string communicate;
            bool ResultOfExecute;

            if (this.StartTime < DateTime.Now || this.EndTime < DateTime.Now)
            {
                communicate = "Nie można zarejestrować usługi/operacji z datą przeszłą!";
                ResultOfExecute = false;
            }
            else if (this.EndTime < this.StartTime)
            {
                communicate = "Data końcowa nie może być mniejsza niż data początkowa!";
                ResultOfExecute = false;
            }
            else
            {
                try
                {
                    this.RegisterOperationInTransations(IdLoggingUser);
                    communicate = "Operacja została pomyślnie wykonana !";
                    ResultOfExecute = true;
                }
                catch
                {
                    communicate = "coś poszło nie tak. Transakcja nie została pomyślnie przetworzona, spróbój ponownie !";
                    ResultOfExecute = false;
                }
            }
            resultProcess.TextCommunicate = communicate;
            resultProcess.ResultOfProcess = ResultOfExecute;

            return resultProcess;
        }
        public void RegisterOperationInTransations(int IdLogginUser)
        {
            HotelManagementDbContext hotelManagementDbEntities = new HotelManagementDbContext();
            Transactions newTransaction = new Transactions();

            newTransaction.Id_LoggingUser = IdLogginUser;
            newTransaction.Id_TypeTrans = this.Id_TypeTrans;
            newTransaction.Id_HotelRoom = this.Id_HotelRoom;
            newTransaction.Cost = this.Cost;
            newTransaction.FromTime = this.StartTime;
            newTransaction.ToTime = this.EndTime;
            newTransaction.Description = this.Comment;
            newTransaction.CreatedRecordDateTime = DateTime.Now;

            hotelManagementDbEntities.Transactions.Add(newTransaction);
            hotelManagementDbEntities.SaveChanges();
        }
    }
}