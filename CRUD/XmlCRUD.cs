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

namespace TripCourseReservation
{
    public class XmlCRUD: ICourseCRUD, ITripCRUD
    {
        private static readonly string connectionString_1 = ConfigurationManager.ConnectionStrings["courses"].ConnectionString;
        private static readonly string coursesDataRepoPath = connectionString_1.Substring(connectionString_1.IndexOf('=') + 1);

        private static readonly string connectionString_2 = ConfigurationManager.ConnectionStrings["trips"].ConnectionString;
        private static readonly string tripsDataRepoPath = connectionString_2.Substring(connectionString_2.IndexOf('=') + 1);

        private XmlDocument xDoc = new XmlDocument();

        #region Course CRUD methods

        public void CreateCourse(Course course)
        {
            //TO DO: use "using" statement
            
            var courses = new List<Course>();
            courses.Add(course);
            //courseList.Courses.Add(course);
            XmlSerializer xsCourse = new XmlSerializer(typeof(List<Course>));
            //XmlSerializer xsTrip = new XmlSerializer(typeof(Trip));

            TextWriter txtWritter = new StreamWriter(@coursesDataRepoPath);

            xsCourse.Serialize(txtWritter, courses);
            //xsTrip.Serialize(txtWritter, trip);

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
            XmlSerializer formatter = new XmlSerializer(typeof(List<Course>));
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
            var rootNode = xDoc.GetElementsByTagName("ArrayOfCourse")[0];
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

        //public void CreateCourse(Course course)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    sb.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
        //                "<ArrayOfCourse><Course>" +
        //                "<Title>" + course.Title + "</Title>" +
        //                "<Code>" + course.Code + "</Code>" +
        //                "<Description>" + course.Description + "</Description>" +
        //                "<CanContainTransport>" + course.CanContainTransport.ToString().ToLower() + "</CanContainTransport>" +
        //                "<Trainer>" + course.Trainer + "</Trainer>");
        //    foreach (Term term in course.Terms)
        //    {
        //        sb.Append("<Terms>" +
        //                      "<Term>" +
        //                          "<Event>" + term.Event + "</Event>" +
        //                          "<DateFrom>" + term.DateFrom + "</DateFrom>" +
        //                          "<DateTo>" + term.DateTo + "</DateTo>" +
        //                          "<Price>" + term.Price.ToString() + "</Price>" +
        //                          "<Capacity>" + term.Capacity.ToString() + "</Capacity>");
        //        if (course.CanContainTransport)
        //        {
        //            sb.Append("<TransportIncluded>" + term.TransportIncluded.ToString().ToLower() + "</TransportIncluded>");
        //            if (term.TransportIncluded)
        //                sb.Append("<PickUpPlace>" + term.PickUpPlace + "</PickUpPlace>");
        //        }
        //        sb.Append("</Term>" +
        //               "</Terms>");
        //    }
        //    sb.Append("</Course></ArrayOfCourse>");

        //    //Create the XmlDocument.  
        //    XmlDocument doc = new XmlDocument();
        //    //doc.LoadXml((sb.ToString()));
        //    doc.LoadXml((Convert.ToString(sb)));
        //    //Save the document to a file.  
        //    doc.Save(dataRepoPathes[0]);
        //}

        public void UpdateCourseData(Course course)
        {
            XDocument xdoc = XDocument.Load(coursesDataRepoPath);
            XElement root = xdoc.Element("ArrayOfCourse");

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
            XElement root = xdoc.Element("ArrayOfCourse");

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

        public void RemoveData(Trip trip)
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

        public void RemoveTrip(Trip trip)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
