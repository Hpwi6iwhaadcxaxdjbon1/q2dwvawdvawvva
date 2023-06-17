using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000073 RID: 115
public class ExcavatorArm : global::BaseEntity
{
	// Token: 0x04000719 RID: 1817
	public float yaw1;

	// Token: 0x0400071A RID: 1818
	public float yaw2;

	// Token: 0x0400071B RID: 1819
	public Transform wheel;

	// Token: 0x0400071C RID: 1820
	public float wheelSpeed = 2f;

	// Token: 0x0400071D RID: 1821
	public float turnSpeed = 0.1f;

	// Token: 0x0400071E RID: 1822
	public Transform miningOffset;

	// Token: 0x0400071F RID: 1823
	public GameObjectRef bounceEffect;

	// Token: 0x04000720 RID: 1824
	public LightGroupAtTime lights;

	// Token: 0x04000721 RID: 1825
	public Material conveyorMaterial;

	// Token: 0x04000722 RID: 1826
	public float beltSpeedMax = 0.1f;

	// Token: 0x04000723 RID: 1827
	public const global::BaseEntity.Flags Flag_HasPower = global::BaseEntity.Flags.Reserved8;

	// Token: 0x04000724 RID: 1828
	public List<ExcavatorOutputPile> outputPiles;

	// Token: 0x04000725 RID: 1829
	public SoundDefinition miningStartButtonSoundDef;

	// Token: 0x04000726 RID: 1830
	[Header("Production")]
	public ItemAmount[] resourcesToMine;

	// Token: 0x04000727 RID: 1831
	public float resourceProductionTickRate = 3f;

	// Token: 0x04000728 RID: 1832
	public float timeForFullResources = 120f;

	// Token: 0x04000729 RID: 1833
	private ItemAmount[] pendingResources;

	// Token: 0x0400072A RID: 1834
	public Translate.Phrase excavatorPhrase;

	// Token: 0x0400072B RID: 1835
	private float movedAmount;

	// Token: 0x0400072C RID: 1836
	private float currentTurnThrottle;

	// Token: 0x0400072D RID: 1837
	private float lastMoveYaw;

	// Token: 0x0400072E RID: 1838
	private float excavatorStartTime;

	// Token: 0x0400072F RID: 1839
	private float nextNotificationTime;

	// Token: 0x04000730 RID: 1840
	private int resourceMiningIndex;

	// Token: 0x06000AF1 RID: 2801 RVA: 0x00062EF0 File Offset: 0x000610F0
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ExcavatorArm.OnRpcMessage", 0))
		{
			if (rpc == 2059417170U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_SetResourceTarget ");
				}
				using (TimeWarning.New("RPC_SetResourceTarget", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(2059417170U, "RPC_SetResourceTarget", this, player, 3f))
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
							this.RPC_SetResourceTarget(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_SetResourceTarget");
					}
				}
				return true;
			}
			if (rpc == 2882020740U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_StopMining ");
				}
				using (TimeWarning.New("RPC_StopMining", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(2882020740U, "RPC_StopMining", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg3 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_StopMining(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in RPC_StopMining");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000AF2 RID: 2802 RVA: 0x00003278 File Offset: 0x00001478
	public bool IsPowered()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved8);
	}

	// Token: 0x06000AF3 RID: 2803 RVA: 0x00007641 File Offset: 0x00005841
	public bool IsMining()
	{
		return base.IsOn();
	}

	// Token: 0x1700010E RID: 270
	// (get) Token: 0x06000AF4 RID: 2804 RVA: 0x000348EE File Offset: 0x00032AEE
	protected override float PositionTickRate
	{
		get
		{
			return 0.05f;
		}
	}

	// Token: 0x06000AF5 RID: 2805 RVA: 0x0000627E File Offset: 0x0000447E
	public override float GetNetworkTime()
	{
		return UnityEngine.Time.fixedTime;
	}

	// Token: 0x06000AF6 RID: 2806 RVA: 0x000631F0 File Offset: 0x000613F0
	public void FixedUpdate()
	{
		if (base.isClient)
		{
			return;
		}
		bool flag = this.IsMining() && this.IsPowered();
		float num = flag ? 1f : 0f;
		this.currentTurnThrottle = Mathf.Lerp(this.currentTurnThrottle, num, UnityEngine.Time.fixedDeltaTime * (flag ? 0.333f : 1f));
		if (Mathf.Abs(num - this.currentTurnThrottle) < 0.025f)
		{
			this.currentTurnThrottle = num;
		}
		this.movedAmount += UnityEngine.Time.fixedDeltaTime * this.turnSpeed * this.currentTurnThrottle;
		float t = (Mathf.Sin(this.movedAmount) + 1f) / 2f;
		float num2 = Mathf.Lerp(this.yaw1, this.yaw2, t);
		if (num2 != this.lastMoveYaw)
		{
			this.lastMoveYaw = num2;
			base.transform.rotation = Quaternion.Euler(0f, num2, 0f);
			base.transform.hasChanged = true;
		}
	}

	// Token: 0x06000AF7 RID: 2807 RVA: 0x000632EC File Offset: 0x000614EC
	public void BeginMining()
	{
		if (!this.IsPowered())
		{
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.On, true, false, true);
		base.InvokeRepeating(new Action(this.ProduceResources), this.resourceProductionTickRate, this.resourceProductionTickRate);
		if (UnityEngine.Time.time > this.nextNotificationTime)
		{
			foreach (global::BasePlayer basePlayer in global::BasePlayer.activePlayerList)
			{
				if (!basePlayer.IsNpc && basePlayer.IsConnected)
				{
					basePlayer.ShowToast(GameTip.Styles.Server_Event, this.excavatorPhrase, Array.Empty<string>());
				}
			}
			this.nextNotificationTime = UnityEngine.Time.time + 60f;
		}
		ExcavatorServerEffects.SetMining(true, false);
		Analytics.Server.ExcavatorStarted();
		this.excavatorStartTime = this.GetNetworkTime();
	}

	// Token: 0x06000AF8 RID: 2808 RVA: 0x000633C0 File Offset: 0x000615C0
	public void StopMining()
	{
		ExcavatorServerEffects.SetMining(false, false);
		base.CancelInvoke(new Action(this.ProduceResources));
		if (base.HasFlag(global::BaseEntity.Flags.On))
		{
			Analytics.Server.ExcavatorStopped(this.GetNetworkTime() - this.excavatorStartTime);
		}
		base.SetFlag(global::BaseEntity.Flags.On, false, false, true);
	}

	// Token: 0x06000AF9 RID: 2809 RVA: 0x00063400 File Offset: 0x00061600
	public void ProduceResources()
	{
		float num = this.resourceProductionTickRate / this.timeForFullResources;
		float num2 = this.resourcesToMine[this.resourceMiningIndex].amount * num;
		this.pendingResources[this.resourceMiningIndex].amount += num2;
		foreach (ItemAmount itemAmount in this.pendingResources)
		{
			if (itemAmount.amount >= (float)this.outputPiles.Count)
			{
				int num3 = Mathf.FloorToInt(itemAmount.amount / (float)this.outputPiles.Count);
				itemAmount.amount -= (float)(num3 * 2);
				foreach (ExcavatorOutputPile excavatorOutputPile in this.outputPiles)
				{
					global::Item item = ItemManager.Create(this.resourcesToMine[this.resourceMiningIndex].itemDef, num3, 0UL);
					Analytics.Azure.OnExcavatorProduceItem(item, this);
					if (!item.MoveToContainer(excavatorOutputPile.inventory, -1, true, false, null, true))
					{
						item.Drop(excavatorOutputPile.GetDropPosition(), excavatorOutputPile.GetDropVelocity(), default(Quaternion));
					}
				}
			}
		}
	}

	// Token: 0x06000AFA RID: 2810 RVA: 0x0006354C File Offset: 0x0006174C
	public override void OnEntityMessage(global::BaseEntity from, string msg)
	{
		base.OnEntityMessage(from, msg);
		if (msg == "DieselEngineOn")
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved8, true, false, true);
			return;
		}
		if (msg == "DieselEngineOff")
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved8, false, false, true);
			this.StopMining();
		}
	}

	// Token: 0x06000AFB RID: 2811 RVA: 0x000635A0 File Offset: 0x000617A0
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_SetResourceTarget(global::BaseEntity.RPCMessage msg)
	{
		string a = msg.read.String(256);
		if (a == "HQM")
		{
			this.resourceMiningIndex = 0;
		}
		else if (a == "Sulfur")
		{
			this.resourceMiningIndex = 1;
		}
		else if (a == "Stone")
		{
			this.resourceMiningIndex = 2;
		}
		else if (a == "Metal")
		{
			this.resourceMiningIndex = 3;
		}
		if (!base.IsOn())
		{
			this.BeginMining();
		}
	}

	// Token: 0x06000AFC RID: 2812 RVA: 0x000063A5 File Offset: 0x000045A5
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_StopMining(global::BaseEntity.RPCMessage msg)
	{
	}

	// Token: 0x06000AFD RID: 2813 RVA: 0x00063622 File Offset: 0x00061822
	public override void Spawn()
	{
		base.Spawn();
	}

	// Token: 0x06000AFE RID: 2814 RVA: 0x0006362A File Offset: 0x0006182A
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.Init();
		if (base.IsOn() && this.IsPowered())
		{
			this.BeginMining();
			return;
		}
		this.StopMining();
	}

	// Token: 0x06000AFF RID: 2815 RVA: 0x00063658 File Offset: 0x00061858
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.ioEntity = Facepunch.Pool.Get<ProtoBuf.IOEntity>();
		info.msg.ioEntity.genericFloat1 = this.movedAmount;
		info.msg.ioEntity.genericInt1 = this.resourceMiningIndex;
	}

	// Token: 0x06000B00 RID: 2816 RVA: 0x000636A8 File Offset: 0x000618A8
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.movedAmount = info.msg.ioEntity.genericFloat1;
			this.resourceMiningIndex = info.msg.ioEntity.genericInt1;
		}
	}

	// Token: 0x06000B01 RID: 2817 RVA: 0x000636F5 File Offset: 0x000618F5
	public override void PostMapEntitySpawn()
	{
		base.PostMapEntitySpawn();
		this.Init();
	}

	// Token: 0x06000B02 RID: 2818 RVA: 0x00063704 File Offset: 0x00061904
	public void Init()
	{
		this.pendingResources = new ItemAmount[this.resourcesToMine.Length];
		for (int i = 0; i < this.resourcesToMine.Length; i++)
		{
			this.pendingResources[i] = new ItemAmount(this.resourcesToMine[i].itemDef, 0f);
		}
		List<ExcavatorOutputPile> list = Facepunch.Pool.GetList<ExcavatorOutputPile>();
		global::Vis.Entities<ExcavatorOutputPile>(base.transform.position, 200f, list, 512, QueryTriggerInteraction.Collide);
		this.outputPiles = new List<ExcavatorOutputPile>();
		foreach (ExcavatorOutputPile excavatorOutputPile in list)
		{
			if (!excavatorOutputPile.isClient)
			{
				this.outputPiles.Add(excavatorOutputPile);
			}
		}
		Facepunch.Pool.FreeList<ExcavatorOutputPile>(ref list);
	}
}
