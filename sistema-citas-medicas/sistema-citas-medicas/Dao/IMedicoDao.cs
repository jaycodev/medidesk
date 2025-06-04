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
        List<Medico> consultarTodo(string indicador, Medico p);
       /* Medico obtenerPais(string codigo);
        int registrar(Medico p);
        int actualizar(Medico p);
        int eliminar(string codigo);
       */
    }
}
