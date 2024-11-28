namespace CsGraphics
{
    using Microsoft.Maui.Graphics;

    /// <summary>
    /// オブジェクトのすべての頂点の情報の保持や管理を行う.
    /// </summary>
    public class Vertex
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Vertex"/> class.
        /// </summary>
        /// <param name="objectId">頂点ID.</param>
        /// <param name="coordinate">3次元空間上の座標(オブジェクト基準).</param>
        /// <param name="color">頂点の色.</param>
        /// <param name="visible">頂点の可視/不可視.</param>
        public Vertex(int objectId, double[,] coordinate, Color[]? color = null, bool visible = true)
        {
            // 初期値の適用
            this.ObjectId = objectId;
            this.Coordinate = this.ConvertMatriix2Calcable( new Math.Matrix(coordinate));
            this.IsVisible = visible;

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
        }

        /// <summary>
        /// Gets オブジェクトID.
        /// </summary>
        public int ObjectId { get; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets a value indicating visible. 可視/不可視状態.
        /// </summary>
        public bool IsVisible { get; set; } = true;

        /// <summary>
        /// Gets 3D空間上の3次元座標(オブジェクト基準).
        /// </summary>
        public Math.Matrix Coordinate { get; }

        /// <summary>
        /// Gets or sets 頂点の色.
        /// </summary>
        public Color[] Color { get; set; }

        private Math.Matrix ConvertMatriix2Calcable(Math.Matrix matrix)
        {
            matrix.Resize(4, value: new double[] { 0, 1 });
            return matrix;
        }

        /// <summary>
        /// 頂点情報をStringにする.
        /// </summary>
        /// <returns>頂点情報.</returns>
        public override string ToString() // Vertex test = new(0, new double[] { 0, 0, 0 });
        {
            string result = string.Empty;


            result =
                "ObjectID : " + this.ObjectId.ToString() + "\n" +
                "Visible : " + this.IsVisible.ToString() + "\n" +
                string.Join(
                    "\n\n",
                    Enumerable.Range(0, this.Coordinate.GetLength(1)) // 各列を対象にする
                        .Select(i =>
                            $"ID : {i}\n" +
                            $"XYZ : ({this.Coordinate[0, i]}, {this.Coordinate[1, i]}, {this.Coordinate[2, i]})\n" +
                            $"Color : ({this.Color[i].Red}, {this.Color[i].Green}, {this.Color[i].Blue}, {this.Color[i].Alpha})")
                        .ToArray());

            return result;
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