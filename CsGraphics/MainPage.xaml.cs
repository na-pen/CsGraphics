namespace CsGraphics
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnCounterClicked(object sender, EventArgs e)
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

            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }
    }

}
