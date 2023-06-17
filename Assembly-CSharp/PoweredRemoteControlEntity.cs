using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000B4 RID: 180
public class PoweredRemoteControlEntity : global::IOEntity, IRemoteControllable
{
	// Token: 0x04000A74 RID: 2676
	public string rcIdentifier = "";

	// Token: 0x04000A75 RID: 2677
	public Transform viewEyes;

	// Token: 0x04000A76 RID: 2678
	public GameObjectRef IDPanelPrefab;

	// Token: 0x04000A77 RID: 2679
	public RemoteControllableControls rcControls;

	// Token: 0x04000A78 RID: 2680
	public bool isStatic;

	// Token: 0x04000A79 RID: 2681
	public bool appendEntityIDToIdentifier;

	// Token: 0x06001049 RID: 4169 RVA: 0x00087EF8 File Offset: 0x000860F8
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("PoweredRemoteControlEntity.OnRpcMessage", 0))
		{
			if (rpc == 1053317251U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_SetID ");
				}
				using (TimeWarning.New("Server_SetID", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(1053317251U, "Server_SetID", this, player, 3f))
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
							this.Server_SetID(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in Server_SetID");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600104A RID: 4170 RVA: 0x00088060 File Offset: 0x00086260
	public bool IsStatic()
	{
		return this.isStatic;
	}

	// Token: 0x17000183 RID: 387
	// (get) Token: 0x0600104B RID: 4171 RVA: 0x00007A3C File Offset: 0x00005C3C
	public virtual bool RequiresMouse
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000184 RID: 388
	// (get) Token: 0x0600104C RID: 4172 RVA: 0x000231E8 File Offset: 0x000213E8
	public virtual float MaxRange
	{
		get
		{
			return 10000f;
		}
	}

	// Token: 0x17000185 RID: 389
	// (get) Token: 0x0600104D RID: 4173 RVA: 0x00088068 File Offset: 0x00086268
	public RemoteControllableControls RequiredControls
	{
		get
		{
			return this.rcControls;
		}
	}

	// Token: 0x17000186 RID: 390
	// (get) Token: 0x0600104E RID: 4174 RVA: 0x00088070 File Offset: 0x00086270
	public bool CanPing
	{
		get
		{
			return this.EntityCanPing;
		}
	}

	// Token: 0x17000187 RID: 391
	// (get) Token: 0x0600104F RID: 4175 RVA: 0x00007A3C File Offset: 0x00005C3C
	protected virtual bool EntityCanPing
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000188 RID: 392
	// (get) Token: 0x06001050 RID: 4176 RVA: 0x00007A3C File Offset: 0x00005C3C
	public virtual bool CanAcceptInput
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000189 RID: 393
	// (get) Token: 0x06001051 RID: 4177 RVA: 0x00088078 File Offset: 0x00086278
	// (set) Token: 0x06001052 RID: 4178 RVA: 0x00088080 File Offset: 0x00086280
	public int ViewerCount { get; private set; }

	// Token: 0x1700018A RID: 394
	// (get) Token: 0x06001053 RID: 4179 RVA: 0x00088089 File Offset: 0x00086289
	// (set) Token: 0x06001054 RID: 4180 RVA: 0x00088091 File Offset: 0x00086291
	public CameraViewerId? ControllingViewerId { get; private set; }

	// Token: 0x1700018B RID: 395
	// (get) Token: 0x06001055 RID: 4181 RVA: 0x0008809C File Offset: 0x0008629C
	public bool IsBeingControlled
	{
		get
		{
			return this.ViewerCount > 0 && this.ControllingViewerId != null;
		}
	}

	// Token: 0x06001056 RID: 4182 RVA: 0x000880C2 File Offset: 0x000862C2
	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		base.UpdateHasPower(inputAmount, inputSlot);
		this.UpdateRCAccess(this.IsPowered());
	}

	// Token: 0x06001057 RID: 4183 RVA: 0x000880D8 File Offset: 0x000862D8
	public void UpdateRCAccess(bool isOnline)
	{
		if (isOnline)
		{
			RemoteControlEntity.InstallControllable(this);
			return;
		}
		RemoteControlEntity.RemoveControllable(this);
	}

	// Token: 0x06001058 RID: 4184 RVA: 0x000880EC File Offset: 0x000862EC
	public override void Spawn()
	{
		base.Spawn();
		string text = "#ID";
		if (this.IsStatic() && this.rcIdentifier.Contains(text))
		{
			int length = this.rcIdentifier.IndexOf(text);
			int length2 = text.Length;
			string text2 = this.rcIdentifier.Substring(0, length);
			text2 += this.net.ID.ToString();
			this.UpdateIdentifier(text2, false);
		}
	}

	// Token: 0x06001059 RID: 4185 RVA: 0x00088164 File Offset: 0x00086364
	public virtual bool InitializeControl(CameraViewerId viewerID)
	{
		int viewerCount = this.ViewerCount;
		this.ViewerCount = viewerCount + 1;
		if (this.CanAcceptInput && this.ControllingViewerId == null)
		{
			this.ControllingViewerId = new CameraViewerId?(viewerID);
			return true;
		}
		return !this.CanAcceptInput;
	}

	// Token: 0x0600105A RID: 4186 RVA: 0x000881B0 File Offset: 0x000863B0
	public virtual void StopControl(CameraViewerId viewerID)
	{
		int viewerCount = this.ViewerCount;
		this.ViewerCount = viewerCount - 1;
		CameraViewerId? controllingViewerId = this.ControllingViewerId;
		if (controllingViewerId != null && (controllingViewerId == null || controllingViewerId.GetValueOrDefault() == viewerID))
		{
			this.ControllingViewerId = null;
		}
	}

	// Token: 0x0600105B RID: 4187 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void UserInput(InputState inputState, CameraViewerId viewerID)
	{
	}

	// Token: 0x0600105C RID: 4188 RVA: 0x0008820C File Offset: 0x0008640C
	public Transform GetEyes()
	{
		return this.viewEyes;
	}

	// Token: 0x0600105D RID: 4189 RVA: 0x00006CA5 File Offset: 0x00004EA5
	public virtual float GetFovScale()
	{
		return 1f;
	}

	// Token: 0x0600105E RID: 4190 RVA: 0x00088214 File Offset: 0x00086414
	public virtual bool CanControl(ulong playerID)
	{
		return this.IsPowered() || this.IsStatic();
	}

	// Token: 0x0600105F RID: 4191 RVA: 0x000037E7 File Offset: 0x000019E7
	public global::BaseEntity GetEnt()
	{
		return this;
	}

	// Token: 0x06001060 RID: 4192 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void RCSetup()
	{
	}

	// Token: 0x06001061 RID: 4193 RVA: 0x0002357C File Offset: 0x0002177C
	public virtual void RCShutdown()
	{
		if (base.isServer)
		{
			RemoteControlEntity.RemoveControllable(this);
		}
	}

	// Token: 0x06001062 RID: 4194 RVA: 0x00088228 File Offset: 0x00086428
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void Server_SetID(global::BaseEntity.RPCMessage msg)
	{
		if (this.IsStatic())
		{
			return;
		}
		global::BasePlayer player = msg.player;
		if (!this.CanChangeID(player))
		{
			return;
		}
		string text = msg.read.String(256);
		if (!string.IsNullOrEmpty(text) && !global::ComputerStation.IsValidIdentifier(text))
		{
			return;
		}
		string text2 = msg.read.String(256);
		if (!global::ComputerStation.IsValidIdentifier(text2))
		{
			return;
		}
		if (text == this.GetIdentifier())
		{
			this.UpdateIdentifier(text2, false);
		}
	}

	// Token: 0x06001063 RID: 4195 RVA: 0x000882A1 File Offset: 0x000864A1
	public override bool CanUseNetworkCache(Connection connection)
	{
		return this.IsStatic() && base.CanUseNetworkCache(connection);
	}

	// Token: 0x06001064 RID: 4196 RVA: 0x000882B4 File Offset: 0x000864B4
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (!info.forDisk && !this.IsStatic())
		{
			Connection forConnection = info.forConnection;
			if (!this.CanChangeID(((forConnection != null) ? forConnection.player : null) as global::BasePlayer))
			{
				return;
			}
		}
		info.msg.rcEntity = Facepunch.Pool.Get<RCEntity>();
		info.msg.rcEntity.identifier = this.GetIdentifier();
	}

	// Token: 0x06001065 RID: 4197 RVA: 0x00088320 File Offset: 0x00086520
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.rcEntity != null && global::ComputerStation.IsValidIdentifier(info.msg.rcEntity.identifier))
		{
			this.UpdateIdentifier(info.msg.rcEntity.identifier, false);
		}
	}

	// Token: 0x06001066 RID: 4198 RVA: 0x0008836F File Offset: 0x0008656F
	public void UpdateIdentifier(string newID, bool clientSend = false)
	{
		string text = this.rcIdentifier;
		if (base.isServer)
		{
			if (!RemoteControlEntity.IDInUse(newID))
			{
				this.rcIdentifier = newID;
			}
			if (!Rust.Application.isLoadingSave)
			{
				base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			}
		}
	}

	// Token: 0x06001067 RID: 4199 RVA: 0x0008839D File Offset: 0x0008659D
	public string GetIdentifier()
	{
		return this.rcIdentifier;
	}

	// Token: 0x06001068 RID: 4200 RVA: 0x000883A5 File Offset: 0x000865A5
	public override void InitShared()
	{
		base.InitShared();
		this.RCSetup();
	}

	// Token: 0x06001069 RID: 4201 RVA: 0x000883B3 File Offset: 0x000865B3
	public override void DestroyShared()
	{
		this.RCShutdown();
		base.DestroyShared();
	}

	// Token: 0x0600106A RID: 4202 RVA: 0x000883C1 File Offset: 0x000865C1
	protected bool CanChangeID(global::BasePlayer player)
	{
		return player != null && player.CanBuild() && player.IsBuildingAuthed();
	}
}
