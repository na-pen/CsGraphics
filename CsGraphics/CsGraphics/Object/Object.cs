using Microsoft.Maui.Primitives;

namespace CsGraphics.Object
{
    /// <summary>
    /// オブジェクトの情報の保持や管理を行う.
    /// </summary>
    public class Object
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
        public Object(string name, double[,] vertexCoord, int id = -1, Color[]? vertexColor = null, double[]? origin = null, bool vidible = true)
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

            this.Origin.Resize(4, value: new double[] { 1D });
            this.Vertex = new (id, vertexCoord, vertexColor);
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
        /// Gets or sets 頂点情報.
        /// </summary>
        public Vertex Vertex { get; set; }

        /// <summary>
        /// 平行移動
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
    }
}
