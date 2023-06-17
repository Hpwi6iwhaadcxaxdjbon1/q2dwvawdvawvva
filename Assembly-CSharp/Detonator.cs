using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000067 RID: 103
public class Detonator : global::HeldEntity, IRFObject
{
	// Token: 0x040006C4 RID: 1732
	public int frequency = 55;

	// Token: 0x040006C5 RID: 1733
	private float timeSinceDeploy;

	// Token: 0x040006C6 RID: 1734
	public GameObjectRef frequencyPanelPrefab;

	// Token: 0x040006C7 RID: 1735
	public GameObjectRef attackEffect;

	// Token: 0x040006C8 RID: 1736
	public GameObjectRef unAttackEffect;

	// Token: 0x040006C9 RID: 1737
	private float nextChangeTime;

	// Token: 0x06000A58 RID: 2648 RVA: 0x0005F2B4 File Offset: 0x0005D4B4
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Detonator.OnRpcMessage", 0))
		{
			if (rpc == 2778616053U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ServerSetFrequency ");
				}
				using (TimeWarning.New("ServerSetFrequency", 0))
				{
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
							this.ServerSetFrequency(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in ServerSetFrequency");
					}
				}
				return true;
			}
			if (rpc == 1106698135U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SetPressed ");
				}
				using (TimeWarning.New("SetPressed", 0))
				{
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage pressed = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SetPressed(pressed);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in SetPressed");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000A59 RID: 2649 RVA: 0x0005F514 File Offset: 0x0005D714
	[global::BaseEntity.RPC_Server]
	public void SetPressed(global::BaseEntity.RPCMessage msg)
	{
		if (msg.player == null || msg.player != base.GetOwnerPlayer())
		{
			return;
		}
		bool flag = base.HasFlag(global::BaseEntity.Flags.On);
		bool flag2 = msg.read.Bit();
		this.InternalSetPressed(flag2);
		if (flag != flag2)
		{
			Effect.server.Run(flag2 ? this.attackEffect.resourcePath : this.unAttackEffect.resourcePath, this, 0U, Vector3.zero, Vector3.zero, null, false);
		}
	}

	// Token: 0x06000A5A RID: 2650 RVA: 0x0005F58E File Offset: 0x0005D78E
	internal void InternalSetPressed(bool pressed)
	{
		base.SetFlag(global::BaseEntity.Flags.On, pressed, false, true);
		if (pressed)
		{
			RFManager.AddBroadcaster(this.frequency, this);
			return;
		}
		RFManager.RemoveBroadcaster(this.frequency, this);
	}

	// Token: 0x06000A5B RID: 2651 RVA: 0x0002C673 File Offset: 0x0002A873
	public Vector3 GetPosition()
	{
		return base.transform.position;
	}

	// Token: 0x06000A5C RID: 2652 RVA: 0x0005F5B6 File Offset: 0x0005D7B6
	public float GetMaxRange()
	{
		return 100000f;
	}

	// Token: 0x06000A5D RID: 2653 RVA: 0x000063A5 File Offset: 0x000045A5
	public void RFSignalUpdate(bool on)
	{
	}

	// Token: 0x06000A5E RID: 2654 RVA: 0x0005F5BD File Offset: 0x0005D7BD
	public override void SetHeld(bool bHeld)
	{
		if (!bHeld)
		{
			this.InternalSetPressed(false);
		}
		base.SetHeld(bHeld);
	}

	// Token: 0x06000A5F RID: 2655 RVA: 0x0005F5D0 File Offset: 0x0005D7D0
	[global::BaseEntity.RPC_Server]
	public void ServerSetFrequency(global::BaseEntity.RPCMessage msg)
	{
		this.ServerSetFrequency(msg.player, msg.read.Int32());
	}

	// Token: 0x06000A60 RID: 2656 RVA: 0x0005F5EC File Offset: 0x0005D7EC
	public void ServerSetFrequency(global::BasePlayer player, int freq)
	{
		if (player == null)
		{
			return;
		}
		if (base.GetOwnerPlayer() != player)
		{
			return;
		}
		if (UnityEngine.Time.time < this.nextChangeTime)
		{
			return;
		}
		this.nextChangeTime = UnityEngine.Time.time + 2f;
		if (RFManager.IsReserved(freq))
		{
			RFManager.ReserveErrorPrint(player);
			return;
		}
		global::Item ownerItem = base.GetOwnerItem();
		RFManager.ChangeFrequency(this.frequency, freq, this, false, base.IsOn());
		this.frequency = freq;
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		global::Item item = this.GetItem();
		if (item != null)
		{
			if (item.instanceData == null)
			{
				item.instanceData = new ProtoBuf.Item.InstanceData();
				item.instanceData.ShouldPool = false;
			}
			item.instanceData.dataInt = this.frequency;
			item.MarkDirty();
		}
		if (ownerItem != null)
		{
			ownerItem.LoseCondition(ownerItem.maxCondition * 0.01f);
		}
	}

	// Token: 0x06000A61 RID: 2657 RVA: 0x0005F6BE File Offset: 0x0005D8BE
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.ioEntity == null)
		{
			info.msg.ioEntity = Facepunch.Pool.Get<ProtoBuf.IOEntity>();
		}
		info.msg.ioEntity.genericInt1 = this.frequency;
	}

	// Token: 0x06000A62 RID: 2658 RVA: 0x0005F6FA File Offset: 0x0005D8FA
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.frequency = info.msg.ioEntity.genericInt1;
		}
	}

	// Token: 0x06000A63 RID: 2659 RVA: 0x0005F726 File Offset: 0x0005D926
	public int GetFrequency()
	{
		return this.frequency;
	}
}
