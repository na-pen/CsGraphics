namespace CsGraphics
{
    using System.Collections;
    using CsGraphics.Math;
    using CsGraphics.Object;
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

        public bool IsUpdated { get; set; } = true;

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
                    Point[] points = Array.Empty<Point>();
                    Color[] color = Array.Empty<Color>();
                    bool[] isVisiblePolygon = Array.Empty<bool>();

                    if (@object.IsUpdated == true)
                    {
                        (points, color, isVisiblePolygon) = Calculation.Calc((Object.Object)@object.Clone()); // 点や面の計算

                        @object.Points = points;
                        @object.PointsColor = color;
                        @object.IsVisiblePolygon = isVisiblePolygon;
                        @object.IsUpdated = false;
                    }
                    else
                    {
                        (points, color, isVisiblePolygon) = (@object.Points, @object.PointsColor, @object.IsVisiblePolygon);
                    }

                    // 点を描画
                    points.Zip(color, (point, c) =>
                        {
                            canvas.FillColor = c;  // 点の色を設定
                            canvas.FillCircle((float)point.X, (float)point.Y, 1);  // 点を描画

                            return 0; // 必要に応じて適切な値を返す
                        }).ToList();

                    // 面を描画
                    canvas.FillColor = Colors.LightBlue; // 塗りつぶしの色
                    canvas.StrokeColor = Colors.Blue; // 線の色
                    canvas.StrokeSize = 1;

                    if (@object.Polygon != null)
                    {
                        int j = 0;
                        foreach (int[] indices in (Polygon)@object.Polygon)
                        {
                            if (isVisiblePolygon[j]) // カメラに対して向いている面のみ描画
                            {
                                PathF path = new ();
                                path.MoveTo((float)points[indices[0] - 1].X, (float)points[indices[0] - 1].Y); // 初期点
                                for (int i = 1; i < indices.Length; i++)
                                {
                                    path.LineTo((float)points[indices[i] - 1].X, (float)points[indices[i] - 1].Y); // 次の頂点
                                }

                                path.Close();
                                canvas.FillPath(path);
                                canvas.DrawPath(path);
                            }

                            j++;
                        }
                    }
                }
            }

            // 元の状態に戻す
            canvas.RestoreState();
            this.IsUpdated = false;
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
        /// <param name="polygon">面を構成する点の情報.</param>
        /// <returns>id.</returns>
        public int AddObject(string name, double[,] vertexCoord, Color[]? vertexColor = null, double[]? origin = null, bool visible = true, double[]? scale = null, int[][]? polygon = null)
        {
            int id = this.Objects.Count;
            Object.Object @object = new (name, vertexCoord, id, vertexColor, origin, visible, scale, polygon, null);
            this.Objects.Add(@object);

            this.IsUpdated = true;
            return id;
        }

        private int AddObject(string name, double[,] vertexCoord, Color[]? vertexColor = null, double[]? origin = null, bool visible = true, double[]? scale = null, int[][]? polygon = null, Matrix[]? normal = null)
        {
            int id = this.Objects.Count;
            Object.Object @object = new(name, vertexCoord, id, vertexColor, origin, visible, scale, polygon, normal);
            this.Objects.Add(@object);

            this.IsUpdated = true;
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
            (double[,] vertices, int[][] polygon, Matrix[] normal) = Parser.ObjParseVertices(filePath);
            int id = this.AddObject(name, vertices, polygon: polygon, normal: normal);

            this.IsUpdated = true;
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
            this.IsUpdated = true;
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
            this.IsUpdated = true;
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
            this.IsUpdated = true;
        }
    }
}
