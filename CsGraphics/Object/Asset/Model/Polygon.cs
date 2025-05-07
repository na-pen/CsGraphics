namespace CsGraphics.Object.Asset.Model
{
    using CsGraphics.Math;
    using System.Collections.Generic;

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
        internal Polygon(int objectId, Dictionary<string, int[][]> vertexID, Matrix normal, Dictionary<string, int[][]> mtlVertexID, Dictionary<string, int[][]>? normalId = null)
        {
            ObjectId = objectId;
            VertexID = vertexID;
            Normal = normal;
            Bounds = new Dictionary<string, double[,]>();
            MtlVertexID = mtlVertexID;
            NormalID = normalId;
        }

        /// <summary>
        /// Gets オブジェクトID.
        /// </summary>
        internal int ObjectId { get; }

        /// <summary>
        /// Gets or sets n個の頂点からなる多角形面の頂点IDを保存.
        /// </summary>
        internal Dictionary<string, int[][]> VertexID { get; set; }

        /// <summary>
        /// Gets or sets 法線ベクトル.
        /// </summary>
        internal Matrix Normal { get; set; }

        /// <summary>
        /// Gets or sets 各面ごとのScreen座標上のバウンディングボックス.
        /// </summary>
        internal Dictionary<string, double[,]> Bounds { get; set; }

        /// <summary>
        /// 各頂点のマテリアル座標のIDを取得.
        /// </summary>
        internal Dictionary<string, int[][]> MtlVertexID { get; set; }

        internal Dictionary<string, int[][]> NormalID { get; set; }


        /// <summary>
        /// 面の数を取得.
        /// </summary>
        /// <returns>長さ.</returns>
        internal int Length()
        {
            int result = 0;
            foreach (var kvp in VertexID)
            {
                int[][] array = kvp.Value;
                result += array.Length;
            }
            return result;
        }
    }
}
