using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SystemConfigurator;
using UserStorage;

namespace Client
{
    class Program
    {
        static ManualResetEventSlim mres =  new ManualResetEventSlim(false);

        static void Main(string[] args)
        {
            var proxy = Configurator.ConfigurateServices();
            //var user = proxy.FindByNameAndLastName("Jane", "Doe");

            //Console.WriteLine(user.FirstOrDefault());

            //var newUser = new User
            //{
            //    Name = "John",
            //    LastName = "Smith",
            //    DateOfBirth = DateTime.Now,
            //    PersonalId = "12345678901234",
            //    Gender = Gender.Male
            //};

            //var newUserId = proxy.CreateUser(newUser);
            //Console.WriteLine(newUserId);

            Task.Factory.StartNew(() => Searh(proxy, "John", "Smith"));
            Task.Factory.StartNew(() => CreateDelete(proxy));
            Task.Factory.StartNew(() => Delete(proxy));
            mres.Set();

            Console.ReadKey();

            Configurator.SaveServiceState();
        }

        private static void Searh(ProxyService proxy, string name, string lastName)
        {
            while (true)
            {
                Console.WriteLine("Search result = {0}", proxy.FindByNameAndLastName(name, lastName).Count());
                Thread.Sleep(500);
            }
        }

        private static void CreateDelete(ProxyService proxy)
        {
            var newUser = new User
            {
                Name = "John",
                LastName = "Smith",
                DateOfBirth = DateTime.Now,
                PersonalId = "12345678901234",
                Gender = Gender.Male
            };
            mres.Wait();
            while (true)
            {
                var newUserId = proxy.CreateUser(newUser);
                Console.WriteLine("New user : {0}", newUser.Id);
                Thread.Sleep(2000);
                proxy.DeleteUser(newUser);
                Console.WriteLine("User deleted.");
            }
        }

        private static void Delete(ProxyService proxy)
        {
            var newUser = new User
            {
                Name = "John",
                LastName = "Smith",
                DateOfBirth = DateTime.Now,
                PersonalId = "12345678901234",
                Gender = Gender.Male
            };
            mres.Wait();
            while (true)
            {
                proxy.DeleteUser(newUser);
                Console.WriteLine("User deleted.");
                Thread.Sleep(2000);
            }
        }
    }
}
