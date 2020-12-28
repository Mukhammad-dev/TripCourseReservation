using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripCourseReservation.Entities;

namespace TripCourseReservation.CRUD
{
    public interface ITripCRUD
    {
        void CreateTrip(Trip trip);
        List<Trip> ReadTripsData();
        void AddTrip(Trip trip);
        void UpdateTripData(Trip trip);
        void RemoveTrip(Trip trip);
    }
}
