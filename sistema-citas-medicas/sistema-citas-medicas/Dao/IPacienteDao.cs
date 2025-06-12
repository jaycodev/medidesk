using sistema_citas_medicas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace sistema_citas_medicas.Dao
{
    public interface IPacienteDao
    {
        int operacionesEscritura(string indicador, Paciente objPaciente);
        List<Paciente> operacionesLectura(string indicador, Paciente objPaciente);
    }
}