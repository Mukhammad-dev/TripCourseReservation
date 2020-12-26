using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TripCourseReservation.Entities
{
    public class Course : EventPoperties
    {
        public string Trainer { get; set; }
        public List<Term> Terms { get; set; }
    }
}