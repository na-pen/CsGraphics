namespace CsGraphics
{
    using CsGraphics.Math;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// オブジェクトの情報から画面上の座標を計算する.
    /// </summary>
    internal static class Calculation
    {
        /// <summary>
        /// オブジェクトの情報から画面上の座標を計算する.
        /// </summary>
        /// <param name="object">オブジェクト.</param>
        /// <returns>スクリーン座標のリスト.</returns>
        internal static (Point[], Color[], bool[]) Calc(Object.Object @object)
        {
            return DrawFromOrigin(@object);
        }

        /// <summary>
        /// 原点から平行投影で真横に見て描画したとき.
        /// </summary>
        /// <param name="object">オブジェクト.</param>
        /// <returns>スクリーン座標のリスト.</returns>
        private static (Point[], Color[], bool[]) DrawFromOrigin(Object.Object @object)
        {
            const int z_max = 1920; // 描画基準面から奥行方向への最大描画距離
            List<double> depthZ = new List<double>(); // z深度

            List<Point> result = new (); // 画面上の描画座標
            bool[] isVisiblePolygon = Array.Empty<bool>(); // 面を描画するかどうかのフラグ

            Math.Matrix scale = CalcScale(@object); // 拡大縮小行列
            Math.Matrix rotate = CalcRotation(@object); // 回転行列
            Math.Matrix translate = CalcTranslation(@object); // 平行移動行列

            Matrix matrix = translate * rotate * scale; // 拡大縮小 → 回転 → 平行移動 をした変換行列を計算

            foreach (Math.Matrix vertex in matrix * @object.Vertex.Coordinate) // 変換行列 * 頂点行列を行い、移動後の3D空間上の頂点座標を計算する
            {
                result.Add(new Point(vertex[0, 0], vertex[1, 0])); // スクリーン上の座標を求める計算 この場合はそのままコピー

                depthZ.Add(vertex[2, 0]); // カメラから見た各頂点の深度情報を保存 この場合はz軸の値をコピー
                depthZ = MinMaxNormalization(depthZ, 0, z_max); // 値を正規化
            }

            // ポリゴンの法線がz=0の面とどの向きで交差するかどうか確認する
            if (@object.Polygon != null)
            {
                isVisiblePolygon = new bool[((Object.Polygon)@object.Polygon).Length()];
                for (int i = 0; i < ((Object.Polygon)@object.Polygon).Length(); i++)
                {
                    // 各ポリゴンの法線ベクトルに対して、オブジェクトの移動を反映する
                    // この場合、法線ベクトル.z > 0 ならば正の向きで交差する.
                    // 交差するときのみ、描画フラグを立てる
                    if ((matrix * ((Object.Polygon)@object.Polygon).Normal[i])[2, 0] > 0)
                    {
                        isVisiblePolygon[i] = true;
                    }
                    else
                    {
                        isVisiblePolygon[i] = false;
                    }
                }

                GetPolygonBounds(result, (Object.Polygon)@object.Polygon, isVisiblePolygon); // 面ごとの画面上の描画範囲を求める
            }

            return (result.ToArray(), @object.Vertex.Color, isVisiblePolygon);
        }

        private static void GetPolygonBounds(List<Point> points, Object.Polygon polygon, bool[] isVisiblePolygon)
        {
            int j = 0;
            foreach (int[] t in polygon.VertexID)
            {
                if (isVisiblePolygon[j])
                {
                    Point[] temp = new Point[t.Length];
                    for (int i = 0; i < t.Length; i++)
                    {
                        temp[i] = points[t[i] - 1];
                    }

                    polygon.Bounds[j, 0] = (short)temp.Min(p => p.X); // x軸の最小値
                    polygon.Bounds[j, 1] = (short)temp.Max(p => p.X); // x軸の最大値
                    polygon.Bounds[j, 2] = (short)temp.Min(p => p.Y); // y軸の最小値
                    polygon.Bounds[j, 3] = (short)temp.Max(p => p.Y); // y軸の最大値
                }
                else
                {
                    polygon.Bounds[j, 0] = -1; // x軸の最小値
                    polygon.Bounds[j, 1] = -1; // x軸の最大値
                    polygon.Bounds[j, 2] = -1; // y軸の最小値
                    polygon.Bounds[j, 3] = -1; // y軸の最大値
                }
                j++;
            }
        }

        private static void CalcBoundingBoxOnScreen()
        {

        }

        private static List<double> MinMaxNormalization(List<double> data,double max, double min)
        {
            List<double> result = new ();
            foreach (double d in data)
            {
                if (min < d && d < max) // 上限値以下かつ下限値以上であれば正規化した値を代入
                {
                    result.Add((d - min) / (max - min));
                }
                else // それ以外の場合は -1 を代入
                {
                    result.Add(-1);
                }
            }

            return result;
        }

        /// <summary>
        /// オブジェクトの移動を計算する.
        /// </summary>
        /// <returns>移動の行列.</returns>
        private static Math.Matrix CalcTranslation(Object.Object @object)
        {
            Math.Matrix temp = new (4);
            temp.Identity();
            temp[0, 3] = @object.Origin[0, 0];
            temp[1, 3] = @object.Origin[1, 0];
            temp[2, 3] = @object.Origin[2, 0];

            return temp;
        }

        /// <summary>
        /// オブジェクトの拡大縮小を計算する.
        /// </summary>
        /// <returns>拡大縮小の行列.</returns>
        private static Math.Matrix CalcScale(Object.Object @object)
        {
            Math.Matrix temp = new (4);
            temp.Identity();
            Enumerable.Range(0, 3).ToList().ForEach(i => temp[i, i] = @object.Magnification[i]);

            return temp;
        }

        /// <summary>
        /// 行列を用いて、YXZの順に回転を計算する.
        /// </summary>
        /// <returns>回転行列.</returns>
        private static Math.Matrix CalcRotation(Object.Object @object)
        {
            Math.Matrix xAxis = new (4);
            xAxis.Identity();
            xAxis[1, 1] = System.Math.Cos(@object.Angle[0]);
            xAxis[2, 1] = System.Math.Sin(@object.Angle[0]);
            xAxis[1, 2] = -1 * System.Math.Sin(@object.Angle[0]);
            xAxis[2, 2] = System.Math.Cos(@object.Angle[0]);

            Math.Matrix yAxis = new (4);
            yAxis.Identity();
            yAxis[0, 0] = System.Math.Cos(@object.Angle[1]);
            yAxis[2, 0] = -1 * System.Math.Sin(@object.Angle[1]);
            yAxis[0, 2] = System.Math.Sin(@object.Angle[1]);
            yAxis[2, 2] = System.Math.Cos(@object.Angle[1]);

            Math.Matrix zAxis = new (4);
            zAxis.Identity();
            zAxis[0, 0] = System.Math.Cos(@object.Angle[2]);
            zAxis[0, 1] = -1 * System.Math.Sin(@object.Angle[2]);
            zAxis[1, 0] = System.Math.Sin(@object.Angle[2]);
            zAxis[1, 1] = System.Math.Cos(@object.Angle[2]);

            return yAxis * xAxis * zAxis;
        }


    }
}
