using sistema_citas_medicas.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace sistema_citas_medicas.Dao.DaoImpl
{
    internal class MedicoDaoImpl : IMedicoDao
    {
        public int operacionesEscritura(string indicador, Medico objMedico)
        {
            string cadena = ConfigurationManager.ConnectionStrings["cnx_bd_citas_medicas"].ConnectionString;
            int procesar;
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                
                    cn.Open();

                    using (SqlCommand cmd = new SqlCommand("usp_medico_crud", cn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@indicador", indicador);
                        cmd.Parameters.AddWithValue("@id_usuario", objMedico.IdUsuario);
                        cmd.Parameters.AddWithValue("@nombre", objMedico.Nombre);
                        cmd.Parameters.AddWithValue("@apellido", objMedico.Apellido);
                        cmd.Parameters.AddWithValue("@correo", objMedico.Correo);
                        cmd.Parameters.AddWithValue("@contraseña", objMedico.Contraseña);
                        cmd.Parameters.AddWithValue("@telefono", objMedico.Telefono);
                        cmd.Parameters.AddWithValue("@foto_perfil", objMedico.FotoPerfil);
                        cmd.Parameters.AddWithValue("@rol", objMedico.Rol);
                        cmd.Parameters.AddWithValue("@id_especialidad", objMedico.IdEspecialidad);
                        cmd.Parameters.AddWithValue("@estado", objMedico.Estado);

                        procesar = cmd.ExecuteNonQuery();


                }
            }
            return procesar;
        }

        public List<Medico> operacionesLectura(string indicador, Medico objMedico)
        {
            List<Medico> lista = new List<Medico>();
            string cadena = ConfigurationManager.ConnectionStrings["cnx_bd_citas_medicas"].ConnectionString;

            using(SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                 
                using(SqlCommand cmd = new SqlCommand("usp_medico_crud", cn))
                { 
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    
                    cmd.Parameters.AddWithValue("@indicador", indicador);
                    cmd.Parameters.AddWithValue("@id_usuario", objMedico.IdUsuario);
                    cmd.Parameters.AddWithValue("@nombre", objMedico.Nombre);
                    cmd.Parameters.AddWithValue("@apellido", objMedico.Apellido);
                    cmd.Parameters.AddWithValue("@correo", objMedico.Correo);
                    cmd.Parameters.AddWithValue("@contraseña", objMedico.Contraseña);
                    cmd.Parameters.AddWithValue("@telefono", objMedico.Telefono);
                    cmd.Parameters.AddWithValue("@foto_perfil", objMedico.FotoPerfil);
                    cmd.Parameters.AddWithValue("@rol", objMedico.Rol);
                    cmd.Parameters.AddWithValue("@id_especialidad", objMedico.IdEspecialidad);
                    cmd.Parameters.AddWithValue("@estado", objMedico.Estado);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add( new Medico
                            {
                                IdUsuario = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                                Nombre = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                                Apellido = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                                Correo = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                                Contraseña = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                                Telefono = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                                FotoPerfil = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                                Rol = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                            IdEspecialidad = reader.IsDBNull(8) ? 0 : reader.GetInt32(8),
                                EspecialidadNombre = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
                                Estado = reader.IsDBNull(10) ? false : reader.GetBoolean(10)
                            });
                        }
                    }
                }  
            }
            return lista;
        }



    }
}
