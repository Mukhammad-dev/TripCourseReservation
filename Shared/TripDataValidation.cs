using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripCourseReservation.CRUD;
using TripCourseReservation.Entities;

namespace TripCourseReservation.Shared
{
    public class TripDataValidation
    {
        #region Properties
        Trip _trip = new Trip();
        Term _term = new Term();
        XmlCRUD _xmlTripCRUD = new XmlCRUD();
        #endregion

        #region Validation Methods
        public bool CheckIfTripDataExist()
        {
            if (_xmlTripCRUD.ReadTripsData() != null)
            {
                return true;
            }
            else
                return false;
        }

        public bool CheckIfTripAlreadyExists(Trip trip)
        {
            var trips = _xmlTripCRUD.ReadTripsData();

            if (trips.Any(cr => cr.Code == trip.Code))
            {
                return true;
            }
            else
                return false;
        }

        public bool CheckIfTripIsUpdatable(List<Term> terms, DateTime? dateFrom, DateTime? dateTo, bool? isChecked)
        {

            if (terms.Any(tr => tr.DateFrom == dateFrom
                                    && tr.DateTo == dateTo
                                    && tr?.TransportIncluded == isChecked)) return false;
            return true;
        }

        public bool ValidateTripBasicData(string title, string description, string code, ref StringBuilder errorMessage)
        {
            bool tripHasProblem = false;
            if (title == "" || description == "" || code == "")
            {
                errorMessage.AppendLine(" - Please fill required trip fields where must not be empty");
                tripHasProblem = true;
            }

            return tripHasProblem;
        }

        public bool ValidateTripBasicDataToUpdate(string title, string description, ref StringBuilder errorMessage)
        {
            bool tripHasProblem = false;
            if (title == "" || description == "")
            {
                errorMessage.AppendLine(" - Please fill required trip fields where must not be empty");
                tripHasProblem = true;
            }

            return tripHasProblem;
        }

        public bool CheckIfTermExists(List<Term> terms, ref StringBuilder errorMessage)
        {
            bool noTerm = terms.Count == 0;
            if (noTerm)
            {
                errorMessage.AppendLine(" - Trip must have atleast one Term");
                return noTerm;
            }
            return noTerm;
        }

        public bool ValidateTermFieldData(DateTime? dateFrom, DateTime? dateTo, string Price, string Capacity, ref StringBuilder errorMessage)
        {
            errorMessage.Clear();
            bool termHasProblem = false;

            if (!(dateFrom.HasValue && dateTo.HasValue))
            {
                errorMessage.AppendLine(" - DateFrom and DateTo must not be empty.");
                termHasProblem = true;
            }

            if (Price == "0" || Capacity == "0")
            {
                errorMessage.AppendLine(" - Please fill required terms fields where must not be empty");
                termHasProblem = true;
            }
            if (termHasProblem)
            {
                int value;
                if (!Int32.TryParse(Price, out value))
                {
                    errorMessage.AppendLine(" - Price field can contain only integer type");
                    termHasProblem = true;
                }

                if (!Int32.TryParse(Capacity, out value))
                {
                    errorMessage.AppendLine(" - Capacity field can contain only integer type");
                    termHasProblem = true;
                }
            }

            return termHasProblem;
        }

        public bool ValidateTermData(List<Term> terms, bool yesIsChecked, ref StringBuilder errorMessage)
        {
            bool hasProblem = false;
            if (yesIsChecked)
            {
                foreach (Term term in terms)
                {
                    if (term.TransportIncluded == null)
                    {
                        if (errorMessage.Length == 0)
                        {
                            errorMessage.AppendLine("Current Terms must have Transport Included choice - Yes or No :");

                        }
                        errorMessage.AppendLine(" - " + term.Event + " : " + term.DateFrom.ToString("dd/M/yyyy") + " - " + term.DateTo.ToString("dd/M/yyyy"));
                        hasProblem = true;
                    }
                }
            }
            return hasProblem;
        }
        #endregion
    }
}