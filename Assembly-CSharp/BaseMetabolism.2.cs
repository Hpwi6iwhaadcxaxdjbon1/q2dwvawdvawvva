using System;
using ConVar;
using Rust;
using UnityEngine;

// Token: 0x020003FF RID: 1023
public abstract class BaseMetabolism<T> : EntityComponent<T> where T : BaseCombatEntity
{
	// Token: 0x04001AE6 RID: 6886
	protected T owner;

	// Token: 0x04001AE7 RID: 6887
	public MetabolismAttribute calories = new MetabolismAttribute();

	// Token: 0x04001AE8 RID: 6888
	public MetabolismAttribute hydration = new MetabolismAttribute();

	// Token: 0x04001AE9 RID: 6889
	public MetabolismAttribute heartrate = new MetabolismAttribute();

	// Token: 0x04001AEA RID: 6890
	protected float timeSinceLastMetabolism;

	// Token: 0x060022D7 RID: 8919 RVA: 0x000DF674 File Offset: 0x000DD874
	public virtual void Reset()
	{
		this.calories.Reset();
		this.hydration.Reset();
		this.heartrate.Reset();
	}

	// Token: 0x060022D8 RID: 8920 RVA: 0x000DF697 File Offset: 0x000DD897
	protected virtual void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		this.owner = default(T);
	}

	// Token: 0x060022D9 RID: 8921 RVA: 0x000DF6AD File Offset: 0x000DD8AD
	public virtual void ServerInit(T owner)
	{
		this.Reset();
		this.owner = owner;
	}

	// Token: 0x060022DA RID: 8922 RVA: 0x000DF6BC File Offset: 0x000DD8BC
	public virtual void ServerUpdate(BaseCombatEntity ownerEntity, float delta)
	{
		this.timeSinceLastMetabolism += delta;
		if (this.timeSinceLastMetabolism <= ConVar.Server.metabolismtick)
		{
			return;
		}
		if (this.owner && !this.owner.IsDead())
		{
			this.RunMetabolism(ownerEntity, this.timeSinceLastMetabolism);
			this.DoMetabolismDamage(ownerEntity, this.timeSinceLastMetabolism);
		}
		this.timeSinceLastMetabolism = 0f;
	}

	// Token: 0x060022DB RID: 8923 RVA: 0x000DF730 File Offset: 0x000DD930
	protected virtual void DoMetabolismDamage(BaseCombatEntity ownerEntity, float delta)
	{
		if (this.calories.value <= 20f)
		{
			using (TimeWarning.New("Calories Hurt", 0))
			{
				ownerEntity.Hurt(Mathf.InverseLerp(20f, 0f, this.calories.value) * delta * 0.083333336f, DamageType.Hunger, null, true);
			}
		}
		if (this.hydration.value <= 20f)
		{
			using (TimeWarning.New("Hyration Hurt", 0))
			{
				ownerEntity.Hurt(Mathf.InverseLerp(20f, 0f, this.hydration.value) * delta * 0.13333334f, DamageType.Thirst, null, true);
			}
		}
	}

	// Token: 0x060022DC RID: 8924 RVA: 0x000DF804 File Offset: 0x000DDA04
	protected virtual void RunMetabolism(BaseCombatEntity ownerEntity, float delta)
	{
		if (this.calories.value > 200f)
		{
			ownerEntity.Heal(Mathf.InverseLerp(200f, 1000f, this.calories.value) * delta * 0.016666668f);
		}
		if (this.hydration.value > 200f)
		{
			ownerEntity.Heal(Mathf.InverseLerp(200f, 1000f, this.hydration.value) * delta * 0.016666668f);
		}
		this.hydration.MoveTowards(0f, delta * 0.008333334f);
		this.calories.MoveTowards(0f, delta * 0.016666668f);
		this.heartrate.MoveTowards(0.05f, delta * 0.016666668f);
	}

	// Token: 0x060022DD RID: 8925 RVA: 0x000DF8CC File Offset: 0x000DDACC
	public void ApplyChange(MetabolismAttribute.Type type, float amount, float time)
	{
		MetabolismAttribute metabolismAttribute = this.FindAttribute(type);
		if (metabolismAttribute == null)
		{
			return;
		}
		metabolismAttribute.Add(amount);
	}

	// Token: 0x060022DE RID: 8926 RVA: 0x000DF8EC File Offset: 0x000DDAEC
	public bool ShouldDie()
	{
		return this.owner && this.owner.Health() <= 0f;
	}

	// Token: 0x060022DF RID: 8927 RVA: 0x000DF91C File Offset: 0x000DDB1C
	public virtual MetabolismAttribute FindAttribute(MetabolismAttribute.Type type)
	{
		switch (type)
		{
		case MetabolismAttribute.Type.Calories:
			return this.calories;
		case MetabolismAttribute.Type.Hydration:
			return this.hydration;
		case MetabolismAttribute.Type.Heartrate:
			return this.heartrate;
		default:
			return null;
		}
	}
}
