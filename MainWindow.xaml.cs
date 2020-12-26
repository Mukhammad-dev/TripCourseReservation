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

        private ICourseCRUD _courseCRUD = new XmlCRUD();
        private ITripCRUD _tripCRUD = new XmlCRUD();
        DataValidation dataValidation = new DataValidation();

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
            DataContext = new MainWindowsVM();
        }
        #endregion

        #region Events
        private void onCoursesSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshTerms();
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
                RefreshTerms();
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

        private void UpdateTrip(object sender, RoutedEventArgs e)
        {

        }

        private void CreateTrip(object sender, RoutedEventArgs e)
        {
            
        }
        #endregion

        #region Methods
        private void InitializeCourseData()
        {
            var data = dataValidation.CheckIfDataExist();
            if (data)
            {
                Courses.ItemsSource = _courseCRUD.ReadCoursesData();
            }
        }

        private void RefreshTerms()
        {
            Course course = (Course)Courses.SelectedItem;
            var courses = _courseCRUD.ReadCoursesData();

            if (course != null)
            {
                Course selectedCourse = courses.Find(cr => cr.Code == course.Code);
                Terms.ItemsSource = selectedCourse.Terms;
            }
        }
        #endregion
    }
}
