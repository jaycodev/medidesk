namespace Api.Data.Contract
{
    public interface IGenericContract<T>
    {
        int ExecuteWrite(string indicator, T entity);
        List<T> ExecuteRead(string indicator, T entity);
    }
}