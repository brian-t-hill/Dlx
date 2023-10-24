using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pentomino.Helpers;

public static class RandomGenerator
{
    [ThreadStatic] private static Random? s_rnd = null;

    public static Random Current => s_rnd ??= new Random();


    public static List<T> Shuffle<T>(this List<T> list)
    {
        Random rnd = RandomGenerator.Current;

        for (int jj = list.Count - 1; jj >= 1; --jj)
        {
            int kk = rnd.Next(jj + 1);

            (list[jj], list[kk]) = (list[kk], list[jj]);
        }

        return list;
    }


}


