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

            return DrawFromOrigin((Object.Object)@object.Clone());
        }

        /// <summary>
        /// 原点から真横に見て描画したとき.
        /// </summary>
        /// <param name="object">オブジェクト.</param>
        /// <returns>スクリーン座標のリスト.</returns>
        private static (Point[], Color[]) DrawFromOrigin(Object.Object @object)
        {
            List<Point> result = new ();
            @object = CalcScale(@object);
            foreach (Math.Matrix vertex in @object.Vertex)
            {
                Math.Matrix temp = vertex + @object.Origin;
                result.Add(new Point(temp[0, 0], temp[1, 0]));
            }

            return (result.ToArray(), @object.Vertex.Color);
        }

        private static Object.Object CalcScale(Object.Object @object)
        {
            Math.Matrix temp = new(4);
            temp.Identity();
            Enumerable.Range(0, 3).ToList().ForEach(i => temp[i, i] = @object.Magnification[i, 0]);
            @object.Vertex.Coordinate = temp * @object.Vertex.Coordinate;

            return @object;
        }
    }

}
