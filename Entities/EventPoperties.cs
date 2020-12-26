using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripCourseReservation.Entities
{
    public class EventPoperties
    {
        public string Title { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool CanContainTransport { get; set; }
    }
}
