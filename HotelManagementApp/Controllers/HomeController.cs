using HotelManagementApp;
using HotelManagementApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HotelMenApp.Controllers
{
    public class HomeController : Controller
    {
        HotelManagementDbContext hotelManagementDbEntities;

        public HomeController()
        {
            hotelManagementDbEntities = new HotelManagementDbContext();

            HotelRooms.updatingRoomStatus(hotelManagementDbEntities);
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult LoginStart()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LoginStart(string Login, string Password)
        {
            var existUser = hotelManagementDbEntities.LoggingUsers.FirstOrDefault(a => a.Login == Login && a.Password == Password);

            if (existUser != null)
            {
                Session["Login"] = existUser.Login;
                Session["Id_LoggingUser"] = existUser.Id_LoggingUser;
                Session["Id_UserHotel"] = existUser.Id_UserHotel;
                Session["IsHotelOwner"] = existUser.IsHotelOwner;
                ViewBag.ResultCommunicate = "Pomyślne zalogowanie !";
            }
            else
            {
                ViewBag.ResultCommunicate = "Nieprawidłowy login lub hasło. Spróboj ponownie !";
            }

            return View();
        }

        [HttpPost]
        public ActionResult LoggingOut()
        {
            Session.Clear();
            ViewBag.ResultCommunicate = "Pomyślne wylogowano !";
            return View("LoginStart");
        }


        [HttpGet]
        public ActionResult Rooms(int? conditionOfRoom)
        {
            int IdHotel = (int)Session["Id_UserHotel"];
            var RoomsInUserHotel = hotelManagementDbEntities.HotelRooms.Where(a => a.Id_UserHotel == IdHotel);

            if (conditionOfRoom == null)
            {
                return View("Rooms", RoomsInUserHotel);
            }
            else
            {
                var roomsToShow = RoomsInUserHotel.Where(a => a.Id_Condition == conditionOfRoom);
                ConditionsOfHotelRooms conditionRoom = hotelManagementDbEntities.ConditionsOfHotelRooms.SingleOrDefault(a => a.Id_Condition == conditionOfRoom);
                ViewBag.ConditionRooms = conditionRoom.Description;
                return View("Rooms", roomsToShow);
            }
        }

        [HttpGet]
        public ActionResult RoomsForDay(DateTime dateTimeToReaearch, int? conditionOfRoom)
        {
            int IdHotel = (int)Session["Id_UserHotel"];

            if (conditionOfRoom != null)
            {
                var roomsOneConditionForDay = HotelRooms.roomsWithConditionForDay(dateTimeToReaearch, IdHotel).Where(a => a.Id_Condition == conditionOfRoom);
                ConditionsOfHotelRooms conditionRoom = hotelManagementDbEntities.ConditionsOfHotelRooms.Single(a => a.Id_Condition == conditionOfRoom);
                ViewBag.ConditionRooms = conditionRoom.Description;
                ViewBag.DayToShow = dateTimeToReaearch;
                return View("Rooms", roomsOneConditionForDay);
            }
            else
            {
                var roomsOneConditionForDay = HotelRooms.roomsWithConditionForDay(dateTimeToReaearch, IdHotel);
                ViewBag.DayToShow = dateTimeToReaearch;
                return View("Rooms", roomsOneConditionForDay);
            }
        }

        [HttpGet]
        public ActionResult RentOrReserveRoom(int? id_HotelRoom, int? typeOfTrans)
        {
            ViewBag.TypeOfTrans = typeOfTrans;
            ViewBag.Id_HotelRoom = id_HotelRoom;

            return View("RentOrReserveRoom");

        }

        [HttpPost]
        public ActionResult RentOrReserveRoom(RoomRentOrReservation roomRentOrReservation)
        {
            ResultProcess resultProcess;

            if (ModelState.IsValid)
            {
                resultProcess = roomRentOrReservation.ProcessRentOrReservaiotnRegisterTrans((int)Session["Id_LoggingUser"]);

                if (resultProcess.ResultOfProcess)
                {
                    ViewBag.Communicate = resultProcess.TextCommunicate;
                    return View("CommunicateView");
                }
                else
                {
                    ViewBag.ErrorCommunicate = resultProcess.TextCommunicate;
                    return RedirectToAction("RentOrReserveRoom", new { id_HotelRoom = roomRentOrReservation.Id_HotelRoom, typeOfTrans = roomRentOrReservation.Id_TypeOfTrans });
                }
            }
            else
            {
                ViewBag.ErrorCommunicate = "Wypełnij wszystkie dane prawidłowo !";
                return RedirectToAction("RentOrReserveRoom", new { id_HotelRoom = roomRentOrReservation.Id_HotelRoom, typeOfTrans = roomRentOrReservation.Id_TypeOfTrans });
            }
        }

        public ActionResult AllTransactions(int? typeOfTrans)
        {
            double[] RevenueAndIssue = new double[2];
            var hotelTransRaw = Transactions.GetAllHotelTransactions(hotelManagementDbEntities, (int)Session["Id_UserHotel"]);

            ViewBag.AllTypesTransToSelect = hotelManagementDbEntities.TypesTrans;

            // łączny przychód z transakcji jeszcze do zaimplementowania
            if (typeOfTrans == null)
            {
                RevenueAndIssue = Transactions.GetTransRevenueAndIssue(hotelManagementDbEntities, hotelTransRaw);
                ViewBag.Revenue = RevenueAndIssue[0];
                ViewBag.Issue = RevenueAndIssue[1];
                var hotelTransView = TransUserViewModel.GetTransactionsUserView(hotelManagementDbEntities, hotelTransRaw).ToList();
                return View("Transactions", hotelTransView);
            }
            else
            {
                var specificTransactionsRaw = hotelTransRaw.Where(a => a.Id_TypeTrans == typeOfTrans);
                RevenueAndIssue = Transactions.GetTransRevenueAndIssue(hotelManagementDbEntities, specificTransactionsRaw);
                ViewBag.Revenue = RevenueAndIssue[0];
                ViewBag.Issue = RevenueAndIssue[1];
                var specificTransactionsView = TransUserViewModel.GetTransactionsUserView(hotelManagementDbEntities, specificTransactionsRaw).ToList();
                TypesTrans typeTransDescription = hotelManagementDbEntities.TypesTrans.SingleOrDefault(a => a.Id_TypeTrans == typeOfTrans);
                ViewBag.TypeTransDescription = typeTransDescription.Description;
                return View("Transactions", specificTransactionsView);
            }
        }

        [HttpGet]
        public ActionResult TransactionsInPeriod(DateTime startTime, DateTime endTime, int? typeOfTrans)
        {
            double[] RevenueAndIssue = new double[2];
            var hotelTrans = Transactions.GetAllHotelTransactions(hotelManagementDbEntities, (int)Session["Id_UserHotel"]);
            ViewBag.AllTypesTransToSelect = hotelManagementDbEntities.TypesTrans;

            var transInPeriodRaw = hotelTrans.Where(a =>
                 ((a.FromTime >= startTime && a.FromTime <= endTime) || (a.ToTime >= startTime && a.ToTime <= endTime))).OrderByDescending(a => a.Id_Trans);

            if (typeOfTrans == null)
            {
                RevenueAndIssue = Transactions.GetTransRevenueAndIssue(hotelManagementDbEntities, transInPeriodRaw);
                ViewBag.Revenue = RevenueAndIssue[0];
                ViewBag.Issue = RevenueAndIssue[1];
                var transInPeriodView = TransUserViewModel.GetTransactionsUserView(hotelManagementDbEntities, transInPeriodRaw).ToList();
                ViewBag.TypeTransDescription = "wszystkie transakcje w okresie od: " + startTime + " do: " + endTime;
                return View("Transactions", transInPeriodView);
            }
            else
            {
                var transInPeriodWithConditionRaw = transInPeriodRaw.Where(a => a.Id_TypeTrans == typeOfTrans);
                RevenueAndIssue = Transactions.GetTransRevenueAndIssue(hotelManagementDbEntities, transInPeriodWithConditionRaw);
                ViewBag.Revenue = RevenueAndIssue[0];
                ViewBag.Issue = RevenueAndIssue[1];
                var transInPeriodWithConditionView = TransUserViewModel.GetTransactionsUserView(hotelManagementDbEntities, transInPeriodWithConditionRaw).ToList();
                TypesTrans typeTransDescription = hotelManagementDbEntities.TypesTrans.SingleOrDefault(a => a.Id_TypeTrans == typeOfTrans);
                ViewBag.TypeTransDescription = typeTransDescription.Description + " w okresie od: " + startTime + " do: " + endTime;
                return View("Transactions", transInPeriodWithConditionView);
            }
        }

        [HttpGet]
        public ActionResult RoomDetails(int? idRoom)
        {
            dynamic allRoomData = new ExpandoObject();
            var roomTransRaw = hotelManagementDbEntities.Transactions.Where(a => a.Id_HotelRoom == idRoom).OrderByDescending(a => a.Id_Trans);
        
            allRoomData.RoomDetails = hotelManagementDbEntities.HotelRooms.SingleOrDefault(a => a.Id_HotelRoom == idRoom);
            allRoomData.RoomTransactions = TransUserViewModel.GetTransactionsUserView(hotelManagementDbEntities, roomTransRaw).ToList();
            allRoomData.RoomFeatures = hotelManagementDbEntities.HotelRoomsFeatures.Where(a => a.Id_HotelRoom == idRoom);
       
            return View("RoomDetails", allRoomData);
        }

        [HttpPost]
        public ActionResult AddFeaturesToRoom(string Description,int? idRoomHotel)
        {
            HotelRoomsFeatures.AddFeaturesRoom(Description,(int)idRoomHotel);
            return RedirectToAction("RoomDetails", new { idRoom = idRoomHotel });
        }


        [HttpGet]
        public ActionResult RegisterService(int? idRoom)
        {
            var OperationsToSelect = hotelManagementDbEntities.TypesTrans.Where(a => a.Id_TypeTrans > (int)TypeOfTrans.CancellinfOfent);
            ViewBag.Id_HotelRoom = idRoom;
            ViewBag.OperationsToList = new SelectList(OperationsToSelect, "Id_TypeTrans", "Description");
            return View("RegisterService");
        }
        [HttpPost]
        public ActionResult RegisterService(RegisterServiceOrOperation registerServiceOrOperation)
        {
            ResultProcess resultProcess;

            if (ModelState.IsValid)
            {
                resultProcess = registerServiceOrOperation.ProcessRegisterOperationInTransations((int)Session["Id_LoggingUser"]);

                if (resultProcess.ResultOfProcess)
                {
                    ViewBag.Communicate = resultProcess.TextCommunicate;
                    return View("CommunicateView");
                }
                else
                {
                    ViewBag.ErrorCommunicate = resultProcess.TextCommunicate;
                    return RedirectToAction("RegisterService", new { idRoom = registerServiceOrOperation.Id_HotelRoom });
                }
            }
            else
            {
                ViewBag.ErrorCommunicate = "Poprawnie uzupełnij wszystkie dane !";
                return RedirectToAction("RegisterService", new { idRoom = registerServiceOrOperation.Id_HotelRoom });
            }
        }


        [HttpGet]
        public ActionResult ChangeRoomAvailabilityConfirm(int idRoom, bool statusChangeUp = false)
        {
            if (statusChangeUp)
            {
                ViewBag.ConfirmCommunicate = "Operacja umożliwi wykonywanie transakcji związanych z pokojem. Czy chcesz kontynuować ?";
            }
            else
            {
                ViewBag.ConfirmCommunicate = "Czy napewno chcesz zmienić status pokoju na niedostępny ? Trnasakcje na pokoju nie będą możliwe. Czy chcesz kontynuować ?";
            }

            ViewBag.ControllerMethod = "ChangeRoomAvailabilityConfirm";
            ViewBag.IdRoom = idRoom;
            ViewBag.StatusChangeUp = statusChangeUp;
            return View("ConfirmationView");
        }


        [HttpPost]
        public ActionResult ChangeRoomAvailability(int idRoom, bool statusChangeUp)
        {
            Transactions.RoomStatusChangeUpOrDown((int)Session["Id_LoggingUser"], idRoom, statusChangeUp);
            ViewBag.Communicate = "Operacja została zarejestrowana pomyślnie !";
            return View("CommunicateView");
        }

        [HttpGet]
        public ActionResult Clients()
        {
            var allHotelClient = ClientData.GetAllHotelClients((int)Session["Id_UserHotel"]);

            return View("Clients", allHotelClient);
        }

        [HttpGet]
        public ActionResult ClientDetails(int? IdClient)
        {
            dynamic allClientData = new ExpandoObject();

            allClientData.ClientDetails = hotelManagementDbEntities.Clients.SingleOrDefault(a => a.Id_Client == IdClient);
            allClientData.ClientTransactions = hotelManagementDbEntities.Transactions.Where(a => a.Id_Client == IdClient).OrderByDescending(a => a.Id_Trans);

            return View("ClientDetails", allClientData);
        }

        [HttpGet]
        public ActionResult CancellingRentOrReservationConfirm(int? IdTrans, int? IdRoom, DateTime? InDay)
        {
            Transactions transToCancel;

            if (IdTrans != null)
            {
                transToCancel = hotelManagementDbEntities.Transactions.FirstOrDefault(a => a.Id_Trans == IdTrans);
            }
            else
            {
                transToCancel = hotelManagementDbEntities.Transactions.FirstOrDefault(a => a.Id_HotelRoom == IdRoom && InDay >= a.FromTime && InDay <= a.ToTime);
            }

            HotelRooms hotelRoomToResearch = hotelManagementDbEntities.HotelRooms.FirstOrDefault(a => a.Id_HotelRoom == transToCancel.Id_HotelRoom);

            ViewBag.ControllerMethod = "CancellingRentOrReservationConfirm";
            ViewBag.IdTrans = transToCancel.Id_Trans;
            ViewBag.TypeTrans = transToCancel.Id_TypeTrans == (int)TypeOfTrans.Reservation ? "Rezerwacja" : "Wynajęcie";
            ViewBag.FromTime = transToCancel.FromTime;
            ViewBag.ToTime = transToCancel.ToTime;
            ViewBag.RoomName = hotelRoomToResearch.NameOfRoom;
            ViewBag.ConfirmCommunicate = "Czy na pewno chcesz anulować poniższą transakcję ?";
            return View("ConfirmationView");
        }

        [HttpPost]
        public ActionResult CancellingRentOrReservation(int IdTrans)
        {
            Transactions.CancellingRentOrReservation((int)Session["Id_LoggingUser"], IdTrans);
            ViewBag.Communicate = "Pomyślnie anulowano transakcję !";
            return View("CommunicateView");
        }

        [HttpGet]
        public ActionResult HotelLoggingUsers()
        {
            int IdHotel = (int)Session["Id_UserHotel"];
            var users = hotelManagementDbEntities.LoggingUsers.Where(a => a.Id_UserHotel == IdHotel);
            ViewBag.HotelName = hotelManagementDbEntities.Hotels.FirstOrDefault(a => a.Id_UserHotel == IdHotel).HotelName;
            return View("HotelLoggingUsers", users);
        }

        [HttpGet]
        public JsonResult GetSelectClientData(string jsonInputPersonalNumber)
        {
            String PersonalNumber = JsonConvert.DeserializeObject(jsonInputPersonalNumber).ToString();
            Clients existClient = hotelManagementDbEntities.Clients.FirstOrDefault(a => a.PersonalNum == PersonalNumber);
            ClientData clientData = new ClientData();

            if (existClient != null)
            {
                clientData = new ClientData()
                {
                    Surname = existClient.Surname,
                    Name = existClient.Name,
                    City = existClient.City,
                    PostalCode = existClient.PostalCode,
                    Street = existClient.Street,
                    BuildingNumber = existClient.BuildingNumber,
                    FlatNumber = existClient.FlatNumber,
                    Telephone = existClient.Telephone,
                    Email = existClient.Email,
                    PersonalNum = existClient.PersonalNum
                };
            }

            return Json(clientData, JsonRequestBehavior.AllowGet);
            
            //return Json(JsonConvert.SerializeObject(Newtonsoft.Json.JsonConvert.SerializeObject(existClient, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore })), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult RoomCreate()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RoomCreate(RoomCreateModel roomCreateModel)
        {
            if(ModelState.IsValid)
            {
                RoomCreateModel.CreateNewRoom(roomCreateModel, (int)Session["Id_UserHotel"]);
                return RedirectToAction("Rooms");
            }
            else
            {
                ViewBag.ErrorCommunicate = "Poprawnie uzupełnij wszystkie dane !";
                return View("RoomCreate");
            }
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}