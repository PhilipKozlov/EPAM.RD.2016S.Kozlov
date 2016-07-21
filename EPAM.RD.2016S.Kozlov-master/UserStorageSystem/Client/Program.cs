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
            var configurator = new Configurator();
            configurator.ConfigurateServices();
            var user = configurator.Services.FirstOrDefault().FindByNameAndLastName("Jane", "Doe");
            Console.WriteLine(user.FirstOrDefault());

            var newUser = new User
            {
                Name = "John",
                LastName = "Smith",
                DateOfBirth = DateTime.Now,
                PersonalId = "12345678901234",
                Gender = Gender.Male
            };

            var newUserId = configurator.Services.FirstOrDefault().CreateUser(newUser);
            Console.WriteLine(newUserId);
            configurator.SaveServiceState();

            Console.ReadKey();
        }
    }
}
