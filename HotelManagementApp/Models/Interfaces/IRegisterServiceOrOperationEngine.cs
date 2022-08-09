using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementApp.Models
{
    public interface IRegisterServiceOrOperationEngine
    {
        ResultProcess ProcessRegisterOperationInTransations(RegisterServiceOrOperation registerServiceOrOperation, int idLoggingUser);
        void RegisterOperationInTransations();
    }
}
