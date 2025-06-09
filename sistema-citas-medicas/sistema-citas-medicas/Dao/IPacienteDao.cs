using sistema_citas_medicas.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace sistema_citas_medicas.Dao
{
    public interface IPacienteDao
    {
        void Registrar(PacienteViewModel model);
        List<PacienteViewModel> Listar();
    }
}