using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EuclidImage
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            pictureBox1.Hide();
            dataGridView1.Hide();
            dataGridView1.Font = new Font("Verdana", 10, FontStyle.Italic);
            dataGridView1.AutoGenerateColumns = false;
            panel2.Location = new Point(5, 5);
            this.Width = panel2.Width + 25;
            this.Height = panel2.Height + 50;
            chart1.ChartAreas[0].AxisX.Interval = 0.1;

        }

        double[,] matr;
        int[,] signsForGreyScaleBitmap;
        Bitmap secondBitMap;
        Bitmap firstBitmap;

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                openFileDialog1 = new OpenFileDialog
                {
                    InitialDirectory = @"C:\",
                    Title = "Browse Text Files",

                    CheckFileExists = true,
                    CheckPathExists = true,

                    DefaultExt = "bmp",
                    Filter = "bmp files (*.bmp)|*.bmp",
                    FilterIndex = 2,
                    RestoreDirectory = true,

                    ReadOnlyChecked = true,
                    ShowReadOnly = true
                };

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    dataGridView1.Rows.Clear();

                    pictureBox2.Hide();
                    pictureBox1.Show();
                    dataGridView1.Show();
                    var file = openFileDialog1.FileName;
                    firstBitmap = new Bitmap(file);

                    pictureBox1.Width = firstBitmap.Width + 120;
                    pictureBox1.Height = firstBitmap.Height + 120;
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

                    if ((50 * firstBitmap.Width) > 750)
                    {
                        dataGridView1.Width = 750;
                        dataGridView1.Height = 375;
                        dataGridView1.ScrollBars = ScrollBars.Both;
                    }
                    else
                    {
                        dataGridView1.Width = 50 * firstBitmap.Width;
                        dataGridView1.Height = 25 * firstBitmap.Height;
                    }

                    trackBar1.Height = dataGridView1.Height - 30;
                    label1.Location = new Point(pictureBox1.Width + 30, label1.Location.Y);
                    trackBar1.Location = new Point(pictureBox1.Width + 20, label1.Location.Y + 30);

                    panel2.Location = new Point(panel2.Location.X, dataGridView1.Height + 20);

                    this.Width = firstBitmap.Width + 160 + dataGridView1.Width + trackBar1.Width + 3;
                    this.Height = dataGridView1.Height + panel2.Height + 65;

                    chart1.Width = dataGridView1.Width + trackBar1.Width + 3;
                    chart1.Height = panel2.Height;

                    dataGridView1.Location = new Point(pictureBox1.Width + 20 + trackBar1.Width + 6, dataGridView1.Location.Y);
                    chart1.Location = new Point(dataGridView1.Location.X - trackBar1.Width - 5, dataGridView1.Height + 20);
                    pictureBox1.Image = firstBitmap;

                    var bmpWidth = firstBitmap.Width;
                    var bmpHeight = firstBitmap.Height;
                    int[,] pixels = new int[bmpWidth, bmpHeight];

                    dataGridView1.Columns.Clear();
                    for (int i = 0; i < bmpWidth; i++)
                    {
                        dataGridView1.Columns.Add(i + 1 + "", i + 1 + "");

                    }

                    dataGridView1.Rows.Add(bmpWidth);

                    for (int i = 0; i < bmpWidth; i++)
                    {
                        DataGridViewRow row2 = dataGridView1.Rows[i];
                        dataGridView1.Columns[i].Width = 50;
                    }
                    var isBinaryImage = IsBinaryImage(firstBitmap);


                    if (!isBinaryImage)
                    {
                        AddDataToDataGridView(GetAvgValuesFromGreyScaleBitmap(firstBitmap));
                    }
                    else
                    {
                        AddDataToDataGridView(GetBinaryArrayFromBitmap(firstBitmap));
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            button2.Enabled = true;
            button5.Enabled = true;
            button8.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            chart1.Height = panel2.Height;
            dataGridView1.Height = 25 * firstBitmap.Height;
            chart1.Location = new Point(chart1.Location.X, dataGridView1.Height + 20);


            for (int i = 0; i < firstBitmap.Width; i++)
            {
                DataGridViewRow row2 = dataGridView1.Rows[i];
                dataGridView1.Columns[i].Width = 50;
            }
            dataGridView1.ScrollBars = ScrollBars.None;


            var res = GetCalculateEwclidDistance(GetBinaryArrayFromBitmap(firstBitmap));

            int k = 0;

            for (int i = 0; i < firstBitmap.Width; i++)
            {
                for (int j = 0; j < firstBitmap.Height; j++)
                {
                    if (signsForGreyScaleBitmap[i, j] == 0)
                    {
                        dataGridView1[j, i].Value = res[k] * -1;

                    }
                    else
                    {
                        dataGridView1[j, i].Value = res[k];
                    }
                    k++;
                }
            }

            chart1.Series[0].Points.Clear();
            secondBitMap = CreateGrayscaleBitmap(secondBitMap);
            pictureBox2.Show();
            pictureBox2.Image = secondBitMap;
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.Location = new Point(pictureBox1.Location.X, pictureBox1.Height + 15);
            pictureBox2.Width = secondBitMap.Width + 120;
            pictureBox2.Height = secondBitMap.Height + 120;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            chart1.Series[0].Points.Clear();
            double[,] avgValuesFromBitmap = GetAvgValuesFromGreyScaleBitmap(firstBitmap);
            double[,] borderValuesArr = GetBorderValuesFromBitmap(firstBitmap, avgValuesFromBitmap);
            var bitmap = new Bitmap(firstBitmap.Width, firstBitmap.Height);
            GetBinaryBitmap(bitmap, borderValuesArr);
            pictureBox2.Show();
            pictureBox2.Image = bitmap;
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.Location = new Point(pictureBox1.Location.X, pictureBox1.Height + 15);
            pictureBox2.Width = bitmap.Width + 120;
            pictureBox2.Height = bitmap.Height + 120;
            var binaryValues = GetBinaryArrayFromBitmap(bitmap);
            AddDataToDataGridView(binaryValues);
            DrawGistogramm(binaryValues);
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            colorDialog1 = new ColorDialog();
            Bitmap newBmp = new Bitmap(firstBitmap.Width, firstBitmap.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {

                for (int i = 0; i < newBmp.Width; i++)
                {
                    for (int j = 0; j < newBmp.Height; j++)
                    {
                        if (e.RowIndex == j && i == e.ColumnIndex)
                        {
                            newBmp.SetPixel(e.ColumnIndex, e.RowIndex, colorDialog1.Color);
                            dataGridView1[i, j].Value = colorDialog1.Color.R + "-" + colorDialog1.Color.G + "-" + colorDialog1.Color.B;
                            dataGridView1.Update();
                        }
                        else
                        {
                            newBmp.SetPixel(i, j, firstBitmap.GetPixel(i, j));
                        }

                    }
                }
                firstBitmap = newBmp;
                pictureBox1.Image = firstBitmap;
            }


        }

        private void button8_Click(object sender, EventArgs e)
        {
            dataGridView1.Height = 25 * firstBitmap.Height + 25;
            chart1.Height = chart1.Height - 25;
            chart1.Location = new Point(chart1.Location.X, dataGridView1.Height + 20);
            dataGridView1.ScrollBars = ScrollBars.Horizontal;
            for (int i = 0; i < firstBitmap.Width; i++)
            {
                DataGridViewRow row2 = dataGridView1.Rows[i];
                dataGridView1.Columns[i].Width = 120;
            }
            dataGridView1.ScrollBars = ScrollBars.Horizontal;
            for (int i = 0; i < firstBitmap.Width; i++)
            {
                for (int j = 0; j < firstBitmap.Height; j++)
                {
                    Color a = firstBitmap.GetPixel(j, i);
                    dataGridView1[j, i].Value = a.R + "-" + a.G + "-" + a.B;

                }

            }
        }

        public List<double> GetCalculateEwclidDistance(double[,] pointss)
        {
            try
            {
                Dictionary<string, List<string>> indexes = new Dictionary<string, List<string>>();
                for (int i = 0; i < firstBitmap.Width; i++)
                {
                    for (int j = 0; j < firstBitmap.Height; j++)
                    {
                        if (pointss[i, j] == 0)
                        {
                            indexes.Add(i + "," + j, GetIndex(1, pointss));
                        }
                        if (pointss[i, j] == 1)
                        {
                            indexes.Add(i + "," + j, GetIndex(0, pointss));
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

                        res.Add(Distance(double.Parse(itemArray[0]), double.Parse(itemArray[1]), double.Parse(itemValues[i][0]), double.Parse(itemValues[i][1])));
                    }

                    result.Add(res.Min());
                }
                return result;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            return null;
        }

        private List<string> GetIndex(int number, double[,] pointss)
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

        public double Distance(double x1, double y1, double x2, double y2) => Math.Round(Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2)), 2);

        private void SaveImg(Bitmap bitmap)
        {
            saveFileDialog1 = new SaveFileDialog
            {
                InitialDirectory = @"C:\Desktop",
                Title = "Browse Text Files",
                DefaultExt = "bmp",
                Filter = "bmp files (*.bmp)|*.bmp",
                FilterIndex = 2,
            };

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string path = saveFileDialog1.FileName;


                bitmap.Save(path);
            }
        }

        private double[,] GetBinaryArrayFromBitmap(Bitmap bitmap)
        {
            signsForGreyScaleBitmap = new int[bitmap.Width, bitmap.Height];
            double[,] binaryArray = new double[bitmap.Width, bitmap.Height];

            for (int i = 0; i < binaryArray.GetLength(0); i++)
            {
                for (int j = 0; j < binaryArray.GetLength(1); j++)
                {
                    Color a = bitmap.GetPixel(j, i);
                    if (a.R == 255 && a.G == 255 && a.B == 255)
                    {
                        binaryArray[i, j] = 0;
                        signsForGreyScaleBitmap[i, j] = 0;
                    }
                    else
                    {
                        binaryArray[i, j] = 1;
                        signsForGreyScaleBitmap[i, j] = 1;
                    }

                }
            }

            return binaryArray;
        }

        private bool IsBinaryImage(Bitmap bitmap)
        {
            var arr = new int[bitmap.Width * bitmap.Height];
            int k = 0;
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    var pixel = bitmap.GetPixel(i, j);
                    var pixelColor = (pixel.R + pixel.G + pixel.B);
                    if (pixelColor == 765 || pixelColor == 0)
                    {
                        arr[k++] = 0;
                    }
                    else
                    {
                        arr[k++] = 1;
                    }
                }
            }

            return !arr.Contains(1);
        }

        private Bitmap CreateGrayscaleBitmap(Bitmap secondBitMap)
        {
            chart1.Series[0].Points.Clear();
            matr = new double[firstBitmap.Width, firstBitmap.Height];
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                for (int j = 0; j < dataGridView1.ColumnCount; j++)
                {
                    matr[i, j] = double.Parse(dataGridView1.Rows[i].Cells[j].Value.ToString());
                }
            }
            int Count = 0;     ///Коллво цветов в изобр

            Dictionary<double, double> rezul = new Dictionary<double, double>(); //считаем эл-ты в массиве
            for (int i = 0; i < matr.GetLength(0); i++)
            {
                for (int j = 0; j < matr.GetLength(1); j++)
                {
                    if (rezul.ContainsKey(matr[i, j]))
                    {
                        rezul[matr[i, j]]++;
                    }
                    else
                    {
                        rezul.Add(matr[i, j], 1); Count++;
                    }
                }

            }

            double[] difColor;
            double[] countColor;
            difColor = new double[Count];
            countColor = new double[Count];
            int l = 0;



            foreach (KeyValuePair<double, double> par in rezul)
            {
                chart1.Series[0].Points.AddXY(par.Key, par.Value);
                chart1.Series[0].IsValueShownAsLabel = true;
                difColor[l] = par.Key;
                countColor[l] = par.Value;
                l++;

            }


            int indexForColor = 255 / Count;
            secondBitMap = new Bitmap(firstBitmap.Width, firstBitmap.Height);
            Array.Sort(difColor);
            Array.Reverse(difColor);
            int position;
            for (int i = 0; i < secondBitMap.Width; i++)
            {
                for (int j = 0; j < secondBitMap.Height; j++)
                {

                    for (int ii = 0; ii < difColor.GetLength(0); ii++)
                    {
                        if (matr[i, j] == difColor[ii])
                        {
                            position = ii + 1;
                            Color newColor = Color.FromArgb(position * indexForColor, position * indexForColor, position * indexForColor);
                            secondBitMap.SetPixel(j, i, newColor);

                        }


                    }

                }
            }

            return secondBitMap;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label1.Text = trackBar1.Value.ToString();
            var trackBarValue = int.Parse(trackBar1.Value.ToString());
            if (trackBarValue >= 175)
            {
                trackBar1.BackColor = Color.Red;
            }
            else if (trackBarValue >= 150 && trackBarValue < 175)
            {
                trackBar1.BackColor = Color.Tomato;
            }
            else if (trackBarValue >= 100 && trackBarValue < 150)
            {
                trackBar1.BackColor = Color.LightSalmon;
            }
            else
            {
                trackBar1.BackColor = SystemColors.GradientInactiveCaption;
            }

            var changebleBitmap = new Bitmap(firstBitmap.Width, firstBitmap.Height);
            for (int x = 0; x < changebleBitmap.Width; x++)
            {
                for (int y = 0; y < changebleBitmap.Height; y++)
                {
                    Color color = firstBitmap.GetPixel(x, y);
                    int avgColor = (color.R + color.G + color.B) / 3;
                    if (avgColor > trackBar1.Value)
                    {

                        changebleBitmap.SetPixel(x, y, Color.White);
                        dataGridView1[x, y].Value = 0;
                    }
                    else
                    {
                        changebleBitmap.SetPixel(x, y, Color.Black);
                        dataGridView1[x, y].Value = 1;

                    }
                }
            }


            secondBitMap = changebleBitmap;
            pictureBox2.Show();
            pictureBox2.Image = secondBitMap;
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.Location = new Point(pictureBox1.Location.X, pictureBox1.Height + 15);
            pictureBox2.Width = secondBitMap.Width + 120;
            pictureBox2.Height = secondBitMap.Height + 120;


            pictureBox2.Image = secondBitMap;
        }

        private void DrawGistogramm(double[,] arr)
        {
            int count = 0;
            Dictionary<double, double> rezul = new Dictionary<double, double>(); //считаем эл-ты в массиве
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                for (int j = 0; j < arr.GetLength(1); j++)
                {
                    if (rezul.ContainsKey(arr[i, j]))
                    {
                        rezul[arr[i, j]]++;
                    }
                    else
                    {
                        rezul.Add(arr[i, j], 1); count++;
                    }
                }

            }

            foreach (KeyValuePair<double, double> par in rezul)
            {
                chart1.Series[0].Points.AddXY(par.Key, par.Value);
                chart1.Series[0].IsValueShownAsLabel = true;
            }
        }

        private void AddDataToDataGridView(double[,] arr)
        {
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                for (int j = 0; j < arr.GetLength(1); j++)
                {
                    dataGridView1[j, i].Value = arr[i, j];
                }
            }
        }

        private void GetBinaryBitmap(Bitmap bitmap, double[,] arr)
        {
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {

                    if (arr[i, j] == 0)
                    {
                        bitmap.SetPixel(i, j, Color.White);

                    }
                    else
                    {
                        bitmap.SetPixel(i, j, Color.Black);

                    }
                }
            }

            secondBitMap = bitmap;
        }

        private double[,] GetBorderValuesFromBitmap(Bitmap bitmap, double[,] borderValues)
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
            //button1.Text = avgValue.ToString();
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

        private double[,] GetAvgValuesFromGreyScaleBitmap(Bitmap bitmap)
        {
            double[,] pixels = new double[bitmap.Width, bitmap.Height];
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    Color a = bitmap.GetPixel(j, i);
                    int avgColor = (a.R + a.G + a.B) / 3;
                    pixels[i, j] = 255 - avgColor;

                }

            }

            return pixels;
        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            Bitmap bit = new Bitmap(firstBitmap.Width, firstBitmap.Height);

            for (int i = 0; i < firstBitmap.Width; i++)
            {
                for (int j = 0; j < firstBitmap.Height; j++)
                {
                    bit.SetPixel(i, j, firstBitmap.GetPixel(i, j));
                }

            }
            SaveImg(bit);
        }

        private void pictureBox2_DoubleClick(object sender, EventArgs e)
        {
            Bitmap bit = new Bitmap(firstBitmap.Width, firstBitmap.Height);

            for (int i = 0; i < firstBitmap.Width; i++)
            {
                for (int j = 0; j < firstBitmap.Height; j++)
                {
                    bit.SetPixel(i, j, secondBitMap.GetPixel(i, j));
                }

            }
            SaveImg(bit);

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Bitmap bitmap = new Bitmap(firstBitmap.Width, firstBitmap.Height);

            for (int i = 0; i < firstBitmap.Width; i++)
            {
                for (int j = 0; j < firstBitmap.Height; j++)
                {
                    var avg = int.Parse(dataGridView1[j, i].Value.ToString());
                    Color c = Color.FromArgb(avg, avg, avg);
                    bitmap.SetPixel(i, j, c);
                }
            }


            secondBitMap = bitmap;
            pictureBox2.Show();
            pictureBox2.Image = secondBitMap;
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.Location = new Point(pictureBox1.Location.X, pictureBox1.Height + 15);
            pictureBox2.Width = secondBitMap.Width + 120;
            pictureBox2.Height = secondBitMap.Height + 120;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            dataGridView2.Rows.Add(6);
            for (int i = 0; i < 4; i++)
            {
                DataGridViewRow row2 = dataGridView2.Rows[i];
                dataGridView2.Columns[i].Width = 80;
            }
            dataGridView2.Width = 4 * 80;
            dataGridView2.Height = 30 * 5;
            dataGridView2.Location = new Point(dataGridView1.Location.X + dataGridView1.Width + 10, dataGridView1.Location.Y);
            this.Width = this.Width + dataGridView2.Width + 12;
            List<int> R = new List<int>();
            List<int> G = new List<int>();
            List<int> B = new List<int>();
            for (int i = 0; i < firstBitmap.Width; i++)
            {
                for (int j = 0; j < firstBitmap.Height; j++)
                {
                    var color = firstBitmap.GetPixel(i, j);
                    R.Add(color.R);
                    G.Add(color.G);
                    B.Add(color.B);
                }
            }

            int avgR = ((R.Max() - R.Min()) / 2) + R.Min();
            int avgG = ((G.Max() - G.Min()) / 2) + G.Min();
            int avgB = ((B.Max() - B.Min()) / 2) + B.Min();

            dataGridView2[1, 0].Value = "R";
            dataGridView2[1, 0].Style.BackColor = Color.Red;
            dataGridView2[2, 0].Value = "G";
            dataGridView2[2, 0].Style.BackColor = Color.Green;
            dataGridView2[3, 0].Value = "B";
            dataGridView2[3, 0].Style.BackColor = Color.Blue;
            dataGridView2[0, 1].Value = "Min";
            dataGridView2[1, 1].Value = R.Min();
            dataGridView2[2, 1].Value = G.Min();
            dataGridView2[3, 1].Value = B.Min();
            dataGridView2[0, 2].Value = "Max";
            dataGridView2[1, 2].Value = R.Max();
            dataGridView2[2, 2].Value = G.Max();
            dataGridView2[3, 2].Value = B.Max();
            dataGridView2[0, 3].Value = "Avg";
            dataGridView2[1, 3].Value = avgR;
            dataGridView2[2, 3].Value = avgG;
            dataGridView2[3, 3].Value = avgB;
            dataGridView2[0, 4].Value = "Color1";
            dataGridView2[1, 4].Value = ((R.Max() - R.Min()) / 4) + R.Min();
            dataGridView2[2, 4].Value = ((G.Max() - G.Min()) / 4) + G.Min();
            dataGridView2[3, 4].Value = ((B.Max() - B.Min()) / 4) + B.Min();
            dataGridView2[0, 5].Value = "Color2";
            dataGridView2[1, 5].Value = ((R.Max() - avgR) / 2) + avgR;
            dataGridView2[2, 5].Value = ((G.Max() - avgG) / 2) + avgG;
            dataGridView2[3, 5].Value = ((B.Max() - avgB) / 2) + avgB;


            dataGridView3.Rows.Add(9);
            for (int i = 0; i < 4; i++)
            {
                DataGridViewRow row3 = dataGridView3.Rows[i];
                dataGridView3.Columns[i].Width = 80;
            }
            dataGridView3.Width = 4 * 80;
            dataGridView3.Height = (30 * 8) - 11;



            dataGridView3.Location = new Point(dataGridView1.Location.X + dataGridView1.Width + 10, dataGridView1.Location.Y + dataGridView2.Height + 10);

            dataGridView3[1, 0].Value = "R";
            dataGridView3[1, 0].Style.BackColor = Color.Red;
            dataGridView3[2, 0].Value = "G";
            dataGridView3[2, 0].Style.BackColor = Color.Green;
            dataGridView3[3, 0].Value = "B";
            dataGridView3[3, 0].Style.BackColor = Color.Blue;
            dataGridView3[0, 1].Value = "1";
            dataGridView3[1, 1].Value = dataGridView2[1, 4].Value;
            dataGridView3[2, 1].Value = dataGridView2[2, 4].Value;
            dataGridView3[3, 1].Value = dataGridView2[3, 4].Value;
            dataGridView3[0, 2].Value = "2";
            dataGridView3[1, 2].Value = dataGridView2[1, 4].Value;
            dataGridView3[2, 2].Value = dataGridView2[2, 4].Value;
            dataGridView3[3, 2].Value = dataGridView2[3, 5].Value;
            dataGridView3[0, 3].Value = "3";
            dataGridView3[1, 3].Value = dataGridView2[1, 4].Value;
            dataGridView3[2, 3].Value = dataGridView2[2, 5].Value;
            dataGridView3[3, 3].Value = dataGridView2[3, 5].Value;
            dataGridView3[0, 4].Value = "4";
            dataGridView3[1, 4].Value = dataGridView2[1, 5].Value;
            dataGridView3[2, 4].Value = dataGridView2[2, 5].Value;
            dataGridView3[3, 4].Value = dataGridView2[3, 5].Value;
            dataGridView3[0, 5].Value = "5";
            dataGridView3[1, 5].Value = dataGridView2[1, 5].Value;
            dataGridView3[2, 5].Value = dataGridView2[2, 5].Value;
            dataGridView3[3, 5].Value = dataGridView2[3, 4].Value;
            dataGridView3[0, 6].Value = "6";
            dataGridView3[1, 6].Value = dataGridView2[1, 4].Value;
            dataGridView3[2, 6].Value = dataGridView2[2, 5].Value;
            dataGridView3[3, 6].Value = dataGridView2[3, 4].Value;
            dataGridView3[0, 7].Value = "7";
            dataGridView3[1, 7].Value = dataGridView2[1, 5].Value;
            dataGridView3[2, 7].Value = dataGridView2[2, 4].Value;
            dataGridView3[3, 7].Value = dataGridView2[3, 5].Value;
            dataGridView3[0, 8].Value = "8";
            dataGridView3[1, 8].Value = dataGridView2[1, 5].Value;
            dataGridView3[2, 8].Value = dataGridView2[2, 4].Value;
            dataGridView3[3, 8].Value = dataGridView2[3, 4].Value;

            double[,] valuesForGistagramm = new double[firstBitmap.Width, firstBitmap.Height];
            for (int i = 0; i < firstBitmap.Width; i++)
            {
                for (int j = 0; j < firstBitmap.Height; j++)
                {
                    var minR = firstBitmap.GetPixel(i, j).R;
                    var minG = firstBitmap.GetPixel(i, j).G;
                    var minB = firstBitmap.GetPixel(i, j).B;
                    int[] arrR = new int[8];
                    for (int k = 0; k < 8; k++)
                    {
                        arrR[k] = int.Parse(dataGridView3[1, k + 1].Value.ToString());
                    }
                    int[] arrG = new int[8];
                    for (int k = 0; k < 8; k++)
                    {
                        arrG[k] = int.Parse(dataGridView3[2, k + 1].Value.ToString());
                    }
                    int[] arrB = new int[8];
                    for (int k = 0; k < 8; k++)
                    {
                        arrB[k] = int.Parse(dataGridView3[3, k + 1].Value.ToString());
                    }

                                                                            
                    dataGridView1[j, i].Value = GetRowIndexFromDataGridView3(GetColorValuesFromDataGridView3(minR, arrR),
                                                                            GetColorValuesFromDataGridView3(minG, arrG), 
                                                                            GetColorValuesFromDataGridView3(minB, arrB));
                    valuesForGistagramm[i, j] = double.Parse(dataGridView1[j, i].Value.ToString());
                }
            }


            Bitmap bitmap = new Bitmap(firstBitmap.Width, firstBitmap.Height);

            for (int i = 0; i < firstBitmap.Width; i++)
            {
                for (int j = 0; j < firstBitmap.Height; j++)
                {

                    if(int.Parse(dataGridView1[j,i].Value.ToString()) == 1)
                    {
                        Color c = Color.FromArgb(int.Parse(dataGridView3[1, 1].Value.ToString()), int.Parse(dataGridView3[2, 1].Value.ToString()), int.Parse(dataGridView3[3, 1].Value.ToString()));
                        bitmap.SetPixel(i, j, c);
                    }
                    else if (int.Parse(dataGridView1[j, i].Value.ToString()) == 2)
                    {
                        Color c = Color.FromArgb(int.Parse(dataGridView3[1, 2].Value.ToString()), int.Parse(dataGridView3[2, 2].Value.ToString()), int.Parse(dataGridView3[3, 2].Value.ToString()));
                        bitmap.SetPixel(i, j, c);
                    }
                    else if (int.Parse(dataGridView1[j, i].Value.ToString()) == 3)
                    {
                        Color c = Color.FromArgb(int.Parse(dataGridView3[1, 3].Value.ToString()), int.Parse(dataGridView3[2, 3].Value.ToString()), int.Parse(dataGridView3[3, 3].Value.ToString()));
                        bitmap.SetPixel(i, j, c);
                    }
                    else if (int.Parse(dataGridView1[j, i].Value.ToString()) == 4)
                    {
                        Color c = Color.FromArgb(int.Parse(dataGridView3[1, 4].Value.ToString()), int.Parse(dataGridView3[2, 4].Value.ToString()), int.Parse(dataGridView3[3, 4].Value.ToString()));
                        bitmap.SetPixel(i, j, c);
                    }
                    else if (int.Parse(dataGridView1[j, i].Value.ToString()) == 5)
                    {
                        Color c = Color.FromArgb(int.Parse(dataGridView3[1, 5].Value.ToString()), int.Parse(dataGridView3[2, 5].Value.ToString()), int.Parse(dataGridView3[3, 5].Value.ToString()));
                        bitmap.SetPixel(i, j, c);
                    }
                    else if (int.Parse(dataGridView1[j, i].Value.ToString()) == 6)
                    {
                        Color c = Color.FromArgb(int.Parse(dataGridView3[1, 6].Value.ToString()), int.Parse(dataGridView3[2, 6].Value.ToString()), int.Parse(dataGridView3[3, 6].Value.ToString()));
                        bitmap.SetPixel(i, j, c);
                    }
                    else if (int.Parse(dataGridView1[j, i].Value.ToString()) == 7)
                    {
                        Color c = Color.FromArgb(int.Parse(dataGridView3[1, 7].Value.ToString()), int.Parse(dataGridView3[2, 7].Value.ToString()), int.Parse(dataGridView3[3, 7].Value.ToString()));
                        bitmap.SetPixel(i, j, c);
                    }
                    else if (int.Parse(dataGridView1[j, i].Value.ToString()) == 8)
                    {
                        Color c = Color.FromArgb(int.Parse(dataGridView3[1, 8].Value.ToString()), int.Parse(dataGridView3[2, 8].Value.ToString()), int.Parse(dataGridView3[3, 8].Value.ToString()));
                        bitmap.SetPixel(i, j, c);
                    }

                }
            }


            secondBitMap = bitmap;
            pictureBox2.Show();
            pictureBox2.Image = secondBitMap;
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.Location = new Point(pictureBox1.Location.X, pictureBox1.Height + 15);
            pictureBox2.Width = secondBitMap.Width + 120;
            pictureBox2.Height = secondBitMap.Height + 120;

            DrawGistogramm(valuesForGistagramm);
        }


        private int GetColorValuesFromDataGridView3(int colorValue, int[] arr)
        {

            return arr.OrderBy(v => Math.Abs((long)v - colorValue)).First();
        }


        private int GetRowIndexFromDataGridView3(int r, int g, int b)
        {

            for (int i = 0; i < 8; i++)
            {
                if (r == int.Parse(dataGridView3[1, i + 1].Value.ToString()) && 
                    g == int.Parse(dataGridView3[2, i + 1].Value.ToString()) && 
                    b == int.Parse(dataGridView3[3, i + 1].Value.ToString()))
                {
                    return i + 1;
                }
            }

            return 0;
        }


    }

}
