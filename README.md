# CsGraphics

これは、C# .net9 maui 環境上で 3Dモデルの表示および操作することを目的に開発している個人的な学習用プログラムです。



## 特徴

CsGraphics は、標準ライブラリ以外を使用していません。

#### サポートされているオブジェクト形式

- オブジェクトファイル (.obj)

#### できること

- 3次元空間上にオブジェクトの頂点と面、テクスチャを表示する

- 3次元空間上でオブジェクト単位の移動/拡大縮小/回転/可視状態の変更

- 頂点ごとの色付け
  
  

## 環境

#### 開発環境

- C# net9.0 maui

#### 動作環境 (確認済み)

- Windows net9.0

- Android net9.0



## 内部のデータ管理について

データは次のような構造で保存、管理されています。
各レイヤーの詳細は、それぞれのReadmeでご確認ください。

- シーン
  
  - オブジェクト
    
    - 頂点


