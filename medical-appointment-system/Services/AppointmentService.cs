using System.Collections.Generic;
using medical_appointment_system.Dao;
using medical_appointment_system.Dao.DaoImpl;
using medical_appointment_system.Models;

namespace medical_appointment_system.Services
{
    public class AppointmentService
    {
        IGenericDao<Appointment> dao = new AppointmentDaoImpl();

        public int ExecuteWrite(string indicator, Appointment a)
        {
            return dao.ExecuteWrite(indicator, a);
        }

        public List<Appointment> ExecuteRead(string indicator, Appointment a)
        {
            return dao.ExecuteRead(indicator, a);
        }

    }
}