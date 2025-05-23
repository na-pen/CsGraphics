﻿namespace Main
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

        private bool isPointerPressing = false;
        private bool isPointerLongPressing = false;

        private PointF PointerPressed = new PointF(0, 0);

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

            initCam();
            this.UpdateLoop();
        }

        private async void initCam()
        {
            await Task.Delay(1500);
            // レイアウトが変更された後、すべての要素が描画されたタイミングで実行される処理
            if (graphicsView.Width != -1)
            {
                //Scene.objectManager.Get<CsGraphics.Object.Camera.Camera>("MainCam").SetTranslation((int)graphicsView.Width / 2, (int)graphicsView.Height / 5, 0);
                Scene.objectManager.Get<CsGraphics.Object.Camera.Camera>("MainCam").Height = (int)graphicsView.Height;
                Scene.objectManager.Get<CsGraphics.Object.Camera.Camera>("MainCam").Width = (int)graphicsView.Width;
            }
            else
            {
                initCam();
            }

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
                _ when command.StartsWith("translation") => this.TranslationTest(int.Parse(args[0]), float.Parse(args[1]), float.Parse(args[2]), float.Parse(args[3])),
                _ when command.StartsWith("scale") => this.ScaleTest(int.Parse(args[0]), float.Parse(args[1]), float.Parse(args[2]), float.Parse(args[3])),
                _ when command.StartsWith("rotation") => this.RotationTest(int.Parse(args[0]), float.Parse(args[1]), float.Parse(args[2]), float.Parse(args[3])),
                _ when command.StartsWith("object") => "ID : " + this.Scene.AddObjectFromObj(args[0].Replace("\"", string.Empty), args[1].Replace("\"", string.Empty)).ToString(),
                _ => "Unknown command."
            };
        }

        private string TranslationTest(int id, float x, float y, float z)
        {
            // 平行移動
            this.Scene.TranslationObject(id, x, y, z);

            return "Done!";
        }

        private string ScaleTest(int id, float x, float y, float z)
        {
            // 拡大
            this.Scene.ScaleObject(id, x, y, z);

            return "Done!";
        }

        private string RotationTest(int id, float x, float y, float z)
        {
            // 回転
            this.Scene.RotationObject(id, x, y, z);

            return "Done!";
        }

        private async void PointerGestureRecognizer_PointerPressed(object sender, PointerEventArgs e)
        {
            this.PointerPressed = (PointF)e.GetPosition(this.graphicsView);
            this.isPointerPressing = true;
            await WaitForLongPress();
        }

        private async Task WaitForLongPress()
        {
            // 0.5秒(500ミリ秒)待機
            await Task.Delay(200);

            // マウスがまだ押されているか確認
            if (this.isPointerPressing)
            {
                this.isPointerLongPressing = true;
            }
        }

        private void SwitchingProjection(object sender, EventArgs e)
        {
            Scene.IsPerspectiveProjection = !Scene.IsPerspectiveProjection;
            Scene.IsUpdated = true;
        }

        private void PointerGestureRecognizer_PointerReleased(object sender, PointerEventArgs e)
        {
            this.isPointerPressing = false;
            if (this.isPointerLongPressing)
            {
                this.isPointerLongPressing = false;
                var t = (PointF)e.GetPosition(this.graphicsView) - this.PointerPressed;
                Scene.objectManager.Get<CsGraphics.Object.Camera.Camera>("MainCam").SetTranslation(t.Width / 20f, -1 * t.Height / 20f, 0);
                Scene.IsUpdated = true;
                //float x =  * 180) * Math.PI / 180f;
                //Scene.SetRotationViewCam(t.Height / graphicsView.Height * 360, t.Width / graphicsView.Width * 360, 0);
            }
        }

        private void EnlargementCam(object sender, EventArgs e)
        {
            Scene.objectManager.Get<CsGraphics.Object.Camera.Camera>("MainCam").SetTranslation(0, 0, -10);
            Scene.IsUpdated = true;
            //Scene.ScaleParallelProjection *= 1.25f;
        }

        private void ReductionCam(object sender, EventArgs e)
        {
            Scene.objectManager.Get<CsGraphics.Object.Camera.Camera>("MainCam").SetTranslation(0, 0, 10);
            Scene.IsUpdated = true;
            //Scene.ScaleParallelProjection *= 0.8f;
        }
    }
}
