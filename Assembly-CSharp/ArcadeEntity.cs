using System;
using UnityEngine;

// Token: 0x02000145 RID: 325
public class ArcadeEntity : BaseMonoBehaviour
{
	// Token: 0x04000F67 RID: 3943
	public uint id;

	// Token: 0x04000F68 RID: 3944
	public uint spriteID;

	// Token: 0x04000F69 RID: 3945
	public uint soundID;

	// Token: 0x04000F6A RID: 3946
	public bool visible;

	// Token: 0x04000F6B RID: 3947
	public Vector3 heading = new Vector3(0f, 1f, 0f);

	// Token: 0x04000F6C RID: 3948
	public bool isEnabled;

	// Token: 0x04000F6D RID: 3949
	public bool dirty;

	// Token: 0x04000F6E RID: 3950
	public float alpha = 1f;

	// Token: 0x04000F6F RID: 3951
	public BoxCollider boxCollider;

	// Token: 0x04000F70 RID: 3952
	public bool host;

	// Token: 0x04000F71 RID: 3953
	public bool localAuthorativeOverride;

	// Token: 0x04000F72 RID: 3954
	public ArcadeEntity arcadeEntityParent;

	// Token: 0x04000F73 RID: 3955
	public uint prefabID;

	// Token: 0x04000F74 RID: 3956
	[Header("Health")]
	public bool takesDamage;

	// Token: 0x04000F75 RID: 3957
	public float health = 1f;

	// Token: 0x04000F76 RID: 3958
	public float maxHealth = 1f;

	// Token: 0x04000F77 RID: 3959
	[NonSerialized]
	public bool mapLoadedEntiy;
}
