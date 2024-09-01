namespace AttractorScheduler.Code;

public class DependencyResolver
{
    private readonly Dictionary<Type, Func<object>> _services = new();

    public void Register<TService>(Func<TService> implementationFactory) where TService : class 
        => _services[typeof(TService)] = implementationFactory;

    public TService Resolve<TService>() where TService : class
    {
        if (_services.TryGetValue(typeof(TService), out var implementationFactory))
            return (implementationFactory() as TService)!;
        
        throw new InvalidOperationException($"Сервис '{typeof(TService)}' не зарегистрирован.");
    }
}