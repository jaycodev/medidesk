using sistema_citas_medicas.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Web;

namespace sistema_citas_medicas.Dao.DaoImpl
{
    public class EspecialidadDaoImpl : IEspecialidadDao
    {
        public int operacionesEscritura(string indicador, Especialidad objEspec)
        {
            string cadena = ConfigurationManager.ConnectionStrings["cnx_bd_citas_medicas"].ConnectionString;
            int procesar;
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand("usp_especialidad_crud", cn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@indicador", indicador);
                    cmd.Parameters.AddWithValue("@id_especialidad", objEspec.IdEspecialidad);
                    cmd.Parameters.AddWithValue("@nombre", objEspec.Nombre);
                    cmd.Parameters.AddWithValue("@descripcion", objEspec.Descripcion);
                    procesar = cmd.ExecuteNonQuery();
                }
            }
            return procesar;
        }

        public List<Especialidad> operacionesLectura(string indicador, Especialidad objEspec)
        {
            List<Especialidad> lista = new List<Especialidad>();
            string cadena = ConfigurationManager.ConnectionStrings["cnx_bd_citas_medicas"].ConnectionString;

            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand("usp_especialidad_crud", cn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@indicador", indicador);
                    cmd.Parameters.AddWithValue("@id_especialidad", objEspec.IdEspecialidad);
                    cmd.Parameters.AddWithValue("@nombre", objEspec.Nombre);
                    cmd.Parameters.AddWithValue("@descripcion", objEspec.Descripcion);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Especialidad()
                            {
                                IdEspecialidad = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                                Nombre = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                                Descripcion = reader.IsDBNull(2) ? string.Empty : reader.GetString(2)
                            });

                        }
                    }
                }
            }
            return lista;

        }
    }
}