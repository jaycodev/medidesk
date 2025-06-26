using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using medical_appointment_system.Dao;
using medical_appointment_system.Dao.DaoImpl;
using medical_appointment_system.Models;

namespace medical_appointment_system.Services
{
    public class NotificationService
    {
        IGenericDao<Notification> dao = new NotificationDaoImpl();

        public int ExecuteWrite(string indicator, Notification a)
        {
            return dao.ExecuteWrite(indicator, a);
        }

        public List<Notification> ExecuteRead(string indicator, Notification a)
        {
            return dao.ExecuteRead(indicator, a);
        }


    }
}