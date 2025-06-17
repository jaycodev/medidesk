using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sistema_citas_medicas.Models.ViewModels
{
    public class UsuarioDetalleViewModel
    {
        public Usuario Usuario { get; set; }
        public Medico Medico { get; set; }
        public Paciente Paciente { get; set; }
    }
}