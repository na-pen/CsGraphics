# CsGraphics

ここは、描画に関するプログラムのトップディレクトリです。

直下には以下のディレクトリまたはファイルを含みます。

- [Scene.cs](#Scene) (シーンの制御)

- Parser.cs (外部ファイルの読み込み)

- CalcScreenCoord.cs (オブジェクトの座標から画面の座標を算出する)

- DataExtensions.cs (クラスを便利にするための実装)

- Math (行列, 複素数, 虚数の実装)

- Object (オブジェクトの実装)



<a id="Scene"></a>

## Scene.cs

シーンはアプリケーションに1つ存在し、オブジェクトの保存や管理、操作を行います。
また、開発者がCsGraphicsにおいて唯一アクセス可能なクラスです。



#### 定数・変数

Scene は次のメンバ変数を持ちます。

| アクセス         | 型                   | 名前        | 内容               |
| ------------ | ------------------- | --------- | ---------------- |
| public       | int                 | FrameRate | 1秒間の画面の更新回数      |
| public const | double              | Gravity   | 重力加速度 = 9.80665  |
| internal     | List<Object.Object> | Objects   | シーンに含まれるオブジェクト一覧 |


