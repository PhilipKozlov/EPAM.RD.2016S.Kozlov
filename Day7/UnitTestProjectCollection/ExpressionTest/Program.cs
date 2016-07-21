using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionTest
{
    class Program
    {
        static void Main(string[] args)
        {
            ParameterExpression numParam = Expression.Parameter(typeof(int), "num");
            ConstantExpression five = Expression.Constant(5, typeof(int));
            BinaryExpression numLessThanFive = Expression.LessThan(numParam, five);
            Expression<Func<int, bool>> lambda1 = Expression.Lambda<Func<int, bool>>(numLessThanFive, new ParameterExpression[] { numParam });

            Console.WriteLine(lambda1);
            Console.WriteLine(lambda1.Compile());
            Console.WriteLine(lambda1.Compile().Invoke(1));

            Func<int, bool> func = (i) => i < 5;
            Console.ReadKey();
        }
    }
}
