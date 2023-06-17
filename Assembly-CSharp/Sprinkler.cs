using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using UnityEngine;

// Token: 0x02000122 RID: 290
public class Sprinkler : IOEntity
{
	// Token: 0x04000EA9 RID: 3753
	public float SplashFrequency = 1f;

	// Token: 0x04000EAA RID: 3754
	public Transform Eyes;

	// Token: 0x04000EAB RID: 3755
	public int WaterPerSplash = 1;

	// Token: 0x04000EAC RID: 3756
	public float DecayPerSplash = 0.8f;

	// Token: 0x04000EAD RID: 3757
	private ItemDefinition currentFuelType;

	// Token: 0x04000EAE RID: 3758
	private IOEntity currentFuelSource;

	// Token: 0x04000EAF RID: 3759
	private HashSet<ISplashable> cachedSplashables = new HashSet<ISplashable>();

	// Token: 0x04000EB0 RID: 3760
	private TimeSince updateSplashableCache;

	// Token: 0x04000EB1 RID: 3761
	private bool forceUpdateSplashables;

	// Token: 0x170001F4 RID: 500
	// (get) Token: 0x06001680 RID: 5760 RVA: 0x000ADF3C File Offset: 0x000AC13C
	public override bool BlockFluidDraining
	{
		get
		{
			return this.currentFuelSource != null;
		}
	}

	// Token: 0x06001681 RID: 5761 RVA: 0x0004E73F File Offset: 0x0004C93F
	public override int ConsumptionAmount()
	{
		return 2;
	}

	// Token: 0x06001682 RID: 5762 RVA: 0x000ADF4A File Offset: 0x000AC14A
	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		base.UpdateHasPower(inputAmount, inputSlot);
		this.SetSprinklerState(inputAmount > 0);
	}

	// Token: 0x06001683 RID: 5763 RVA: 0x00036DC0 File Offset: 0x00034FC0
	public override int CalculateCurrentEnergy(int inputAmount, int inputSlot)
	{
		return inputAmount;
	}

	// Token: 0x06001684 RID: 5764 RVA: 0x000ADF60 File Offset: 0x000AC160
	private void DoSplash()
	{
		using (TimeWarning.New("SprinklerSplash", 0))
		{
			int num = this.WaterPerSplash;
			if (this.updateSplashableCache > this.SplashFrequency * 4f || this.forceUpdateSplashables)
			{
				this.cachedSplashables.Clear();
				this.forceUpdateSplashables = false;
				this.updateSplashableCache = 0f;
				Vector3 position = this.Eyes.position;
				Vector3 up = base.transform.up;
				float num2 = Server.sprinklerEyeHeightOffset;
				float num3 = Vector3.Angle(up, Vector3.up) / 180f;
				num3 = Mathf.Clamp(num3, 0.2f, 1f);
				num2 *= num3;
				Vector3 startPosition = position + up * (Server.sprinklerRadius * 0.5f);
				Vector3 endPosition = position + up * num2;
				List<BaseEntity> list = Facepunch.Pool.GetList<BaseEntity>();
				global::Vis.Entities<BaseEntity>(startPosition, endPosition, Server.sprinklerRadius, list, 1236478737, QueryTriggerInteraction.Collide);
				if (list.Count > 0)
				{
					foreach (BaseEntity baseEntity in list)
					{
						ISplashable splashable;
						IOEntity entity;
						if (!baseEntity.isClient && (splashable = (baseEntity as ISplashable)) != null && !this.cachedSplashables.Contains(splashable) && splashable.WantsSplash(this.currentFuelType, num) && baseEntity.IsVisible(position, float.PositiveInfinity) && ((entity = (baseEntity as IOEntity)) == null || !base.IsConnectedTo(entity, IOEntity.backtracking, false)))
						{
							this.cachedSplashables.Add(splashable);
						}
					}
				}
				Facepunch.Pool.FreeList<BaseEntity>(ref list);
			}
			if (this.cachedSplashables.Count > 0)
			{
				int amount = num / this.cachedSplashables.Count;
				foreach (ISplashable splashable2 in this.cachedSplashables)
				{
					if (!splashable2.IsUnityNull<ISplashable>() && splashable2.WantsSplash(this.currentFuelType, amount))
					{
						int num4 = splashable2.DoSplash(this.currentFuelType, amount);
						num -= num4;
						if (num <= 0)
						{
							break;
						}
					}
				}
			}
			if (this.DecayPerSplash > 0f)
			{
				base.Hurt(this.DecayPerSplash);
			}
		}
	}

	// Token: 0x06001685 RID: 5765 RVA: 0x000AE1F8 File Offset: 0x000AC3F8
	public void SetSprinklerState(bool wantsOn)
	{
		if (wantsOn)
		{
			this.TurnOn();
			return;
		}
		this.TurnOff();
	}

	// Token: 0x06001686 RID: 5766 RVA: 0x000AE20C File Offset: 0x000AC40C
	public void TurnOn()
	{
		if (base.IsOn())
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.On, true, false, true);
		this.forceUpdateSplashables = true;
		if (!base.IsInvoking(new Action(this.DoSplash)))
		{
			base.InvokeRandomized(new Action(this.DoSplash), this.SplashFrequency * 0.5f, this.SplashFrequency, this.SplashFrequency * 0.2f);
		}
	}

	// Token: 0x06001687 RID: 5767 RVA: 0x000AE278 File Offset: 0x000AC478
	public void TurnOff()
	{
		if (!base.IsOn())
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
		if (base.IsInvoking(new Action(this.DoSplash)))
		{
			base.CancelInvoke(new Action(this.DoSplash));
		}
		this.currentFuelSource = null;
		this.currentFuelType = null;
	}

	// Token: 0x06001688 RID: 5768 RVA: 0x000AE2CC File Offset: 0x000AC4CC
	public override void SetFuelType(ItemDefinition def, IOEntity source)
	{
		base.SetFuelType(def, source);
		this.currentFuelType = def;
		this.currentFuelSource = source;
	}

	// Token: 0x06001689 RID: 5769 RVA: 0x000AE2E4 File Offset: 0x000AC4E4
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.fromDisk)
		{
			base.SetFlag(BaseEntity.Flags.On, false, false, false);
		}
	}
}
