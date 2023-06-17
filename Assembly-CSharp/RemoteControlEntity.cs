using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000BC RID: 188
public class RemoteControlEntity : BaseCombatEntity, IRemoteControllable
{
	// Token: 0x04000AB6 RID: 2742
	public static List<IRemoteControllable> allControllables = new List<IRemoteControllable>();

	// Token: 0x04000AB7 RID: 2743
	[Header("RC Entity")]
	public string rcIdentifier = "";

	// Token: 0x04000AB8 RID: 2744
	public Transform viewEyes;

	// Token: 0x04000AB9 RID: 2745
	public GameObjectRef IDPanelPrefab;

	// Token: 0x04000ABA RID: 2746
	public RemoteControllableControls rcControls;

	// Token: 0x060010F4 RID: 4340 RVA: 0x0008C0A4 File Offset: 0x0008A2A4
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("RemoteControlEntity.OnRpcMessage", 0))
		{
			if (rpc == 1053317251U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
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

	// Token: 0x17000191 RID: 401
	// (get) Token: 0x060010F5 RID: 4341 RVA: 0x0000441C File Offset: 0x0000261C
	public bool CanPing
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060010F6 RID: 4342 RVA: 0x0008C20C File Offset: 0x0008A40C
	public Transform GetEyes()
	{
		return this.viewEyes;
	}

	// Token: 0x060010F7 RID: 4343 RVA: 0x00006CA5 File Offset: 0x00004EA5
	public float GetFovScale()
	{
		return 1f;
	}

	// Token: 0x060010F8 RID: 4344 RVA: 0x000037E7 File Offset: 0x000019E7
	public global::BaseEntity GetEnt()
	{
		return this;
	}

	// Token: 0x060010F9 RID: 4345 RVA: 0x0008C214 File Offset: 0x0008A414
	public string GetIdentifier()
	{
		return this.rcIdentifier;
	}

	// Token: 0x17000192 RID: 402
	// (get) Token: 0x060010FA RID: 4346 RVA: 0x00007A3C File Offset: 0x00005C3C
	public virtual bool CanAcceptInput
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000193 RID: 403
	// (get) Token: 0x060010FB RID: 4347 RVA: 0x0008C21C File Offset: 0x0008A41C
	// (set) Token: 0x060010FC RID: 4348 RVA: 0x0008C224 File Offset: 0x0008A424
	public int ViewerCount { get; private set; }

	// Token: 0x17000194 RID: 404
	// (get) Token: 0x060010FD RID: 4349 RVA: 0x0008C22D File Offset: 0x0008A42D
	// (set) Token: 0x060010FE RID: 4350 RVA: 0x0008C235 File Offset: 0x0008A435
	public CameraViewerId? ControllingViewerId { get; private set; }

	// Token: 0x17000195 RID: 405
	// (get) Token: 0x060010FF RID: 4351 RVA: 0x0008C240 File Offset: 0x0008A440
	public bool IsBeingControlled
	{
		get
		{
			return this.ViewerCount > 0 && this.ControllingViewerId != null;
		}
	}

	// Token: 0x06001100 RID: 4352 RVA: 0x0008C268 File Offset: 0x0008A468
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

	// Token: 0x06001101 RID: 4353 RVA: 0x0008C2B4 File Offset: 0x0008A4B4
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

	// Token: 0x06001102 RID: 4354 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void UserInput(InputState inputState, CameraViewerId viewerID)
	{
	}

	// Token: 0x06001103 RID: 4355 RVA: 0x0008C310 File Offset: 0x0008A510
	public void UpdateIdentifier(string newID, bool clientSend = false)
	{
		string text = this.rcIdentifier;
		if (base.isServer)
		{
			if (!RemoteControlEntity.IDInUse(newID))
			{
				this.rcIdentifier = newID;
			}
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06001104 RID: 4356 RVA: 0x0002356B File Offset: 0x0002176B
	public virtual void RCSetup()
	{
		if (base.isServer)
		{
			RemoteControlEntity.InstallControllable(this);
		}
	}

	// Token: 0x06001105 RID: 4357 RVA: 0x0002357C File Offset: 0x0002177C
	public virtual void RCShutdown()
	{
		if (base.isServer)
		{
			RemoteControlEntity.RemoveControllable(this);
		}
	}

	// Token: 0x06001106 RID: 4358 RVA: 0x0008C337 File Offset: 0x0008A537
	public override void InitShared()
	{
		base.InitShared();
		this.RCSetup();
	}

	// Token: 0x06001107 RID: 4359 RVA: 0x0008C345 File Offset: 0x0008A545
	public override void DestroyShared()
	{
		this.RCShutdown();
		base.DestroyShared();
	}

	// Token: 0x06001108 RID: 4360 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool CanControl(ulong playerID)
	{
		return true;
	}

	// Token: 0x17000196 RID: 406
	// (get) Token: 0x06001109 RID: 4361 RVA: 0x00007A3C File Offset: 0x00005C3C
	public virtual bool RequiresMouse
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000197 RID: 407
	// (get) Token: 0x0600110A RID: 4362 RVA: 0x000231E8 File Offset: 0x000213E8
	public virtual float MaxRange
	{
		get
		{
			return 10000f;
		}
	}

	// Token: 0x17000198 RID: 408
	// (get) Token: 0x0600110B RID: 4363 RVA: 0x0008C353 File Offset: 0x0008A553
	public RemoteControllableControls RequiredControls
	{
		get
		{
			return this.rcControls;
		}
	}

	// Token: 0x0600110C RID: 4364 RVA: 0x0008C35C File Offset: 0x0008A55C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void Server_SetID(global::BaseEntity.RPCMessage msg)
	{
		if (msg.player == null || !this.CanControl(msg.player.userID) || !this.CanChangeID(msg.player))
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
			Debug.Log("SetID success!");
			this.UpdateIdentifier(text2, false);
		}
	}

	// Token: 0x0600110D RID: 4365 RVA: 0x00007A3C File Offset: 0x00005C3C
	public override bool CanUseNetworkCache(Connection connection)
	{
		return false;
	}

	// Token: 0x0600110E RID: 4366 RVA: 0x0008C3F8 File Offset: 0x0008A5F8
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (!info.forDisk)
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

	// Token: 0x0600110F RID: 4367 RVA: 0x0008C45C File Offset: 0x0008A65C
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.rcEntity != null && global::ComputerStation.IsValidIdentifier(info.msg.rcEntity.identifier))
		{
			this.UpdateIdentifier(info.msg.rcEntity.identifier, false);
		}
	}

	// Token: 0x06001110 RID: 4368 RVA: 0x0008C4AB File Offset: 0x0008A6AB
	protected virtual bool CanChangeID(global::BasePlayer player)
	{
		return player != null && player.CanBuild() && player.IsBuildingAuthed() && player.IsHoldingEntity<Hammer>();
	}

	// Token: 0x06001111 RID: 4369 RVA: 0x0008C4CE File Offset: 0x0008A6CE
	public static bool IDInUse(string id)
	{
		return RemoteControlEntity.FindByID(id) != null;
	}

	// Token: 0x06001112 RID: 4370 RVA: 0x0008C4DC File Offset: 0x0008A6DC
	public static IRemoteControllable FindByID(string id)
	{
		foreach (IRemoteControllable remoteControllable in RemoteControlEntity.allControllables)
		{
			if (remoteControllable != null && remoteControllable.GetIdentifier() == id)
			{
				return remoteControllable;
			}
		}
		return null;
	}

	// Token: 0x06001113 RID: 4371 RVA: 0x0008C540 File Offset: 0x0008A740
	public static bool InstallControllable(IRemoteControllable newControllable)
	{
		if (RemoteControlEntity.allControllables.Contains(newControllable))
		{
			return false;
		}
		RemoteControlEntity.allControllables.Add(newControllable);
		return true;
	}

	// Token: 0x06001114 RID: 4372 RVA: 0x0008C55D File Offset: 0x0008A75D
	public static bool RemoveControllable(IRemoteControllable newControllable)
	{
		if (!RemoteControlEntity.allControllables.Contains(newControllable))
		{
			return false;
		}
		RemoteControlEntity.allControllables.Remove(newControllable);
		return true;
	}
}
