using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;


namespace ClimateObservationsG21
{
    class ClimateObservations
    {
        private static readonly string connectionString = "Server=localhost;Port=5432;Database=Climate_Observations_G21; User ID= fredrik; Password=Hemligt";

        #region Get Methods


        /// <summary>
        /// Gets a category from database
        /// </summary>
        /// <param name="id"></param>
        /// <returns>category</returns>
        public Category GetCategory(int id)
        {
            string statement = "select * from category where id = @id";
            using var connection = new NpgsqlConnection(connectionString);

            connection.Open();

            using var command = new NpgsqlCommand(statement, connection);
            command.Parameters.AddWithValue("id", id);

            using var reader = command.ExecuteReader();

            Category category = null;

            while (reader.Read())
            {
                category = new Category
                {
                    Id = (int)reader["id"],
                    Name = Convert.IsDBNull(reader["name"]) ? null : (string?)reader["name"],
                    BaseCategoryId = Convert.IsDBNull(reader["basecategory_id"]) ? null : (int?)reader["basecategory_id"],
                    UnitId = Convert.IsDBNull(reader["unit_id"]) ? null : (int?)reader["unit_id"]
                };
                
            }
            return category;
        }

        /// <summary>
        /// Gets an observer from database
        /// </summary>
        /// <param name="id"></param>
        /// <returns>observer</returns>
        public Observer GetObserver(int id)
        {
            string statement = "select * from observer where id = @id";
            using var connection = new NpgsqlConnection(connectionString);

            connection.Open();

            using var command = new NpgsqlCommand(statement, connection);


            command.Parameters.AddWithValue("id", id);

            using var reader = command.ExecuteReader();

            Observer observer = null;

            while (reader.Read())
            {
                observer = new Observer


                {
                    Id = (int)reader["id"],
                    FirstName = (string)reader["firstname"],
                    LastName = (string)reader["lastname"]
                };

            }

            return observer;
        }

        /// <summary>
        /// Gets an observation from database
        /// </summary>
        /// <param name="id"></param>
        /// <returns>observation</returns>
        //public Observation GetObservation(Observer observer)
        //{
        //    string statement = "select * from observation where id = @id";
        //    using var connection = new NpgsqlConnection(connectionString);

        //    connection.Open();

        //    using var command = new NpgsqlCommand(statement, connection);
        //    command.Parameters.AddWithValue("id", id);

        //    using var reader = command.ExecuteReader();

        //    Observation observation = null;

        //    while (reader.Read())
        //    {
        //        observation = new Observation
        //        {
        //            Id = (int)reader["id"],
        //            Date = (DateTime)reader["date"],
        //            ObserverId = (int)reader["observer_id"],
        //            GeolocationId = (int)reader["geolocation_id"]
        //        };

        //    }
        //    return observation;
        //}

        /// <summary>
        /// Gets a measurement from database
        /// </summary>
        /// <param name="observation"></param>
        /// <returns>measurement</returns>
        public Measurement GetMeasurement(Observation observation)
        {
            string statement = "select * from measurement where observation_id = @observationId";
            using var connection = new NpgsqlConnection(connectionString);

            connection.Open();

            using var command = new NpgsqlCommand(statement, connection);


            command.Parameters.AddWithValue("observationId", observation.Id);

            using var reader = command.ExecuteReader();

            Measurement measurement = null;

            while (reader.Read())
            {
                measurement = new Measurement
                {
                    Id = (int)reader["id"],
                    Value = Convert.IsDBNull(reader["value"]) ? null : Convert.ToSingle(reader["value"]), //Ta reda på varför convert to single
                    CategoryID = (int)reader["category_id"],
                    ObservationId = (int)reader["observation_id"]
                };
                
            }

            return measurement;

        }

        /// <summary>
        /// Gets a list of observations 
        /// </summary>
        /// <returns>List<Observation></returns>
        public List<Observation> GetListObservations(Observer observer)
        {
            string statement = "SELECT * FROM observation WHERE observer_id = @observerId";
            using var connection = new NpgsqlConnection(connectionString);

            connection.Open();

            using var command = new NpgsqlCommand(statement, connection);
           
            command.Parameters.AddWithValue("observerId", observer.Id);
            using var reader = command.ExecuteReader();

            var listOfObservations = new List<Observation>();

            Observation observation = null;
            while (reader.Read())
            {
                observation = new Observation
                {
                    Id = (int)reader["id"],
                    Date = (DateTime)reader["date"],
                    ObserverId = (int)reader["observer_id"],
                    GeolocationId = (int)reader["geolocation_id"]
                };
                listOfObservations.Add(observation);
            }


            return listOfObservations;
        }

        /// <summary>
        /// Gets a list of observers 
        /// </summary>
        /// <returns>List<Observer></returns>
        public List<Observer> GetListOfObservers()
        {
            string statement = "select * from observer order by lastname desc";
            using var connection = new NpgsqlConnection(connectionString);

            connection.Open();

            using var command = new NpgsqlCommand(statement, connection);
            using var reader = command.ExecuteReader();
            Observer observer = null;
            var observers = new List<Observer>();

            while (reader.Read())
            {
                observer = new Observer
                {
                    Id = (int)reader["id"],
                    FirstName = (string)reader["firstname"],
                    LastName = (string)reader["lastname"]
                };
                observers.Add(observer);
            }

            return observers;
        }

        public List<Measurement> GetListOfMeasurements(Observation observation)
        {
            string statement = "select * from measurement WHERE observation_id = @observationId";
            using var connection = new NpgsqlConnection(connectionString);

            connection.Open();

            using var command = new NpgsqlCommand(statement, connection);
            
            command.Parameters.AddWithValue("observationId", observation.Id);
            using var reader = command.ExecuteReader();


            Measurement measurement = null;
            var listOfMeasurements = new List<Measurement>();

            while (reader.Read())
            {
                measurement = new Measurement
                {
                    Id = (int)reader["id"],
                    Value = Convert.IsDBNull(reader["value"]) ? null : Convert.ToSingle(reader["value"]), //Ta reda på varför convert to single
                    CategoryID = (int)reader["category_id"],
                    ObservationId = (int)reader["observation_id"]
                };
                listOfMeasurements.Add(measurement);
            }

            return listOfMeasurements;
        }

        public List<Category> GetListOfCategories(Measurement measurement)
        {
            string statement = "select * from category WHERE id = @categoryId";
            using var connection = new NpgsqlConnection(connectionString);

            connection.Open();
            Category category = null;
            using var command = new NpgsqlCommand(statement, connection);
            
            command.Parameters.AddWithValue("categoryId", measurement.CategoryID);
            using var reader = command.ExecuteReader();
            var listOfCategories = new List<Category>();

            while (reader.Read())
            {
                category = new Category
                {
                    Id = (int)reader["id"],
                    Name = Convert.IsDBNull(reader["name"]) ? null : (string?)reader["name"],
                    BaseCategoryId = Convert.IsDBNull(reader["basecategory_id"]) ? null : (int?)reader["basecategory_id"],
                    UnitId = Convert.IsDBNull(reader["unit_id"]) ? null : (int?)reader["unit_id"]
                };
                listOfCategories.Add(category);
            }

            return listOfCategories;
        }

        public List<Unit> GetListOfUnits(Category category)
        {
            string statement = "select * from unit WHERE id = @unitId";
            using var connection = new NpgsqlConnection(connectionString);

            connection.Open();
            
            using var command = new NpgsqlCommand(statement, connection);
            
            command.Parameters.AddWithValue("unitId", category.UnitId);
            using var reader = command.ExecuteReader();
            var listOfUnits = new List<Unit>();
            Unit unit = null;

            while (reader.Read())
            {
                unit = new Unit
                {
                    Id = (int)reader["id"],
                    Type = (string)reader["type"],
                    Abbreviation = (string)reader["abbreviation"]
                };
                listOfUnits.Add(unit);
            }

            return listOfUnits;
        }


        public void GetInfoAboutObservation(Observation observation)
        {

            string statement = "SELECT measurement.observation_id, category.name, measurement.value, unit.abbreviation" +
                "FROM measurement " +
                "INNER JOIN category " +
                "ON measurement.category_id = category.id " +
                "INNER JOIN unit " +
                "ON category.unit_id = unit.id " +
                "INNER JOIN observation " +
                "ON measurement.observation_id = observation.id " +
                "WHERE observation_id = @observationId;";


            using var connection = new NpgsqlConnection(connectionString);

            connection.Open();
            using var command = new NpgsqlCommand(statement, connection);

            command.Parameters.AddWithValue("observationId", observation.Id);
            command.ExecuteReader();



            

            return // listor med respektive objekt? category.Name, unit.Abbreviation, measurement.Value


        }

        #endregion




        //public void GetMultiAnswer(Observation observation)
        //{
        //string statement = "SELECT measurement.observation_id, category.name, measurement.value, unit.abbreviation" +
        //        "FROM measurement " +
        //        "INNER JOIN category " +
        //        "ON measurement.category_id = category.id " +
        //        "INNER JOIN unit " +
        //        "ON category.unit_id = unit.id " +
        //        "INNER JOIN observation " +
        //        "ON measurement.observation_id = observation.id " +
        //        "WHERE observation_id = @observationId;";


        //    using var connection = new NpgsqlConnection(connectionString);

        //    connection.Open();

        //    using var command = new NpgsqlCommand(statement, connection);

        //    command.Parameters.AddWithValue("observationId", observation.Id);
        //    using var reader = command.ExecuteReader();

        //    List<Measurement> measurements = null;
        //    List<Category> categories = null;
        //    List<Unit> units = null;


        //    while (reader.Read())
        //    {

        //        measurement = new Measurement
        //        {
        //            Id = (int)reader["id"],
        //            Value = Convert.IsDBNull(reader["value"]) ? null : Convert.ToSingle(reader["value"]), //Ta reda på varför convert to single
        //            CategoryID = (int)reader["category_id"],
        //            ObservationId = (int)reader["observation_id"]
        //        };
        //        listOfMeasurements.Add(measurement);
        //    }

        //    return listOfMeasurements;

        //}

        /*
         * SELECT measurement.observation_id, category.name, measurement.value, unit.abbreviation
           FROM measurement

           INNER JOIN category
           ON measurement.category_id = category.id
            
           INNER JOIN unit
           ON category.unit_id = unit.id
            
           INNER JOIN observation
           on measurement.observation_id = observation.id
           WHERE observation_id = 20;
         
        obs!! "20" ÄR HÅRDKODAT!    

        */

        #region Add and Remove Methods

        /// <summary>
        /// Adds an observer to database
        /// </summary>
        /// <param name="observer"></param>
        /// <returns>observer</returns>
        public Observer AddObserver(string firstName, string lastName)
        {
            string statement = "insert into observer(firstname, lastname) values (@firstname, @lastname) returning id";
            try
            {

                using var connection = new NpgsqlConnection(connectionString);
                connection.Open();
                using var command = new NpgsqlCommand(statement, connection);
                command.Parameters.AddWithValue("firstname", firstName ?? Convert.DBNull);
                command.Parameters.AddWithValue("lastname", lastName ?? Convert.DBNull);
                Observer observer = null;
                using var reader = command.ExecuteReader();

                //while (reader.Read())
                //{
                //    observer.Id = (int)reader["id"];
                //}

                return observer;
            }
            catch (PostgresException ex)
            {

                throw new Exception("Du måste ange både för- och efternamn", ex);
            }
        }

        /// <summary>
        /// Adds observation to database
        /// </summary>
        /// <param name="observation"></param>
        /// <returns>observation</returns>
        public Observation AddObservation(Observation observation)
        {


            string statement = "insert into observation(observer_id, geolocation_id) values (@observer, @geolocation)";
            try
            {

                using var connection = new NpgsqlConnection(connectionString);
                connection.Open();
                using var command = new NpgsqlCommand(statement, connection);

                command.Parameters.AddWithValue("observer", observation.ObserverId);
                command.Parameters.AddWithValue("geolocation", observation.GeolocationId);

                using var reader = command.ExecuteReader();

                //while (reader.Read())
                //{
                //    observation.Id = (int)reader["id"];
                //}

                return observation;
            }
            catch (PostgresException ex)
            {

                throw new Exception("Du måste ange både för- och efternamn", ex);
            }
        }

        /// <summary>
        /// Adds measurement to database
        /// </summary>
        /// <param name="measurement"></param>
        /// <returns>measurement</returns>
        public Measurement AddMeasurement(Measurement measurement)
        {

            // inputValue kommer från värdet användaren skriver in, inputCategory kommer från värdet användaren väljer, observationId kommer när en observation skapas

            // Skicka in datan i databasen, returner measurement och sedan presentera

            string statement = "INSERT INTO measurement(value, category_id, observation_id) VALUES (@inputValue, @inputCategory, @observationId)";

            // insert into measurement(value, observation_id, category) values (@inputValue, @observationId, @inputCategory)

            using var connection = new NpgsqlConnection(connectionString);

            connection.Open();

            using var command = new NpgsqlCommand(statement, connection);

            // 
            command.Parameters.AddWithValue("inputValue", measurement.Value ?? Convert.DBNull);
            command.Parameters.AddWithValue("inputCategory", measurement.CategoryID);
            command.Parameters.AddWithValue("observationId", measurement.ObservationId);

            using var reader = command.ExecuteReader();

            return measurement;
        }

        /// <summary>
        /// Removes observer from database
        /// </summary>
        /// <param name="id"></param>
        public void RemoveObserver(Observer observer)
        {

            string statement = "delete from observer where id = @id";

            try
            {
                using var connection = new NpgsqlConnection(connectionString);

                connection.Open();

                using var command = new NpgsqlCommand(statement, connection);

                command.Parameters.Add(new NpgsqlParameter("@id", observer.Id));

                command.ExecuteNonQuery();
            }
            catch (PostgresException ex)
            {

                throw new Exception("Du kan inte ta bort en observatör som har gjort en observation");
            }
        }

        #endregion

    }
}
