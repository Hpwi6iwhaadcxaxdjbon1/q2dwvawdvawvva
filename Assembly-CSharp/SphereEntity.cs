using System;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000423 RID: 1059
public class SphereEntity : global::BaseEntity
{
	// Token: 0x04001C00 RID: 7168
	public float currentRadius = 1f;

	// Token: 0x04001C01 RID: 7169
	public float lerpRadius = 1f;

	// Token: 0x04001C02 RID: 7170
	public float lerpSpeed = 1f;

	// Token: 0x060023D5 RID: 9173 RVA: 0x000E51A1 File Offset: 0x000E33A1
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.sphereEntity = Pool.Get<ProtoBuf.SphereEntity>();
		info.msg.sphereEntity.radius = this.currentRadius;
	}

	// Token: 0x060023D6 RID: 9174 RVA: 0x000E51D0 File Offset: 0x000E33D0
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (base.isServer)
		{
			if (info.msg.sphereEntity != null)
			{
				this.currentRadius = (this.lerpRadius = info.msg.sphereEntity.radius);
			}
			this.UpdateScale();
		}
	}

	// Token: 0x060023D7 RID: 9175 RVA: 0x000E521E File Offset: 0x000E341E
	public void LerpRadiusTo(float radius, float speed)
	{
		this.lerpRadius = radius;
		this.lerpSpeed = speed;
	}

	// Token: 0x060023D8 RID: 9176 RVA: 0x000E522E File Offset: 0x000E342E
	protected void UpdateScale()
	{
		base.transform.localScale = new Vector3(this.currentRadius, this.currentRadius, this.currentRadius);
	}

	// Token: 0x060023D9 RID: 9177 RVA: 0x000E5254 File Offset: 0x000E3454
	protected void Update()
	{
		if (this.currentRadius == this.lerpRadius)
		{
			return;
		}
		if (base.isServer)
		{
			this.currentRadius = Mathf.MoveTowards(this.currentRadius, this.lerpRadius, Time.deltaTime * this.lerpSpeed);
			this.UpdateScale();
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}
}
