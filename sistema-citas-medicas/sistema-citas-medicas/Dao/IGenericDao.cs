using System.Collections.Generic;

namespace sistema_citas_medicas.Dao
{
    public interface IGenericDao<T>
    {
        int ExecuteWrite(string indicator, T entity);
        List<T> ExecuteRead(string indicator, T entity);
    }
}