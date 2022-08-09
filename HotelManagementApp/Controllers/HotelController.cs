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
    public class HotelController : Controller
    {
        private IRegisterServiceOrOperationEngine registerServiceOrOperation;
        private IGetHotelData getHotelData;
        private IRoomRentOrReservationEngine roomRentOrReservation;
        private IRoomProperitiesOperations roomProperitiesOperations;

        public HotelController(IRegisterServiceOrOperationEngine registerServiceOrOperationEngine, IGetHotelData getHotelDatas, IRoomRentOrReservationEngine roomRentOrReservationEngine, IRoomProperitiesOperations RoomOnProperitiesOperations)
        {
            this.registerServiceOrOperation = registerServiceOrOperationEngine;
            this.getHotelData = getHotelDatas;
            this.roomRentOrReservation = roomRentOrReservationEngine;
            this.roomProperitiesOperations = RoomOnProperitiesOperations;

            roomProperitiesOperations.UpdatingRoomNowStatus();
        }

        [HttpGet]
        public ActionResult LoginStart()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LoginStart(string Login, string Password)
        {
            var existUser = getHotelData.getHotelDb().LoggingUsers.FirstOrDefault(a => a.Login == Login && a.Password == Password);

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
            int idHotel = (int)Session["Id_UserHotel"];
            var roomsInUserHotel = getHotelData.getHotelDb().HotelRooms.Where(a => a.Id_UserHotel == idHotel);

            if (conditionOfRoom == null)
            {
                return View("Rooms", roomsInUserHotel);
            }
            else
            {
                var roomsToShow = roomsInUserHotel.Where(a => a.Id_Condition == conditionOfRoom);
                ConditionsOfHotelRooms conditionRoom = getHotelData.getHotelDb().ConditionsOfHotelRooms.SingleOrDefault(a => a.Id_Condition == conditionOfRoom);
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
                var roomsOneConditionForDay = getHotelData.ConditionOfRoomsAsOfDay(dateTimeToReaearch, IdHotel).Where(a => a.Id_Condition == conditionOfRoom);
                ConditionsOfHotelRooms conditionRoom = getHotelData.getHotelDb().ConditionsOfHotelRooms.Single(a => a.Id_Condition == conditionOfRoom);
                ViewBag.ConditionRooms = conditionRoom.Description;
                ViewBag.DayToShow = dateTimeToReaearch;
                return View("Rooms", roomsOneConditionForDay);
            }
            else
            {
                var roomsOneConditionForDay = getHotelData.ConditionOfRoomsAsOfDay(dateTimeToReaearch, IdHotel);
                ViewBag.DayToShow = dateTimeToReaearch;
                return View("Rooms", roomsOneConditionForDay);
            }
        }

        [HttpGet]
        public ActionResult RentOrReserveRoom(int? id_HotelRoom, int? typeOfTrans,string communicate = "")
        {
            var roomToReserveOrRent = getHotelData.getHotelDb().HotelRooms.FirstOrDefault(a => a.Id_HotelRoom == id_HotelRoom);
            ViewBag.TypeOfTrans = typeOfTrans;
            ViewBag.RoomName = roomToReserveOrRent.NameOfRoom +" , numer pokoju "+ roomToReserveOrRent.NumberInUserHotel;
            if(communicate!="")
            {
                ViewBag.ErrorCommunicate = communicate;
            }
            return View("RentOrReserveRoom");

        }

        [HttpPost]
        public ActionResult RentOrReserveRoom(RoomRentOrReservation roomRentOrReservationData)
        {
            ResultProcess resultProcess;

            if(ModelState.IsValid)
            {
                resultProcess = roomRentOrReservation.ProcessRentOrReservaiotnRegisterTrans(roomRentOrReservationData, (int)Session["Id_LoggingUser"]);

                if (resultProcess.ResultOfProcess)
                {
                    ViewBag.Communicate = resultProcess.TextCommunicate;
                    return View("CommunicateView");
                }
                else
                {
                    return RedirectToAction("RentOrReserveRoom", new { id_HotelRoom = roomRentOrReservationData.Id_HotelRoom, typeOfTrans = roomRentOrReservationData.Id_TypeOfTrans, communicate = resultProcess.TextCommunicate });
                }
            }
            else
            {
                return RedirectToAction("RentOrReserveRoom", new { id_HotelRoom = roomRentOrReservationData.Id_HotelRoom, typeOfTrans = roomRentOrReservationData.Id_TypeOfTrans,communicate = "Wypełnij wszystkie dane prawidłowo !" });
            }
        }

        public ActionResult AllTransactions(int? typeOfTrans)
        {
            double[] RevenueAndIssue = new double[2];
            var hotelTransRaw = getHotelData.GetAllHotelTransactions((int)Session["Id_UserHotel"]);

            ViewBag.AllTypesTransToSelect = getHotelData.getHotelDb().TypesTrans;

            if (typeOfTrans == null)
            {
                RevenueAndIssue = getHotelData.GetTransRevenueAndIssue(hotelTransRaw);
                ViewBag.Revenue = RevenueAndIssue[0];
                ViewBag.Issue = RevenueAndIssue[1];
                var hotelTransView = getHotelData.GetTransactionsUserView(hotelTransRaw).ToList();
                ViewBag.TypeTransDescription = "Wszystkie transakcje dla hotelu";
                return View("Transactions", hotelTransView);
            }
            else
            {
                var specificTransactionsRaw = hotelTransRaw.Where(a => a.Id_TypeTrans == typeOfTrans);
                RevenueAndIssue = getHotelData.GetTransRevenueAndIssue(specificTransactionsRaw);
                ViewBag.Revenue = RevenueAndIssue[0];
                ViewBag.Issue = RevenueAndIssue[1];
                var specificTransactionsView = getHotelData.GetTransactionsUserView(specificTransactionsRaw).ToList();
                TypesTrans typeTransDescription = getHotelData.getHotelDb().TypesTrans.SingleOrDefault(a => a.Id_TypeTrans == typeOfTrans);
                ViewBag.TypeTransDescription = "Wszystkie transakcje dla hotelu o typie " + typeTransDescription.Description;
                return View("Transactions", specificTransactionsView);
            }
        }

        [HttpGet]
        public ActionResult TransactionsInPeriod(DateTime startTime, DateTime endTime, int? typeOfTrans)
        {
            double[] RevenueAndIssue = new double[2];
            var hotelTrans = getHotelData.GetAllHotelTransactions((int)Session["Id_UserHotel"]);
            ViewBag.AllTypesTransToSelect = getHotelData.getHotelDb().TypesTrans;
            var transInPeriodRaw = getHotelData.GetTransInSpecifiedPeriod(hotelTrans, startTime, endTime);

            if (typeOfTrans == null)
            {
                RevenueAndIssue = getHotelData.GetTransRevenueAndIssue(transInPeriodRaw);
                ViewBag.Revenue = RevenueAndIssue[0];
                ViewBag.Issue = RevenueAndIssue[1];
                var transInPeriodView = getHotelData.GetTransactionsUserView(transInPeriodRaw).ToList();
                ViewBag.TypeTransDescription = "Wszystkie transakcje w okresie od: " + startTime + " do: " + endTime;
                return View("Transactions", transInPeriodView);
            }
            else
            {
                var transInPeriodWithConditionRaw = transInPeriodRaw.Where(a => a.Id_TypeTrans == typeOfTrans);
                RevenueAndIssue = getHotelData.GetTransRevenueAndIssue(transInPeriodWithConditionRaw);
                ViewBag.Revenue = RevenueAndIssue[0];
                ViewBag.Issue = RevenueAndIssue[1];
                var transInPeriodWithConditionView = getHotelData.GetTransactionsUserView(transInPeriodWithConditionRaw).ToList();
                TypesTrans typeTransDescription = getHotelData.getHotelDb().TypesTrans.SingleOrDefault(a => a.Id_TypeTrans == typeOfTrans);
                ViewBag.TypeTransDescription = "Transakcje o typie "+ typeTransDescription.Description + " w okresie od: " + startTime + " do: " + endTime;
                return View("Transactions", transInPeriodWithConditionView);
            }
        }

        [HttpGet]
        public ActionResult RoomDetails(int? idRoom)
        {
            dynamic allRoomData = new ExpandoObject();
            var roomTransRaw = getHotelData.getHotelDb().Transactions.Where(a => a.Id_HotelRoom == idRoom).OrderByDescending(a => a.Id_Trans);
        
            allRoomData.RoomDetails = getHotelData.getHotelDb().HotelRooms.SingleOrDefault(a => a.Id_HotelRoom == idRoom);
            allRoomData.RoomTransactions = getHotelData.GetTransactionsUserView(roomTransRaw).ToList();
            allRoomData.RoomFeatures = getHotelData.getHotelDb().HotelRoomsFeatures.Where(a => a.Id_HotelRoom == idRoom);
       
            return View("RoomDetails", allRoomData);
        }

        [HttpPost]
        public ActionResult AddFeaturesToRoom(string Description,int? idRoomHotel)
        {
            roomProperitiesOperations.AddFeaturesRoom(Description, (int)idRoomHotel);
            return RedirectToAction("RoomDetails", new { idRoom = idRoomHotel });
        }


        [HttpGet]
        public ActionResult RegisterService(int? idRoom,string communicate = "")
        {
            var OperationsToSelect = getHotelData.getHotelDb().TypesTrans.Where(a => a.Id_TypeTrans > (int)TypeOfTrans.CancellinfOfent);
            var roomToService = getHotelData.getHotelDb().HotelRooms.FirstOrDefault(a => a.Id_HotelRoom == idRoom);
            ViewBag.Id_HotelRoom = idRoom;
            ViewBag.RoomName = roomToService.NameOfRoom + " , numer pokoju "+ roomToService.NumberInUserHotel;
            ViewBag.OperationsToList = new SelectList(OperationsToSelect, "Id_TypeTrans", "Description");

            if(communicate != "")
            {
                ViewBag.ErrorCommunicate = communicate;
            }

            return View("RegisterService");
        }
        [HttpPost]
        public ActionResult RegisterService(RegisterServiceOrOperation registerData)
        {
            ResultProcess resultProcess;

            if (ModelState.IsValid)
            {
                resultProcess = registerServiceOrOperation.ProcessRegisterOperationInTransations(registerData, (int)Session["Id_LoggingUser"]);
                ViewBag.Communicate = resultProcess.TextCommunicate;

                if (resultProcess.ResultOfProcess)
                {
                    ViewBag.Communicate = resultProcess.TextCommunicate;
                    return View("CommunicateView");
                }
                else
                {
                    return RedirectToAction("RegisterService", new { idRoom = registerData.Id_HotelRoom,communicate = resultProcess.TextCommunicate });
                }
            }
            else
            {
                return RedirectToAction("RegisterService", new { idRoom = registerData.Id_HotelRoom, communicate = "Poprawnie uzupełnij wszystkie dane !" });
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
            roomProperitiesOperations.RoomStatusChangeUpOrDown((int)Session["Id_LoggingUser"], idRoom, statusChangeUp);
            ViewBag.Communicate = "Operacja została zarejestrowana pomyślnie !";
            return View("CommunicateView");
        }

        [HttpGet]
        public ActionResult Clients()
        {
            var allHotelClient = getHotelData.GetAllHotelClients((int)Session["Id_UserHotel"]);

            return View("Clients", allHotelClient);
        }

        [HttpGet]
        public ActionResult ClientDetails(int? IdClient)
        {
            dynamic allClientData = new ExpandoObject();

            var ClientTransactionsRaw = getHotelData.getHotelDb().Transactions.Where(a => a.Id_Client == IdClient).OrderByDescending(a => a.Id_Trans);

            allClientData.ClientDetails = getHotelData.getHotelDb().Clients.SingleOrDefault(a => a.Id_Client == IdClient);
            allClientData.ClientTransactions = getHotelData.GetTransactionsUserView(ClientTransactionsRaw);

            return View("ClientDetails", allClientData);
        }

        [HttpGet]
        public ActionResult CancellingRentOrReservationConfirm(int? IdTrans, int? IdRoom, DateTime? InDay)
        {
            Transactions transToCancel;

            if (IdTrans != null)
            {
                transToCancel = getHotelData.getHotelDb().Transactions.FirstOrDefault(a => a.Id_Trans == IdTrans);
            }
            else
            {
                transToCancel = getHotelData.getHotelDb().Transactions.FirstOrDefault(a => a.Id_HotelRoom == IdRoom && InDay >= a.FromTime && InDay <= a.ToTime);
            }

            HotelRooms hotelRoomToResearch = getHotelData.getHotelDb().HotelRooms.FirstOrDefault(a => a.Id_HotelRoom == transToCancel.Id_HotelRoom);

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
            roomRentOrReservation.CancellingRentOrReservation((int)Session["Id_LoggingUser"], IdTrans);
            ViewBag.Communicate = "Pomyślnie anulowano transakcję !";
            return View("CommunicateView");
        }

        [HttpGet]
        public ActionResult HotelLoggingUsers()
        {
            int IdHotel = (int)Session["Id_UserHotel"];
            var users = getHotelData.getHotelDb().LoggingUsers.Where(a => a.Id_UserHotel == IdHotel);
            ViewBag.HotelName = getHotelData.getHotelDb().Hotels.FirstOrDefault(a => a.Id_UserHotel == IdHotel).HotelName;
            return View("HotelLoggingUsers", users);
        }

        [HttpGet]
        public JsonResult GetSelectClientData(string jsonInputPersonalNumber)
        {
            ClientData clientData = getHotelData.GetSelectClientDataFromJson(jsonInputPersonalNumber);
            return Json(clientData, JsonRequestBehavior.AllowGet);
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
                roomProperitiesOperations.CreateNewRoom(roomCreateModel, (int)Session["Id_UserHotel"],(int)Session["Id_LoggingUser"]);
                return RedirectToAction("Rooms");
            }
            else
            {
                ViewBag.ErrorCommunicate = "Poprawnie uzupełnij wszystkie dane !";
                return View("RoomCreate");
            }
        }


        public void CheckSessionUser()
        {
            if(Session["Login"] == null)
            {
                RedirectToAction("LoginStart");
            }
        }

        public ActionResult AppInformations()
        {
            return View();
        }
    }
}