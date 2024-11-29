﻿namespace CsGraphics.Object
{
    using CsGraphics.Math;
    using Microsoft.Maui.Primitives;

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
        internal Object(string name, double[,] vertexCoord, int id = -1, Color[]? vertexColor = null, double[]? origin = null, bool visible = true, double[]? scale = null, int[][]? polygon = null, Matrix[] normal = null)
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
                this.Magnification = new double[] { 1, 1, 1 };
            }
            else
            {
                this.Magnification = scale;
            }

            this.Vertex = new (id, vertexCoord, vertexColor);

            if (polygon != null)
            {
                this.Polygon = new Polygon(this.ID, polygon, normal);
            }
        }

        private Object(string name, Vertex vertex, int id, Math.Matrix origin, double[] magnification, bool visible, double[] angle, Polygon? polygon)
        {
            this.Name = name;
            this.IsVisible = visible;
            this.Origin = origin;
            this.ID = id;
            this.Vertex = vertex;
            this.Magnification = magnification;
            this.Angle = angle;
            this.Polygon = polygon;
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
        /// Gets or sets 多角形面の情報.
        /// </summary>
        internal Polygon? Polygon { get; set; } = null;

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets オブジェクトが更新されたかどうか.
        /// </summary>
        internal bool IsUpdated { get; set; } = true;

        //------------------------------------------------------ ここから 計算済み情報の保持 ------------------------------------------------------/

        /// <summary>
        /// Gets or sets 計算後の画面上の描画座標を保持.
        /// </summary>
        internal Point[] Points { get; set; } = Array.Empty<Point>();

        /// <summary>
        /// Gets or sets 計算後の画面上の頂点色を保持.
        /// </summary>
        internal Color[] PointsColor { get; set; } = Array.Empty<Color>();

        /// <summary>
        /// Gets or sets 計算後の画面上の面の表示状態の保持.
        /// </summary>
        internal bool[] IsVisiblePolygon { get; set; } = Array.Empty<bool>();

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
                (double[])this.Magnification.Clone(),
                this.IsVisible,
                this.Angle,
                this.Polygon)
            {
            };
        }

        /// <summary>
        /// 平行移動(オブジェクトの原点を移動).
        /// </summary>
        /// <param name="x">x軸の移動量.</param>
        /// <param name="y">y軸の移動量.</param>
        /// <param name="z">z軸の移動量.</param>
        internal void SetTranslation(double x, double y, double z)
        {
            this.IsUpdated = true;

            Math.Matrix temp = new (3, 1);

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
            this.IsUpdated = true;
            this.Magnification = new double[] { this.Magnification[0] * x, this.Magnification[1] * y, this.Magnification[2] * z };
        }

        /// <summary>
        /// 回転(オブジェクトの原点基準).
        /// </summary>
        /// <param name="x">x軸の回転角度.</param>
        /// <param name="y">y軸の回転角度.</param>
        /// <param name="z">z軸の回転角度.</param>
        internal void SetRotation(double x, double y, double z)
        {
            this.IsUpdated = true;
            this.Angle = new double[] { this.Angle[0] + x, this.Angle[1] + y, this.Angle[2] + z };
        }
    }
}
