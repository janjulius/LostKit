using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LostKit.Helpers
{
    public class NumberHelper
    {
        public static string MakeBigNumberReadable(int number)
            => number.ToString("N0");

        public static string MakeBigNumberReadable(double number)
            => MakeBigNumberReadable(Convert.ToInt32(number));
    }
}
