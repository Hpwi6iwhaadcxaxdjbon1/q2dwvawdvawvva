using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000749 RID: 1865
[CreateAssetMenu(menuName = "Rust/Building Grade")]
public class BuildingGrade : ScriptableObject
{
	// Token: 0x04002A49 RID: 10825
	public BuildingGrade.Enum type;

	// Token: 0x04002A4A RID: 10826
	public ulong skin;

	// Token: 0x04002A4B RID: 10827
	public bool enabledInStandalone;

	// Token: 0x04002A4C RID: 10828
	public float baseHealth;

	// Token: 0x04002A4D RID: 10829
	public List<ItemAmount> baseCost;

	// Token: 0x04002A4E RID: 10830
	public PhysicMaterial physicMaterial;

	// Token: 0x04002A4F RID: 10831
	public ProtectionProperties damageProtecton;

	// Token: 0x04002A50 RID: 10832
	public bool supportsColourChange;

	// Token: 0x04002A51 RID: 10833
	public BaseEntity.Menu.Option upgradeMenu;

	// Token: 0x02000E55 RID: 3669
	public enum Enum
	{
		// Token: 0x04004B19 RID: 19225
		None = -1,
		// Token: 0x04004B1A RID: 19226
		Twigs,
		// Token: 0x04004B1B RID: 19227
		Wood,
		// Token: 0x04004B1C RID: 19228
		Stone,
		// Token: 0x04004B1D RID: 19229
		Metal,
		// Token: 0x04004B1E RID: 19230
		TopTier,
		// Token: 0x04004B1F RID: 19231
		Count
	}
}
