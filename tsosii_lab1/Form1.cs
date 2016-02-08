using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using ZedGraph;
using System.Threading;

namespace tsosii_lab1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //pixelFormat_label1.Text = "";
            //pixelFormat_label2.Text = "";
            //label_objectCount.Text = "";
        }

        string fileName;

        Bitmap bitmap = null;
        Bitmap bitmapRes = null;

        private void openButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png, *.bmp) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png; *.bmp";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                //label_objectCount.Text = "";

                if (bitmap != null)
                    bitmap.Dispose();

                if (bitmapRes != null)
                {
                    bitmapRes.Dispose();
                    bitmapRes = null;
                    pictureBox_result.Image = null;
                    //pictureBox_intermediate.Image = null;
                    //pixelFormat_label2.Text = "";
                }

                Bitmap origBitmap = new Bitmap(dialog.FileName);
                fileName = System.IO.Path.GetFileNameWithoutExtension(dialog.FileName);
                //pixelFormat_label1.Text = origBitmap.PixelFormat.ToString();

                if (origBitmap.PixelFormat != PixelFormat.Format24bppRgb && origBitmap.PixelFormat != PixelFormat.Format32bppArgb
                    && origBitmap.PixelFormat != PixelFormat.Format32bppRgb && origBitmap.PixelFormat != PixelFormat.Canonical)
                {
                    Bitmap cloneBitmap = new Bitmap(origBitmap.Width, origBitmap.Height, PixelFormat.Format24bppRgb);
                    using (Graphics gr = Graphics.FromImage(cloneBitmap))
                    {
                        gr.DrawImage(origBitmap, new Rectangle(0, 0, cloneBitmap.Width, cloneBitmap.Height));
                    }
                    origBitmap.Dispose();
                    origBitmap = cloneBitmap;
                }

                pictureBox_source.Image = origBitmap;
                bitmap = origBitmap;
            }
        }


        private void executeButton_Click(object sender, EventArgs e)
        {
            if (bitmap == null)
                return;

            executeButton.Enabled = false;

            bitmapRes = null;

            byte treshold;
            bool isSuccess = byte.TryParse(textBox_threshold.Text, out treshold);
            if (isSuccess == false)
            {
                MessageBox.Show("Порог - число от 0 до 255");
                return;
            }

            int medianKernelSize = 3;
            isSuccess = int.TryParse(textBox_median.Text, out medianKernelSize);
            if (isSuccess == false || medianKernelSize%2 == 0 || medianKernelSize == 1)
            {
                MessageBox.Show("Размер ядра медианного фильтра - нечётное число большее либо равное 3.");
                return;
            }

            int clustNum = 0;
            isSuccess = int.TryParse(textBox_clustNumber.Text, out clustNum);
            if (isSuccess == false || clustNum <= 0)
            {
                MessageBox.Show("Количество кластеров должно быть больше нуля.");
                return;
            }

            bitmapRes = bitmap;

            if(checkBox_medianFilter.Checked)
                bitmapRes = ImageProcessing.MedianFilter_unsafe(bitmapRes, medianKernelSize);
            bitmapRes = ImageProcessing.GrayscaleAndBinarization_unsafe(bitmapRes, treshold);
           
            //pictureBox_intermediate.Image = bitmapRes;

            СlusterАnalysis clustInfo = new СlusterАnalysis(bitmapRes, clustNum);
            bitmapRes = clustInfo.GetClusteredBitmap();

            //label_objectCount.Text = "Количество значимых объектов: " + clustInfo.GetGoodObjectsCount();

            pictureBox_result.Image = bitmapRes;
            //pixelFormat_label2.Text = bitmapRes.PixelFormat.ToString();

            executeButton.Enabled = true;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (bitmapRes == null)
                return;

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Png Image (.png)|*.png|Gif Image (.gif)|*.gif|JPEG Image (.jpeg)|*.jpeg|Bitmap Image (.bmp)|*.bmp|Tiff Image (.tiff)|*.tiff";
            dialog.FileName = fileName;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    switch (dialog.FilterIndex)
                    {
                        case 1:      // png
                            bitmapRes.Save(dialog.FileName, ImageFormat.Png);
                            break;

                        case 2:     // gif
                            bitmapRes.Save(dialog.FileName, ImageFormat.Gif);
                            break;

                        case 3:     // jpeg
                            bitmapRes.Save(dialog.FileName, ImageFormat.Jpeg);
                            break;

                        case 4:    // bmp
                            bitmapRes.Save(dialog.FileName, ImageFormat.Bmp);
                            break;

                        case 5:    // tiff
                            bitmapRes.Save(dialog.FileName, ImageFormat.Tiff);
                            break;
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка сохранения", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                                
            }
        }

        private void pictureBox_source_DoubleClick(object sender, EventArgs e)
        {
            frmPreview f = new frmPreview();
            f.PreviewImage = pictureBox_source.Image;
            f.Show();
        }

        private void pictureBox_result_DoubleClick(object sender, EventArgs e)
        {
            frmPreview f = new frmPreview();
            f.PreviewImage = pictureBox_result.Image;
            f.Show();
        }

        //private void pictureBox_intermediate_DoubleClick(object sender, EventArgs e)
        //{
        //    frmPreview f = new frmPreview();
        //    f.PreviewImage = pictureBox_intermediate.Image;
        //    f.Show();
        //}

        private void checkBox_medianFilter_CheckedChanged(object sender, EventArgs e)
        {
            //label2.Enabled = checkBox_medianFilter.Checked;
            textBox_median.Enabled = checkBox_medianFilter.Checked;            
        }


        //private void groupBox3_Enter(object sender, EventArgs e)
        //{

        //}

        //private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        //{

        //}

        
    }
}
