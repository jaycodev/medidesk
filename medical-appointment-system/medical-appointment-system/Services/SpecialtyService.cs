using System.Collections.Generic;
using medical_appointment_system.Models;
using medical_appointment_system.Dao;
using medical_appointment_system.Dao.DaoImpl;

namespace medical_appointment_system.Services
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