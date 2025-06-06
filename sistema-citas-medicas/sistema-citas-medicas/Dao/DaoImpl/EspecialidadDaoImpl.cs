using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using sistema_citas_medicas.Models;

namespace sistema_citas_medicas.Dao.DaoImpl
{
    public class EspecialidadDaoImpl : IEspecialidadDao
    {
        public IEnumerable<Especialidad> Listado()
        {
            List<Especialidad> list = new List<Especialidad>();
            using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["cnx_bd_citas_medicas"].ConnectionString))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("exec usp_especialidad", cn);
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    list.Add(new Especialidad()
                    {
                        IdEspecialidad = rdr.GetInt32(0),
                        Nombre = rdr.GetString(1),

                    });
                }
                rdr.Close();
            }
            return list;
        }

    }
}