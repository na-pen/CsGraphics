namespace Main
{
    using CsGraphics;
    using Microsoft.Maui.Graphics.Platform;
    using Microsoft.Maui.Graphics;
    using System;
    using System.Diagnostics;
    using System.Text.RegularExpressions;

    /// <summary>
    /// アプリケーションのメインページ.
    /// </summary>
    public partial class MainPage : ContentPage
    {
        private Scene scene;

        /// <summary>
        /// 画面の更新をするかどうか.
        /// </summary>
        private bool isUpdating = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();

            this.scene = new Scene(10);  // MyDrawable インスタンスを作成
            this.graphicsView.Drawable = this.scene;
            this.BindingContext = this;
            this.Scene = this.scene;  // Drawable に設定

            this.UpdateLoop();
        }

        /// <summary>
        /// Gets or sets シーン.
        /// </summary>
        public Scene Scene { get; set; }

        /// <summary>
        /// 画面更新を止める処理.
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            this.isUpdating = false; // 更新ループを停止
        }

        /// <summary>
        /// 画面更新処理.
        /// </summary>
        private async void UpdateLoop()
        {
            if (this.isUpdating)
            {
                return;
            }

            this.isUpdating = true;
            TimeSpan interval = TimeSpan.FromSeconds(1.0 / this.Scene.FrameRate);

            while (this.isUpdating)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                if (this.scene.IsUpdated)
                {
                    // 画面を更新
                    this.graphicsView.Invalidate();
                }

                // 次のフレームまで待機
                TimeSpan elapsed = stopwatch.Elapsed;
                TimeSpan delay = interval - elapsed; // 待機の時間をラグを見て調整
                if (delay > TimeSpan.Zero)
                {
                    await Task.Delay(delay);
                }
            }
        }

        // コマンドが入力された時の処理
        private void OnCommandEntered(object sender, EventArgs e)
        {
            string? command = this.InputField.Text?.Trim();
            if (string.IsNullOrEmpty(command))
            {
                return;
            }

            // 出力領域にコマンドを追加
            this.AppendOutput($"> {command}");

            // コマンドに応じた結果を出力
            string result = string.Empty;
            result = this.ProcessCommand(command) + "\n";

            this.AppendOutput(result);

            // 入力フィールドをクリア
            this.InputField.Text = string.Empty;
        }

        // 出力領域にテキストを追加
        private void AppendOutput(string text)
        {
            this.OutputArea.Text += text + Environment.NewLine;

            // スクロールを下に自動移動
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await this.OutputScrollView.ScrollToAsync(0, this.OutputArea.Height, true);
            });
        }

        // コマンド処理ロジック
        private string ProcessCommand(string command)
        {
            string pattern = @"\((.*?)\)";
            string[] args = Regex.Match(command, pattern).Groups[1].Value.Split(',');
            return command.ToLower() switch
            {
                _ when command.StartsWith("translation") => this.TranslationTest(int.Parse(args[0]), double.Parse(args[1]), double.Parse(args[2]), double.Parse(args[3])),
                _ when command.StartsWith("scale") => this.ScaleTest(int.Parse(args[0]), double.Parse(args[1]), double.Parse(args[2]), double.Parse(args[3])),
                _ when command.StartsWith("rotation") => this.RotationTest(int.Parse(args[0]), double.Parse(args[1]), double.Parse(args[2]), double.Parse(args[3])),
                _ when command.StartsWith("object") => "ID : " + this.Scene.AddObjectFromObj(args[0].Replace("\"", string.Empty), args[1].Replace("\"", string.Empty)).ToString(),
                _ when command.StartsWith("texture") => this.Png(),
                _ => "Unknown command."
            };
        }

        private string TranslationTest(int id, double x, double y, double z)
        {
            // 平行移動
            this.Scene.TranslationObject(id, x, y, z);

            return "Done!";
        }

        private string ScaleTest(int id, double x, double y, double z)
        {
            // 拡大
            this.Scene.ScaleObject(id, x, y, z);

            return "Done!";
        }

        private string RotationTest(int id, double x, double y, double z)
        {
            // 回転
            this.Scene.RotationObject(id, x, y, z);

            return "Done!";
        }

        private string Png()
        {
            string filePath = "C:\\Users\\mail\\OneDrive\\tx\\服1.png";
            (PngFile pngFile, byte[] b) = ParsePngFile(filePath);

            Color[,] colors = GetPixelsFromDecompressedData(b, 2048, 2048);
            using (var stream = new MemoryStream())
            {
                Bitmap bitmap = new(2048, 2048, colors);
                // ストリームに画像データを書き込む
                stream.Write(bitmap.FileHeader.Bytes);
                stream.Write(bitmap.InfoHeader.Bytes);
                stream.Write(bitmap.img);
                stream.Position = 0;
                IImage image = PlatformImage.FromStream(stream, ImageFormat.Bmp);
                // PlatformImage をストリームから読み込んで画像を作成


                using (FileStream fs = new FileStream("E:\\Projects\\CsGraphics\\Main\\test.bmp", FileMode.Create))
                {
                    fs.Write(bitmap.FileHeader.Bytes);
                    fs.Write(bitmap.InfoHeader.Bytes);
                    fs.Write(bitmap.img);
                }
            }
            return "Done";
        }

        static (PngFile, byte[]) ParsePngFile(string filePath)
        {
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (var reader = new BinaryReader(stream))
            {
                var pngFile = new PngFile();

                // シグネチャを読み取る
                pngFile.Signature = reader.ReadBytes(8);
                if (BitConverter.ToString(pngFile.Signature) != "89-50-4E-47-0D-0A-1A-0A")
                    throw new InvalidDataException("Invalid PNG signature.");

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
                        pngFile.Chunks.Add(new IhdrChunk(data));
                    }
                    else if (type == "IDAT")
                    {
                        pngFile.Chunks.Add(new PngChunk(length, type, data, crc));
                        pngFile.IDAT.AddRange(data);
                    }

                    if (type == "IEND")
                        break;
                }
                byte[] a = pngFile.DecompressIdatData();
                return (pngFile, a);
            }
        }
        static byte PaethPredictor(byte a, byte b, byte c)
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

        static Color[,] GetPixelsFromDecompressedData(byte[] data, int width, int height)
        {
            int bytesPerPixel = 4; // RGBA
            int rowLength = width * bytesPerPixel; // 1行あたりのバイト数
            Color[,] pixels = new Color[width, height];
            int dataIndex = 0;

            

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

                        pixels[x, y] = new Color(r, g, b, a);
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

                        pixels[x, y] = new Color(r, g, b, a);

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

                        r += (byte)(int)(pixels[x, y -1].Red *255);
                        g += (byte)(int)(pixels[x, y - 1].Green * 255);
                        b += (byte)(int)(pixels[x, y - 1].Blue * 255);
                        a += (byte)(int)(pixels[x, y - 1].Alpha * 255);

                        pixels[x, y] = new Color(r, g, b, a);

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

                        byte avgR = (byte)((previousR + (byte)(int)(pixels[x, y - 1].Red * 255)) / 2);
                        byte avgG = (byte)((previousG + (byte)(int)(pixels[x, y - 1].Green * 255)) / 2);
                        byte avgB = (byte)((previousB + (byte)(int)(pixels[x, y - 1].Blue * 255)) / 2);
                        byte avgA = (byte)((previousA + (byte)(int)(pixels[x, y - 1].Alpha * 255)) / 2);

                        r += avgR;
                        g += avgG;
                        b += avgB;
                        a += avgA;

                        pixels[x, y] = new Color(r, g, b, a);

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

                        try
                        {
                            pr = PaethPredictor(previousR, (byte)(int)(pixels[x, y - 1].Red * 255), (byte)(int)(pixels[x - 1, y - 1].Red * 255)); // Red
                            pg = PaethPredictor(previousG, (byte)(int)(pixels[x, y - 1].Green * 255), (byte)(int)(pixels[x - 1, y - 1].Green * 255)); // Green
                            pb = PaethPredictor(previousB, (byte)(int)(pixels[x, y - 1].Blue * 255), (byte)(int)(pixels[x - 1, y - 1].Blue * 255)); // Blue
                            pa = PaethPredictor(previousA, (byte)(int)(pixels[x, y - 1].Alpha * 255), (byte)(int)(pixels[x - 1, y - 1].Alpha * 255)); // Alpha
                        }
                        catch
                        {

                            pr = PaethPredictor(previousR, (byte)(int)(pixels[x, y - 1].Red * 255), 0); // Red
                            pg = PaethPredictor(previousG, (byte)(int)(pixels[x, y - 1].Green * 255), 0); // Green
                            pb = PaethPredictor(previousB, (byte)(int)(pixels[x, y - 1].Blue * 255), 0); // Blue
                            pa = PaethPredictor(previousA, (byte)(int)(pixels[x, y - 1].Alpha * 255), 0); // Alpha
                        }

                        // 差分を加算して元に戻す
                        r += pr;
                        g += pg;
                        b += pb;
                        a += pa;

                        // ピクセルを配列に格納
                        pixels[x, y] = new Color(r, g, b, a);

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

            return pixels;
        }
    }
}
