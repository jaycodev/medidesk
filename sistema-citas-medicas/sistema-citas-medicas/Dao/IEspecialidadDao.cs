using sistema_citas_medicas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sistema_citas_medicas.Dao
{
    internal interface IEspecialidadDao
    {
        int operacionesEscritura(string indicador, Especialidad objEspec);

        List<Especialidad> operacionesLectura(string indicador, Especialidad objEspec);
    }
}
