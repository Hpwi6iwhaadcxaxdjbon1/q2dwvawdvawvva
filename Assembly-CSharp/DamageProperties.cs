using System;
using UnityEngine;

// Token: 0x0200055A RID: 1370
[CreateAssetMenu(menuName = "Rust/Damage Properties")]
public class DamageProperties : ScriptableObject
{
	// Token: 0x04002260 RID: 8800
	public DamageProperties fallback;

	// Token: 0x04002261 RID: 8801
	[Horizontal(1, 0)]
	public DamageProperties.HitAreaProperty[] bones;

	// Token: 0x06002A3B RID: 10811 RVA: 0x00101580 File Offset: 0x000FF780
	public float GetMultiplier(HitArea area)
	{
		for (int i = 0; i < this.bones.Length; i++)
		{
			DamageProperties.HitAreaProperty hitAreaProperty = this.bones[i];
			if (hitAreaProperty.area == area)
			{
				return hitAreaProperty.damage;
			}
		}
		if (!this.fallback)
		{
			return 1f;
		}
		return this.fallback.GetMultiplier(area);
	}

	// Token: 0x06002A3C RID: 10812 RVA: 0x001015D8 File Offset: 0x000FF7D8
	public void ScaleDamage(HitInfo info)
	{
		HitArea boneArea = info.boneArea;
		if (boneArea == (HitArea)(-1) || boneArea == (HitArea)0)
		{
			return;
		}
		info.damageTypes.ScaleAll(this.GetMultiplier(boneArea));
	}

	// Token: 0x02000D46 RID: 3398
	[Serializable]
	public class HitAreaProperty
	{
		// Token: 0x040046CD RID: 18125
		public HitArea area = HitArea.Head;

		// Token: 0x040046CE RID: 18126
		public float damage = 1f;
	}
}
