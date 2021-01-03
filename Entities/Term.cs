using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TripCourseReservation.Entities
{
    public class Term
    {
        public string Event { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public double Price { get; set; }
        public int Capacity{ get; set; }
        public bool? TransportIncluded { get; set; }
        public string PickUpPlace { get; set; }

        public bool ShouldSerializeTransportIncluded()
        {
            return TransportIncluded.HasValue;
        }
    }
}
