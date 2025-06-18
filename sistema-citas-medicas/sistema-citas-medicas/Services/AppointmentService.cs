using System.Collections.Generic;
using sistema_citas_medicas.Dao;
using sistema_citas_medicas.Dao.DaoImpl;
using sistema_citas_medicas.Models;

namespace sistema_citas_medicas.Services
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