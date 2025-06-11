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
        void Registrar(Paciente objPaciente);
        List<Paciente> Listar();
    }
}