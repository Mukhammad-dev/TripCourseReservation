using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripCourseReservation.Shared
{
    public class CoursesXSDValidation
    {
        private readonly string XSDpath = Path.Combine("Data", "Courses.xsd");

        public CoursesXSDValidation()
        {
           var a = File.OpenRead(XSDpath);
        }

        public string GetXSDpath()
        {
            return XSDpath;
        }
    }

}
