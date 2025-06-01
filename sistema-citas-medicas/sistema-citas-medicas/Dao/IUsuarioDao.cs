using sistema_citas_medicas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sistema_citas_medicas.Dao
{
    internal interface IUsuarioDao
    {
        int operacionesEscritura(string indicador, Usuario objUsuario);

        List<Usuario> operacionesLectura(string indicador, Usuario objUsuario);
    }
}
