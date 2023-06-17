using System;
using ConVar;
using Facepunch.Rust;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000054 RID: 84
public class CardReader : IOEntity
{
	// Token: 0x0400061D RID: 1565
	public float accessDuration = 10f;

	// Token: 0x0400061E RID: 1566
	public int accessLevel;

	// Token: 0x0400061F RID: 1567
	public GameObjectRef accessGrantedEffect;

	// Token: 0x04000620 RID: 1568
	public GameObjectRef accessDeniedEffect;

	// Token: 0x04000621 RID: 1569
	public GameObjectRef swipeEffect;

	// Token: 0x04000622 RID: 1570
	public Transform audioPosition;

	// Token: 0x04000623 RID: 1571
	public BaseEntity.Flags AccessLevel1 = BaseEntity.Flags.Reserved1;

	// Token: 0x04000624 RID: 1572
	public BaseEntity.Flags AccessLevel2 = BaseEntity.Flags.Reserved2;

	// Token: 0x04000625 RID: 1573
	public BaseEntity.Flags AccessLevel3 = BaseEntity.Flags.Reserved3;

	// Token: 0x06000931 RID: 2353 RVA: 0x00057D78 File Offset: 0x00055F78
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("CardReader.OnRpcMessage", 0))
		{
			if (rpc == 979061374U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ServerCardSwiped ");
				}
				using (TimeWarning.New("ServerCardSwiped", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(979061374U, "ServerCardSwiped", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage msg2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.ServerCardSwiped(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in ServerCardSwiped");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000932 RID: 2354 RVA: 0x00057EE0 File Offset: 0x000560E0
	public override void ResetIOState()
	{
		base.ResetIOState();
		base.CancelInvoke(new Action(this.GrantCard));
		base.CancelInvoke(new Action(this.CancelAccess));
		this.CancelAccess();
	}

	// Token: 0x06000933 RID: 2355 RVA: 0x00057F12 File Offset: 0x00056112
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (!base.IsOn())
		{
			return 0;
		}
		return base.GetPassthroughAmount(outputSlot);
	}

	// Token: 0x06000934 RID: 2356 RVA: 0x00057F25 File Offset: 0x00056125
	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
	}

	// Token: 0x06000935 RID: 2357 RVA: 0x00057F2F File Offset: 0x0005612F
	public void CancelAccess()
	{
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
		this.MarkDirty();
	}

	// Token: 0x06000936 RID: 2358 RVA: 0x00057F41 File Offset: 0x00056141
	public void FailCard()
	{
		Effect.server.Run(this.accessDeniedEffect.resourcePath, this.audioPosition.position, Vector3.up, null, false);
	}

	// Token: 0x06000937 RID: 2359 RVA: 0x00057F68 File Offset: 0x00056168
	public override void ServerInit()
	{
		base.ServerInit();
		base.SetFlag(this.AccessLevel1, this.accessLevel == 1, false, true);
		base.SetFlag(this.AccessLevel2, this.accessLevel == 2, false, true);
		base.SetFlag(this.AccessLevel3, this.accessLevel == 3, false, true);
	}

	// Token: 0x06000938 RID: 2360 RVA: 0x00057FC0 File Offset: 0x000561C0
	public void GrantCard()
	{
		base.SetFlag(BaseEntity.Flags.On, true, false, true);
		this.MarkDirty();
		Effect.server.Run(this.accessGrantedEffect.resourcePath, this.audioPosition.position, Vector3.up, null, false);
	}

	// Token: 0x06000939 RID: 2361 RVA: 0x00057FF4 File Offset: 0x000561F4
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void ServerCardSwiped(BaseEntity.RPCMessage msg)
	{
		if (!this.IsPowered())
		{
			return;
		}
		if (Vector3Ex.Distance2D(msg.player.transform.position, base.transform.position) > 1f)
		{
			return;
		}
		if (base.IsInvoking(new Action(this.GrantCard)) || base.IsInvoking(new Action(this.FailCard)))
		{
			return;
		}
		if (base.HasFlag(BaseEntity.Flags.On))
		{
			return;
		}
		NetworkableId uid = msg.read.EntityID();
		Keycard keycard = BaseNetworkable.serverEntities.Find(uid) as Keycard;
		Effect.server.Run(this.swipeEffect.resourcePath, this.audioPosition.position, Vector3.up, msg.player.net.connection, false);
		if (keycard != null)
		{
			Item item = keycard.GetItem();
			if (item != null && keycard.accessLevel == this.accessLevel && item.conditionNormalized > 0f)
			{
				Analytics.Azure.OnKeycardSwiped(msg.player, this);
				base.Invoke(new Action(this.GrantCard), 0.5f);
				item.LoseCondition(1f);
				return;
			}
			base.Invoke(new Action(this.FailCard), 0.5f);
		}
	}

	// Token: 0x0600093A RID: 2362 RVA: 0x00058127 File Offset: 0x00056327
	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.ioEntity.genericInt1 = this.accessLevel;
		info.msg.ioEntity.genericFloat1 = this.accessDuration;
	}

	// Token: 0x0600093B RID: 2363 RVA: 0x0005815C File Offset: 0x0005635C
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.accessLevel = info.msg.ioEntity.genericInt1;
			this.accessDuration = info.msg.ioEntity.genericFloat1;
		}
	}
}
