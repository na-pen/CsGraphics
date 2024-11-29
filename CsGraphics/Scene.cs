﻿namespace CsGraphics
{
    using System.Collections;
    using Microsoft.Maui.Graphics;

    /// <summary>
    /// シーン.
    /// </summary>
    public class Scene : IDrawable
    {
        /// <summary>
        /// 重力加速度.
        /// </summary>
        public const double Gravity = 9.80665;

        /// <summary>
        /// シーンに含まれるオブジェクト.
        /// </summary>
        internal List<Object.Object> Objects;

        /// <summary>
        /// Initializes a new instance of the <see cref="Scene"/> class.
        /// </summary>
        /// <param name="frameRate">フレームレート.</param>
        public Scene(int frameRate)
        {
            this.FrameRate = frameRate;
            this.Objects = new List<Object.Object>(); // 初期化
        }

        /// <summary>
        /// Gets 画面の更新頻度.
        /// </summary>
        public int FrameRate { get; }

        /// <summary>
        /// オブジェクトを画面に描画.
        /// </summary>
        /// <param name="canvas">キャンバス.</param>
        /// <param name="dirtyRect">dirtyRect.</param>
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            // 背景を白に設定
            canvas.FillColor = Colors.White;
            canvas.FillRectangle(dirtyRect);

            // 座標軸の補正
            canvas.SaveState(); // 現在の状態を保存
            canvas.Translate(0, dirtyRect.Height); // Y軸を下に移動
            canvas.Scale(1, -1);

            // 各点を指定された色で描画
            foreach (Object.Object @object in this.Objects)
            {
                if (@object.IsVisible == true)
                {
                    (Point[] points, Color[] color) = CalcScreenCoord.Calc((Object.Object)@object.Clone());
                    points.Zip(color, (point, c) =>
                        {
                            canvas.FillColor = c;  // 点の色を設定
                            canvas.FillCircle((float)point.X, (float)point.Y, 5);  // 点を描画
                            return 0; // 必要に応じて適切な値を返す
                        }).ToList();
                }
            }

            // 元の状態に戻す
            canvas.RestoreState();
        }

        /// <summary>
        /// シーンにオブジェクトを追加.
        /// </summary>
        /// <param name="name">オブジェクト名.</param>
        /// <param name="vertexCoord">頂点座標.</param>
        /// <param name="vertexColor">頂点カラー.</param>
        /// <param name="origin">オブジェクトの原点.</param>
        /// <param name="visible">オブジェクトの可視状態.</param>
        /// <param name="scale">拡大率.</param>
        /// <returns>id.</returns>
        public int AddObject(string name, double[,] vertexCoord, Color[]? vertexColor = null, double[]? origin = null, bool visible = true, double[]? scale = null)
        {
            int id = this.Objects.Count;
            Object.Object @object = new (name, vertexCoord, id, vertexColor, origin, visible, scale);
            this.Objects.Add(@object);

            return id;
        }

        /// <summary>
        /// .objから3Dモデルをシーンに追加.
        /// </summary>
        /// <param name="name">オブジェクト名.</param>
        /// <param name="filePath">.objのパス.</param>
        /// <returns>ID.</returns>
        public int AddObjectFromObj(string name, string filePath)
        {
            double[,] vertices = Parser.ObjParseVertices(filePath);
            int id = this.AddObject(name, vertices);

            return id;
        }

        /// <summary>
        /// オブジェクトの情報を取得する.
        /// </summary>
        /// <param name="id">オブジェクトID.</param>
        /// <param name="level">表示レベル.</param>
        /// <returns>オブジェクト情報.</returns>
        /// <exception cref="ArgumentOutOfRangeException">存在しないオブジェクトを参照することはできません.</exception>
        public string GetObjectInfo(int id, int level = 0)
        {
            if (id >= this.Objects.Count)
            {
                throw new ArgumentOutOfRangeException($"Object ID {id} does not exist in this scene.");
            }

            Object.Object @object = this.Objects[id];

            string result =
                "ObjectID : " + id + "\n" +
                "Name : " + @object.Name + "\n" +
                "Origin : " + @object.Origin.ToString().Replace("\n", ",") + "\n" +
                "Angle : " + @object.Angle.ToString().Replace("\n", ",") + "\n" +
                "Scale : " + @object.Magnification.ToString().Replace("\n", ",") + "\n" +
                "Visible : " + @object.IsVisible.ToString() + "\n" +
                "Num of Vertex : " + @object.Vertex.GetLength(1);

            return result;
        }

        /// <summary>
        /// オブジェクトを移動する.
        /// </summary>
        /// <param name="id">オブジェクトID.</param>
        /// <param name="x">x軸移動量.</param>
        /// <param name="y">y軸移動量.</param>
        /// <param name="z">z軸移動量.</param>
        public void TranslationObject(int id, double x, double y, double z)
        {
            this.Objects[id].SetTranslation(x, y, z);
        }

        /// <summary>
        /// オブジェクトの拡大.
        /// </summary>
        /// <param name="id">オブジェクトID.</param>
        /// <param name="x">x軸移動量.</param>
        /// <param name="y">y軸移動量.</param>
        /// <param name="z">z軸移動量.</param>
        public void ScaleObject(int id, double x, double y, double z)
        {
            this.Objects[id].SetScale(x, y, z);
        }

        /// <summary>
        /// オブジェクトの回転.
        /// </summary>
        /// <param name="id">オブジェクトID.</param>
        /// <param name="x">x軸移動量.</param>
        /// <param name="y">y軸移動量.</param>
        /// <param name="z">z軸移動量.</param>
        public void RotationObject(int id, double x, double y, double z)
        {
            this.Objects[id].SetRotation(x, y, z);
        }
    }
}
