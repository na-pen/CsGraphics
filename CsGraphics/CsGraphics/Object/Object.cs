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
        /// <param name="vidible">オブジェクトの表示状態.</param>
        /// <param name="scale">オブジェクトの拡大倍率.</param>
        public Object(string name, double[,] vertexCoord, int id = -1, Color[]? vertexColor = null, double[]? origin = null, bool vidible = true, double[]? scale = null)
        {
            this.ID = id;
            this.Name = name;
            this.IsVisible = vidible;

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

        private Object(string name, Vertex vertex, int id, Math.Matrix origin, Math.Matrix magnification, bool vidible)
        {
            this.Name = name;
            this.IsVisible = vidible;
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
        /// 平行移動.
        /// </summary>
        /// <param name="matrix">移動量</param>
        public void Translation(Math.Matrix matrix)
        {
            Math.Matrix temp = new (4);
            temp.Identity();

            temp[0, 3] = matrix[0, 0];
            temp[1, 3] = matrix[1, 0];
            temp[2, 3] = matrix[2, 0];

            this.Origin = temp * this.Origin;
        }

        /// <summary>
        /// 拡大・縮小
        /// </summary>
        /// <param name="matrix">拡大・縮小後のベクトル</param>
        public void Scale(double x, double y, double z)
        {
            this.Magnification = new (new double[,] { { x }, { y }, { z } });
        }

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
