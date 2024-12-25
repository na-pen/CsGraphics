namespace CsGraphics.Object.Asset.Image
{
    using Microsoft.Maui.Graphics.Platform;
    using Microsoft.Maui.Graphics;
    using System;
    using System.Collections.Generic;
    using System.IO.Compression;
    using System.Reflection;
    using CsGraphics.Object.Asset.Image;
    using CsGraphics.Object;

    internal class Png
    {
        internal byte[] Signature { get; set; } // PNGファイルシグネチャ

        internal List<byte> IDAT { get; set; } = new List<byte>();

        internal IhdrChunk IHDR { get; set; }

        internal Png()
        {
            Signature = new byte[8];
        }

        internal static (int, int, byte[]) LoadFromFile(string filePath)
        {
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (var reader = new BinaryReader(stream))
            {
                var pngFile = new Png();

                // シグネチャを読み取る
                pngFile.Signature = reader.ReadBytes(8);
                if (BitConverter.ToString(pngFile.Signature) != "89-50-4E-47-0D-0A-1A-0A")
                {
                    throw new InvalidDataException("Invalid PNG signature.");
                }

                // チャンクを読み取る
                while (stream.Position < stream.Length)
                {
                    byte[] l = reader.ReadBytes(4);
                    Array.Reverse(l);
                    uint length = (uint)BitConverter.ToInt32(l, 0);
                    string type = new string(reader.ReadChars(4));
                    byte[] data = reader.ReadBytes((int)length);
                    uint crc = reader.ReadUInt32();

                    if (type == "IHDR")
                    {
                        pngFile.IHDR = new IhdrChunk(data);
                    }
                    else if (type == "IDAT")
                    {
                        pngFile.IDAT.AddRange(data);
                    }

                    if (type == "IEND")
                        break;
                }


                return GetPixelsFromDecompressedData(pngFile.DecompressIdatData(), (int)pngFile.IHDR.Width, (int)pngFile.IHDR.Height);
            }
        }

        private static byte PaethPredictor(byte a, byte b, byte c)
        {
            int p = a + b - c; // 初期予測値

            // 各絶対誤差を計算
            int pa = Math.Abs(p - a);
            int pb = Math.Abs(p - b);
            int pc = Math.Abs(p - c);

            // 最小の誤差を持つ予測値を選ぶ
            if (pa <= pb && pa <= pc)
            {
                return a;
            }
            else if (pb <= pc)
            {
                return b;
            }
            else
            {
                return c;
            }
        }

        private static (int, int, byte[]) GetPixelsFromDecompressedData(byte[] data, int width, int height)
        {
            int bytesPerPixel = 4; // RGBA
            int rowLength = width * bytesPerPixel; // 1行あたりのバイト数
            //Color[,] pixels = new Color[width, height];
            int dataIndex = 0;

            byte[] bytes = new byte[width * height * 4];

            // データは1行ずつ処理される
            for (int y = 0; y < height; y++)
            {
                // フィルタバイトを取得
                byte filterByte = data[dataIndex];
                dataIndex++; // フィルタバイトをスキップ

                byte previousR = 0, previousG = 0, previousB = 0, previousA = 0;
                byte aboveR = 0, aboveG = 0, aboveB = 0, aboveA = 0;

                // フィルタの種類に応じて処理
                if (filterByte == 0) // None
                {
                    // フィルタがない場合はそのまま格納
                    for (int x = 0; x < width; x++)
                    {
                        byte r = data[dataIndex++];
                        byte g = data[dataIndex++];
                        byte b = data[dataIndex++];
                        byte a = data[dataIndex++];

                        //pixels[x, height - y - 1] = new Color(r, g, b, a);
                        bytes[width * (height - y - 1) * 4 + x * 4 + 0] = r;
                        bytes[width * (height - y - 1) * 4 + x * 4 + 1] = g;
                        bytes[width * (height - y - 1) * 4 + x * 4 + 2] = b;
                        bytes[width * (height - y - 1) * 4 + x * 4 + 3] = a;

                    }
                }
                else if (filterByte == 1) // Sub
                {
                    // Subフィルタ（前のピクセルとの差分を復元）
                    for (int x = 0; x < width; x++)
                    {
                        byte r = data[dataIndex++];
                        byte g = data[dataIndex++];
                        byte b = data[dataIndex++];
                        byte a = data[dataIndex++];

                        r += previousR;
                        g += previousG;
                        b += previousB;
                        a += previousA;

                        //pixels[x, height - y - 1] = new Color(r, g, b, a);
                        bytes[width * (height - y - 1) * 4 + x * 4 + 0] = r;
                        bytes[width * (height - y - 1) * 4 + x * 4 + 1] = g;
                        bytes[width * (height - y - 1) * 4 + x * 4 + 2] = b;
                        bytes[width * (height - y - 1) * 4 + x * 4 + 3] = a;

                        previousR = r;
                        previousG = g;
                        previousB = b;
                        previousA = a;
                    }
                }
                else if (filterByte == 2) // Up
                {
                    // Upフィルタ（前の行の同じ位置のピクセルとの差分を復元）
                    for (int x = 0; x < width; x++)
                    {
                        byte r = data[dataIndex++];
                        byte g = data[dataIndex++];
                        byte b = data[dataIndex++];
                        byte a = data[dataIndex++];

                        r += (byte)(int)bytes[width * (height - y) * 4 + x * 4 + 0];
                        g += (byte)(int)bytes[width * (height - y) * 4 + x * 4 + 1];
                        b += (byte)(int)bytes[width * (height - y) * 4 + x * 4 + 2];
                        a += (byte)(int)bytes[width * (height - y) * 4 + x * 4 + 3];
                        /*
                        r += (byte)(int)(pixels[x, height - y].Red * 255);
                        g += (byte)(int)(pixels[x, height - y].Green * 255);
                        b += (byte)(int)(pixels[x, height - y].Blue * 255);
                        a += (byte)(int)(pixels[x, height - y].Alpha * 255);
                        */

                        //pixels[x, height - y - 1] = new Color(r, g, b, a);
                        bytes[width * (height - y - 1) * 4 + x * 4 + 0] = r;
                        bytes[width * (height - y - 1) * 4 + x * 4 + 1] = g;
                        bytes[width * (height - y - 1) * 4 + x * 4 + 2] = b;
                        bytes[width * (height - y - 1) * 4 + x * 4 + 3] = a;

                    }
                }
                else if (filterByte == 3) // Average
                {
                    // Averageフィルタ（前のピクセルと前の行の同じ位置のピクセルの平均値を復元）
                    for (int x = 0; x < width; x++)
                    {
                        byte r = data[dataIndex++];
                        byte g = data[dataIndex++];
                        byte b = data[dataIndex++];
                        byte a = data[dataIndex++];

                        byte avgR = (byte)((previousR + (byte)(int)bytes[width * (height - y) * 4 + x * 4 + 0]) / 2);
                        byte avgG = (byte)((previousG + (byte)(int)bytes[width * (height - y) * 4 + x * 4 + 1]) / 2);
                        byte avgB = (byte)((previousB + (byte)(int)bytes[width * (height - y) * 4 + x * 4 + 2]) / 2);
                        byte avgA = (byte)((previousA + (byte)(int)bytes[width * (height - y) * 4 + x * 4 + 3]) / 2);
                        /*
                        byte avgR = (byte)((previousR + (byte)(int)(pixels[x, height - y].Red * 255)) / 2);
                        byte avgG = (byte)((previousG + (byte)(int)(pixels[x, height - y].Green * 255)) / 2);
                        byte avgB = (byte)((previousB + (byte)(int)(pixels[x, height - y].Blue * 255)) / 2);
                        byte avgA = (byte)((previousA + (byte)(int)(pixels[x, height - y].Alpha * 255)) / 2);
                        */

                        r += avgR;
                        g += avgG;
                        b += avgB;
                        a += avgA;

                        //pixels[x, height - y - 1] = new Color(r, g, b, a);
                        bytes[width * (height - y - 1) * 4 + x * 4 + 0] = r;
                        bytes[width * (height - y - 1) * 4 + x * 4 + 1] = g;
                        bytes[width * (height - y - 1) * 4 + x * 4 + 2] = b;
                        bytes[width * (height - y - 1) * 4 + x * 4 + 3] = a;

                        previousR = r;
                        previousG = g;
                        previousB = b;
                        previousA = a;
                    }
                }
                else if (filterByte == 4) // Paeth
                {
                    // Paethフィルタ（最適な値を前のピクセル、前の行、前の行の前のピクセルから選択）
                    for (int x = 0; x < width; x++)
                    {
                        byte r = data[dataIndex++];
                        byte g = data[dataIndex++];
                        byte b = data[dataIndex++];
                        byte a = data[dataIndex++];

                        // Paethフィルタを適用して前のピクセル、上のピクセル、左上のピクセルを考慮
                        byte pr = 0; // Red
                        byte pg = 0; // Green
                        byte pb = 0; // Blue
                        byte pa = 0; // Alpha

                        if (x != 0)
                        {

                            pr = PaethPredictor(previousR, (byte)(int)bytes[width * (height - y) * 4 + x * 4 + 0], (byte)(int)bytes[width * (height - y) * 4 + (x - 1) * 4 + 0]); // Red
                            pg = PaethPredictor(previousG, (byte)(int)bytes[width * (height - y) * 4 + x * 4 + 1], (byte)(int)bytes[width * (height - y) * 4 + (x - 1) * 4 + 1]); // Green
                            pb = PaethPredictor(previousB, (byte)(int)bytes[width * (height - y) * 4 + x * 4 + 2], (byte)(int)bytes[width * (height - y) * 4 + (x - 1) * 4 + 2]); // Blue
                            pa = PaethPredictor(previousA, (byte)(int)bytes[width * (height - y) * 4 + x * 4 + 3], (byte)(int)bytes[width * (height - y) * 4 + (x - 1) * 4 + 3]); // Alpha
                            /*
                            pr = PaethPredictor(previousR, (byte)(int)(pixels[x, height - y].Red * 255), (byte)(int)(pixels[x - 1, height - y].Red * 255)); // Red
                            pg = PaethPredictor(previousG, (byte)(int)(pixels[x, height - y].Green * 255), (byte)(int)(pixels[x - 1, height - y].Green * 255)); // Green
                            pb = PaethPredictor(previousB, (byte)(int)(pixels[x, height - y].Blue * 255), (byte)(int)(pixels[x - 1, height - y].Blue * 255)); // Blue
                            pa = PaethPredictor(previousA, (byte)(int)(pixels[x, height - y].Alpha * 255), (byte)(int)(pixels[x - 1, height - y].Alpha * 255)); // Alpha
                            */
                        }
                        else
                        {

                            pr = PaethPredictor(previousR, (byte)(int)bytes[width * (height - y) * 4 + x * 4 + 0], 0); // Red
                            pg = PaethPredictor(previousG, (byte)(int)bytes[width * (height - y) * 4 + x * 4 + 1], 0); // Green
                            pb = PaethPredictor(previousB, (byte)(int)bytes[width * (height - y) * 4 + x * 4 + 2], 0); // Blue
                            pa = PaethPredictor(previousA, (byte)(int)bytes[width * (height - y) * 4 + x * 4 + 3], 0); // Alpha
                            /*
                            pr = PaethPredictor(previousR, (byte)(int)(pixels[x, height - y].Red * 255), 0); // Red
                            pg = PaethPredictor(previousG, (byte)(int)(pixels[x, height - y].Green * 255), 0); // Green
                            pb = PaethPredictor(previousB, (byte)(int)(pixels[x, height - y].Blue * 255), 0); // Blue
                            pa = PaethPredictor(previousA, (byte)(int)(pixels[x, height - y].Alpha * 255), 0); // Alpha
                            */
                        }

                        // 差分を加算して元に戻す
                        r += pr;
                        g += pg;
                        b += pb;
                        a += pa;

                        // ピクセルを配列に格納
                        //pixels[x, height - y - 1] = new Color(r, g, b, a);
                        bytes[width * (height - y - 1) * 4 + x * 4 + 0] = r;
                        bytes[width * (height - y - 1) * 4 + x * 4 + 1] = g;
                        bytes[width * (height - y - 1) * 4 + x * 4 + 2] = b;
                        bytes[width * (height - y - 1) * 4 + x * 4 + 3] = a;

                        // 前のピクセルとして現在の値を保存
                        previousR = r;
                        previousG = g;
                        previousB = b;
                        previousA = a;

                        // 上のピクセルとして現在の値を保存
                        aboveR = r;
                        aboveG = g;
                        aboveB = b;
                        aboveA = a;
                    }
                }
            }

            return (width, height, bytes);
        }

        private byte[] DecompressIdatData()
        {
            byte[] idatData = IDAT.ToArray();
            if (idatData == null || idatData.Length < 2)
            {
                throw new ArgumentException("Invalid IDAT data.");
            }

            // zlib ヘッダーを検証 (最初の2バイト)
            if (idatData[0] != 0x78)
            {
                throw new InvalidOperationException("Invalid zlib header in IDAT data.");
            }

            idatData = RemoveZlibHeader(idatData);
            // zlib データを解凍
            using (var inputStream = new MemoryStream(idatData))
            using (var deflateStream = new DeflateStream(inputStream, CompressionMode.Decompress))
            using (var outputStream = new MemoryStream())
            {
                deflateStream.CopyTo(outputStream);
                return outputStream.ToArray();
            }
        }

        private static byte[] RemoveZlibHeader(byte[] data)
        {
            // zlib ヘッダー (最初の2バイト) を取り除く
            if (data.Length > 2)
            {
                return data[2..];
            }
            throw new ArgumentException("Invalid zlib data: Data too short.");
        }
    }

    // チャンクの基本構造
    internal class PngChunk
    {
        internal uint Length { get; set; } // チャンクデータの長さ

        internal string Type { get; set; } // チャンクタイプ（例: IHDR, IDAT, etc.）

        internal byte[] Data { get; set; } // チャンクデータ

        internal uint Crc { get; set; } // チャンクのCRC

        internal PngChunk(uint length, string type, byte[] data, uint crc)
        {
            Length = length;
            Type = type;
            Data = data;
            Crc = crc;
        }

    }

    // IHDRチャンク専用クラス
    internal class IhdrChunk : PngChunk
    {
        internal uint Width { get; set; } // 画像の幅

        internal uint Height { get; set; } // 画像の高さ

        internal byte BitDepth { get; set; } // ビット深度

        internal byte ColorType { get; set; } // カラーモード

        internal byte CompressionMethod { get; set; } // 圧縮方法

        internal byte FilterMethod { get; set; } // フィルタリング方法

        internal byte InterlaceMethod { get; set; } // インタレース方法

        internal IhdrChunk(byte[] data)
            : base((uint)data.Length, "IHDR", data, 0)
        {
            if (data.Length != 13)
            {
                throw new ArgumentException("IHDR data must be 13 bytes.");
            }

            var w = new ArraySegment<byte>(data, 0, 4).ToArray();
            Array.Reverse(w);


            var l = new ArraySegment<byte>(data, 4, 4).ToArray();
            Array.Reverse(l);

            Width = BitConverter.ToUInt32(w);
            Height = BitConverter.ToUInt32(l);
            BitDepth = data[8];
            ColorType = data[9];
            CompressionMethod = data[10];
            if (CompressionMethod != 0)
            {
                throw new Exception("その圧縮方式には対応していません");
            }
            FilterMethod = data[11];
            if (FilterMethod != 0)
            {
                throw new Exception("そのフィルター方式には対応していません");
            }
            InterlaceMethod = data[12];
        }
    }
}
