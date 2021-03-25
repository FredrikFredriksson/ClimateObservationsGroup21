using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClimateObservationsG21
{
    public class Category
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public int? BaseCategoryId { get; set; }

        public Category BaseCategory { get; set; }

        public int? UnitId { get; set; }

        public Unit Unit { get; set; }


        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
