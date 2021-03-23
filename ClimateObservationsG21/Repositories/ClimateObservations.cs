using Npgsql;
using System;
using System.Collections.Generic;


namespace ClimateObservationsG21
{
    class ClimateObservations
    {
        private static readonly string connectionString = "Server=localhost;Port=5432;Database=Climate_Observations_G21; User ID= fredrik; Password=Hemligt";



        #region Get Methods

        public void AddViewModelToDatabase(NewObservationViewModel vm)
        {
            string statement = "insert into observation (geolocation_id, date, observer_id) values (@gId, @date, @oId) returning id"; //insert-fråga del 1. Vad vill vi returnera?

            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            var transaction = connection.BeginTransaction();//transaktionen börjar
            using var command = new NpgsqlCommand(statement, connection);

            command.Parameters.AddWithValue("gId", vm.GeolocationId);
            command.Parameters.AddWithValue("date", vm.Date);
            command.Parameters.AddWithValue("oId", vm.Observer.Id);

            int observationId = (int)command.ExecuteScalar();

            transaction.Commit();

            for (int i = 0; i < vm.Measurements.Count; i++)
            {
                AddMeasurement(vm.Measurements[i], observationId);
            }
        }

        public NewObservationViewModel GetViewModel(Observation observation)
        {

            string statement = "SELECT measurement.id as \"mId\", measurement.value as \"mValue\", category.id as \"cId\", category.name as \"cName\", unit.id as \"uId\", unit.type as \"uType\", unit.abbreviation as \"uAb\" FROM measurement LEFT JOIN category on category_id = category.id LEFT JOIN unit on unit_id = unit.id WHERE measurement.observation_id = @observationId";
            //  unit.id as \"uId\", unit.type as \"uType\" unit.abbreviation as \"uAb\" LEFT JOIN unit on unit_id = unit.id 


            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            using var command = new NpgsqlCommand(statement, connection);
            command.Parameters.AddWithValue("observationId", observation.Id);

            using var reader = command.ExecuteReader();


            Category category = null;
            Unit unit = null;
            Measurement measurement = null;
            NewObservationViewModel vm = new NewObservationViewModel();



            while (reader.Read())
            {
                measurement = null;


                unit = new Unit
                {
                    Id = (int)reader["uId"],
                    Type = (string)reader["uType"],
                    Abbreviation = (string)reader["uAb"]
                };

                category = new Category
                {
                    Id = (int)reader["cId"],
                    Name = (string)reader["cName"],
                    UnitId = unit.Id,
                    Unit = unit

                };

                measurement = new Measurement
                {
                    Id = (int)reader["mId"],
                    Category = category,
                    CategoryId = category.Id,
                    Value = Convert.IsDBNull(reader["mValue"]) ? null : Convert.ToSingle(reader["mValue"]),
                    ObservationId = observation.Id,
                    Observation = observation
                };

                vm.Measurements.Add(measurement);

            }

            return vm;
        }

        public Unit GetUnit(Category category)
        {
            string statement = "select * from unit where id = @unitId";
            using var connection = new NpgsqlConnection(connectionString);

            connection.Open();

            using var command = new NpgsqlCommand(statement, connection);


            command.Parameters.AddWithValue("unitId", category.UnitId);

            using var reader = command.ExecuteReader();

            Unit unit = null;

            while (reader.Read())
            {
                unit = new Unit
                {
                    Id = (int)reader["id"],
                    Type = (string)reader["type"],
                    Abbreviation = (string)reader["abbreviation"]
                };

            }

            return unit;
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
                    CategoryId = (int)reader["category_id"],
                    ObservationId = (int)reader["observation_id"]
                };
                listOfMeasurements.Add(measurement);
            }

            return listOfMeasurements;
        }

        public List<Category> GetListOfSubCategories(Category selectedCategory)
        {
            string statement = "select category.id as \"cId\", category.name as \"cName\", category.unit_id as \"uId\" from category WHERE category.basecategory_id = @cId;";
            using var connection = new NpgsqlConnection(connectionString);

            connection.Open();

            using var command = new NpgsqlCommand(statement, connection);



            command.Parameters.AddWithValue("cId", selectedCategory.Id);
            using var reader = command.ExecuteReader();
            var listOfCategories = new List<Category>();

            Category category = null;
            while (reader.Read())
            {
                category = new Category
                {
                    Id = (int)reader["cId"],
                    Name = Convert.IsDBNull(reader["cName"]) ? null : (string?)reader["cName"],
                    UnitId = Convert.IsDBNull(reader["uId"]) ? null : (int?)reader["uId"]

                };

                listOfCategories.Add(category);
            }

            return listOfCategories;
        }

        public List<Category> GetListOfBaseCategories()
        {
            string statement = "select category.id as \"cId\", category.name as \"cName\" from category WHERE category.basecategory_id is null;";
            using var connection = new NpgsqlConnection(connectionString);

            connection.Open();
            Category category = null;
            using var command = new NpgsqlCommand(statement, connection);


            using var reader = command.ExecuteReader();
            var listOfCategories = new List<Category>();

            while (reader.Read())
            {
                category = new Category
                {
                    Id = (int)reader["cId"],
                    Name = Convert.IsDBNull(reader["cName"]) ? null : (string?)reader["cName"]
                };
                listOfCategories.Add(category);
            }

            return listOfCategories;
        }

        public List<Country> GetListOfCountries()
        {
            string statement = "select * from country";
            using var connection = new NpgsqlConnection(connectionString);

            connection.Open();

            using var command = new NpgsqlCommand(statement, connection);

            using var reader = command.ExecuteReader();


            var listOfCountries = new List<Country>();

            Country country = null;

            while (reader.Read())
            {
                country = new Country
                {
                    Id = (int)reader["id"],
                    Name = (string)reader["name"]

                };

                listOfCountries.Add(country);
            }

            return listOfCountries;
        }

        public List<Area> GetListOfArea(Country country)
        {
            string statement = "SELECT area.id as \"aId\", area.name as \"name\", area.country_id as \"countryId\" from area INNER JOIN country ON country_id = country.id WHERE country.id = @cId";
            using var connection = new NpgsqlConnection(connectionString);

            connection.Open();

            using var command = new NpgsqlCommand(statement, connection);

            command.Parameters.AddWithValue("cId", country.Id);
            using var reader = command.ExecuteReader();


            var listOfAreas = new List<Area>();

            Area area = null;

            while (reader.Read())
            {
                area = new Area
                {
                    Id = (int)reader["aId"],
                    Name = (string)reader["name"],
                    CountryId = (int)reader["countryId"],
                    Country = country
                };

                listOfAreas.Add(area);
            }

            return listOfAreas;
        }

        #endregion


        public int UpdateMeasurementWithTransaction(Measurement measurement)
        {

            string statement = "UPDATE measurement SET VALUE = @mValue WHERE id = @mId";
            int result = 0;

            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            var transaction = connection.BeginTransaction();

            try
            {
                using var command = new NpgsqlCommand(statement, connection);



                command.Parameters.AddWithValue("mValue", measurement.Value);
                command.Parameters.AddWithValue("mId", measurement.Id);

                result += command.ExecuteNonQuery();

                
                transaction.Commit(); //bekräfatr att det gått bra typ

                return result;
            }


            catch (PostgresException ex)
            {
                transaction.Rollback();
                string errorCode = ex.SqlState;
                throw new Exception("Uppdateringen misslyckades", ex);


            }


        }



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



                observer = new Observer
                {
                    Id = (int)command.ExecuteScalar(),
                    FirstName = firstName,
                    LastName = lastName
                };



                return observer;
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
        public void AddMeasurement(Measurement measurement, int id)
        {

            string statement = "INSERT INTO measurement (value, observation_id, category_id) VALUES(@value, @oid, @cid) returning id";

            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            using var command = new NpgsqlCommand(statement, connection);



            command.Parameters.AddWithValue("value", measurement.Value);
            command.Parameters.AddWithValue("oid", id);
            command.Parameters.AddWithValue("cid", measurement.CategoryId);


            command.ExecuteNonQuery();






            //int i = 0;

            //foreach (var m in vm.Measurements)
            //{


            //    command.Parameters.AddWithValue("value", vm.Measurements[i].Value);
            //    command.Parameters.AddWithValue("oid", id);
            //    command.Parameters.AddWithValue("cid", vm.Measurements[i].CategoryId);

            //    i++;


            //}


























            // inputValue kommer från värdet användaren skriver in, inputCategory kommer från värdet användaren väljer, observationId kommer när en observation skapas

            // Skicka in datan i databasen, returner measurement och sedan presentera









            //string statement = "INSERT INTO measurement(value, category_id, observation_id) VALUES (@inputValue, @inputCategory, @observationId)";

            //// insert into measurement(value, observation_id, category) values (@inputValue, @observationId, @inputCategory)

            //using var connection = new NpgsqlConnection(connectionString);

            //connection.Open();

            //using var command = new NpgsqlCommand(statement, connection);

            //// 
            //command.Parameters.AddWithValue("inputValue", measurement.Value ?? Convert.DBNull);
            //command.Parameters.AddWithValue("inputCategory", measurement.CategoryId);
            //command.Parameters.AddWithValue("observationId", measurement.ObservationId);

            //using var reader = command.ExecuteReader();

            //return measurement;
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
                string errorCode = ex.SqlState;
                throw new Exception("Du kan inte ta bort en observatör som har gjort en observation", ex);
            }
        }

        #endregion

    }
}
