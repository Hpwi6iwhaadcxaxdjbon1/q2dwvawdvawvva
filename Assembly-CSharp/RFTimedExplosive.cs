using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000C2 RID: 194
public class RFTimedExplosive : global::TimedExplosive, IRFObject
{
	// Token: 0x04000AD5 RID: 2773
	public SoundPlayer beepLoop;

	// Token: 0x04000AD6 RID: 2774
	private ulong creatorPlayerID;

	// Token: 0x04000AD7 RID: 2775
	public ItemDefinition pickupDefinition;

	// Token: 0x04000AD8 RID: 2776
	public float minutesUntilDecayed = 1440f;

	// Token: 0x04000AD9 RID: 2777
	private int RFFrequency = -1;

	// Token: 0x04000ADA RID: 2778
	private float decayTickDuration = 3600f;

	// Token: 0x04000ADB RID: 2779
	private float minutesDecayed;

	// Token: 0x0600115E RID: 4446 RVA: 0x0008E1EC File Offset: 0x0008C3EC
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("RFTimedExplosive.OnRpcMessage", 0))
		{
			if (rpc == 2778075470U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Pickup ");
				}
				using (TimeWarning.New("Pickup", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(2778075470U, "Pickup", this, player, 3f))
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
							this.Pickup(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in Pickup");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600115F RID: 4447 RVA: 0x0002C673 File Offset: 0x0002A873
	public Vector3 GetPosition()
	{
		return base.transform.position;
	}

	// Token: 0x06001160 RID: 4448 RVA: 0x00081B87 File Offset: 0x0007FD87
	public float GetMaxRange()
	{
		return float.PositiveInfinity;
	}

	// Token: 0x06001161 RID: 4449 RVA: 0x0008E354 File Offset: 0x0008C554
	public void RFSignalUpdate(bool on)
	{
		if (this.IsArmed() && on && !base.IsInvoking(new Action(this.Explode)))
		{
			base.Invoke(new Action(this.Explode), UnityEngine.Random.Range(0f, 0.2f));
		}
	}

	// Token: 0x06001162 RID: 4450 RVA: 0x0008E3A2 File Offset: 0x0008C5A2
	public void SetFrequency(int newFreq)
	{
		RFManager.RemoveListener(this.RFFrequency, this);
		this.RFFrequency = newFreq;
		if (this.RFFrequency > 0)
		{
			RFManager.AddListener(this.RFFrequency, this);
		}
	}

	// Token: 0x06001163 RID: 4451 RVA: 0x0008E3CC File Offset: 0x0008C5CC
	public int GetFrequency()
	{
		return this.RFFrequency;
	}

	// Token: 0x06001164 RID: 4452 RVA: 0x0008E3D4 File Offset: 0x0008C5D4
	public override void SetFuse(float fuseLength)
	{
		if (base.isServer)
		{
			if (this.GetFrequency() > 0)
			{
				if (base.IsInvoking(new Action(this.Explode)))
				{
					base.CancelInvoke(new Action(this.Explode));
				}
				base.Invoke(new Action(this.ArmRF), fuseLength);
				base.SetFlag(global::BaseEntity.Flags.Reserved1, true, false, false);
				base.SetFlag(global::BaseEntity.Flags.Reserved2, true, false, true);
				return;
			}
			base.SetFuse(fuseLength);
		}
	}

	// Token: 0x06001165 RID: 4453 RVA: 0x0008E451 File Offset: 0x0008C651
	public void ArmRF()
	{
		base.SetFlag(global::BaseEntity.Flags.On, true, false, false);
		base.SetFlag(global::BaseEntity.Flags.Reserved2, false, false, true);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06001166 RID: 4454 RVA: 0x000517FC File Offset: 0x0004F9FC
	public void DisarmRF()
	{
		base.SetFlag(global::BaseEntity.Flags.On, false, false, true);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06001167 RID: 4455 RVA: 0x0008E474 File Offset: 0x0008C674
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.explosive == null)
		{
			info.msg.explosive = Facepunch.Pool.Get<ProtoBuf.TimedExplosive>();
		}
		if (info.forDisk)
		{
			info.msg.explosive.freq = this.GetFrequency();
		}
		info.msg.explosive.creatorID = this.creatorPlayerID;
	}

	// Token: 0x06001168 RID: 4456 RVA: 0x0008E4D9 File Offset: 0x0008C6D9
	public override void ServerInit()
	{
		base.ServerInit();
		this.SetFrequency(this.RFFrequency);
		base.InvokeRandomized(new Action(this.DecayCheck), this.decayTickDuration, this.decayTickDuration, 10f);
	}

	// Token: 0x06001169 RID: 4457 RVA: 0x0008E510 File Offset: 0x0008C710
	public void DecayCheck()
	{
		BuildingPrivlidge buildingPrivilege = this.GetBuildingPrivilege();
		global::BasePlayer basePlayer = global::BasePlayer.FindByID(this.creatorPlayerID);
		if (basePlayer != null && (buildingPrivilege == null || !buildingPrivilege.IsAuthed(basePlayer)))
		{
			this.minutesDecayed += this.decayTickDuration / 60f;
		}
		if (this.minutesDecayed >= this.minutesUntilDecayed)
		{
			base.Kill(global::BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x0600116A RID: 4458 RVA: 0x0008E57C File Offset: 0x0008C77C
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (this.RFFrequency > 0)
		{
			if (base.IsInvoking(new Action(this.Explode)))
			{
				base.CancelInvoke(new Action(this.Explode));
			}
			this.SetFrequency(this.RFFrequency);
			this.ArmRF();
		}
	}

	// Token: 0x0600116B RID: 4459 RVA: 0x0008E5D2 File Offset: 0x0008C7D2
	internal override void DoServerDestroy()
	{
		if (this.RFFrequency > 0)
		{
			RFManager.RemoveListener(this.RFFrequency, this);
		}
		base.DoServerDestroy();
	}

	// Token: 0x0600116C RID: 4460 RVA: 0x0008E5EF File Offset: 0x0008C7EF
	public void ChangeFrequency(int newFreq)
	{
		RFManager.ChangeFrequency(this.RFFrequency, newFreq, this, true, true);
		this.RFFrequency = newFreq;
	}

	// Token: 0x0600116D RID: 4461 RVA: 0x0008E608 File Offset: 0x0008C808
	public override void SetCreatorEntity(global::BaseEntity newCreatorEntity)
	{
		base.SetCreatorEntity(newCreatorEntity);
		global::BasePlayer component = newCreatorEntity.GetComponent<global::BasePlayer>();
		if (component)
		{
			this.creatorPlayerID = component.userID;
			if (this.GetFrequency() > 0)
			{
				component.ConsoleMessage("Frequency is:" + this.GetFrequency());
			}
		}
	}

	// Token: 0x0600116E RID: 4462 RVA: 0x0008E65C File Offset: 0x0008C85C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void Pickup(global::BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		if (!this.IsArmed())
		{
			return;
		}
		global::Item item = ItemManager.Create(this.pickupDefinition, 1, 0UL);
		if (item == null)
		{
			return;
		}
		item.instanceData.dataInt = this.GetFrequency();
		item.SetFlag(global::Item.Flag.IsOn, this.IsArmed());
		msg.player.GiveItem(item, global::BaseEntity.GiveItemReason.PickedUp);
		base.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x0600116F RID: 4463 RVA: 0x0002A4EC File Offset: 0x000286EC
	public bool IsArmed()
	{
		return base.HasFlag(global::BaseEntity.Flags.On);
	}

	// Token: 0x06001170 RID: 4464 RVA: 0x0008E6C8 File Offset: 0x0008C8C8
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.explosive != null)
		{
			this.creatorPlayerID = info.msg.explosive.creatorID;
			if (base.isServer)
			{
				if (info.fromDisk)
				{
					this.RFFrequency = info.msg.explosive.freq;
				}
				this.creatorEntity = global::BasePlayer.FindByID(this.creatorPlayerID);
			}
		}
	}

	// Token: 0x06001171 RID: 4465 RVA: 0x0008E736 File Offset: 0x0008C936
	public bool CanPickup(global::BasePlayer player)
	{
		return this.IsArmed();
	}
}
