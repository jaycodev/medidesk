using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sistema_citas_medicas.Models;

namespace sistema_citas_medicas.Dao
{
    internal interface IEspecialidadDao
    {
        IEnumerable<Especialidad> Listado();


    }
}
