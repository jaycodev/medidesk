using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using sistema_citas_medicas.Models;

namespace sistema_citas_medicas.Dao.DaoImpl
{
    public class CitaDaoImpl : ICitaDao
    {
        public int operacionesEscritura(string indicador, Cita cita)
        {
            string cadenaCon = ConfigurationManager.ConnectionStrings["cnx_bd_citas_medicas"].ConnectionString;
            int procesar;
            using (SqlConnection con = new SqlConnection(cadenaCon))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("usp_citas_crud", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@indicador", indicador);
                    cmd.Parameters.AddWithValue("@id_cita", cita.IdCita);
                    cmd.Parameters.AddWithValue("@id_medico", cita.IdMedico);
                    cmd.Parameters.AddWithValue("@id_paciente", cita.IdPaciente);
                    cmd.Parameters.AddWithValue("@id_especialidad", cita.IdEspecialidad);
                    cmd.Parameters.AddWithValue("@fecha", cita.Fecha);
                    cmd.Parameters.AddWithValue("@hora", cita.Hora);
                    cmd.Parameters.AddWithValue("@tipo_consulta", cita.TipoConsulta);
                    cmd.Parameters.AddWithValue("@sintomas", cita.Sintomas);
                    cmd.Parameters.AddWithValue("@estado", cita.Estado);
                    procesar = cmd.ExecuteNonQuery();
                }
            }
            return procesar;
        }

        public List<Cita> operacionesLectura(string indicador, Cita cita)
        {
            List<Cita> lista = new List<Cita>();
            Cita objcitas;
            string cadena = ConfigurationManager.ConnectionStrings["cnx_bd_citas_medicas"].ConnectionString;

            using (SqlConnection con = new SqlConnection(cadena))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("usp_citas_crud", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Clear();

                    cmd.Parameters.AddWithValue("@indicador", indicador);
                    cmd.Parameters.AddWithValue("@id_cita", cita.IdCita);
                    cmd.Parameters.AddWithValue("@id_medico", cita.IdMedico);
                    cmd.Parameters.AddWithValue("@id_paciente", cita.IdPaciente);
                    cmd.Parameters.AddWithValue("@id_especialidad", cita.IdEspecialidad);
                    cmd.Parameters.AddWithValue("@fecha", cita.Fecha);
                    cmd.Parameters.AddWithValue("@hora", cita.Hora);
                    cmd.Parameters.AddWithValue("@tipo_consulta", cita.TipoConsulta);
                    cmd.Parameters.AddWithValue("@sintomas", cita.Sintomas);
                    cmd.Parameters.AddWithValue("@estado", cita.Estado);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            objcitas = new Cita()
                            {
                                IdCita = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                                IdMedico = reader.IsDBNull(1) ? 0 : reader.GetInt32(1),
                                NomMedico = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                                IdPaciente = reader.IsDBNull(3) ? 0 : reader.GetInt32(3),
                                NomPaciente = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                                IdEspecialidad = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                                NomEspecialidad = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                                Fecha = reader.IsDBNull(7) ? new DateTime(1753, 1, 1) : reader.GetDateTime(7),
                                Hora = reader.IsDBNull(8) ? TimeSpan.Zero : reader.GetTimeSpan(8),
                                TipoConsulta = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
                                Sintomas = reader.IsDBNull(10) ? string.Empty : reader.GetString(10),
                                Estado = reader.IsDBNull(11) ? string.Empty : reader.GetString(11)
                            };
                            lista.Add(objcitas);
                        }
                    }
                }
            }
            return lista;
        }
    }
}