using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClimateObservationsG21
{
    public class NewObservationViewModel
    {

        public Observer Observer { get; set; }

        public List<Measurement> Measurements = new List<Measurement>();

        public DateTime? Date { get; set; } // Använder DateTime? för att kunna hämta datumet från kalender i gränssnittet. Den returnerar DateTime?. 

        public int GeolocationId { get; set; }

        public override string ToString()
        {
            return Date?.ToString("yyyy-MM-dd");
        }

        //Lägg till public Geolocation Geolocation { get; set; } om vi hinner, gör då om metoden GetViewModel()
    }
}
