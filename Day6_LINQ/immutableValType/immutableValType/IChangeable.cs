using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace immutableValType
{
    public interface IChangeable
    {
        void Change(int x, int y);
    }
}
