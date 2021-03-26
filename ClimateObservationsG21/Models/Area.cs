using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClimateObservationsG21
{
    public class Area
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Country Country { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
