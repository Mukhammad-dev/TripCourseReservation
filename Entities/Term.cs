using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripCourseReservation.Entities
{
    class Term
    {
        public Trip Trip { get; set; }
        public Course Course { get; set; }
        public string Event { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public double Price { get; set; }
        public int Capacity{ get; set; }
        public bool TransportIncluded { get; set; }
        public string PickUpPlace { get; set; }
    }
}
