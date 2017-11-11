using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Semi2017 {
  public partial class Form1 : Form {
    //
    // メンバ
    //

    //
    // フィールド
    //

    //
    // 画像の切り替え表示用インデックス
    //
    private int m_Index;

    //
    // 読み込んだ画像配列
    //
    private Bitmap[] m_InputImageArray;

    //
    // 画像のサイズ
    //  読み込んだ画像のサイズは全て同じものとする
    //
    private int m_Width, m_Height;

    //
    // 画像の画素数
    //
    private int m_NumPixel;


    //
    // コンストラクタ
    //
    public Form1() {
      InitializeComponent();

      m_Index = 0;

      m_InputImageArray = null;

      m_Width = m_Height = 0;

      m_NumPixel = 0;
    }


    //
    // メソッド
    //

    //
    // Openボタンクリック時に呼ばれるメソッド
    //
    private void openToolStripMenuItem_Click(object sender, EventArgs e) {
      //
      // ダイアログを開き、指定されたフォルダパスを取得する
      //

      //
      // フォルダパスから、フォルダ内のjpgファイルパスを全て取得する
      //

      //
      // ビットマップ配列を生成し、全ての画像を読み込む
      //

      //
      // 画像に関するデータの変数を格納する
      //

      //
      // ピクチャーボックスに最初の画像を設定する
      // 表示画像の切り替え方法
      // 　pictureBox1.Image = [Bitmapの変数];
      //
    }


    //
    // NextImageボタンクリック時に呼ばれるメソッド
    //
    private void NextImageButton_Click(object sender, EventArgs e) {
      //
      // インデックスの変数を増加させ、表示する画像を切り替える
      // 全ての画像を表示し終えたら最初の画像に戻る
      //

      //
      // 表示画像の切り替え方法
      // 　pictureBox1.Image = [Bitmapの変数];
      //
    }

    //
    // Processボタンクリック時に呼ばれるメソッド
    //
    private void ProcessButton_Click(object sender, EventArgs e) {
      //
      // 画像処理実行コード
      //
    }
  }
}
