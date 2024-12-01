using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

        /// <summary>
        /// 平行投影時のz深度を求める.
        /// </summary>
        /// <returns>z深度.</returns>
        internal static double ZDepsParallel(double[] pixel, double[] polygonPointA, double[] polygonPointB, double[] polygonPointC,int zMax, int zMin)
        {
            double result = 0;

            double px = pixel[0];
            double py = pixel[1];

            // 三角形の面積を計算
            double area = TriangleArea(polygonPointA[0], polygonPointA[1], polygonPointB[0], polygonPointB[1], polygonPointC[0], polygonPointC[1]);

            // 各重心座標を計算
            double A = TriangleArea(px, py, polygonPointB[0], polygonPointB[1], polygonPointC[0], polygonPointC[1]) / area;
            double B = TriangleArea(polygonPointA[0], polygonPointA[1], px, py, polygonPointC[0], polygonPointC[1]) / area;
            double C = TriangleArea(polygonPointA[0], polygonPointA[1], polygonPointB[0], polygonPointB[1], px, py) / area;

            // ピクセルの深度を計算 (加重平均)
            result = A * polygonPointA[2] + B * polygonPointB[2] + C * polygonPointC[2];


            return MinMaxNormalization(result,zMax,zMin);
        }

        internal static double TriangleArea(double x1, double y1, double x2, double y2, double x3, double y3)
        {
            // 三角形の面積を計算 (符号付き)
            return System.Math.Abs((x1 * (y2 - y3) + x2 * (y3 - y1) + x3 * (y1 - y2)) / 2.0);
        }

        /// <summary>
        /// 2点の距離を求める.
        /// </summary>
        /// <param name="vectorA">点a.</param>
        /// <param name="vectorB">点b.</param>
        /// <returns>2点の距離.</returns>
        internal static double Distance(Vector vectorA, Vector vectorB)
        {
            double result = System.Math.Sqrt(System.Math.Pow(vectorB.X - vectorA.X, 2) + System.Math.Pow(vectorB.Y - vectorA.Y, 2) + System.Math.Pow(vectorB.Z - vectorA.Z, 2));
            return result;
        }

        /// <summary>
        /// 距離を0から1に正規化する.
        /// </summary>
        /// <param name="distance">距離.</param>
        /// <param name="max">最大値.</param>
        /// <param name="min">最小値.</param>
        /// <returns>正規化された距離.</returns>
        private static double MinMaxNormalization(double distance, double max, double min)
        {
            double result = 0;

            if (min < distance && distance < max) // 上限値以下かつ下限値以上であれば正規化した値を代入
            {
                result = (distance - min) / (max - min);
            }
            else // それ以外の場合は -1 を代入
            {
                result = -1;
            }

            return result;
        }
    }
}
