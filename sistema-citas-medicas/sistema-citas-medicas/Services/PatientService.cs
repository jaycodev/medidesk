using System.Collections.Generic;
using sistema_citas_medicas.Dao;
using sistema_citas_medicas.Dao.DaoImpl;
using sistema_citas_medicas.Models;

namespace sistema_citas_medicas.Services
{
    public class PatientService
    {
        IGenericDao<Patient> dao = new PatientDaoImpl();

        public int ExecuteWrite(string indicator, Patient p)
        {
            return dao.ExecuteWrite(indicator, p);
        }

        public List<Patient> ExecuteRead(string indicator, Patient p)
        {
            return dao.ExecuteRead(indicator, p);
        }
    }
}