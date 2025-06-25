using System.Collections.Generic;
using medical_appointment_system.Dao;
using medical_appointment_system.Dao.DaoImpl;
using medical_appointment_system.Models;

namespace medical_appointment_system.Services
{
    public class ScheduleService
    {
        IGenericDao<Schedule> dao = new ScheduleDaoImpl();

        public int ExecuteWrite(string indicator, Schedule s)
        {
            return dao.ExecuteWrite(indicator, s);
        }

        public List<Schedule> ExecuteRead(string indicator, Schedule s)
        {
            return dao.ExecuteRead(indicator, s);
        }
    }
}