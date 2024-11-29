namespace CsGraphics
{
    using CsGraphics.Math;

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
        /// 原点から真横に見て描画したとき.
        /// </summary>
        /// <param name="object">オブジェクト.</param>
        /// <returns>スクリーン座標のリスト.</returns>
        private static (Point[], Color[], bool[]) DrawFromOrigin(Object.Object @object)
        {
            List<Point> result = new ();
            bool[] isVisiblePolygon = Array.Empty<bool>();

            Math.Matrix scale = CalcScale(@object);
            Math.Matrix rotate = CalcRotation(@object);
            Math.Matrix translate = CalcTranslation(@object);

            Matrix matrix = translate * rotate * scale; // 拡大縮小 → 回転 → 平行移動 をした変換行列を計算

            foreach (Math.Matrix vertex in matrix * @object.Vertex.Coordinate) // 変換行列 * 頂点行列を行い、移動後の3D空間上の頂点座標を計算する
            {
                result.Add(new Point(vertex[0, 0], vertex[1, 0]));
            }

            // ポリゴンの法線がz=0の面とどの向きで交差するかどうか確認する
            if (@object.Polygon != null)
            {
                isVisiblePolygon = new bool[((Object.Polygon)@object.Polygon).Length()];
                for (int i = 0; i < ((Object.Polygon)@object.Polygon).Length(); i++)
                {
                    // 各ポリゴンの法線ベクトルに対して、オブジェクトの移動を反映する
                    // 法線ベクトル.z > 0 ならば正の向きで交差する.
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
            }

            return (result.ToArray(), @object.Vertex.Color, isVisiblePolygon);
        }

        private static Math.Matrix CalcTranslation(Object.Object @object)
        {
            Math.Matrix temp = new (4);
            temp.Identity();
            temp[0, 3] = @object.Origin[0, 0];
            temp[1, 3] = @object.Origin[1, 0];
            temp[2, 3] = @object.Origin[2, 0];

            return temp;
        }

        private static Math.Matrix CalcScale(Object.Object @object)
        {
            Math.Matrix temp = new (4);
            temp.Identity();
            Enumerable.Range(0, 3).ToList().ForEach(i => temp[i, i] = @object.Magnification[i]);

            return temp;
        }

        /// <summary>
        /// 回転行列を用いて、YXZの順に回転を計算する.
        /// </summary>
        /// <returns>回転後の座標.</returns>
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
