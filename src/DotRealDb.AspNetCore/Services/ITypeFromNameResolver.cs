namespace DotRealDb.AspNetCore.Services
{
    public interface ITypeFromNameResolver
    {
        object Resolve(string name);

        T Resolve<T>(string name);
    }
}
