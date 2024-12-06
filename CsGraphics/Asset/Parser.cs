namespace CsGraphics.Asset
{
    using CsGraphics.Math;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// 外部ファイルの読み込み.
    /// </summary>
    internal static class Parser
    {
        internal static (double[,], Dictionary<string, int[][]>, Matrix[], Dictionary<string, (Color, string)>, Dictionary<string, int[][]>, double[][]) ObjParseVerticesV2(string filePath)
        {
            var vertices = new List<double[]>(); // 動的リストで頂点情報を一時的に格納
            List<double[]> verticesT = new List<double[]>(); // 動的リストでテクスチャ座標情報を一時的に格納
            Dictionary<string, List<List<int>>> polygon = new Dictionary<string, List<List<int>>>(); // 動的リストで面を構成する頂点のIDを一時的に格納
            Dictionary<string, List<List<int>>> mtlV = new Dictionary<string, List<List<int>>>();
            List<Matrix> normal = new List<Matrix>(); // 動的リストで面の法線ベクトルを一時的に格納
            List<Color> color = new List<Color>(); // ポリゴンカラー
            Dictionary<string, (Color, string)> dic = new Dictionary<string, (Color, string)>();
            string mtlNow = string.Empty;

            polygon.Add(mtlNow, new List<List<int>> { });
            mtlV.Add(mtlNow, new List<List<int>> { });

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
                        vertices.Add(new[] { -x, y, z });
                    }
                }
                else if (line.StartsWith("vt ")) // テクスチャ頂点情報の場合
                {
                    // "vt x y" を解析
                    var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 3) // テクスチャ頂点座標が存在する場合
                    {
                        double x = double.Parse(parts[1]);
                        double y = double.Parse(parts[2]);

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
                        if (mtlNow != string.Empty)
                        {
                            color.Add(dic[mtlNow].Item1);
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

                    polygon[mtlNow].Add(p);
                    mtlV[mtlNow].Add(mtl);
                }
                else if (line.StartsWith("mtllib ")) // 面情報のとき
                {
                    var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    dic = MtlParserV2(Path.GetDirectoryName(filePath) + "\\" + parts[1]);
                }
                else if (line.StartsWith("usemtl ")) // 面情報のとき
                {
                    var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    mtlNow = parts[1];
                    polygon.TryAdd(mtlNow, new List<List<int>> { });
                    mtlV.TryAdd(mtlNow, new List<List<int>> { });

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
            foreach (KeyValuePair<string, List<List<int>>> ply in polygon)
            {
                foreach (List<int> indices in ply.Value)
                {
                    CsGraphics.Math.Vector ab = new(
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

                    CsGraphics.Math.Vector bc = new(new double[]
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

                    CsGraphics.Math.Vector temp = Vector.CrossProduct(ab, bc);
                    normal.Add(temp.Data);
                }
            }

            return (vertexArray,
                polygon.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Select(innerList => innerList.ToArray()).ToArray()
                ),
                normal.ToArray(),
                dic,
                mtlV.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Select(innerList => innerList.ToArray()).ToArray()
                    ),
                verticesT.ToArray());
        }

        private static Dictionary<string, (Color, string)> MtlParserV2(string path)
        {
            Dictionary<string, (Color, string)> dic = new Dictionary<string, (Color, string)>();

            string mtlName = string.Empty;
            double d = 1;
            foreach (var line in File.ReadLines(path))
            {
                if (line.StartsWith("newmtl ")) // 頂点情報の場合
                {
                    var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    mtlName = parts[1];
                    dic.Add(mtlName, (new Color(), string.Empty));
                }
                else if (line.StartsWith("Kd ")) // 面情報のとき
                {
                    var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    Color x = new Color((int)(double.Parse(parts[1]) * 255), (int)(double.Parse(parts[2]) * 255), (int)(double.Parse(parts[3]) * 255), (int)(d * 255));
                    if (!dic.TryAdd(mtlName, (x, string.Empty)))
                    {
                        dic[mtlName] = (x, dic[mtlName].Item2);
                    }
                }
                else if (line.StartsWith("map_Kd "))
                {
                    var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    dic[mtlName] = (dic[mtlName].Item1, Path.GetDirectoryName(path) + "\\" + parts[1]);
                }
                else if (line.StartsWith("d ")) // テクスチャ透過率のとき
                {
                    var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    d = double.Parse(parts[1]);
                    string lastKey = dic.Keys.Last();
                    dic[lastKey] = (new Color(dic[lastKey].Item1.Red, dic[lastKey].Item1.Green, dic[lastKey].Item1.Blue, (int)(d * 255)), dic[lastKey].Item2);
                }
            }

            return dic;
        }
    }
}
