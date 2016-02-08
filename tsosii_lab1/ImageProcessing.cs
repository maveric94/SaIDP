using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace tsosii_lab1
{
    static class ImageProcessing
    {

        // Соляризация
        unsafe static public Bitmap Solarisation(Bitmap BitmapSrc, bool dynamicMatch, double k)
        {
            Bitmap BitmapRes = new Bitmap(BitmapSrc.Width, BitmapSrc.Height, BitmapSrc.PixelFormat);

            int depth = Bitmap.GetPixelFormatSize(BitmapSrc.PixelFormat);
            Byte* pBaseSrc, pBaseRes;

            UnsafeBitmap src = new UnsafeBitmap(BitmapSrc);
            UnsafeBitmap res = new UnsafeBitmap(BitmapRes);

            pBaseSrc = src.LockBitmap();
            pBaseRes = res.LockBitmap();

            int width = BitmapSrc.Width;
            int height = BitmapSrc.Height;

            byte R_max = 0, G_max = 0, B_max = 0;
            byte r, g, b, a;
            double R_k, G_k, B_k;
            int i;

            if (depth == 32)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        i = ((y * width) + x) * 4;
                        r = pBaseSrc[i];
                        g = pBaseSrc[i + 1];
                        b = pBaseSrc[i + 2];

                        if (R_max < r) R_max = r;
                        if (G_max < r) G_max = g;
                        if (B_max < r) B_max = b;

                        if (R_max == 255 && G_max == 255 && B_max == 255)
                            goto loop_End;
                    }
                }
            loop_End:

                if (dynamicMatch == true)
                {
                    R_k = 4.0 / R_max;
                    G_k = 4.0 / G_max;
                    B_k = 4.0 / B_max;
                }
                else
                {
                    R_k = k;
                    G_k = k;
                    B_k = k;
                }

                double R, G, B;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        i = ((y * width) + x) * 4;
                        r = pBaseSrc[i];
                        g = pBaseSrc[i + 1];
                        b = pBaseSrc[i + 2];
                        a = pBaseSrc[i + 3];

                        R = R_k * r * (R_max - r);
                        G = G_k * g * (G_max - g);
                        B = B_k * b * (B_max - b);


                        if (R < 0) R = 0;
                        else if (R > 255) R = 255;

                        if (G < 0) G = 0;
                        else if (G > 255) G = 255;

                        if (B < 0) B = 0;
                        else if (B > 255) B = 255;

                        pBaseRes[i] = (byte)R;
                        pBaseRes[i + 1] = (byte)G;
                        pBaseRes[i + 2] = (byte)B;
                        pBaseRes[i + 3] = a;
                    }
                }

            }
            else if (depth == 24)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        i = ((y * width) + x) * 3;
                        r = pBaseSrc[i];
                        g = pBaseSrc[i + 1];
                        b = pBaseSrc[i + 2];

                        if (R_max < r) R_max = r;
                        if (G_max < r) G_max = g;
                        if (B_max < r) B_max = b;

                        if (R_max == 255 && G_max == 255 && B_max == 255)
                            goto loop_End;
                    }
                }
            loop_End:

                if (dynamicMatch == true)
                {
                    R_k = 4.0 / R_max;
                    G_k = 4.0 / G_max;
                    B_k = 4.0 / B_max;
                }
                else
                {
                    R_k = k;
                    G_k = k;
                    B_k = k;
                }

                double R, G, B;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        i = ((y * width) + x) * 3;
                        r = pBaseSrc[i];
                        g = pBaseSrc[i + 1];
                        b = pBaseSrc[i + 2];

                        R = R_k * r * (R_max - r);
                        G = G_k * g * (G_max - g);
                        B = B_k * b * (B_max - b);


                        if (R < 0) R = 0;
                        else if (R > 255) R = 255;

                        if (G < 0) G = 0;
                        else if (G > 255) G = 255;

                        if (B < 0) B = 0;
                        else if (B > 255) B = 255;

                        pBaseRes[i] = (byte)R;
                        pBaseRes[i + 1] = (byte)G;
                        pBaseRes[i + 2] = (byte)B;
                    }
                }
            }

            src.UnlockBitmap();
            res.UnlockBitmap();

            return BitmapRes;
        }

        // Эффект тиснения
        unsafe static public Bitmap Emboss(Bitmap bitmapSrc)
        {
            double[,] H = {{0,  1,  0},
                           {1,  0, -1},
                           {0, -1,  0}};


            /*
            double[,] H = {{0,  1,  0},
                           {1, -4,  1},
                           {0,  1,  0}};
            */

            return ApplyKernelWithScaling(bitmapSrc, H);
        }

        // Низкочастотный фильтр
        public enum LowpassFilterеType : byte { H1, H2, H3 }
        public static Bitmap LowpassFilter(Bitmap bitmapSrc, LowpassFilterеType type)
        {
            double[,] h1 = {{1, 1, 1},
                           {1, 1, 1},
                           {1, 1, 1}};

            double[,] h2 = {{1, 1, 1},
                            {1, 2, 1},
                            {1, 1, 1}};

            double[,] h3 = {{1, 2, 1},
                            {2, 4, 2},
                            {1, 2, 1}};

            double[,] H = null;

            if (type == LowpassFilterеType.H1) H = h1;
            else if (type == LowpassFilterеType.H2) H = h2;
            else if (type == LowpassFilterеType.H3) H = h3;

            return ApplyNormalizedKernel(bitmapSrc, H);
        }

        // Псевдополутоновое бинарное изображение
        public enum BinaryMaskType : byte { D2, D4 }
        public static Bitmap PseudoHalftone(Bitmap bitmapSrc, BinaryMaskType type, bool grayscale = true)
        {
            double[,] d2 = {{0, 2},
                            {3, 1,}};

            double[,] d4 = {{0,   4,   2, 10},
                            {12,  4,  14,  6},
                            {3,  11,   1,  9},
                            {15,  7,  13,  5}};

            double[,] mask = null;

            if (type == BinaryMaskType.D2)
                mask = d2;
            else if (type == BinaryMaskType.D4)
                mask = d4;

            if (grayscale)
                return ApplyMask_grayscale(bitmapSrc, mask);
            else
                return ApplyMask(bitmapSrc, mask);
        }

        //-----------------------------------------------------------------------------------------------

        // Свёртка с ядром с масштабированием результата
        unsafe static public Bitmap ApplyKernelWithScaling(Bitmap bitmapSrc, double[,] kernel)
        {
            Bitmap bitmapRes = new Bitmap(bitmapSrc.Width, bitmapSrc.Height, bitmapSrc.PixelFormat);

            int depth = Bitmap.GetPixelFormatSize(bitmapSrc.PixelFormat);
            Byte* pBaseSrc, pBaseRes;

            UnsafeBitmap src_wrap = new UnsafeBitmap(bitmapSrc);
            UnsafeBitmap res_wrap = new UnsafeBitmap(bitmapRes);

            pBaseSrc = src_wrap.LockBitmap();
            pBaseRes = res_wrap.LockBitmap();

            int width = bitmapSrc.Width;
            int height = bitmapSrc.Height;

            Int16[] array = new Int16[width * height * depth / 3];

            int strNum = kernel.GetLength(0);
            int colNum = kernel.GetLength(1);

            int pos, pixelPosX, pixelPosY;
            Int16 max = (Int16)pBaseSrc[0], min = (Int16)pBaseSrc[0];

            if (depth == 32)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        double rSum = 0, gSum = 0, bSum = 0;
                        for (int i = 0; i < strNum; i++)
                        {
                            for (int j = 0; j < colNum; j++)
                            {
                                pixelPosX = x + (i - (strNum / 2));
                                pixelPosY = y + (j - (colNum / 2));

                                if (pixelPosX == width) pixelPosX = 0;
                                if (pixelPosX == -1) pixelPosX = width - 1;
                                if (pixelPosY == height) pixelPosY = 0;
                                if (pixelPosY == -1) pixelPosY = height - 1;

                                pos = ((pixelPosY * width) + pixelPosX) * 4;

                                rSum += pBaseSrc[pos] * kernel[i, j];
                                gSum += pBaseSrc[pos + 1] * kernel[i, j];
                                bSum += pBaseSrc[pos + 2] * kernel[i, j];
                            }
                        }

                        pos = ((y * width) + x) * 4;

                        array[pos] = (Int16)rSum;
                        array[pos + 1] = (Int16)gSum;
                        array[pos + 2] = (Int16)bSum;

                        if (array[pos] > max) max = array[pos];
                        if (array[pos + 1] > max) max = array[pos + 1];
                        if (array[pos + 2] > max) max = array[pos + 2];
                        if (array[pos] < min) min = array[pos];
                        if (array[pos + 1] < min) min = array[pos + 1];
                        if (array[pos + 2] < min) min = array[pos + 2];
                    }
                }

                Int16 delta = (Int16)(max - min);
                if (delta == 0) delta = 1;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        pos = ((y * width) + x) * 4;

                        Int16 r = array[pos];
                        Int16 g = array[pos + 1];
                        Int16 b = array[pos + 2];

                        pBaseRes[pos] = (byte)((r - min) * 255 / delta);
                        pBaseRes[pos + 1] = (byte)((g - min) * 255 / delta);
                        pBaseRes[pos + 2] = (byte)((b - min) * 255 / delta);
                        pBaseRes[pos + 3] = pBaseSrc[pos + 3];
                    }
                }
            }
            else if (depth == 24)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        double rSum = 0, gSum = 0, bSum = 0;
                        for (int i = 0; i < strNum; i++)
                        {
                            for (int j = 0; j < colNum; j++)
                            {
                                pixelPosX = x + (i - (strNum / 2));
                                pixelPosY = y + (j - (colNum / 2));

                                if (pixelPosX == width) pixelPosX = 0;
                                if (pixelPosX == -1) pixelPosX = width - 1;
                                if (pixelPosY == height) pixelPosY = 0;
                                if (pixelPosY == -1) pixelPosY = height - 1;

                                pos = ((pixelPosY * width) + pixelPosX) * 3;

                                rSum += pBaseSrc[pos] * kernel[i, j];
                                gSum += pBaseSrc[pos + 1] * kernel[i, j];
                                bSum += pBaseSrc[pos + 2] * kernel[i, j];
                            }
                        }

                        pos = ((y * width) + x) * 3;


                        array[pos] = (Int16)rSum;
                        array[pos + 1] = (Int16)gSum;
                        array[pos + 2] = (Int16)bSum;

                        if (array[pos] > max) max = array[pos];
                        if (array[pos + 1] > max) max = array[pos + 1];
                        if (array[pos + 2] > max) max = array[pos + 2];
                        if (array[pos] < min) min = array[pos];
                        if (array[pos + 1] < min) min = array[pos + 1];
                        if (array[pos + 2] < min) min = array[pos + 2];
                    }
                }

                Int16 delta = (Int16)(max - min);
                if (delta == 0) delta = 1;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        pos = ((y * width) + x) * 3;

                        Int16 r = array[pos];
                        Int16 g = array[pos + 1];
                        Int16 b = array[pos + 2];

                        pBaseRes[pos] = (byte)((r - min) * 255 / delta);
                        pBaseRes[pos + 1] = (byte)((g - min) * 255 / delta);
                        pBaseRes[pos + 2] = (byte)((b - min) * 255 / delta);
                    }
                }

            }


            src_wrap.UnlockBitmap();
            res_wrap.UnlockBitmap();
            return bitmapRes;
        }

        // Свёртка с ядром с независимым масштабированием каналов R, G, B
        unsafe static public Bitmap ApplyKernelWithIndependentScaling(Bitmap bitmapSrc, double[,] kernel)
        {
            Bitmap bitmapRes = new Bitmap(bitmapSrc.Width, bitmapSrc.Height, bitmapSrc.PixelFormat);

            int depth = Bitmap.GetPixelFormatSize(bitmapSrc.PixelFormat);
            Byte* pBaseSrc, pBaseRes;

            UnsafeBitmap src_wrap = new UnsafeBitmap(bitmapSrc);
            UnsafeBitmap res_wrap = new UnsafeBitmap(bitmapRes);

            pBaseSrc = src_wrap.LockBitmap();
            pBaseRes = res_wrap.LockBitmap();

            int width = bitmapSrc.Width;
            int height = bitmapSrc.Height;

            Int16[] array = new Int16[width * height * depth / 3];

            int strNum = kernel.GetLength(0);
            int colNum = kernel.GetLength(1);

            int pos, pixelPosX, pixelPosY;
            Int16 maxR = (Int16)pBaseSrc[0], maxG = (Int16)pBaseSrc[1], maxB = (Int16)pBaseSrc[2];
            Int16 minR = (Int16)pBaseSrc[0], minG = (Int16)pBaseSrc[1], minB = (Int16)pBaseSrc[2];

            if (depth == 32)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        double rSum = 0, gSum = 0, bSum = 0;
                        for (int i = 0; i < strNum; i++)
                        {
                            for (int j = 0; j < colNum; j++)
                            {
                                pixelPosX = x + (i - (strNum / 2));
                                pixelPosY = y + (j - (colNum / 2));

                                if (pixelPosX == width) pixelPosX = 0;
                                if (pixelPosX == -1) pixelPosX = width - 1;
                                if (pixelPosY == height) pixelPosY = 0;
                                if (pixelPosY == -1) pixelPosY = height - 1;

                                pos = ((pixelPosY * width) + pixelPosX) * 4;

                                rSum += pBaseSrc[pos] * kernel[i, j];
                                gSum += pBaseSrc[pos + 1] * kernel[i, j];
                                bSum += pBaseSrc[pos + 2] * kernel[i, j];
                            }
                        }

                        pos = ((y * width) + x) * 4;

                        array[pos] = (Int16)rSum;
                        array[pos + 1] = (Int16)gSum;
                        array[pos + 2] = (Int16)bSum;

                        if (array[pos] > maxR) maxR = array[pos];
                        if (array[pos + 1] > maxG) maxG = array[pos + 1];
                        if (array[pos + 2] > maxB) maxB = array[pos + 2];
                        if (array[pos] < minR) minR = array[pos];
                        if (array[pos + 1] < minG) minG = array[pos + 1];
                        if (array[pos + 2] < minB) minB = array[pos + 2];
                    }
                }

                Int16 deltaR = (Int16)(maxR - minR), deltaG = (Int16)(maxG - minG), deltaB = (Int16)(maxB - minB);
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        pos = ((y * width) + x) * 4;

                        Int16 r = array[pos];
                        Int16 g = array[pos + 1];
                        Int16 b = array[pos + 2];

                        pBaseRes[pos] = (byte)((r - minR) * 255 / deltaR);
                        pBaseRes[pos + 1] = (byte)((g - minG) * 255 / deltaG);
                        pBaseRes[pos + 2] = (byte)((b - minB) * 255 / deltaB);
                        pBaseRes[pos + 3] = pBaseSrc[pos + 3];
                    }
                }
            }
            else if (depth == 24)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        double rSum = 0, gSum = 0, bSum = 0;
                        for (int i = 0; i < strNum; i++)
                        {
                            for (int j = 0; j < colNum; j++)
                            {
                                pixelPosX = x + (i - (strNum / 2));
                                pixelPosY = y + (j - (colNum / 2));

                                if (pixelPosX == width) pixelPosX = 0;
                                if (pixelPosX == -1) pixelPosX = width - 1;
                                if (pixelPosY == height) pixelPosY = 0;
                                if (pixelPosY == -1) pixelPosY = height - 1;

                                pos = ((pixelPosY * width) + pixelPosX) * 3;

                                rSum += pBaseSrc[pos] * kernel[i, j];
                                gSum += pBaseSrc[pos + 1] * kernel[i, j];
                                bSum += pBaseSrc[pos + 2] * kernel[i, j];
                            }
                        }

                        pos = ((y * width) + x) * 3;


                        array[pos] = (Int16)rSum;
                        array[pos + 1] = (Int16)gSum;
                        array[pos + 2] = (Int16)bSum;

                        if (array[pos] > maxR)
                            maxR = array[pos];
                        if (array[pos + 1] > maxG) maxG = array[pos + 1];
                        if (array[pos + 2] > maxB) maxB = array[pos + 2];
                        if (array[pos] < minR) minR = array[pos];
                        if (array[pos + 1] < minG) minG = array[pos + 1];
                        if (array[pos + 2] < minB) minB = array[pos + 2];
                    }
                }

                Int16 deltaR = (Int16)(maxR - minR), deltaG = (Int16)(maxG - minG), deltaB = (Int16)(maxB - minB);
                if (deltaR == 0) deltaR = 1;
                if (deltaG == 0) deltaG = 1;
                if (deltaB == 0) deltaB = 1;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        pos = ((y * width) + x) * 3;

                        Int16 r = array[pos];
                        Int16 g = array[pos + 1];
                        Int16 b = array[pos + 2];

                        pBaseRes[pos] = (byte)((r - minR) * 255 / deltaR);
                        pBaseRes[pos + 1] = (byte)((g - minG) * 255 / deltaG);
                        pBaseRes[pos + 2] = (byte)((b - minB) * 255 / deltaB);
                    }
                }

            }


            src_wrap.UnlockBitmap();
            res_wrap.UnlockBitmap();
            return bitmapRes;
        }

        // Свёртка с нормализованным ядром
        unsafe public static Bitmap ApplyNormalizedKernel(Bitmap bitmapSrc, double[,] kernel)
        {
            Bitmap bitmapRes = new Bitmap(bitmapSrc.Width, bitmapSrc.Height, bitmapSrc.PixelFormat);

            int depth = Bitmap.GetPixelFormatSize(bitmapSrc.PixelFormat);
            Byte* pBaseSrc, pBaseRes;

            UnsafeBitmap src_wrap = new UnsafeBitmap(bitmapSrc);
            UnsafeBitmap res_wrap = new UnsafeBitmap(bitmapRes);

            pBaseSrc = src_wrap.LockBitmap();
            pBaseRes = res_wrap.LockBitmap();

            int width = bitmapSrc.Width;
            int height = bitmapSrc.Height;

            int kernelWidth = kernel.GetLength(0);
            int kernelHeight = kernel.GetLength(1);

            int pos;

            //Производим вычисления
            if (depth == 24)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        double rSum = 0, gSum = 0, bSum = 0, kSum = 0;

                        for (int i = 0; i < kernelWidth; i++)
                        {
                            for (int j = 0; j < kernelHeight; j++)
                            {
                                int pixelPosX = x + (i - (kernelWidth / 2));
                                int pixelPosY = y + (j - (kernelHeight / 2));

                                if (pixelPosX == width) pixelPosX = 0;
                                if (pixelPosX == -1) pixelPosX = width - 1;
                                if (pixelPosY == height) pixelPosY = 0;
                                if (pixelPosY == -1) pixelPosY = height - 1;

                                pos = 3 * (width * pixelPosY + pixelPosX);
                                byte r = pBaseSrc[pos + 0];
                                byte g = pBaseSrc[pos + 1];
                                byte b = pBaseSrc[pos + 2];

                                double kernelVal = kernel[i, j];

                                rSum += r * kernelVal;
                                gSum += g * kernelVal;
                                bSum += b * kernelVal;

                                kSum += kernelVal;
                            }
                        }

                        if (kSum <= 0) kSum = 1;

                        //Контролируем переполнения переменных
                        rSum /= kSum;
                        if (rSum < 0) rSum = 0;
                        if (rSum > 255) rSum = 255;

                        gSum /= kSum;
                        if (gSum < 0) gSum = 0;
                        if (gSum > 255) gSum = 255;

                        bSum /= kSum;
                        if (bSum < 0) bSum = 0;
                        if (bSum > 255) bSum = 255;

                        //Записываем значения в результирующее изображение
                        pos = 3 * (width * y + x);
                        pBaseRes[pos + 0] = (byte)rSum;
                        pBaseRes[pos + 1] = (byte)gSum;
                        pBaseRes[pos + 2] = (byte)bSum;
                    }
                }

            }
            else if (depth == 32)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        double rSum = 0, gSum = 0, bSum = 0, kSum = 0;

                        for (int i = 0; i < kernelWidth; i++)
                        {
                            for (int j = 0; j < kernelHeight; j++)
                            {
                                int pixelPosX = x + (i - (kernelWidth / 2));
                                int pixelPosY = y + (j - (kernelHeight / 2));

                                if (pixelPosX == width) pixelPosX = 0;
                                if (pixelPosX == -1) pixelPosX = width - 1;
                                if (pixelPosY == height) pixelPosY = 0;
                                if (pixelPosY == -1) pixelPosY = height - 1;

                                pos = 4 * (width * pixelPosY + pixelPosX);
                                byte r = pBaseSrc[pos + 0];
                                byte g = pBaseSrc[pos + 1];
                                byte b = pBaseSrc[pos + 2];

                                double kernelVal = kernel[i, j];

                                rSum += r * kernelVal;
                                gSum += g * kernelVal;
                                bSum += b * kernelVal;

                                kSum += kernelVal;
                            }
                        }

                        if (kSum <= 0) kSum = 1;

                        //Контролируем переполнения переменных
                        rSum /= kSum;
                        if (rSum < 0) rSum = 0;
                        if (rSum > 255) rSum = 255;

                        gSum /= kSum;
                        if (gSum < 0) gSum = 0;
                        if (gSum > 255) gSum = 255;

                        bSum /= kSum;
                        if (bSum < 0) bSum = 0;
                        if (bSum > 255) bSum = 255;

                        //Записываем значения в результирующее изображение
                        pos = 4 * (width * y + x);
                        pBaseRes[pos + 0] = (byte)rSum;
                        pBaseRes[pos + 1] = (byte)gSum;
                        pBaseRes[pos + 2] = (byte)bSum;
                        pBaseRes[pos + 3] = pBaseSrc[pos + 3];
                    }
                }
            }

            res_wrap.UnlockBitmap();
            src_wrap.UnlockBitmap();

            //Возвращаем отфильтрованное изображение
            return bitmapRes;
        }

        // Свёртка с ядром без масштабирования (<0 = 0, >255 = 255)
        unsafe public static Bitmap ApplyKernel(Bitmap bitmapSrc, double[,] kernel)
        {
            Bitmap bitmapRes = new Bitmap(bitmapSrc.Width, bitmapSrc.Height, bitmapSrc.PixelFormat);

            int depth = Bitmap.GetPixelFormatSize(bitmapSrc.PixelFormat);
            Byte* pBaseSrc, pBaseRes;

            UnsafeBitmap src_wrap = new UnsafeBitmap(bitmapSrc);
            UnsafeBitmap res_wrap = new UnsafeBitmap(bitmapRes);

            pBaseSrc = src_wrap.LockBitmap();
            pBaseRes = res_wrap.LockBitmap();

            int width = bitmapSrc.Width;
            int height = bitmapSrc.Height;

            int kernelWidth = kernel.GetLength(0);
            int kernelHeight = kernel.GetLength(1);

            int pos;

            //Производим вычисления
            if (depth == 24)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        double rSum = 0, gSum = 0, bSum = 0;

                        for (int i = 0; i < kernelWidth; i++)
                        {
                            for (int j = 0; j < kernelHeight; j++)
                            {
                                int pixelPosX = x + (i - (kernelWidth / 2));
                                int pixelPosY = y + (j - (kernelHeight / 2));

                                if (pixelPosX == width) pixelPosX = 0;
                                if (pixelPosX == -1) pixelPosX = width - 1;
                                if (pixelPosY == height) pixelPosY = 0;
                                if (pixelPosY == -1) pixelPosY = height - 1;

                                pos = 3 * (width * pixelPosY + pixelPosX);
                                byte r = pBaseSrc[pos + 0];
                                byte g = pBaseSrc[pos + 1];
                                byte b = pBaseSrc[pos + 2];

                                double kernelVal = kernel[i, j];

                                rSum += r * kernelVal;
                                gSum += g * kernelVal;
                                bSum += b * kernelVal;
                            }
                        }

                        if (rSum < 0) rSum = 0;
                        if (rSum > 255) rSum = 255;

                        if (gSum < 0) gSum = 0;
                        if (gSum > 255) gSum = 255;

                        if (bSum < 0) bSum = 0;
                        if (bSum > 255) bSum = 255;

                        //Записываем значения в результирующее изображение
                        pos = 3 * (width * y + x);
                        pBaseRes[pos + 0] = (byte)rSum;
                        pBaseRes[pos + 1] = (byte)gSum;
                        pBaseRes[pos + 2] = (byte)bSum;
                    }
                }

            }
            else if (depth == 32)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        double rSum = 0, gSum = 0, bSum = 0;

                        for (int i = 0; i < kernelWidth; i++)
                        {
                            for (int j = 0; j < kernelHeight; j++)
                            {
                                int pixelPosX = x + (i - (kernelWidth / 2));
                                int pixelPosY = y + (j - (kernelHeight / 2));

                                if (pixelPosX == width) pixelPosX = 0;
                                if (pixelPosX == -1) pixelPosX = width - 1;
                                if (pixelPosY == height) pixelPosY = 0;
                                if (pixelPosY == -1) pixelPosY = height - 1;

                                pos = 4 * (width * pixelPosY + pixelPosX);
                                byte r = pBaseSrc[pos + 0];
                                byte g = pBaseSrc[pos + 1];
                                byte b = pBaseSrc[pos + 2];

                                double kernelVal = kernel[i, j];

                                rSum += r * kernelVal;
                                gSum += g * kernelVal;
                                bSum += b * kernelVal;
                            }
                        }

                        if (rSum < 0) rSum = 0;
                        if (rSum > 255) rSum = 255;

                        if (gSum < 0) gSum = 0;
                        if (gSum > 255) gSum = 255;

                        if (bSum < 0) bSum = 0;
                        if (bSum > 255) bSum = 255;

                        //Записываем значения в результирующее изображение
                        pos = 4 * (width * y + x);
                        pBaseRes[pos + 0] = (byte)rSum;
                        pBaseRes[pos + 1] = (byte)gSum;
                        pBaseRes[pos + 2] = (byte)bSum;
                        pBaseRes[pos + 3] = pBaseSrc[pos + 3];
                    }
                }
            }

            res_wrap.UnlockBitmap();
            src_wrap.UnlockBitmap();

            //Возвращаем отфильтрованное изображение
            return bitmapRes;
        }

        // Пропорциональная маска без перекрытия - бинарное изображение независимо в каждом канале
        unsafe public static Bitmap ApplyMask(Bitmap bitmapSrc, double[,] mask)
        {
            Bitmap bitmapRes = new Bitmap(bitmapSrc.Width, bitmapSrc.Height, bitmapSrc.PixelFormat);

            int depth = Bitmap.GetPixelFormatSize(bitmapSrc.PixelFormat);
            Byte* pBaseSrc, pBaseRes;

            UnsafeBitmap src_wrap = new UnsafeBitmap(bitmapSrc);
            UnsafeBitmap res_wrap = new UnsafeBitmap(bitmapRes);

            pBaseSrc = src_wrap.LockBitmap();
            pBaseRes = res_wrap.LockBitmap();

            int width = bitmapSrc.Width;
            int height = bitmapSrc.Height;

            int maskWidth = mask.GetLength(0);
            int maskHeight = mask.GetLength(1);

            double maxValue = mask.Cast<double>().Max();

            int pos;

            if (depth == 24)
            {
                for (int x = 0; x < width; x += maskWidth)
                {
                    for (int y = 0; y < height; y += maskHeight)
                    {
                        double mul = 255 / maxValue;

                        for (int i = 0; i < maskWidth; i++)
                        {
                            for (int j = 0; j < maskHeight; j++)
                            {
                                int pixelPosX = x + i;
                                int pixelPosY = y + j;

                                if (pixelPosX >= width) pixelPosX -= width;
                                if (pixelPosY >= height) pixelPosY -= height;

                                pos = 3 * (width * pixelPosY + pixelPosX);

                                if (pBaseSrc[pos + 0] < mul * mask[j, i])
                                    pBaseRes[pos + 0] = 0;
                                else pBaseRes[pos + 0] = 255;

                                if (pBaseSrc[pos + 1] < mul * mask[j, i])
                                    pBaseRes[pos + 1] = 0;
                                else pBaseRes[pos + 1] = 255;

                                if (pBaseSrc[pos + 2] < mul * mask[j, i])
                                    pBaseRes[pos + 2] = 0;
                                else pBaseRes[pos + 2] = 255;
                            }
                        }
                    }
                }

            }
            else if (depth == 32)
            {
                for (int x = 0; x < width; x += maskWidth)
                {
                    for (int y = 0; y < height; y += maskHeight)
                    {
                        double mul = 255 / maxValue;

                        for (int i = 0; i < maskWidth; i++)
                        {
                            for (int j = 0; j < maskHeight; j++)
                            {
                                int pixelPosX = x + i;
                                int pixelPosY = y + j;

                                if (pixelPosX >= width) pixelPosX -= width;
                                if (pixelPosY >= height) pixelPosY -= height;

                                pos = 4 * (width * pixelPosY + pixelPosX);

                                if (pBaseSrc[pos + 0] < mul * mask[j, i])
                                    pBaseRes[pos + 0] = 0;
                                else pBaseRes[pos + 0] = 255;

                                if (pBaseSrc[pos + 1] < mul * mask[j, i])
                                    pBaseRes[pos + 1] = 0;
                                else pBaseRes[pos + 1] = 255;

                                if (pBaseSrc[pos + 2] < mul * mask[j, i])
                                    pBaseRes[pos + 2] = 0;
                                else pBaseRes[pos + 2] = 255;

                                pBaseRes[pos + 3] = pBaseSrc[pos + 3];
                            }
                        }
                    }
                }
            }

            res_wrap.UnlockBitmap();
            src_wrap.UnlockBitmap();

            return bitmapRes;
        }

        // Пропорциональная маска без перекрытия - бинарное изображение одинаково во всех каналах
        unsafe public static Bitmap ApplyMask_grayscale(Bitmap bitmapSrc, double[,] mask)
        {
            Bitmap bitmapRes = new Bitmap(bitmapSrc.Width, bitmapSrc.Height, bitmapSrc.PixelFormat);

            int depth = Bitmap.GetPixelFormatSize(bitmapSrc.PixelFormat);
            Byte* pBaseSrc, pBaseRes;

            UnsafeBitmap src_wrap = new UnsafeBitmap(bitmapSrc);
            UnsafeBitmap res_wrap = new UnsafeBitmap(bitmapRes);

            pBaseSrc = src_wrap.LockBitmap();
            pBaseRes = res_wrap.LockBitmap();

            int width = bitmapSrc.Width;
            int height = bitmapSrc.Height;

            int maskWidth = mask.GetLength(0);
            int maskHeight = mask.GetLength(1);

            double maxValue = mask.Cast<double>().Max();

            int pos;

            if (depth == 24)
            {
                for (int x = 0; x < width; x += maskWidth)
                {
                    for (int y = 0; y < height; y += maskHeight)
                    {

                        double mul = 255 / maxValue;

                        for (int i = 0; i < maskWidth; i++)
                        {
                            for (int j = 0; j < maskHeight; j++)
                            {
                                int pixelPosX = x + i;
                                int pixelPosY = y + j;

                                if (pixelPosX >= width) pixelPosX -= width;
                                if (pixelPosY >= height) pixelPosY -= height;

                                pos = 3 * (width * pixelPosY + pixelPosX);

                                double value = 0.3 * pBaseSrc[pos + 0] + 0.59 * pBaseSrc[pos + 1] + 0.11 * pBaseSrc[pos + 2];

                                if (value < mul * mask[j, i])
                                {
                                    pBaseRes[pos + 0] = 0;
                                    pBaseRes[pos + 1] = 0;
                                    pBaseRes[pos + 2] = 0;
                                }
                                else
                                {
                                    pBaseRes[pos + 0] = 255;
                                    pBaseRes[pos + 1] = 255;
                                    pBaseRes[pos + 2] = 255;
                                }
                            }
                        }
                    }
                }

            }
            else if (depth == 32)
            {
                for (int x = 0; x < width; x += maskWidth)
                {
                    for (int y = 0; y < height; y += maskHeight)
                    {
                        double mul = 255 / maxValue;

                        for (int i = 0; i < maskWidth; i++)
                        {
                            for (int j = 0; j < maskHeight; j++)
                            {
                                int pixelPosX = x + i;
                                int pixelPosY = y + j;

                                if (pixelPosX >= width) pixelPosX -= width;
                                if (pixelPosY >= height) pixelPosY -= height;

                                pos = 4 * (width * pixelPosY + pixelPosX);

                                double value = 0.3 * pBaseSrc[pos + 0] + 0.59 * pBaseSrc[pos + 1] + 0.11 * pBaseSrc[pos + 2];

                                if (value < mul * mask[j, i])
                                {
                                    pBaseRes[pos + 0] = 0;
                                    pBaseRes[pos + 1] = 0;
                                    pBaseRes[pos + 2] = 0;
                                }
                                else
                                {
                                    pBaseRes[pos + 0] = 255;
                                    pBaseRes[pos + 1] = 255;
                                    pBaseRes[pos + 2] = 255;
                                }
                                pBaseRes[pos + 3] = pBaseSrc[pos + 3];
                            }
                        }
                    }
                }
            }

            res_wrap.UnlockBitmap();
            src_wrap.UnlockBitmap();

            return bitmapRes;
        }


        //----------------------------------------------------------------------------------------------

        // Перевод в оттенки серого и пороговая бинаризация
        unsafe public static Bitmap GrayscaleAndBinarization_unsafe(Bitmap bitmapSrc, byte threshold)
        {
            if (bitmapSrc.PixelFormat != PixelFormat.Format24bppRgb && bitmapSrc.PixelFormat != PixelFormat.Format32bppArgb
                && bitmapSrc.PixelFormat != PixelFormat.Format32bppRgb && bitmapSrc.PixelFormat != PixelFormat.Canonical)
                throw new Exception("Unsupported color depth. Support only 24bppRgb, 32bppArgb and 32bppRgb");

            Bitmap bitmapRes = new Bitmap(bitmapSrc.Width, bitmapSrc.Height, bitmapSrc.PixelFormat);
            int depth = Bitmap.GetPixelFormatSize(bitmapSrc.PixelFormat);
            int byteDepth = depth / 8;

            Byte* pBaseSrc, pBaseRes;

            UnsafeBitmap src = new UnsafeBitmap(bitmapSrc);
            UnsafeBitmap res = new UnsafeBitmap(bitmapRes);

            pBaseSrc = src.LockBitmap();
            pBaseRes = res.LockBitmap();

            int width = bitmapSrc.Width;

            int byteWidth = bitmapSrc.Width * byteDepth;
            if (byteWidth % 4 != 0)
                byteWidth = 4 * (byteWidth / 4 + 1);

            int height = bitmapSrc.Height;

            int i;
            double value;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    i = (y * byteWidth) + x * byteDepth;
                    value = 0.3 * pBaseSrc[i + 0] + 0.59 * pBaseSrc[i + 1] + 0.11 * pBaseSrc[i + 2];

                    if (value >= (double)threshold)
                    {
                        pBaseRes[i] = 255;
                        pBaseRes[i + 1] = 255;
                        pBaseRes[i + 2] = 255;
                    }
                    else
                    {
                        pBaseRes[i] = 0;
                        pBaseRes[i + 1] = 0;
                        pBaseRes[i + 2] = 0;
                    }

                    if (byteDepth == 4)
                        pBaseRes[i + 3] = pBaseSrc[i + 3];
                }
            }

            src.UnlockBitmap();
            res.UnlockBitmap();

            return bitmapRes;
        }

        // Перевод в оттенки серого и пороговая бинаризация через bitmap.GetPixel
        public static Bitmap GrayscaleAndBinarization(Bitmap bitmapSrc, byte threshold)
        {
            Bitmap bitmapRes = new Bitmap(bitmapSrc.Width, bitmapSrc.Height, bitmapSrc.PixelFormat);

            int width = bitmapSrc.Width;
            int height = bitmapSrc.Height;

            double value;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color pixel = bitmapSrc.GetPixel(x, y);

                    value = 0.3 * pixel.R + 0.59 * pixel.G + 0.11 * pixel.B;

                    if (value >= (double)threshold)
                        bitmapRes.SetPixel(x, y, Color.FromArgb(255, 255, 255));
                    else
                        bitmapRes.SetPixel(x, y, Color.FromArgb(0, 0, 0));
                }
            }
            return bitmapRes;
        }

        //----------------------------------------------------------------------------------------------

        // Медианный фильтр
        unsafe public static Bitmap MedianFilter_unsafe(Bitmap bitmapSrc, int kernelSize)
        {
            if (bitmapSrc.PixelFormat != PixelFormat.Format24bppRgb && bitmapSrc.PixelFormat != PixelFormat.Format32bppArgb
                && bitmapSrc.PixelFormat != PixelFormat.Format32bppRgb && bitmapSrc.PixelFormat != PixelFormat.Canonical)
                throw new Exception("Unsupported color depth. Support only 24bppRgb, 32bppArgb and 32bppRgb");

            if(kernelSize < 3 || kernelSize % 2 == 0)
                throw new Exception("Size of kernel of Median filter should be greater than or equal to 3 and odd.");

            Bitmap bitmapRes = new Bitmap(bitmapSrc.Width, bitmapSrc.Height, bitmapSrc.PixelFormat);
            int depth = Bitmap.GetPixelFormatSize(bitmapSrc.PixelFormat);
            int byteDepth = depth / 8;

            Byte* pBaseSrc, pBaseRes;

            UnsafeBitmap src = new UnsafeBitmap(bitmapSrc);
            UnsafeBitmap res = new UnsafeBitmap(bitmapRes);

            pBaseSrc = src.LockBitmap();
            pBaseRes = res.LockBitmap();

            int width = bitmapSrc.Width;

            int byteWidth = bitmapSrc.Width * byteDepth;
            if (byteWidth % 4 != 0)
                byteWidth = 4 * (byteWidth / 4 + 1);

            int height = bitmapSrc.Height;

            int strNum = kernelSize;
            int colNum = kernelSize;

            int pixelPosX, pixelPosY, pos;

            int indent = kernelSize / 2;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (y >= indent && y < height - indent && x >= indent && x < width - indent)
                    {
                        byte[] arrayR = new byte[colNum * strNum];
                        byte[] arrayG = new byte[colNum * strNum];
                        byte[] arrayB = new byte[colNum * strNum];
                        int arrIndex = 0;

                        for (int i = 0; i < strNum; i++)
                        {
                            for (int j = 0; j < colNum; j++)
                            {
                                pixelPosX = x + (i - (strNum / 2));
                                pixelPosY = y + (j - (colNum / 2));

                                pos = (pixelPosY * byteWidth) + pixelPosX * byteDepth;

                                arrayR[arrIndex] = pBaseSrc[pos];
                                arrayG[arrIndex] = pBaseSrc[pos + 1];
                                arrayB[arrIndex] = pBaseSrc[pos + 2];
                                arrIndex++;
                            }
                        }

                        Array.Sort(arrayR);
                        Array.Sort(arrayG);
                        Array.Sort(arrayB);

                        pos = (y * byteWidth) + x * byteDepth;

                        pBaseRes[pos] = arrayR[arrayR.Length / 2];
                        pBaseRes[pos + 1] = arrayG[arrayG.Length / 2];
                        pBaseRes[pos + 2] = arrayB[arrayB.Length / 2];
                    }
                    else
                    {
                        pos = (y * byteWidth) + x * byteDepth;

                        pBaseRes[pos] = pBaseSrc[pos];
                        pBaseRes[pos + 1] = pBaseSrc[pos + 1];
                        pBaseRes[pos + 2] = pBaseSrc[pos + 2];
                    }

                    if (byteDepth == 4)
                        pBaseRes[pos + 3] = pBaseSrc[pos + 3];
                }
            }

            src.UnlockBitmap();
            res.UnlockBitmap();

            return bitmapRes;
        }

    }
}
