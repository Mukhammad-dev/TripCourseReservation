using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripCourseReservation.Shared
{
    public class TripsXSDValidation
    {
        private readonly string TripXSDpath = Path.Combine("Data", "Trips.xsd");

        public TripsXSDValidation()
        {
           
        }

        public string GetTripXSDpath()
        {
            return TripXSDpath;
        }
    }
}
