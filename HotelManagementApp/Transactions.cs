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
    
    public partial class Transactions
    {
        public int Id_Trans { get; set; }
        public int Id_LoggingUser { get; set; }
        public int Id_TypeTrans { get; set; }
        public Nullable<double> Cost { get; set; }
        public string Description { get; set; }
        public Nullable<int> Id_Client { get; set; }
        public Nullable<int> Id_HotelRoom { get; set; }
        public Nullable<System.DateTime> FromTime { get; set; }
        public Nullable<System.DateTime> ToTime { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<int> Id_thisRef { get; set; }
        public Nullable<System.DateTime> CreatedRecordDateTime { get; set; }
    
        public virtual Clients Clients { get; set; }
        public virtual HotelRooms HotelRooms { get; set; }
        public virtual LoggingUsers LoggingUsers { get; set; }
        public virtual TypesTrans TypesTrans { get; set; }
    }
}
