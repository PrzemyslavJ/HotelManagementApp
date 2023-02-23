using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HotelManagementApp.Models
{
    public interface IGetHotelAdvancedData
    {
        HotelManagementDbContext getHotelDb();
        IEnumerable<Transactions> GetAllHotelTransactions(int idHotel);
        IEnumerable<Clients> GetAllHotelClients(int idHotel);
        double[] GetTransRevenueAndIssue(IEnumerable<Transactions> trans);
        IEnumerable<HotelRooms> ConditionOfRoomsAsOfDay(DateTime dateTimeToResearch, int idHotel);
        ClientData GetSelectClientDataFromJson(string jsonInputPersonalNumber,int idHotel);
        IEnumerable<TransUserViewModel> GetTransactionsUserView(IEnumerable<Transactions> transactions);
        IEnumerable<Transactions> GetTransInSpecifiedPeriod(IEnumerable<Transactions> specifiedTrans, DateTime startTime, DateTime endTime);
    }
}
