using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClimateObservationsG21
{
    public class Observer
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        // Går det kanske lägga in List<Observation> Observations = new List<Observation> här?  


        public override string ToString()
        {
            return $"{FirstName} {LastName}";
        }


    }
}
