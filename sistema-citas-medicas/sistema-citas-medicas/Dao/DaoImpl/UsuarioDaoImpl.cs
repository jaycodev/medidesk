using sistema_citas_medicas.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace sistema_citas_medicas.Dao.DaoImpl
{
    public class UsuarioDaoImpl : IUsuarioDao
    {

        public int operacionesEscritura(string indicador, Usuario objUsario)
        {
            string cadenaCon = ConfigurationManager.ConnectionStrings["cnx_bd_citas_medicas"].ConnectionString;
            int procesar;
            using (SqlConnection con = new SqlConnection(cadenaCon))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("usp_usuarios_crud", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@indicador", indicador);
                    cmd.Parameters.AddWithValue("@id_usuario", objUsario.IdUsuario);
                    cmd.Parameters.AddWithValue("@nombre", objUsario.Nombre);
                    cmd.Parameters.AddWithValue("@apellido", objUsario.Apellido);
                    cmd.Parameters.AddWithValue("@correo", objUsario.Correo);
                    cmd.Parameters.AddWithValue("@contraseña", objUsario.Contraseña);
                    cmd.Parameters.AddWithValue("@telefono", objUsario.Telefono);
                    cmd.Parameters.AddWithValue("@rol", objUsario.Rol);
                    cmd.Parameters.AddWithValue("@foto_perfil", objUsario.FotoPerfil);
                    procesar = cmd.ExecuteNonQuery();
                }
            }
            return procesar;
        }


        public List<Usuario> operacionesLectura(string indicador, Usuario objUsuario)
        {
            List<Usuario> lista = new List<Usuario>();
            Usuario Aeropuertos;
            string cadena = ConfigurationManager.ConnectionStrings["cnx_bd_citas_medicas"].ConnectionString;

            using (SqlConnection con = new SqlConnection(cadena))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("usp_usuarios_crud", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Clear();

                    cmd.Parameters.AddWithValue("@indicador", indicador);
                    cmd.Parameters.AddWithValue("@id_usuario", objUsuario.IdUsuario);
                    cmd.Parameters.AddWithValue("@nombre", objUsuario.Nombre);
                    cmd.Parameters.AddWithValue("@apellido", objUsuario.Apellido);
                    cmd.Parameters.AddWithValue("@correo", objUsuario.Correo);
                    cmd.Parameters.AddWithValue("@contraseña", objUsuario.Contraseña);
                    cmd.Parameters.AddWithValue("@telefono", objUsuario.Telefono);
                    cmd.Parameters.AddWithValue("@rol", objUsuario.Rol);
                    cmd.Parameters.AddWithValue("@foto_perfil", objUsuario.FotoPerfil);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Aeropuertos = new Usuario()
                            {
                                IdUsuario = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                                Nombre = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                                Apellido = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                                Correo = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                                Contraseña = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                                Telefono = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                                Rol = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                                FotoPerfil = reader.IsDBNull(7) ? string.Empty : reader.GetString(7)
                            };
                            lista.Add(Aeropuertos);
                        }
                    }
                }
            }
            return lista;
        }
    }
}