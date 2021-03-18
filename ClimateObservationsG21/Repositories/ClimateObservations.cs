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

        #region Create

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


        #endregion

    }
}
