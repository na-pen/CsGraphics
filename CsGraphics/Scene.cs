namespace CsGraphics
{
    using Microsoft.Maui.Graphics;
    using Microsoft.Maui.Graphics.Platform;
    using Color = Microsoft.Maui.Graphics.Color;
    using Point = Microsoft.Maui.Graphics.Point;
    using CsGraphics.Calc;
    using System.Numerics;

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
        /// Gets or sets a value indicating whether シーンが更新されたかどうか.
        /// </summary>
        public bool IsUpdated { get; set; } = true;

        double[] zDepths = Array.Empty<double>();

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            // 背景を白に設定
            canvas.FillColor = Colors.White;
            canvas.FillRectangle(dirtyRect);

            int canvasHeight = (int)dirtyRect.Height;
            int canvasWidth = (int)dirtyRect.Width;

            double[] viewPlanePointA = new double[3] { 0, 0, 0 }; // 描画面の左下の座標
            double[] viewPlanePointB = new double[3] { 0, canvasHeight, 0 }; // 描画面の左上の座標
            double[] viewPlanePointC = new double[3] { canvasWidth, 0, 0 }; // 描画面の右下の座標
            Math.Vector ab = new Math.Vector(viewPlanePointA, viewPlanePointB); // ABベクトル
            Math.Vector ac = new Math.Vector(viewPlanePointA, viewPlanePointC); // ACベクトル
            Math.Vector viewPlaneEquation = Calc.ZDepth.PlaneEquation(ab, ac); // 描画面の平面方程式

            Color[,] pixelColors = new Color[canvasWidth, canvasHeight];
            for (int x = 0; x < canvasWidth; x++)
            {
                for (int y = 0; y < canvasHeight; y++)
                {
                    pixelColors[x, y] = Colors.White;
                }
            }

            // Zバッファの初期化 (全て無限大で初期化)
            double[,] zBuffer = new double[canvasWidth, canvasHeight];
            for (int x = 0; x < canvasWidth; x++)
            {
                for (int y = 0; y < canvasHeight; y++)
                {
                    zBuffer[x, y] = 1;
                }
            }

            // 各点を指定された色で描画
            foreach (Object.Object @object in this.Objects)
            {
                if (@object.IsVisible == true)
                {
                    Point[] points = Array.Empty<Point>();
                    Color[] color = Array.Empty<Color>();
                    bool[] isVisiblePolygon = Array.Empty<bool>();
                    Object.Object obj;

                    if (@object.IsUpdated == true) // オブジェクトの情報に更新があれば再計算
                    {
                        (points, color, isVisiblePolygon, zDepths, obj) = Calculation.Calc((Object.Object)@object.Clone()); // 点や面の計算

                        @object.Points = points;
                        @object.PointsColor = color;
                        @object.IsVisiblePolygon = isVisiblePolygon;
                        @object.IsUpdated = false;
                    }
                    else
                    {
                        (points, color, isVisiblePolygon, obj) = (@object.Points, @object.PointsColor, @object.IsVisiblePolygon,@object);
                    }

                    if (@object.Polygon != null) // ポリゴンが存在する場合のみ描画
                    {
                        // 各ポリゴンをチェック
                        for (int i = 0; i < ((Object.Polygon)@object.Polygon).Length(); i++)
                        {
                            if (!isVisiblePolygon[i])
                            {
                                continue; // カメラに向いていないポリゴンは描画しない
                            }

                            // ポリゴンの頂点インデックスを取得
                            int[] polygon = ((Object.Polygon)@object.Polygon).VertexID[i];
                            Point p1 = points[polygon[0] - 1];
                            Point p2 = points[polygon[1] - 1];
                            Point p3 = points[polygon[2] - 1];



                            double[] polygonPointA = new double[3] { obj.Vertex.Coordinate[0,polygon[0] - 1], obj.Vertex.Coordinate[1, polygon[0] - 1], obj.Vertex.Coordinate[2, polygon[0] - 1] }; // ポリゴンの頂点
                            double[] polygonPointB = new double[3] { obj.Vertex.Coordinate[0, polygon[1] - 1], obj.Vertex.Coordinate[1, polygon[1] - 1], obj.Vertex.Coordinate[2, polygon[1] - 1] }; // 
                            double[] polygonPointC = new double[3] { obj.Vertex.Coordinate[0, polygon[2] - 1], obj.Vertex.Coordinate[1, polygon[2] - 1], obj.Vertex.Coordinate[2, polygon[2] - 1] }; // 
                            Math.Vector abP = new Math.Vector(polygonPointA, polygonPointB); // ABベクトル
                            Math.Vector acP = new Math.Vector(polygonPointA, polygonPointC); // ACベクトル
                            Math.Vector polygonEquation = Calc.ZDepth.PlaneEquation(abP, acP); // 描画面の平面方程式

                            // バウンディングボックスの範囲を取得
                            double xMin = ((Object.Polygon)@object.Polygon).Bounds[i, 0];
                            double xMax = ((Object.Polygon)@object.Polygon).Bounds[i, 1];
                            double yMin = ((Object.Polygon)@object.Polygon).Bounds[i, 2];
                            double yMax = ((Object.Polygon)@object.Polygon).Bounds[i, 3];

                            // バウンディングボックス内のピクセルを描画
                            for (int x = (int)System.Math.Ceiling(xMin); x <= xMax; x++)
                            {
                                for (int y = (int)System.Math.Ceiling(yMin); y <= yMax; y++)
                                {
                                    if (x < 0 || y < 0 || y > canvasHeight -1 || x > canvasWidth -1)
                                    {
                                        continue;
                                    }
                                    else
                                    {

                                        //pixelColors[x, y] = this.GetColorForPolygon(i);
                                        Math.Vector pixel = new(x, y, 0);
                                        // Z深度を計算
                                        double depth = Calc.ZDepth.ZDepsParallel(viewPlaneEquation, pixel, polygonEquation, polygonPointA, polygonPointB, polygonPointC);

                                        // Zバッファを更新 (近いものだけ描画)
                                        if (depth < zBuffer[x, y] && depth >= 0)
                                        {
                                            zBuffer[x, y] = depth;
                                            pixelColors[x, y] = this.GetColorForPolygon(i); // 色を設定
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
            }

            canvas.DrawImage(this.CreateImageFromColors(pixelColors, canvasWidth, canvasHeight), 0, 0, dirtyRect.Width, dirtyRect.Height);
            this.IsUpdated = false;
        }

        private IImage CreateImageFromColors(Color[,] colors, int width, int height)
        {
            // メモリストリームを使用して画像データを作成
            using (var stream = new MemoryStream())
            {
                Bitmap bitmap = new(width, height, colors);
                // ストリームに画像データを書き込む
                stream.Write(bitmap.FileHeader.Bytes);
                stream.Write(bitmap.InfoHeader.Bytes);
                stream.Write(bitmap.img);
                stream.Position = 0;
                IImage image = PlatformImage.FromStream(stream, ImageFormat.Bmp);
                // PlatformImage をストリームから読み込んで画像を作成

                /*
                using (FileStream fs = new FileStream("E:\\Projects\\CsGraphics\\Main\\test.bmp", FileMode.Create))
                {
                    fs.Write(bitmap.FileHeader.Bytes);
                    fs.Write(bitmap.InfoHeader.Bytes);
                    fs.Write(bitmap.img);
                }*/
                return image;
            }
        }

        // ポリゴンの色を取得 (色設定)
        private Color GetColorForPolygon(int polygonIndex)
        {
            // 例: ポリゴンごとの色を設定
            return Color.FromRgb(255, 0, 0); // 赤色
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
            Object.Object @object = new(name, vertexCoord, id, vertexColor, origin, visible, scale, polygon, null);
            this.Objects.Add(@object);

            this.IsUpdated = true;
            return id;
        }

        private int AddObject(string name, double[,] vertexCoord, Color[]? vertexColor = null, double[]? origin = null, bool visible = true, double[]? scale = null, int[][]? polygon = null, Math.Matrix[]? normal = null)
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
            (double[,] vertices, int[][] polygon, Math.Matrix[] normal) = Parser.ObjParseVertices(filePath);
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
