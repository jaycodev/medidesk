using sistema_citas_medicas.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace sistema_citas_medicas.Dao.DaoImpl
{
    public class HorarioDisponibleDaoImpl : IHorarioDisponibleDao
    {
        public int operacionesEscritura(string indicador, HorarioDisponible objHorario)
        {
            string cadenaCon = ConfigurationManager.ConnectionStrings["cnx_bd_citas_medicas"].ConnectionString;
            int procesar;
            using (SqlConnection con = new SqlConnection(cadenaCon))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("usp_horarios_disponibles_crud", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@indicador", indicador);
                    cmd.Parameters.AddWithValue("@id_horario", objHorario.IdHorario);
                    cmd.Parameters.AddWithValue("@id_medico", objHorario.IdMedico);
                    cmd.Parameters.AddWithValue("@dia_semana", objHorario.DiaSemana);
                    cmd.Parameters.AddWithValue("@hora_inicio", objHorario.HoraInicio);
                    cmd.Parameters.AddWithValue("@hora_fin", objHorario.HoraFin);
                    cmd.Parameters.AddWithValue("@habilita", objHorario.Habilita);

                    procesar = cmd.ExecuteNonQuery();
                }
            }
            return procesar;
        }

        public List<HorarioDisponible> operacionesLectura(string indicador, HorarioDisponible objHorario)
        {
            List<HorarioDisponible> lista = new List<HorarioDisponible>();
            string cadena = ConfigurationManager.ConnectionStrings["cnx_bd_citas_medicas"].ConnectionString;

            using (SqlConnection con = new SqlConnection(cadena))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("usp_horarios_disponibles_crud", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Clear();

                    cmd.Parameters.AddWithValue("@indicador", indicador);
                    cmd.Parameters.AddWithValue("@id_horario", objHorario.IdHorario);
                    cmd.Parameters.AddWithValue("@id_medico", objHorario.IdMedico);
                    cmd.Parameters.AddWithValue("@dia_semana", objHorario.DiaSemana);
                    cmd.Parameters.AddWithValue("@hora_inicio", objHorario.HoraInicio);
                    cmd.Parameters.AddWithValue("@hora_fin", objHorario.HoraFin);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            HorarioDisponible horario = new HorarioDisponible()
                            {
                                IdHorario = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                                IdMedico = reader.IsDBNull(1) ? 0 : reader.GetInt32(1),
                                DiaSemana = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                                HoraInicio = reader.IsDBNull(3) ? TimeSpan.Zero : reader.GetTimeSpan(3),
                                HoraFin = reader.IsDBNull(4) ? TimeSpan.Zero : reader.GetTimeSpan(4)
                            };
                            lista.Add(horario);
                        }
                    }
                }
            }
            return lista;
        }
    }
}
