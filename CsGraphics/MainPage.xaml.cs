namespace CsGraphics
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using Microsoft.CodeAnalysis.CSharp.Scripting;
    using Microsoft.CodeAnalysis.Scripting;


    public partial class MainPage : ContentPage
    {
        private Scene scene;

        public Scene Scene { get; set; }

        private bool _isUpdating = false;

        public MainPage()
        {
            InitializeComponent();

            scene = new Scene();  // MyDrawable インスタンスを作成
            graphicsView.Drawable = scene;
            BindingContext = this;
            Scene = scene;  // Drawable に設定

            this.UpdateLoop();

        }

        /// <summary>
        /// 画面更新処理.
        /// </summary>
        private async void UpdateLoop()
        {
            if (this._isUpdating)
            {
                return;
            }

            this._isUpdating = true;
            TimeSpan interval = TimeSpan.FromSeconds(1.0 / Scene.FrameRate);

            while (this._isUpdating)
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

        /// <summary>
        /// 画面更新を止める処理.
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            this._isUpdating = false; // 更新ループを停止
        }

        // コマンドが入力された時の処理
        private async void OnCommandEntered(object sender, EventArgs e)
        {
            string command = InputField.Text?.Trim();
            if (string.IsNullOrEmpty(command))
                return ;

            // 出力領域にコマンドを追加
            AppendOutput($"> {command}");

            // コマンドに応じた結果を出力
            string result = string.Empty;
            result = await ProcessCommand(command) + "\n";

            AppendOutput(result);

            // 入力フィールドをクリア
            InputField.Text = string.Empty;
        }

        // 出力領域にテキストを追加
        private void AppendOutput(string text)
        {
            OutputArea.Text += text + Environment.NewLine;

            // スクロールを下に自動移動
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await OutputScrollView.ScrollToAsync(0, OutputArea.Height, true);
            });
        }

        // コマンド処理ロジック
        private async Task<string?> ProcessCommand(string command)
        {
            return command.ToLower() switch
            {
                _ when command.StartsWith("!") => await ExecuteCodeAsync(command.Substring(1)),
                "hello" => "Hello, user!",
                "time" => DateTime.Now.ToString(),
                "exit" => "Goodbye!",
                "translation" => TranslationTest(),
                "scale" => ScaleTest(),
                "rotationz" => RotationZTest(),
                "test" => Test(),
                "teapot" => AddTeapot(),
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
            Object.Object quadrangle = new Object.Object("rectangle", new double[,] { { 100, 300, 300, 100 }, { 100, 100, 400, 400 } });
            this.scene.AddObject(quadrangle);
            return quadrangle.ToString() + "\nDone!";
        }

        private string TranslationTest()
        {
            // 平行移動
            Math.Matrix trans = new (new double[] { 200, -100, 0 });
            this.Scene.Objects[0].Translation(trans); // 移動の適用

            return this.Scene.Objects[0].ToString() + "\nDone!";
        }

        private string ScaleTest()
        {
            /*
            //テスト用、加工前データ
            Math.Vector vec = new(2, 4);
            vec.Initialize(new double[,] { { 100, 300, 300, 100 }, { 100, 100, 400, 400 } }); // 四角形を描画
            //_myDrawable.AddPoints(vec, Colors.Aqua);

            //拡大縮小
            Math.Matrix scale = new(2, 1);
            //scale.Initialize(new double[] { 2, 0.5 }); //移動量
            vec.Scale(scale); // 移動の適用
            //_myDrawable.AddPoints(vec, Colors.Black);

            // 描画を更新
            graphicsView.Invalidate();  // GraphicsView を再描画*/

            return "\nDone!";
        }

        private string RotationZTest()
        {
            /*
            //テスト用、加工前データ
            Math.Vector vec = new(2, 4);
            vec.Initialize(new double[,] { { 100, 300, 300, 100 }, { 100, 100, 400, 400 } }); // 四角形を描画
            //_myDrawable.AddPoints(vec, Colors.Aqua);

            //拡大縮小
            Math.Matrix rotate = new(2, 1);
            vec.RotationZdeg(30); // 回転の適用(回転量(θ))
            //_myDrawable.AddPoints(vec, Colors.Black);

            // 描画を更新
            graphicsView.Invalidate();  // GraphicsView を再描画*/

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
