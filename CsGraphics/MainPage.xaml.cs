namespace CsGraphics
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using Microsoft.CodeAnalysis.CSharp.Scripting;
    using Microsoft.CodeAnalysis.Scripting;


    public partial class MainPage : ContentPage
    {
        private MyDrawable _myDrawable;

        public MyDrawable MyDrawable { get; set; }

        public MainPage()
        {
            InitializeComponent();

            _myDrawable = new MyDrawable();  // MyDrawable インスタンスを作成
            graphicsView.Drawable = _myDrawable;
            BindingContext = this;
            MyDrawable = _myDrawable;  // Drawable に設定

        }

        // コマンドが入力された時の処理
        private async void OnCommandEntered(object sender, EventArgs e)
        {
            string command = InputField.Text?.Trim();
            if (string.IsNullOrEmpty(command))
                return;

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
                    Assembly.GetAssembly(typeof(System.Collections.Generic.List<>)) // System.Collections.Generic
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

        private string TranslationTest()
        {
            //テスト用、加工前データ
            Math.Vector vec = new(2, 4);
            vec.Initialize(new double[,] { { 100, 300, 300, 100 }, { 100, 100, 400, 400 } }); // 四角形を描画
            //_myDrawable.AddPoints(vec,Colors.Aqua);

            //平行移動
            Math.Matrix trans = new(2, 1);
            //trans.Initialize(new double[] { 200, -100 }); //移動量
            vec.Translation(trans); // 移動の適用
            //_myDrawable.AddPoints(vec, Colors.Red);

            // 描画を更新
            graphicsView.Invalidate();  // GraphicsView を再描画

            return vec.ToString()+"\nDone!";
        }

        private string ScaleTest()
        {
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
            graphicsView.Invalidate();  // GraphicsView を再描画

            return vec.ToString() + "\nDone!";
        }

        private string RotationZTest()
        {
            //テスト用、加工前データ
            Math.Vector vec = new(2, 4);
            vec.Initialize(new double[,] { { 100, 300, 300, 100 }, { 100, 100, 400, 400 } }); // 四角形を描画
            //_myDrawable.AddPoints(vec, Colors.Aqua);

            //拡大縮小
            Math.Matrix rotate = new(2, 1);
            vec.RotationZdeg(30); // 回転の適用(回転量(θ))
            //_myDrawable.AddPoints(vec, Colors.Black);

            // 描画を更新
            graphicsView.Invalidate();  // GraphicsView を再描画

            return vec.ToString() + "\nDone!";
        }

        private string Test()
        {
            //Vector vec = files.VerticesFromObj("E:/Projects/CsGraphics/CsGraphics/teapot.obj");
            Math.Vector vec = files.VerticesFromObj("C:/Users/mail/Documents/CsGraphics/CsGraphics/teapot.obj");

            Math.Matrix trans = new(3, 1);
            //trans.Initialize(new double[] { 20, 10 ,0}); //移動量
            vec.Translation(trans);

            //拡大縮小
            Math.Matrix scale = new(3, 1);
            //scale.Initialize(new double[] { 50, 50,50 }); //移動量
            vec.Scale(scale); // 移動の適用

            //_myDrawable.AddPoints(vec, Colors.Black);
            graphicsView.Invalidate();
            return "Done!";
        }
    }

    }
