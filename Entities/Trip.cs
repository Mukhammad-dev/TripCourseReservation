using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripCourseReservation.Entities
{
    class Trip
    {
        public int MyProperty { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool CanContainTransport { get; set; }
        public List<Term> Terms { get; set; }
    }
}
