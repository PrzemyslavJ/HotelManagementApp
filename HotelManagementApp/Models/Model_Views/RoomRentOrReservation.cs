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
    }
}