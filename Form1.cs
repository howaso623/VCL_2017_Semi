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

namespace Semi2017
{
    public partial class Form1 : Form
    {
        //
        // メンバ
        //
        private Bitmap m_AverageImage;
        private Bitmap m_VarianceImage;
        private double[,] m_InputImageAverageArray;
        private double[] m_AverageImageAverage;
        private double[] m_VarianceImageAverage;
        bool[] m_ExcludeImageFlagArray;
        int m_Num_UnExcludeImage;
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
        public Form1()
        {
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
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
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

            m_Num_UnExcludeImage = m_InputImageArray.Length;
            m_ExcludeImageFlagArray = new bool[m_InputImageArray.Length];

        }


        //
        // NextImageボタンクリック時に呼ばれるメソッド
        //
        private void NextImageButton_Click(object sender, EventArgs e)
        {
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
        private void ProcessButton_Click(object sender, EventArgs e)
        {
            //
            // 画像処理実行コード
            //
            if (m_InputImageArray == null) return;

            for (int i = 0; i < 3; i++)
            {
                CalcAverage();
                pictureBox1.Image = m_AverageImage;
                CalcVariance();
                pictureBox1.Image = m_VarianceImage;
                CalcPixelAverage();
                ExclideFrame();
            }
            CalcAverage();
            pictureBox1.Image = m_AverageImage;

            Save();
        }

        private void Average_Click(object sender, EventArgs e)
        {
            CalcAverage();
            pictureBox1.Image = m_AverageImage;
        }

        private void Variance_Click(object sender, EventArgs e)
        {
            CalcVariance();
            pictureBox1.Image = m_VarianceImage;
        }


        private void CalcAverage()
        {
            m_AverageImage = new Bitmap(m_Width, m_Height); //インスタンス化
            var colorSum = new double[m_Width, m_Height, 3]; //色の値の合計値を保存するためのバッファ

            //すべてのピクセルについて
            for (int x = 0; x < m_Width; x++)
            {
                for (int y = 0; y < m_Height; y++)
                {
                    //RGBをそれぞれ枚数分合計
                    for (int i = 0; i < m_InputImageArray.Length; i++)
                    {
                        #region Zemi Day3
                        if (m_ExcludeImageFlagArray[i]) continue;
                        #endregion
                        var pixelColor = m_InputImageArray[i].GetPixel(x, y);
                        colorSum[x, y, 0] += pixelColor.R;
                        colorSum[x, y, 1] += pixelColor.G;
                        colorSum[x, y, 2] += pixelColor.B;
                    }
                    //合計値を枚数で割り（平均），その値をm_AverageImageの画素の色にする
                    /*
                    var red = (int)(colorSum[x, y, 0] / m_InputImageArray.Length);
                    var green = (int)(colorSum[x, y, 1] / m_InputImageArray.Length);
                    var blue = (int)(colorSum[x, y, 2] / m_InputImageArray.Length);
                    */
                    #region Zemi Day3
                    var red = (int)(colorSum[x, y, 0] / m_Num_UnExcludeImage);
                    var green = (int)(colorSum[x, y, 1] / m_Num_UnExcludeImage);
                    var blue = (int)(colorSum[x, y, 2] / m_Num_UnExcludeImage);
                    #endregion

                    m_AverageImage.SetPixel(x, y, Color.FromArgb(red, green, blue));
                }
            }
        }


        private void CalcVariance()
        {
            m_VarianceImage = new Bitmap(m_Width, m_Height);
            var colorVariance = new double[m_Width, m_Height, 3];
            var maxVariance = 0.0d;     //分散の最大値

            //すべてのピクセルについて
            for (int x = 0; x < m_Width; x++)
            {
                for (int y = 0; y < m_Height; y++)
                {
                    double redSum, greenSum, blueSum;                //平均との差の二乗の合計値を保持する一時変数
                    redSum = greenSum = blueSum = 0;                 //↑を0で初期化
                    var avePixel = m_AverageImage.GetPixel(x, y);      //平均の画素

                    //RGBをそれぞれ枚数分合計
                    for (int i = 0; i < m_InputImageArray.Length; i++)
                    {
                        if (m_ExcludeImageFlagArray[i]) continue;
                        var pixelColor = m_InputImageArray[i].GetPixel(x, y);
                        redSum += (avePixel.R - pixelColor.R) * (avePixel.R - pixelColor.R);
                        greenSum += (avePixel.G - pixelColor.G) * (avePixel.G - pixelColor.G);
                        blueSum += (avePixel.B - pixelColor.B) * (avePixel.B - pixelColor.B);
                    }

                    //平均との差の二乗を枚数で割る(平均) = 分散
                    /*
                    colorVariance[x, y, 0] = redSum / m_InputImageArray.Length;
                    colorVariance[x, y, 1] = greenSum / m_InputImageArray.Length;
                    colorVariance[x, y, 2] = blueSum / m_InputImageArray.Length;
                    */

                    colorVariance[x, y, 0] = redSum / m_Num_UnExcludeImage;
                    colorVariance[x, y, 1] = greenSum / m_Num_UnExcludeImage;
                    colorVariance[x, y, 2] = blueSum / m_Num_UnExcludeImage;
                    // 分散の最大値を更新
                    maxVariance = Math.Max(maxVariance,
                                    Math.Max(colorVariance[x, y, 0],
                                        Math.Max(colorVariance[x, y, 1],
                                            colorVariance[x, y, 2])));
                }
            }

            for (int y = 0; y < m_Height; y++)
            {
                for (int x = 0; x < m_Width; x++)
                {
                    //値を0~255に正規化して画像を生成
                    var red = (int)(colorVariance[x, y, 0] * 255 / maxVariance);
                    var green = (int)(colorVariance[x, y, 1] * 255 / maxVariance);
                    var blue = (int)(colorVariance[x, y, 2] * 255 / maxVariance);
                    m_VarianceImage.SetPixel(x, y, Color.FromArgb(red, green, blue));
                }
            }
        }
        private void CalcPixelAverage()
        {
            m_InputImageAverageArray = new double[m_InputImageArray.Length, 3]; //m_ImputImageArrayのそれぞれの画像の色の平均値を保存
            m_AverageImageAverage = new double[3];  //m_AverageImageの画像の色の平均値を保存
            m_VarianceImageAverage = new double[3]; //m_VarianceImageの画像の色の平均値を保存

            var colorSumArr = new double[m_InputImageArray.Length, 3]; //m_ImputImageArrayの色の値の合計値を保存するためのバッファ
            var colorSumAve = new double[3];    //m_AverageImageの色の値の合計値を保存するためのバッファ
            var colorSumVar = new double[3];    //m_VarianceImageの色の値の合計値を保存するためのバッファ

            /** m_ImputImageArrayのそれぞれの画像の色の平均値を求める **/
            for (int i = 0; i < m_InputImageArray.Length; i++)
            {
                for (int x = 0; x < m_Width; x++)
                {
                    for (int y = 0; y < m_Height; y++)
                    {
                        var pixelColor = m_InputImageArray[i].GetPixel(x, y);
                        colorSumArr[i, 0] += pixelColor.R;
                        colorSumArr[i, 1] += pixelColor.G;
                        colorSumArr[i, 2] += pixelColor.B;
                    }
                }
                m_InputImageAverageArray[i, 0] = colorSumArr[i, 0] / m_NumPixel;
                m_InputImageAverageArray[i, 1] = colorSumArr[i, 1] / m_NumPixel;
                m_InputImageAverageArray[i, 2] = colorSumArr[i, 2] / m_NumPixel;

                if (i == 0 || i == 8 || i == 14)
                {
                    Console.WriteLine("[{0}, 0] = " + m_InputImageAverageArray[i, 0], i);
                    Console.WriteLine("[{0}, 1] = " + m_InputImageAverageArray[i, 1], i);
                    Console.WriteLine("[{0}, 2] = " + m_InputImageAverageArray[i, 2], i);
                }
            }

            /** m_AverageImageとm_VarianceImageのそれぞれの画像の色の平均値を求める **/
            for (int x = 0; x < m_Width; x++)
            {
                for (int y = 0; y < m_Height; y++)
                {
                    var pixelColorAve = m_AverageImage.GetPixel(x, y);
                    var pixelColorVar = m_VarianceImage.GetPixel(x, y);

                    colorSumAve[0] += pixelColorAve.R;
                    colorSumAve[1] += pixelColorAve.G;
                    colorSumAve[2] += pixelColorAve.B;

                    colorSumVar[0] += pixelColorVar.R;
                    colorSumVar[1] += pixelColorVar.G;
                    colorSumVar[2] += pixelColorVar.B;
                }
            }
            m_AverageImageAverage[0] = colorSumAve[0] / m_NumPixel;
            m_AverageImageAverage[1] = colorSumAve[1] / m_NumPixel;
            m_AverageImageAverage[2] = colorSumAve[2] / m_NumPixel;

            m_VarianceImageAverage[0] = colorSumVar[0] / m_NumPixel;
            m_VarianceImageAverage[1] = colorSumVar[1] / m_NumPixel;
            m_VarianceImageAverage[2] = colorSumVar[2] / m_NumPixel;

            Console.WriteLine("[0] = " + m_AverageImageAverage[0]);
            Console.WriteLine("[1] = " + m_AverageImageAverage[1]);
            Console.WriteLine("[2] = " + m_AverageImageAverage[2]);

            Console.WriteLine("[0] = " + m_VarianceImageAverage[0]);
            Console.WriteLine("[1] = " + m_VarianceImageAverage[1]);
            Console.WriteLine("[2] = " + m_VarianceImageAverage[2]);
        }

        private void ExclideFrame()
        {
            #region Zemi Day2
            //
            // 標準偏差の計算用
            //
            double[] standardDeviation = new double[3];
            double[] substructAverage = new double[3];

            // 標準偏差を計算
            standardDeviation[0] = Math.Sqrt(m_VarianceImageAverage[0]);
            standardDeviation[1] = Math.Sqrt(m_VarianceImageAverage[1]);
            standardDeviation[2] = Math.Sqrt(m_VarianceImageAverage[2]);

            //分散があまりに小さいときはこっち
            /*
            standardDeviation[0] = m_VarianceImageAverage[0];
            standardDeviation[1] = m_VarianceImageAverage[1];
            standardDeviation[2] = m_VarianceImageAverage[2];
            */
            #region Zemi Day3
            m_Num_UnExcludeImage = 0;
            #endregion

            //
            // 除外判定
            //
            for (int i = 0; i < m_InputImageArray.Length; i++)
            {
                //
                // 平均画像の平均画素と現在処理している画像の平均画素の差の絶対値を求める
                //
                substructAverage[0] = Math.Abs(m_AverageImageAverage[0] - m_InputImageAverageArray[i, 0]);
                substructAverage[1] = Math.Abs(m_AverageImageAverage[1] - m_InputImageAverageArray[i, 1]);
                substructAverage[2] = Math.Abs(m_AverageImageAverage[2] - m_InputImageAverageArray[i, 2]);

                //
                // 条件判定
                //
                if (substructAverage[0] < standardDeviation[0] && substructAverage[1] < standardDeviation[1] && substructAverage[2] < standardDeviation[2])
                {
                    Console.WriteLine(i + "弾かない");
                    #region Zemi Day3
                    m_Num_UnExcludeImage++;
                    #endregion
                }
                else
                {
                    Console.WriteLine(i + "弾く");

                    #region Zemi Day3
                    m_ExcludeImageFlagArray[i] = true;
                    #endregion
                }
            }
            #endregion
        }

        private void Save()
        {
            for (int i = 0; i < m_InputImageArray.Length; i++)
            {
                if (!m_ExcludeImageFlagArray[i]) m_InputImageArray[i].Save(i + ".bmp");
            }
            m_AverageImage.Save("Average.bmp");
        }
    }
}
