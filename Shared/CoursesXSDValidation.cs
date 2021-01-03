using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripCourseReservation.Shared
{
    public class XSDValidation
    {
        private readonly string CourseXSDpath = Path.Combine("Data", "Courses.xsd");

        public XSDValidation()
        {
           var a = File.OpenRead(CourseXSDpath);
        }

        public string GetCourseXSDpath()
        {
            return CourseXSDpath;
        }
    }

}
