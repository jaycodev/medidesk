using System.Collections.Generic;
using sistema_citas_medicas.Dao;
using sistema_citas_medicas.Dao.DaoImpl;
using sistema_citas_medicas.Models;

namespace sistema_citas_medicas.Services
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