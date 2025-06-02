using sistema_citas_medicas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sistema_citas_medicas.Dao
{
    internal interface IHorarioDisponibleDao
    {
        int operacionesEscritura(string indicador, HorarioDisponible objHorarioDisponible);

        List<HorarioDisponible> operacionesLectura(string indicador, HorarioDisponible objHorarioDisponible);
    }
}
