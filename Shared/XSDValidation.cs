using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using System.Xml.Schema;

namespace TripCourseReservation.Shared
{
    public class XSDValidation
    {
        private readonly string courseXSDpath;
        private readonly string tripXSDpath;

        public XSDValidation(string rootPath)
        {
            courseXSDpath = Path.Combine(rootPath, "Data", "Courses.xsd");
            tripXSDpath = Path.Combine(rootPath, "Data", "Trips.xsd");
        }

        public bool ValidateCourse(XDocument xDocument)
        {
            bool result = true;
            XmlSchemaSet schema = new XmlSchemaSet();
            schema.Add("", courseXSDpath);
            xDocument.Validate(schema, (sender, e) => {
                XmlSeverityType type = XmlSeverityType.Warning;
                if (Enum.TryParse<XmlSeverityType>("Error", out type))
                {
                    try
                    {
                        if (type == XmlSeverityType.Error) throw new Exception(e.Message);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        result = false;
                    }
                }
            });
            return result;
        }

        public bool ValidateTrip(XDocument xDocument)
        {
            bool result = true;
            XmlSchemaSet schema = new XmlSchemaSet();
            schema.Add("", tripXSDpath);
            xDocument.Validate(schema, (sender, e) => {
                XmlSeverityType type = XmlSeverityType.Warning;
                if (Enum.TryParse<XmlSeverityType>("Error", out type))
                {
                    try
                    {
                        if (type == XmlSeverityType.Error) throw new Exception(e.Message);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        result = false;
                    }
                }
            });
            return result;
        }
    }

}
