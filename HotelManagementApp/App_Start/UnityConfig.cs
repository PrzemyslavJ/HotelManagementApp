using HotelManagementApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Unity;
using Unity.Mvc5;

namespace HotelManagementApp.App_Start
{
    public class UnityConfig
    {
        public static void RegisterComponents()
        {
            
            var container = new UnityContainer();
            container.RegisterType<IRegisterServiceOrOperationEngine, RegisterServiceOrOperationEngine>();
            container.RegisterType<IRoomRentOrReservationEngine, RoomRentOrReservationEngine>();
            container.RegisterType<IGetHotelData, GetHotelData>();
            container.RegisterType<IRoomProperitiesOperations, RoomProperitiesOperations>();
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));

        }
    }
}