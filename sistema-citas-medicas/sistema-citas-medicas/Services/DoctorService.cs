using System.Collections.Generic;
using sistema_citas_medicas.Dao;
using sistema_citas_medicas.Dao.DaoImpl;
using sistema_citas_medicas.Models;

namespace sistema_citas_medicas.Services
{
    public class DoctorService
    {
        IGenericDao<Doctor> dao = new DoctorDaoImpl();

        public int ExecuteWrite(string indicator, Doctor d)
        {
            return dao.ExecuteWrite(indicator, d);
        }

        public List<Doctor> ExecuteRead(string indicator, Doctor d)
        {
            return dao.ExecuteRead(indicator, d);
        }
    }
}