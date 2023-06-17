using System;
using Facepunch;
using ProtoBuf;

// Token: 0x02000432 RID: 1074
public class Modifier
{
	// Token: 0x170002F8 RID: 760
	// (get) Token: 0x06002433 RID: 9267 RVA: 0x000E6DED File Offset: 0x000E4FED
	// (set) Token: 0x06002434 RID: 9268 RVA: 0x000E6DF5 File Offset: 0x000E4FF5
	public global::Modifier.ModifierType Type { get; private set; }

	// Token: 0x170002F9 RID: 761
	// (get) Token: 0x06002435 RID: 9269 RVA: 0x000E6DFE File Offset: 0x000E4FFE
	// (set) Token: 0x06002436 RID: 9270 RVA: 0x000E6E06 File Offset: 0x000E5006
	public global::Modifier.ModifierSource Source { get; private set; }

	// Token: 0x170002FA RID: 762
	// (get) Token: 0x06002437 RID: 9271 RVA: 0x000E6E0F File Offset: 0x000E500F
	// (set) Token: 0x06002438 RID: 9272 RVA: 0x000E6E17 File Offset: 0x000E5017
	public float Value { get; private set; } = 1f;

	// Token: 0x170002FB RID: 763
	// (get) Token: 0x06002439 RID: 9273 RVA: 0x000E6E20 File Offset: 0x000E5020
	// (set) Token: 0x0600243A RID: 9274 RVA: 0x000E6E28 File Offset: 0x000E5028
	public float Duration { get; private set; } = 10f;

	// Token: 0x170002FC RID: 764
	// (get) Token: 0x0600243B RID: 9275 RVA: 0x000E6E31 File Offset: 0x000E5031
	// (set) Token: 0x0600243C RID: 9276 RVA: 0x000E6E39 File Offset: 0x000E5039
	public float TimeRemaining { get; private set; }

	// Token: 0x170002FD RID: 765
	// (get) Token: 0x0600243D RID: 9277 RVA: 0x000E6E42 File Offset: 0x000E5042
	// (set) Token: 0x0600243E RID: 9278 RVA: 0x000E6E4A File Offset: 0x000E504A
	public bool Expired { get; private set; }

	// Token: 0x0600243F RID: 9279 RVA: 0x000E6E53 File Offset: 0x000E5053
	public void Init(global::Modifier.ModifierType type, global::Modifier.ModifierSource source, float value, float duration, float remaining)
	{
		this.Type = type;
		this.Source = source;
		this.Value = value;
		this.Duration = duration;
		this.Expired = false;
		this.TimeRemaining = remaining;
	}

	// Token: 0x06002440 RID: 9280 RVA: 0x000E6E81 File Offset: 0x000E5081
	public void Tick(BaseCombatEntity ownerEntity, float delta)
	{
		this.TimeRemaining -= delta;
		this.Expired = (this.TimeRemaining <= 0f);
	}

	// Token: 0x06002441 RID: 9281 RVA: 0x000E6EA7 File Offset: 0x000E50A7
	public ProtoBuf.Modifier Save()
	{
		ProtoBuf.Modifier modifier = Pool.Get<ProtoBuf.Modifier>();
		modifier.type = (int)this.Type;
		modifier.source = (int)this.Source;
		modifier.value = this.Value;
		modifier.timeRemaing = this.TimeRemaining;
		return modifier;
	}

	// Token: 0x06002442 RID: 9282 RVA: 0x000E6EDE File Offset: 0x000E50DE
	public void Load(ProtoBuf.Modifier m)
	{
		this.Type = (global::Modifier.ModifierType)m.type;
		this.Source = (global::Modifier.ModifierSource)m.source;
		this.Value = m.value;
		this.TimeRemaining = m.timeRemaing;
	}

	// Token: 0x02000CE0 RID: 3296
	public enum ModifierType
	{
		// Token: 0x04004544 RID: 17732
		Wood_Yield,
		// Token: 0x04004545 RID: 17733
		Ore_Yield,
		// Token: 0x04004546 RID: 17734
		Radiation_Resistance,
		// Token: 0x04004547 RID: 17735
		Radiation_Exposure_Resistance,
		// Token: 0x04004548 RID: 17736
		Max_Health,
		// Token: 0x04004549 RID: 17737
		Scrap_Yield
	}

	// Token: 0x02000CE1 RID: 3297
	public enum ModifierSource
	{
		// Token: 0x0400454B RID: 17739
		Tea
	}
}
