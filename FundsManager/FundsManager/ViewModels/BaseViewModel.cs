namespace FundsManager.ViewModels
{
    public class BaseViewModel<T>
    {
        public T toDBModel()
        {
            T t = default(T);
            return t;
        }
    }
}