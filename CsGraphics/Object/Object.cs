using Microsoft.Maui.Primitives;

namespace CsGraphics.Object
{
    /// <summary>
    /// オブジェクトの情報の保持や管理を行う.
    /// </summary>
    internal class Object : ICloneable
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="Object"/> class.
        /// </summary>
        /// <param name="id">オブジェクトID.</param>
        /// <param name="name">オブジェクト名.</param>
        /// <param name="vertexCoord">頂点座標.</param>
        /// <param name="vertexColor">頂点色.</param>
        /// <param name="origin">オブジェクトの原点.</param>
        /// <param name="visible">オブジェクトの表示状態.</param>
        /// <param name="scale">オブジェクトの拡大倍率.</param>
        internal Object(string name, double[,] vertexCoord, int id = -1, Color[]? vertexColor = null, double[]? origin = null, bool visible = true, double[]? scale = null)
        {
            ID = id;
            Name = name;
            IsVisible = visible;

            if (origin == null)
            {
                Origin = new(new double[,] { { 0 }, { 0 }, { 0 } });
            }
            else
            {
                Origin = new(origin);
            }

            if (scale == null)
            {
                Magnification = new double[] { 1, 1, 1 };
            }
            else
            {
                Magnification = scale;
            }

            Vertex = new(id, vertexCoord, vertexColor);
        }

        private Object(string name, Vertex vertex, int id, Math.Matrix origin, double[] magnification, bool visible, double[] angle)
        {
            Name = name;
            IsVisible = visible;
            Origin = origin;
            ID = id;
            Vertex = vertex;
            Magnification = magnification;
            Angle = angle;
        }

        /// <summary>
        /// Gets or sets オブジェクトID.
        /// </summary>
        internal int ID { get; set; }

        /// <summary>
        /// Gets オブジェクト名.
        /// </summary>
        internal string Name { get; }

        /// <summary>
        /// Gets or sets a value indicating whether オブジェクトの表示状態.
        /// </summary>
        internal bool IsVisible { get; set; }

        /// <summary>
        /// Gets or sets オブジェクトの原点.
        /// </summary>
        internal Math.Matrix Origin { get; set; }

        /// <summary>
        /// Gets or sets オブジェクトの拡大倍数.
        /// </summary>
        internal double[] Magnification { get; set; }

        /// <summary>
        /// Gets or sets 頂点情報.
        /// </summary>
        internal Vertex Vertex { get; set; }

        /// <summary>
        /// Gets or sets オブジェクトの傾き.
        /// </summary>
        internal double[] Angle { get; set; } = { 0, 0, 0 };

        /// <summary>
        /// 平行移動(オブジェクトの原点を移動).
        /// </summary>
        /// <param name="x">x軸の移動量.</param>
        /// <param name="y">y軸の移動量.</param>
        /// <param name="z">z軸の移動量.</param>
        internal void SetTranslation(double x, double y, double z)
        {
            Math.Matrix temp = new(3, 1);

            temp[0, 0] = x;
            temp[1, 0] = y;
            temp[2, 0] = z;

            this.Origin += temp;
        }

        /// <summary>
        /// 拡大・縮小(オブジェクトの原点基準).
        /// </summary>
        /// <param name="x">x軸の拡大率.</param>
        /// <param name="y">y軸の拡大率.</param>
        /// <param name="z">z軸の拡大率.</param>
        internal void SetScale(double x, double y, double z)
        {
            Magnification = new double[] { Magnification[0] * x, Magnification[1] * y, Magnification[2] * z };
        }

        public void SetRotation(double x, double y, double z)
        {
            this.Angle = new double[] { Angle[0] + x, Angle[1] + y, Angle[2] + z };
        }

        /// <summary>
        /// 自身をシャドーコピーする.
        /// </summary>
        /// <returns>Clone.</returns>
        public object Clone()
        {
            return new Object(
                Name,
                (Vertex)Vertex.Clone(),
                ID,
                (Math.Matrix)Origin.Clone(),
                (double[])Magnification.Clone(),
                IsVisible,
                Angle)
            {
            };
        }
    }
}
