namespace SipkaTemplate.Core.Services
{
    public interface ICustomUpdateService<T> where T : class
    {
        T Check(T oldEntity, T newEntity);
    }
}
