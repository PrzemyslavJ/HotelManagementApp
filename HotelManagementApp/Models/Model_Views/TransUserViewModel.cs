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

        public int? Id_HotelRoom { get; set; }

        public int? NumberInUserHotel { get; set; }

        public string TypeTransDesc { get; set; }

        public double? Cost { get; set; }

        public string TransDesc { get; set; }

        public Nullable<System.DateTime> FromTime { get; set; }

        public Nullable<System.DateTime> ToTime { get; set; }

        public string SurnameLoggingUser { get; set; }

        public Nullable<System.DateTime> CreatedRecordDateTime { get; set; }

        public bool? IsActive { get; set; }
    }
}