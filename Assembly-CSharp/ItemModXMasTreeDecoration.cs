using System;

// Token: 0x0200018B RID: 395
public class ItemModXMasTreeDecoration : ItemMod
{
	// Token: 0x040010A4 RID: 4260
	public ItemModXMasTreeDecoration.xmasFlags flagsToChange;

	// Token: 0x02000C2E RID: 3118
	public enum xmasFlags
	{
		// Token: 0x04004268 RID: 17000
		pineCones = 128,
		// Token: 0x04004269 RID: 17001
		candyCanes = 256,
		// Token: 0x0400426A RID: 17002
		gingerbreadMen = 512,
		// Token: 0x0400426B RID: 17003
		Tinsel = 1024,
		// Token: 0x0400426C RID: 17004
		Balls = 2048,
		// Token: 0x0400426D RID: 17005
		Star = 16384,
		// Token: 0x0400426E RID: 17006
		Lights = 32768
	}
}
