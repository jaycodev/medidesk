using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using sistema_citas_medicas.Dao.DaoImpl;
using sistema_citas_medicas.Dao;
using sistema_citas_medicas.Models;

namespace sistema_citas_medicas.Servicio
{
    public class ServicioEspecialidad
    {
        public List<Especialidad> operacionesLectura()
        {
            IEspecialidadDao dao = new EspecialidadDaoImpl();
            return (List<Especialidad>)dao.Listado();
        }
    }
}