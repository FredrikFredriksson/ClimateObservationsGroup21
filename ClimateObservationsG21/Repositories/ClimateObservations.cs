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

        #region Methods

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
                    Name = (string)reader["name"],
                    BaseCategoryId = Convert.IsDBNull(reader["basecategory_id"]) ? null : (int?)reader["basecategory_id"],
                    UnitId = Convert.IsDBNull(reader["unit_id"]) ? null : (int?)reader["unit_id"]
                };
                
            }
            return category;
        }



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


        public List<Observer> GetObservers()
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

        public Observer AddObserver(Observer observer)
        {
            string statement = "insert into observer(firstname, lastname) values (@firstname, @lastname) returning id";
            try
            {

                using var connection = new NpgsqlConnection(connectionString);
                connection.Open();
                using var command = new NpgsqlCommand(statement, connection);
                command.Parameters.AddWithValue("firstname", observer.FirstName ?? Convert.DBNull);
                command.Parameters.AddWithValue("lastname", observer.LastName ?? Convert.DBNull);

                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    observer.Id = (int)reader["id"];
                }

                return observer;
            }
            catch (PostgresException ex)
            {

                throw new Exception("Du måste ange både för- och efternamn", ex);
            }
        }

        public void RemoveObserver(int id)
        {

            string statement = "delete from observer where id = @id";

            try
            {
                using var connection = new NpgsqlConnection(connectionString);

                connection.Open();

                using var command = new NpgsqlCommand(statement, connection);

                command.Parameters.Add(new NpgsqlParameter("@id", id));

                command.ExecuteNonQuery();
            }
            catch (PostgresException ex)
            {

                throw new Exception("Du kan inte ta bort en observatör som har gjort en observation");
            }
        }


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

        public List<Observation> GetListObservations()
        {
            string statement = "SELECT * FROM observation";
            using var connection = new NpgsqlConnection(connectionString);

            connection.Open();

            using var command = new NpgsqlCommand(statement, connection);
            using var reader = command.ExecuteReader();

            Observation observation;

            var listOfObservations = new List<Observation>();

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


        public Observation GetObservation(int id)
        {
            string statement = "select * from observation where id = @id";
            using var connection = new NpgsqlConnection(connectionString);

            connection.Open();

            using var command = new NpgsqlCommand(statement, connection);
            command.Parameters.AddWithValue("id", id);

            using var reader = command.ExecuteReader();

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

            }
            return observation;
        }

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


        #endregion

    }
}
