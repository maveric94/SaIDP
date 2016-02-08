using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Collections;

namespace tsosii_lab1
{
    class СlusterАnalysis
    {
        private int[] labels;
        Bitmap bitmap;

        int width;
        int height;

        int byteDepth;
        int byteWidth;

        int objectsCount;
        int clustCount;

        ObjectParameters[] objects;

        public СlusterАnalysis(Bitmap bitmapSrc, int k)
        {
            if (bitmapSrc.PixelFormat != PixelFormat.Format24bppRgb && bitmapSrc.PixelFormat != PixelFormat.Format32bppArgb
                && bitmapSrc.PixelFormat != PixelFormat.Format32bppRgb && bitmapSrc.PixelFormat != PixelFormat.Canonical)
                throw new Exception("Unsupported color depth. Support only 24bppRgb, 32bppArgb and 32bppRgb");

            bitmap = bitmapSrc;
            width = bitmap.Width;
            height = bitmap.Height;
            byteDepth = Bitmap.GetPixelFormatSize(bitmap.PixelFormat) / 8;

            byteWidth = bitmap.Width * byteDepth;
            if (byteWidth % 4 != 0)
                byteWidth = 4 * (byteWidth / 4 + 1);

            labels = new int[width * height];
            clustCount = k;
        }

        // Выделение связных областей рекурсивным алгоритмом
        unsafe private void AllocationOfConnectedDomains_recursively()
        {
            int maxStackSize = 100000000;
            Bitmap tmp = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), bitmap.PixelFormat);

            Thread t = new Thread(() =>
            {              
                UnsafeBitmap src = new UnsafeBitmap(tmp);
                Byte* pBaseSrc = src.LockBitmap();
                int L = 1;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        FillLabels(pBaseSrc, x, y, L++);
                    }
                }
                src.UnlockBitmap();
            }, maxStackSize);

            t.Start();
            t.Join();

            objectsCount = RenumberDomains();            
        }

        unsafe private void FillLabels(Byte* pImage, int x, int y, int L)
        {
            int labelIndex = width * y + x;
            int imageIndex = y * byteWidth + x * byteDepth;

            if (labels[labelIndex] == 0 && pImage[imageIndex] == 255)
            {
                labels[labelIndex] = L;

                if (x > 0)
                    FillLabels(pImage, x - 1, y, L);

                if (x < width - 1)
                    FillLabels(pImage, x + 1, y, L);

                if (y > 0)
                    FillLabels(pImage, x, y - 1, L);

                if (y < height - 1)
                    FillLabels(pImage, x, y + 1, L);
            }
        }

        private int RenumberDomains()
        {
            List<int> uniqueValues = new List<int>();
            uniqueValues.Add(0);
            for (int i = 0; i < labels.Length; i++)
            {
                if (labels[i] == 0)
                    continue;

                if (uniqueValues.Contains(labels[i]) == false)
                {
                    uniqueValues.Add(labels[i]);
                    labels[i] = uniqueValues.Count() - 1;
                }
                else
                    labels[i] = uniqueValues.IndexOf(labels[i]);                   
            }

            return uniqueValues.Count();
        }


        unsafe private void CalculateParameters()
        {
            objects = new ObjectParameters[objectsCount];
            for (int i = 0; i < objects.Length; i++ )
                objects[i] = new ObjectParameters();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int labelIndex = GetLabelIndex(x, y);
                    if (labels[labelIndex] != 0)
                    {
                        objects[labels[labelIndex]].Square++;
                        objects[labels[labelIndex]].Xmas += x;
                        objects[labels[labelIndex]].Ymas += y;

                        if (GetLabelIndex(x + 1, y) < 0 || GetLabelIndex(x + 1, y) >= labels.Length ||
                            GetLabelIndex(x - 1, y) < 0 || GetLabelIndex(x - 1, y) >= labels.Length ||
                            GetLabelIndex(x, y + 1) < 0 || GetLabelIndex(x, y + 1) >= labels.Length ||
                            GetLabelIndex(x, y - 1) < 0 || GetLabelIndex(x, y - 1) >= labels.Length)
                            objects[labels[labelIndex]].Perimeter++;
                        // Связность 4, внутренняя граница
                        else if (labels[GetLabelIndex(x + 1, y)] != labels[labelIndex] || labels[GetLabelIndex(x - 1, y)] != labels[labelIndex]
                            || labels[GetLabelIndex(x, y + 1)] != labels[labelIndex] || labels[GetLabelIndex(x, y - 1)] != labels[labelIndex])
                            objects[labels[labelIndex]].Perimeter++;
                    }
                }
            }
            // Вычисление центра масс и компактности
            for (int i = 1; i < objects.Length; i++)
            {
                objects[i].Xmas /= (double)objects[i].Square;
                objects[i].Ymas /= (double)objects[i].Square;
                objects[i].Compactness = objects[i].Perimeter * objects[i].Perimeter / objects[i].Square;
            }

            // Дискретные центральные моменты
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int labelIndex = GetLabelIndex(x, y);
                    if (labels[labelIndex] != 0)
                    {
                        objects[labels[labelIndex]].m20 += Math.Pow((x - objects[labels[labelIndex]].Xmas), 2);
                        objects[labels[labelIndex]].m02 += Math.Pow((y - objects[labels[labelIndex]].Ymas), 2);
                        objects[labels[labelIndex]].m11 += (x - objects[labels[labelIndex]].Xmas) * (y - objects[labels[labelIndex]].Ymas);
                    }
                }
            }

            // Вычисление удлинённости и ориентаци главной оси
            for (int i = 1; i < objects.Length; i++)
            {
                objects[i].elongation = (objects[i].m20 + objects[i].m02 + Math.Sqrt(Math.Pow((objects[i].m20 - objects[i].m02), 2) + 4 * objects[i].m11 * objects[i].m11)) /
                    (objects[i].m20 + objects[i].m02 - Math.Sqrt(Math.Pow((objects[i].m20 - objects[i].m02), 2) + 4 * objects[i].m11 * objects[i].m11));

                objects[i].OrientationOfMainAxis = 1.0 / 2.0 * Math.Atan(2 * objects[i].m11 / (objects[i].m20 - objects[i].m02));
            }
        }

        private int GetLabelIndex(int x, int y)
        {
            return (width * y + x);
        }

        private bool isBadValue(ObjectParameters obj)
        {
            return double.IsNaN(obj.Compactness) || double.IsNaN(obj.elongation) ||
                    double.IsNaN(obj.OrientationOfMainAxis) || double.IsNaN(obj.Xmas) ||
                    double.IsNaN(obj.Ymas) || double.IsNaN(obj.m02) ||
                    double.IsNaN(obj.m20) || double.IsNaN(obj.m11) || double.IsInfinity(obj.Compactness) || 
                    double.IsInfinity(obj.elongation) || double.IsInfinity(obj.OrientationOfMainAxis) || 
                    double.IsInfinity(obj.Xmas) || double.IsInfinity(obj.Ymas) || double.IsInfinity(obj.m02) ||
                    double.IsInfinity(obj.m20) || double.IsInfinity(obj.m11);
        }


        private Hashtable k_means()
        {
            ObjectParameters[] medians = k_means_plus_plus(); /*new ObjectParameters[clustCount];*/
            Hashtable table = new Hashtable(clustCount);

            //Random random = new Random();
            for (int i = 0; i < clustCount; i++)
            {                
                table[i] = new List<int>();
            }
            
            Hashtable prevTable = null;
            while (true)
            {
                for(int i = 0; i < table.Count; i++)
                {
                    ((List<int>)table[i]).Clear();
                }


                // Для каждого объекта считаем расстояние до центров
                for (int i = 1; i < objects.Length; i++)
                {
                    if (isBadValue(objects[i]))
                        continue;

                    double[] distances = new double[clustCount];
                    for (int j = 0; j < clustCount; j++)
                        distances[j] = calculateDistance(objects[i], medians[j]);

                    int indexOfMin = Array.IndexOf(distances, distances.Min());
                    ((List<int>)table[indexOfMin]).Add(i);
                }

                if (prevTable != null)
                {
                    bool isSame = true;
                    for (int i = 0; i < table.Count; i++)
                    {
                        isSame = ((List<int>)table[i]).SequenceEqual((List<int>)prevTable[i]);
                        if (isSame == false)
                            break;
                    }
                    if (isSame == true)
                        break;
                }

                prevTable = (Hashtable)table.Clone();

                // Пересчитываем средние (центры масс) по всем кластерам
                for (int i = 0; i < clustCount; i++)
                {
                    /*int squareSum = 0, perimSum = 0;
                    double compactSum = 0, elongationSum = 0, orientSum = 0;
                    foreach (int numberOfObject in (List<int>)table[i])
                    {
                        squareSum += objects[numberOfObject].Square;
                        perimSum += objects[numberOfObject].Perimeter;
                        compactSum += objects[numberOfObject].Compactness;
                        elongationSum += objects[numberOfObject].elongation;
                     //   orientSum += objects[numberOfObject].OrientationOfMainAxis;
                    }
                    int count = ((List<int>)table[i]).Count();
                    if (count != 0)
                    {
                        // change here average value to median value
                        medians[i].Square = squareSum / count;
                        medians[i].Perimeter = perimSum / count;
                        medians[i].Compactness = compactSum / count;
                        medians[i].elongation = elongationSum / count;
                     //   medians[i].OrientationOfMainAxis = orientSum / count;
                    }*/

                    List<int> square = new List<int>();
                    List<int> perimeter = new List<int>();
                    List<double> compactness = new List<double>();
                    List<double> elongation = new List<double>();

                    foreach(int objectNumber in (List<int>)table[i])
                    {
                        square.Add(objects[objectNumber].Square);
                        perimeter.Add(objects[i].Perimeter);
                        compactness.Add(objects[i].Compactness);
                        elongation.Add(objects[i].elongation);
                    }

                    square.Sort();
                    perimeter.Sort();
                    compactness.Sort();
                    elongation.Sort();

                    int count = ((List<int>)table[i]).Count - 1;
                    //if (count % 2 == 1)
                    {
                        medians[i].Square = square[(count + 1) / 2];
                        medians[i].Perimeter = perimeter[(count + 1) / 2];
                        medians[i].Compactness = compactness[(count + 1) / 2];
                        medians[i].elongation = elongation[(count + 1) / 2];
                    }
                    //else
                    //{
                    //    int number = count / 2;

                    //    medians[i].Square = (square[number] + square[number - 1]) / 2;
                    //    medians[i].Perimeter = (perimeter[number] + perimeter[number - 1]) / 2;
                    //    medians[i].Compactness = (compactness[number] + compactness[number - 1]) / 2;
                    //    medians[i].elongation = (elongation[number] + elongation[number - 1]) / 2;
                    //}
                    
                }
            }
            return table;
        }

        private ObjectParameters[] k_means_plus_plus()
        {
            if (objects.Length == 1)
                return new ObjectParameters[] { new ObjectParameters(objects[0]) };
            List<ObjectParameters> medians = new List<ObjectParameters>();
            Random random = new Random();
           
            while (true)
            {                
                int index = random.Next(1, objects.Length - 1);
                if (isBadValue(objects[index]))
                    continue;
                else
                {
                    medians.Add(objects[index]);
                    medians.First().Perimeter /= 2;
                    break;
                }
            }

            while (medians.Count < clustCount)
            {
                double distSum = 0;
                for (int i = 1; i < objects.Length; i++)
                {
                    if (isBadValue(objects[i]))
                        continue;

                    double[] distances = new double[medians.Count];
                    int k = 0;
                    foreach (ObjectParameters median in medians)
                    {
                        distances[k++] = calculateDistance(objects[i], median);
                    }
                    distSum += distances.Min();
                }
                double rnd = distSum * random.NextDouble();

                distSum = 0;
                for (int i = 1; i < objects.Length; i++)
                {
                    if (isBadValue(objects[i]))
                        continue;

                    double[] distances = new double[medians.Count];
                    int k = 0;
                    foreach (ObjectParameters median in medians)
                    {
                        distances[k++] = calculateDistance(objects[i], median);
                    }
                    distSum += distances.Min();
                    if (distSum >= rnd)
                    {
                        medians.Add(objects[i]);
                        break;
                    }
                }
            }
            return medians.ToArray();
        }

        private double calculateDistance(ObjectParameters obj, ObjectParameters median)
        {
            double per = Math.Pow(obj.Perimeter - median.Perimeter, 2);
            double sq = Math.Pow(obj.Square - median.Square, 2);
            double el = Math.Pow(obj.elongation - median.elongation, 2);
            double comp = Math.Pow(obj.Compactness - median.Compactness, 2);
         //   double orient = Math.Pow(obj.OrientationOfMainAxis - median.OrientationOfMainAxis, 2);
            return Math.Sqrt(per + sq + el + comp /*+ orient*/);
        }


        unsafe public Bitmap GetClusteredBitmap()
        {
            AllocationOfConnectedDomains_recursively();
            CalculateParameters();

            Bitmap bitmapRes = new Bitmap(bitmap.Width, bitmap.Height, bitmap.PixelFormat);
            if (GetGoodObjectsCount() == 0)
                return bitmapRes;

            Hashtable table = k_means();

            Random random = new Random();
            Color[] colors = new Color[table.Count];
            for (int i = 0; i < table.Count; i++)
            {
                colors[i] = Color.FromArgb(random.Next(50, 255), random.Next(50, 255), 
                                                                    random.Next(50, 255));
            }           

            UnsafeBitmap src = new UnsafeBitmap(bitmap);
            UnsafeBitmap res = new UnsafeBitmap(bitmapRes);
            Byte* pBaseRes = res.LockBitmap();
            Byte* pBaseSrc = src.LockBitmap();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int labelIndex = GetLabelIndex(x, y);
                    if (labels[labelIndex] != 0)
                    {
                        for (int i = 0; i < table.Count; i++)
                        {
                            if (((List<int>)table[i]).Contains(labels[labelIndex]))
                            {
                                int imageIndex = (y * byteWidth) + x * byteDepth;
                                pBaseRes[imageIndex] = colors[i].R;
                                pBaseRes[imageIndex + 1] = colors[i].G;
                                pBaseRes[imageIndex + 2] = colors[i].B;

                                if(byteDepth == 4)
                                    pBaseRes[imageIndex + 3] = pBaseSrc[imageIndex + 3];

                                break;
                            }
                        }
                    }
                }
            }

            res.UnlockBitmap();
            src.UnlockBitmap();
            return bitmapRes;
        }

        public int GetGoodObjectsCount()
        {
            int count = 0;
            if (objects != null)
            {
                for (int i = 1; i < objects.Count(); i++)
                {
                    if (objects[i] != null && isBadValue(objects[i]) == false)
                        count++;
                }
            }
            return count;
        }
    }
}
