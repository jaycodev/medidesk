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

        public List<Medico> consultarTodo(string indicador, Medico objMedico)
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
                    cmd.Parameters.AddWithValue("@telefono", objMedico.Telefono);
                    cmd.Parameters.AddWithValue("@id_especialidad", objMedico.IdEspecialidad);

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
                                Telefono = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                                EspecialidadNombre = reader.IsDBNull(5) ? string.Empty : reader.GetString(5)
                            });
                        }
                    }
                }  
            }
            return lista;
        }



    }
}
