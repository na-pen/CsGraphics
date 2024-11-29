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
        /// Initializes a new instance of the <see cref="Vertex"/> class.
        /// </summary>
        /// <param name="objectId">オブジェクトID.</param>
        /// <param name="coordinate">3次元空間上の座標(オブジェクト基準).</param>
        /// <param name="color">頂点の色.</param>
        internal Vertex(int objectId, double[,] coordinate, Color[]? color)
        {
            // 初期値の適用
            ObjectId = objectId;
            Coordinate = ConvertMatriix2Calcable(new Math.Matrix(coordinate));

            Color = new Color[coordinate.GetLength(1)];
            if (color != null)
            {
                if (coordinate.GetLength(1) != color.Length) // すべての頂点に対して色指定がされているか
                {
                    throw new ArgumentException("頂点の色を指定する場合は、すべての頂点に対して指定する必要があります。");
                }

                Color = color;
            }
            else
            {
                Array.Fill(Color, Colors.Black);
            }
        }

        private Vertex(int objectId, Matrix coordinate, Color[] color)
        {
            ObjectId = objectId;
            Coordinate = coordinate;
            Color = color;
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
                "ObjectID : " + ObjectId.ToString() + "\n" +
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
        /// 頂点の行列長を取得.
        /// </summary>
        /// <param name="dimension">取得する方向.</param>
        /// <returns>長さ.</returns>
        internal int GetLength(int dimension)
        {
            return Coordinate.GetLength(dimension);
        }

        public object Clone()
        {
            return new Vertex(
                ObjectId,
                (Matrix)Coordinate.Clone(),
                (Color[])Color.Clone()
                );
        }

        /*
        /// <summary>
        /// オブジェクトの基準座標と頂点座標から、2次元空間上の頂点を算出する
        /// </summary>
        /// <param name="rootCoordinate">オブジェクトの原点座標.</param>
        public Point CalcScreenCoordinate(Math.Vector rootCoordinate)
        {
            Math.Vector temp = rootCoordinate + this.Coordinate;
            return new Point(temp[0, 0], temp[0, 1]);
        }*/
    }
}