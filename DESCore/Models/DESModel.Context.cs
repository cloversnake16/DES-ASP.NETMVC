﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DESEntities : DbContext
    {
        public DESEntities()
            : base("name=DESEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<BackTask> BackTasks { get; set; }
        public virtual DbSet<Channel> Channels { get; set; }
        public virtual DbSet<Description> Descriptions { get; set; }
        public virtual DbSet<Device> Devices { get; set; }
        public virtual DbSet<Door> Doors { get; set; }
        public virtual DbSet<EventLog> EventLogs { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<RemoteSiteEventLog> RemoteSiteEventLogs { get; set; }
        public virtual DbSet<Resident> Residents { get; set; }
        public virtual DbSet<Schedule> Schedules { get; set; }
        public virtual DbSet<Setting> Settings { get; set; }
        public virtual DbSet<Site> Sites { get; set; }
        public virtual DbSet<SiteData> SiteDatas { get; set; }
        public virtual DbSet<Staff> Staffs { get; set; }
        public virtual DbSet<StaffAccess> StaffAccesses { get; set; }
        public virtual DbSet<StaffGroup> StaffGroups { get; set; }
        public virtual DbSet<StaffGroupSite> StaffGroupSites { get; set; }
        public virtual DbSet<SystemOption> SystemOptions { get; set; }
        public virtual DbSet<TradeCode> TradeCodes { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserLogin> UserLogins { get; set; }
        public virtual DbSet<UserType> UserTypes { get; set; }
    }
}
