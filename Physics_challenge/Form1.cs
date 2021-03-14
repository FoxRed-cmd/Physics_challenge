using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Physics_challenge
{
	public partial class Form1 : Form
	{
		double speed, angle, quantity, time, height, x, y;
		const double G = 9.80665;
		private List<string> _temps = new List<string>() { "Time", "X", "Y" };

		[DllImport("winmm.dll")]
		public static extern int waveOutGetVolume(IntPtr h, out uint dwVolume);

		[DllImport("winmm.dll")]
		public static extern int waveOutSetVolume(IntPtr h, uint dwVolume);

		public Form1()
		{
			InitializeComponent();
			textBox1.Text = "69,344";
			textBox2.Text = "45";
			groupBox1.Text = "Параметры";
			label1.Text = "Скорость:";
			label2.Text = "Угол:";
			label3.Text = "Количество попыток:";
			button1.Text = "Рассчитать";
			tabPage1.Text = "График";
			tabPage2.Text = "Таблица";
			chart1.Series[0].LegendText = "Траектория";
			this.Text = "Камнем по голове";
		}
		private void Form1_Load(object sender, EventArgs e)
		{
			KeyPreview = true;
			uint _savedVolume;
			waveOutGetVolume(IntPtr.Zero, out _savedVolume);

			this.FormClosing += delegate
			{
				// восстанавливаем громкость на выходе
				waveOutSetVolume(IntPtr.Zero, _savedVolume);
			};

			// глушим
			waveOutSetVolume(IntPtr.Zero, 0);

		}

		private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
		{
			if (tabPage1.Focus() == true)
			{
				Bitmap bitmap1 = new Bitmap(chart1.Size.Width, chart1.Size.Height);
				chart1.DrawToBitmap(bitmap1, chart1.Bounds);
				e.Graphics.DrawImage(bitmap1, 0, 0);
			}
			if (tabPage2.Focus() == true)
			{
				Bitmap bitmap = new Bitmap(tableLayoutPanel1.Size.Width, tableLayoutPanel1.Size.Width);
				tableLayoutPanel1.DrawToBitmap(bitmap, tableLayoutPanel1.Bounds);
				e.Graphics.DrawImage(bitmap, 0, 0);
			}
		}

		private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
		{
			AboutBox1 aboutBox1 = new AboutBox1();
			aboutBox1.Show();
		}

		private void печатьToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			if (tabPage2.Focus() == true)
			{
				if (printDialog1.ShowDialog() == DialogResult.OK)
				{
					printDocument1.Print();
				}
			}
			if (tabPage1.Focus() == true)
			{
				if (printDialog1.ShowDialog() == DialogResult.OK)
				{
					printDocument1.Print();
				}
			}
		}

		private void Form1_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyValue)
			{
				case (int)Keys.Enter:
					button1.Focus();
					button1_Click(sender, e);
					tabControl1.Focus();
					break;

				case (int)Keys.Escape:
					Close();
					break;

				default:
					break;
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			tableLayoutPanel1.Controls.Clear();
			chart1.Series[0].Points.Clear();

			for (int i = 0; i < 3; i++)
			{
				Label temps = new Label();
				temps.ColorChange();
				temps.Text = _temps[i];
				tableLayoutPanel1.Controls.Add(temps, i, 0);
			}

			for (int i = 0, j = 1; i <= 10; i++, j++)
			{
				Label findX = new Label();
				Label findTime = new Label();
				Label findY = new Label();

				findTime.ColorChange();
				findX.ColorChange();
				findY.ColorChange();

				try
				{
					x = FindX(speed = double.Parse(textBox1.Text), angle = double.Parse(textBox2.Text), i);
					y = FindY(speed = double.Parse(textBox1.Text), angle = double.Parse(textBox2.Text), i);
				}
				catch (Exception)
				{
					MessageBox.Show("Введены неверные данные!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
					break;
				}

				string _x = String.Format("{0:f1}", x);
				string _y = String.Format("{0:f1}", y);

				chart1.Series[0].Points.AddXY(x, y);
				findX.Text = _x;
				findY.Text = _y;
				findTime.Text = i.ToString();

				tableLayoutPanel1.Controls.Add(findTime, 0, j);
				tableLayoutPanel1.Controls.Add(findX, 1, j);
				tableLayoutPanel1.Controls.Add(findY, 2, j);
			}
		}

		static double TimeFly(double speed, double angle)
		{
			angle *= Math.PI / 180.0;
			return (2 * speed * Math.Sin(angle)) / G;
		}

		static double SqrtSin(double value)
		{
			return (1 - Math.Cos(2 * value)) / 2;
		}

		static double HeightFly(double speed, double angle)
		{
			return (Math.Pow(speed, 2) / 2 * G) * SqrtSin(angle);
		}

		static double FindX(double speed, double angle, double time)
		{
			angle *= Math.PI / 180.0;
			return speed * Math.Cos(angle) * time;
		}

		static double FindY(double speed, double angle, double time)
		{
			angle *= Math.PI / 180.0;
			return (-(G * Math.Pow(time, 2)) / 2) + (speed * Math.Sin(angle) * time);
		}
	}
}
