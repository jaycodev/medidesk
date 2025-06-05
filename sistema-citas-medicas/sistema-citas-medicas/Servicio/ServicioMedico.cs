using sistema_citas_medicas.Dao.DaoImpl;
using sistema_citas_medicas.Dao;
using sistema_citas_medicas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sistema_citas_medicas.Servicio
{
    public class ServicioMedico
    {
        public int operacionesEscritura(string indicador, Medico s)
        {
            IMedicoDao dao = new MedicoDaoImpl();
            return dao.operacionesEscritura(indicador, s);
        }
        public List<Medico> operacionesLectura(string indicador, Medico s)
        {
            IMedicoDao dao = new MedicoDaoImpl();
            return dao.operacionesLectura(indicador, s);
        }
    }
}