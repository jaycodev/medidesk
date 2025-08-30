using Api.Domains.Patients.DTOs;
using Api.Domains.Patients.Models;
using Api.Domains.Patients.Repositories;
using Api.Helpers;
using Microsoft.Data.SqlClient;

namespace Api.Data.Repository
{
    public class PatientRepository : BaseRepository, IPatient
    {
        public PatientRepository(IConfiguration configuration) : base(configuration) { }

        string crudCommand = "Patient_CRUD";

        public List<Patient> GetList()
        {
            List<Patient> list = new List<Patient>();

            using (SqlConnection cn = GetConnection())
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(crudCommand, cn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@indicator", "GET_ALL");

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Patient
                            {
                                UserId = reader.SafeGetInt("user_id"),
                                FirstName = reader.SafeGetString("first_name"),
                                LastName = reader.SafeGetString("last_name"),
                                Email = reader.SafeGetString("email"),
                                Phone = reader.SafeGetString("phone"),
                                ProfilePicture = reader.SafeGetString("profile_picture"),
                                BirthDate = reader.SafeGetDateTime("birth_date"),
                                BloodType = reader.SafeGetString("blood_type")
                            });
                        }
                    }
                }
            }
            return list;
        }

        public Patient GetById(int id)
        {
            Patient patient = null;

            using (SqlConnection cn = GetConnection())
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(crudCommand, cn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@indicator", "GET_BY_ID");
                    cmd.Parameters.AddWithValue("@user_id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            patient = (new Patient
                            {
                                UserId = reader.SafeGetInt("user_id"),
                                FirstName = reader.SafeGetString("first_name"),
                                LastName = reader.SafeGetString("last_name"),
                                Email = reader.SafeGetString("email"),
                                Phone = reader.SafeGetString("phone"),
                                ProfilePicture = reader.SafeGetString("profile_picture"),
                                BirthDate = reader.SafeGetDateTime("birth_date"),
                                BloodType = reader.SafeGetString("blood_type")
                            });
                        }
                    }
                }
            }
            return patient;
        }

        public int Create(PatientCreateDTO dto)
        {
            int process = -1;
            using (SqlConnection cn = GetConnection())
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(crudCommand, cn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@indicator", "INSERT");
                    cmd.Parameters.AddWithValue("@first_name", dto.FirstName);
                    cmd.Parameters.AddWithValue("@last_name", dto.LastName);
                    cmd.Parameters.AddWithValue("@email", dto.Email);
                    cmd.Parameters.AddWithValue("@password", dto.Password);
                    cmd.Parameters.AddWithValue("@phone", dto.Phone);
                    cmd.Parameters.AddWithValue("@birth_date", dto.BirthDate);
                    cmd.Parameters.AddWithValue("@blood_type", dto.BloodType);

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

        public int Update(int id,PatientUpdateDTO dto)
        {
            int process = -1;
            using (SqlConnection cn = GetConnection())
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(crudCommand, cn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@indicator", "UPDATE");
                    cmd.Parameters.AddWithValue("@user_id", id);
                    cmd.Parameters.AddWithValue("@birth_date", dto.BirthDate);
                    cmd.Parameters.AddWithValue("@blood_type", dto.BloodType);

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
                return process;
            }
        }
    }
}
