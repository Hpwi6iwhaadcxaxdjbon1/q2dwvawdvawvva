using System;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000CD RID: 205
public class SlotMachine : BaseMountable
{
	// Token: 0x04000B64 RID: 2916
	[ServerVar]
	public static int ForcePayoutIndex = -1;

	// Token: 0x04000B65 RID: 2917
	[Header("Slot Machine")]
	public Transform Reel1;

	// Token: 0x04000B66 RID: 2918
	public Transform Reel2;

	// Token: 0x04000B67 RID: 2919
	public Transform Reel3;

	// Token: 0x04000B68 RID: 2920
	public Transform Arm;

	// Token: 0x04000B69 RID: 2921
	public AnimationCurve Curve;

	// Token: 0x04000B6A RID: 2922
	public int Reel1Spins = 16;

	// Token: 0x04000B6B RID: 2923
	public int Reel2Spins = 48;

	// Token: 0x04000B6C RID: 2924
	public int Reel3Spins = 80;

	// Token: 0x04000B6D RID: 2925
	public int MaxReelSpins = 96;

	// Token: 0x04000B6E RID: 2926
	public float SpinDuration = 2f;

	// Token: 0x04000B6F RID: 2927
	private int SpinResult1;

	// Token: 0x04000B70 RID: 2928
	private int SpinResult2;

	// Token: 0x04000B71 RID: 2929
	private int SpinResult3;

	// Token: 0x04000B72 RID: 2930
	private int SpinResultPrevious1;

	// Token: 0x04000B73 RID: 2931
	private int SpinResultPrevious2;

	// Token: 0x04000B74 RID: 2932
	private int SpinResultPrevious3;

	// Token: 0x04000B75 RID: 2933
	private float SpinTime;

	// Token: 0x04000B76 RID: 2934
	public GameObjectRef StoragePrefab;

	// Token: 0x04000B77 RID: 2935
	public EntityRef StorageInstance;

	// Token: 0x04000B78 RID: 2936
	public SoundDefinition SpinSound;

	// Token: 0x04000B79 RID: 2937
	public SlotMachinePayoutDisplay PayoutDisplay;

	// Token: 0x04000B7A RID: 2938
	public SlotMachinePayoutSettings PayoutSettings;

	// Token: 0x04000B7B RID: 2939
	public Transform HandIkTarget;

	// Token: 0x04000B7C RID: 2940
	private const global::BaseEntity.Flags HasScrapForSpin = global::BaseEntity.Flags.Reserved1;

	// Token: 0x04000B7D RID: 2941
	private const global::BaseEntity.Flags IsSpinningFlag = global::BaseEntity.Flags.Reserved2;

	// Token: 0x04000B7E RID: 2942
	public Material PayoutIconMaterial;

	// Token: 0x04000B7F RID: 2943
	public bool UseTimeOfDayAdjustedSprite = true;

	// Token: 0x04000B80 RID: 2944
	public MeshRenderer[] PulseRenderers;

	// Token: 0x04000B81 RID: 2945
	public float PulseSpeed = 5f;

	// Token: 0x04000B82 RID: 2946
	[ColorUsage(true, true)]
	public Color PulseFrom;

	// Token: 0x04000B83 RID: 2947
	[ColorUsage(true, true)]
	public Color PulseTo;

	// Token: 0x04000B85 RID: 2949
	private global::BasePlayer CurrentSpinPlayer;

	// Token: 0x0600125A RID: 4698 RVA: 0x0009463C File Offset: 0x0009283C
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("SlotMachine.OnRpcMessage", 0))
		{
			if (rpc == 1251063754U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Deposit ");
				}
				using (TimeWarning.New("RPC_Deposit", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(1251063754U, "RPC_Deposit", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpc2 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_Deposit(rpc2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_Deposit");
					}
				}
				return true;
			}
			if (rpc == 1455840454U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Spin ");
				}
				using (TimeWarning.New("RPC_Spin", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(1455840454U, "RPC_Spin", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpc3 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_Spin(rpc3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in RPC_Spin");
					}
				}
				return true;
			}
			if (rpc == 3942337446U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_RequestMultiplierChange ");
				}
				using (TimeWarning.New("Server_RequestMultiplierChange", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(3942337446U, "Server_RequestMultiplierChange", this, player, 5UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(3942337446U, "Server_RequestMultiplierChange", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg2 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_RequestMultiplierChange(msg2);
						}
					}
					catch (Exception exception3)
					{
						Debug.LogException(exception3);
						player.Kick("RPC Error in Server_RequestMultiplierChange");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x170001A7 RID: 423
	// (get) Token: 0x0600125B RID: 4699 RVA: 0x0000564C File Offset: 0x0000384C
	private bool IsSpinning
	{
		get
		{
			return base.HasFlag(global::BaseEntity.Flags.Reserved2);
		}
	}

	// Token: 0x170001A8 RID: 424
	// (get) Token: 0x0600125C RID: 4700 RVA: 0x00094AB4 File Offset: 0x00092CB4
	// (set) Token: 0x0600125D RID: 4701 RVA: 0x00094ABC File Offset: 0x00092CBC
	public int CurrentMultiplier { get; private set; } = 1;

	// Token: 0x0600125E RID: 4702 RVA: 0x00094AC8 File Offset: 0x00092CC8
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.slotMachine = Facepunch.Pool.Get<ProtoBuf.SlotMachine>();
		info.msg.slotMachine.oldResult1 = this.SpinResultPrevious1;
		info.msg.slotMachine.oldResult2 = this.SpinResultPrevious2;
		info.msg.slotMachine.oldResult3 = this.SpinResultPrevious3;
		info.msg.slotMachine.newResult1 = this.SpinResult1;
		info.msg.slotMachine.newResult2 = this.SpinResult2;
		info.msg.slotMachine.newResult3 = this.SpinResult3;
		info.msg.slotMachine.isSpinning = this.IsSpinning;
		info.msg.slotMachine.spinTime = this.SpinTime;
		info.msg.slotMachine.storageID = this.StorageInstance.uid;
		info.msg.slotMachine.multiplier = this.CurrentMultiplier;
	}

	// Token: 0x0600125F RID: 4703 RVA: 0x00094BD0 File Offset: 0x00092DD0
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.slotMachine != null)
		{
			this.SpinResultPrevious1 = info.msg.slotMachine.oldResult1;
			this.SpinResultPrevious2 = info.msg.slotMachine.oldResult2;
			this.SpinResultPrevious3 = info.msg.slotMachine.oldResult3;
			this.SpinResult1 = info.msg.slotMachine.newResult1;
			this.SpinResult2 = info.msg.slotMachine.newResult2;
			this.SpinResult3 = info.msg.slotMachine.newResult3;
			this.CurrentMultiplier = info.msg.slotMachine.multiplier;
			if (base.isServer)
			{
				this.SpinTime = info.msg.slotMachine.spinTime;
			}
			this.StorageInstance.uid = info.msg.slotMachine.storageID;
			if (info.fromDisk && base.isServer)
			{
				base.SetFlag(global::BaseEntity.Flags.Reserved2, false, false, true);
			}
		}
	}

	// Token: 0x06001260 RID: 4704 RVA: 0x00006CA5 File Offset: 0x00004EA5
	public override float GetComfort()
	{
		return 1f;
	}

	// Token: 0x06001261 RID: 4705 RVA: 0x00094CE8 File Offset: 0x00092EE8
	public override void Spawn()
	{
		base.Spawn();
		if (!Rust.Application.isLoadingSave)
		{
			global::BaseEntity baseEntity = GameManager.server.CreateEntity(this.StoragePrefab.resourcePath, default(Vector3), default(Quaternion), true);
			baseEntity.Spawn();
			baseEntity.SetParent(this, false, false);
			this.StorageInstance.Set(baseEntity);
		}
	}

	// Token: 0x06001262 RID: 4706 RVA: 0x00094D48 File Offset: 0x00092F48
	internal override void DoServerDestroy()
	{
		SlotMachineStorage slotMachineStorage = this.StorageInstance.Get(base.isServer) as SlotMachineStorage;
		if (slotMachineStorage.IsValid())
		{
			slotMachineStorage.DropItems(null);
		}
		base.DoServerDestroy();
	}

	// Token: 0x06001263 RID: 4707 RVA: 0x00094D84 File Offset: 0x00092F84
	private int GetBettingAmount()
	{
		SlotMachineStorage component = this.StorageInstance.Get(base.isServer).GetComponent<SlotMachineStorage>();
		if (component == null)
		{
			return 0;
		}
		global::Item slot = component.inventory.GetSlot(0);
		if (slot != null)
		{
			return slot.amount;
		}
		return 0;
	}

	// Token: 0x06001264 RID: 4708 RVA: 0x00094DCC File Offset: 0x00092FCC
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RPC_Spin(global::BaseEntity.RPCMessage rpc)
	{
		if (this.IsSpinning)
		{
			return;
		}
		if (rpc.player != base.GetMounted())
		{
			return;
		}
		SlotMachineStorage component = this.StorageInstance.Get(base.isServer).GetComponent<SlotMachineStorage>();
		int num = (int)this.PayoutSettings.SpinCost.amount * this.CurrentMultiplier;
		if (this.GetBettingAmount() < num)
		{
			return;
		}
		if (rpc.player == null)
		{
			return;
		}
		global::BasePlayer player = rpc.player;
		this.CurrentSpinPlayer = player;
		player.inventory.loot.Clear();
		global::Item slot = component.inventory.GetSlot(0);
		int amount = 0;
		if (slot != null)
		{
			if (slot.amount > num)
			{
				slot.MarkDirty();
				slot.amount -= num;
				amount = slot.amount;
			}
			else
			{
				slot.amount -= num;
				slot.RemoveFromContainer();
			}
		}
		component.UpdateAmount(amount);
		base.SetFlag(global::BaseEntity.Flags.Reserved2, true, false, true);
		this.SpinResultPrevious1 = this.SpinResult1;
		this.SpinResultPrevious2 = this.SpinResult2;
		this.SpinResultPrevious3 = this.SpinResult3;
		this.CalculateSpinResults();
		this.SpinTime = UnityEngine.Time.time;
		base.ClientRPC<sbyte, sbyte, sbyte>(null, "RPC_OnSpin", (sbyte)this.SpinResult1, (sbyte)this.SpinResult2, (sbyte)this.SpinResult3);
		base.Invoke(new Action(this.CheckPayout), this.SpinDuration);
	}

	// Token: 0x06001265 RID: 4709 RVA: 0x00094F30 File Offset: 0x00093130
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RPC_Deposit(global::BaseEntity.RPCMessage rpc)
	{
		global::BasePlayer player = rpc.player;
		if (player == null)
		{
			return;
		}
		if (base.HasFlag(global::BaseEntity.Flags.Reserved2))
		{
			return;
		}
		if (this.StorageInstance.IsValid(base.isServer))
		{
			this.StorageInstance.Get(base.isServer).GetComponent<StorageContainer>().PlayerOpenLoot(player, "", false);
		}
	}

	// Token: 0x06001266 RID: 4710 RVA: 0x00094F94 File Offset: 0x00093194
	private void CheckPayout()
	{
		bool flag = false;
		if (this.PayoutSettings != null)
		{
			SlotMachinePayoutSettings.PayoutInfo payoutInfo;
			int num;
			if (this.CalculatePayout(out payoutInfo, out num))
			{
				int num2 = ((int)payoutInfo.Item.amount + num) * this.CurrentMultiplier;
				global::BaseEntity baseEntity = this.StorageInstance.Get(true);
				SlotMachineStorage slotMachineStorage;
				if (baseEntity != null && (slotMachineStorage = (baseEntity as SlotMachineStorage)) != null)
				{
					global::Item slot = slotMachineStorage.inventory.GetSlot(1);
					if (slot != null)
					{
						slot.amount += num2;
						slot.MarkDirty();
					}
					else
					{
						ItemManager.Create(payoutInfo.Item.itemDef, num2, 0UL).MoveToContainer(slotMachineStorage.inventory, 1, true, false, null, true);
					}
				}
				if (this.CurrentSpinPlayer.IsValid() && this.CurrentSpinPlayer == this._mounted)
				{
					this.CurrentSpinPlayer.ChatMessage(string.Format("You received {0}x {1} for slots payout!", num2, payoutInfo.Item.itemDef.displayName.english));
				}
				Analytics.Server.SlotMachineTransaction((int)this.PayoutSettings.SpinCost.amount * this.CurrentMultiplier, num2);
				Analytics.Azure.OnGamblingResult(this.CurrentSpinPlayer, this, (int)this.PayoutSettings.SpinCost.amount, num2, null);
				if (payoutInfo.OverrideWinEffect != null && payoutInfo.OverrideWinEffect.isValid)
				{
					Effect.server.Run(payoutInfo.OverrideWinEffect.resourcePath, this, 0U, Vector3.zero, Vector3.zero, null, false);
				}
				else if (this.PayoutSettings.DefaultWinEffect != null && this.PayoutSettings.DefaultWinEffect.isValid)
				{
					Effect.server.Run(this.PayoutSettings.DefaultWinEffect.resourcePath, this, 0U, Vector3.zero, Vector3.zero, null, false);
				}
				if (payoutInfo.OverrideWinEffect != null && payoutInfo.OverrideWinEffect.isValid)
				{
					flag = true;
				}
			}
			else
			{
				Analytics.Server.SlotMachineTransaction((int)this.PayoutSettings.SpinCost.amount * this.CurrentMultiplier, 0);
				Analytics.Azure.OnGamblingResult(this.CurrentSpinPlayer, this, (int)this.PayoutSettings.SpinCost.amount * this.CurrentMultiplier, 0, null);
			}
		}
		else
		{
			Debug.LogError(string.Format("Failed to process spin results: PayoutSettings != null {0} CurrentSpinPlayer.IsValid {1} CurrentSpinPlayer == mounted {2}", this.PayoutSettings != null, this.CurrentSpinPlayer.IsValid(), this.CurrentSpinPlayer == this._mounted));
		}
		if (!flag)
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved2, false, false, true);
		}
		else
		{
			base.Invoke(new Action(this.DelayedSpinningReset), 4f);
		}
		this.CurrentSpinPlayer = null;
	}

	// Token: 0x06001267 RID: 4711 RVA: 0x00095242 File Offset: 0x00093442
	private void DelayedSpinningReset()
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved2, false, false, true);
	}

	// Token: 0x06001268 RID: 4712 RVA: 0x00095254 File Offset: 0x00093454
	private void CalculateSpinResults()
	{
		if (global::SlotMachine.ForcePayoutIndex != -1)
		{
			this.SpinResult1 = this.PayoutSettings.Payouts[global::SlotMachine.ForcePayoutIndex].Result1;
			this.SpinResult2 = this.PayoutSettings.Payouts[global::SlotMachine.ForcePayoutIndex].Result2;
			this.SpinResult3 = this.PayoutSettings.Payouts[global::SlotMachine.ForcePayoutIndex].Result3;
			return;
		}
		this.SpinResult1 = this.RandomSpinResult();
		this.SpinResult2 = this.RandomSpinResult();
		this.SpinResult3 = this.RandomSpinResult();
	}

	// Token: 0x06001269 RID: 4713 RVA: 0x000952F0 File Offset: 0x000934F0
	private int RandomSpinResult()
	{
		int num = new System.Random(UnityEngine.Random.Range(0, 1000)).Next(0, this.PayoutSettings.TotalStops);
		int num2 = 0;
		int num3 = 0;
		foreach (int num4 in this.PayoutSettings.VirtualFaces)
		{
			if (num < num4 + num2)
			{
				return num3;
			}
			num2 += num4;
			num3++;
		}
		return 15;
	}

	// Token: 0x0600126A RID: 4714 RVA: 0x0009535C File Offset: 0x0009355C
	public override void OnPlayerDismounted(global::BasePlayer player)
	{
		base.OnPlayerDismounted(player);
		global::BaseEntity baseEntity = this.StorageInstance.Get(true);
		SlotMachineStorage slotMachineStorage;
		if (baseEntity != null && (slotMachineStorage = (baseEntity as SlotMachineStorage)) != null)
		{
			global::Item slot = slotMachineStorage.inventory.GetSlot(1);
			if (slot != null)
			{
				slot.MoveToContainer(player.inventory.containerMain, -1, true, false, null, true);
			}
		}
	}

	// Token: 0x0600126B RID: 4715 RVA: 0x000953B8 File Offset: 0x000935B8
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	private void Server_RequestMultiplierChange(global::BaseEntity.RPCMessage msg)
	{
		if (msg.player != this._mounted)
		{
			return;
		}
		if (base.HasFlag(global::BaseEntity.Flags.Reserved2))
		{
			return;
		}
		this.CurrentMultiplier = Mathf.Clamp(msg.read.Int32(), 1, 5);
		this.OnBettingScrapUpdated(this.GetBettingAmount());
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x0600126C RID: 4716 RVA: 0x00095412 File Offset: 0x00093612
	public void OnBettingScrapUpdated(int amount)
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved1, (float)amount >= this.PayoutSettings.SpinCost.amount * (float)this.CurrentMultiplier, false, true);
	}

	// Token: 0x0600126D RID: 4717 RVA: 0x00095440 File Offset: 0x00093640
	private bool CalculatePayout(out SlotMachinePayoutSettings.PayoutInfo info, out int bonus)
	{
		info = default(SlotMachinePayoutSettings.PayoutInfo);
		bonus = 0;
		foreach (SlotMachinePayoutSettings.IndividualPayouts individualPayouts in this.PayoutSettings.FacePayouts)
		{
			if (individualPayouts.Result == this.SpinResult1)
			{
				bonus += (int)individualPayouts.Item.amount;
			}
			if (individualPayouts.Result == this.SpinResult2)
			{
				bonus += (int)individualPayouts.Item.amount;
			}
			if (individualPayouts.Result == this.SpinResult3)
			{
				bonus += (int)individualPayouts.Item.amount;
			}
			if (bonus > 0)
			{
				info.Item = new ItemAmount(individualPayouts.Item.itemDef, 0f);
			}
		}
		foreach (SlotMachinePayoutSettings.PayoutInfo payoutInfo in this.PayoutSettings.Payouts)
		{
			if (payoutInfo.Result1 == this.SpinResult1 && payoutInfo.Result2 == this.SpinResult2 && payoutInfo.Result3 == this.SpinResult3)
			{
				info = payoutInfo;
				return true;
			}
		}
		return bonus > 0;
	}

	// Token: 0x02000C03 RID: 3075
	public enum SlotFaces
	{
		// Token: 0x04004194 RID: 16788
		Scrap,
		// Token: 0x04004195 RID: 16789
		Rope,
		// Token: 0x04004196 RID: 16790
		Apple,
		// Token: 0x04004197 RID: 16791
		LowGrade,
		// Token: 0x04004198 RID: 16792
		Wood,
		// Token: 0x04004199 RID: 16793
		Bandage,
		// Token: 0x0400419A RID: 16794
		Charcoal,
		// Token: 0x0400419B RID: 16795
		Gunpowder,
		// Token: 0x0400419C RID: 16796
		Rust,
		// Token: 0x0400419D RID: 16797
		Meat,
		// Token: 0x0400419E RID: 16798
		Hammer,
		// Token: 0x0400419F RID: 16799
		Sulfur,
		// Token: 0x040041A0 RID: 16800
		TechScrap,
		// Token: 0x040041A1 RID: 16801
		Frags,
		// Token: 0x040041A2 RID: 16802
		Cloth,
		// Token: 0x040041A3 RID: 16803
		LuckySeven
	}
}
