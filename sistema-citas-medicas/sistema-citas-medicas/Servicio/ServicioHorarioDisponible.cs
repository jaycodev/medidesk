using sistema_citas_medicas.Dao.DaoImpl;
using sistema_citas_medicas.Dao;
using sistema_citas_medicas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sistema_citas_medicas.Servicio
{
    public class ServicioHorarioDisponible
    {
        public int operacionesEscritura(string indicador, HorarioDisponible s)
        {
            IHorarioDisponibleDao dao = new HorarioDisponibleDaoImpl();
            return dao.operacionesEscritura(indicador, s);
        }
        public List<HorarioDisponible> operacionesLectura(string indicador, HorarioDisponible s)
        {
            IHorarioDisponibleDao dao = new HorarioDisponibleDaoImpl();
            return dao.operacionesLectura(indicador, s);
        }
    }
}