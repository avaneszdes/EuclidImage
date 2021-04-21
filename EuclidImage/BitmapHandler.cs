using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EuclidImage
{
    public class BitmapHandler
    {
        public double[,] GetBinaryArrayFromBitmap(Bitmap bitmap)
        {
            double[,] binaryArray = new double[bitmap.Width, bitmap.Height];

            for (int i = 0; i < binaryArray.GetLength(0); i++)
            {
                for (int j = 0; j < binaryArray.GetLength(1); j++)
                {
                    Color a = bitmap.GetPixel(j, i);
                    if (a.R == 255 && a.G == 255 && a.B == 255)
                    {
                        binaryArray[i, j] = 0 * -1;
                    }
                    else
                    {
                        binaryArray[i, j] = 1;
                    }

                }
            }
            return binaryArray;
        }

        public static double EqulidDistance(double x1, double y1, double x2, double y2) => Math.Round(Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2)), 2);

        public static double[,] Eroziya(double[,] arrayForEroziya, double[,] mask)
        {
            double[,] result = new double[arrayForEroziya.GetLength(0), arrayForEroziya.GetLength(1)];
            for (int i = 1; i < arrayForEroziya.GetLength(0) - 1; i++)
            {
                for (int j = 1; j < arrayForEroziya.GetLength(1) - 1; j++)
                {
                    if (arrayForEroziya[i, j] == mask[1, 1] &&
                        arrayForEroziya[i - 1, j - 1] == mask[0, 0] &&
                        arrayForEroziya[i - 1, j] == mask[0, 1] &&
                        arrayForEroziya[i - 1, j + 1] == mask[0, 2] &&
                        arrayForEroziya[i, j - 1] == mask[1, 0] &&
                        arrayForEroziya[i, j + 1] == mask[1, 2] &&
                        arrayForEroziya[i + 1, j - 1] == mask[2, 0] &&
                        arrayForEroziya[i + 1, j + 1] == mask[2, 2] &&
                        arrayForEroziya[i + 1, j] == mask[2, 1]
                       )
                    {
                        result[i, j] = 1;
                    }
                    else
                    {
                        result[i, j] = 0;
                    }
                }
            }


            return result;
        }

        public static double[,] GetMaskValues(DataGridView mask)
        {
            double[,] result = new double[3, 3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    result[i, j] = int.Parse(mask[j, i].Value.ToString());
                }
            }

            return result;
        }


        public static double[,] GetArrayForFourthLabBitmap(double[,] pixels, string textBox1)
        {

            double[,] res = new double[pixels.GetLength(0), pixels.GetLength(1)];

            for (int i = 1; i < res.GetLength(0) - 1; i++)
            {
                for (int j = 1; j < res.GetLength(1) - 1; j++)
                {
                    double temp = Math.Abs(
                        pixels[i, j] - ((pixels[i, j]
                        + pixels[i - 1, j - 1]
                        + pixels[i - 1, j] + pixels[i - 1, j + 1]
                        + pixels[i, j - 1] + pixels[i, j + 1]
                        + pixels[i + 1, j - 1] + pixels[i + 1, j + 1]
                        + pixels[i + 1, j]) / 9));
                    if (double.Parse(textBox1) > temp)
                    {
                        res[i, j] = (pixels[i, j] + pixels[i - 1, j - 1] + pixels[i - 1, j] + pixels[i - 1, j + 1] + pixels[i, j - 1] + pixels[i, j + 1] + pixels[i + 1, j - 1] + pixels[i + 1, j + 1] + pixels[i + 1, j]) / 9;
                    }
                    else
                    {
                        res[i, j] = pixels[i - 1, j - 1];
                    }
                }
            }

            return res;
        }

        private static List<string> GetIndexesForEqulidianDistance(int number, double[,] pointss, Bitmap firstBitmap)
        {
            int globalIForOne = 0;
            int globalJForOne = 0;
            int globalIForZero = 0;
            int globalJForZero = 0;

            List<string> result = new List<string>();
            if (number == 1)
            {
                for (int i = 0; i < firstBitmap.Width; i++)
                {
                    for (int j = 0; j < firstBitmap.Height; j++)
                    {
                        if (pointss[i, j] == 1 && i >= globalIForOne && j >= globalJForOne)
                        {
                            globalIForOne = i;
                            globalJForOne = j;
                            result.Add(i + "," + j);
                        }
                    }
                    globalJForOne = 0;
                }
            }
            if (number == 0)
            {
                for (int i = 0; i < firstBitmap.Width; i++)
                {
                    for (int j = 0; j < firstBitmap.Height; j++)
                    {
                        if (pointss[i, j] == 0 && i >= globalIForZero && j >= globalJForZero)
                        {
                            globalIForZero = i;
                            globalJForZero = j;
                            result.Add(i + "," + j);
                        }
                    }
                    globalJForZero = 0;
                }
            }

            return result;
        }

        public static List<double> GetCalculateEwclidDistance(double[,] pointss, Bitmap firstBitmap)
        {
            Dictionary<string, List<string>> indexes = new Dictionary<string, List<string>>();
            for (int i = 0; i < firstBitmap.Width; i++)
            {
                for (int j = 0; j < firstBitmap.Height; j++)
                {
                    if (pointss[i, j] == 0)
                    {
                        indexes.Add(i + "," + j, GetIndexesForEqulidianDistance(1, pointss, firstBitmap));
                    }
                    if (pointss[i, j] == 1)
                    {
                        indexes.Add(i + "," + j, GetIndexesForEqulidianDistance(0, pointss, firstBitmap));
                    }
                }
            }

            List<double> result = new List<double>();
            foreach (var item in indexes)
            {
                var itemArray = item.Key.Split(',').ToArray();
                var itemValues = item.Value.Select(x => x.Split(',').ToArray()).ToArray();
                List<double> res = new List<double>();

                for (int i = 0; i < itemValues.Length; i++)
                {
                    var a = itemValues[i][0];
                    var b = itemValues[i][1];
                    var c = itemArray[0];
                    var d = itemArray[1];

                    res.Add(BitmapHandler.EqulidDistance(double.Parse(itemArray[0]), double.Parse(itemArray[1]), double.Parse(itemValues[i][0]), double.Parse(itemValues[i][1])));
                }

                result.Add(res.Min());
            }
            return result;

        }

        public static double[,] GetAverageValuesFromBitmap(Bitmap bitmap, double[,] borderValues)
        {
            double[,] borderValuesArr = new double[bitmap.Width, bitmap.Height];

            List<double> tempList = new List<double>();
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    tempList.Add(borderValues[i, j]);
                }
            }

            var avgValue = tempList.Average();
            for (int i = 1; i < borderValues.GetLength(0) - 1; i++)
            {
                for (int j = 1; j < borderValues.GetLength(1) - 1; j++)
                {
                    var pixelValue = 255 - borderValues[i, j];
                    if (pixelValue < avgValue)
                    {
                        borderValuesArr[j, i] = 1;

                    }
                    else
                    {
                        borderValuesArr[j, i] = 0;
                    }
                }
            }

            return borderValuesArr;
        }
    }
}
