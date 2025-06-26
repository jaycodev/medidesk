using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using medical_appointment_system.Helpers;
using medical_appointment_system.Models;
using medical_appointment_system.Models.ViewModels;

namespace medical_appointment_system.Dao.DaoImpl
{
    public class UserDaoImpl : IGenericDao<User>, IGenericDao<ChangePasswordValidator>
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        string crudCommand = "User_CRUD";

        public int ExecuteWrite(string indicator, User u)
        {
            int process = -1;
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(crudCommand, cn))
                {
                    AddParameters(cmd, indicator, u);

                    try
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
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

        public List<User> ExecuteRead(string indicator, User u)
        {
            List<User> list = new List<User>();

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(crudCommand, cn))
                {
                    AddParameters(cmd, indicator, u);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new User
                            {
                                UserId = reader.SafeGetInt("user_id"),
                                FirstName = reader.SafeGetString("first_name"),
                                LastName = reader.SafeGetString("last_name"),
                                Email = reader.SafeGetString("email"),
                                Phone = reader.SafeGetString("phone"),
                                Roles = reader.SafeGetString("roles")
                                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(r => r.Trim())
                                    .ToList(),
                                ProfilePicture = reader.SafeGetString("profile_picture"),
                                CanDelete = reader.SafeGetBool("can_delete")
                            });
                        }
                    }
                }
            }
            return list;
        }

        public int ExecuteWrite(string indicator, ChangePasswordValidator c)
        {
            int process = -1;
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(crudCommand, cn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@indicator", "UPDATE_PASSWORD");
                    cmd.Parameters.AddWithValue("@user_id", c.UserId);
                    cmd.Parameters.AddWithValue("@current_password", c.CurrentPassword);
                    cmd.Parameters.AddWithValue("@password", c.NewPassword);

                    try
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
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
            cmd.Parameters.AddWithValue("@phone", u.Phone);
            cmd.Parameters.AddWithValue("@roles", string.Join(",", u.Roles));
            cmd.Parameters.AddWithValue("@profile_picture", u.ProfilePicture);
        }

        public List<ChangePasswordValidator> ExecuteRead(string indicator, ChangePasswordValidator entity)
        {
            throw new NotImplementedException();
        }
    }
}