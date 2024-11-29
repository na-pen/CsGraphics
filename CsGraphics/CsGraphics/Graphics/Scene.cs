namespace CsGraphics
{
    using System.Collections;
    using Microsoft.Maui.Graphics;

    public class Scene : IDrawable
    {
        /// <summary>
        /// 画面の更新頻度.
        /// </summary>
        public const int FrameRate = 10;

        /// <summary>
        /// 重力加速度.
        /// </summary>
        public const double Gravity = 9.80665;

        /// <summary>
        /// シーンに含まれるオブジェクト.
        /// </summary>
        public List<Object.Object> Objects;

        public Scene()
        {
            this.Objects = new List<Object.Object>(); // 初期化
        }

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
                    (Point[] points, Color[] color) = CalcScreenCoord.Calc(@object);
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
        /// <param name="_object">オブジェクト.</param>
        public void AddObject(Object.Object _object)
        {
            _object.ID = this.Objects.Count;
            this.Objects.Add(_object);
        }

        /// <summary>
        /// .objから3Dモデルをシーンに追加.
        /// </summary>
        /// <param name="name">オブジェクト名.</param>
        /// <param name="filePath">.objのパス.</param>
        public void AddObjectFromObj(string name, string filePath)
        {
            double[,] vertices = Parser.ObjParseVertices(filePath);
            this.AddObject(new Object.Object(name, vertices));
        }
    }
}
