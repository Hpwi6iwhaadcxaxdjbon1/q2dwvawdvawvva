using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000088 RID: 136
public class IOEntity : global::DecayEntity
{
	// Token: 0x0400083E RID: 2110
	[Header("IOEntity")]
	public Transform debugOrigin;

	// Token: 0x0400083F RID: 2111
	public ItemDefinition sourceItem;

	// Token: 0x04000840 RID: 2112
	[NonSerialized]
	public int lastResetIndex;

	// Token: 0x04000841 RID: 2113
	[ServerVar]
	[Help("How many miliseconds to budget for processing io entities per server frame")]
	public static float framebudgetms = 1f;

	// Token: 0x04000842 RID: 2114
	[ServerVar]
	public static float responsetime = 0.1f;

	// Token: 0x04000843 RID: 2115
	[ServerVar]
	public static int backtracking = 8;

	// Token: 0x04000844 RID: 2116
	[ServerVar(Help = "Print out what is taking so long in the IO frame budget")]
	public static bool debugBudget = false;

	// Token: 0x04000845 RID: 2117
	[ServerVar(Help = "Ignore frames with a lower ms than this while debugBudget is active")]
	public static float debugBudgetThreshold = 2f;

	// Token: 0x04000846 RID: 2118
	public const global::BaseEntity.Flags Flag_ShortCircuit = global::BaseEntity.Flags.Reserved7;

	// Token: 0x04000847 RID: 2119
	public const global::BaseEntity.Flags Flag_HasPower = global::BaseEntity.Flags.Reserved8;

	// Token: 0x04000848 RID: 2120
	public global::IOEntity.IOSlot[] inputs;

	// Token: 0x04000849 RID: 2121
	public global::IOEntity.IOSlot[] outputs;

	// Token: 0x0400084A RID: 2122
	public global::IOEntity.IOType ioType;

	// Token: 0x0400084B RID: 2123
	public static Queue<global::IOEntity> _processQueue = new Queue<global::IOEntity>();

	// Token: 0x0400084C RID: 2124
	private static List<global::IOEntity.FrameTiming> timings = new List<global::IOEntity.FrameTiming>();

	// Token: 0x0400084D RID: 2125
	private int cachedOutputsUsed;

	// Token: 0x0400084E RID: 2126
	protected int lastPassthroughEnergy;

	// Token: 0x0400084F RID: 2127
	private int lastEnergy;

	// Token: 0x04000850 RID: 2128
	protected int currentEnergy;

	// Token: 0x04000851 RID: 2129
	protected float lastUpdateTime;

	// Token: 0x04000852 RID: 2130
	protected int lastUpdateBlockedFrame;

	// Token: 0x04000853 RID: 2131
	protected bool ensureOutputsUpdated;

	// Token: 0x04000854 RID: 2132
	public const int MaxContainerSourceCount = 32;

	// Token: 0x04000855 RID: 2133
	private List<BoxCollider> spawnedColliders = new List<BoxCollider>();

	// Token: 0x06000CBB RID: 3259 RVA: 0x0006E3A0 File Offset: 0x0006C5A0
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("IOEntity.OnRpcMessage", 0))
		{
			if (rpc == 4161541566U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_RequestData ");
				}
				using (TimeWarning.New("Server_RequestData", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(4161541566U, "Server_RequestData", this, player, 10UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(4161541566U, "Server_RequestData", this, player, 6f))
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
							this.Server_RequestData(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in Server_RequestData");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000CBC RID: 3260 RVA: 0x0006E564 File Offset: 0x0006C764
	public override void ResetState()
	{
		base.ResetState();
		if (base.isServer)
		{
			this.lastResetIndex = 0;
			this.cachedOutputsUsed = 0;
			this.lastPassthroughEnergy = 0;
			this.lastEnergy = 0;
			this.currentEnergy = 0;
			this.lastUpdateTime = 0f;
			this.ensureOutputsUpdated = false;
		}
		this.ClearIndustrialPreventBuilding();
	}

	// Token: 0x06000CBD RID: 3261 RVA: 0x0006E5BA File Offset: 0x0006C7BA
	public string GetDisplayName()
	{
		if (this.sourceItem != null)
		{
			return this.sourceItem.displayName.translated;
		}
		return base.ShortPrefabName;
	}

	// Token: 0x06000CBE RID: 3262 RVA: 0x00007A3C File Offset: 0x00005C3C
	public virtual bool IsRootEntity()
	{
		return false;
	}

	// Token: 0x17000136 RID: 310
	// (get) Token: 0x06000CBF RID: 3263 RVA: 0x00007A3C File Offset: 0x00005C3C
	public virtual bool IsGravitySource
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06000CC0 RID: 3264 RVA: 0x0006E5E4 File Offset: 0x0006C7E4
	public global::IOEntity FindGravitySource(ref Vector3 worldHandlePosition, int depth, bool ignoreSelf)
	{
		if (depth <= 0)
		{
			return null;
		}
		if (!ignoreSelf && this.IsGravitySource)
		{
			worldHandlePosition = base.transform.TransformPoint(this.outputs[0].handlePosition);
			return this;
		}
		global::IOEntity.IOSlot[] array = this.inputs;
		for (int i = 0; i < array.Length; i++)
		{
			global::IOEntity ioentity = array[i].connectedTo.Get(base.isServer);
			if (ioentity != null)
			{
				if (ioentity.IsGravitySource)
				{
					worldHandlePosition = ioentity.transform.TransformPoint(ioentity.outputs[0].handlePosition);
					return ioentity;
				}
				ioentity = ioentity.FindGravitySource(ref worldHandlePosition, depth - 1, false);
				if (ioentity != null)
				{
					worldHandlePosition = ioentity.transform.TransformPoint(ioentity.outputs[0].handlePosition);
					return ioentity;
				}
			}
		}
		return null;
	}

	// Token: 0x06000CC1 RID: 3265 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void SetFuelType(ItemDefinition def, global::IOEntity source)
	{
	}

	// Token: 0x06000CC2 RID: 3266 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool WantsPower()
	{
		return true;
	}

	// Token: 0x06000CC3 RID: 3267 RVA: 0x0006E6B5 File Offset: 0x0006C8B5
	public virtual bool WantsPassthroughPower()
	{
		return this.WantsPower();
	}

	// Token: 0x06000CC4 RID: 3268 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual int ConsumptionAmount()
	{
		return 1;
	}

	// Token: 0x06000CC5 RID: 3269 RVA: 0x0006E6BD File Offset: 0x0006C8BD
	public virtual bool ShouldDrainBattery(global::IOEntity battery)
	{
		return this.ioType == battery.ioType;
	}

	// Token: 0x06000CC6 RID: 3270 RVA: 0x00007A3C File Offset: 0x00005C3C
	public virtual int MaximalPowerOutput()
	{
		return 0;
	}

	// Token: 0x06000CC7 RID: 3271 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool AllowDrainFrom(int outputSlot)
	{
		return true;
	}

	// Token: 0x06000CC8 RID: 3272 RVA: 0x00003278 File Offset: 0x00001478
	public virtual bool IsPowered()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved8);
	}

	// Token: 0x06000CC9 RID: 3273 RVA: 0x0006E6D0 File Offset: 0x0006C8D0
	public bool IsConnectedToAnySlot(global::IOEntity entity, int slot, int depth, bool defaultReturn = false)
	{
		if (depth > 0 && slot < this.inputs.Length)
		{
			global::IOEntity ioentity = this.inputs[slot].connectedTo.Get(true);
			if (ioentity != null)
			{
				if (ioentity == entity)
				{
					return true;
				}
				if (this.ConsiderConnectedTo(entity))
				{
					return true;
				}
				if (ioentity.IsConnectedTo(entity, depth - 1, defaultReturn))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06000CCA RID: 3274 RVA: 0x0006E730 File Offset: 0x0006C930
	public bool IsConnectedTo(global::IOEntity entity, int slot, int depth, bool defaultReturn = false)
	{
		if (depth > 0 && slot < this.inputs.Length)
		{
			global::IOEntity.IOSlot ioslot = this.inputs[slot];
			if (ioslot.mainPowerSlot)
			{
				global::IOEntity ioentity = ioslot.connectedTo.Get(true);
				if (ioentity != null)
				{
					if (ioentity == entity)
					{
						return true;
					}
					if (this.ConsiderConnectedTo(entity))
					{
						return true;
					}
					if (ioentity.IsConnectedTo(entity, depth - 1, defaultReturn))
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	// Token: 0x06000CCB RID: 3275 RVA: 0x0006E79C File Offset: 0x0006C99C
	public bool IsConnectedTo(global::IOEntity entity, int depth, bool defaultReturn = false)
	{
		if (depth > 0)
		{
			for (int i = 0; i < this.inputs.Length; i++)
			{
				global::IOEntity.IOSlot ioslot = this.inputs[i];
				if (ioslot.mainPowerSlot)
				{
					global::IOEntity ioentity = ioslot.connectedTo.Get(true);
					if (ioentity != null)
					{
						if (ioentity == entity)
						{
							return true;
						}
						if (this.ConsiderConnectedTo(entity))
						{
							return true;
						}
						if (ioentity.IsConnectedTo(entity, depth - 1, defaultReturn))
						{
							return true;
						}
					}
				}
			}
			return false;
		}
		return defaultReturn;
	}

	// Token: 0x06000CCC RID: 3276 RVA: 0x00007A3C File Offset: 0x00005C3C
	protected virtual bool ConsiderConnectedTo(global::IOEntity entity)
	{
		return false;
	}

	// Token: 0x06000CCD RID: 3277 RVA: 0x0006E814 File Offset: 0x0006CA14
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(6f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(10UL)]
	private void Server_RequestData(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		int slot = msg.read.Int32();
		bool input = msg.read.Int32() == 1;
		this.SendAdditionalData(player, slot, input);
	}

	// Token: 0x06000CCE RID: 3278 RVA: 0x0006E84C File Offset: 0x0006CA4C
	public virtual void SendAdditionalData(global::BasePlayer player, int slot, bool input)
	{
		int passthroughAmountForAnySlot = this.GetPassthroughAmountForAnySlot(slot, input);
		base.ClientRPCPlayer<int, int, float, float>(null, player, "Client_ReceiveAdditionalData", this.currentEnergy, passthroughAmountForAnySlot, 0f, 0f);
	}

	// Token: 0x06000CCF RID: 3279 RVA: 0x0006E880 File Offset: 0x0006CA80
	protected int GetPassthroughAmountForAnySlot(int slot, bool isInputSlot)
	{
		int result = 0;
		if (isInputSlot)
		{
			if (slot >= 0 && slot < this.inputs.Length)
			{
				global::IOEntity.IOSlot ioslot = this.inputs[slot];
				global::IOEntity ioentity = ioslot.connectedTo.Get(true);
				if (ioentity != null && ioslot.connectedToSlot >= 0 && ioslot.connectedToSlot < ioentity.outputs.Length)
				{
					result = ioentity.GetPassthroughAmount(this.inputs[slot].connectedToSlot);
				}
			}
		}
		else if (slot >= 0 && slot < this.outputs.Length)
		{
			result = this.GetPassthroughAmount(slot);
		}
		return result;
	}

	// Token: 0x06000CD0 RID: 3280 RVA: 0x0006E908 File Offset: 0x0006CB08
	public static void ProcessQueue()
	{
		float realtimeSinceStartup = UnityEngine.Time.realtimeSinceStartup;
		float num = global::IOEntity.framebudgetms / 1000f;
		if (global::IOEntity.debugBudget)
		{
			global::IOEntity.timings.Clear();
		}
		while (global::IOEntity._processQueue.Count > 0 && UnityEngine.Time.realtimeSinceStartup < realtimeSinceStartup + num && !global::IOEntity._processQueue.Peek().HasBlockedUpdatedOutputsThisFrame)
		{
			float realtimeSinceStartup2 = UnityEngine.Time.realtimeSinceStartup;
			global::IOEntity ioentity = global::IOEntity._processQueue.Dequeue();
			if (ioentity.IsValid())
			{
				ioentity.UpdateOutputs();
			}
			if (global::IOEntity.debugBudget)
			{
				global::IOEntity.timings.Add(new global::IOEntity.FrameTiming
				{
					PrefabName = ioentity.ShortPrefabName,
					Time = (UnityEngine.Time.realtimeSinceStartup - realtimeSinceStartup2) * 1000f
				});
			}
		}
		if (global::IOEntity.debugBudget)
		{
			float num2 = UnityEngine.Time.realtimeSinceStartup - realtimeSinceStartup;
			float num3 = global::IOEntity.debugBudgetThreshold / 1000f;
			if (num2 > num3)
			{
				TextTable textTable = new TextTable();
				textTable.AddColumns(new string[]
				{
					"Prefab Name",
					"Time (in ms)"
				});
				foreach (global::IOEntity.FrameTiming frameTiming in global::IOEntity.timings)
				{
					TextTable textTable2 = textTable;
					string[] array = new string[2];
					array[0] = frameTiming.PrefabName;
					int num4 = 1;
					float time = frameTiming.Time;
					array[num4] = time.ToString();
					textTable2.AddRow(array);
				}
				textTable.AddRow(new string[]
				{
					"Total time",
					(num2 * 1000f).ToString()
				});
				Debug.Log(textTable.ToString());
			}
		}
	}

	// Token: 0x06000CD1 RID: 3281 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void ResetIOState()
	{
	}

	// Token: 0x06000CD2 RID: 3282 RVA: 0x0006EAB0 File Offset: 0x0006CCB0
	public virtual void Init()
	{
		for (int i = 0; i < this.outputs.Length; i++)
		{
			global::IOEntity.IOSlot ioslot = this.outputs[i];
			ioslot.connectedTo.Init();
			if (ioslot.connectedTo.Get(true) != null)
			{
				int connectedToSlot = ioslot.connectedToSlot;
				if (connectedToSlot < 0 || connectedToSlot >= ioslot.connectedTo.Get(true).inputs.Length)
				{
					Debug.LogError(string.Concat(new object[]
					{
						"Slot IOR Error: ",
						base.name,
						" setting up inputs for ",
						ioslot.connectedTo.Get(true).name,
						" slot : ",
						ioslot.connectedToSlot
					}));
				}
				else
				{
					ioslot.connectedTo.Get(true).inputs[ioslot.connectedToSlot].connectedTo.Set(this);
					ioslot.connectedTo.Get(true).inputs[ioslot.connectedToSlot].connectedToSlot = i;
					ioslot.connectedTo.Get(true).inputs[ioslot.connectedToSlot].connectedTo.Init();
				}
			}
		}
		this.UpdateUsedOutputs();
		if (this.IsRootEntity())
		{
			base.Invoke(new Action(this.MarkDirtyForceUpdateOutputs), UnityEngine.Random.Range(1f, 1f));
		}
	}

	// Token: 0x06000CD3 RID: 3283 RVA: 0x0006EC09 File Offset: 0x0006CE09
	internal override void DoServerDestroy()
	{
		if (base.isServer)
		{
			this.Shutdown();
		}
		base.DoServerDestroy();
	}

	// Token: 0x06000CD4 RID: 3284 RVA: 0x0006EC20 File Offset: 0x0006CE20
	public void ClearConnections()
	{
		List<global::IOEntity> list = Facepunch.Pool.GetList<global::IOEntity>();
		List<global::IOEntity> list2 = Facepunch.Pool.GetList<global::IOEntity>();
		foreach (global::IOEntity.IOSlot ioslot in this.inputs)
		{
			global::IOEntity ioentity = null;
			if (ioslot.connectedTo.Get(true) != null)
			{
				ioentity = ioslot.connectedTo.Get(true);
				if (ioslot.type == global::IOEntity.IOType.Industrial)
				{
					list2.Add(ioentity);
				}
				foreach (global::IOEntity.IOSlot ioslot2 in ioslot.connectedTo.Get(true).outputs)
				{
					if (ioslot2.connectedTo.Get(true) != null && ioslot2.connectedTo.Get(true).EqualNetID(this))
					{
						ioslot2.Clear();
					}
				}
			}
			ioslot.Clear();
			if (ioentity)
			{
				ioentity.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			}
		}
		foreach (global::IOEntity.IOSlot ioslot3 in this.outputs)
		{
			if (ioslot3.connectedTo.Get(true) != null)
			{
				list.Add(ioslot3.connectedTo.Get(true));
				if (ioslot3.type == global::IOEntity.IOType.Industrial)
				{
					list2.Add(list[list.Count - 1]);
				}
				foreach (global::IOEntity.IOSlot ioslot4 in ioslot3.connectedTo.Get(true).inputs)
				{
					if (ioslot4.connectedTo.Get(true) != null && ioslot4.connectedTo.Get(true).EqualNetID(this))
					{
						ioslot4.Clear();
					}
				}
			}
			if (ioslot3.connectedTo.Get(true))
			{
				ioslot3.connectedTo.Get(true).UpdateFromInput(0, ioslot3.connectedToSlot);
			}
			ioslot3.Clear();
		}
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		foreach (global::IOEntity ioentity2 in list)
		{
			if (ioentity2 != null)
			{
				ioentity2.MarkDirty();
				ioentity2.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			}
		}
		for (int k = 0; k < this.inputs.Length; k++)
		{
			this.UpdateFromInput(0, k);
		}
		foreach (global::IOEntity ioentity3 in list2)
		{
			if (ioentity3 != null)
			{
				ioentity3.NotifyIndustrialNetworkChanged();
			}
			ioentity3.RefreshIndustrialPreventBuilding();
		}
		Facepunch.Pool.FreeList<global::IOEntity>(ref list);
		Facepunch.Pool.FreeList<global::IOEntity>(ref list2);
		this.RefreshIndustrialPreventBuilding();
	}

	// Token: 0x06000CD5 RID: 3285 RVA: 0x0006EEEC File Offset: 0x0006D0EC
	public void Shutdown()
	{
		this.SendChangedToRoot(true);
		this.ClearConnections();
	}

	// Token: 0x06000CD6 RID: 3286 RVA: 0x0006EEFB File Offset: 0x0006D0FB
	public void MarkDirtyForceUpdateOutputs()
	{
		this.ensureOutputsUpdated = true;
		this.MarkDirty();
	}

	// Token: 0x06000CD7 RID: 3287 RVA: 0x0006EF0C File Offset: 0x0006D10C
	public void UpdateUsedOutputs()
	{
		this.cachedOutputsUsed = 0;
		global::IOEntity.IOSlot[] array = this.outputs;
		for (int i = 0; i < array.Length; i++)
		{
			global::IOEntity ioentity = array[i].connectedTo.Get(true);
			if (ioentity != null && !ioentity.IsDestroyed)
			{
				this.cachedOutputsUsed++;
			}
		}
	}

	// Token: 0x06000CD8 RID: 3288 RVA: 0x0006EF63 File Offset: 0x0006D163
	public virtual void MarkDirty()
	{
		if (base.isClient)
		{
			return;
		}
		this.UpdateUsedOutputs();
		this.TouchIOState();
	}

	// Token: 0x06000CD9 RID: 3289 RVA: 0x0006EF7A File Offset: 0x0006D17A
	public virtual int DesiredPower()
	{
		return this.ConsumptionAmount();
	}

	// Token: 0x06000CDA RID: 3290 RVA: 0x00036DC0 File Offset: 0x00034FC0
	public virtual int CalculateCurrentEnergy(int inputAmount, int inputSlot)
	{
		return inputAmount;
	}

	// Token: 0x06000CDB RID: 3291 RVA: 0x0006EF82 File Offset: 0x0006D182
	public virtual int GetCurrentEnergy()
	{
		return Mathf.Clamp(this.currentEnergy - this.ConsumptionAmount(), 0, this.currentEnergy);
	}

	// Token: 0x06000CDC RID: 3292 RVA: 0x0006EFA0 File Offset: 0x0006D1A0
	public virtual int GetPassthroughAmount(int outputSlot = 0)
	{
		if (outputSlot < 0 || outputSlot >= this.outputs.Length)
		{
			return 0;
		}
		global::IOEntity ioentity = this.outputs[outputSlot].connectedTo.Get(true);
		if (ioentity == null || ioentity.IsDestroyed)
		{
			return 0;
		}
		int num = (this.cachedOutputsUsed == 0) ? 1 : this.cachedOutputsUsed;
		return this.GetCurrentEnergy() / num;
	}

	// Token: 0x06000CDD RID: 3293 RVA: 0x0006EFFF File Offset: 0x0006D1FF
	public virtual void UpdateHasPower(int inputAmount, int inputSlot)
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved8, inputAmount >= this.ConsumptionAmount() && inputAmount > 0, false, false);
	}

	// Token: 0x06000CDE RID: 3294 RVA: 0x0006F020 File Offset: 0x0006D220
	public void TouchInternal()
	{
		int passthroughAmount = this.GetPassthroughAmount(0);
		bool flag = this.lastPassthroughEnergy != passthroughAmount;
		this.lastPassthroughEnergy = passthroughAmount;
		if (flag)
		{
			this.IOStateChanged(this.currentEnergy, 0);
			this.ensureOutputsUpdated = true;
		}
		global::IOEntity._processQueue.Enqueue(this);
	}

	// Token: 0x06000CDF RID: 3295 RVA: 0x0006F06C File Offset: 0x0006D26C
	public virtual void UpdateFromInput(int inputAmount, int inputSlot)
	{
		if (this.inputs[inputSlot].type != this.ioType || this.inputs[inputSlot].type == global::IOEntity.IOType.Industrial)
		{
			this.IOStateChanged(inputAmount, inputSlot);
			return;
		}
		this.UpdateHasPower(inputAmount, inputSlot);
		this.lastEnergy = this.currentEnergy;
		this.currentEnergy = this.CalculateCurrentEnergy(inputAmount, inputSlot);
		int passthroughAmount = this.GetPassthroughAmount(0);
		bool flag = this.lastPassthroughEnergy != passthroughAmount;
		this.lastPassthroughEnergy = passthroughAmount;
		if (this.currentEnergy != this.lastEnergy || flag)
		{
			this.IOStateChanged(inputAmount, inputSlot);
			this.ensureOutputsUpdated = true;
		}
		global::IOEntity._processQueue.Enqueue(this);
	}

	// Token: 0x06000CE0 RID: 3296 RVA: 0x0006F114 File Offset: 0x0006D314
	public virtual void TouchIOState()
	{
		if (base.isClient)
		{
			return;
		}
		this.TouchInternal();
	}

	// Token: 0x06000CE1 RID: 3297 RVA: 0x0006F125 File Offset: 0x0006D325
	public virtual void SendIONetworkUpdate()
	{
		base.SendNetworkUpdate_Flags();
	}

	// Token: 0x06000CE2 RID: 3298 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void IOStateChanged(int inputAmount, int inputSlot)
	{
	}

	// Token: 0x06000CE3 RID: 3299 RVA: 0x0006F12D File Offset: 0x0006D32D
	public virtual void OnCircuitChanged(bool forceUpdate)
	{
		if (forceUpdate)
		{
			this.MarkDirtyForceUpdateOutputs();
		}
	}

	// Token: 0x06000CE4 RID: 3300 RVA: 0x0006F138 File Offset: 0x0006D338
	public virtual void SendChangedToRoot(bool forceUpdate)
	{
		List<global::IOEntity> list = Facepunch.Pool.GetList<global::IOEntity>();
		this.SendChangedToRootRecursive(forceUpdate, ref list);
		Facepunch.Pool.FreeList<global::IOEntity>(ref list);
	}

	// Token: 0x06000CE5 RID: 3301 RVA: 0x0006F15C File Offset: 0x0006D35C
	public virtual void SendChangedToRootRecursive(bool forceUpdate, ref List<global::IOEntity> existing)
	{
		bool flag = this.IsRootEntity();
		if (!existing.Contains(this))
		{
			existing.Add(this);
			bool flag2 = false;
			for (int i = 0; i < this.inputs.Length; i++)
			{
				global::IOEntity.IOSlot ioslot = this.inputs[i];
				if (ioslot.mainPowerSlot)
				{
					global::IOEntity ioentity = ioslot.connectedTo.Get(true);
					if (!(ioentity == null) && !existing.Contains(ioentity))
					{
						flag2 = true;
						if (forceUpdate)
						{
							ioentity.ensureOutputsUpdated = true;
						}
						ioentity.SendChangedToRootRecursive(forceUpdate, ref existing);
					}
				}
			}
			if (flag)
			{
				forceUpdate = (forceUpdate && !flag2);
				this.OnCircuitChanged(forceUpdate);
			}
		}
	}

	// Token: 0x06000CE6 RID: 3302 RVA: 0x0006F1F8 File Offset: 0x0006D3F8
	public void NotifyIndustrialNetworkChanged()
	{
		List<global::IOEntity> list = Facepunch.Pool.GetList<global::IOEntity>();
		this.OnIndustrialNetworkChanged();
		this.NotifyIndustrialNetworkChanged(list, true, 128);
		list.Clear();
		this.NotifyIndustrialNetworkChanged(list, false, 128);
		Facepunch.Pool.FreeList<global::IOEntity>(ref list);
	}

	// Token: 0x06000CE7 RID: 3303 RVA: 0x0006F238 File Offset: 0x0006D438
	private void NotifyIndustrialNetworkChanged(List<global::IOEntity> existing, bool input, int maxDepth)
	{
		if (maxDepth <= 0)
		{
			return;
		}
		if (!existing.Contains(this))
		{
			if (existing.Count != 0)
			{
				this.OnIndustrialNetworkChanged();
			}
			existing.Add(this);
			foreach (global::IOEntity.IOSlot ioslot in input ? this.inputs : this.outputs)
			{
				if (ioslot.type == global::IOEntity.IOType.Industrial && ioslot.connectedTo.Get(true) != null)
				{
					ioslot.connectedTo.Get(true).NotifyIndustrialNetworkChanged(existing, input, maxDepth - 1);
				}
			}
		}
	}

	// Token: 0x06000CE8 RID: 3304 RVA: 0x000063A5 File Offset: 0x000045A5
	protected virtual void OnIndustrialNetworkChanged()
	{
	}

	// Token: 0x06000CE9 RID: 3305 RVA: 0x0006F2C0 File Offset: 0x0006D4C0
	protected bool ShouldUpdateOutputs()
	{
		if (UnityEngine.Time.realtimeSinceStartup - this.lastUpdateTime < global::IOEntity.responsetime)
		{
			this.lastUpdateBlockedFrame = UnityEngine.Time.frameCount;
			global::IOEntity._processQueue.Enqueue(this);
			return false;
		}
		this.lastUpdateTime = UnityEngine.Time.realtimeSinceStartup;
		this.SendIONetworkUpdate();
		if (this.outputs.Length == 0)
		{
			this.ensureOutputsUpdated = false;
			return false;
		}
		return true;
	}

	// Token: 0x17000137 RID: 311
	// (get) Token: 0x06000CEA RID: 3306 RVA: 0x0006F31C File Offset: 0x0006D51C
	private bool HasBlockedUpdatedOutputsThisFrame
	{
		get
		{
			return UnityEngine.Time.frameCount == this.lastUpdateBlockedFrame;
		}
	}

	// Token: 0x06000CEB RID: 3307 RVA: 0x0006F32C File Offset: 0x0006D52C
	public virtual void UpdateOutputs()
	{
		if (!this.ShouldUpdateOutputs())
		{
			return;
		}
		if (this.ensureOutputsUpdated)
		{
			this.ensureOutputsUpdated = false;
			using (TimeWarning.New("ProcessIOOutputs", 0))
			{
				for (int i = 0; i < this.outputs.Length; i++)
				{
					global::IOEntity.IOSlot ioslot = this.outputs[i];
					bool flag = true;
					global::IOEntity ioentity = ioslot.connectedTo.Get(true);
					if (ioentity != null)
					{
						if (this.ioType == global::IOEntity.IOType.Fluidic && !this.DisregardGravityRestrictionsOnLiquid && !ioentity.DisregardGravityRestrictionsOnLiquid)
						{
							using (TimeWarning.New("FluidOutputProcessing", 0))
							{
								if (!ioentity.AllowLiquidPassthrough(this, base.transform.TransformPoint(ioslot.handlePosition), false))
								{
									flag = false;
								}
							}
						}
						int passthroughAmount = this.GetPassthroughAmount(i);
						ioentity.UpdateFromInput(flag ? passthroughAmount : 0, ioslot.connectedToSlot);
					}
				}
			}
		}
	}

	// Token: 0x06000CEC RID: 3308 RVA: 0x0006F438 File Offset: 0x0006D638
	public override void Spawn()
	{
		base.Spawn();
		if (!Rust.Application.isLoadingSave)
		{
			this.Init();
		}
	}

	// Token: 0x06000CED RID: 3309 RVA: 0x0006F44D File Offset: 0x0006D64D
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.Init();
	}

	// Token: 0x06000CEE RID: 3310 RVA: 0x0006F45B File Offset: 0x0006D65B
	public override void PostMapEntitySpawn()
	{
		base.PostMapEntitySpawn();
		this.Init();
	}

	// Token: 0x06000CEF RID: 3311 RVA: 0x0006F46C File Offset: 0x0006D66C
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.ioEntity = Facepunch.Pool.Get<ProtoBuf.IOEntity>();
		info.msg.ioEntity.inputs = Facepunch.Pool.GetList<ProtoBuf.IOEntity.IOConnection>();
		info.msg.ioEntity.outputs = Facepunch.Pool.GetList<ProtoBuf.IOEntity.IOConnection>();
		foreach (global::IOEntity.IOSlot ioslot in this.inputs)
		{
			ProtoBuf.IOEntity.IOConnection ioconnection = Facepunch.Pool.Get<ProtoBuf.IOEntity.IOConnection>();
			ioconnection.connectedID = ioslot.connectedTo.entityRef.uid;
			ioconnection.connectedToSlot = ioslot.connectedToSlot;
			ioconnection.niceName = ioslot.niceName;
			ioconnection.type = (int)ioslot.type;
			ioconnection.inUse = ioconnection.connectedID.IsValid;
			ioconnection.colour = (int)ioslot.wireColour;
			ioconnection.lineThickness = ioslot.lineThickness;
			info.msg.ioEntity.inputs.Add(ioconnection);
		}
		foreach (global::IOEntity.IOSlot ioslot2 in this.outputs)
		{
			ProtoBuf.IOEntity.IOConnection ioconnection2 = Facepunch.Pool.Get<ProtoBuf.IOEntity.IOConnection>();
			ioconnection2.connectedID = ioslot2.connectedTo.entityRef.uid;
			ioconnection2.connectedToSlot = ioslot2.connectedToSlot;
			ioconnection2.niceName = ioslot2.niceName;
			ioconnection2.type = (int)ioslot2.type;
			ioconnection2.inUse = ioconnection2.connectedID.IsValid;
			ioconnection2.colour = (int)ioslot2.wireColour;
			ioconnection2.worldSpaceRotation = ioslot2.worldSpaceLineEndRotation;
			ioconnection2.lineThickness = ioslot2.lineThickness;
			if (ioslot2.linePoints != null)
			{
				ioconnection2.linePointList = Facepunch.Pool.GetList<ProtoBuf.IOEntity.IOConnection.LineVec>();
				ioconnection2.linePointList.Clear();
				for (int j = 0; j < ioslot2.linePoints.Length; j++)
				{
					Vector3 v = ioslot2.linePoints[j];
					ProtoBuf.IOEntity.IOConnection.LineVec lineVec = Facepunch.Pool.Get<ProtoBuf.IOEntity.IOConnection.LineVec>();
					lineVec.vec = v;
					if (ioslot2.slackLevels.Length > j)
					{
						lineVec.vec.w = ioslot2.slackLevels[j];
					}
					ioconnection2.linePointList.Add(lineVec);
				}
			}
			info.msg.ioEntity.outputs.Add(ioconnection2);
		}
	}

	// Token: 0x06000CF0 RID: 3312 RVA: 0x0006F6A8 File Offset: 0x0006D8A8
	public virtual float IOInput(global::IOEntity from, global::IOEntity.IOType inputType, float inputAmount, int slot = 0)
	{
		foreach (global::IOEntity.IOSlot ioslot in this.outputs)
		{
			if (ioslot.connectedTo.Get(true) != null)
			{
				inputAmount = ioslot.connectedTo.Get(true).IOInput(this, ioslot.type, inputAmount, ioslot.connectedToSlot);
			}
		}
		return inputAmount;
	}

	// Token: 0x17000138 RID: 312
	// (get) Token: 0x06000CF1 RID: 3313 RVA: 0x00007A3C File Offset: 0x00005C3C
	public virtual bool BlockFluidDraining
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06000CF2 RID: 3314 RVA: 0x0006F704 File Offset: 0x0006D904
	public void FindContainerSource(List<global::IOEntity.ContainerInputOutput> found, int depth, bool input, int parentId = -1, int stackSize = 0)
	{
		global::IOEntity.<>c__DisplayClass86_0 CS$<>8__locals1;
		CS$<>8__locals1.found = found;
		if (depth <= 0 || CS$<>8__locals1.found.Count >= 32)
		{
			return;
		}
		int num = 0;
		int num2 = 1;
		if (!input)
		{
			num2 = 0;
			global::IOEntity.IOSlot[] array = this.outputs;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].type == global::IOEntity.IOType.Industrial)
				{
					num2++;
				}
			}
		}
		List<int> list = Facepunch.Pool.GetList<int>();
		foreach (global::IOEntity.IOSlot ioslot in input ? this.inputs : this.outputs)
		{
			num++;
			if (ioslot.type == global::IOEntity.IOType.Industrial)
			{
				global::IOEntity ioentity = ioslot.connectedTo.Get(base.isServer);
				if (ioentity != null)
				{
					int num3 = -1;
					IIndustrialStorage storage;
					if ((storage = (ioentity as IIndustrialStorage)) != null)
					{
						num = ioslot.connectedToSlot;
						if (global::IOEntity.<FindContainerSource>g__GetExistingCount|86_0(storage, ref CS$<>8__locals1) < 2)
						{
							CS$<>8__locals1.found.Add(new global::IOEntity.ContainerInputOutput
							{
								SlotIndex = num,
								Storage = storage,
								ParentStorage = parentId,
								MaxStackSize = stackSize / num2
							});
							num3 = CS$<>8__locals1.found.Count - 1;
							list.Add(num3);
						}
					}
					if ((!(ioentity is IIndustrialStorage) || ioentity is IndustrialStorageAdaptor) && !(ioentity is global::IndustrialConveyor) && ioentity != null)
					{
						ioentity.FindContainerSource(CS$<>8__locals1.found, depth - 1, input, (num3 == -1) ? parentId : num3, stackSize / num2);
					}
				}
			}
		}
		int count = list.Count;
		foreach (int index in list)
		{
			global::IOEntity.ContainerInputOutput value = CS$<>8__locals1.found[index];
			value.IndustrialSiblingCount = count;
			CS$<>8__locals1.found[index] = value;
		}
		Facepunch.Pool.FreeList<int>(ref list);
	}

	// Token: 0x17000139 RID: 313
	// (get) Token: 0x06000CF3 RID: 3315 RVA: 0x00006CA5 File Offset: 0x00004EA5
	protected virtual float LiquidPassthroughGravityThreshold
	{
		get
		{
			return 1f;
		}
	}

	// Token: 0x1700013A RID: 314
	// (get) Token: 0x06000CF4 RID: 3316 RVA: 0x00007A3C File Offset: 0x00005C3C
	protected virtual bool DisregardGravityRestrictionsOnLiquid
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06000CF5 RID: 3317 RVA: 0x0006F8F8 File Offset: 0x0006DAF8
	public virtual bool AllowLiquidPassthrough(global::IOEntity fromSource, Vector3 sourceWorldPosition, bool forPlacement = false)
	{
		if (fromSource.DisregardGravityRestrictionsOnLiquid || this.DisregardGravityRestrictionsOnLiquid)
		{
			return true;
		}
		if (this.inputs.Length == 0)
		{
			return false;
		}
		Vector3 vector = base.transform.TransformPoint(this.inputs[0].handlePosition);
		float num = sourceWorldPosition.y - vector.y;
		return num > 0f || Mathf.Abs(num) < this.LiquidPassthroughGravityThreshold;
	}

	// Token: 0x06000CF6 RID: 3318 RVA: 0x0006F964 File Offset: 0x0006DB64
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity == null)
		{
			return;
		}
		if (!info.fromDisk && info.msg.ioEntity.inputs != null)
		{
			int count = info.msg.ioEntity.inputs.Count;
			if (this.inputs.Length != count)
			{
				this.inputs = new global::IOEntity.IOSlot[count];
			}
			for (int i = 0; i < count; i++)
			{
				if (this.inputs[i] == null)
				{
					this.inputs[i] = new global::IOEntity.IOSlot();
				}
				ProtoBuf.IOEntity.IOConnection ioconnection = info.msg.ioEntity.inputs[i];
				this.inputs[i].connectedTo = new global::IOEntity.IORef();
				this.inputs[i].connectedTo.entityRef.uid = ioconnection.connectedID;
				if (base.isClient)
				{
					this.inputs[i].connectedTo.InitClient();
				}
				this.inputs[i].connectedToSlot = ioconnection.connectedToSlot;
				this.inputs[i].niceName = ioconnection.niceName;
				this.inputs[i].type = (global::IOEntity.IOType)ioconnection.type;
				this.inputs[i].wireColour = (WireTool.WireColour)ioconnection.colour;
				this.inputs[i].lineThickness = ioconnection.lineThickness;
			}
		}
		if (info.msg.ioEntity.outputs != null)
		{
			int count2 = info.msg.ioEntity.outputs.Count;
			if (this.outputs.Length != count2 && count2 > 0)
			{
				global::IOEntity.IOSlot[] array = this.outputs;
				this.outputs = new global::IOEntity.IOSlot[count2];
				for (int j = 0; j < array.Length; j++)
				{
					if (j < count2)
					{
						this.outputs[j] = array[j];
					}
				}
			}
			for (int k = 0; k < count2; k++)
			{
				if (this.outputs[k] == null)
				{
					this.outputs[k] = new global::IOEntity.IOSlot();
				}
				ProtoBuf.IOEntity.IOConnection ioconnection2 = info.msg.ioEntity.outputs[k];
				if (ioconnection2.linePointList == null || ioconnection2.linePointList.Count == 0 || !ioconnection2.connectedID.IsValid)
				{
					this.outputs[k].Clear();
				}
				this.outputs[k].connectedTo = new global::IOEntity.IORef();
				this.outputs[k].connectedTo.entityRef.uid = ioconnection2.connectedID;
				if (base.isClient)
				{
					this.outputs[k].connectedTo.InitClient();
				}
				this.outputs[k].connectedToSlot = ioconnection2.connectedToSlot;
				this.outputs[k].niceName = ioconnection2.niceName;
				this.outputs[k].type = (global::IOEntity.IOType)ioconnection2.type;
				this.outputs[k].wireColour = (WireTool.WireColour)ioconnection2.colour;
				this.outputs[k].worldSpaceLineEndRotation = ioconnection2.worldSpaceRotation;
				this.outputs[k].lineThickness = ioconnection2.lineThickness;
				if (info.fromDisk || base.isClient)
				{
					List<ProtoBuf.IOEntity.IOConnection.LineVec> linePointList = ioconnection2.linePointList;
					if (this.outputs[k].linePoints == null || this.outputs[k].linePoints.Length != linePointList.Count)
					{
						this.outputs[k].linePoints = new Vector3[linePointList.Count];
					}
					if (this.outputs[k].slackLevels == null || this.outputs[k].slackLevels.Length != linePointList.Count)
					{
						this.outputs[k].slackLevels = new float[linePointList.Count];
					}
					for (int l = 0; l < linePointList.Count; l++)
					{
						this.outputs[k].linePoints[l] = linePointList[l].vec;
						this.outputs[k].slackLevels[l] = linePointList[l].vec.w;
					}
				}
			}
		}
		this.RefreshIndustrialPreventBuilding();
	}

	// Token: 0x06000CF7 RID: 3319 RVA: 0x0006FD84 File Offset: 0x0006DF84
	public int GetConnectedInputCount()
	{
		int num = 0;
		global::IOEntity.IOSlot[] array = this.inputs;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].connectedTo.Get(base.isServer) != null)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x06000CF8 RID: 3320 RVA: 0x0006FDC8 File Offset: 0x0006DFC8
	public int GetConnectedOutputCount()
	{
		int num = 0;
		global::IOEntity.IOSlot[] array = this.outputs;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].connectedTo.Get(base.isServer) != null)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x06000CF9 RID: 3321 RVA: 0x0006FE0C File Offset: 0x0006E00C
	public bool HasConnections()
	{
		return this.GetConnectedInputCount() > 0 || this.GetConnectedOutputCount() > 0;
	}

	// Token: 0x06000CFA RID: 3322 RVA: 0x0006FE22 File Offset: 0x0006E022
	public override void DestroyShared()
	{
		base.DestroyShared();
		this.ClearIndustrialPreventBuilding();
	}

	// Token: 0x06000CFB RID: 3323 RVA: 0x0006FE30 File Offset: 0x0006E030
	public void RefreshIndustrialPreventBuilding()
	{
		this.ClearIndustrialPreventBuilding();
		Matrix4x4 localToWorldMatrix = base.transform.localToWorldMatrix;
		for (int i = 0; i < this.outputs.Length; i++)
		{
			global::IOEntity.IOSlot ioslot = this.outputs[i];
			if (ioslot.type == global::IOEntity.IOType.Industrial && ioslot.linePoints != null && ioslot.linePoints.Length > 1)
			{
				Vector3 b = localToWorldMatrix.MultiplyPoint3x4(ioslot.linePoints[0]);
				for (int j = 1; j < ioslot.linePoints.Length; j++)
				{
					Vector3 vector = localToWorldMatrix.MultiplyPoint3x4(ioslot.linePoints[j]);
					Vector3 pos = Vector3.Lerp(vector, b, 0.5f);
					float z = Vector3.Distance(vector, b);
					Quaternion rot = Quaternion.LookRotation((vector - b).normalized);
					GameObject gameObject = base.gameManager.CreatePrefab("assets/prefabs/misc/ioentitypreventbuilding.prefab", pos, rot, true);
					gameObject.transform.SetParent(base.transform);
					BoxCollider boxCollider;
					if (gameObject.TryGetComponent<BoxCollider>(out boxCollider))
					{
						boxCollider.size = new Vector3(0.1f, 0.1f, z);
						this.spawnedColliders.Add(boxCollider);
					}
					ColliderInfo_Pipe colliderInfo_Pipe;
					if (gameObject.TryGetComponent<ColliderInfo_Pipe>(out colliderInfo_Pipe))
					{
						colliderInfo_Pipe.OutputSlotIndex = i;
						colliderInfo_Pipe.ParentEntity = this;
					}
					b = vector;
				}
			}
		}
	}

	// Token: 0x06000CFC RID: 3324 RVA: 0x0006FF78 File Offset: 0x0006E178
	private void ClearIndustrialPreventBuilding()
	{
		foreach (BoxCollider boxCollider in this.spawnedColliders)
		{
			base.gameManager.Retire(boxCollider.gameObject);
		}
		this.spawnedColliders.Clear();
	}

	// Token: 0x06000CFF RID: 3327 RVA: 0x00070034 File Offset: 0x0006E234
	[CompilerGenerated]
	internal static int <FindContainerSource>g__GetExistingCount|86_0(IIndustrialStorage storage, ref global::IOEntity.<>c__DisplayClass86_0 A_1)
	{
		int num = 0;
		using (List<global::IOEntity.ContainerInputOutput>.Enumerator enumerator = A_1.found.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.Storage == storage)
				{
					num++;
				}
			}
		}
		return num;
	}

	// Token: 0x02000BD7 RID: 3031
	public enum IOType
	{
		// Token: 0x04004104 RID: 16644
		Electric,
		// Token: 0x04004105 RID: 16645
		Fluidic,
		// Token: 0x04004106 RID: 16646
		Kinetic,
		// Token: 0x04004107 RID: 16647
		Generic,
		// Token: 0x04004108 RID: 16648
		Industrial
	}

	// Token: 0x02000BD8 RID: 3032
	[Serializable]
	public class IORef
	{
		// Token: 0x04004109 RID: 16649
		public EntityRef entityRef;

		// Token: 0x0400410A RID: 16650
		public global::IOEntity ioEnt;

		// Token: 0x06004DAA RID: 19882 RVA: 0x001A1404 File Offset: 0x0019F604
		public void Init()
		{
			if (this.ioEnt != null && !this.entityRef.IsValid(true))
			{
				this.entityRef.Set(this.ioEnt);
			}
			if (this.entityRef.IsValid(true))
			{
				this.ioEnt = this.entityRef.Get(true).GetComponent<global::IOEntity>();
			}
		}

		// Token: 0x06004DAB RID: 19883 RVA: 0x001A1463 File Offset: 0x0019F663
		public void InitClient()
		{
			if (this.entityRef.IsValid(false) && this.ioEnt == null)
			{
				this.ioEnt = this.entityRef.Get(false).GetComponent<global::IOEntity>();
			}
		}

		// Token: 0x06004DAC RID: 19884 RVA: 0x001A1498 File Offset: 0x0019F698
		public global::IOEntity Get(bool isServer = true)
		{
			if (this.ioEnt == null && this.entityRef.IsValid(isServer))
			{
				this.ioEnt = (this.entityRef.Get(isServer) as global::IOEntity);
			}
			return this.ioEnt;
		}

		// Token: 0x06004DAD RID: 19885 RVA: 0x001A14D3 File Offset: 0x0019F6D3
		public void Clear()
		{
			this.ioEnt = null;
			this.entityRef.Set(null);
		}

		// Token: 0x06004DAE RID: 19886 RVA: 0x001A14E8 File Offset: 0x0019F6E8
		public void Set(global::IOEntity newIOEnt)
		{
			this.entityRef.Set(newIOEnt);
		}
	}

	// Token: 0x02000BD9 RID: 3033
	[Serializable]
	public class IOSlot
	{
		// Token: 0x0400410B RID: 16651
		public string niceName;

		// Token: 0x0400410C RID: 16652
		public global::IOEntity.IOType type;

		// Token: 0x0400410D RID: 16653
		public global::IOEntity.IORef connectedTo;

		// Token: 0x0400410E RID: 16654
		public int connectedToSlot;

		// Token: 0x0400410F RID: 16655
		public Vector3[] linePoints;

		// Token: 0x04004110 RID: 16656
		public float[] slackLevels;

		// Token: 0x04004111 RID: 16657
		public Vector3 worldSpaceLineEndRotation;

		// Token: 0x04004112 RID: 16658
		public ClientIOLine line;

		// Token: 0x04004113 RID: 16659
		public Vector3 handlePosition;

		// Token: 0x04004114 RID: 16660
		public Vector3 handleDirection;

		// Token: 0x04004115 RID: 16661
		public bool rootConnectionsOnly;

		// Token: 0x04004116 RID: 16662
		public bool mainPowerSlot;

		// Token: 0x04004117 RID: 16663
		public WireTool.WireColour wireColour;

		// Token: 0x04004118 RID: 16664
		public float lineThickness;

		// Token: 0x06004DB0 RID: 19888 RVA: 0x001A14F6 File Offset: 0x0019F6F6
		public void Clear()
		{
			if (this.connectedTo == null)
			{
				this.connectedTo = new global::IOEntity.IORef();
			}
			else
			{
				this.connectedTo.Clear();
			}
			this.connectedToSlot = 0;
			this.linePoints = null;
		}
	}

	// Token: 0x02000BDA RID: 3034
	private struct FrameTiming
	{
		// Token: 0x04004119 RID: 16665
		public string PrefabName;

		// Token: 0x0400411A RID: 16666
		public float Time;
	}

	// Token: 0x02000BDB RID: 3035
	public struct ContainerInputOutput
	{
		// Token: 0x0400411B RID: 16667
		public IIndustrialStorage Storage;

		// Token: 0x0400411C RID: 16668
		public int SlotIndex;

		// Token: 0x0400411D RID: 16669
		public int MaxStackSize;

		// Token: 0x0400411E RID: 16670
		public int ParentStorage;

		// Token: 0x0400411F RID: 16671
		public int IndustrialSiblingCount;
	}
}
