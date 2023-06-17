using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000B9 RID: 185
public class RecorderTool : ThrownWeapon, ICassettePlayer
{
	// Token: 0x04000A95 RID: 2709
	[ClientVar(Saved = true)]
	public static bool debugRecording;

	// Token: 0x04000A96 RID: 2710
	public AudioSource RecorderAudioSource;

	// Token: 0x04000A97 RID: 2711
	public SoundDefinition RecordStartSfx;

	// Token: 0x04000A98 RID: 2712
	public SoundDefinition RewindSfx;

	// Token: 0x04000A99 RID: 2713
	public SoundDefinition RecordFinishedSfx;

	// Token: 0x04000A9A RID: 2714
	public SoundDefinition PlayTapeSfx;

	// Token: 0x04000A9B RID: 2715
	public SoundDefinition StopTapeSfx;

	// Token: 0x04000A9C RID: 2716
	public float ThrowScale = 3f;

	// Token: 0x0600109F RID: 4255 RVA: 0x000894D0 File Offset: 0x000876D0
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("RecorderTool.OnRpcMessage", 0))
		{
			if (rpc == 3075830603U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_TogglePlaying ");
				}
				using (TimeWarning.New("Server_TogglePlaying", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.FromOwner.Test(3075830603U, "Server_TogglePlaying", this, player))
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
							this.Server_TogglePlaying(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in Server_TogglePlaying");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x1700018C RID: 396
	// (get) Token: 0x060010A0 RID: 4256 RVA: 0x00089634 File Offset: 0x00087834
	// (set) Token: 0x060010A1 RID: 4257 RVA: 0x0008963C File Offset: 0x0008783C
	public Cassette cachedCassette { get; private set; }

	// Token: 0x1700018D RID: 397
	// (get) Token: 0x060010A2 RID: 4258 RVA: 0x00089645 File Offset: 0x00087845
	public Sprite LoadedCassetteIcon
	{
		get
		{
			if (!(this.cachedCassette != null))
			{
				return null;
			}
			return this.cachedCassette.HudSprite;
		}
	}

	// Token: 0x060010A3 RID: 4259 RVA: 0x00089662 File Offset: 0x00087862
	private bool HasCassette()
	{
		return this.cachedCassette != null;
	}

	// Token: 0x1700018E RID: 398
	// (get) Token: 0x060010A4 RID: 4260 RVA: 0x000037E7 File Offset: 0x000019E7
	public BaseEntity ToBaseEntity
	{
		get
		{
			return this;
		}
	}

	// Token: 0x060010A5 RID: 4261 RVA: 0x00089670 File Offset: 0x00087870
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.FromOwner]
	public void Server_TogglePlaying(BaseEntity.RPCMessage msg)
	{
		bool b = msg.read.ReadByte() == 1;
		base.SetFlag(BaseEntity.Flags.On, b, false, true);
	}

	// Token: 0x060010A6 RID: 4262 RVA: 0x00089696 File Offset: 0x00087896
	public void OnCassetteInserted(Cassette c)
	{
		this.cachedCassette = c;
		base.ClientRPC<NetworkableId>(null, "Client_OnCassetteInserted", c.net.ID);
	}

	// Token: 0x060010A7 RID: 4263 RVA: 0x000896B6 File Offset: 0x000878B6
	public void OnCassetteRemoved(Cassette c)
	{
		this.cachedCassette = null;
		base.ClientRPC(null, "Client_OnCassetteRemoved");
	}

	// Token: 0x060010A8 RID: 4264 RVA: 0x000896CC File Offset: 0x000878CC
	protected override void SetUpThrownWeapon(BaseEntity ent)
	{
		base.SetUpThrownWeapon(ent);
		if (base.GetOwnerPlayer() != null)
		{
			ent.OwnerID = base.GetOwnerPlayer().userID;
		}
		DeployedRecorder deployedRecorder;
		if (this.cachedCassette != null && (deployedRecorder = (ent as DeployedRecorder)) != null)
		{
			this.GetItem().contents.itemList[0].SetParent(deployedRecorder.inventory);
		}
	}

	// Token: 0x060010A9 RID: 4265 RVA: 0x00089738 File Offset: 0x00087938
	public override void OnHeldChanged()
	{
		base.OnHeldChanged();
		if (base.IsDisabled())
		{
			base.SetFlag(BaseEntity.Flags.On, false, false, true);
		}
	}
}
