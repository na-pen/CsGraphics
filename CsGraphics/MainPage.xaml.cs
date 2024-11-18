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
            Matrix matrixA = new Matrix(2, 2);
            Matrix matrixB = new Matrix(2, 2);

            // 行列Aを初期化
            matrixA.Initialize((i, j) => i + j + 1); // 例: [[1, 2], [2, 3]]
            Console.WriteLine("Matrix A:");
            matrixA.Print();

            // 行列Bを初期化
            matrixB.Initialize((i, j) => (i + 1) * (j + 1)); // 例: [[1, 2], [2, 4]]
            Console.WriteLine("Matrix B:");
            matrixB.Print();

            // 行列Aと行列Bを加算（演算子オーバーロードを使用）
            Matrix result = matrixA + matrixB;

            // 結果を表示
            Console.WriteLine("Matrix A + Matrix B:");
            result.Print();

            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }
    }

}
