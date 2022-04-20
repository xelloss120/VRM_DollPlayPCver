# VRM_DollPlayPCver
VRMでお人形遊びするやつのPC版

## 見る所
私が書いた部分は以下にまとめてます。  
https://github.com/xelloss120/VRM_DollPlayPCver/tree/master/Assets/Script

## 使い方
https://docs.google.com/presentation/d/1WRol9mT7jh9rw1KqK9kF3JpmW1qVh9cYcNg5dYzCH0w/edit?usp=sharing

## 作った人
https://twitter.com/120byte

## 免責
本ソフトウェアの利用により発生した問題は、  
本ソフトウェア利用者の責任とし、  
本ソフトウェア作成者は一切の責任を負わないものとします。

## BOOTH
https://120byte.booth.pm/items/1654585

## 注意
使用しているアセットを除外しているため、cloneしただけではビルドできません。

## 使用しているアセット一覧
[Runtime Transform Handles](https://assetstore.unity.com/packages/tools/modeling/runtime-transform-handles-65363)たぶん3.5.0  
[Let's Tweet In Unity](https://assetstore.unity.com/packages/tools/integration/let-s-tweet-in-unity-536)たぶん1.1  
[Final IK](https://assetstore.unity.com/packages/tools/animation/final-ik-14290)2.1  
[PuppetMaster](https://assetstore.unity.com/packages/tools/physics/puppetmaster-48977)1.1  
[UnityStandaloneFileBrowser](https://github.com/gkngkc/UnityStandaloneFileBrowser)たぶん1.2  
[TriLib](https://assetstore.unity.com/packages/tools/modeling/trilib-model-loader-package-91777?locale=ja-JP)1.9.0b  
[UnityWindowsFileDrag&Drop](https://github.com/Bunny83/UnityWindowsFileDrag-Drop)???  
[VRMLoaderUI](https://github.com/m2wasabi/VRMLoaderUI)0.3  
[VRoidSDK](https://developer.vroid.com/)0.1.0  
※末尾はバージョン

## その他
アイコン画像や効果音も除外しています。

## 言い訳
元々公開しないつもりだったので中身が酷いです。  
2回ほど公開しないのか？的なことを海外ニキに聞かれたので、  
差分管理が辛くなってきたし、バージョン管理ついでに公開します。

## 色々抜けてるけどコレ本当にビルドできるの？
以下の手順は私自身がgitからcloneしたプロジェクトに対して、  
出来るだけ手軽にビルドできるまでをまとめています。  
  
■UniVRM  
まずUniVRMを入れます。  
https://github.com/vrm-c/UniVRM/releases  
  
バージョンは一旦最新で試してみましょう。  
今回は2020/09/03時点の最新であるv0.59.0で行きます。  
  
本来はVRoidSDKに同梱されているUniVRMを参照するのですが、  
VRoidSDKの入手は比較的困難なので、UniVRM単体で代用します。  
  
■Runtime Transform Handles  
最低限度の機能として欲しいので、これは避けようが無いです。  
https://assetstore.unity.com/packages/tools/modeling/runtime-transform-handles-65363  
  
バージョンは今見たら2.11になってました。  
私のローカル作業環境より新しくなってるようです。  
  
■FinalIK  
IK操作したいなら入れるしかありません。  
要らなければ関連個所をコメントアウトで行ける気がします。（未確認）  
https://assetstore.unity.com/packages/tools/animation/final-ik-14290  
今回は入れました。  
  
■PuppetMaster  
これが必要な人は少ないと思うので関連個所をコメントアウトします。  
LoadFileのusingとSetRagDollメソッドの中身をコメントアウトします。  
  
■TriLib  
これは元々おまけ機能的に入れていたのでコメントアウトします。  
LoadFileのusingとTriLibメソッドの中身をコメントアウトします。  
  
■Let's Tweet In Unity  
これも別に必須ということはないと思うので削除します。  
SendTwitter.csを削除します。  
  
■その他  
この辺は気にせず入れましょう。  
VRMLoaderUI  
UnityWindowsFileDrag&Drop  
UnityStandaloneFileBrowser  
  
■UnityWindowsFileDrag-Dropのエラー対策  
2020/09/03時点の最新版と私が使っているもので差分があるようです。  
LoadFileのOnEnableとOnDisableの変更が必要です。  
UnityWindowsFileDrag-DropのImageExampleが参考になります。  
  
■TMP Importer  
シーン(main)を開くとウィンドウが出てきます。  
ボタン2個ともクリックしときます。  
  
■まとめ  
これらの作業をすることでビルドできることを一応確認しました。  
削除した部分などは当然動きませんし、テストは不十分ですから不具合は色々あると思います。  
![build](https://user-images.githubusercontent.com/13127051/92105197-7275b380-ee1d-11ea-8af7-0c739e4094c6.jpg)  
© Unity Technologies Japan／UCL
