using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaitingRoomBigScreen.Model
{
    public class Content
    {
        public string PatientName { get; set; }
        public string DeptName { get; set; }
        public string Number { get; set; }
        public string DeptType { get; set; }
        public List<Content> TwoLevelData { get; set; }
    }
}
