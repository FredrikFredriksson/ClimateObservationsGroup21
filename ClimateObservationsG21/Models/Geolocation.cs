using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClimateObservationsG21
{
    public class Geolocation
    {
        public int Id { get; set; }
        public float? Lattitude { get; set; }
        public float? Longitude { get; set; }
        public int AreaId { get; set; }
        public Area Area { get; set; }
    }
}
