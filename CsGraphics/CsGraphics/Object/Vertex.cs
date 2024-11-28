namespace CsGraphics {
    using Microsoft.Maui.Graphics;

    /// <summary>
    /// 頂点の情報の保持や管理を行う.
    /// </summary>
    public class Vertex
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Vertex"/> class.
        /// </summary>
        /// <param name="id">頂点ID.</param>
        /// <param name="coordinate">3次元空間上の座標(オブジェクト基準).</param>
        /// <param name="color">頂点の色.</param>
        /// <param name="visible">頂点の可視/不可視.</param>
        public Vertex(int id, double[] coordinate, Color? color = null, bool visible = true)
        {
            if (coordinate.Length != 3) // 頂点座標が3次元かどうか
            {
                throw new ArgumentException("配列の長さが正しくありません。長さは3である必要があります。");
            }

            // 初期値の適用
            this.ID = id;
            this.Coordinate = new Math.Matrix(coordinate);
            this.IsVisible = visible;
            if (color != null)
            {
                this.Color = color;
            }

        }

        /// <summary>
        /// Gets 頂点ID(オブジェクト内において一意).
        /// </summary>
        public int ID { get; }

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
        public Color Color { get; set; } = Colors.Black;

        /// <summary>
        /// 頂点情報をStringにする.
        /// </summary>
        /// <returns>頂点情報.</returns>
        public override string ToString()
        {
            string result = string.Empty;

            result =
                "ID : " + this.ID.ToString() + " " +
                "Visible : " + this.IsVisible.ToString() + "\n" +
                "XYZ : (" + this.Coordinate[0,0].ToString() + "," + this.Coordinate[1,0].ToString() + "," + this.Coordinate[2,0].ToString() + ")" + "\n" +
                "Color : " + this.Color.ToString();

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