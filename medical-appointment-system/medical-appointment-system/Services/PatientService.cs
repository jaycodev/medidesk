using System.Collections.Generic;
using medical_appointment_system.Dao;
using medical_appointment_system.Dao.DaoImpl;
using medical_appointment_system.Models;

namespace medical_appointment_system.Services
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