using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LostKit
{
    public class RunescapeCalculators
    {
        public static int GetExperienceForLevel(int level)
        {
            double points = 0;
            double output = 0;
            for (int lvl = 1; lvl <= level; lvl++)
            {
                points += Math.Floor(lvl + 300.0 * Math.Pow(2.0, lvl / 7.0));
                if (lvl >= level)
                {
                    return Convert.ToInt32(Math.Round(output));
                }
                output = (int)Math.Floor(points / 4);
            }
            return 0;
        }
    }
}
