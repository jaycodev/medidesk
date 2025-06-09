using sistema_citas_medicas.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace sistema_citas_medicas.Dao.DaoImpl
{
    public class PacienteDao : IPacienteDao
    {
        private string cadenaConexion = ConfigurationManager.ConnectionStrings["cnx_bd_citas_medicas"].ToString();

        public void Registrar(PacienteViewModel model)
        {
            using (SqlConnection cn = new SqlConnection(cadenaConexion))
            {
                SqlCommand cmd = new SqlCommand("usp_RegistrarPaciente", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Nombre", model.Nombre);
                cmd.Parameters.AddWithValue("@Apellido", model.Apellido);
                cmd.Parameters.AddWithValue("@Correo", model.Correo);
                cmd.Parameters.AddWithValue("@Contraseña", model.Contraseña);
                cmd.Parameters.AddWithValue("@Telefono", model.Telefono ?? "");
                cmd.Parameters.AddWithValue("@FotoPerfil", model.FotoPerfil ?? "");
                cmd.Parameters.AddWithValue("@FechaNacimiento", model.FechaNacimiento);
                cmd.Parameters.AddWithValue("@GrupoSanguineo", model.GrupoSanguineo);
                cmd.Parameters.AddWithValue("@IdUsuario", model.IdUsuario);

                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public List<PacienteViewModel> Listar()
        {
            var lista = new List<PacienteViewModel>();

            using (SqlConnection cn = new SqlConnection(cadenaConexion))
            {
                SqlCommand cmd = new SqlCommand("usp_ListarPacientes", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var paciente = new PacienteViewModel
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
                        lista.Add(paciente);
                    }
                }
            }
            return lista;
        }


    }
}