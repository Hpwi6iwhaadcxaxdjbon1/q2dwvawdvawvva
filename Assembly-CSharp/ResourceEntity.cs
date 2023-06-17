using System;
using Facepunch;
using ProtoBuf;
using UnityEngine.Serialization;

// Token: 0x0200045C RID: 1116
public class ResourceEntity : global::BaseEntity
{
	// Token: 0x04001D45 RID: 7493
	[FormerlySerializedAs("health")]
	public float startHealth;

	// Token: 0x04001D46 RID: 7494
	[FormerlySerializedAs("protection")]
	public ProtectionProperties baseProtection;

	// Token: 0x04001D47 RID: 7495
	protected float health;

	// Token: 0x04001D48 RID: 7496
	internal ResourceDispenser resourceDispenser;

	// Token: 0x04001D49 RID: 7497
	[NonSerialized]
	protected bool isKilled;

	// Token: 0x060024E5 RID: 9445 RVA: 0x000E999B File Offset: 0x000E7B9B
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.resource == null)
		{
			return;
		}
		this.health = info.msg.resource.health;
	}

	// Token: 0x060024E6 RID: 9446 RVA: 0x000E99C8 File Offset: 0x000E7BC8
	public override void InitShared()
	{
		base.InitShared();
		if (base.isServer)
		{
			DecorComponent[] components = PrefabAttribute.server.FindAll<DecorComponent>(this.prefabID);
			base.transform.ApplyDecorComponentsScaleOnly(components);
		}
	}

	// Token: 0x060024E7 RID: 9447 RVA: 0x000E9A00 File Offset: 0x000E7C00
	public override void ServerInit()
	{
		base.ServerInit();
		this.resourceDispenser = base.GetComponent<ResourceDispenser>();
		if (this.health == 0f)
		{
			this.health = this.startHealth;
		}
	}

	// Token: 0x060024E8 RID: 9448 RVA: 0x000E9A2D File Offset: 0x000E7C2D
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.forDisk)
		{
			info.msg.resource = Pool.Get<BaseResource>();
			info.msg.resource.health = this.Health();
		}
	}

	// Token: 0x060024E9 RID: 9449 RVA: 0x000E9A64 File Offset: 0x000E7C64
	public override float MaxHealth()
	{
		return this.startHealth;
	}

	// Token: 0x060024EA RID: 9450 RVA: 0x000E9A6C File Offset: 0x000E7C6C
	public override float Health()
	{
		return this.health;
	}

	// Token: 0x060024EB RID: 9451 RVA: 0x000063A5 File Offset: 0x000045A5
	protected virtual void OnHealthChanged()
	{
	}

	// Token: 0x060024EC RID: 9452 RVA: 0x000E9A74 File Offset: 0x000E7C74
	public override void OnAttacked(HitInfo info)
	{
		if (base.isServer && !this.isKilled)
		{
			if (this.resourceDispenser != null)
			{
				this.resourceDispenser.OnAttacked(info);
			}
			if (!info.DidGather)
			{
				if (this.baseProtection)
				{
					this.baseProtection.Scale(info.damageTypes, 1f);
				}
				float num = info.damageTypes.Total();
				this.health -= num;
				if (this.health <= 0f)
				{
					this.OnKilled(info);
					return;
				}
				this.OnHealthChanged();
			}
		}
	}

	// Token: 0x060024ED RID: 9453 RVA: 0x000E9B0E File Offset: 0x000E7D0E
	public virtual void OnKilled(HitInfo info)
	{
		this.isKilled = true;
		base.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x060024EE RID: 9454 RVA: 0x00006CA5 File Offset: 0x00004EA5
	public override float BoundsPadding()
	{
		return 1f;
	}
}
