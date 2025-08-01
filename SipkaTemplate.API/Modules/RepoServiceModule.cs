using Autofac;
using System.Reflection;
using SipkaTemplate.Core.Repositories;
using SipkaTemplate.Core.Services;
using SipkaTemplate.Core.UnitOfWorks;
using SipkaTemplate.Repository;
using SipkaTemplate.Repository.Repositories;
using SipkaTemplate.Repository.UnitOfWorks;
using SipkaTemplate.Service.Mappings;
using SipkaTemplate.Service.Services;
using Module = Autofac.Module;


namespace SipkaTemplate.API.Modules
{
    public class RepoServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(GenericRepository<>))
                .As(typeof(IGenericRepository<>)).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(Service<>))
                .As(typeof(IService<>)).InstancePerLifetimeScope();
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>();
            builder.RegisterType<TokenHandler>().As<ITokenHandler>();
            var apiAssembly = Assembly.GetExecutingAssembly();
            var repoAssembly = Assembly.GetAssembly(typeof(AppDbContext));
            var serviceAssembly = Assembly.GetAssembly(typeof(MapProfile));

            if (repoAssembly == null)
                throw new InvalidOperationException("AppDbContext assembly bulunamadı.");

            if (serviceAssembly == null)
                throw new InvalidOperationException("MapProfile assembly bulunamadı.");

            builder.RegisterAssemblyTypes(apiAssembly, repoAssembly, serviceAssembly)
                .Where(x => x.Name.EndsWith("Repository"))
                .AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(apiAssembly, repoAssembly, serviceAssembly)
                .Where(x => x.Name.EndsWith("Service"))
                .AsImplementedInterfaces().InstancePerLifetimeScope();
        }
    }
}
