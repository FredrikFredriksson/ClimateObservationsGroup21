﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClimateObservationsG21
{
    public class Unit
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Abbreviation { get; set; }

        public override string ToString()
        {
            return Abbreviation;
        }
    }
}
