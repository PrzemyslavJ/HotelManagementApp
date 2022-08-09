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
 
    }
}