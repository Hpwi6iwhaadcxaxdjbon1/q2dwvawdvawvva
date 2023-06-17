using System;
using UnityEngine;

// Token: 0x0200054B RID: 1355
public class LevelInfo : SingletonComponent<LevelInfo>
{
	// Token: 0x0400221E RID: 8734
	public string shortName;

	// Token: 0x0400221F RID: 8735
	public string displayName;

	// Token: 0x04002220 RID: 8736
	[TextArea]
	public string description;

	// Token: 0x04002221 RID: 8737
	[Tooltip("A background image to be shown when loading the map")]
	public Texture2D image;

	// Token: 0x04002222 RID: 8738
	[Space(10f)]
	[Tooltip("You should incrememnt this version when you make changes to the map that will invalidate old saves")]
	public int version = 1;
}
