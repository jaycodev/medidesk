using sistema_citas_medicas.Dao.DaoImpl;
using sistema_citas_medicas.Dao;
using sistema_citas_medicas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sistema_citas_medicas.Servicio
{
    public class ServicioEspecialidad
    {
        public int operacionesEscritura(string indicador, Especialidad s)
        {
            IEspecialidadDao dao = new EspecialidadDaoImpl();
            return dao.operacionesEscritura(indicador, s);
        }
        public List<Especialidad> operacionesLectura(string indicador, Especialidad s)
        {
            IEspecialidadDao dao = new EspecialidadDaoImpl();
            return dao.operacionesLectura(indicador, s);
        }
    }
}