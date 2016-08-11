/*
 * DJS-google disjoint sets to know what is this
 * in few word,tree-type structure for working with sets
 */
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Windows.Forms;
using Meta.Numerics.Matrices;
namespace WindowsFormsApplication3
{


    public class Djs
    {
        
        static public void init(int[] DisSet, int size)
        {
            for (int i = 0; i < size; i++)
            {
                DisSet[i] = i;
            }
        }
        static public int findset(int x, int[] DisSet)
        {
            if (x == DisSet[x]) return x;
            return DisSet[x] = findset(DisSet[x], DisSet);
        }
        static  public void union1(int x, int y, int[] DisSet, int[] Size)
        {
            x = findset(x, DisSet);
            y = findset(y, DisSet);
            Random rc = new Random();
            if (rc.Next() % 2 == 0)
            {
                DisSet[y] = x;
                Size[x] = Size[y] + Size[x];
            }
            else
            {
                DisSet[x] = y;
                Size[y] = Size[x] + Size[y];
            }
        }


    }
}