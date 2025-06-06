using sistema_citas_medicas.Dao.DaoImpl;
using sistema_citas_medicas.Models;
using System.Collections.Generic;
using sistema_citas_medicas.Dao;
using System.Linq;
using System.Web;

namespace sistema_citas_medicas.Servicio
{
    public class ServicioUsuario
    {
        public int operacionesEscritura(string indicador, Usuario s)
        {
            IUsuarioDao dao = new UsuarioDaoImpl();
            return dao.operacionesEscritura(indicador, s);
        }
        public List<Usuario> operacionesLectura(string indicador, Usuario s)
        {
            IUsuarioDao dao = new UsuarioDaoImpl();
            return dao.operacionesLectura(indicador, s);
        }
    }
}

