using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            Console.WriteLine(user.FirstOrDefault());

            var newUser = new User
            {
                Name = "John",
                LastName = "Smith",
                DateOfBirth = DateTime.Now,
                PersonalId = "12345678901234",
                Gender = Gender.Male
            };

            var newUserId = proxy.CreateUser(newUser);
            Console.WriteLine(newUserId);

            Console.ReadKey();

            Configurator.SaveServiceState();
        }
    }
}
