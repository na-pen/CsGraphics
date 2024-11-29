using Microsoft.Maui.Primitives;

namespace CsGraphics.Object
{
    /// <summary>
    /// オブジェクトの情報の保持や管理を行う.
    /// </summary>
    public class Object : ICloneable
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
        public Object(string name, double[,] vertexCoord, int id = -1, Color[]? vertexColor = null, double[]? origin = null, bool visible = true, double[]? scale = null)
        {
            this.ID = id;
            this.Name = name;
            this.IsVisible = visible;

            if (origin == null)
            {
                this.Origin = new (new double[,] { { 0 }, { 0 }, { 0 } });
            }
            else
            {
                this.Origin = new (origin);
            }

            if (scale == null)
            {
                this.Magnification = new (new double[,] { { 1 }, { 1 }, { 1 } });
            }
            else
            {
                this.Magnification = new (scale);
            }

            this.Origin.Resize(4, value: new double[] { 1D });
            this.Vertex = new (id, vertexCoord, vertexColor);
        }

        private Object(string name, Vertex vertex, int id, Math.Matrix origin, Math.Matrix magnification, bool visible)
        {
            this.Name = name;
            this.IsVisible = visible;
            this.Origin = origin;
            this.ID = id;
            this.Vertex = vertex;
            this.Magnification = magnification;
        }

        /// <summary>
        /// Gets or sets オブジェクトID.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets オブジェクト名.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets or sets a value indicating whether オブジェクトの表示状態.
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// Gets or sets オブジェクトの原点.
        /// </summary>
        public Math.Matrix Origin { get; set; }

        /// <summary>
        /// Gets or sets オブジェクトの拡大倍数.
        /// </summary>
        public Math.Matrix Magnification { get; set; }

        /// <summary>
        /// Gets or sets 頂点情報.
        /// </summary>
        public Vertex Vertex { get; set; }

        /// <summary>
        /// 平行移動(オブジェクトの原点を移動).
        /// </summary>
        /// <param name="x">x軸の移動量.</param>
        /// <param name="y">y軸の移動量.</param>
        /// <param name="z">z軸の移動量.</param>
        public void Translation(double x, double y, double z)
        {
            Math.Matrix temp = new (4);
            temp.Identity();

            temp[0, 3] = x;
            temp[1, 3] = y;
            temp[2, 3] = z;

            this.Origin = temp * this.Origin;
        }

        /// <summary>
        /// 拡大・縮小(オブジェクトの原点基準).
        /// </summary>
        /// <param name="x">x軸の拡大率.</param>
        /// <param name="y">y軸の拡大率.</param>
        /// <param name="z">z軸の拡大率.</param>
        public void Scale(double x, double y, double z)
        {
            this.Magnification = new (new double[,] { { x }, { y }, { z } });
        }

        /// <summary>
        /// 自身をシャドーコピーする.
        /// </summary>
        /// <returns>Clone.</returns>
        public object Clone()
        {
            return new Object(
                this.Name,
                (Vertex)this.Vertex.Clone(),
                this.ID,
                (Math.Matrix)this.Origin.Clone(),
                (Math.Matrix)this.Magnification.Clone(),
                this.IsVisible)
            {
            };
        }
    }
}
