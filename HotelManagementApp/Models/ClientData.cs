using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotelManagementApp.Models
{
    public class ClientData
    {
        public string Surname { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Street { get; set; }
        public string BuildingNumber { get; set; }
        public string FlatNumber { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public string PersonalNum { get; set; }

        public static IEnumerable<Clients> GetAllHotelClients(int IdHotel)
        {
            HotelManagementDbContext hotelManagementDbEntities = new HotelManagementDbContext();

            var hotelClients = from i in hotelManagementDbEntities.Clients
                               join j in hotelManagementDbEntities.Transactions
                               on i.Id_Client equals j.Id_Client
                               join k in hotelManagementDbEntities.HotelRooms
                               on j.Id_HotelRoom equals k.Id_HotelRoom
                               where k.Id_UserHotel == IdHotel
                               select i;
            //group i by i.Id_Client into newGroup
            //select newGroup;

            hotelClients = hotelClients.Distinct();

            return hotelClients;
            //return (IEnumerable<Clients>)hotelClients;
        }

    }
}