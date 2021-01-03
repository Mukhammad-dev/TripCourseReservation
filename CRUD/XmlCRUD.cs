using DocumentFormat.OpenXml.Bibliography;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using TripCourseReservation.CRUD;
using TripCourseReservation.Entities;
using TripCourseReservation.Shared;

namespace TripCourseReservation
{
    public class XmlCRUD: ICourseCRUD, ITripCRUD
    {
        private static readonly string connectionString_1 = ConfigurationManager.ConnectionStrings["courses"].ConnectionString;
        private static readonly string coursesDataRepoPath = connectionString_1.Substring(connectionString_1.IndexOf('=') + 1);

        private static readonly string connectionString_2 = ConfigurationManager.ConnectionStrings["trips"].ConnectionString;
        private static readonly string tripsDataRepoPath = connectionString_2.Substring(connectionString_2.IndexOf('=') + 1);

        private XmlDocument xDoc = new XmlDocument();
        private static readonly XDocument coursesXDocument;

        static XmlCRUD()
        {
            if (!File.Exists(coursesDataRepoPath))
            {
                using(var tw = new StreamWriter(@coursesDataRepoPath))
                {
                    XmlSerializer xsCourse = new XmlSerializer(typeof(List<Course>), new XmlRootAttribute("Courses"));
                    xsCourse.Serialize(tw, new List<Course>());
                }
            }
            coursesXDocument = XDocument.Load(coursesDataRepoPath);
        }



        #region Course CRUD methods

        public void SaveCourse(Course course)
        {
            var courseXElement = coursesXDocument
                                    .Element("Courses")
                                    .Elements("Course")
                                    .FirstOrDefault(a => a.Element("Code").Value == course.Code);
            
            if(courseXElement != null)
                UpdateCourse(course, courseXElement);
            else
                InsertCourse(course);
        }

        private void InsertCourse(Course course)
        {
            var courseXElement = course.ToXElement<Course>();

            coursesXDocument.Root.Add(courseXElement);
            coursesXDocument.Save(coursesDataRepoPath);
        }

        private void UpdateCourse(Course course, XElement courseXElement)
        {
            courseXElement.ReplaceWith(course.ToXElement<Course>()); 

            coursesXDocument.Save(coursesDataRepoPath);
        }

        public void CreateCourse(Course course)
        {
            //TO DO: use "using" statement
            
            var courses = new List<Course>();
            courses.Add(course);
            XmlSerializer xsCourse = new XmlSerializer(typeof(List<Course>), new XmlRootAttribute("Courses"));
            TextWriter txtWritter = new StreamWriter(@coursesDataRepoPath);
            xsCourse.Serialize(txtWritter, courses);

            txtWritter.Close();

            XNamespace i = "http://www.w3.org/2001/XMLSchema-instance";
            XDocument xdoc = XDocument.Load(@coursesDataRepoPath);
            xdoc.Descendants()
                       .Where(node => (string)node.Attribute(i + "nil") == "true")
                       .Remove();

            xdoc.Save(@coursesDataRepoPath);
        }

        public List<Course> ReadCoursesData()
        {
            List<Course> coursesData = new List<Course>();
            XmlSerializer formatter = new XmlSerializer(typeof(List<Course>), new XmlRootAttribute("Courses"));
            using (FileStream fs = new FileStream(coursesDataRepoPath, FileMode.OpenOrCreate))
            {
                try
                {
                    coursesData = (List<Course>)formatter.Deserialize(fs);
                }
                catch (Exception)
                {
                    return null;
                }
            }

            return coursesData;
        }

        public void AddCourse(Course course)
        {
            xDoc.Load(coursesDataRepoPath);
            var rootNode = xDoc.GetElementsByTagName("Courses")[0];
            var nav = rootNode.CreateNavigator();
            var emptyNamepsaces = new XmlSerializerNamespaces(new[] {
            XmlQualifiedName.Empty
        });

            using (var writer = nav.AppendChild())
            {
                var serializer = new XmlSerializer(course.GetType());
                writer.WriteWhitespace("");
                serializer.Serialize(writer, course, emptyNamepsaces);
                writer.Close();
            }
            xDoc.Save(@coursesDataRepoPath);
        }

      

        public void UpdateCourseData(Course course)
        {
            XDocument xdoc = XDocument.Load(coursesDataRepoPath);
            XElement root = xdoc.Element("Courses");

            var foundCourse = root.Elements("Course").FirstOrDefault(a => a.Element("Code").Value == course.Code);

            XmlSerializer courseSerializer = new XmlSerializer(typeof(Course));

            //courseSerializer.Serialize()

            foreach (XElement xe in root.Elements("Course").ToList())
            {
                if (xe.Element("Code").Value == course.Code)
                {
                    xe.Element("Title").Value = course.Title;
                    xe.Element("Code").Value = course.Code;
                    xe.Element("Description").Value = course.Description;
                    xe.Element("CanContainTransport").Value = course.CanContainTransport.ToString().ToLower();
                    xe.Element("Trainer").Value = course.Trainer;
                    xe.Element("Terms").RemoveNodes();
                    foreach (Term term in course.Terms)
                    {
                        XElement xmlTerm = new XElement("Term",
                                                new XElement("Event", term.Event),
                                                new XElement("DateFrom", term.DateFrom),
                                                new XElement("DateTo", term.DateTo),
                                                new XElement("Price", term.Price),
                                                new XElement("Capacity", term.Capacity));

                        if (course.CanContainTransport)
                        {
                            xmlTerm.Add(new XElement("TransportIncluded", term.TransportIncluded.ToString().ToLower()));
                            if (term.TransportIncluded ?? true)
                                xmlTerm.Add(new XElement("PickUpPlace", term.PickUpPlace));
                        }
                        xe.Element("Terms").Add(xmlTerm);
                    }
                }
            }

            xdoc.Save(coursesDataRepoPath);
        }

        public void RemoveCourse(Course course)
        {
            XDocument xdoc = XDocument.Load(coursesDataRepoPath);
            XElement root = xdoc.Element("Courses");

            foreach (XElement xe in root.Elements("Course").ToList())
            {
                if (xe.Element("Code").Value == course.Code)
                {
                    xe.Remove();
                }
            }

            xdoc.Save(coursesDataRepoPath);
        }

        #endregion


        #region Trip CRUD methods
        public void CreateTrip(Trip trip)
        {
            var trips = new List<Trip>();
            trips.Add(trip);
            XmlSerializer xsTrip = new XmlSerializer(typeof(List<Trip>));
            TextWriter txtWritter = new StreamWriter(@tripsDataRepoPath);
            xsTrip.Serialize(txtWritter, trips);
            txtWritter.Close();

            XNamespace i = "http://www.w3.org/2001/XMLSchema-instance";
            XDocument xdoc = XDocument.Load(@tripsDataRepoPath);
            xdoc.Descendants()
                       .Where(node => (string)node.Attribute(i + "nil") == "true")
                       .Remove();
            xdoc.Save(@tripsDataRepoPath);
        }

        public List<Trip> ReadTripsData()
        {
            List<Trip> tripsData = new List<Trip>();
            XmlSerializer formatter = new XmlSerializer(typeof(List<Trip>));
            using (FileStream fs = new FileStream(tripsDataRepoPath, FileMode.OpenOrCreate))
            {
                try
                {
                    tripsData = (List<Trip>)formatter.Deserialize(fs);
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return tripsData;
        }

        public void AddTrip(Trip trip)
        {
            xDoc.Load(tripsDataRepoPath);
            var rootNode = xDoc.GetElementsByTagName("ArrayOfTrip")[0];
            var nav = rootNode.CreateNavigator();
            var emptyNamepsaces = new XmlSerializerNamespaces(new[] {
            XmlQualifiedName.Empty
        });

            using (var writer = nav.AppendChild())
            {
                var serializer = new XmlSerializer(trip.GetType());
                writer.WriteWhitespace("");
                serializer.Serialize(writer, trip, emptyNamepsaces);
                writer.Close();
            }
            xDoc.Save(@tripsDataRepoPath);
        }

        public void UpdateTripData(Trip trip)
        {
            XDocument xdoc = XDocument.Load(tripsDataRepoPath);
            XElement root = xdoc.Element("ArrayOfTrip");

            foreach (XElement xe in root.Elements("Trip").ToList())
            {
                if (xe.Element("Code").Value == trip.Code)
                {
                    xe.Element("Title").Value = trip.Title;
                    xe.Element("Subtitle").Value = trip.Subtitle;
                    xe.Element("Code").Value = trip.Code;
                    xe.Element("Description").Value = trip.Description;
                    xe.Element("CanContainTransport").Value = trip.CanContainTransport.ToString().ToLower();
                    xe.Element("Terms").RemoveNodes();
                    foreach (Term term in trip.Terms)
                    {
                        XElement xmlTerm = new XElement("Term",
                                                new XElement("Event", term.Event),
                                                new XElement("DateFrom", term.DateFrom),
                                                new XElement("DateTo", term.DateTo),
                                                new XElement("Price", term.Price),
                                                new XElement("Capacity", term.Capacity));

                        if (trip.CanContainTransport)
                        {
                            xmlTerm.Add(new XElement("TransportIncluded", term.TransportIncluded.ToString().ToLower()));
                            if (term.TransportIncluded ?? true)
                                xmlTerm.Add(new XElement("PickUpPlace", term.PickUpPlace));
                        }
                        xe.Element("Terms").Add(xmlTerm);
                    }
                }
            }
            xdoc.Save(tripsDataRepoPath);
        }

        public void RemoveTrip(Trip trip)
        {
            XDocument xdoc = XDocument.Load(tripsDataRepoPath);
            XElement root = xdoc.Element("ArrayOfTrip");

            foreach (XElement xe in root.Elements("Trip").ToList())
            {
                if (xe.Element("Code").Value == trip.Code)
                {
                    xe.Remove();
                }
            }
            xdoc.Save(tripsDataRepoPath);
        }
        #endregion
    }
}
