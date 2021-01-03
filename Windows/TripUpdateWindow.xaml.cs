using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TripCourseReservation.CRUD;
using TripCourseReservation.Entities;
using TripCourseReservation.Shared;

namespace TripCourseReservation
{
    /// <summary>
    /// Interaction logic for TripUpdateWindow.xaml
    /// </summary>
    public partial class TripUpdateWindow : Window
    {
        #region Properties
        public Trip trip { get; set; }
        List<Term> listOfTerms = new List<Term>();
        ITripCRUD tripCRUD = new XmlCRUD();
        TripDataValidation tripDataValidation = new TripDataValidation();
        StringBuilder errorMessage = new StringBuilder();

        bool termIsUpdated = false;
        #endregion

        #region Constructor
        public TripUpdateWindow()
        {
            InitializeComponent();
            //InitializeCourseData();
        }
        #endregion

        #region Events
        private void Tr_Yes_Click(object sender, RoutedEventArgs e)
        {
            errorMessage.Clear();

            Tr_Inc_No.IsChecked = true;
            if (Tr_Yes.IsChecked == true)
            {
                if (tripDataValidation.ValidateTermData(GetAllTermsListBoxItems(), Tr_Yes.IsChecked == true, ref errorMessage))
                {
                    MessageBox.Show(errorMessage.ToString());
                }
            }
        }

        private void Tr_No_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PickUpPlace.Text != "")
                    PickUpPlace.Text = "";
            }
            catch
            {

            }
        }

        private void Tr_Inc_Yes_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Tr_Inc_Yes.IsEnabled == true)
            {
                if (Terms.SelectedItems.Count > 0)
                {
                    Term term = (Term)Terms.SelectedItem;
                    switch (term.TransportIncluded)
                    {
                        case true:
                            Tr_Inc_Yes.IsChecked = true;
                            PickUpPlace.Text = term.PickUpPlace;
                            break;
                        case false:
                            Tr_Inc_Yes.IsChecked = true;
                            break;
                    }
                }
            }
            else
            {
                Tr_Inc_Yes.IsChecked = false;
            }
        }

        private void Tr_Inc_No_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Tr_Inc_No.IsEnabled == true)
            {
                if (Terms.SelectedItems.Count > 0)
                {
                    Term term = (Term)Terms.SelectedItem;
                    switch (term.TransportIncluded)
                    {
                        case true:
                            Tr_Inc_No.IsChecked = false;
                            break;
                        case false:
                            PickUpPlace.Text = "";
                            Tr_Inc_No.IsChecked = true;
                            break;
                    }
                }
                else
                {
                    Tr_Inc_No.IsChecked = true;
                }
            }
            else
            {
                Tr_Inc_No.IsChecked = false;
            }
        }

        private void Clear_Term_Click(object sender, RoutedEventArgs e)
        {
            ClearTerm();
        }

        private void Update_Term_Click(object sender, RoutedEventArgs e)
        {
            Term term = (Term)Terms.SelectedItem;
            if (ValidateTermFieldData())
            {
                MessageBox.Show(errorMessage.ToString());
            }
            else if (TermUnchanged(term))
            {
                MessageBox.Show("You changed nothing in this term");
            }
            else
            {
                if (tripDataValidation.CheckIfTripIsUpdatable(listOfTerms, DateFrom.SelectedDate, DateTo.SelectedDate, IsTransportIncluded()))
                {
                    listOfTerms.Remove(term);
                    listOfTerms.Add(GenerateTerm());
                    Terms.ItemsSource = null;
                    Terms.ItemsSource = listOfTerms;
                    termIsUpdated = true;
                    ClearTerm();
                }
                else
                    MessageBox.Show("The term with the same Dates already exists!");
            }
        }

        private void Add_Term_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateTermFieldData())
            {
                MessageBox.Show(errorMessage.ToString());
            }
            else if (tripDataValidation.CheckIfTripIsUpdatable(listOfTerms, DateFrom.SelectedDate, DateTo.SelectedDate, IsTransportIncluded()))
            {
                listOfTerms.Add(GenerateTerm());
                Terms.ItemsSource = null;
                Terms.ItemsSource = listOfTerms;
                ClearTerm();
            }
            else
                MessageBox.Show("The term with the same Dates already exists!");
        }

        private void onDeleteTerm(object sender, RoutedEventArgs e)
        {
            Term term = (Term)Terms.SelectedItem;
            listOfTerms.Remove(term);
            Terms.ItemsSource = null;
            Terms.ItemsSource = listOfTerms;
            ClearTerm();
        }

        private void onTermsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Term term = (Term)Terms.SelectedItem;

            Add_Term.Visibility = Visibility.Collapsed;
            Update_Term.Visibility = Visibility.Visible;

            if (term != null)
            {
                DateFrom.SelectedDate = term.DateFrom;
                DateTo.SelectedDate = term.DateTo;
                Price.Text = term.Price.ToString();
                Capacity.Text = term.Capacity.ToString();
                if (term.TransportIncluded != null)
                {
                    switch (term.TransportIncluded)
                    {
                        case true:
                            Tr_Inc_Yes.IsChecked = true;
                            PickUpPlace.Text = term.PickUpPlace;
                            break;
                        case false:
                            Tr_Inc_No.IsChecked = true;
                            break;
                    }
                }
            }
        }

        private void onSaveChanges(object sender, RoutedEventArgs e)
        {
            if (tripDataValidation.ValidateTripBasicDataToUpdate(Title.Text, Description.Text, ref errorMessage)
                || tripDataValidation.CheckIfTermExists(listOfTerms, ref errorMessage))
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
                        if (tripDataValidation.ValidateTermData(GetAllTermsListBoxItems(), Tr_Yes.IsChecked == true, ref errorMessage))
                        {
                            MessageBox.Show(errorMessage.ToString());
                        }
                        else
                            UpdateTrip();
                        break;
                    case MessageBoxResult.No:
                        break;
                }
            }
            else
            {
                if (tripDataValidation.ValidateTermData(GetAllTermsListBoxItems(), Tr_Yes.IsChecked == true, ref errorMessage))
                {
                    MessageBox.Show(errorMessage.ToString());
                }
                else
                    UpdateTrip();
            }
        }

        private void onCancel(object sender, RoutedEventArgs e)
        {
            if (trip.Terms.Count != Terms.Items.Count || termIsUpdated)
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
        }

        #endregion

        #region Methods

        public void InitializeTripData()
        {
            Title.Text = trip.Title;
            Description.Text = trip.Description;
            if (trip.CanContainTransport)
            {
                Tr_Yes.IsChecked = true;
            }
            else
            {
                Tr_No.IsChecked = true;
            }
            Subtitle.Text = trip.Subtitle;
            Update_Term.Visibility = Visibility.Collapsed;
            Terms.ItemsSource = trip.Terms;
            listOfTerms = trip.Terms;
        }

        private bool ValidateTermFieldData()
        {
            return tripDataValidation.ValidateTermFieldData(DateFrom.SelectedDate, DateTo.SelectedDate, Price.Text, Capacity.Text, PickUpPlace.Text, IsTransportIncluded(), ref errorMessage);
        }

        private bool? IsTransportIncluded()
        {
            bool? isChecked;
            if (Tr_Inc_Yes.IsEnabled == true)
            {
                isChecked = Tr_Inc_Yes.IsChecked == true ? true : false;
            }
            else
                isChecked = null;

            return isChecked;
        }

        private bool TermUnchanged(Term term)
        {
            if (term.DateFrom == DateFrom.SelectedDate
                && term.DateTo == DateTo.SelectedDate
                && term.Price == Convert.ToDouble(Price.Text)
                && term.Capacity == Convert.ToInt32(Capacity.Text))
            {
                if (Tr_Inc_Yes.IsEnabled == true)
                {
                    if (term.TransportIncluded == (Tr_Inc_Yes.IsChecked == true))
                    {
                        if (term.PickUpPlace == PickUpPlace.Text)
                            return true;
                    }
                }
                else
                    return true;
            }

            return false;
        }

        private Term GenerateTerm()
        {
            var term = new Term();
            term.Event = Title.Text + " : " + DateFrom.SelectedDate.Value.ToString("dd/M/yyyy") + " - " + DateTo.SelectedDate.Value.ToString("dd/M/yyyy");
            term.DateFrom = DateFrom.SelectedDate.Value;
            term.DateTo = DateTo.SelectedDate.Value;
            term.Price = Convert.ToInt32(Price.Text);
            term.Capacity = Convert.ToInt32(Capacity.Text);
            if (trip.CanContainTransport)
            {
                switch (Tr_Inc_Yes.IsChecked)
                {
                    case true:
                        term.TransportIncluded = true;
                        term.PickUpPlace = PickUpPlace.Text;
                        break;
                    case false:
                        term.TransportIncluded = false;
                        break;
                }
            }

            return term;
        }

        private void ClearTerm()
        {
            DateFrom.SelectedDate = null;
            DateTo.SelectedDate = null;
            Price.Text = "0";
            Capacity.Text = "0";
            PickUpPlace.Text = "";
            Terms.UnselectAll();

            Add_Term.Visibility = Visibility.Visible;
            Update_Term.Visibility = Visibility.Collapsed;
        }

        private void UpdateTrip()
        {
            trip.Title = Title.Text;
            trip.Description = Description.Text;
            trip.CanContainTransport = Tr_Yes.IsChecked == true ? true : false;
            trip.Subtitle = Subtitle.Text;
            trip.Terms = listOfTerms;
            tripCRUD.UpdateTripData(trip);

            this.Close();
        }

        //Gets all Terms listBox items as a List<Term>
        private List<Term> GetAllTermsListBoxItems()
        {
            var listBoxTerms = new List<Term>();
            foreach (Term tr in Terms.Items)
            {
                listBoxTerms.Add(tr);
            }

            return listBoxTerms;
        }
        #endregion
    }
}
