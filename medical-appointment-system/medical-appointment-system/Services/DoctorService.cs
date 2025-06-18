using System.Collections.Generic;
using medical_appointment_system.Dao;
using medical_appointment_system.Dao.DaoImpl;
using medical_appointment_system.Models;

namespace medical_appointment_system.Services
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