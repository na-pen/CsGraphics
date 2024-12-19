using CsGraphics.Asset.Image;

namespace CsGraphics.Asset
{
    /// <summary>
    /// オブジェクトの情報の保持や管理を行う.
    /// </summary>
    internal class Object3D : ICloneable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="object"/> class.
        /// </summary>
        /// <param name="id">オブジェクトID.</param>
        /// <param name="name">オブジェクト名.</param>
        /// <param name="vertexCoord">頂点座標.</param>
        /// <param name="vertexColor">頂点色.</param>
        /// <param name="origin">オブジェクトの原点.</param>
        /// <param name="visible">オブジェクトの表示状態.</param>
        /// <param name="scale">オブジェクトの拡大倍率.</param>
        internal Object3D(string name, float[,] vertexCoord, int id = -1, Dictionary<string, (Color, string)>? polygonColor = null, float[]? origin = null, bool visible = true, float[]? scale = null, Dictionary<string, int[][]>? polygon = null, Math.Matrix[] normal = null, Dictionary<string, int[][]>? mtlV = null, float[][] vt = null)
        {
            ID = id;
            Name = name;
            IsVisible = visible;

            if (origin == null)
            {
                Origin = new(new float[,] { { 0 }, { 0 }, { 0 } });
            }
            else
            {
                Origin = new(origin);
            }

            if (scale == null)
            {
                Magnification = new float[] { 20, 20, 20 };
            }
            else
            {
                Magnification = scale;
            }

            Vertex = new(id, vertexCoord, vt);

            if (polygon != null && polygonColor != null)
            {
                Polygon = new Object3d.Polygon(ID, polygon, normal, polygonColor, mtlV);
            }
        }

        private Object3D(string name, Object3d.Vertex vertex, int id, Math.Matrix origin, float[] magnification, bool visible, float[] angle, Object3d.Polygon? polygon, Dictionary<string, (int, int, byte[])>? texture)
        {
            Name = name;
            IsVisible = visible;
            Origin = origin;
            ID = id;
            Vertex = vertex;
            Magnification = magnification;
            Angle = angle;
            Polygon = polygon;
            Texture = texture;
        }

        internal Object3D(Object3D obj)
        {
            Name = obj.Name;
            IsVisible = obj.IsVisible;
            Origin = obj.Origin;
            ID = obj.ID;
            Vertex = obj.Vertex;
            Magnification = obj.Magnification;
            Angle = obj.Angle;
            Polygon = obj.Polygon;
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
        internal float[] Magnification { get; set; }

        /// <summary>
        /// Gets or sets 頂点情報.
        /// </summary>
        internal Object3d.Vertex Vertex { get; set; }

        /// <summary>
        /// Gets or sets オブジェクトの傾き.
        /// </summary>
        internal float[] Angle { get; set; } = { 0, 3.14f, 0 };

        /// <summary>
        /// Gets or sets 多角形面の情報.
        /// </summary>
        internal Object3d.Polygon? Polygon { get; set; } = null;

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets オブジェクトが更新されたかどうか.
        /// </summary>
        internal bool IsUpdated { get; set; } = true;

        internal Dictionary<string, (int, int, byte[])>? Texture { get; set; } = null;

        //------------------------------------------------------ ここから 計算済み情報の保持 ------------------------------------------------------/

        /// <summary>
        /// Gets or sets 計算後の画面上の描画座標を保持.
        /// </summary>
        internal Point[] Points { get; set; } = Array.Empty<Point>();

        /// <summary>
        /// 自身をシャドーコピーする.
        /// </summary>
        /// <returns>Clone.</returns>
        public object Clone()
        {
            return new Object3D(
                Name,
                (Object3d.Vertex)Vertex.Clone(),
                ID,
                (Math.Matrix)Origin.Clone(),
                (float[])Magnification.Clone(),
                IsVisible,
                Angle,
                Polygon,
                Texture)
            {
            };
        }

        /// <summary>
        /// 平行移動(オブジェクトの原点を移動).
        /// </summary>
        /// <param name="x">x軸の移動量.</param>
        /// <param name="y">y軸の移動量.</param>
        /// <param name="z">z軸の移動量.</param>
        internal void SetTranslation(float x, float y, float z)
        {
            IsUpdated = true;

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
            IsUpdated = true;
            Magnification = new float[] { Magnification[0] * x, Magnification[1] * y, Magnification[2] * z };
        }

        /// <summary>
        /// 回転(オブジェクトの原点基準).
        /// </summary>
        /// <param name="x">x軸の回転角度.</param>
        /// <param name="y">y軸の回転角度.</param>
        /// <param name="z">z軸の回転角度.</param>
        internal void SetRotation(float x, float y, float z)
        {
            IsUpdated = true;
            Angle = new float[] { Angle[0] + x, Angle[1] + y, Angle[2] + z };
        }

        internal void AddTexture(string matName, string path)
        {
            if (Texture == null)
            {
                Texture = new Dictionary<string, (int, int, byte[])>();
            }
            IsUpdated = true;
            if (path != string.Empty && !Texture.ContainsKey(path))
            {
                switch (Path.GetExtension(path))
                {
                    case ".bmp":

                        Texture.Add(path, Bitmap.LoadFromFile(path));
                        break;

                    case ".png":

                        Texture.Add(path, Png.LoadFromFile(path));
                        break;
                }
            }
        }

        internal int GetTextureLength(int dimension)
        {

            int result = 0;
            foreach (var kvp in Texture)
            {
                (int, int, byte[]) array = kvp.Value;
                result += array.Item3.GetLength(dimension);
            }
            return result;
        }
    }
}
