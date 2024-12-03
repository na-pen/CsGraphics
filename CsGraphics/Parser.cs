namespace CsGraphics
{
    using CsGraphics.Math;
    using Microsoft.Maui.Graphics.Platform;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

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
        internal static (double[,],int[][], Matrix[], Color[], int[][], double[][]) ObjParseVertices(string filePath)
        {
            var vertices = new List<double[]>(); // 動的リストで頂点情報を一時的に格納
            var verticesT = new List<double[]>(); // 動的リストでテクスチャ座標情報を一時的に格納
            List<List<int>> polygon = new List<List<int>>(); // 動的リストで面を構成する頂点のIDを一時的に格納
            List<Matrix> normal = new List<Matrix>(); // 動的リストで面の法線ベクトルを一時的に格納
            List<Color> color = new List<Color>(); // ポリゴンカラー
            Dictionary<string, Color> dic = new Dictionary<string, Color>();
            List<List<int>> mtlV = new List<List<int>>();
            string mtlNow = String.Empty;

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
                else if (line.StartsWith("vt ")) // 頂点情報の場合
                {
                    // "vt x y" を解析
                    var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 3) // 頂点座標が存在する場合
                    {
                        double x = double.Parse(parts[1]) % 1;
                        double y = double.Parse(parts[2]) % 1;
                        verticesT.Add(new[] { x, y });
                    }
                }
                else if (line.StartsWith("f ")) // 面情報のとき
                {
                    List<int> p = new List<int>();
                    List<int> mtl = new List<int>();
                    var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 4) // 面の情報が存在する場合
                    {
                        if (mtlNow != String.Empty)
                        {
                            color.Add(dic[mtlNow]);
                        }
                        else
                        {
                            color.Add(new Color(0, 0, 0));
                        }

                        foreach (var part in parts.Skip(1))
                        {
                            string[] _part = part.Split("/");

                            p.Add(int.Parse(_part[0]));
                            mtl.Add(int.Parse(_part[1]));
                        }
                    }

                    polygon.Add(p);
                    mtlV.Add(mtl);
                }
                else if (line.StartsWith("mtllib ")) // 面情報のとき
                {
                    var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    dic = MtlParser(System.IO.Path.GetDirectoryName(filePath) + "\\" +parts[1]);
                }
                else if (line.StartsWith("usemtl ")) // 面情報のとき
                {
                    var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    mtlNow = parts[1];
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

            // 面の法線ベクトルを計算
            foreach (List<int> indices in polygon)
            {
                Math.Vector ab = new (
                    new double[]
                    {
                        vertices[indices[0] - 1][0],
                        vertices[indices[0] - 1][1],
                        vertices[indices[0] - 1][2],
                    },
                    new double[]
                    {
                        vertices[indices[1] - 1][0],
                        vertices[indices[1] - 1][1],
                        vertices[indices[1] - 1][2],
                    });

                Math.Vector bc = new (new double[]
                    {
                        vertices[indices[1] - 1][0],
                        vertices[indices[1] - 1][1],
                        vertices[indices[1] - 1][2],
                    },
                    new double[]
                    {
                        vertices[indices[2] - 1][0],
                        vertices[indices[2] - 1][1],
                        vertices[indices[2] - 1][2]
                    });

                Math.Vector temp = Vector.CrossProduct(ab, bc);
                normal.Add(temp.Data);
            }

            return (vertexArray, polygon.Select(innerList => innerList.ToArray()).ToArray(), normal.ToArray(), color.ToArray(), mtlV.Select(innerList => innerList.ToArray()).ToArray(), verticesT.ToArray());
        }

        private static Dictionary<string, Color> MtlParser(string path)
        {
            Dictionary<string, Color> dic = new Dictionary<string, Color>();

            string mtlName = string.Empty;
            foreach (var line in File.ReadLines(path))
            {
                if (line.StartsWith("newmtl ")) // 頂点情報の場合
                {
                    var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    mtlName = parts[1];
                }
                else if (line.StartsWith("Kd ")) // 面情報のとき
                {
                    var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    dic.Add(mtlName, new Color((int)(double.Parse(parts[1]) * 255), (int)(double.Parse(parts[2]) * 255), (int)(double.Parse(parts[3]) * 255)));
                }
            }

            return dic;
        }
    }
}
