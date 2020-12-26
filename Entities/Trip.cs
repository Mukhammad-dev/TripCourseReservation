﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripCourseReservation.Entities
{
    public class Trip : EventPoperties
    {
        public string Subtitle { get; set; }
        public List<Term> Terms { get; set; }
    }
}
