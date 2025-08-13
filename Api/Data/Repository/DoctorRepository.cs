using Api.Data.Contract;
using Api.Helpers;
using Api.Models;
using Microsoft.Data.SqlClient;

namespace Api.Data.Repository
{
    public class DoctorRepository : BaseRepository, IGenericContract<Doctor>
    {
        string crudCommand = "Doctor_CRUD";

        public DoctorRepository(IConfiguration configuration) : base(configuration) { }

        public List<Doctor> ExecuteRead(string indicator, Doctor d)
        {
            List<Doctor> list = new List<Doctor>();

            using (var cn = GetConnection())
            {
                cn.Open();
                using (var cmd = new SqlCommand(crudCommand, cn))
                {
                    AddParameters(cmd, indicator, d);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Doctor
                            {
                                UserId = reader.SafeGetInt("user_id"),
                                FirstName = reader.SafeGetString("first_name"),
                                LastName = reader.SafeGetString("last_name"),
                                Email = reader.SafeGetString("email"),
                                Phone = reader.SafeGetString("phone"),
                                ProfilePicture = reader.SafeGetString("profile_picture"),
                                SpecialtyId = reader.SafeGetInt("specialty_id"),
                                SpecialtyName = reader.SafeGetString("specialty_name"),
                                Status = reader.SafeGetBool("status")
                            });
                        }
                    }
                }
            }
            return list;
        }

        public int ExecuteWrite(string indicator, Doctor d)
        {
            int process = -1;
            using (var cn = GetConnection())
            {
                cn.Open();
                using (var cmd = new SqlCommand(crudCommand, cn))
                {
                    AddParameters(cmd, indicator, d);

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

        private void AddParameters(SqlCommand cmd, string indicator, Doctor d)
        {
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@indicator", indicator);
            cmd.Parameters.AddWithValue("@user_id", d.UserId);
            cmd.Parameters.AddWithValue("@first_name", d.FirstName);
            cmd.Parameters.AddWithValue("@last_name", d.LastName);
            cmd.Parameters.AddWithValue("@email", d.Email);
            cmd.Parameters.AddWithValue("@password", d.Password);
            cmd.Parameters.AddWithValue("@phone", d.Phone);
            cmd.Parameters.AddWithValue("@profile_picture", d.ProfilePicture);
            cmd.Parameters.AddWithValue("@specialty_id", d.SpecialtyId);
            cmd.Parameters.AddWithValue("@status", d.Status);
        }
    }
}
