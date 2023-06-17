using System;
using ConVar;
using Facepunch.Rust;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000D8 RID: 216
public class StashContainer : StorageContainer
{
	// Token: 0x04000BFA RID: 3066
	public Transform visuals;

	// Token: 0x04000BFB RID: 3067
	public float burriedOffset;

	// Token: 0x04000BFC RID: 3068
	public float raisedOffset;

	// Token: 0x04000BFD RID: 3069
	public GameObjectRef buryEffect;

	// Token: 0x04000BFE RID: 3070
	public float uncoverRange = 3f;

	// Token: 0x04000BFF RID: 3071
	private float lastToggleTime;

	// Token: 0x0600131A RID: 4890 RVA: 0x00099A8C File Offset: 0x00097C8C
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("StashContainer.OnRpcMessage", 0))
		{
			if (rpc == 4130263076U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_HideStash ");
				}
				using (TimeWarning.New("RPC_HideStash", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(4130263076U, "RPC_HideStash", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage rpc2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_HideStash(rpc2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_HideStash");
					}
				}
				return true;
			}
			if (rpc == 298671803U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_WantsUnhide ");
				}
				using (TimeWarning.New("RPC_WantsUnhide", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(298671803U, "RPC_WantsUnhide", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage rpc3 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_WantsUnhide(rpc3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in RPC_WantsUnhide");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600131B RID: 4891 RVA: 0x00003FA8 File Offset: 0x000021A8
	public bool IsHidden()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved5);
	}

	// Token: 0x0600131C RID: 4892 RVA: 0x00099D8C File Offset: 0x00097F8C
	public bool PlayerInRange(BasePlayer ply)
	{
		if (Vector3.Distance(base.transform.position, ply.transform.position) <= this.uncoverRange)
		{
			Vector3 normalized = (base.transform.position - ply.eyes.position).normalized;
			if (Vector3.Dot(ply.eyes.BodyForward(), normalized) > 0.95f)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600131D RID: 4893 RVA: 0x00099DFB File Offset: 0x00097FFB
	public override void InitShared()
	{
		base.InitShared();
		this.visuals.transform.localPosition = this.visuals.transform.localPosition.WithY(this.raisedOffset);
	}

	// Token: 0x0600131E RID: 4894 RVA: 0x00099E30 File Offset: 0x00098030
	public void DoOccludedCheck()
	{
		if (UnityEngine.Physics.SphereCast(new Ray(base.transform.position + Vector3.up * 5f, Vector3.down), 0.25f, 5f, 2097152))
		{
			base.DropItems(null);
			base.Kill(BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x0600131F RID: 4895 RVA: 0x00099E8A File Offset: 0x0009808A
	public void OnPhysicsNeighbourChanged()
	{
		if (!base.IsInvoking(new Action(this.DoOccludedCheck)))
		{
			base.Invoke(new Action(this.DoOccludedCheck), UnityEngine.Random.Range(5f, 10f));
		}
	}

	// Token: 0x06001320 RID: 4896 RVA: 0x00099EC4 File Offset: 0x000980C4
	public void SetHidden(bool isHidden)
	{
		if (UnityEngine.Time.realtimeSinceStartup - this.lastToggleTime < 3f)
		{
			return;
		}
		if (isHidden == base.HasFlag(BaseEntity.Flags.Reserved5))
		{
			return;
		}
		this.lastToggleTime = UnityEngine.Time.realtimeSinceStartup;
		base.Invoke(new Action(this.Decay), 259200f);
		if (base.isServer)
		{
			base.SetFlag(BaseEntity.Flags.Reserved5, isHidden, false, true);
		}
	}

	// Token: 0x06001321 RID: 4897 RVA: 0x00099F2C File Offset: 0x0009812C
	public void DisableNetworking()
	{
		base.limitNetworking = true;
		base.SetFlag(BaseEntity.Flags.Disabled, true, false, true);
	}

	// Token: 0x06001322 RID: 4898 RVA: 0x00003384 File Offset: 0x00001584
	public void Decay()
	{
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x06001323 RID: 4899 RVA: 0x00099F40 File Offset: 0x00098140
	public override void ServerInit()
	{
		base.ServerInit();
		this.SetHidden(false);
	}

	// Token: 0x06001324 RID: 4900 RVA: 0x00099F4F File Offset: 0x0009814F
	public void ToggleHidden()
	{
		this.SetHidden(!this.IsHidden());
	}

	// Token: 0x06001325 RID: 4901 RVA: 0x00099F60 File Offset: 0x00098160
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_HideStash(BaseEntity.RPCMessage rpc)
	{
		Analytics.Azure.OnStashHidden(rpc.player, this);
		this.SetHidden(true);
	}

	// Token: 0x06001326 RID: 4902 RVA: 0x00099F78 File Offset: 0x00098178
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_WantsUnhide(BaseEntity.RPCMessage rpc)
	{
		if (!this.IsHidden())
		{
			return;
		}
		BasePlayer player = rpc.player;
		if (this.PlayerInRange(player))
		{
			Analytics.Azure.OnStashRevealed(rpc.player, this);
			this.SetHidden(false);
		}
	}

	// Token: 0x06001327 RID: 4903 RVA: 0x00099FB4 File Offset: 0x000981B4
	public override void OnFlagsChanged(BaseEntity.Flags old, BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		bool flag = (old & BaseEntity.Flags.Reserved5) == BaseEntity.Flags.Reserved5;
		bool flag2 = (next & BaseEntity.Flags.Reserved5) == BaseEntity.Flags.Reserved5;
		if (flag != flag2)
		{
			float to = flag2 ? this.burriedOffset : this.raisedOffset;
			LeanTween.cancel(this.visuals.gameObject);
			LeanTween.moveLocalY(this.visuals.gameObject, to, 1f);
		}
	}

	// Token: 0x02000C09 RID: 3081
	public static class StashContainerFlags
	{
		// Token: 0x040041B5 RID: 16821
		public const BaseEntity.Flags Hidden = BaseEntity.Flags.Reserved5;
	}
}
