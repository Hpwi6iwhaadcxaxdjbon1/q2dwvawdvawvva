using System;
using Network;
using Rust;
using UnityEngine;

// Token: 0x020000A5 RID: 165
public class NewYearGong : BaseCombatEntity
{
	// Token: 0x04000A11 RID: 2577
	public SoundDefinition gongSound;

	// Token: 0x04000A12 RID: 2578
	public float minTimeBetweenSounds = 0.25f;

	// Token: 0x04000A13 RID: 2579
	public GameObject soundRoot;

	// Token: 0x04000A14 RID: 2580
	public Transform gongCentre;

	// Token: 0x04000A15 RID: 2581
	public float gongRadius = 1f;

	// Token: 0x04000A16 RID: 2582
	public AnimationCurve pitchCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04000A17 RID: 2583
	public Animator gongAnimator;

	// Token: 0x04000A18 RID: 2584
	private float lastSound;

	// Token: 0x06000F40 RID: 3904 RVA: 0x00080674 File Offset: 0x0007E874
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("NewYearGong.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000F41 RID: 3905 RVA: 0x000806B4 File Offset: 0x0007E8B4
	public override void Hurt(HitInfo info)
	{
		if (!info.damageTypes.IsMeleeType() && !info.damageTypes.Has(DamageType.Bullet) && !info.damageTypes.Has(DamageType.Arrow))
		{
			base.Hurt(info);
			return;
		}
		Vector3 a = this.gongCentre.InverseTransformPoint(info.HitPositionWorld);
		a.z = 0f;
		float num = Vector3.Distance(a, Vector3.zero);
		if (num < this.gongRadius)
		{
			if (Time.time - this.lastSound > this.minTimeBetweenSounds)
			{
				this.lastSound = Time.time;
				base.ClientRPC<float>(null, "PlaySound", Mathf.Clamp01(num / this.gongRadius));
				return;
			}
		}
		else
		{
			base.Hurt(info);
		}
	}
}
