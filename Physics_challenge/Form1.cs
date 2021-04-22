using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Physics_challenge
{
	public partial class Form1 : Form
	{
		double speed, angle, timeFly, x, y, time = 0, interval;
		List<MultiPoint<double>> points = new List<MultiPoint<double>>();
		Graphics graphics;
		Pen pen;
		Bitmap ball = Resource1.ball;
		SolidBrush brush;
		SolidBrush clear = new SolidBrush(Color.White);
		/// <summary>
		/// Ускорение свободного падения
		/// </summary>
		const double G = 9.80665;
		private List<string> _temps = new List<string>() { "Время(сек)", "Дальность(м)", "Высота(м)" };
		bool flagClear = false;
		int count = 1, countSeries = 0;

		[DllImport("winmm.dll")]
		public static extern int waveOutGetVolume(IntPtr h, out uint dwVolume);

		[DllImport("winmm.dll")]
		public static extern int waveOutSetVolume(IntPtr h, uint dwVolume);

		public Form1()
		{
			InitializeComponent();
			textBox1.Text = "69,344";
			textBox2.Text = "45";
			textBox4.Text = "0,1";
			groupBox1.Text = "Параметры";
			label1.Text = "Скорость (м/с):";
			label2.Text = "Угол (градусы):";
			label3.Text = "Количество попыток:";
			label4.Text = "Интервал (cек):";
			button1.Text = "Рассчитать";
			tabPage1.Text = "График";
			tabPage2.Text = "Таблица";
			tabPage3.Text = "Анимация";
			this.Text = "Камнем по голове";
			radioButton1.Text = "Обычный ввод";
			radioButton2.Text = "С количеством попыток";
			button2.Text = "Старт";
			radioButton1.CheckedChanged += (a, e) =>
			{
				if (radioButton1.Checked)
				{
					textBox3.Visible = false;
					label3.Visible = false;
				}
			};
			radioButton2.CheckedChanged += (a, e) =>
			{
				if (radioButton2.Checked)
				{
					textBox3.Visible = true;
					label3.Visible = true;
				}
			};

			SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
			UpdateStyles();

		}
		private void Form1_Load(object sender, EventArgs e)
		{
			radioButton1.Checked = true;
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
		private void ОчиститьToolStripMenuItem_Click(object sender, EventArgs e)
		{
			tableLayoutPanel1.Controls.Clear();
			chart1.Series.Clear();
			countSeries = 0;
			count = 1;
			flagClear = false;
			textBox1.Clear();
			textBox3.Clear();
			textBox2.Clear();
			textBox4.Clear();
		}
		private void РаспечататьВсёToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (printDialog1.ShowDialog() == DialogResult.OK)
			{
				printDocument2.Print();
			}
		}
		private void printDocument2_PrintPage(object sender, PrintPageEventArgs e)
		{
			Bitmap bitmap1 = new Bitmap(chart1.Size.Width, chart1.Size.Height);
			chart1.DrawToBitmap(bitmap1, chart1.Bounds);
			e.Graphics.DrawImage(bitmap1, 0, 0);

			Bitmap bitmap = new Bitmap(tableLayoutPanel1.Size.Width, tableLayoutPanel1.Size.Width);
			tableLayoutPanel1.DrawToBitmap(bitmap, tableLayoutPanel1.Bounds);
			e.Graphics.DrawImage(bitmap, 0, chart1.Size.Height + 20);
		}

		private void button1_Click(object sender, EventArgs e)
		{
			points.Clear();
			if (radioButton1.Checked)
			{
				Result();
			}
			if (radioButton2.Checked)
			{
				try
				{
					int value = int.Parse(textBox3.Text);
					Result(value);
				}
				catch (Exception)
				{
					MessageBox.Show("Введены неверные данные!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
			}
		}
		private void button2_Click(object sender, EventArgs e)
		{
			if (points.Count > 0)
			{
				Draw();
				//timer1.Enabled = true;
				//timer1.Interval = 3000;
				//flagAnim = false;
			}
			else
			{
				MessageBox.Show("Ошибка", "ошибка", MessageBoxButtons.OK);
			}

		}



		//private async void timer1_Tick(object sender, EventArgs e)
		//{

		//}

		static double TimeFly(double speed, double angle)
		{
			angle *= Math.PI / 180.0;
			return (2 * speed * Math.Sin(angle)) / G;
		}
		public async void Draw()
		{
			graphics = pictureBox1.CreateGraphics();
			//pen = new Pen(Color.LimeGreen, 2);
			//brush = new SolidBrush(Color.Silver);
			while (tabPage3.Focus() == true)
			{

				foreach (var item in points)
				{
					graphics.DrawImage(ball, new Rectangle ((int)item.X, (int)item.Y, 40, 40));
					//graphics.DrawEllipse(pen, (float)item.X, (float)item.Y, 10, 10);
					await Task.Delay(10);
					//Refresh();
					graphics.FillRectangle(clear, 0, 0, pictureBox1.Width, pictureBox1.Height);
				}
			}
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
		private void Result()
		{
			Label findX = new Label();
			Label findTime = new Label();
			Label findY = new Label();
			points.Clear();

			Series series = new Series();
			series.BorderWidth = 3;
			series.ChartArea = "ChartArea1";
			series.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
			series.Name = "Траектория " + count.ToString();
			chart1.Series.Add(series);
			chart1.Size = new System.Drawing.Size(578, 395);
			chart1.TabIndex = 0;
			chart1.Text = "chart1";


			if (flagClear == false)
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
				flagClear = true;
			}

			try
			{
				timeFly = Math.Round(TimeFly(speed = double.Parse(textBox1.Text), angle = double.Parse(textBox2.Text)), 2);
				interval = double.Parse(textBox4.Text);
			}
			catch (Exception)
			{
				MessageBox.Show("Введены неверные данные!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
				chart1.Series.RemoveAt(countSeries);
				return;
			}

			if (angle == 90)
			{
				findTime.ColorChange();
				findX.ColorChange();
				findY.ColorChange();

				y = HeightFly(speed, angle) / 100;
				chart1.Series[countSeries].Points.AddXY(0, y);

				findX.Text = "0";
				findY.Text = "Y=" + Math.Round(y).ToString();
				findTime.Text = Math.Round(timeFly).ToString();

				tableLayoutPanel1.Controls.Add(findTime, 0, count);
				tableLayoutPanel1.Controls.Add(findX, 1, count);
				tableLayoutPanel1.Controls.Add(findY, 2, count);
				count++;
				countSeries++;
			}
			else
			{
				do
				{
					findTime.ColorChange();
					findX.ColorChange();
					findY.ColorChange();

					x = Math.Round(FindX(speed = double.Parse(textBox1.Text), angle = double.Parse(textBox2.Text), time), 1);
					y = Math.Round(FindY(speed = double.Parse(textBox1.Text), angle = double.Parse(textBox2.Text), time), 1);

					points.Add(new MultiPoint<double> { X = x, Y = pictureBox1.Height - y });
					chart1.Series[countSeries].Points.AddXY(x, y);

					time += interval;

				} while (!(y < 0 && x != 0));

				string maxY = chart1.Series[countSeries].Points.FindMaxByValue("Y1", 1).ToString();
				int countMaxY = 0;
				foreach (var item in maxY)
				{
					if (item != 'Y')
					{
						countMaxY++;

					}
					else
					{
						break;
					}
				}
				findY.Text = maxY.Substring(countMaxY).Replace("}", "");
				findX.Text = "X=" + x.ToString();
				findTime.Text = Math.Round(time).ToString();

				tableLayoutPanel1.Controls.Add(findTime, 0, count);
				tableLayoutPanel1.Controls.Add(findX, 1, count);
				tableLayoutPanel1.Controls.Add(findY, 2, count);
				count++;
				countSeries++;
				time = 0;
			}
		}
		private void Result(int value)
		{
			points.Clear();
			try
			{
				timeFly = Math.Round(TimeFly(speed = double.Parse(textBox1.Text), angle = double.Parse(textBox2.Text)), 2);
			}
			catch (Exception)
			{
				MessageBox.Show("Введены неверные данные!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			for (int i = 0; i < value; i++)
			{
				Label findX = new Label();
				Label findTime = new Label();
				Label findY = new Label();

				Series series = new Series();
				series.BorderWidth = 3;
				series.ChartArea = "ChartArea1";
				series.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
				series.Name = "Траектория " + count.ToString();
				chart1.Series.Add(series);
				chart1.Size = new System.Drawing.Size(578, 395);
				chart1.TabIndex = 0;
				chart1.Text = "chart1";


				if (flagClear == false)
				{
					tableLayoutPanel1.Controls.Clear();
					chart1.Series[0].Points.Clear();

					for (int j = 0; j < 3; j++)
					{
						Label temps = new Label();
						temps.ColorChange();
						temps.Text = _temps[j];
						tableLayoutPanel1.Controls.Add(temps, j, 0);
					}
					flagClear = true;
				}

				if (angle == 90)
				{
					findTime.ColorChange();
					findX.ColorChange();
					findY.ColorChange();

					y = HeightFly(speed, angle) / 100;
					chart1.Series[countSeries].Points.AddXY(0, y);

					findX.Text = "0";
					findY.Text = "Y=" + Math.Round(y).ToString();
					findTime.Text = Math.Round(timeFly).ToString();

					tableLayoutPanel1.Controls.Add(findTime, 0, count);
					tableLayoutPanel1.Controls.Add(findX, 1, count);
					tableLayoutPanel1.Controls.Add(findY, 2, count);
					count++;
					countSeries++;
				}
				else
				{

					do
					{
						findTime.ColorChange();
						findX.ColorChange();
						findY.ColorChange();

						x = Math.Round(FindX(speed = double.Parse(textBox1.Text), angle, time), 1);
						y = Math.Round(FindY(speed = double.Parse(textBox1.Text), angle, time), 1);

						chart1.Series[countSeries].Points.AddXY(x, y);

						time += 0.1;

					} while (!(y <= 0 && x != 0));

					angle += 5;

					string maxY = chart1.Series[countSeries].Points.FindMaxByValue("Y1", 1).ToString();
					int countMaxY = 0;
					foreach (var item in maxY)
					{
						if (item != 'Y')
						{
							countMaxY++;

						}
						else
						{
							break;
						}
					}
					findY.Text = maxY.Substring(countMaxY).Replace("}", "");
					findX.Text = "X=" + x.ToString();
					findTime.Text = Math.Round(time).ToString();

					tableLayoutPanel1.Controls.Add(findTime, 0, count);
					tableLayoutPanel1.Controls.Add(findX, 1, count);
					tableLayoutPanel1.Controls.Add(findY, 2, count);
					count++;
					countSeries++;
					time = 0;


				}
			}

		}
	}
}
