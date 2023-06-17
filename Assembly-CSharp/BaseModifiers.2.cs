using System;
using System.Collections.Generic;
using ConVar;
using Rust;
using UnityEngine;

// Token: 0x02000430 RID: 1072
public abstract class BaseModifiers<T> : EntityComponent<T> where T : BaseCombatEntity
{
	// Token: 0x04001C32 RID: 7218
	public List<Modifier> All = new List<Modifier>();

	// Token: 0x04001C33 RID: 7219
	protected Dictionary<Modifier.ModifierType, float> totalValues = new Dictionary<Modifier.ModifierType, float>();

	// Token: 0x04001C34 RID: 7220
	protected Dictionary<Modifier.ModifierType, float> modifierVariables = new Dictionary<Modifier.ModifierType, float>();

	// Token: 0x04001C35 RID: 7221
	protected T owner;

	// Token: 0x04001C36 RID: 7222
	protected bool dirty = true;

	// Token: 0x04001C37 RID: 7223
	protected float timeSinceLastTick;

	// Token: 0x04001C38 RID: 7224
	protected float lastTickTime;

	// Token: 0x170002F7 RID: 759
	// (get) Token: 0x0600241D RID: 9245 RVA: 0x000E6955 File Offset: 0x000E4B55
	public int ActiveModifierCoount
	{
		get
		{
			return this.All.Count;
		}
	}

	// Token: 0x0600241E RID: 9246 RVA: 0x000E6964 File Offset: 0x000E4B64
	public void Add(List<ModifierDefintion> modDefs)
	{
		foreach (ModifierDefintion def in modDefs)
		{
			this.Add(def);
		}
	}

	// Token: 0x0600241F RID: 9247 RVA: 0x000E69B4 File Offset: 0x000E4BB4
	protected void Add(ModifierDefintion def)
	{
		Modifier modifier = new Modifier();
		modifier.Init(def.type, def.source, def.value, def.duration, def.duration);
		this.Add(modifier);
	}

	// Token: 0x06002420 RID: 9248 RVA: 0x000E69F4 File Offset: 0x000E4BF4
	protected void Add(Modifier modifier)
	{
		if (!this.CanAdd(modifier))
		{
			return;
		}
		int maxModifiersForSourceType = this.GetMaxModifiersForSourceType(modifier.Source);
		if (this.GetTypeSourceCount(modifier.Type, modifier.Source) >= maxModifiersForSourceType)
		{
			Modifier shortestLifeModifier = this.GetShortestLifeModifier(modifier.Type, modifier.Source);
			if (shortestLifeModifier == null)
			{
				return;
			}
			this.Remove(shortestLifeModifier);
		}
		this.All.Add(modifier);
		if (!this.totalValues.ContainsKey(modifier.Type))
		{
			this.totalValues.Add(modifier.Type, modifier.Value);
		}
		else
		{
			Dictionary<Modifier.ModifierType, float> dictionary = this.totalValues;
			Modifier.ModifierType type = modifier.Type;
			dictionary[type] += modifier.Value;
		}
		this.SetDirty(true);
	}

	// Token: 0x06002421 RID: 9249 RVA: 0x000E6AAD File Offset: 0x000E4CAD
	private bool CanAdd(Modifier modifier)
	{
		return !this.All.Contains(modifier);
	}

	// Token: 0x06002422 RID: 9250 RVA: 0x000E6AC0 File Offset: 0x000E4CC0
	private int GetMaxModifiersForSourceType(Modifier.ModifierSource source)
	{
		if (source == Modifier.ModifierSource.Tea)
		{
			return 1;
		}
		return int.MaxValue;
	}

	// Token: 0x06002423 RID: 9251 RVA: 0x000E6ACC File Offset: 0x000E4CCC
	private int GetTypeSourceCount(Modifier.ModifierType type, Modifier.ModifierSource source)
	{
		int num = 0;
		foreach (Modifier modifier in this.All)
		{
			if (modifier.Type == type && modifier.Source == source)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x06002424 RID: 9252 RVA: 0x000E6B34 File Offset: 0x000E4D34
	private Modifier GetShortestLifeModifier(Modifier.ModifierType type, Modifier.ModifierSource source)
	{
		Modifier modifier = null;
		foreach (Modifier modifier2 in this.All)
		{
			if (modifier2.Type == type && modifier2.Source == source)
			{
				if (modifier == null)
				{
					modifier = modifier2;
				}
				else if (modifier2.TimeRemaining < modifier.TimeRemaining)
				{
					modifier = modifier2;
				}
			}
		}
		return modifier;
	}

	// Token: 0x06002425 RID: 9253 RVA: 0x000E6BAC File Offset: 0x000E4DAC
	private void Remove(Modifier modifier)
	{
		if (!this.All.Contains(modifier))
		{
			return;
		}
		this.All.Remove(modifier);
		Dictionary<Modifier.ModifierType, float> dictionary = this.totalValues;
		Modifier.ModifierType type = modifier.Type;
		dictionary[type] -= modifier.Value;
		this.SetDirty(true);
	}

	// Token: 0x06002426 RID: 9254 RVA: 0x000E6BFF File Offset: 0x000E4DFF
	public void RemoveAll()
	{
		this.All.Clear();
		this.totalValues.Clear();
		this.SetDirty(true);
	}

	// Token: 0x06002427 RID: 9255 RVA: 0x000E6C20 File Offset: 0x000E4E20
	public float GetValue(Modifier.ModifierType type, float defaultValue = 0f)
	{
		float result;
		if (this.totalValues.TryGetValue(type, out result))
		{
			return result;
		}
		return defaultValue;
	}

	// Token: 0x06002428 RID: 9256 RVA: 0x000E6C40 File Offset: 0x000E4E40
	public float GetVariableValue(Modifier.ModifierType type, float defaultValue)
	{
		float result;
		if (this.modifierVariables.TryGetValue(type, out result))
		{
			return result;
		}
		return defaultValue;
	}

	// Token: 0x06002429 RID: 9257 RVA: 0x000E6C60 File Offset: 0x000E4E60
	public void SetVariableValue(Modifier.ModifierType type, float value)
	{
		float num;
		if (this.modifierVariables.TryGetValue(type, out num))
		{
			this.modifierVariables[type] = value;
			return;
		}
		this.modifierVariables.Add(type, value);
	}

	// Token: 0x0600242A RID: 9258 RVA: 0x000E6C98 File Offset: 0x000E4E98
	public void RemoveVariable(Modifier.ModifierType type)
	{
		this.modifierVariables.Remove(type);
	}

	// Token: 0x0600242B RID: 9259 RVA: 0x000E6CA7 File Offset: 0x000E4EA7
	protected virtual void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		this.owner = default(T);
	}

	// Token: 0x0600242C RID: 9260 RVA: 0x000E6CBD File Offset: 0x000E4EBD
	protected void SetDirty(bool flag)
	{
		this.dirty = flag;
	}

	// Token: 0x0600242D RID: 9261 RVA: 0x000E6CC6 File Offset: 0x000E4EC6
	public virtual void ServerInit(T owner)
	{
		this.owner = owner;
		this.ResetTicking();
		this.RemoveAll();
	}

	// Token: 0x0600242E RID: 9262 RVA: 0x000E6CDB File Offset: 0x000E4EDB
	public void ResetTicking()
	{
		this.lastTickTime = UnityEngine.Time.realtimeSinceStartup;
		this.timeSinceLastTick = 0f;
	}

	// Token: 0x0600242F RID: 9263 RVA: 0x000E6CF4 File Offset: 0x000E4EF4
	public virtual void ServerUpdate(BaseCombatEntity ownerEntity)
	{
		float num = UnityEngine.Time.realtimeSinceStartup - this.lastTickTime;
		this.lastTickTime = UnityEngine.Time.realtimeSinceStartup;
		this.timeSinceLastTick += num;
		if (this.timeSinceLastTick <= ConVar.Server.modifierTickRate)
		{
			return;
		}
		if (this.owner != null && !this.owner.IsDead())
		{
			this.TickModifiers(ownerEntity, this.timeSinceLastTick);
		}
		this.timeSinceLastTick = 0f;
	}

	// Token: 0x06002430 RID: 9264 RVA: 0x000E6D74 File Offset: 0x000E4F74
	protected virtual void TickModifiers(BaseCombatEntity ownerEntity, float delta)
	{
		for (int i = this.All.Count - 1; i >= 0; i--)
		{
			Modifier modifier = this.All[i];
			modifier.Tick(ownerEntity, delta);
			if (modifier.Expired)
			{
				this.Remove(modifier);
			}
		}
	}
}
