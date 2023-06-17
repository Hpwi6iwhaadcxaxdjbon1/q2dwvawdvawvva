using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

// Token: 0x02000593 RID: 1427
public class TriggerPlayerTimer : TriggerBase
{
	// Token: 0x04002341 RID: 9025
	public BaseEntity TargetEntity;

	// Token: 0x04002342 RID: 9026
	public float DamageAmount = 20f;

	// Token: 0x04002343 RID: 9027
	public float TimeToDamage = 3f;

	// Token: 0x06002B88 RID: 11144 RVA: 0x00107DA0 File Offset: 0x00105FA0
	internal override GameObject InterestedInObject(GameObject obj)
	{
		obj = base.InterestedInObject(obj);
		if (obj != null)
		{
			BaseEntity baseEntity = obj.ToBaseEntity();
			BasePlayer basePlayer;
			if ((basePlayer = (baseEntity as BasePlayer)) != null && baseEntity.isServer && !basePlayer.isMounted)
			{
				return baseEntity.gameObject;
			}
		}
		return obj;
	}

	// Token: 0x06002B89 RID: 11145 RVA: 0x00107DE8 File Offset: 0x00105FE8
	internal override void OnObjects()
	{
		base.OnObjects();
		base.Invoke(new Action(this.DamageTarget), this.TimeToDamage);
	}

	// Token: 0x06002B8A RID: 11146 RVA: 0x00107E08 File Offset: 0x00106008
	internal override void OnEmpty()
	{
		base.OnEmpty();
		base.CancelInvoke(new Action(this.DamageTarget));
	}

	// Token: 0x06002B8B RID: 11147 RVA: 0x00107E24 File Offset: 0x00106024
	private void DamageTarget()
	{
		bool flag = false;
		using (HashSet<BaseEntity>.Enumerator enumerator = this.entityContents.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				BasePlayer basePlayer;
				if ((basePlayer = (enumerator.Current as BasePlayer)) != null && !basePlayer.isMounted)
				{
					flag = true;
				}
			}
		}
		if (flag && this.TargetEntity != null)
		{
			this.TargetEntity.OnAttacked(new HitInfo(null, this.TargetEntity, DamageType.Generic, this.DamageAmount));
		}
		base.Invoke(new Action(this.DamageTarget), this.TimeToDamage);
	}
}
