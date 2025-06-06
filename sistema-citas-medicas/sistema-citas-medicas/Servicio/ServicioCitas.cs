using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using sistema_citas_medicas.Dao.DaoImpl;
using sistema_citas_medicas.Dao;
using sistema_citas_medicas.Models;

namespace sistema_citas_medicas.Servicio
{
    public class ServicioCitas
    {
        public int operacionesEscritura(string indicador, Cita s)
        {
            ICitaDao dao = new CitaDaoImpl();
            return dao.operacionesEscritura(indicador, s);
        }
        public List<Cita> operacionesLectura(string indicador, Cita s)
        {
            ICitaDao dao = new CitaDaoImpl();
            return dao.operacionesLectura(indicador, s);
        }

    }
}