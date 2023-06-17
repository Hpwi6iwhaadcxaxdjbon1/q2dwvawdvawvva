using System;
using UnityEngine;

// Token: 0x020002FC RID: 764
public class Buttons
{
	// Token: 0x02000CA1 RID: 3233
	public class ConButton : ConsoleSystem.IConsoleButton
	{
		// Token: 0x04004423 RID: 17443
		private int frame;

		// Token: 0x17000699 RID: 1689
		// (get) Token: 0x06004F3F RID: 20287 RVA: 0x001A5F63 File Offset: 0x001A4163
		// (set) Token: 0x06004F40 RID: 20288 RVA: 0x001A5F6B File Offset: 0x001A416B
		public bool IsDown { get; set; }

		// Token: 0x1700069A RID: 1690
		// (get) Token: 0x06004F41 RID: 20289 RVA: 0x001A5F74 File Offset: 0x001A4174
		public bool JustPressed
		{
			get
			{
				return this.IsDown && this.frame == Time.frameCount;
			}
		}

		// Token: 0x1700069B RID: 1691
		// (get) Token: 0x06004F42 RID: 20290 RVA: 0x001A5F8D File Offset: 0x001A418D
		public bool JustReleased
		{
			get
			{
				return !this.IsDown && this.frame == Time.frameCount;
			}
		}

		// Token: 0x06004F43 RID: 20291 RVA: 0x000063A5 File Offset: 0x000045A5
		public void Call(ConsoleSystem.Arg arg)
		{
		}

		// Token: 0x1700069C RID: 1692
		// (get) Token: 0x06004F44 RID: 20292 RVA: 0x001A5FA6 File Offset: 0x001A41A6
		// (set) Token: 0x06004F45 RID: 20293 RVA: 0x001A5FAE File Offset: 0x001A41AE
		public bool IsPressed
		{
			get
			{
				return this.IsDown;
			}
			set
			{
				if (value == this.IsDown)
				{
					return;
				}
				this.IsDown = value;
				this.frame = Time.frameCount;
			}
		}
	}
}
