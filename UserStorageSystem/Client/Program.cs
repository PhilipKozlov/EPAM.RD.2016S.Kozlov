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
        static void Main(string[] args)
        {
            var proxy = Configurator.ConfigurateServices();
            var user = proxy.FindByNameAndLastName("Jane", "Doe");

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


            Task.Factory.StartNew(() => CreateDelete(proxy));
            //Task.Factory.StartNew(() => Delete(proxy));
            //Task.Factory.StartNew(() => Searh(proxy, "John", "Smith"));

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
            while (true)
            {
                var newUser = new User
                {
                    Name = "John",
                    LastName = "Smith",
                    DateOfBirth = DateTime.Now,
                    PersonalId = "12345678901234",
                    Gender = Gender.Male
                };
                var newUserId = proxy.CreateUser(newUser);
                newUser.Id = newUserId;
                Console.WriteLine("New user : {0}", newUserId);
                Thread.Sleep(1000);
                proxy.DeleteUser(newUser);
                Console.WriteLine("User deleted.");
            }
        }

        private static void Delete(ProxyService proxy)
        {
            while (true)
            {
                var newUser = new User
                {
                    Name = "John",
                    LastName = "Smith",
                    DateOfBirth = DateTime.Now,
                    PersonalId = "12345678901234",
                    Gender = Gender.Male
                };
                newUser.Id = proxy.FindByNameAndLastName("John", "Smith").FirstOrDefault();
                proxy.DeleteUser(newUser);
                Console.WriteLine("User deleted.");
                Thread.Sleep(3000);
            }
        }
    }
}
