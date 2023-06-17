using System;

// Token: 0x02000150 RID: 336
public class MenuButtonArcadeEntity : TextArcadeEntity
{
	// Token: 0x04000FBB RID: 4027
	public string titleText = "";

	// Token: 0x04000FBC RID: 4028
	public string selectionSuffix = " - ";

	// Token: 0x04000FBD RID: 4029
	public string clickMessage = "";

	// Token: 0x06001714 RID: 5908 RVA: 0x000B0261 File Offset: 0x000AE461
	public bool IsHighlighted()
	{
		return this.alpha == 1f;
	}
}
