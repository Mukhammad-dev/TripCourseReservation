using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripCourseReservation.Entities
{
    class Course
    {
        public string Title { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool CanContainTransport { get; set; }
        public string Trainer { get; set; }
        public List<Term> Terms { get; set; }
    }
}