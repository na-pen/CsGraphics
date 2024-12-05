namespace CsGraphics
{
    using Microsoft.Maui.Graphics;
    using Microsoft.Maui.Graphics.Platform;
    using Color = Microsoft.Maui.Graphics.Color;
    using Point = Microsoft.Maui.Graphics.Point;
    using CsGraphics.Calc;
    using System.Numerics;
    using System.Collections.Generic;

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
                    double[] pT = Array.Empty<double>();

                    //if (@object.IsUpdated == true) // オブジェクトの情報に更新があれば再計算
                    if (true)
                    {
                        (points, color, isVisiblePolygon, _, obj) = Calculation.Calc((Object.Object)@object.Clone()); // 点や面の計算

                        @object.Points = points;
                        @object.PointsColor = color;
                        @object.IsVisiblePolygon = isVisiblePolygon;
                        @object.IsUpdated = false;
                    }
                    else
                    {
                        (points, color, isVisiblePolygon, obj) = (@object.Points, @object.PointsColor, @object.IsVisiblePolygon, @object);
                    }

                    if (@object.Polygon != null) // ポリゴンが存在する場合のみ描画
                    {
                        // polygonのグループごとに処理
                        foreach (var kvp in ((Object.Polygon)@object.Polygon).VertexID)
                        {
                            string key = kvp.Key;
                            int[][] array = kvp.Value;

                            int[][] array2 = ((Object.Polygon)@object.Polygon).MtlVertexID[key];

                            // 各ポリゴンをチェック
                            for (int i = 0; i < array.GetLength(0); i++)
                            {
                                /*
                                if (!isVisiblePolygon[i])
                                {
                                    continue; // カメラに向いていないポリゴンは描画しない
                                }
                                */

                                // ポリゴンの頂点を取得
                                int[] polygon = array[i];
                                Point[] vertex = polygon.Select(p => points[p - 1]).ToArray();

                                // テクスチャ頂点番号の取得

                                int[] vTId = array2[i];
                                double[][] vt = null;
                                if (@object.Vertex.Vt != null)
                                {
                                    vt = vTId.Select(p => @object.Vertex.Vt[p - 1]).ToArray();
                                }

                                // 面を描く
                                Point[] pixels = RasterizeTriangle(vertex); // 描画するPixelの一覧
                                double[] polygonPointA = new double[3] { obj.Vertex.Coordinate[0, polygon[0] - 1], obj.Vertex.Coordinate[1, polygon[0] - 1], obj.Vertex.Coordinate[2, polygon[0] - 1] }; // ポリゴンの頂点
                                double[] polygonPointB = new double[3] { obj.Vertex.Coordinate[0, polygon[1] - 1], obj.Vertex.Coordinate[1, polygon[1] - 1], obj.Vertex.Coordinate[2, polygon[1] - 1] }; // 
                                double[] polygonPointC = new double[3] { obj.Vertex.Coordinate[0, polygon[2] - 1], obj.Vertex.Coordinate[1, polygon[2] - 1], obj.Vertex.Coordinate[2, polygon[2] - 1] }; // 
                                Math.Vector abP = new Math.Vector(polygonPointA, polygonPointB); // ABベクトル
                                Math.Vector acP = new Math.Vector(polygonPointA, polygonPointC); // ACベクトル
                                Math.Vector polygonEquation = Calc.ZDepth.PlaneEquation(abP, acP);

                                foreach (Point p in pixels)
                                {
                                    if ((int)p.X < 0 || (int)p.Y < 0 || (int)p.Y > canvasHeight - 1 || (int)p.X > canvasWidth - 1)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        Color? pixelcolor = null;
                                        double[] pixel = new double[2] { (int)p.X, (int)p.Y };

                                        // Z深度を計算
                                        (double depth, double a, double b, double c) = Calc.ZDepth.ZDepsParallel(pixel, polygonPointA, polygonPointB, polygonPointC, 500, -500);

                                        // テクスチャ中の座標を計算
                                        if (vt != null)
                                        {
                                            double texVx = (a * vt[0][0]) + (b * vt[1][0]) + (c * vt[2][0]);
                                            double texVy = (a * vt[0][1]) + (b * vt[1][1]) + (c * vt[2][1]);
                                            if (@object.Texture != null && @object.Texture.ContainsKey(key))
                                            {
                                                (Color cl, _) = ((Object.Polygon)@object.Polygon).Colors[key];
                                                int x = (int)((texVx % 1) * @object.Texture[key].GetLength(0)) - 1;
                                                int y = ((int)((texVy % 1) * @object.Texture[key].GetLength(1))) - 1;
                                                pixelcolor = @object.Texture[key][x, y].MultiplyAlpha(cl.Alpha);
                                                int m = 0;
                                            }
                                        }
                                        // Zバッファを更新 (近いものだけ描画)
                                        if (depth < zBuffer[(int)p.X, (int)p.Y] && depth >= 0)
                                        {
                                            zBuffer[(int)p.X, (int)p.Y] = depth;

                                            if (pixelcolor == null)
                                            {
                                                (Color cl, _) = ((Object.Polygon)@object.Polygon).Colors[key];
                                                pixelColors[(int)p.X, (int)p.Y] = cl; // 色を設定
                                            }
                                            else if (pixelcolor.Alpha != 1)
                                            {
                                                pixelColors[(int)p.X, (int)p.Y] = BlendColors(pixelcolor, pixelColors[(int)p.X, (int)p.Y]);

                                            }
                                            else
                                            {
                                                pixelColors[(int)p.X, (int)p.Y] = pixelcolor;

                                            }
                                        }
                                    }
                                }

                                /*
                                // 頂点を描く
                                foreach (Point v in vertex)
                                {
                                    double[] pixel = new double[2] { (int)v.X, (int)v.Y };
                                    if ((int)v.X < 0 || (int)v.Y < 0 || (int)v.Y > canvasHeight - 1 || (int)v.X > canvasWidth)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        // Z深度を計算
                                        (double depth, double a, double b, double c) = Calc.ZDepth.ZDepsParallel(pixel, polygonPointA, polygonPointB, polygonPointC, 500, -500);
                                        if (depth <= zBuffer[(int)v.X, (int)v.Y] && depth >= 0)
                                        {
                                            zBuffer[(int)v.X, (int)v.Y] = depth;
                                            pixelColors[(int)v.X, (int)v.Y] = this.GetColorForPolygon(0, 0, 0); // 色を設定
                                        }
                                    }
                                }
                                */
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

        private static Point[] RasterizeTriangle(Point[] pt)
        {
            // 境界ボックスを計算
            double xmin = System.Math.Min(pt[0].X, System.Math.Min(pt[1].X, pt[2].X));
            double xmax = System.Math.Max(pt[0].X, System.Math.Max(pt[1].X, pt[2].X));
            double ymin = System.Math.Min(pt[0].Y, System.Math.Min(pt[1].Y, pt[2].Y));
            double ymax = System.Math.Max(pt[0].Y, System.Math.Max(pt[1].Y, pt[2].Y));

            List<Point> pixels = new();

            // 境界ボックス内のピクセルを調べる
            for (int x = (int)System.Math.Floor(xmin); x <= System.Math.Ceiling(xmax); x++)
            {
                for (int y = (int)System.Math.Floor(ymin); y <= System.Math.Ceiling(ymax); y++)
                {
                    if (IsPointInTriangle(x, y, pt))
                    {
                        pixels.Add(new Point(x, y));
                    }
                }
            }

            return pixels.ToArray();
        }

        private static bool IsPointInTriangle(int px, int py, Point[] pt)
        {
            double ax = pt[0].X;
            double ay = pt[0].Y;
            double bx = pt[1].X;
            double by = pt[1].Y;
            double cx = pt[2].X;
            double cy = pt[2].Y;

            // バリューコーディネート法による内外判定
            double denominator = (double)(((bx - ax) * (cy - ay)) - ((cx - ax) * (by - ay)));
            double lambda1 = (((bx - px) * (cy - py)) - ((cx - px) * (by - py))) / denominator;
            double lambda2 = (((cx - px) * (ay - py)) - ((ax - px) * (cy - py))) / denominator;
            double lambda3 = 1.0 - lambda1 - lambda2;

            return lambda1 >= 0 && lambda2 >= 0 && lambda3 >= 0;
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
        private static Color BlendColors(Color foreground, Color background)
        {
            // 前景色のRGBA成分
            float rF = (float)foreground.Red;
            float gF = (float)foreground.Green;
            float bF = (float)foreground.Blue;
            float aF = (float)foreground.Alpha;

            // 背景色のRGBA成分
            float rB = (float)background.Red;
            float gB = (float)background.Green;
            float bB = (float)background.Blue;
            float aB = (float)background.Alpha;

            // アルファ値の合成
            float aResult = aF + aB * (1 - aF);

            // 各成分のブレンド計算
            float rResult = (aF * rF + (1 - aF) * aB * rB) / aResult;
            float gResult = (aF * gF + (1 - aF) * aB * gB) / aResult;
            float bResult = (aF * bF + (1 - aF) * aB * bB) / aResult;

            // 合成結果の色を返す
            return new Color(rResult, gResult, bResult, aResult);
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
        public int AddObject(string name, double[,] vertexCoord, Dictionary<string, (Color, string)>? polygonColor = null, Color[]? vertexColor = null, double[]? origin = null, bool visible = true, double[]? scale = null, Dictionary<string, int[][]>? polygon = null)
        {
            int id = this.Objects.Count;
            Object.Object @object = new(name, vertexCoord, id, polygonColor, vertexColor, origin, visible, scale, polygon);
            this.Objects.Add(@object);

            this.IsUpdated = true;
            return id;
        }

        private int AddObject(string name, double[,] vertexCoord, Dictionary<string, (Color, string)>? polygonColor = null, Color[]? vertexColor = null, double[]? origin = null, bool visible = true, double[]? scale = null, Dictionary<string, int[][]>? polygon = null, Math.Matrix[]? normal = null, Dictionary<string, int[][]>? mtlV = null, double[][] vt = null)
        {
            int id = this.Objects.Count;
            Object.Object @object = new(name, vertexCoord, id, polygonColor, vertexColor, origin, visible, scale, polygon, normal, mtlV, vt);
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
        public int AddObjectFromObj(string name, string filePath, string texturePath = null)
        {
            (double[,] vertices, Dictionary<string, int[][]> polygon, Math.Matrix[] normal, Dictionary<string, (Color, string)>? polygonColor, Dictionary<string, int[][]> mtlV, double[][] vt) = Parser.ObjParseVerticesV2(filePath);
            int id = this.AddObject(name, vertices, polygon: polygon, normal: normal, polygonColor: polygonColor, mtlV: mtlV, vt: vt);
            foreach (var kvp in polygonColor)
            {
                string key = kvp.Key;
                (Color c, string s) = kvp.Value;
                this.Objects[id].AddTexture(key, s);
            }

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
