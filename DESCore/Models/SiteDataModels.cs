using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DESCore.Models
{
    public enum SiteDataType { LastProgrammed, LastSaved, Template, Retrieve, BackWork, ProgramStaff, MoveSite };
    public enum SiteParaType { Begin, Channel, Schedule, TradeCode, Door, StaffAccess, SystemOption, End };
    public enum WorkStatusType { Wait, Progress, Completed, Failed };

    public class SiteParaModel
    {
        public SiteData SiteData { get; set; }
        public SiteDataType SiteDataType { get; set; }
        public SiteParaType SiteParaType { get; set; }
        public bool All { get; set; }
        public bool Next { get; set; }
        public Channel Channel { get; set; }
        public Schedule Schedule { get; set; }
        public TradeCode TradeCode { get; set; }
        public Door Door { get; set; }
        public StaffAccess StaffAccess { get; set; }
        public SystemOption SystemOption { get; set; }
    }
}
