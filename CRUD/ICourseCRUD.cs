using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripCourseReservation.Entities;

namespace TripCourseReservation.CRUD
{
    public interface ICourseCRUD
    {
        void CreateCourse(Course course);
        List<Course> ReadCoursesData();
        void AddCourse(Course course);
        void UpdateCourseData(Course course);
        void RemoveCourse(Course course);
    }
}
