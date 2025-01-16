using Gstudent.Repositories.Implementation;
using Gstudent.Repositories.Interface;
using System.Web.Mvc;
using Unity;
using Unity.Mvc5;

namespace Gstudent
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();
            container.RegisterType<IStudentRepository, StudentRepository>();
            container.RegisterType<Iloginrepository, loginrepository>();
            container.RegisterType<IPasswordRepository, PasswordRepository>();
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}