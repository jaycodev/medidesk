using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sistema_citas_medicas.Models.ViewModel
{
    public class PacienteViewModel
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Correo { get; set; }
        public string Contraseña { get; set; }
        public string Telefono { get; set; }
        public string FotoPerfil { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string GrupoSanguineo { get; set; }
    }
}