namespace CsGraphics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// 外部ファイルの読み込み.
    /// </summary>
    internal static class Parser
    {
        /// <summary>
        /// .objを読み込む.
        /// </summary>
        /// <param name="filePath">ファイルパス.</param>
        /// <returns>オブジェクトの頂点座標.</returns>
        internal static (double[,],int[][]) ObjParseVertices(string filePath)
        {
            var vertices = new List<double[]>(); // 動的リストで頂点情報を一時的に格納
            List<List<int>> polygon = new List<List<int>>(); // 動的リストで面を構成する頂点のIDを一時的に格納

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
                else if (line.StartsWith("f ")) // 面情報のとき
                {
                    List<int> p = new List<int>();
                    var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 4) // 面の情報が存在する場合
                    {
                        foreach (var part in parts.Skip(1))
                        {
                            string[] _part = part.Split("/");
                            p.Add(int.Parse(_part[0]));
                        }
                    }

                    polygon.Add(p);
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

            return (vertexArray, polygon.Select(innerList => innerList.ToArray()).ToArray());
        }
    }
}
