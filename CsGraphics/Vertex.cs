using Microsoft.Maui.Graphics;

namespace CsGraphics { 
    public class Vertex
    {
        public PointF vertex { get; set; }  // 描画する点
        public Color color { get; set; }   // 点の色

        public Vertex(PointF point, Color color)
        {
            this.vertex = point;
            this.color = color;
        }
}
}