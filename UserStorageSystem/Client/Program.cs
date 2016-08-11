namespace Client
{
    using System;
    using System.Linq;
    using System.ServiceModel;
    using System.Threading;
    using System.Threading.Tasks;
    using UserStorage;

    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.Title = "Client";
            var cf = new ChannelFactory<IUserService>(new NetTcpBinding(), "net.tcp://127.0.0.1:5555");
            IUserService proxy = cf.CreateChannel();

            Task.Factory.StartNew(() => Searh(proxy, "John", "Smith"));
            Task.Factory.StartNew(() => CreateDelete(proxy));
            Task.Factory.StartNew(() => Delete(proxy));

            Console.ReadKey();
        }

        private static void Searh(IUserService proxy, string name, string lastName)
        {
            while (true)
            {
                Console.WriteLine("Search result = {0}", proxy.FindByNameAndLastName(name, lastName).Count());
                Thread.Sleep(500);
            }
        }

        private static void CreateDelete(IUserService proxy)
        {
            var newUser = new User
            {
                Name = "John",
                LastName = "Smith",
                DateOfBirth = DateTime.Now,
                PersonalId = "12345678901234",
                Gender = Gender.Male
            };
            while (true)
            {
                var user = proxy.CreateUser(newUser);
                Console.WriteLine("New user : {0}", user);
                Thread.Sleep(1000);
                proxy.DeleteUser(user);
                Console.WriteLine("User deleted.");
            }
        }

        private static void Delete(IUserService proxy)
        {
            var newUser = new User
            {
                Name = "John",
                LastName = "Smith",
                DateOfBirth = DateTime.Now,
                PersonalId = "12345678901234",
                Gender = Gender.Male
            };
            while (true)
            {
                proxy.DeleteUser(newUser);
                Console.WriteLine("User deleted.");
                Thread.Sleep(2000);
            }
        }
    }
}
