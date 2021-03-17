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

        #region

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

        #endregion

    }
}
