//using DogGo.Models;
//using Microsoft.Data.SqlClient;
//using Microsoft.Extensions.Configuration;
//using System.Collections.Generic;

//namespace DogGo.Repositories
//{
//    public class WalkRepository : IWalkRepository
//    {
//        private readonly IConfiguration _config;

//        public WalkRepository(IConfiguration config)
//        {
//            _config = config;
//        }

//        public SqlConnection Connection
//        {
//            get
//            {
//                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
//            }
//        }

//        public Walks AddWalk(Walks walk)
//        {
//            using (SqlConnection conn = Connection)
//            {
//                conn.Open();
//                using (SqlCommand cmd = conn.CreateCommand())
//                {
//                    cmd.CommandText = @"
//                INSERT INTO Walk (Date, Duration, WalkerId, DogId, Status)
//                OUTPUT INSERTED.ID
//                VALUES (@date, @duration, @walkerId, @dogId, @status);
//            ";

//                    // Add parameters
//                    cmd.Parameters.AddWithValue("@date", walk.Date);
//                    cmd.Parameters.AddWithValue("@duration", walk.Duration);
//                    cmd.Parameters.AddWithValue("@walkerId", walk.WalkerId);
//                    cmd.Parameters.AddWithValue("@dogId", walk.DogId);
//                    cmd.Parameters.AddWithValue("@status", walk.Status);

//                    // Execute and retrieve the ID of the inserted record
//                    int id = (int)cmd.ExecuteScalar();

//                    // Set the ID of the walk object
//                    walk.Id = id;
//                }
//            }
//            return walk; // Return the walk object with the new ID
//        }
//    }
//}
