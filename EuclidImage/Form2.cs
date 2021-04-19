using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace EuclidImage
{
    public partial class Form2 : Form
    {
        private double[,] Array { get; set; }
        public Form2(double[,] array)
        {
            InitializeComponent();
            Array = array;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            dataGridView1.Location = new Point(2, 2);
            if(Array.GetLength(0) > 15)
            {
                this.dataGridView1.Width = Array.GetLength(0) * 15;
                this.dataGridView1.Height = Array.GetLength(1) * 12;
                dataGridView1.ScrollBars = ScrollBars.Both;
            }
            else
            {
                this.dataGridView1.Width = Array.GetLength(0) * 50;
                this.dataGridView1.Height = Array.GetLength(1) * 25;
            }
           
           
           
            chart1.Location = new Point(2, dataGridView1.Height + 6);
            chart1.Width = dataGridView1.Width;
            chart1.Height = dataGridView1.Height / 2;
            this.Width = dataGridView1.Width + 20;
            this.Height = dataGridView1.Height + 42 + chart1.Height + 6;

            dataGridView1.Columns.Clear();
            for (int i = 0; i < Array.GetLength(0); i++)
            {
                dataGridView1.Columns.Add(i + 1 + "", i + 1 + "");

            }

            dataGridView1.Rows.Add(Array.GetLength(1));

            for (int i = 0; i < Array.GetLength(1); i++)
            {
                DataGridViewRow row2 = dataGridView1.Rows[i];
                dataGridView1.Columns[i].Width = 50;
            }

            for (int i = 0; i < Array.GetLength(0); i++)
            {
                for (int j = 0; j < Array.GetLength(1); j++)
                {
                    this.dataGridView1[j, i].Value = Array[i, j];
                }
            }

            DrawGistogramm(Array);



        }

        private void DrawGistogramm(double[,] arr)
        {
            chart1.Series[0].Points.Clear();
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

            }
            chart1.Series[0].IsValueShownAsLabel = true;

        }
    }
}
