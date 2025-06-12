using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using sistema_citas_medicas.Helpers;
using sistema_citas_medicas.Models;

namespace sistema_citas_medicas.Dao.DaoImpl
{
    public class PacienteDao : IPacienteDao
    {
        public int operacionesEscritura(string indicador, Paciente objPaciente)
        {
            string cadena = ConfigurationManager.ConnectionStrings["cnx_bd_citas_medicas"].ConnectionString;
            int procesar = -1;

            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();

                using (SqlCommand cmd = new SqlCommand("usp_pacientes_crud", cn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@indicador", indicador);
                    cmd.Parameters.AddWithValue("@id_usuario", objPaciente.IdUsuario);
                    cmd.Parameters.AddWithValue("@nombre", objPaciente.Nombre);
                    cmd.Parameters.AddWithValue("@apellido", objPaciente.Apellido);
                    cmd.Parameters.AddWithValue("@correo", objPaciente.Correo);
                    cmd.Parameters.AddWithValue("@contraseña", objPaciente.Contraseña);
                    cmd.Parameters.AddWithValue("@telefono", objPaciente.Telefono);
                    cmd.Parameters.AddWithValue("@foto_perfil", objPaciente.FotoPerfil);
                    cmd.Parameters.AddWithValue("@fecha_nacimiento", objPaciente.FechaNacimiento);
                    cmd.Parameters.AddWithValue("@grupo_sanguineo", objPaciente.GrupoSanguineo);

                    try
                    {
                        procesar = cmd.ExecuteNonQuery();
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
            return procesar;
        }

        public List<Paciente> operacionesLectura(string indicador, Paciente objPaciente)
        {
            List<Paciente> lista = new List<Paciente>();
            string cadena = ConfigurationManager.ConnectionStrings["cnx_bd_citas_medicas"].ConnectionString;

            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();

                using (SqlCommand cmd = new SqlCommand("usp_pacientes_crud", cn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@indicador", indicador);
                    cmd.Parameters.AddWithValue("@id_usuario", objPaciente.IdUsuario);
                    cmd.Parameters.AddWithValue("@nombre", objPaciente.Nombre);
                    cmd.Parameters.AddWithValue("@apellido", objPaciente.Apellido);
                    cmd.Parameters.AddWithValue("@correo", objPaciente.Correo);
                    cmd.Parameters.AddWithValue("@contraseña", objPaciente.Contraseña);
                    cmd.Parameters.AddWithValue("@telefono", objPaciente.Telefono);
                    cmd.Parameters.AddWithValue("@foto_perfil", objPaciente.FotoPerfil);
                    cmd.Parameters.AddWithValue("@fecha_nacimiento", objPaciente.FechaNacimiento);
                    cmd.Parameters.AddWithValue("@grupo_sanguineo", objPaciente.GrupoSanguineo);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Paciente
                            {
                                IdUsuario = reader["id_usuario"] as int? ?? 0,
                                Nombre = reader["nombre"]?.ToString() ?? string.Empty,
                                Apellido = reader["apellido"]?.ToString() ?? string.Empty,
                                Correo = reader.HasColumn("correo") ? reader["correo"]?.ToString() ?? string.Empty : string.Empty,
                                Telefono = reader.HasColumn("telefono") ? reader["telefono"]?.ToString() ?? string.Empty : string.Empty,
                                FotoPerfil = reader["foto_perfil"]?.ToString() ?? string.Empty,
                                FechaNacimiento = reader["fecha_nacimiento"] as DateTime? ?? DateTime.MinValue,
                                GrupoSanguineo = reader["grupo_sanguineo"]?.ToString() ?? string.Empty
                            });
                        }
                    }
                }
            }
            return lista;
        }
    }
}