using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
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
        
        CourseDataValidation courseDataValidation = new CourseDataValidation();
        TripDataValidation tripDataValidation = new TripDataValidation();

        private static readonly string connectionString_1 = ConfigurationManager.ConnectionStrings["courses"].ConnectionString;
        private static readonly string coursesDataRepoPath = connectionString_1.Substring(connectionString_1.IndexOf('=') + 1);
        private static readonly string connectionString_2 = ConfigurationManager.ConnectionStrings["trips"].ConnectionString;
        private static readonly string tripsDataRepoPath = connectionString_2.Substring(connectionString_2.IndexOf('=') + 1);
        private readonly string[] pathes = new string[] { coursesDataRepoPath, tripsDataRepoPath };

        private string appPath = "";
        #endregion

        #region Constructor
        public MainWindow()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.InnerException.ToString());
            }
            RepoCreation.createDataFile(pathes);

            appPath = Path.GetDirectoryName(
                     Assembly.GetAssembly(typeof(MainWindow)).CodeBase);

            InitializeCourseData();
            InitializeTripData();
            DataContext = new MainWindowsVM();
        }
        #endregion

        #region Course Events
        private void onCoursesSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshCourseTerms();
            Trips.SelectedItem = null;
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
                MessageBoxResult result = MessageBox.Show("Are you sure that you want to delete this course?",
                    "Confirmation", MessageBoxButton.YesNo);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        _courseCRUD.RemoveCourse(course);
                        InitializeCourseData();
                        Terms.ItemsSource = null;
                        break;
                    case MessageBoxResult.No:
                        break;
                }
            }            
        }
        #endregion

        #region Trip Events
        private void onTripsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshTripTerms();
            Courses.SelectedItem = null;
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
                Terms.ItemsSource = null;
            }
        }
        #endregion

        #region Methods
        //Load courses data
        private void InitializeCourseData()
        {
            var data = courseDataValidation.CheckIfDataExist();
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

        public string GetApplicationPath(string appname)
        {
            using (Microsoft.Win32.RegistryKey key = Microsoft.Win32.RegistryKey.OpenRemoteBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, ""))
            {
                using (Microsoft.Win32.RegistryKey subkey = key.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\" + appname))
                {
                    if (subkey == null)
                        return "";

                    object path = subkey.GetValue("Path");

                    if (path != null)
                        return (string)path;
                }

            }
            return "";
        }

        private void onImportCourseData(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
            openFileDlg.DefaultExt = ".xml";
            openFileDlg.Filter = "Text documents (.xml)|*.xml";
            // Launch OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = openFileDlg.ShowDialog();

            var path = new Uri(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase)).LocalPath;
            XmlSchemaSet schema = new XmlSchemaSet();
            schema.Add("", path + "\\" + CoursesXSDValidation.GetXSDpath());
            XmlReader rd = XmlReader.Create(openFileDlg.FileName);
            XDocument doc = XDocument.Load(rd);
            doc.Validate(schema, ValidationEventHandler);

            var dir = Path.GetDirectoryName(coursesDataRepoPath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            if (File.Exists(coursesDataRepoPath))
            {
                File.Delete(coursesDataRepoPath);
                File.Copy(openFileDlg.FileName, coursesDataRepoPath);
            }
            else
            {
                File.Copy(openFileDlg.FileName, coursesDataRepoPath);
            }
        }

        private void onImportTripData(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
            openFileDlg.DefaultExt = ".xml";
            openFileDlg.Filter = "Text documents (.xml)|*.xml";
            // Launch OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = openFileDlg.ShowDialog();

            var path = new Uri(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase)).LocalPath;
            XmlSchemaSet schema = new XmlSchemaSet();
            schema.Add("", path + "\\" + CoursesXSDValidation.GetXSDpath());
            XmlReader rd = XmlReader.Create(openFileDlg.FileName);
            XDocument doc = XDocument.Load(rd);
            doc.Validate(schema, ValidationEventHandler);

            var dir = Path.GetDirectoryName(coursesDataRepoPath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            if (File.Exists(coursesDataRepoPath))
            {
                File.Delete(coursesDataRepoPath);
                File.Copy(openFileDlg.FileName, coursesDataRepoPath);
            }
            else
            {
                File.Copy(openFileDlg.FileName, coursesDataRepoPath);
            }
        }

        static void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            XmlSeverityType type = XmlSeverityType.Warning;
            if (Enum.TryParse<XmlSeverityType>("Error", out type))
            {
                try
                {
                    if (type == XmlSeverityType.Error) throw new Exception(e.Message);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }                
            }
        }

        CoursesXSDValidation CoursesXSDValidation = new CoursesXSDValidation();
    }
}
