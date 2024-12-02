namespace CsGraphics.Object
{
    using System;
    using CsGraphics.Math;

    /// <summary>
    /// ポリゴンの情報を保持する.
    /// </summary>
    internal struct Polygon
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Polygon"/> struct.
        /// </summary>
        /// <param name="objectId">オブジェクトID.</param>
        /// <param name="vertexID">多角形面の頂点ID.</param>
        /// <param name="normal">法線ベクトル.</param>
        internal Polygon(int objectId, int[][] vertexID, Matrix[] normal, Color[] color)
        {
            this.ObjectId = objectId;
            this.VertexID = vertexID;
            this.Normal = normal;
            this.Bounds = new double[normal.Length, 4];
            this.NormalCalced = normal;
            this.Colors = color;
        }

        /// <summary>
        /// Gets オブジェクトID.
        /// </summary>
        internal int ObjectId { get; }

        /// <summary>
        /// Gets or sets n個の頂点からなる多角形面の頂点IDを保存.
        /// </summary>
        internal int[][] VertexID { get; set; }

        /// <summary>
        /// Gets or sets 法線ベクトル.
        /// </summary>
        internal Matrix[] Normal { get; set; }

        /// <summary>
        /// Gets or sets 計算後の法線ベクトル.
        /// </summary>
        internal Matrix[] NormalCalced { get; set; }

        /// <summary>
        /// Gets or sets 各面ごとのScreen座標上のバウンディングボックス.
        /// </summary>
        internal double[,] Bounds { get; set; }

        internal Color[] Colors { get; set; }

        /// <summary>
        /// 面の数や面の数を取得.
        /// </summary>
        /// <returns>長さ.</returns>
        internal int Length()
        {
            return this.VertexID.Length;
        }
    }
}
