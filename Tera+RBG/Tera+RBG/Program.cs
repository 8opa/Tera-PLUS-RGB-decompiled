using System;
using System.Windows.Forms;

namespace Tera_RBG
{
	// Token: 0x02000006 RID: 6
	internal static class Program
	{
		// Token: 0x06000056 RID: 86 RVA: 0x00004C75 File Offset: 0x00002E75
		[STAThread]
		private static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Form1());
		}
	}
}
