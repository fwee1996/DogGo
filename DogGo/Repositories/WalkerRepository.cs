using DogGo.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace DogGo.Repositories
{
    public class WalkerRepository : IWalkerRepository
    {
        private readonly IConfiguration _config;

        // The constructor accepts an IConfiguration object as a parameter. This class comes from the ASP.NET framework and is useful for retrieving things out of the appsettings.json file like connection strings.
        public WalkerRepository(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        public List<Walker> GetAllWalkers()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    //instead of SELECT Id, [Name], ImageUrl, NeighborhoodId
                    //FROM Walker
                    cmd.CommandText = @"
                        SELECT w.Id, w.[Name], w.NeighborhoodId, w.ImageUrl, n.[Name] AS NeighborhoodName
                        FROM Walker w 
                        JOIN Neighborhood n ON n.Id=w.NeighborhoodId
                    ";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Walker> walkers = new List<Walker>();
                    while (reader.Read())
                    {
                        Walker walker = new Walker
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            ImageUrl = reader.GetString(reader.GetOrdinal("ImageUrl")),
                            NeighborhoodId = reader.GetInt32(reader.GetOrdinal("NeighborhoodId")),
                            Neighborhood=new Neighborhood
                            {
                                Name = reader.GetString(reader.GetOrdinal("NeighborhoodName")),
                            }
                        };

                        walkers.Add(walker);
                    }

                    reader.Close();

                    return walkers;
                }
            }
        }

        public Walker GetWalkerById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    //instead of SELECT Id, [Name], ImageUrl, NeighborhoodId
                   // FROM Walker
                    cmd.CommandText = @"
                        SELECT w.Id, w.[Name], w.NeighborhoodId, w.ImageUrl, n.[Name] AS NeighborhoodName
                        FROM Walker w 
                        JOIN Neighborhood n ON n.Id=w.NeighborhoodId
                        WHERE w.Id = @id
                    ";

                    cmd.Parameters.AddWithValue("@id", id);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        Walker walker = new Walker
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            ImageUrl = reader.GetString(reader.GetOrdinal("ImageUrl")),
                            NeighborhoodId = reader.GetInt32(reader.GetOrdinal("NeighborhoodId")),
                            Neighborhood = new Neighborhood
                            {
                                Name = reader.GetString(reader.GetOrdinal("NeighborhoodName")), //check SELECT for "{name}"
                            }
                        };

                        reader.Close();
                        return walker;
                    }
                    else
                    {
                        reader.Close();
                        return null;
                    }
                }
            }
        }//changing display of NeighborhoodId to NeighborhoodName now go to Views- Details.cshtml and Index.cshtml

        //added this new method to list  WalkersInNeighborhood so go IWalkerRepository to add method there too!
        public List<Walker> GetWalkersInNeighborhood(int neighborhoodId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                SELECT Id, [Name], ImageUrl, NeighborhoodId
                FROM Walker
                WHERE NeighborhoodId = @neighborhoodId
            ";

                    cmd.Parameters.AddWithValue("@neighborhoodId", neighborhoodId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Walker> walkers = new List<Walker>();
                    while (reader.Read())
                    {
                        Walker walker = new Walker
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            ImageUrl = reader.GetString(reader.GetOrdinal("ImageUrl")),
                            NeighborhoodId = reader.GetInt32(reader.GetOrdinal("NeighborhoodId"))
                        };

                        walkers.Add(walker);
                    }

                    reader.Close();

                    return walkers;
                }
            }
        }



        //added this for profile view: where you can see recent walks -date, client, walk duration and to the right total walk time
        public List<Walks> GetRecentWalksByWalkerId(int walkerId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                SELECT 
                w.Date, 
                o.Name AS ClientName, 
                w.Duration
            FROM Walks w
            JOIN Dog d ON w.DogId = d.Id
            JOIN Owner o ON d.OwnerId = o.Id
            WHERE w.WalkerId = @walkerId
            ORDER BY o.Name";
                    //WHERE w.WalkerId = @walkerId 
                    cmd.Parameters.AddWithValue("@walkerId", walkerId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        var walks = new List<Walks>();
                        while (reader.Read())
                        {
                            walks.Add(new Walks
                            {
                                Date = reader.GetDateTime(0),
                                ClientName = reader.GetString(1),
                                Duration = reader.GetInt32(2)
                            });
                        }
                        return walks;
                    }
                }
            }
        }

        public TimeSpan CalculateTotalWalkTime(int walkerId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                SELECT ISNULL(SUM(Duration), 0) 
                FROM Walks 
                WHERE WalkerId = @walkerId";
                    cmd.Parameters.AddWithValue("@walkerId", walkerId);

                    var result = cmd.ExecuteScalar();

                    // Convert the result to double first and then to TimeSpan
                    double totalMinutes = Convert.ToDouble(result);
                    return TimeSpan.FromMinutes(totalMinutes);
                }
            }
        }





    }
}

