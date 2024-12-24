namespace CsGraphics.Asset.Object3d
{
    using CsGraphics.Math;
    using Microsoft.Maui.Graphics;

    /// <summary>
    /// オブジェクトのすべての頂点の情報の保持や管理を行う.
    /// </summary>
    internal struct Vertex // : ICloneable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Vertex"/> struct.
        /// </summary>
        /// <param name="objectId">オブジェクトID.</param>
        /// <param name="coordinate">3次元空間上の座標(オブジェクト基準).</param>
        /// <exception cref="ArgumentException">頂点の色を指定する場合は、すべての頂点に対して指定する必要があります.</exception>
        internal Vertex(int objectId, float[,] coordinate, float[][]? vt)
        {
            // 初期値の適用
            ObjectId = objectId;
            Coordinate = ConvertMatriix2Calcable(new Matrix(coordinate));

            Vt = vt;
        }

        private Vertex(int objectId, Matrix coordinate, float[][] vt)
        {
            ObjectId = objectId;
            Coordinate = coordinate;
            Vt = vt;
        }

        /// <summary>
        /// Gets オブジェクトID.
        /// </summary>
        internal int ObjectId { get; }

        /// <summary>
        /// Gets or Sets 3D空間上の3次元座標(オブジェクト基準).
        /// </summary>
        internal Matrix Coordinate { get; set; }

        internal float[][]? Vt { get; set; }

        /// <summary>
        /// 頂点情報をStringにする.
        /// </summary>
        /// <returns>頂点情報.</returns>
        public override string ToString() // Vertex test = new(0, new float[] { 0, 0, 0 });
        {
            string result = string.Empty;
            Matrix coordinate = Coordinate;

            result =
                "ObjectID : " + ObjectId.ToString() + "\n" +
                string.Join(
                    "\n\n",
                    Enumerable.Range(0, coordinate.GetLength(1)) // 各列を対象にする
                        .Select(i =>
                            $"ID : {i}\n" +
                            $"XYZ : ({coordinate[0, i]}, {coordinate[1, i]}, {coordinate[2, i]})\n")
                        .ToArray());

            return result;
        }

        /// <summary>
        /// Cloneをするための実装.
        /// </summary>
        /// <returns>object.</returns>
        /*
        public object Clone()
        {
            return new Vertex(
                ObjectId,
                (Matrix)Coordinate.Clone(),
                Vt);
        }*/

        /// <summary>
        /// 頂点の行列長を取得.
        /// </summary>
        /// <param name="dimension">取得する方向.</param>
        /// <returns>長さ.</returns>
        internal int GetLength(int dimension)
        {
            return Coordinate.GetLength(dimension);
        }

        /// <summary>
        /// データを3次元計算可能な状態に変換する.
        /// </summary>
        /// <param name="matrix">変換する行列.</param>
        /// <returns>行列.</returns>
        /// <exception cref="ArgumentException">データの次元数が誤っています.</exception>
        private Matrix ConvertMatriix2Calcable(Matrix matrix)
        {
            switch (matrix.Rows)
            {
                case 2:
                    matrix.Resize(4, value: new float[] { 0, 1 });
                    break;

                case 3:
                    matrix.Resize(4, value: new float[] { 1 });
                    break;

                default:
                    throw new ArgumentException("データの次元数が誤っています");
            }

            return matrix;
        }
    }
}