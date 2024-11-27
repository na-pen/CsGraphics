namespace CsGraphics
{
    using System;
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

        /*
        private void OnCounterClicked()
        {
            // 2x2の行列を作成
            Matrix matrixA = new Matrix(2, 6);
            Matrix matrixB = new Matrix(2, 6);
            Matrix matrixC = new Matrix(6, 6);

            // 行列Aを初期化
            matrixA.Initialize((i, j) => i + j + 1); // 例: [[1, 2], [2, 3]]
            this.TestLabel.Text += "Matrix A:\n";
            this.TestLabel.Text += matrixA.ToString();

            // 行列Bを初期化
            matrixB.Initialize((i, j) => (i + 1) * (j + 1)); // 例: [[1, 2], [2, 4]]
            this.TestLabel.Text += "Matrix B:\n";
            this.TestLabel.Text += matrixB.ToString();

            // 行列Cを初期化
            matrixC.Initialize((i, j) => (i + 1) * (j + 1)); // 例: [[1, 2], [2, 4]]
            this.TestLabel.Text += "Matrix C:\n";
            this.TestLabel.Text += matrixC.ToString();

            // 行列Aと行列Bを加算（演算子オーバーロードを使用）
            Matrix result = matrixA + matrixB;

            // 結果を表示
            this.TestLabel.Text += "Matrix A + Matrix B:\n";
            this.TestLabel.Text += result.ToString();

            // 行列Aと行列Bを乗算（演算子オーバーロードを使用）
            Matrix result2 = matrixA * matrixC;

            // 結果を表示
            this.TestLabel.Text += "Matrix A * Matrix C:\n";
            this.TestLabel.Text += result2.ToString();


            // 行列Aと行列Bを減算（演算子オーバーロードを使用）
            Matrix result3 = matrixA - matrixB;

            // 結果を表示
            this.TestLabel.Text += "Matrix A - Matrix B:\n";
            this.TestLabel.Text += result3.ToString();

            // 行列Aと15を加算（演算子オーバーロードを使用）
            Matrix result4 = matrixA + 15;

            // 結果を表示
            this.TestLabel.Text += "Matrix A + 15:\n";
            this.TestLabel.Text += result4.ToString();


            // 行列Aと5を減算（演算子オーバーロードを使用）
            Matrix result5 = matrixA - 5;

            // 結果を表示
            this.TestLabel.Text += "Matrix A - 5:\n";
            this.TestLabel.Text += result5.ToString();

            count++;}*/

        private string TranslationTest()
        {
            //テスト用、加工前データ
            Vector vec = new(2, 4);
            vec.Initialize(new double[,] { { 100, 300, 300, 100 }, { 100, 100, 400, 400 } }); // 四角形を描画
            _myDrawable.AddPoints(vec,Colors.Aqua);

            //平行移動
            Matrix trans = new(2, 1);
            trans.Initialize(new double[] { 200, -100 }); //移動量
            vec.Translation(trans); // 移動の適用
            _myDrawable.AddPoints(vec, Colors.Red);

            // 描画を更新
            graphicsView.Invalidate();  // GraphicsView を再描画

            return vec.ToString()+"\nDone!";
        }

        private string ScaleTest()
        {
            //テスト用、加工前データ
            Vector vec = new(2, 4);
            vec.Initialize(new double[,] { { 100, 300, 300, 100 }, { 100, 100, 400, 400 } }); // 四角形を描画
            _myDrawable.AddPoints(vec, Colors.Aqua);

            //拡大縮小
            Matrix scale = new(2, 1);
            scale.Initialize(new double[] { 2, 0.5 }); //移動量
            vec.Scale(scale); // 移動の適用
            _myDrawable.AddPoints(vec, Colors.Black);

            // 描画を更新
            graphicsView.Invalidate();  // GraphicsView を再描画

            return vec.ToString() + "\nDone!";
        }

        private string RotationZTest()
        {
            //テスト用、加工前データ
            Vector vec = new(2, 4);
            vec.Initialize(new double[,] { { 100, 300, 300, 100 }, { 100, 100, 400, 400 } }); // 四角形を描画
            _myDrawable.AddPoints(vec, Colors.Aqua);

            //拡大縮小
            Matrix rotate = new(2, 1);
            vec.RotationZdeg(30); // 回転の適用(回転量(θ))
            _myDrawable.AddPoints(vec, Colors.Black);

            // 描画を更新
            graphicsView.Invalidate();  // GraphicsView を再描画

            return vec.ToString() + "\nDone!";
        }
    }

    }
