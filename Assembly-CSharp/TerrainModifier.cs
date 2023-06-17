using System;
using UnityEngine;

// Token: 0x020006EE RID: 1774
public abstract class TerrainModifier : PrefabAttribute
{
	// Token: 0x040028D3 RID: 10451
	public float Opacity = 1f;

	// Token: 0x040028D4 RID: 10452
	public float Radius;

	// Token: 0x040028D5 RID: 10453
	public float Fade;

	// Token: 0x06003217 RID: 12823 RVA: 0x00134CCC File Offset: 0x00132ECC
	public void Apply(Vector3 pos, float scale)
	{
		float opacity = this.Opacity;
		float radius = scale * this.Radius;
		float fade = scale * this.Fade;
		this.Apply(pos, opacity, radius, fade);
	}

	// Token: 0x06003218 RID: 12824
	protected abstract void Apply(Vector3 position, float opacity, float radius, float fade);

	// Token: 0x06003219 RID: 12825 RVA: 0x00134CFC File Offset: 0x00132EFC
	protected override Type GetIndexedType()
	{
		return typeof(TerrainModifier);
	}
}
