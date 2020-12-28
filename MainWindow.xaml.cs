using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using TripCourseReservation.CRUD;
using TripCourseReservation.Entities;
using TripCourseReservation.Shared;
using TripCourseReservation.View_Models;

namespace TripCourseReservation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Properties
        CourseCreateWindow courseCreateWindow;
        CourseUpdateWindow courseUpdateWindow;
        TripCreateWindow tripCreateWindow;
        TripUpdateWindow tripUpdateWindow;

        private ICourseCRUD _courseCRUD = new XmlCRUD();
        private ITripCRUD _tripCRUD = new XmlCRUD();
        DataValidation dataValidation = new DataValidation();
        TripDataValidation tripDataValidation = new TripDataValidation();

        private static readonly string connectionString_1 = ConfigurationManager.ConnectionStrings["courses"].ConnectionString;
        private static readonly string coursesDataRepoPath = connectionString_1.Substring(connectionString_1.IndexOf('=') + 1);
        private static readonly string connectionString_2 = ConfigurationManager.ConnectionStrings["trips"].ConnectionString;
        private static readonly string tripsDataRepoPath = connectionString_2.Substring(connectionString_2.IndexOf('=') + 1);
        private readonly string[] pathes = new string[] { coursesDataRepoPath, tripsDataRepoPath };
        #endregion

        #region Constructor
        public MainWindow()
        {
            InitializeComponent();
            RepoCreation.createDataFile(pathes);
            InitializeCourseData();
            InitializeTripData();
            DataContext = new MainWindowsVM();
        }
        #endregion

        #region Events
        //Course events 
        private void onCoursesSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshCourseTerms();
        }

        private void CreateCourse(object sender, RoutedEventArgs e)
        {
            courseCreateWindow = new CourseCreateWindow();
            courseCreateWindow.ShowDialog();
            InitializeCourseData();
        }

        private void UpdateCourse(object sender, RoutedEventArgs e)
        {
            courseUpdateWindow = new CourseUpdateWindow();
            Course course = (Course)Courses.SelectedItem;

            if (course == null)
            {
                MessageBox.Show("Please select the course from the Courses listbox");
            }
            else
            {
                courseUpdateWindow.course = course;
                courseUpdateWindow.InitializeCourseData();
                courseUpdateWindow.ShowDialog();
                Terms.ItemsSource = null;
                RefreshCourseTerms();
                Courses.ItemsSource = _courseCRUD.ReadCoursesData();
            }
        }

        private void DeleteCourse(object sender, RoutedEventArgs e)
        {
            Course course = (Course)Courses.SelectedItem;
            if (course == null)
            {
                MessageBox.Show("Please select the course from the Courses listbox");
            }
            else
            {
                _courseCRUD.RemoveCourse(course);
                InitializeCourseData();
            }            
        }

        //Trip events
        private void onTripsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshTripTerms();
        }

        private void CreateTrip(object sender, RoutedEventArgs e)
        {
            tripCreateWindow = new TripCreateWindow();
            tripCreateWindow.ShowDialog();
            InitializeTripData();
        }

        private void UpdateTrip(object sender, RoutedEventArgs e)
        {
            tripUpdateWindow = new TripUpdateWindow();
            Trip trip = (Trip)Trips.SelectedItem;

            if (trip == null)
            {
                MessageBox.Show("Please select the trip from the Trips listbox");
            }
            else
            {
                tripUpdateWindow.trip = trip;
                tripUpdateWindow.InitializeTripData();
                tripUpdateWindow.ShowDialog();
                Terms.ItemsSource = null;
                RefreshTripTerms();
                Trips.ItemsSource = _tripCRUD.ReadTripsData();
            }
        }

        private void DeleteTrip(object sender, RoutedEventArgs e)
        {
            Trip trip = (Trip)Trips.SelectedItem;
            if (trip == null)
            {
                MessageBox.Show("Please select the course from the Trips listbox");
            }
            else
            {
                _tripCRUD.RemoveTrip(trip);
                InitializeTripData();
            }
        }
        #endregion

        #region Methods
        //Load courses data
        private void InitializeCourseData()
        {
            var data = dataValidation.CheckIfDataExist();
            if (data)
            {
                Courses.ItemsSource = _courseCRUD.ReadCoursesData();
            }
        }

        private void RefreshCourseTerms()
        {
            Course course = (Course)Courses.SelectedItem;
            var courses = _courseCRUD.ReadCoursesData();

            if (course != null)
            {
                Course selectedCourse = courses.Find(cr => cr.Code == course.Code);
                Terms.ItemsSource = selectedCourse.Terms;
            }
        }

        //Load trips data
        private void InitializeTripData()
        {
            var data = tripDataValidation.CheckIfTripDataExist();
            if (data)
            {
                Trips.ItemsSource = _tripCRUD.ReadTripsData();
            }
        }

        private void RefreshTripTerms()
        {
            Trip trip = (Trip)Trips.SelectedItem;
            var trips = _tripCRUD.ReadTripsData();

            if (trip != null)
            {
                Trip selectedTrip = trips.Find(cr => cr.Code == trip.Code);
                Terms.ItemsSource = selectedTrip.Terms;
            }
        }
        #endregion
    }
}
