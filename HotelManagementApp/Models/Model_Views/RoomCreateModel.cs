using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HotelManagementApp.Models
{
    public class RoomCreateModel
    {
        public Nullable<int> NumberInUserHotel { get; set; }

        [Required]
        [Display(Name = "Nazwa pokoju")]
        public string NameOfRoom { get; set; }

        [Display(Name = "Kwota")]
        [Range(0, 1000000, ErrorMessage = "Kwota nie może być mniejsza od 0")]
        public double CostOfRent { get; set; }
        public string OverallDescription { get; set; }
        public string OverallDescriptionAdditional { get; set; }
        public Nullable<double> RoomSurface { get; set; }
        public Nullable<int> Floor { get; set; }
        public Nullable<bool> WithBadroom { get; set; }
        public Nullable<int> QtyOfPersonsInRoom { get; set; }

    }
}