using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsGraphics
{
    public class PngFile
    {
        public byte[] Signature { get; set; } // PNGファイルシグネチャ
        public List<PngChunk> Chunks { get; set; } // PNGチャンクのリスト

        public List<byte> IDAT { get; set; } = new List<byte>();

        public PngFile()
        {
            Signature = new byte[8];
            Chunks = new List<PngChunk>();
        }

        public byte[] DecompressIdatData()
        {
            byte[] idatData = this.IDAT.ToArray();
            if (idatData == null || idatData.Length < 2)
                throw new ArgumentException("Invalid IDAT data.");

            // zlib ヘッダーを検証 (最初の2バイト)
            if (idatData[0] != 0x78)
                throw new InvalidOperationException("Invalid zlib header in IDAT data.");

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
    public class PngChunk
    {
        public uint Length { get; set; } // チャンクデータの長さ
        public string Type { get; set; } // チャンクタイプ（例: IHDR, IDAT, etc.）
        public byte[] Data { get; set; } // チャンクデータ
        public uint Crc { get; set; } // チャンクのCRC

        public PngChunk(uint length, string type, byte[] data, uint crc)
        {
            Length = length;
            Type = type;
                Data = data;
            Crc = crc;
        }

    }



    // IHDRチャンク専用クラス
    public class IhdrChunk : PngChunk
    {
        public uint Width { get; set; } // 画像の幅
        public uint Height { get; set; } // 画像の高さ
        public byte BitDepth { get; set; } // ビット深度
        public byte ColorType { get; set; } // カラーモード
        public byte CompressionMethod { get; set; } // 圧縮方法
        public byte FilterMethod { get; set; } // フィルタリング方法
        public byte InterlaceMethod { get; set; } // インタレース方法

        public IhdrChunk(byte[] data) : base((uint)data.Length, "IHDR", data, 0)
        {
            if (data.Length != 13) throw new ArgumentException("IHDR data must be 13 bytes.");

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
            if(FilterMethod != 0)
            {
                throw new Exception("そのフィルター方式には対応していません");
            }
            InterlaceMethod = data[12];
        }
    }
}
