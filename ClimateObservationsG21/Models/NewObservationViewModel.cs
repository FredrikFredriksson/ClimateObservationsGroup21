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

        public DateTime Date { get; set; }

        public int GeolocationId { get; set; }


        //public Observer Observer { get; set; }


        //public override string ToString()
        //{
        //    return $"({Observer.FirstName} {Measurements[0].Observation.Date} {Measurements[0].Category.Unit.Abbreviation})";
        //}
        //MVVM - Googla 


        //public int UpdatePeaksWithTransaction(List<Peak> peaks) //insert istället. 
        //{
        //    string stmt = "update peak set rangeid = @rangeid, elevation=@elevation, peakname=@peakname where peakid = @peakid";
        //    int result = 0; //vid update sql-frågor får vi bara svar på hur många rader/berg som uppdaterats. Drf behövs en int som sparar denna summa åt oss i c#.

        //    using var conn = new NpgsqlConnection(connectionString);//flytta ur try
        //    conn.Open();//flytta ur try
        //    var transaction = conn.BeginTransaction(); // här lägger vi in skillnaden.

        //    try //tydligen behöver vi göra detta på alla våra frågor. FL 3 40 och 50 min in. Vi måste nämligen FÅNGA alla fel!
        //    {

        //        using var command = new NpgsqlCommand(stmt, conn);

        //        foreach (var peak in peaks) //vi vill ju uppdatera hela listan av peaks
        //        {
        //            command.Parameters.AddWithValue("peakname", peak.PeakName ?? Convert.DBNull);
        //            command.Parameters.AddWithValue("elevation", peak.Elevation ?? Convert.DBNull);
        //            command.Parameters.AddWithValue("rangeid", peak.RangeId);
        //            command.Parameters.AddWithValue("peakid", peak.PeakId); //lägger till denna då den hänvisar till wherevillkoret så vi uppdaterar rätt post

        //            result += command.ExecuteNonQuery(); //beror på om vi vill ha tillbaka ngt, kanske ett measurement. 
        //            command.Parameters.Clear(); // viktigt att vi tömmer parametrarna ovan då nästa varva ska börja!
        //        }
        //        transaction.Commit(); //måste bekräfta när det gått bra. 
        //        return result;

        //    }
        //    catch (PostgresException ex) //den får namnet "ex" för det är ett objekt från klassem PostgresException som behöver ett namn
        //    {// i objektet så kan vi nu plocka fram värden. Syns även i xaml.cs-filen!

        //        transaction.Rollback();//lägg till här
        //        string errorCode = ex.SqlState;
        //        throw new Exception("uppdateringen misslyckades", ex);//throw kastar felmeddelandet vidare till användaren så den vet. 

        //    }

        //}


    }
}
