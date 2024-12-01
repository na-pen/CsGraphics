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
        /// <param name="viewPlaneEquation">描画面の方程式.</param>
        /// <param name="viewPixel">描画面上の点.</param>
        /// <param name="polygonEquation">描画対象のポリゴン.</param>
        /// <returns>z深度.</returns>
        internal static double ZDepsParallel(Vector viewPlaneEquation, Vector viewPixel, Vector polygonEquation, double[] polygonPointA, double[] polygonPointB, double[] polygonPointC)
        {
            double result = 0;
            viewPixel.W = 1; // viewPixel は座標なので4次元目を1とする

            Vector o = new (viewPixel); // 光学中心の座標: 平行投影の場合はScreen座標(zのみ-1)
            o.Z = -1; // 要修正: 描画面がz=n のときのみ有効

            double top = -1 * Vector.DotProduct(polygonEquation, viewPixel);
            double bottom = Vector.DotProduct(polygonEquation, o) - Vector.DotProduct(polygonEquation, viewPixel);
            double t = top / bottom;

            if (bottom != 0)
            {
                Vector q = viewPixel + (t * (o - viewPixel)); // ポリゴン上の点Q
                Math.Vector ac = new Math.Vector(polygonPointA, polygonPointC); // ACベクトル
                Math.Vector ab = new Math.Vector(polygonPointA, polygonPointB); // ABベクトル
                Math.Vector aq = new Math.Vector(polygonPointA, new double[] { q.X, q.Y, q.Z }); // AQベクトル

                double d00 = Math.Vector.DotProduct(ac, ac);
                double d01 = Math.Vector.DotProduct(ac, ab);
                double d11 = Math.Vector.DotProduct(ab, ab);
                double d20 = Math.Vector.DotProduct(aq, ac);
                double d21 = Math.Vector.DotProduct(aq, ab);

                Math.Matrix m = new (new double[,]
                    {
                        { d00, d01 },
                        { d01, d11 },
                    });
                Math.Matrix mI = m.Inverse();

                Math.Matrix b = new (new double[,]
                    {
                        { d20 },
                        { d21 },
                    });

                Math.Matrix r = mI * b;

                double r1 = 1 - r[0, 0] - r[1, 0];

                if (r1 > 0 && r[0, 0] > 0 && r[1, 0] > 0)
                {
                    double distancePixelQ = Distance(viewPixel, q);

                    const int z_max = 1920; // 描画基準面から奥行方向への最大描画距離
                    result = MinMaxNormalization(distancePixelQ, z_max, 0); // 値を正規化
                }
                else
                {
                    result = -1;
                }

            }
            else
            {
                result = -1;
            }

            return result;
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
