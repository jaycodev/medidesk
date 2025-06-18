using System.Collections.Generic;
using sistema_citas_medicas.Dao;
using sistema_citas_medicas.Dao.DaoImpl;
using sistema_citas_medicas.Models;

namespace sistema_citas_medicas.Services
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