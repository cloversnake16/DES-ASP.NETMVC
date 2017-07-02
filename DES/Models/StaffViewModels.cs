using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DESCore.Models;

namespace DES.Models
{
    public class StaffModel
    {
        public List<StaffGroupViewModel> ListStaffGroup { get; set; }
        public List<StaffViewModel> ListStaff { get; set; }
    }

    public class StaffViewModel
    {
        public int Id { get; set; }        
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int PayrollNumber { get; set; }
        public string DateExpire { get; set; }
        public string DateUpdated { get; set; }
        public int StaffGroupId { get; set; }
    }

    public class StaffGroupViewModel
    {
        public int Id { get; set; }
        public string StaffGroupName { get; set; }
        public int Door1 { get; set; }
        public int Door2 { get; set; }
        public int Door3 { get; set; }
        public int Door4 { get; set; }
        public int[] Tags { get; set; }
        public int[] StaffIds { get; set; }
        public string SiteIds { get; set; }
        public DateTime DateUpdated { get; set; }
    }
}