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
    
    public partial class RemoteSiteEventLog
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public Nullable<int> SiteId { get; set; }
        public string Status { get; set; }
        public int EventNumber { get; set; }
        public System.DateTime DateEvent { get; set; }
        public System.DateTime DateACM { get; set; }
        public string Description { get; set; }
        public byte[] Request { get; set; }
        public byte[] Response { get; set; }
        public System.DateTime DateUpdated { get; set; }
    }
}
