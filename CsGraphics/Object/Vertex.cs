namespace CsGraphics.Object
{
    using CsGraphics.Math;
    using Microsoft.Maui.Graphics;

    /// <summary>
    /// オブジェクトのすべての頂点の情報の保持や管理を行う.
    /// </summary>
    internal struct Vertex : ICloneable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Vertex"/> struct.
        /// </summary>
        /// <param name="objectId">オブジェクトID.</param>
        /// <param name="coordinate">3次元空間上の座標(オブジェクト基準).</param>
        /// <param name="color">頂点の色.</param>
        /// <exception cref="ArgumentException">頂点の色を指定する場合は、すべての頂点に対して指定する必要があります.</exception>
        internal Vertex(int objectId, double[,] coordinate, Color[]? color, double[][]? vt)
        {
            // 初期値の適用
            this.ObjectId = objectId;
            this.Coordinate = this.ConvertMatriix2Calcable(new Math.Matrix(coordinate));

            this.Color = new Color[coordinate.GetLength(1)];
            if (color != null)
            {
                if (coordinate.GetLength(1) != color.Length) // すべての頂点に対して色指定がされているか
                {
                    throw new ArgumentException("頂点の色を指定する場合は、すべての頂点に対して指定する必要があります。");
                }

                this.Color = color;
            }
            else
            {
                Array.Fill(this.Color, Colors.Black);
            }

            this.Vt = vt;
        }

        private Vertex(int objectId, Matrix coordinate, Color[] color, double[][] vt)
        {
            this.ObjectId = objectId;
            this.Coordinate = coordinate;
            this.Color = color;
            this.Vt = vt;
        }

        /// <summary>
        /// Gets オブジェクトID.
        /// </summary>
        internal int ObjectId { get; }

        /// <summary>
        /// Gets or Sets 3D空間上の3次元座標(オブジェクト基準).
        /// </summary>
        internal Math.Matrix Coordinate { get; set; }

        /// <summary>
        /// Gets or sets 頂点の色.
        /// </summary>
        internal Color[] Color { get; set; }

        internal double[][]? Vt { get; set; }

        /// <summary>
        /// 頂点情報をStringにする.
        /// </summary>
        /// <returns>頂点情報.</returns>
        public override string ToString() // Vertex test = new(0, new double[] { 0, 0, 0 });
        {
            string result = string.Empty;
            Math.Matrix coordinate = this.Coordinate;
            Color[] color = this.Color;

            result =
                "ObjectID : " + this.ObjectId.ToString() + "\n" +
                string.Join(
                    "\n\n",
                    Enumerable.Range(0, coordinate.GetLength(1)) // 各列を対象にする
                        .Select(i =>
                            $"ID : {i}\n" +
                            $"XYZ : ({coordinate[0, i]}, {coordinate[1, i]}, {coordinate[2, i]})\n" +
                            $"Color : ({color[i].Red}, {color[i].Green}, {color[i].Blue}, {color[i].Alpha})")
                        .ToArray());

            return result;
        }

        /// <summary>
        /// Cloneをするための実装.
        /// </summary>
        /// <returns>object.</returns>
        public object Clone()
        {
            return new Vertex(
                this.ObjectId,
                (Matrix)this.Coordinate.Clone(),
                (Color[])this.Color.Clone(),
                (double[][] ?)this.Vt.Clone());
        }

        /// <summary>
        /// 頂点の行列長を取得.
        /// </summary>
        /// <param name="dimension">取得する方向.</param>
        /// <returns>長さ.</returns>
        internal int GetLength(int dimension)
        {
            return this.Coordinate.GetLength(dimension);
        }

        /// <summary>
        /// データを3次元計算可能な状態に変換する.
        /// </summary>
        /// <param name="matrix">変換する行列.</param>
        /// <returns>行列.</returns>
        /// <exception cref="ArgumentException">データの次元数が誤っています.</exception>
        private Math.Matrix ConvertMatriix2Calcable(Math.Matrix matrix)
        {
            switch (matrix.GetLength(0))
            {
                case 2:
                    matrix.Resize(4, value: new double[] { 0, 1 });
                    break;

                case 3:
                    matrix.Resize(4, value: new double[] { 1 });
                    break;

                default:
                    throw new ArgumentException("データの次元数が誤っています");
            }

            return matrix;
        }
    }
}