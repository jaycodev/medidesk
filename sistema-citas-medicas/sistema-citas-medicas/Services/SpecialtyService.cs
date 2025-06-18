using System.Collections.Generic;
using sistema_citas_medicas.Dao;
using sistema_citas_medicas.Dao.DaoImpl;
using sistema_citas_medicas.Models;

namespace sistema_citas_medicas.Services
{
    public class SpecialtyService
    {
        IGenericDao<Specialty> dao = new SpecialtyDaoImpl();

        public int ExecuteWrite(string indicator, Specialty s)
        {
            return dao.ExecuteWrite(indicator, s);
        }

        public List<Specialty> ExecuteRead(string indicator, Specialty s)
        {
            return dao.ExecuteRead(indicator, s);
        }
    }
}