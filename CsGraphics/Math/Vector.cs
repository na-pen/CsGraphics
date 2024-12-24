namespace CsGraphics.Math
{
    internal class Vector
    {
        internal Matrix Data = new Matrix(4, 1);

        public float X
        {
            get => this.Data[0, 0];
            set => this.Data[0, 0] = value;
        }

        public float Y
        {
            get => this.Data[1, 0];
            set => this.Data[1, 0] = value;
        }

        public float Z
        {
            get => this.Data[2, 0];
            set => this.Data[2, 0] = value;
        }

        public float W
        {
            get => this.Data[3, 0];
            set => this.Data[3, 0] = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector"/> class.
        /// </summary>
        /// <param name="x">x成分.</param>
        /// <param name="y">y成分.</param>
        /// <param name="z">z成分.</param>
        public Vector(float x = 0, float y = 0, float z = 0)
        {
            this.Data[0, 0] = x;
            this.Data[1, 0] = y;
            this.Data[2, 0] = z;
            this.Data[3, 0] = 1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector"/> class.
        /// 配列からベクトル(同次座標系行列)を作成する.
        /// </summary>
        /// <param name="a">点A[x,y,z].</param>
        /// <param name="b">点B[x,y,z].</param>
        /// <returns>ベクトルAB.</returns>
        /// <exception cref="ArgumentException">各頂点は3次元である必要があります.</exception>
        public Vector(float[] a, float[] b)
        {
            if (a.Length != 3 || b.Length != 3)
            {
                throw new ArgumentException("各頂点は3次元である必要があります");
            }

            this.Data[0, 0] = b[0] - a[0];
            this.Data[1, 0] = b[1] - a[1];
            this.Data[2, 0] = b[2] - a[2];
            this.Data[3, 0] = 1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector"/> class.
        /// 配列からベクトル(同次座標系行列)を作成する.
        /// </summary>
        /// <param name="arr">点AB[[xa,xb],[ya,yb],[za,zb]].</param>
        /// <returns>ベクトルAB.</returns>
        /// <exception cref="ArgumentException">各頂点は3次元である必要があります.</exception>
        public Vector(float[,] arr)
        {
            if (arr.GetLength(0) != 3)
            {
                throw new ArgumentException("ベクトルは3次元である必要があります");
            }

            this.Data[0, 0] = arr[0, 1] - arr[0, 0];
            this.Data[1, 0] = arr[1, 1] - arr[1, 0];
            this.Data[2, 0] = arr[2, 1] - arr[2, 0];
            this.Data[3, 0] = 1;
        }

        public Vector(Vector vector)
        {
            this.X = vector.X;
            this.Y = vector.Y;
            this.Z = vector.Z;
            this.W = vector.W;
        }

        public static Vector operator +(Vector a, Vector b)
        {
            Vector result = new Vector();
            result.X = a.X + b.X;
            result.Y = a.Y + b.Y;
            result.Z = a.Z + b.Z;

            return result;
        }

        public static Vector operator -(Vector a, Vector b)
        {
            Vector result = new Vector();
            result.X = a.X - b.X;
            result.Y = a.Y - b.Y;
            result.Z = a.Z - b.Z;

            return result;
        }

        public static Vector operator *(float a, Vector b)
        {
            Vector result = new Vector();
            result.X = a * b.X;
            result.Y = a * b.Y;
            result.Z = a * b.Z;

            return result;
        }

        public static Vector operator *(Vector b, float a)
        {
            return a * b;
        }

        /// <summary>
        /// 2つのベクトル(同次座標系行列)の外積を計算する.
        /// 平面内の2つのベクトルから法線ベクトルを計算する.
        /// </summary>
        /// <param name="vectorA">a</param>
        /// <param name="vectorB">b</param>
        /// <returns>外積の結果.</returns>
        internal static Vector CrossProduct(Vector vectorA, Vector vectorB)
        {
            Vector result = new(4, 1);

            // 外積の計算 (a2 * b3 - a3 * b2, a3 * b1 - a1 * b3, a1 * b2 - a2 * b1)
            result.Data[0, 0] = (vectorA.Data[1, 0] * vectorB.Data[2, 0]) - (vectorA.Data[2, 0] * vectorB.Data[1, 0]);
            result.Data[1, 0] = (vectorA.Data[2, 0] * vectorB.Data[0, 0]) - (vectorA.Data[0, 0] * vectorB.Data[2, 0]);
            result.Data[2, 0] = (vectorA.Data[0, 0] * vectorB.Data[1, 0]) - (vectorA.Data[1, 0] * vectorB.Data[0, 0]);
            result.Data[3, 0] = 1;

            return result;
        }

        /// <summary>
        /// 2つのベクトル(同次座標系行列)の内積を計算する.
        /// </summary>
        /// <param name="vectorA">a</param>
        /// <param name="vectorB">b</param>
        /// <returns>内積の結果.</returns>
        internal static float DotProduct(Vector vectorA, Vector vectorB)
        {
            float result = (vectorA.X * vectorB.X) + (vectorA.Y * vectorB.Y) + (vectorA.Z * vectorB.Z) + (vectorA.W * vectorB.W);
            return result;
        }

        public static float Size(Vector vector)
        {
            float result = System.MathF.Sqrt(System.MathF.Pow(vector.X, 2) + System.MathF.Pow(vector.Y, 2) + System.MathF.Pow(vector.Z, 2));
            return result;
        }
    }
}
