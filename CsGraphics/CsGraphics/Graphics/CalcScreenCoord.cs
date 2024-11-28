namespace CsGraphics
{
    /// <summary>
    /// オブジェクトの情報から画面上の座標を計算する.
    /// </summary>
    internal static class CalcScreenCoord
    {
        /// <summary>
        /// オブジェクトの情報から画面上の座標を計算する.
        /// </summary>
        /// <param name="object">オブジェクト.</param>
        /// <returns>スクリーン座標のリスト.</returns>
        internal static (Point[], Color[]) Calc(Object.Object @object)
        {
            return DrawFromOrigin(@object);
        }

        /// <summary>
        /// 原点から真横に見て描画したとき.
        /// </summary>
        /// <param name="object">オブジェクト.</param>
        /// <returns>スクリーン座標のリスト.</returns>
        private static (Point[], Color[]) DrawFromOrigin(Object.Object @object)
        {
            List<Point> result = new ();
            foreach (Math.Matrix vertex in @object.Vertex)
            {
                Math.Matrix temp = vertex + @object.Origin;
                result.Add(new Point(temp[0, 0], temp[1, 0]));
            }

            return (result.ToArray(), @object.Vertex.Color);
        }
    }

}
