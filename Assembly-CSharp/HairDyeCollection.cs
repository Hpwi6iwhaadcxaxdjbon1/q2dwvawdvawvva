using System;
using UnityEngine;

// Token: 0x0200074D RID: 1869
[CreateAssetMenu(menuName = "Rust/Hair Dye Collection")]
public class HairDyeCollection : ScriptableObject
{
	// Token: 0x04002A67 RID: 10855
	public Texture capMask;

	// Token: 0x04002A68 RID: 10856
	public bool applyCap;

	// Token: 0x04002A69 RID: 10857
	public HairDye[] Variations;

	// Token: 0x0600345F RID: 13407 RVA: 0x001447A4 File Offset: 0x001429A4
	public HairDye Get(float seed)
	{
		if (this.Variations.Length != 0)
		{
			return this.Variations[Mathf.Clamp(Mathf.FloorToInt(seed * (float)this.Variations.Length), 0, this.Variations.Length - 1)];
		}
		return null;
	}
}
