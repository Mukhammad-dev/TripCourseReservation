using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripCourseReservation.Shared;

namespace TripCourseReservation.View_Models
{
    public class CourseVM : ObservableObject
    {
        private string _title;
        private string _description; 
        private string _trainer;
        private DateTime? _dateFrom;
        private DateTime? _dateTo;
        private int _price;
        private int _capacity;

        [Required(ErrorMessage = "Must not be empty.")]
        public string Title
        {
            get { return _title; }
            set
            {
                ValidateProperty(value, "Title");
                OnPropertyChanged(ref _title, value);
            }
        }

        [Required(ErrorMessage = "Must not be empty.")]
        public string Description
        {
            get { return _description; }
            set
            {
                ValidateProperty(value, "Description");
                OnPropertyChanged(ref _description, value);
            }
        }

        [Required(ErrorMessage = "Must not be empty.")]
        [StringLength(50, ErrorMessage = "Must be max 5 characters.")]
        public string Trainer
        {
            get { return _trainer; }
            set
            {
                ValidateProperty(value, "Trainer");
                OnPropertyChanged(ref _trainer, value);
            }
        }

        [Required(ErrorMessage = "Must not be empty.")]
        public Nullable<DateTime> DateFrom
        {
            get { return _dateFrom; }
            set
            {
                ValidateProperty(value, "DateFrom");
                OnPropertyChanged(ref _dateFrom, value);
            }
        }

        [Required(ErrorMessage = "Must not be empty.")]
        public Nullable<DateTime> DateTo
        {
            get { return _dateTo; }
            set
            {
                ValidateProperty(value, "DateTo");
                OnPropertyChanged(ref _dateTo, value);
            }
        }

        [Required(ErrorMessage = "Must not be empty.")]
        public int Price
        {
            get { return _price; }
            set
            {
                ValidateProperty(value, "Price");
                OnPropertyChanged(ref _price, value);
            }
        }

        [Required(ErrorMessage = "Must not be empty.")]
        public int Capacity
        {
            get { return _capacity; }
            set
            {
                ValidateProperty(value, "Capacity");
                OnPropertyChanged(ref _capacity, value);
            }
        }

        private void ValidateProperty<T>(T value, string name)
        {
            Validator.ValidateProperty(value, new ValidationContext(this, null, null)
            {
                MemberName = name
            });
        }
    }
}
