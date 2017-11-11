using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;

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
            var openFolderDialog = new CommonOpenFileDialog();
            openFolderDialog.IsFolderPicker = true;
            openFolderDialog.EnsureReadOnly = true;
            openFolderDialog.AllowNonFileSystemItems = false;
            openFolderDialog.DefaultDirectory = Application.StartupPath;

            //ディレクトリのパスを取得
            string path;
            DirectoryInfo dir;
            var result = openFolderDialog.ShowDialog(); //ダイアログを表示し，ユーザ操作の結果を返す
            if (result == CommonFileDialogResult.Ok)
            {
                path = openFolderDialog.FileName; //選択されたフォルダのパスを取得
                dir = new DirectoryInfo(path);
                if (!dir.Exists)
                {
                    return; //ディレクトリが存在しない場合は何もしない
                }
            }
            else
            {
                return; //キャンセルしたりダイアログが閉じられた場合は何もしない
            }



            //
            // フォルダパスから、フォルダ内のjpgファイルパスを全て取得する
            //
            var FileInfo = dir.GetFiles("*.jpg");
            string[] ImageFilePath = new string[FileInfo.Length];

            for (int i = 0; i < FileInfo.Length; i++)
            {
                //Console.WriteLine(FileInfo[i] + " : " + FileInfo[i].FullName);
                ImageFilePath[i] = FileInfo[i].FullName;
            }

            //
            // ビットマップ配列を生成し、全ての画像を読み込む
            //
            m_InputImageArray = new Bitmap[ImageFilePath.Length];
            for (int i = 0; i < ImageFilePath.Length; i++)
            {
                m_InputImageArray[i] = new Bitmap(ImageFilePath[i]);
            }

            //
            // 画像に関するデータの変数を格納する
            //

            //
            // ピクチャーボックスに最初の画像を設定する
            // 表示画像の切り替え方法
            // 　pictureBox1.Image = [Bitmapの変数];
            //
            pictureBox1.Image = m_InputImageArray[0];

            m_Width = m_InputImageArray[m_Index].Width; //画像の横の長さ
            m_Height = m_InputImageArray[m_Index].Height; //画像の縦の長さ
            m_NumPixel = m_Width * m_Height;  //画像の総ピクセル数
        }


    //
    // NextImageボタンクリック時に呼ばれるメソッド
    //
    private void NextImageButton_Click(object sender, EventArgs e) {
            //
            // インデックスの変数を増加させ、表示する画像を切り替える
            // 全ての画像を表示し終えたら最初の画像に戻る
            //
            if (m_InputImageArray == null) return;
            m_Index++;
            if (m_InputImageArray.Length - 1 < m_Index) m_Index = 0;
            //
            // 表示画像の切り替え方法
            // 　pictureBox1.Image = [Bitmapの変数];
            //
            pictureBox1.Image = m_InputImageArray[m_Index];
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
