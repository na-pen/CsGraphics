﻿using CsGraphics.Object.Asset.Image;

namespace CsGraphics.Object.Asset.Model
{
    /// <summary>
    /// オブジェクトの情報の保持や管理を行う.
    /// </summary>
    internal class Model : Object // : ICloneable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Model"/> class.
        /// </summary>
        /// <param name="id">オブジェクトID.</param>
        /// <param name="name">オブジェクト名.</param>
        /// <param name="vertexCoord">頂点座標.</param>
        /// <param name="origin">オブジェクトの原点.</param>
        /// <param name="visible">オブジェクトの表示状態.</param>
        /// <param name="scale">オブジェクトの拡大倍率.</param>
        internal Model(string name, float[,] vertexCoord, Dictionary<string, (string, byte, ushort, Color, Color, Color)>? polygonColor = null, float[]? origin = null, bool visible = true, float[]? scale = null, Dictionary<string, int[][]>? polygon = null, Math.Matrix normal = null, Dictionary<string, int[][]>? mtlV = null, Dictionary<string, int[][]>? normalId = null, float[] vt = null, float[] vn = null)
            : base(name, visible: visible, origin: origin, scale: scale)
        {
            Vertex = new(this.ID, vertexCoord, vt, vn);

            if (polygon != null && polygonColor != null)
            {
                Polygon = new Polygon(ID, polygon, normal, mtlV, normalId);
            }
            if(polygonColor!= null)
            {
                Material = new(polygonColor);
            }
        }

        /*
        private Object3D(string name, Vertex vertex, int id, Math.Matrix origin, float[] magnification, bool visible, float[] angle, Polygon? polygon, Dictionary<string, (int, int, byte[])>? texture)
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
        */

        /// <summary>
        /// Gets or sets 頂点情報.
        /// </summary>
        internal Vertex Vertex { get; set; }

        /// <summary>
        /// Gets or sets 多角形面の情報.
        /// </summary>
        internal Polygon? Polygon { get; set; } = null;

        internal Material? Material { get; set; } = null;

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
        /*
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
        }*/

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
