using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CsGraphics
{
    internal class Bitmap
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct BITMAPFILEHEADER()
        {
            public UInt16 bfType = 0x4d42;
            public UInt32 bfSize;
            public UInt16 bfReserved1 = 0;
            public UInt16 bfReserved2 = 0;
            public UInt32 bfOffBits = 54;

            public int Length = 14;

            public byte[] Bytes
            {
                get
                {
                    byte[] Datas = new byte[14];

                    Array.Copy(BitConverter.GetBytes(bfType), 0, Datas, 0, 2);
                    Array.Copy(BitConverter.GetBytes(bfSize), 0, Datas, 2, 4);
                    Array.Copy(BitConverter.GetBytes(bfReserved1), 0, Datas, 6, 2);
                    Array.Copy(BitConverter.GetBytes(bfReserved2), 0, Datas, 8, 2);
                    Array.Copy(BitConverter.GetBytes(bfOffBits), 0, Datas, 10, 4);

                    return (Datas);
                }
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct BITMAPINFOHEADER(int width, int height, int bitCount = 32)
        {
            public UInt32 biSize = 40;
            public Int32 biWidth = width;
            public Int32 biHeight = height;
            public UInt16 biPlanes = 1;
            public UInt16 biBitCount = (ushort)bitCount;
            public UInt32 biCompression = 0;
            public UInt32 biSizeImage = (uint)((width * height * bitCount) / 8);
            public Int32 biXPelsPerMeter = 0;
            public Int32 biYPelsPerMeter = 0;
            public UInt32 biClrUsed = 0;
            public UInt32 biClrImportant = 0;

            public int Length = 40;

            public byte[] Bytes
            {
                get
                {
                    byte[] Datas = new byte[40];

                    Array.Copy(BitConverter.GetBytes(biSize), 0, Datas, 0, 4);
                    Array.Copy(BitConverter.GetBytes(biWidth), 0, Datas, 4, 4);
                    Array.Copy(BitConverter.GetBytes(biHeight), 0, Datas, 8, 4);
                    Array.Copy(BitConverter.GetBytes(biPlanes), 0, Datas, 12, 2);
                    Array.Copy(BitConverter.GetBytes(biBitCount), 0, Datas, 14, 2);
                    Array.Copy(BitConverter.GetBytes(biCompression), 0, Datas, 16, 4);
                    Array.Copy(BitConverter.GetBytes(biSizeImage), 0, Datas, 20, 4);
                    Array.Copy(BitConverter.GetBytes(biXPelsPerMeter), 0, Datas, 24, 4);
                    Array.Copy(BitConverter.GetBytes(biYPelsPerMeter), 0, Datas, 28, 4);
                    Array.Copy(BitConverter.GetBytes(biClrUsed), 0, Datas, 32, 4);
                    Array.Copy(BitConverter.GetBytes(biClrImportant), 0, Datas, 36, 4);

                    return (Datas);
                }
            }
        }

        internal BITMAPFILEHEADER FileHeader;
        internal BITMAPINFOHEADER InfoHeader;
        internal byte[] img;

        public Bitmap(int width, int height, Color[,] data, int bitCount = 32)
        {
            this.FileHeader = new();

            int rowSize = width * 4; // 4バイト境界に揃える
            int bufferSize = rowSize * height; // 全体のバッファサイズ

            this.InfoHeader = new BITMAPINFOHEADER(rowSize/4, height, bitCount = 32);

            this.FileHeader.bfSize = (uint)((bufferSize/4 * bitCount) / 8) + this.FileHeader.bfOffBits;


            // バッファサイズを計算 (BGR8)
            this.img = new byte[bufferSize * 4]; // BGR
            int index = 0;
            for (int y = 0; y < height; y++)
            {
                int x = 0;
                for (x = 0; x < width; x++)
                {
                    // BGRA に変換して格納
                    img[index++] = (byte)(data[x, y].Blue * 255);  // Blue
                    img[index++] = (byte)(data[x, y].Green * 255); // Green
                    img[index++] = (byte)(data[x, y].Red * 255);   // Red
                    img[index++] = (byte)(data[x, y].Alpha * 255);   // Alpha
                }

                // パディングバイトを追加
                while (index % 4 != 0)
                {
                    img[index++] = 0;
                }
            }
        }

        public static Color[,] LoadFromFile(string filePath)
        {
            using FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using BinaryReader reader = new BinaryReader(fs);

            // 1. ファイルヘッダーを読み取る
            var fileHeaderBytes = reader.ReadBytes(14);
            BITMAPFILEHEADER fileHeader = new();
            fileHeader.bfType = BitConverter.ToUInt16(fileHeaderBytes, 0);
            fileHeader.bfSize = BitConverter.ToUInt32(fileHeaderBytes, 2);
            fileHeader.bfReserved1 = BitConverter.ToUInt16(fileHeaderBytes, 6);
            fileHeader.bfReserved2 = BitConverter.ToUInt16(fileHeaderBytes, 8);
            fileHeader.bfOffBits = BitConverter.ToUInt32(fileHeaderBytes, 10);

            // 2. 情報ヘッダーを読み取る
            var infoHeaderBytes = reader.ReadBytes(40);
            BITMAPINFOHEADER infoHeader = new(
                BitConverter.ToInt32(infoHeaderBytes, 4),
                BitConverter.ToInt32(infoHeaderBytes, 8),
                BitConverter.ToUInt16(infoHeaderBytes, 14)
            )
            {
                biSize = BitConverter.ToUInt32(infoHeaderBytes, 0),
                biPlanes = BitConverter.ToUInt16(infoHeaderBytes, 12),
                biCompression = BitConverter.ToUInt32(infoHeaderBytes, 16),
                biSizeImage = BitConverter.ToUInt32(infoHeaderBytes, 20),
                biXPelsPerMeter = BitConverter.ToInt32(infoHeaderBytes, 24),
                biYPelsPerMeter = BitConverter.ToInt32(infoHeaderBytes, 28),
                biClrUsed = BitConverter.ToUInt32(infoHeaderBytes, 32),
                biClrImportant = BitConverter.ToUInt32(infoHeaderBytes, 36)
            };

            // 3. ピクセルデータを読み取る
            int width = infoHeader.biWidth;
            int height = infoHeader.biHeight;
            int bitCount = infoHeader.biBitCount;
            int rowSize = ((width * bitCount + 31) / 32) * 4; // 4バイト境界
            int imageSize = rowSize * System.Math.Abs(height);

            byte[] pixelData = reader.ReadBytes(imageSize);

            // 4. データをBitmap形式に変換
            Color[,] colors = new Color[width, height];
            int index = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (bitCount == 32)
                    {
                        byte blue = pixelData[index++];
                        byte green = pixelData[index++];
                        byte red = pixelData[index++];
                        byte alpha = pixelData[index++];
                        colors[x, height - y - 1] = new Color(red / 255.0f, green / 255.0f, blue / 255.0f, alpha / 255.0f);
                    }
                    else if (bitCount == 24)
                    {
                        byte blue = pixelData[index++];
                        byte green = pixelData[index++];
                        byte red = pixelData[index++];
                        colors[x, height - y - 1] = new Color(red / 255.0f, green / 255.0f, blue / 255.0f, 1.0f);
                    }
                }

                // パディングバイトをスキップ
                index += rowSize - width * (bitCount / 8);
            }

            return colors;
        }
    }

}
