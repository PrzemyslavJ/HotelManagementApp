//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HotelManagementApp
{
    using System;
    using System.Collections.Generic;
    
    public partial class LoggingUsers
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public LoggingUsers()
        {
            this.Transactions = new HashSet<Transactions>();
        }
    
        public int Id_LoggingUser { get; set; }
        public int Id_UserHotel { get; set; }
        public bool IsHotelOwner { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Street { get; set; }
        public string BuildingNumber { get; set; }
        public string FlatNumber { get; set; }
        public string Telephone { get; set; }
        public string BusinessEmail { get; set; }
        public System.DateTime StartOfWork { get; set; }
        public Nullable<System.DateTime> EndOfWork { get; set; }
        public string TypeOfEmployment { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Transactions> Transactions { get; set; }
    }
}
