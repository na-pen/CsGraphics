using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsGraphics.Math;

namespace CsGraphics.Calc
{
    internal static class ZDepth
    {

        /// <summary>
        /// 平面の方程式を算出する.
        /// </summary>
        /// <param name="normal"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        internal static Vector PlaneEquation(Vector normal, Vector point)
        {
            Vector result = new Vector();

            result.X = normal.X;
            result.Y = normal.Y;
            result.Z = normal.Z;
            result.W = (normal.X * point.X) + (normal.Y * point.Y) + (normal.Z * point.Z);

            return result;
        }
    }
}
