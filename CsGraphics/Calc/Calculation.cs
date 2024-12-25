namespace CsGraphics.Calc
{
    using CsGraphics.Math;
    using System.Collections.Generic;


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

/* プロジェクト 'CsGraphics (net9.0-android35.0)' からのマージされていない変更
前:
        internal static (Point[], float[], Matrix) Calc(Asset.Object3D @object, Matrix matrixCam, float width, float height, bool mode = true, float fov = 60,float scaleParallelProjection = 32)
        {
後:
        internal static (Point[], float[], Matrix) Calc(Object3D @object, Matrix matrixCam, float width, float height, bool mode = true, float fov = 60,float scaleParallelProjection = 32)
        {
*/
        internal static (Point[], float[], Matrix) Calc(Asset.Object3d.Object3D @object, Matrix matrixCam, float width, float height, bool mode = true, float fov = 60,float scaleParallelProjection = 32)
        {
            List<float> depthZ = new List<float>(); // z深度 : 使用しない

            List<Point> result = new(); // 画面上の描画座標

            Matrix scale = CalcScale(@object); // 拡大縮小行列
            Matrix rotate = CalcRotation(@object); // 回転行列
            Matrix translate = CalcTranslation(@object); // 平行移動行列

            Matrix matrix = translate * rotate * scale; // 拡大縮小 → 回転 → 平行移動 をした変換行列を計算

            int far = -5000;
            float near = -1f;
            float left = -width / 2;
            float right = width / 2;
            float bottom = height / 2;
            float top = -height / 2;
            float aspect = width / height;
            float fovY = float.DegreesToRadians(fov);
            float f = (float)(1f / System.MathF.Tan(fovY / 2f));

            Matrix cam2view;

            if (mode) // 透視投影のとき
            {
                cam2view = new (new float[,]
                {
                    { -f / aspect, 0, 0, 0 },
                    { 0, f, 0, 0 },
                    { 0, 0, -1f * (far + near) / (far - near), -2f * (far * near) / (far - near) },
                    { 0, 0, -1f, 0 },
                });
            }
            else // 平行投影のとき
            {
                left = -width / scaleParallelProjection;
                right = width / scaleParallelProjection;
                bottom = height / scaleParallelProjection;
                top = -height / scaleParallelProjection;

                cam2view = new (new float[,]
                {
                    { 2f / (right - left), 0, 0, -(right + left) / (right - left) },
                    { 0, 2f / (top - bottom), 0, -(top + bottom) / (top - bottom) },
                    { 0, 0, -2f / (far - near), -(far + near) / (far - near)},
                    { 0, 0, 0, 1 },
                });
            }




            Matrix vertex = cam2view * matrixCam * matrix * @object.Vertex.Coordinate;

            Matrix coordinate = new Matrix(@object.Vertex.Coordinate.GetLength(0), @object.Vertex.Coordinate.GetLength(1));
            for (int n = 0; n < @object.Vertex.Coordinate.GetLength(1); n++)
            {
                Matrix t = new(new float[] { vertex[0, n], vertex[1, n], vertex[2, n], vertex[3, n] });
                result.Add(new Point((vertex[0, n] / vertex[3, n] + 1) * (width / 2), (1 - vertex[1, n] / vertex[3, n]) * (height / 2))); // スクリーン上の座標を求める計算 この場合はそのままコピー

                // 計算後の3D頂点座標を代入
                coordinate[0, n] = (vertex[0, n] / vertex[3, n] + 1) * (width / 2);
                coordinate[1, n] = (1 - vertex[1, n] / vertex[3, n]) * (height / 2);
                coordinate[2, n] = vertex[2, n] / vertex[3, n];
                coordinate[3, n] = vertex[3, n];
            }

            // ポリゴンの法線がz=0の面とどの向きで交差するかどうか確認する
            if (@object.Polygon != null)
            {
                GetPolygonBounds(result, (Asset.Object3d.Polygon)@object.Polygon); // 面ごとの画面上の描画範囲を求める
            }

            return (result.ToArray(), depthZ.ToArray(), coordinate);
        }
        private static void GetPolygonBounds(List<Point> points, Asset.Object3d.Polygon polygon)
        {
            foreach (var kvp in polygon.VertexID)
            {
                string key = kvp.Key;

                int j = 0;
                double[,] tempArr = new double[polygon.VertexID[key].Length, 4];
                foreach (int[] t in polygon.VertexID[key])
                {
                    Point[] temp = new Point[t.Length];
                    for (int i = 0; i < t.Length; i++)
                    {
                        temp[i] = points[t[i] - 1];
                    }

                    tempArr[j, 0] = temp.Min(p => p.X); // x軸の最小値
                    tempArr[j, 1] = temp.Max(p => p.X); // x軸の最大値
                    tempArr[j, 2] = temp.Min(p => p.Y); // y軸の最小値
                    tempArr[j, 3] = temp.Max(p => p.Y); // y軸の最大値


                    j++;
                }

                if (!polygon.Bounds.TryAdd(key, tempArr))
                {
                    polygon.Bounds[key] = tempArr;
                }
            }
        }

        /// <summary>
        /// オブジェクトの移動を計算する.
        /// </summary>
        /// <returns>移動の行列.</returns>

/* プロジェクト 'CsGraphics (net9.0-android35.0)' からのマージされていない変更
前:
        private static Matrix CalcTranslation(Asset.Object3D @object)
        {
後:
        private static Matrix CalcTranslation(Object3D @object)
        {
*/
        private static Matrix CalcTranslation(Asset.Object3d.Object3D @object)
        {
            Matrix temp = new(4);
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

/* プロジェクト 'CsGraphics (net9.0-android35.0)' からのマージされていない変更
前:
        private static Matrix CalcScale(Asset.Object3D @object)
        {
後:
        private static Matrix CalcScale(Object3D @object)
        {
*/
        private static Matrix CalcScale(Asset.Object3d.Object3D @object)
        {
            Matrix temp = new(4);
            temp.Identity();
            Enumerable.Range(0, 3).ToList().ForEach(i => temp[i, i] = @object.Scale[i]);

            return temp;
        }

        /// <summary>
        /// 行列を用いて、YXZの順に回転を計算する.
        /// </summary>
        /// <returns>回転行列.</returns>

/* プロジェクト 'CsGraphics (net9.0-android35.0)' からのマージされていない変更
前:
        private static Matrix CalcRotation(Asset.Object3D @object)
        {
後:
        private static Matrix CalcRotation(Object3D @object)
        {
*/
        private static Matrix CalcRotation(Asset.Object3d.Object3D @object)
        {
            Matrix xAxis = new(4);
            xAxis.Identity();
            xAxis[1, 1] = System.MathF.Cos(@object.Angle[0]);
            xAxis[2, 1] = System.MathF.Sin(@object.Angle[0]);
            xAxis[1, 2] = -1f * System.MathF.Sin(@object.Angle[0]);
            xAxis[2, 2] = System.MathF.Cos(@object.Angle[0]);

            Matrix yAxis = new(4);
            yAxis.Identity();
            yAxis[0, 0] = System.MathF.Cos(@object.Angle[1]);
            yAxis[2, 0] = -1f * System.MathF.Sin(@object.Angle[1]);
            yAxis[0, 2] = System.MathF.Sin(@object.Angle[1]);
            yAxis[2, 2] = System.MathF.Cos(@object.Angle[1]);

            Matrix zAxis = new(4);
            zAxis.Identity();
            zAxis[0, 0] = System.MathF.Cos(@object.Angle[2]);
            zAxis[0, 1] = -1f * System.MathF.Sin(@object.Angle[2]);
            zAxis[1, 0] = System.MathF.Sin(@object.Angle[2]);
            zAxis[1, 1] = System.MathF.Cos(@object.Angle[2]);

            return yAxis * xAxis * zAxis;
        }
    }
}
