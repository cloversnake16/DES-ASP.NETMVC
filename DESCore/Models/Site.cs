//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DESCore.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Site
    {
        public int Id { get; set; }
        public string SiteName { get; set; }
        public int UserId { get; set; }
        public string DeviceId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerPhone { get; set; }
        public string Address2 { get; set; }
        public string Area { get; set; }
        public string Town { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public Nullable<System.DateTime> DateUpdated { get; set; }
    
        public virtual User User { get; set; }
    }
}
