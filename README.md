# README

## 目的
ユーザー側で簡単にサービスの状況を「確認」及び「起動/停止」できるようにする

## 構成
- C#
- .Net framework4.7.2
- WPF
- Prism
- ReactiveProperty
- Extended.Wpf.Toolkit
- UI
	- Material Design In XAML Toolkit
	- Mahapps

## 表示項目
- サービス名
- サービスの状態

## 操作
- Start
	- サービスを起動します
- Stop
	- サービスを停止します
- ShowLog
	- Logファイルのフォルダを開きます

## 実装予定
- プロセスの強制終了

## 使い方
ProgramData内のConfig.xmlを表示したいサービスに書き換えて使う