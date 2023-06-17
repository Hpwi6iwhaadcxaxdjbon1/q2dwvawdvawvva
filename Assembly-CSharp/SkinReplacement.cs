using System;

// Token: 0x0200075C RID: 1884
[Serializable]
public class SkinReplacement
{
	// Token: 0x04002ABA RID: 10938
	public SkinReplacement.SkinType skinReplacementType;

	// Token: 0x04002ABB RID: 10939
	public GameObjectRef targetReplacement;

	// Token: 0x02000E6C RID: 3692
	public enum SkinType
	{
		// Token: 0x04004B69 RID: 19305
		NONE,
		// Token: 0x04004B6A RID: 19306
		Hands,
		// Token: 0x04004B6B RID: 19307
		Head,
		// Token: 0x04004B6C RID: 19308
		Feet,
		// Token: 0x04004B6D RID: 19309
		Torso,
		// Token: 0x04004B6E RID: 19310
		Legs
	}
}
