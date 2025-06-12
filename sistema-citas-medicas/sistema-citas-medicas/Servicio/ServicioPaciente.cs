using System.Collections.Generic;
using sistema_citas_medicas.Dao;
using sistema_citas_medicas.Dao.DaoImpl;
using sistema_citas_medicas.Models;

namespace sistema_citas_medicas.Servicio
{
    public class ServicioPaciente
    {
        IPacienteDao dao = new PacienteDao();

        public int operacionesEscritura(string indicador, Paciente objPaciente)
        {
            return dao.operacionesEscritura(indicador, objPaciente);
        }

        public List<Paciente> operacionesLectura(string indicador, Paciente objPaciente)
        {
            return dao.operacionesLectura(indicador, objPaciente);
        }
    }
}