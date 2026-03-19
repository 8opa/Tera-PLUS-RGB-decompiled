using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Tera_RBG
{
	// Token: 0x02000002 RID: 2
	public partial class Form1 : Form
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public Form1()
		{
			this.InitializeComponent();
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002068 File Offset: 0x00000268
		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = true;
		}

		// Token: 0x06000003 RID: 3 RVA: 0x00002073 File Offset: 0x00000273
		private void pictureBox1_Click(object sender, EventArgs e)
		{
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002078 File Offset: 0x00000278
		private void timer1_Tick(object sender, EventArgs e)
		{
			Random random = new Random();
			int width = base.Width;
			int height = base.Height;
			int[] array = new int[width * height];
			for (int i = 0; i < array.Length; i++)
			{
				this.ti += 1.0;
				int num = (int)this.ti;
				array[i] = (int)((double)((num & (num / 1) & (num / 2)) * num) / 1000.0 % 2.0);
			}
			Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppRgb);
			BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
			Marshal.Copy(array, 0, bitmapData.Scan0, array.Length);
			bitmap.UnlockBits(bitmapData);
			this.pictureBox1.Image = bitmap;
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002163 File Offset: 0x00000363
		private void pictureBox2_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Enter Text Here", "لKnKr₨₣₵Q₣∄∊∌∐∑≯≲ⅻↅ◀▲◢⧩⨞＄П.سKnK.سr₨₣₵Q₣∄∊∌∐∑≯≲ⅻↅ◀▲◢⧩⨞＄Пل.سKnKr₨₣₵Q₣∄∊∌∐∑≯≲ⅻↅ◀▲◢⧩⨞＄ПnKr₨₣₵Q₣∄∊∌∐∑≯≲ⅻↅ◀▲◢⧩⨞＄П", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}

		// Token: 0x04000001 RID: 1
		private double ti;
	}
}
