using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000065 RID: 101
public class DeployedRecorder : StorageContainer, ICassettePlayer
{
	// Token: 0x040006BE RID: 1726
	public AudioSource SoundSource;

	// Token: 0x040006BF RID: 1727
	public ItemDefinition[] ValidCassettes;

	// Token: 0x040006C0 RID: 1728
	public SoundDefinition PlaySfx;

	// Token: 0x040006C1 RID: 1729
	public SoundDefinition StopSfx;

	// Token: 0x040006C2 RID: 1730
	public SwapKeycard TapeSwapper;

	// Token: 0x040006C3 RID: 1731
	private CollisionDetectionMode? initialCollisionDetectionMode;

	// Token: 0x06000A3E RID: 2622 RVA: 0x0005E8AC File Offset: 0x0005CAAC
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("DeployedRecorder.OnRpcMessage", 0))
		{
			if (rpc == 1785864031U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ServerTogglePlay ");
				}
				using (TimeWarning.New("ServerTogglePlay", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(1785864031U, "ServerTogglePlay", this, player, 3f))
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
							this.ServerTogglePlay(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in ServerTogglePlay");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x17000107 RID: 263
	// (get) Token: 0x06000A3F RID: 2623 RVA: 0x000037E7 File Offset: 0x000019E7
	public BaseEntity ToBaseEntity
	{
		get
		{
			return this;
		}
	}

	// Token: 0x06000A40 RID: 2624 RVA: 0x0005EA14 File Offset: 0x0005CC14
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void ServerTogglePlay(BaseEntity.RPCMessage msg)
	{
		bool play = msg.read.ReadByte() == 1;
		this.ServerTogglePlay(play);
	}

	// Token: 0x06000A41 RID: 2625 RVA: 0x0005EA37 File Offset: 0x0005CC37
	private void ServerTogglePlay(bool play)
	{
		base.SetFlag(BaseEntity.Flags.On, play, false, true);
	}

	// Token: 0x06000A42 RID: 2626 RVA: 0x0005EA43 File Offset: 0x0005CC43
	public void OnCassetteInserted(Cassette c)
	{
		base.ClientRPC<NetworkableId>(null, "Client_OnCassetteInserted", c.net.ID);
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000A43 RID: 2627 RVA: 0x0005EA63 File Offset: 0x0005CC63
	public void OnCassetteRemoved(Cassette c)
	{
		base.ClientRPC(null, "Client_OnCassetteRemoved");
		this.ServerTogglePlay(false);
	}

	// Token: 0x06000A44 RID: 2628 RVA: 0x0005EA78 File Offset: 0x0005CC78
	public override bool ItemFilter(Item item, int targetSlot)
	{
		ItemDefinition[] validCassettes = this.ValidCassettes;
		for (int i = 0; i < validCassettes.Length; i++)
		{
			if (validCassettes[i] == item.info)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000A45 RID: 2629 RVA: 0x0005EAAD File Offset: 0x0005CCAD
	public override void OnCollision(Collision collision, BaseEntity hitEntity)
	{
		if (base.isServer)
		{
			this.DoCollisionStick(collision, hitEntity);
		}
	}

	// Token: 0x06000A46 RID: 2630 RVA: 0x0005EAC0 File Offset: 0x0005CCC0
	private void DoCollisionStick(Collision collision, BaseEntity ent)
	{
		ContactPoint contact = collision.GetContact(0);
		this.DoStick(contact.point, contact.normal, ent, collision.collider);
	}

	// Token: 0x06000A47 RID: 2631 RVA: 0x0005EAF0 File Offset: 0x0005CCF0
	public virtual void SetMotionEnabled(bool wantsMotion)
	{
		Rigidbody component = base.GetComponent<Rigidbody>();
		if (component)
		{
			if (this.initialCollisionDetectionMode == null)
			{
				this.initialCollisionDetectionMode = new CollisionDetectionMode?(component.collisionDetectionMode);
			}
			component.useGravity = wantsMotion;
			if (!wantsMotion)
			{
				component.collisionDetectionMode = CollisionDetectionMode.Discrete;
			}
			component.isKinematic = !wantsMotion;
			if (wantsMotion)
			{
				component.collisionDetectionMode = this.initialCollisionDetectionMode.Value;
			}
		}
	}

	// Token: 0x06000A48 RID: 2632 RVA: 0x0005EB5C File Offset: 0x0005CD5C
	public void DoStick(Vector3 position, Vector3 normal, BaseEntity ent, Collider hitCollider)
	{
		if (ent != null && ent is TimedExplosive)
		{
			if (!ent.HasParent())
			{
				return;
			}
			position = ent.transform.position;
			ent = ent.parentEntity.Get(true);
		}
		this.SetMotionEnabled(false);
		this.SetCollisionEnabled(false);
		if (ent != null && base.HasChild(ent))
		{
			return;
		}
		base.transform.position = position;
		base.transform.rotation = Quaternion.LookRotation(normal, base.transform.up);
		if (hitCollider != null && ent != null)
		{
			base.SetParent(ent, ent.FindBoneID(hitCollider.transform), true, false);
		}
		else
		{
			base.SetParent(ent, StringPool.closest, true, false);
		}
		base.ReceiveCollisionMessages(false);
	}

	// Token: 0x06000A49 RID: 2633 RVA: 0x0005EC29 File Offset: 0x0005CE29
	private void UnStick()
	{
		if (!base.GetParentEntity())
		{
			return;
		}
		base.SetParent(null, true, true);
		this.SetMotionEnabled(true);
		this.SetCollisionEnabled(true);
		base.ReceiveCollisionMessages(true);
	}

	// Token: 0x06000A4A RID: 2634 RVA: 0x0005EC57 File Offset: 0x0005CE57
	internal override void OnParentRemoved()
	{
		this.UnStick();
	}

	// Token: 0x06000A4B RID: 2635 RVA: 0x0005EC60 File Offset: 0x0005CE60
	public virtual void SetCollisionEnabled(bool wantsCollision)
	{
		Collider component = base.GetComponent<Collider>();
		if (component && component.enabled != wantsCollision)
		{
			component.enabled = wantsCollision;
		}
	}

	// Token: 0x06000A4C RID: 2636 RVA: 0x0005EC8C File Offset: 0x0005CE8C
	public override void ResetState()
	{
		base.ResetState();
		if (base.isServer)
		{
			this.initialCollisionDetectionMode = null;
		}
	}
}
