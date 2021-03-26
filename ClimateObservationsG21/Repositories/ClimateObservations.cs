using Npgsql;
using System;
using System.Collections.Generic;


namespace ClimateObservationsG21
{
    class ClimateObservations
    {
        private static readonly string connectionString = "Server=localhost;Port=5432;Database=Climate_Observations_G21; User ID= fredrik; Password=Hemligt";
        //private static readonly string connectionString = "Server=studentpsql.miun.se;Port=5432; Database=environmentDb;User Id=envUser;Password=FridaysForFuture;SslMode=Prefer; Trust Server Certificate=true";

        #region ViewModel Methods
        /// <summary>
        /// Skapar en ny observation med vymodellens värden
        /// </summary>
        /// <param name="vm"></param>
        public Observation AddViewModel(NewObservationViewModel vm)
        {
            string statement = "insert into observation (geolocation_id, date, observer_id) values (@gId, @date, @oId) returning id";
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            var transaction = connection.BeginTransaction(); //transaktionen 
            try
            {
                using var command = new NpgsqlCommand(statement, connection);
                command.Parameters.AddWithValue("gId", vm.Geolocation.Id);
                command.Parameters.AddWithValue("date", vm.Date);
                command.Parameters.AddWithValue("oId", vm.Observer.Id);
                int observationId = (int)command.ExecuteScalar();

                Observation observation = new Observation
                {
                    Id = observationId,
                    Date = vm.Date,
                    Observer = vm.Observer,
                    Geolocation = vm.Geolocation
                };
                transaction.Commit();

                for (int i = 0; i < vm.Measurements.Count; i++)
                {
                    AddMeasurement(vm.Measurements[i], observationId);
                }
                return observation;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                if (ex is PostgresException || ex is NullReferenceException)
                {
                    throw new Exception("Det gick inte att lägga till observationen i databasen");
                }
                throw;
            }
        }

        /// <summary>
        /// Gets viewmodel for the observation
        /// </summary>
        /// <param name="observation"></param>
        /// <returns>viewmodel</returns>
        public NewObservationViewModel GetViewModel(Observation observation)
        {
            Category basecategory = null;
            Category category = null;
            Unit unit = null;
            Measurement measurement = null;
            NewObservationViewModel vm = new NewObservationViewModel();
            
            // Försökte dela statement på fler rader för att kunna läsa det enklare, men lyckades inte få till det så du får leva med att scrolla åt höger. 
            string statement = "SELECT measurement.id as \"mId\", measurement.value as \"mValue\", a.id as \"cId\", a.name as \"cName\", b.name as \"bcName\", b.id as \"bcId\", unit.id as \"uId\", unit.type as \"uType\", unit.abbreviation as \"uAb\" FROM measurement LEFT JOIN category a on category_id = a.id JOIN category b on b.id = a.basecategory_id LEFT JOIN unit on a.unit_id = unit.id WHERE measurement.observation_id = @observationId;";
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            try
            {    
                using var command = new NpgsqlCommand(statement, connection);
                command.Parameters.AddWithValue("observationId", observation.Id);
                using var reader = command.ExecuteReader();
                vm.Date = observation.Date;
                vm.Observer = observation.Observer;
                vm.Geolocation = GetGeolocation(observation);

                while (reader.Read())
                {
                    unit = new Unit
                    {
                        Id = (int)reader["uId"],
                        Type = (string)reader["uType"],
                        Abbreviation = (string)reader["uAb"]
                    };

                    basecategory = new Category
                    {
                        Id = (int)reader["bcId"],
                        Name = (string)reader["bcName"]
                    };

                    category = new Category
                    {
                        Id = (int)reader["cId"],
                        Name = (string)reader["cName"],
                        Unit = unit,
                        BaseCategory = basecategory
                    };

                    measurement = new Measurement
                    {
                        Id = (int)reader["mId"],
                        Category = category,
                        Value = Convert.IsDBNull(reader["mValue"]) ? null : Convert.ToSingle(reader["mValue"]),
                        Observation = observation
                    };
                    vm.Measurements.Add(measurement);
                }
                return vm;
            }
            catch (Exception ex)
            {
                if (ex is PostgresException || ex is NullReferenceException)
                {
                    throw new Exception("Det gick inte att hämta information om observationen", ex);
                }
                throw;
            }
        }

        #endregion

        #region Get Methods

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
            try
            {
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
                        Observer = observer,
                    };
                    observation.Geolocation = GetGeolocation(observation);
                    listOfObservations.Add(observation);
                }
                return listOfObservations;
            }
            catch (PostgresException ex)
            {
                throw new Exception("Kunde inte hämta observationer från databas", ex);
            }
        }

        /// <summary>
        /// Gets a list of observers from database
        /// </summary>
        /// <returns>List<Observer></returns>
        public List<Observer> GetListOfObservers()
        {
            string statement = "select * from observer order by lastname desc";
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            using var command = new NpgsqlCommand(statement, connection);
            try
            {
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
            catch (PostgresException ex)
            {
                throw new Exception("Det gick inte att hämta listan med observatörer", ex);
            }
        }

        /// <summary>
        /// Gets a list of subcategories from database
        /// </summary>
        /// <param name="selectedCategory"></param>
        /// <returns></returns>
        /// 
        public List<Category> GetListOfSubCategories(Category selectedCategory)
        {
            string statement = "select category.id as \"cId\", category.name as \"cName\", unit.id as \"uId\", unit.type as \"uType\", unit.abbreviation as \"uAb\" from category INNER JOIN unit on category.unit_id = unit.id WHERE category.basecategory_id = @cId;";
            try
            {
                using var connection = new NpgsqlConnection(connectionString);
                connection.Open();
                using var command = new NpgsqlCommand(statement, connection);
                command.Parameters.AddWithValue("cId", selectedCategory.Id);
                using var reader = command.ExecuteReader();
                var listOfCategories = new List<Category>();
                Category category = null;
                Unit unit = null;

                while (reader.Read())
                {
                    unit = new Unit
                    {
                        Id = (int)reader["uId"],
                        Type = (string)reader["uType"],
                        Abbreviation = (string)reader["uAb"]
                    };
                    category = new Category
                    {
                        Id = (int)reader["cId"],
                        Name = Convert.IsDBNull(reader["cName"]) ? null : (string?)reader["cName"],
                        Unit = unit
                    };
                    listOfCategories.Add(category);
                }
                return listOfCategories;
            }
            catch (Exception ex)
            {
                if (ex is NullReferenceException || ex is PostgresException)
                {
                    throw new Exception("Det gick inte att hämta lista med underkategorier", ex);
                }
                throw new Exception("Ett fel har inträffat, testa starta om");
            }
        }

        /// <summary>
        /// Gets a list of basecategories from database
        /// </summary>
        /// <returns></returns>
        public List<Category> GetListOfBaseCategories()
        {
            Category category = null;
            string statement = "select category.id as \"cId\", category.name as \"cName\" from category WHERE category.basecategory_id is null;";
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            try
            {
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
            catch (PostgresException ex)
            {
                throw new Exception("Det gick inte att hämta listan med huvudkategorier");
            }
        }

        /// <summary>
        /// Gets the geolocation for inserted observation
        /// </summary>
        /// <param name="observation"></param>
        /// <returns></returns>
        public Geolocation GetGeolocation(Observation observation)
        {
            Geolocation geolocation = null;
            Area area = null;
            Country country = null;
            string statement = "SELECT geolocation.id as \"gId\", area.id as \"aId\", area.name as \"aName\", country.id as \"cId\", country.name as \"cName\" FROM observation LEFT JOIN geolocation on observation.geolocation_id = geolocation.id LEFT JOIN area on geolocation.area_id = area.id LEFT JOIN country on area.country_id = country.id WHERE observation.id = @observationId;";
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            try
            {
                using var command = new NpgsqlCommand(statement, connection);
                command.Parameters.AddWithValue("observationId", observation.Id);
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    country = new Country
                    {
                        Id = (int)reader["cId"],
                        Name = (string)reader["cName"]
                    };
                    area = new Area
                    {
                        Id = (int)reader["aId"],
                        Name = (string)reader["aName"],
                        Country = country
                    };
                    geolocation = new Geolocation
                    {
                        Id = (int)reader["gId"],
                        Area = area
                    };
                }
                return geolocation;
            }
            catch (Exception ex)
            {
                if (ex is PostgresException || ex is NullReferenceException)
                {
                    throw new Exception("Det gick inte att hämta lokaliseringen", ex);
                }
                throw new Exception("Ett fel har inträffat, testa att starta om");
            }
        }
            
        /// <summary>
        /// Gets a list of geolocations from database
        /// </summary>
        /// <returns></returns>
        public List<Geolocation> GetListOfGeolocations()
        {
            List<Geolocation> geolocations = new List<Geolocation>();
            Geolocation geolocation = null;
            Area area = null;
            Country country = null; 

            string statement = "SELECT geolocation.id as \"gId\", area.id as \"aId\", area.name as \"aName\", country.id as \"cId\", country.name as \"cName\" FROM geolocation INNER JOIN area on geolocation.area_id = area.id INNER JOIN country on area.country_id = country.id";
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            using var command = new NpgsqlCommand(statement, connection);

            try
            {    
                using var reader = command.ExecuteReader();   
                while (reader.Read())
                {
                    country = new Country
                    {
                        Id = (int)reader["cId"],
                        Name = (string)reader["cName"]
                    };
                    area = new Area
                    {
                        Id = (int)reader["aId"],
                        Name = (string)reader["aName"],
                        Country = country
                    };
                    geolocation = new Geolocation
                    {
                        Id = (int)reader["gId"],
                        Area = area
                    };
                    geolocations.Add(geolocation);
                }
                return geolocations;
            }
            catch (PostgresException ex)
            {
                throw new Exception("Kunde inte hämta lokationerna från databasen", ex);
            }
        }

        #endregion

        #region Add, Remove and Update methods

        /// <summary>
        /// Adds an observer to database
        /// </summary>
        /// <param name="observer"></param>
        /// <returns>observer</returns>
        public Observer AddObserver(string firstName, string lastName)
        {
            string statement = "insert into observer(firstname, lastname) values (@firstname, @lastname) returning id";
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            using var command = new NpgsqlCommand(statement, connection);
            try
            {
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
            catch (Exception ex)
            {
                if (ex is PostgresException || ex is NullReferenceException)
                {
                    throw new Exception("Du måste ange både för- och efternamn", ex);
                }
                throw new Exception("Ett fel har inträffat, testa att starta om");
            }
        }

        //Kolla på denna, går det att ha en Observation som inparameter istället?
        /// <summary>
        /// Adds measurement to database
        /// </summary>
        /// <param name="measurement"></param>
        /// <returns>measurement</returns>
        public void AddMeasurement(Measurement measurement, int observationid)
        {
            string statement = "INSERT INTO measurement (value, observation_id, category_id) VALUES(@value, @oid, @cid) returning id";
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            using var command = new NpgsqlCommand(statement, connection);
            
            try
            {
                command.Parameters.AddWithValue("value", measurement.Value);
                command.Parameters.AddWithValue("oid", observationid);
                command.Parameters.AddWithValue("cid", measurement.Category.Id);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                if (ex is PostgresException || ex is NullReferenceException)
                {
                    throw new Exception("Det gick inte att lägga till mätpunkter i databasen");
                }
                throw new Exception("Ett fel har inträffat, testa att starta om");
            }
        }

        /// <summary>
        /// Removes observer from database
        /// </summary>
        /// <param name="id"></param>
        public void RemoveObserver(Observer observer)
        {
            string statement = "delete from observer where id = @id";
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            
            try
            {
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

        /// <summary>
        /// Update measurement 
        /// </summary>
        /// <param name="measurement"></param>
        /// <returns></returns>
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
                transaction.Commit(); 
                return result;
            }
            catch (PostgresException ex)
            {
                transaction.Rollback();
                string errorCode = ex.SqlState;
                throw new Exception("Uppdateringen misslyckades", ex);
            }
        }
        #endregion
    }
}
