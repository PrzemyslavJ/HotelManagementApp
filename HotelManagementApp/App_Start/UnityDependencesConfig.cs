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
    public class UnityDependencesConfig
    {
        public static void RegisterDependences()
        {
            var containerIoC = new UnityContainer();
            containerIoC.RegisterType<IRegisterServiceOrOperationEngine, RegisterServiceOrOperationEngine>();
            containerIoC.RegisterType<IRoomRentOrReservationEngine, RoomRentOrReservationEngine>();
            containerIoC.RegisterType<IGetHotelAdvancedData, GetHotelAdvancedData>();
            containerIoC.RegisterType<IRoomProperitiesOperations, RoomProperitiesOperations>();
            DependencyResolver.SetResolver(new UnityDependencyResolver(containerIoC));
        }
    }
}