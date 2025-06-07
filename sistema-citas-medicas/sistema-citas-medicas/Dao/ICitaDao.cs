using System.Collections.Generic;
using sistema_citas_medicas.Models;

namespace sistema_citas_medicas.Dao
{
    internal interface ICitaDao
    {
        int operacionesEscritura(string indicador, Cita cita);
        List<Cita> operacionesLectura(string indicador, Cita objcita);
    }
}
