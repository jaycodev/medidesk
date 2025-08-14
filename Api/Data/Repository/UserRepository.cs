using Api.Data.Contract;
using Api.Helpers;
using Api.Models;
using Api.Models.DTOS;
using Microsoft.Data.SqlClient;

namespace Api.Data.Repository
{
    public class UserRepository : BaseRepository, IGenericContract<User>
    {
        string crudCommand = "User_CRUD";

        public UserRepository(IConfiguration configuration) : base(configuration){}

        public List<User> ExecuteRead(string indicator, User u)
        {
            List<User> list = new List<User>();

            using (var con = GetConnection())
            {
                con.Open();
                using (var cmd = new SqlCommand(crudCommand, con))
                {
                    AddParameters(cmd, indicator, u);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new User()
                            {
                                UserId = reader.SafeGetInt("user_id"),
                                FirstName = reader.SafeGetString("first_name"),
                                LastName = reader.SafeGetString("last_name"),
                                Email = reader.SafeGetString("email"),
                                Phone = reader.SafeGetString("phone"),
                                ActiveRole = reader.SafeGetString("roles"),
                                ProfilePicture = reader.SafeGetString("profile_picture"),
                            });
                        }
                    }
                }
            }
            return list;
        }

        public int ExecuteWrite(string indicator, User u)
        {
            int process = -1;
            using (var cn = GetConnection())
            {
                cn.Open();
                using (var cmd = new SqlCommand(crudCommand, cn))
                {
                    AddParameters(cmd, indicator, u);

                    try
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                                process = Convert.ToInt32(reader["affected_rows"]);
                        }
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == 50000)
                        {
                            throw new ApplicationException(ex.Message);
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
            }
            return process;
        }

        private void AddParameters(SqlCommand cmd, string indicator, User u)
        {
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@indicator", indicator);
            cmd.Parameters.AddWithValue("@user_id", u.UserId);
            cmd.Parameters.AddWithValue("@first_name", u.FirstName);
            cmd.Parameters.AddWithValue("@last_name", u.LastName);
            cmd.Parameters.AddWithValue("@email", u.Email);
            cmd.Parameters.AddWithValue("@password", u.Password);
            cmd.Parameters.AddWithValue("@current_password", u.currentPassword);
            cmd.Parameters.AddWithValue("@phone", u.Phone);
            cmd.Parameters.AddWithValue("@roles", string.Join(",", u.Roles));
            cmd.Parameters.AddWithValue("@profile_picture", u.ProfilePicture);
        }

    }
}
