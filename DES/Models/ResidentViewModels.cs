using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using DESCore.Models;

namespace DES.Models
{
    public class ResidentsViewModel
    {
        public int SiteId { get; set; }
        public int ResidentId { get; set; }
        public IEnumerable<ResidentModel> Residents { get; set; }
    }

    public class ResidentViewModel
    {
        public int ResidentId { get; set; }
        public int SiteId { get; set; }

        [Required]
        public int FlatNumber { get; set; }

        [Required]
        public string ResidentName { get; set; }

        [Required]
        public string HomeTel { get; set; }

        [Required]
        public string MobileTel { get; set; }

        [Required]
        public string Email { get; set; }

        public int TagIndex { get; set; }
        public string Info { get; set; }
        public string Error { get; set; }
    }
}