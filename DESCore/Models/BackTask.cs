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
    
    public partial class BackTask
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string DeviceId { get; set; }
        public int SiteDataId { get; set; }
        public string WorkType { get; set; }
        public string WorkStatus { get; set; }
        public string WorkItem { get; set; }
        public int WorkIndex { get; set; }
        public string Description { get; set; }
        public System.DateTime DateCreated { get; set; }
        public System.DateTime DateUpdated { get; set; }
    }
}