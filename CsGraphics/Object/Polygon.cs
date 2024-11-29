namespace CsGraphics.Object
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

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
        internal Polygon(int objectId, int[][] vertexID)
        {
            this.ObjectId = objectId;
            this.VertexID = vertexID;
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
        /// 面の数や面の数を取得.
        /// </summary>
        /// <param name="dimension">取得する方向.</param>
        /// <returns>長さ.</returns>
        internal int GetLength(int dimension)
        {
            return this.VertexID.GetLength(dimension);
        }
    }
}
