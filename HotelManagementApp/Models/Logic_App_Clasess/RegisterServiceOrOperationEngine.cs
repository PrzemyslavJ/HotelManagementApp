using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotelManagementApp.Models
{
    public class RegisterServiceOrOperationEngine : IRegisterServiceOrOperationEngine
    {
        private HotelManagementDbContext hotelManagementDbEntities = new HotelManagementDbContext();
        private RegisterServiceOrOperation registerServiceOrOperation;
        private int idLoggingUser;

        public ResultProcess ProcessRegisterOperationInTransations(RegisterServiceOrOperation registerServiceOrOperation,int idLoggingUser)
        {
            this.registerServiceOrOperation = registerServiceOrOperation;
            this.idLoggingUser = idLoggingUser;

            ResultProcess resultProcess = new ResultProcess();

            string communicate;
            bool resultOfExecute;

            if (registerServiceOrOperation.StartTime < DateTime.Now || registerServiceOrOperation.EndTime < DateTime.Now)
            {
                communicate = "Nie można zarejestrować usługi/operacji z datą przeszłą!";
                resultOfExecute = false;
            }
            else if (registerServiceOrOperation.EndTime < registerServiceOrOperation.StartTime)
            {
                communicate = "Data końcowa nie może być mniejsza niż data początkowa!";
                resultOfExecute = false;
            }
            else
            {
                try
                {
                    this.RegisterOperationInTransations();
                    communicate = "Operacja została pomyślnie wykonana !";
                    resultOfExecute = true;
                }
                catch
                {
                    communicate = "coś poszło nie tak. Transakcja nie została pomyślnie przetworzona, spróbój ponownie !";
                    resultOfExecute = false;
                }
            }
            resultProcess.TextCommunicate = communicate;
            resultProcess.ResultOfProcess = resultOfExecute;

            return resultProcess;
        }
        public void RegisterOperationInTransations()
        {
            Transactions newTransaction = new Transactions()
            {
                Id_LoggingUser = idLoggingUser,
                Id_TypeTrans = registerServiceOrOperation.Id_TypeTrans,
                Id_HotelRoom = registerServiceOrOperation.Id_HotelRoom,
                Cost = registerServiceOrOperation.Cost,
                FromTime = registerServiceOrOperation.StartTime,
                ToTime = registerServiceOrOperation.EndTime,
                Description = registerServiceOrOperation.Comment,
                CreatedRecordDateTime = DateTime.Now
            };

            hotelManagementDbEntities.Transactions.Add(newTransaction);
            hotelManagementDbEntities.SaveChanges();
        }

    }
}