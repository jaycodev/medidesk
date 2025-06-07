using sistema_citas_medicas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace sistema_citas_medicas.Dao
{
    internal interface IMedicoDao
    {
        int operacionesEscritura(string indicador, Medico objMedico);
        List<Medico> operacionesLectura(string indicador, Medico objMedico);
    }
}
