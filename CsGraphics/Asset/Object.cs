namespace CsGraphics.Asset
{
    /// <summary>
    /// オブジェクトの情報の保持や管理を行う.
    /// </summary>
    internal class Object
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Object"/> class.
        /// </summary>
        /// <param name="id">オブジェクトID.</param>
        /// <param name="name">オブジェクト名.</param>
        /// <param name="origin">オブジェクトの原点.</param>
        /// <param name="visible">オブジェクトの表示状態.</param>
        /// <param name="scale">オブジェクトの拡大倍率.</param>
        internal Object(string name, int id = -1, bool visible = true, float[]? origin = null, float[]? scale = null)
        {
            this.ID = id;
            this.Name = name;
            this.IsVisible = visible;

            if (origin == null)
            {
                this.Origin = new (new float[,] { { 0 }, { 0 }, { 0 } });
            }
            else
            {
                this.Origin = new (origin);
            }

            if (scale == null)
            {
                this.Scale = new float[] { 20, 20, 20 };
            }
            else
            {
                this.Scale = scale;
            }
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
        internal float[] Scale { get; set; }

        /// <summary>
        /// Gets or sets オブジェクトの傾き.
        /// </summary>
        internal float[] Angle { get; set; } = { 0, 3.14f, 0 };

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets オブジェクトが更新されたかどうか.
        /// </summary>
        internal bool IsUpdated { get; set; } = true;

        /// <summary>
        /// 平行移動(オブジェクトの原点を移動).
        /// </summary>
        /// <param name="x">x軸の移動量.</param>
        /// <param name="y">y軸の移動量.</param>
        /// <param name="z">z軸の移動量.</param>
        internal void SetTranslation(float x, float y, float z)
        {
            this.IsUpdated = true;

            Math.Matrix temp = new(3, 1);

            temp[0, 0] = x;
            temp[1, 0] = y;
            temp[2, 0] = z;

            Origin += temp;
        }

        /// <summary>
        /// 拡大・縮小(オブジェクトの原点基準).
        /// </summary>
        /// <param name="x">x軸の拡大率.</param>
        /// <param name="y">y軸の拡大率.</param>
        /// <param name="z">z軸の拡大率.</param>
        internal void SetScale(float x, float y, float z)
        {
            this.IsUpdated = true;
            this.Scale = new float[] { this.Scale[0] * x, this.Scale[1] * y, this.Scale[2] * z };
        }

        /// <summary>
        /// 回転(オブジェクトの原点基準).
        /// </summary>
        /// <param name="x">x軸の回転角度.</param>
        /// <param name="y">y軸の回転角度.</param>
        /// <param name="z">z軸の回転角度.</param>
        internal void SetRotation(float x, float y, float z)
        {
            this.IsUpdated = true;
            this.Angle = new float[] { this.Angle[0] + x, this.Angle[1] + y, this.Angle[2] + z };
        }
    }
}
