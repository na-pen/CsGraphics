using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsGraphics
{
    public class files
    {
        private static double[,] ObjParseVertices(string filePath)
        {
            var vertices = new List<double[]>(); // 動的リストで頂点情報を一時的に格納

            // ファイルを1行ずつ読み取る
            foreach (var line in File.ReadLines(filePath))
            {
                if (line.StartsWith("v ")) // 頂点情報の場合
                {
                    // "v x y z" を解析
                    var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 4) // 頂点座標が存在する場合
                    {
                        double x = double.Parse(parts[1]);
                        double y = double.Parse(parts[2]);
                        double z = double.Parse(parts[3]);
                        vertices.Add(new[] { x, y, z });
                    }
                }
            }

            // List<double[]> を double[,] に変換
            int vertexCount = vertices.Count;
            double[,] vertexArray = new double[3, vertexCount];

            for (int i = 0; i < vertexCount; i++)
            {
                vertexArray[0, i] = vertices[i][0];
                vertexArray[1, i] = vertices[i][1];
                vertexArray[2, i] = vertices[i][2];
            }

            return vertexArray;
        }

        public static Math.Vector VerticesFromObj(string filePath)
        {
            double[,] vertices = ObjParseVertices(filePath);
            int vertexCount = vertices.GetLength(1);
            Math.Vector vector = new(3, vertexCount);

            vector.Initialize(vertices);

            return vector;
        }
    }
}
