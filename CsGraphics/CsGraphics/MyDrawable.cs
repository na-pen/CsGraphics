
namespace CsGraphics
{
    using Microsoft.Maui.Graphics;
    using System;
    using System.Collections.Generic;

    public class MyDrawable : IDrawable
    {
        private List<Vertex> _pointsWithColor;  // 点と色を保持するリスト

        public MyDrawable()
        {
            _pointsWithColor = new List<Vertex>(); // 初期化
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            // 背景を白に設定
            canvas.FillColor = Colors.White;
            canvas.FillRectangle(dirtyRect);

            // 各点を指定された色で描画
            foreach (var pointColor in _pointsWithColor)
            {
                canvas.FillColor = pointColor.color;  // 点の色を設定
                canvas.FillCircle(pointColor.vertex.X, pointColor.vertex.Y, 1);  // 半径5の円として点を描画
            }
        }

        // 外部から点と色を追加するメソッド
        public void AddPoints(IEnumerable<Vertex> pointsWithColor)
        {
            _pointsWithColor.AddRange(pointsWithColor);
        }

        public void AddPoints(Vector vector,Color color)
        {
            List<Vertex> vertices = new List<Vertex>();

            switch (vector.dimension)
            {
                case 2:
                    vertices = Vector2PointF(vector, color); break;

                case 3:
                    vertices = Vector2PointF(ThreeD2TwoD(vector),color); break;

                default:
                    break;
            }

            this.AddPoints(vertices);
        }

        private List<Vertex> Vector2PointF(Vector vector, Color color)
        {
            var vertices = Enumerable.Range(0, vector.data.Columns) // n の列数分繰り返す
            .Select(i =>
            {
                // 各列から a, b の値を取り出して PointF を作成
                double a = vector.data[0, i];
                double b = vector.data[1, i];

                // Vertex インスタンスを作成
                return new Vertex(new PointF((float)a, (float)b), color);
            })
            .ToList();  // リストに変換

            return vertices;
        }

        private Vector ThreeD2TwoD(Vector vector)
        {
            return vector;
        }
    }
}
