using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace UserStorage
{
    public class ProxyUserService : IUserService
    {
        private readonly IList<IUserService> servicePool;

        public ProxyUserService(IList<IUserService> services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            servicePool = services;
        }

        public int CreateUser(User user)
        {
            return servicePool.FirstOrDefault().CreateUser(user);
        }

        public void DeleteUser(User user)
        {
            servicePool.FirstOrDefault().DeleteUser(user);
        }

        public IEnumerable<int> FindByName(string name)
        {
            return servicePool[1].FindByName(name);
        }

        public IEnumerable<int> FindByNameAndLastName(string name, string lastName)
        {
            return servicePool[2].FindByNameAndLastName(name, lastName);
        }

        public IEnumerable<int> FindByPersonalId(string personalId)
        {
            return servicePool[3].FindByPersonalId(personalId);
        }
    }
}
