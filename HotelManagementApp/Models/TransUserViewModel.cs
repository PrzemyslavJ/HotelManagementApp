using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotelManagementApp.Models
{
    public class TransUserViewModel
    {
        public int Id_Trans { get; set; }

        public int Id_TypeTrans { get; set;}

        public string TypeTransDesc { get; set; }

        public double? Cost { get; set; }

        public string TransDesc { get; set; }

        public Nullable<System.DateTime> FromTime { get; set; }

        public Nullable<System.DateTime> ToTime { get; set; }

        public string SurnameLoggingUser { get; set; }

        public Nullable<System.DateTime> CreatedRecordDateTime { get; set; }

        public bool? IsActive { get; set; }

 
        public static IEnumerable<TransUserViewModel> GetTransactionsUserView(HotelManagementDbContext hotelManagementDbEntities, IEnumerable<Transactions> transactions)
        {
            var transview = from i in transactions
                            join j in hotelManagementDbEntities.LoggingUsers
                             on i.Id_LoggingUser equals j.Id_LoggingUser
                            join k in hotelManagementDbEntities.TypesTrans
                            on i.Id_TypeTrans equals k.Id_TypeTrans
                            select (new TransUserViewModel
                            {
                                Id_Trans = i.Id_Trans,
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
                            
            return transview;
        }




    }
}