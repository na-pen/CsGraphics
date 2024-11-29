namespace Main
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using CsGraphics;
    using Microsoft.CodeAnalysis.CSharp.Scripting;
    using Microsoft.CodeAnalysis.Scripting;

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

            this.scene = new Scene(60);  // MyDrawable インスタンスを作成
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

                // 画面を更新
                this.graphicsView.Invalidate();

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
        private async void OnCommandEntered(object sender, EventArgs e)
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
            result = await this.ProcessCommand(command) + "\n";

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
        private async Task<string?> ProcessCommand(string command)
        {
            return command.ToLower() switch
            {
                _ when command.StartsWith("!") => await this.ExecuteCodeAsync(command.Substring(1)),
                "hello" => "Hello, user!",
                "time" => DateTime.Now.ToString(),
                "exit" => "Goodbye!",
                "translation" => this.TranslationTest(),
                "scale" => this.ScaleTest(),
                "rotationz" => this.RotationTest(),
                "test" => this.Test(),
                "teapot" => this.AddTeapot(),
                _ => "Unknown command."
            };
        }

        private async Task<string?> ExecuteCodeAsync(string code)
        {
            try
            {
                // 必要なアセンブリを手動で指定
                var assemblies = new[]
                {
                    Assembly.GetAssembly(typeof(object)),  // System.Object
                    Assembly.GetAssembly(typeof(Enumerable)), // System.Linq
                    Assembly.GetAssembly(typeof(System.Collections.Generic.List<>)), // System.Collections.Generic
                };

                // コードをスクリプトとして実行
                var result = await CSharpScript.EvaluateAsync<object>(
                    code,
                    ScriptOptions.Default
                        .WithReferences(Assembly.GetExecutingAssembly()) // 現在のアセンブリを参照
                        .WithImports("System", "System.Linq", "System.Collections.Generic", "CsGraphics"));

                return result != null ? result.ToString() : "Execution completed.";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        private string Test()
        {
            int idRectangle = this.scene.AddObject(
                "rectangle",
                new double[,] { { 100, 300, 300, 100 }, { 100, 100, 400, 400 } });

            return this.Scene.GetObjectInfo(idRectangle);
        }

        private string TranslationTest()
        {
            // 平行移動
            this.Scene.TranslationObject(0, 200, 100, 0);

            return "\nDone!";
        }

        private string ScaleTest()
        {
            // 拡大
            this.Scene.ScaleObject(0, 2, 0.5, 0);

            return this.Scene.GetObjectInfo(0) + "\nDone!";
        }

        private string RotationTest()
        {
            // 回転
            this.Scene.RotationObject(0, 0, 0, 0.5);

            return "\nDone!";
        }

        private string AddTeapot()
        {
            this.Scene.AddObjectFromObj("teapot", "E:/Projects/CsGraphics/CsGraphics/teapot.obj");

            // Math.Vector vec = files.VerticesFromObj("C:/Users/mail/Documents/CsGraphics/CsGraphics/teapot.obj");
            return "Done!";
        }
    }
}
