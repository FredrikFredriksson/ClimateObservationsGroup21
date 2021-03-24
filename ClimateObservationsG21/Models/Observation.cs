using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClimateObservationsG21
{
    public class Observation
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; } // Använder DateTime? för att kunna hämta ett datum från kalendern i gränssnittet. Den returnerar DateTime?. 
        public int ObserverId { get; set; }
        public Observer Observer { get; set; }
        public int GeolocationId { get; set; }
        
        
        

        public override string ToString()
        {

            return Date?.ToString("yyyy-MM-dd");
        }


       

    }
}
