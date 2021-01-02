using System;
using System.Xml;

namespace TripCourseReservation
{
    internal class XmlXsdResolver : XmlResolver
    {
        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            return GetEntity(absoluteUri, role, ofObjectToReturn);
        }
    }
}