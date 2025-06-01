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
                    cmd.Parameters.AddWithValue("@id_usuario", objUsario.idUsuario);
                    cmd.Parameters.AddWithValue("@nombre", objUsario.nombre);
                    cmd.Parameters.AddWithValue("@apellido", objUsario.apellido);
                    cmd.Parameters.AddWithValue("@correo", objUsario.correo);
                    cmd.Parameters.AddWithValue("@contraseña", objUsario.contraseña);
                    cmd.Parameters.AddWithValue("@telefono", objUsario.telefono);
                    cmd.Parameters.AddWithValue("@rol", objUsario.rol);
                    cmd.Parameters.AddWithValue("@foto_perfil", objUsario.fotoPerfil);
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
                    cmd.Parameters.AddWithValue("@id_usuario", objUsuario.idUsuario);
                    cmd.Parameters.AddWithValue("@nombre", objUsuario.nombre);
                    cmd.Parameters.AddWithValue("@apellido", objUsuario.apellido);
                    cmd.Parameters.AddWithValue("@correo", objUsuario.correo);
                    cmd.Parameters.AddWithValue("@contraseña", objUsuario.contraseña);
                    cmd.Parameters.AddWithValue("@telefono", objUsuario.telefono);
                    cmd.Parameters.AddWithValue("@rol", objUsuario.rol);
                    cmd.Parameters.AddWithValue("@foto_perfil", objUsuario.fotoPerfil);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Aeropuertos = new Usuario()
                            {
                                idUsuario = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                                nombre = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                                apellido = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                                correo = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                                contraseña = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                                telefono = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                                rol = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                                fotoPerfil = reader.IsDBNull(7) ? string.Empty : reader.GetString(7)
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