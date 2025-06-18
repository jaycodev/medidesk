using System.Collections.Generic;
using medical_appointment_system.Dao;
using medical_appointment_system.Dao.DaoImpl;
using medical_appointment_system.Models;

namespace medical_appointment_system.Services
{
    public class UserService
    {
        IGenericDao<User> dao = new UserDaoImpl();

        public int ExecuteWrite(string indicator, User u)
        {
            return dao.ExecuteWrite(indicator, u);
        }

        public List<User> ExecuteRead(string indicator, User u)
        {
            return dao.ExecuteRead(indicator, u);
        }
    }
}