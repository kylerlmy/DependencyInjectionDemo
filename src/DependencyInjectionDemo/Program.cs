using System;
using System.Security.Authentication.ExtendedProtection;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjectionDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();

            //GetSingletone(services);
            // GetTranisent(services);

            GetScroped(services);


            Console.ReadLine();
        }

        private static void GetSingletone(ServiceCollection services)
        {   
            // 默认构造
            services.AddSingleton<IOperationSingleton, Operation>();
            // 自定义传入Guid空值
            services.AddSingleton<IOperationSingleton>(new Operation(Guid.Empty));
            // 自定义传入一个New的Guid
            services.AddSingleton<IOperationSingleton>(new Operation(Guid.NewGuid()));

            var provider = services.BuildServiceProvider();

            //这里的GUID是在传递一个 new Operation(Guid.NewGuid())实例所获取的Guid
            var singletone1 = provider.GetService<IOperationSingleton>();
            Console.WriteLine($"singletone1:{singletone1.OperationId}");

            var singletone2 = provider.GetService<IOperationSingleton>();
            Console.WriteLine($"singletone2:{singletone2.OperationId}");
            Console.WriteLine($"singletone1==singletond2?:{singletone1 == singletone2}");

        }

        private static void GetScroped(ServiceCollection services)
        {
            services.AddScoped<IOperationScoped, Operation>();
            services.AddTransient<IOperationTransient, Operation>();
            services.AddSingleton<IOperationSingleton, Operation>();

            var provider = services.BuildServiceProvider();


            using (var scope1=provider.CreateScope())
            {
                var p = scope1.ServiceProvider;

                var scopeobj1 = p.GetService<IOperationScoped>();
                var transientObj1 = p.GetService<IOperationTransient>();
                var singletonObj1 = p.GetService<IOperationSingleton>();


                var scopeobj2= p.GetService<IOperationScoped>();
                var transientObj2= p.GetService<IOperationTransient>();
                var singletonObj2= p.GetService<IOperationSingleton>();
                Console.WriteLine($"scopeobj1:{scopeobj1.OperationId}\r\n" +
                                  $"transientObj1:{transientObj1.OperationId}\r\n" +
                                  $"singletonObj1:{singletonObj1.OperationId}");
                Console.WriteLine("**********************************************");
                Console.WriteLine($"scopeobj2:{scopeobj2.OperationId}\r\n" +
                                  $"transientObj2:{transientObj2.OperationId}\r\n" +
                                  $"singletonObj2:{singletonObj2.OperationId}");
            }

            Console.WriteLine("**********************************************");

            var scope = provider.GetService<IOperationTransient>();  //Tranisent与Scoped，使用IServiceScop实例的ServiceProvider属性返回的Provider实例，调用GetService方法创建的实例返回的GUID，与  直接调用ServiceCollection实例的BuildServiceProvider方法获取的Provider的实例，调用GetService方法创建的实例 返回的GUID不同
            Console.WriteLine($"scope:{scope.OperationId}");
           
            var transient1 = provider.GetService<IOperationScoped>();
            Console.WriteLine($"transient:{transient1.OperationId}");

            var singletone = provider.GetService<IOperationSingleton>();
            Console.WriteLine($"singletone:{singletone.OperationId}");


        }


        private static void GetTranisent(ServiceCollection services)
        {
            services.AddTransient<IOperationTransient, Operation>();

            var provider = services.BuildServiceProvider();

            var transient1 = provider.GetService<IOperationTransient>();
            Console.WriteLine($"transient1: {transient1.OperationId}");

            var transient2 = provider.GetService<IOperationTransient>();
            Console.WriteLine($"transient2: {transient2.OperationId}");
            Console.WriteLine($"transient1 == transient2 ? :{ transient1 == transient2 } ");
        
           

        }



    }

    public class Operation : IOperationScoped, IOperationSingleton, IOperationTransient
    {
        private Guid _guid;

        public Guid OperationId => _guid;

        public Operation()
        {
            //_guid =new Guid();
            _guid = Guid.NewGuid();
        }

        public Operation(Guid guid)
        {
            _guid = guid;
        }
    }



    public interface IOperation
    {
        Guid OperationId { get; }
    }

    public interface IOperationScoped : IOperation
    {

    }

    public interface IOperationTransient : IOperation
    {
    }

    public interface IOperationSingleton : IOperation
    {
    }






}
