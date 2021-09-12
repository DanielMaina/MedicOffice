using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalOffice.ViewModels
{
    public class AssignedOptionVM
    {
        public int ID { get; set; }
        public string DisplayText { get; set; }
        public bool Assigned { get; set; }
        public string Comment { get; set; }
    }
}
