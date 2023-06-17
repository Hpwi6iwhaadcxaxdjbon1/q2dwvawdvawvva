using System;
using System.Collections.Generic;
using Rust.Ai;
using UnityEngine;

// Token: 0x02000422 RID: 1058
public class SmokeGrenade : TimedExplosive
{
	// Token: 0x04001BF7 RID: 7159
	public float smokeDuration = 45f;

	// Token: 0x04001BF8 RID: 7160
	public GameObjectRef smokeEffectPrefab;

	// Token: 0x04001BF9 RID: 7161
	public GameObjectRef igniteSound;

	// Token: 0x04001BFA RID: 7162
	public SoundPlayer soundLoop;

	// Token: 0x04001BFB RID: 7163
	private GameObject smokeEffectInstance;

	// Token: 0x04001BFC RID: 7164
	public static List<SmokeGrenade> activeGrenades = new List<SmokeGrenade>();

	// Token: 0x04001BFD RID: 7165
	public float fieldMin = 5f;

	// Token: 0x04001BFE RID: 7166
	public float fieldMax = 8f;

	// Token: 0x04001BFF RID: 7167
	protected bool killing;

	// Token: 0x060023CE RID: 9166 RVA: 0x000E501F File Offset: 0x000E321F
	public override void ServerInit()
	{
		base.ServerInit();
		base.InvokeRepeating(new Action(this.CheckForWater), 1f, 1f);
	}

	// Token: 0x060023CF RID: 9167 RVA: 0x000E5044 File Offset: 0x000E3244
	public override void Explode()
	{
		if (this.WaterFactor() >= 0.5f)
		{
			this.FinishUp();
			return;
		}
		if (base.IsOn())
		{
			return;
		}
		base.Invoke(new Action(this.FinishUp), this.smokeDuration);
		base.SetFlag(BaseEntity.Flags.On, true, false, true);
		base.SetFlag(BaseEntity.Flags.Open, true, false, true);
		base.InvalidateNetworkCache();
		base.SendNetworkUpdateImmediate(false);
		SmokeGrenade.activeGrenades.Add(this);
		if (this.creatorEntity)
		{
			Sense.Stimulate(new Sensation
			{
				Type = SensationType.Explosion,
				Position = this.creatorEntity.transform.position,
				Radius = this.explosionRadius * 17f,
				DamagePotential = 0f,
				InitiatorPlayer = (this.creatorEntity as BasePlayer),
				Initiator = this.creatorEntity
			});
		}
	}

	// Token: 0x060023D0 RID: 9168 RVA: 0x000E512A File Offset: 0x000E332A
	public void CheckForWater()
	{
		if (this.WaterFactor() >= 0.5f)
		{
			this.FinishUp();
		}
	}

	// Token: 0x060023D1 RID: 9169 RVA: 0x000E513F File Offset: 0x000E333F
	public void FinishUp()
	{
		if (this.killing)
		{
			return;
		}
		base.Kill(BaseNetworkable.DestroyMode.None);
		this.killing = true;
	}

	// Token: 0x060023D2 RID: 9170 RVA: 0x000E5158 File Offset: 0x000E3358
	public override void DestroyShared()
	{
		SmokeGrenade.activeGrenades.Remove(this);
		base.DestroyShared();
	}
}
