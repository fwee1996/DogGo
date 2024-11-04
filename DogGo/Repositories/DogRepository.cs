using DogGo.Models;
using Humanizer;
using Microsoft.Data.SqlClient;
using System;

namespace DogGo.Repositories
{
    public class DogRepository : IDogRepository
    {
        private readonly IConfiguration _config;

        // The constructor accepts an IConfiguration object as a parameter. This class comes from the ASP.NET framework and is useful for retrieving things out of the appsettings.json file like connection strings.
        public DogRepository(IConfiguration config)
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

        public List<Dog> GetAllDogs()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    //instead of SELECT Id, [Name], ImageUrl, OwnerId
                    //FROM Dog

                    //SELECT d.Id, d.[Name], d.Breed, d.Notes, d.ImageUrl, n.[Name] AS OwnerName
                    //FROM Dog d
                    //JOIN Owner n ON n.Id = w.OwnerId
                    cmd.CommandText = @"
                        SELECT Id, [Name], Breed, Notes, ImageUrl, OwnerId
                        FROM Dog
                    ";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Dog> dogs = new List<Dog>();
                    while (reader.Read())
                    {
                        Dog dog = new Dog
                        {
                            // Read the Id directly as it is expected to be non-nullable
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            // Check if the 'Name' column is DBNull. If it is, assign null to the property.
                            // Otherwise, read the string value from the database.
                            //System.Data.SqlTypes.SqlNullValueException: 'Data is Null. Columns (Name, Breed, Notes, ImageUrl) might contain NULL values in the database, IsDBNull method provided by SqlDataReader to check for NULL values.
                            Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? null : reader.GetString(reader.GetOrdinal("Name")),
                            Breed = reader.IsDBNull(reader.GetOrdinal("Breed")) ? null : reader.GetString(reader.GetOrdinal("Breed")),
                            Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                            ImageUrl = reader.IsDBNull(reader.GetOrdinal("ImageUrl")) ? null : reader.GetString(reader.GetOrdinal("ImageUrl")),
                            // Read the OwnerId directly as it is expected to be non-nullable
                            OwnerId = reader.GetInt32(reader.GetOrdinal("OwnerId"))
                        };

                        dogs.Add(dog);
                    }

                    reader.Close();

                    return dogs;
                }
            }
        }

        public Dog GetDogById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    //instead of SELECT Id, [Name], ImageUrl, OwnerId
                    // FROM Dog


                    //SELECT w.Id, w.[Name], w.OwnerId, w.ImageUrl, n.[Name] AS OwnerName
                    //FROM Dog w
                    //JOIN Owner n ON n.Id = w.OwnerId
                    cmd.CommandText = @"
                        SELECT Id, [Name], Breed, Notes, ImageUrl, OwnerId
                        FROM Dog
                        WHERE Id = @id
                    ";

                    cmd.Parameters.AddWithValue("@id", id);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        Dog dog = new Dog
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? null : reader.GetString(reader.GetOrdinal("Name")),
                            Breed = reader.IsDBNull(reader.GetOrdinal("Breed")) ? null : reader.GetString(reader.GetOrdinal("Breed")),
                            Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                            ImageUrl = reader.IsDBNull(reader.GetOrdinal("ImageUrl")) ? null : reader.GetString(reader.GetOrdinal("ImageUrl")),
                            OwnerId = reader.GetInt32(reader.GetOrdinal("OwnerId"))
                        };

                        reader.Close();
                        return dog;
                    }
                    else
                    {
                        reader.Close();
                        return null;
                    }
                }
            }
        }


        public void AddDog(Dog dog)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    INSERT INTO Dog ([Name], Breed, Notes, ImageUrl, OwnerId)
                    OUTPUT INSERTED.ID
                    VALUES (@name, @breed, @notes, @imageUrl, @ownerId);
                ";

                    // Use DBNull.Value for null parameters
                   // (object)cast: This cast is necessary to avoid issues with the nullable value. The AddWithValue method expects an object type, and casting dog.Name to object ensures that the DBNull.Value assignment works correctly.
                    cmd.Parameters.AddWithValue("@name", (object)dog.Name ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@breed", (object)dog.Breed ?? DBNull.Value); //DBNull.Value to represent a database null value.
                    cmd.Parameters.AddWithValue("@notes", (object)dog.Notes ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@imageUrl", (object)dog.ImageUrl ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ownerId", dog.OwnerId); // OwnerId is assumed to be non-nullable

                    int id = (int)cmd.ExecuteScalar();

                    dog.Id = id;
                }
            }
        }

        //Module version:
        //            cmd.Parameters.AddWithValue("@name", dog.Name);
        //            cmd.Parameters.AddWithValue("@breed", dog.Breed);
        //            cmd.Parameters.AddWithValue("@ownerId", dog.OwnerId);

        //            // nullable columns
        //            if (dog.Notes == null)
        //            {
        //                cmd.Parameters.AddWithValue("@notes", DBNull.Value);
        //            }
        //            else
        //            {
        //                cmd.Parameters.AddWithValue("@notes", dog.Notes);
        //            }

        //            if (dog.ImageUrl == null)
        //            {
        //                cmd.Parameters.AddWithValue("@imageUrl", DBNull.Value);
        //            }
        //            else
        //            {
        //                cmd.Parameters.AddWithValue("@imageUrl", dog.ImageUrl);
        //            }


        //            int newlyCreatedId = (int)cmd.ExecuteScalar();

        //            dog.Id = newlyCreatedId;

        //        }
        //    }
        //}

        public void UpdateDog(Dog dog)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // Make sure to check spellings & case
                    cmd.CommandText = @" 
                            UPDATE Dog
                            SET 
                                [Name] = @name, 
                                Breed = @breed, 
                                ImageUrl = @imageUrl, 
                                Notes = @notes, 
                                OwnerId = @ownerId
                            WHERE Id = @id";

                    // Use DBNull.Value for null parameters
                    cmd.Parameters.AddWithValue("@name", (object)dog.Name ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@breed", (object)dog.Breed ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@imageUrl", (object)dog.ImageUrl ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@notes", (object)dog.Notes ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ownerId", dog.OwnerId); // OwnerId is assumed to be non-nullable
                    cmd.Parameters.AddWithValue("@id", dog.Id); // Id is used to locate the record

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteDog(int dogId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                            DELETE FROM Dog
                            WHERE Id = @id
                        ";

                    cmd.Parameters.AddWithValue("@id", dogId);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        //added this Chapter 3 
        //dont forget to add this new method to the IDogRepository
        public List<Dog> GetDogsByOwnerId(int ownerId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                SELECT Id, Name, Breed, Notes, ImageUrl, OwnerId 
                FROM Dog
                WHERE OwnerId = @ownerId
            ";

                    cmd.Parameters.AddWithValue("@ownerId", ownerId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Dog> dogs = new List<Dog>();

                    while (reader.Read())
                    {
                        Dog dog = new Dog()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Breed = reader.GetString(reader.GetOrdinal("Breed")),
                            OwnerId = reader.GetInt32(reader.GetOrdinal("OwnerId"))
                        };

                        // Check if optional columns are null
                        if (reader.IsDBNull(reader.GetOrdinal("Notes")) == false)
                        {
                            dog.Notes = reader.GetString(reader.GetOrdinal("Notes"));
                        }
                        if (reader.IsDBNull(reader.GetOrdinal("ImageUrl")) == false)
                        {
                            dog.ImageUrl = reader.GetString(reader.GetOrdinal("ImageUrl"));
                        }

                        dogs.Add(dog);
                    }
                    reader.Close();
                    return dogs;
                }
            }
        }
    }
}
