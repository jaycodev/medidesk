using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using sistema_citas_medicas.Models;

namespace sistema_citas_medicas.Dao.DaoImpl
{
    public class PacienteDao : IPacienteDao
    {
        private string cadenaConexion = ConfigurationManager.ConnectionStrings["cnx_bd_citas_medicas"].ToString();

        public void Registrar(Paciente objPaciente)
        {
            using (SqlConnection cn = new SqlConnection(cadenaConexion))
            {
                SqlCommand cmd = new SqlCommand("usp_RegistrarPaciente", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Nombre", objPaciente.Nombre);
                cmd.Parameters.AddWithValue("@Apellido", objPaciente.Apellido);
                cmd.Parameters.AddWithValue("@Correo", objPaciente.Correo);
                cmd.Parameters.AddWithValue("@Contraseña", objPaciente.Contraseña);
                cmd.Parameters.AddWithValue("@Telefono", string.IsNullOrWhiteSpace(objPaciente.Telefono) ? DBNull.Value : (object)objPaciente.Telefono);
                cmd.Parameters.AddWithValue("@FotoPerfil", string.IsNullOrWhiteSpace(objPaciente.FotoPerfil) ? DBNull.Value : (object)objPaciente.FotoPerfil);
                cmd.Parameters.AddWithValue("@FechaNacimiento", objPaciente.FechaNacimiento);
                cmd.Parameters.AddWithValue("@GrupoSanguineo", string.IsNullOrWhiteSpace(objPaciente.GrupoSanguineo) ? DBNull.Value : (object)objPaciente.GrupoSanguineo);

                var paramIdUsuario = new SqlParameter("@IdUsuario", SqlDbType.Int);
                paramIdUsuario.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(paramIdUsuario);

                cn.Open();
                cmd.ExecuteNonQuery();

                objPaciente.IdUsuario = (int)paramIdUsuario.Value;
            }
        }

        public List<Paciente> Listar()
        {
            var lista = new List<Paciente>();

            using (SqlConnection cn = new SqlConnection(cadenaConexion))
            {
                SqlCommand cmd = new SqlCommand("usp_ListarPacientes", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var objPaciente = new Paciente
                        {
                            IdUsuario = Convert.ToInt32(dr["id_usuario"]),
                            Nombre = dr["nombre"].ToString(),
                            Apellido = dr["apellido"].ToString(),
                            Correo = dr["correo"].ToString(),
                            Contraseña = dr["contraseña"].ToString(),
                            Telefono = dr["telefono"] == DBNull.Value ? null : dr["telefono"].ToString(),
                            FotoPerfil = dr["foto_perfil"] == DBNull.Value ? null : dr["foto_perfil"].ToString(),
                            FechaNacimiento = dr["fecha_nacimiento"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dr["fecha_nacimiento"]),
                            GrupoSanguineo = dr["grupo_sanguineo"] == DBNull.Value ? null : dr["grupo_sanguineo"].ToString()
                        };
                        lista.Add(objPaciente);
                    }
                }
            }
            return lista;
        }
    }
}