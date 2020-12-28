using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using TripCourseReservation.CRUD;
using TripCourseReservation.Entities;
using TripCourseReservation.Shared;
using TripCourseReservation.View_Models;

namespace TripCourseReservation
{
    /// <summary>
    /// Interaction logic for TripCreateWindow.xaml
    /// </summary>
    public partial class TripCreateWindow : Window
    {
        #region Properties

        private ITripCRUD tripCRUD = new XmlCRUD();

        Trip trip = new Trip();
        List<Term> terms = new List<Term>();
        TripDataValidation tripDataValidation = new TripDataValidation();
        StringBuilder errorMessage = new StringBuilder();
        bool isAnyDataInXmlFile = false;

        #endregion

        #region Constructor
        public TripCreateWindow()
        {
            isAnyDataInXmlFile = tripDataValidation.CheckIfTripDataExist();

            InitializeComponent();
            InitializeDefaults();
            SaveButtonAbility();
            DataContext = new TripVM();

        }
        #endregion

        #region Events
        private void Tr_Inc_Yes_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Tr_Inc_Yes.IsChecked = false;
        }

        private void Tr_Inc_No_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            switch (Tr_Inc_No.IsEnabled)
            {
                case true:
                    Tr_Inc_No.IsChecked = true;
                    break;
                case false:
                    Tr_Inc_No.IsChecked = false;
                    break;
            }
        }

        private void onSaveTerm(object sender, RoutedEventArgs e)
        {
            if (Title.Text == "")
            {
                MessageBox.Show("Term cannot be created without Trip title");
            }
            else
            {
                if (ValidateTermFieldData())
                {
                    errorMessage.Clear();
                    MessageBox.Show(errorMessage.ToString());
                }
                else
                {
                    Terms.ItemsSource = null;
                    Terms.ItemsSource = AddTerm();
                    ClearTermsField();
                    SaveButtonAbility();
                }
            }
        }

        private void onSave(object sender, RoutedEventArgs e)
        {
            if (tripDataValidation.ValidateTripBasicData(Title.Text, Description.Text, Code.Text, ref errorMessage)
                || tripDataValidation.CheckIfTermExists(terms, ref errorMessage))
            {
                MessageBox.Show(errorMessage.ToString());
            }
            else if ((ValidateTermFieldData() == false))
            {
                MessageBoxResult result = MessageBox.Show("All unsaved Term changes will lose, do you want to continue",
                    "Confirmation", MessageBoxButton.YesNo);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        GenerateTrip();
                        break;
                    case MessageBoxResult.No:
                        break;
                }
            }
            else
            {
                GenerateTrip();
            }
        }

        private void onCancel(object sender, RoutedEventArgs e)
        {
            IsFieldsAreDirty();
        }
        #endregion

        #region Methods
        private void InitializeDefaults()
        {
            No.IsChecked = true;
        }

        private bool ValidateTermFieldData()
        {
            return tripDataValidation.ValidateTermFieldData(DateFrom.SelectedDate, DateTo.SelectedDate, Price.Text, Capacity.Text, ref errorMessage);
        }

        private List<Term> AddTerm()
        {
            var term = new Term();
            term.Event = Title.Text + " : " + DateFrom.SelectedDate.Value.ToString("dd/M/yyyy") + " - " + DateTo.SelectedDate.Value.ToString("dd/M/yyyy");
            term.DateFrom = DateFrom.SelectedDate.Value;
            term.DateTo = DateTo.SelectedDate.Value;
            term.Price = Convert.ToDouble(Price.Text);
            term.Capacity = Convert.ToInt32(Capacity.Text);

            if (Yes.IsChecked == true)
            {
                if (Tr_Inc_Yes.IsChecked == true)
                {
                    term.TransportIncluded = true;
                    term.PickUpPlace = PickUpPlace.Text;
                }
                else if (Tr_Inc_No.IsChecked == true)
                {
                    term.TransportIncluded = false;
                }
                else
                {
                    term.TransportIncluded = null;
                }

                //term.TransportIncluded = Tr_Inc_Yes.IsChecked == true ? true : false;
            }
            terms.Add(term);

            return terms;
        }

        private void ClearTermsField()
        {
            DateFrom.SelectedDate = null;
            DateTo.SelectedDate = null;
            Price.Text = "0";
            Capacity.Text = "0";
        }

        private void GenerateTrip()
        {
            trip.Code = Code.Text;
            trip.Title = Title.Text;
            trip.Subtitle = Subtitle.Text;
            trip.Description = Description.Text;
            trip.CanContainTransport = Yes.IsChecked == true ? true : false;
            trip.Terms = terms;

            if (isAnyDataInXmlFile)
            {
                if (tripDataValidation.CheckIfTripAlreadyExists(trip))
                {
                    MessageBox.Show("The Course with current Code already exists, please choose another Code.");
                }
                else
                {
                    tripCRUD.AddTrip(trip);
                    this.Close();
                }
            }
            else
            {
                tripCRUD.CreateTrip(trip);
                this.Close();
            }
        }

        private bool IsFieldsAreDirty()
        {
            if (Code.Text != "" || Description.Text != "" || Title.Text != "" || DateFrom.SelectedDate.HasValue || DateTo.SelectedDate.HasValue
                || Price.Text != "0" || Capacity.Text != "0" || Tr_Inc_Yes.IsEnabled == true)
            {
                MessageBoxResult result = MessageBox.Show("You didnt save trip changes. All unsaved Trip changes will lose, do you want to continue",
                    "Confirmation", MessageBoxButton.YesNo);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        this.Close();
                        break;
                    case MessageBoxResult.No:
                        break;
                }
            }

            this.Close();
            return true;
        }

        private void SaveButtonAbility()
        {
            Save.IsEnabled = terms.Count > 0 ? true : false;
        }
        #endregion
    }
}
