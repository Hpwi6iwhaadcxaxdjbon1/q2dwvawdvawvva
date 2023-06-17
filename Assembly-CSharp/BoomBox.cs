using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Facepunch.Models;
using Newtonsoft.Json.Linq;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000397 RID: 919
public class BoomBox : EntityComponent<global::BaseEntity>, INotifyLOD
{
	// Token: 0x04001967 RID: 6503
	public static Dictionary<string, string> ValidStations;

	// Token: 0x04001968 RID: 6504
	public static Dictionary<string, string> ServerValidStations;

	// Token: 0x04001969 RID: 6505
	[ReplicatedVar(Saved = true, Help = "A list of radio stations that are valid on this server. Format: NAME,URL,NAME,URL,etc", ShowInAdminUI = true)]
	public static string ServerUrlList = string.Empty;

	// Token: 0x0400196A RID: 6506
	private static string lastParsedServerList;

	// Token: 0x0400196B RID: 6507
	public ShoutcastStreamer ShoutcastStreamer;

	// Token: 0x0400196C RID: 6508
	public GameObjectRef RadioIpDialog;

	// Token: 0x0400196E RID: 6510
	public ulong AssignedRadioBy;

	// Token: 0x0400196F RID: 6511
	public AudioSource SoundSource;

	// Token: 0x04001970 RID: 6512
	public float ConditionLossRate = 0.25f;

	// Token: 0x04001971 RID: 6513
	public ItemDefinition[] ValidCassettes;

	// Token: 0x04001972 RID: 6514
	public SoundDefinition PlaySfx;

	// Token: 0x04001973 RID: 6515
	public SoundDefinition StopSfx;

	// Token: 0x04001974 RID: 6516
	public const global::BaseEntity.Flags HasCassette = global::BaseEntity.Flags.Reserved1;

	// Token: 0x04001975 RID: 6517
	[ServerVar(Saved = true)]
	public static int BacktrackLength = 30;

	// Token: 0x04001976 RID: 6518
	public Action<float> HurtCallback;

	// Token: 0x0600205B RID: 8283 RVA: 0x000D68F0 File Offset: 0x000D4AF0
	[ServerVar]
	public static void ClearRadioByUser(ConsoleSystem.Arg arg)
	{
		ulong @uint = arg.GetUInt64(0, 0UL);
		int num = 0;
		foreach (global::BaseNetworkable baseNetworkable in global::BaseNetworkable.serverEntities)
		{
			DeployableBoomBox deployableBoomBox;
			HeldBoomBox heldBoomBox;
			if ((deployableBoomBox = (baseNetworkable as DeployableBoomBox)) != null)
			{
				if (deployableBoomBox.ClearRadioByUserId(@uint))
				{
					num++;
				}
			}
			else if ((heldBoomBox = (baseNetworkable as HeldBoomBox)) != null && heldBoomBox.ClearRadioByUserId(@uint))
			{
				num++;
			}
		}
		arg.ReplyWith(string.Format("Stopped and cleared saved URL of {0} boom boxes", num));
	}

	// Token: 0x0600205C RID: 8284 RVA: 0x000D698C File Offset: 0x000D4B8C
	public static void LoadStations()
	{
		if (global::BoomBox.ValidStations != null)
		{
			return;
		}
		global::BoomBox.ValidStations = (global::BoomBox.GetStationData() ?? new Dictionary<string, string>());
		global::BoomBox.ParseServerUrlList();
	}

	// Token: 0x0600205D RID: 8285 RVA: 0x000D69B0 File Offset: 0x000D4BB0
	private static Dictionary<string, string> GetStationData()
	{
		Facepunch.Models.Manifest manifest = Facepunch.Application.Manifest;
		JObject jobject = (manifest != null) ? manifest.Metadata : null;
		JArray jarray;
		if ((jarray = (((jobject != null) ? jobject["RadioStations"] : null) as JArray)) != null && jarray.Count > 0)
		{
			string[] array = new string[2];
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (string text in jarray.Values<string>())
			{
				array = text.Split(new char[]
				{
					','
				});
				if (!dictionary.ContainsKey(array[0]))
				{
					dictionary.Add(array[0], array[1]);
				}
			}
			return dictionary;
		}
		return null;
	}

	// Token: 0x0600205E RID: 8286 RVA: 0x000D6A64 File Offset: 0x000D4C64
	private static bool IsStationValid(string url)
	{
		global::BoomBox.ParseServerUrlList();
		return (global::BoomBox.ValidStations != null && global::BoomBox.ValidStations.ContainsValue(url)) || (global::BoomBox.ServerValidStations != null && global::BoomBox.ServerValidStations.ContainsValue(url));
	}

	// Token: 0x0600205F RID: 8287 RVA: 0x000D6A98 File Offset: 0x000D4C98
	public static void ParseServerUrlList()
	{
		if (global::BoomBox.ServerValidStations == null)
		{
			global::BoomBox.ServerValidStations = new Dictionary<string, string>();
		}
		if (global::BoomBox.lastParsedServerList == global::BoomBox.ServerUrlList)
		{
			return;
		}
		global::BoomBox.ServerValidStations.Clear();
		if (!string.IsNullOrEmpty(global::BoomBox.ServerUrlList))
		{
			string[] array = global::BoomBox.ServerUrlList.Split(new char[]
			{
				','
			});
			if (array.Length % 2 != 0)
			{
				Debug.Log("Invalid number of stations in BoomBox.ServerUrlList, ensure you always have a name and a url");
				return;
			}
			for (int i = 0; i < array.Length; i += 2)
			{
				if (global::BoomBox.ServerValidStations.ContainsKey(array[i]))
				{
					Debug.Log("Duplicate station name detected in BoomBox.ServerUrlList, all station names must be unique: " + array[i]);
				}
				else
				{
					global::BoomBox.ServerValidStations.Add(array[i], array[i + 1]);
				}
			}
		}
		global::BoomBox.lastParsedServerList = global::BoomBox.ServerUrlList;
	}

	// Token: 0x170002AD RID: 685
	// (get) Token: 0x06002060 RID: 8288 RVA: 0x000D6B53 File Offset: 0x000D4D53
	// (set) Token: 0x06002061 RID: 8289 RVA: 0x000D6B5B File Offset: 0x000D4D5B
	public string CurrentRadioIp { get; private set; } = "rustradio.facepunch.com";

	// Token: 0x06002062 RID: 8290 RVA: 0x000D6B64 File Offset: 0x000D4D64
	public void Server_UpdateRadioIP(global::BaseEntity.RPCMessage msg)
	{
		string text = msg.read.String(256);
		if (global::BoomBox.IsStationValid(text))
		{
			if (msg.player != null)
			{
				ulong userID = msg.player.userID;
				this.AssignedRadioBy = userID;
			}
			this.CurrentRadioIp = text;
			base.baseEntity.ClientRPC<string>(null, "OnRadioIPChanged", this.CurrentRadioIp);
			if (this.IsOn())
			{
				this.ServerTogglePlay(false);
			}
		}
	}

	// Token: 0x06002063 RID: 8291 RVA: 0x000D6BD8 File Offset: 0x000D4DD8
	public void Save(global::BaseNetworkable.SaveInfo info)
	{
		if (info.msg.boomBox == null)
		{
			info.msg.boomBox = Facepunch.Pool.Get<ProtoBuf.BoomBox>();
		}
		info.msg.boomBox.radioIp = this.CurrentRadioIp;
		info.msg.boomBox.assignedRadioBy = this.AssignedRadioBy;
	}

	// Token: 0x06002064 RID: 8292 RVA: 0x000D6C2E File Offset: 0x000D4E2E
	public bool ClearRadioByUserId(ulong id)
	{
		if (this.AssignedRadioBy == id)
		{
			this.CurrentRadioIp = string.Empty;
			this.AssignedRadioBy = 0UL;
			if (this.HasFlag(global::BaseEntity.Flags.On))
			{
				this.ServerTogglePlay(false);
			}
			return true;
		}
		return false;
	}

	// Token: 0x06002065 RID: 8293 RVA: 0x000D6C5F File Offset: 0x000D4E5F
	public void Load(global::BaseNetworkable.LoadInfo info)
	{
		if (info.msg.boomBox != null)
		{
			this.CurrentRadioIp = info.msg.boomBox.radioIp;
			this.AssignedRadioBy = info.msg.boomBox.assignedRadioBy;
		}
	}

	// Token: 0x170002AE RID: 686
	// (get) Token: 0x06002066 RID: 8294 RVA: 0x000D6C9A File Offset: 0x000D4E9A
	public global::BaseEntity BaseEntity
	{
		get
		{
			return base.baseEntity;
		}
	}

	// Token: 0x06002067 RID: 8295 RVA: 0x000D6CA4 File Offset: 0x000D4EA4
	public void ServerTogglePlay(global::BaseEntity.RPCMessage msg)
	{
		if (!this.IsPowered())
		{
			return;
		}
		bool play = msg.read.ReadByte() == 1;
		this.ServerTogglePlay(play);
	}

	// Token: 0x06002068 RID: 8296 RVA: 0x000D6CD0 File Offset: 0x000D4ED0
	private void DeductCondition()
	{
		Action<float> hurtCallback = this.HurtCallback;
		if (hurtCallback == null)
		{
			return;
		}
		hurtCallback(this.ConditionLossRate * ConVar.Decay.scale);
	}

	// Token: 0x06002069 RID: 8297 RVA: 0x000D6CF0 File Offset: 0x000D4EF0
	public void ServerTogglePlay(bool play)
	{
		if (base.baseEntity == null)
		{
			return;
		}
		this.SetFlag(global::BaseEntity.Flags.On, play);
		global::IOEntity ioentity;
		if ((ioentity = (base.baseEntity as global::IOEntity)) != null)
		{
			ioentity.SendChangedToRoot(true);
			ioentity.MarkDirtyForceUpdateOutputs();
		}
		if (play && !base.IsInvoking(new Action(this.DeductCondition)) && this.ConditionLossRate > 0f)
		{
			base.InvokeRepeating(new Action(this.DeductCondition), 1f, 1f);
			return;
		}
		if (base.IsInvoking(new Action(this.DeductCondition)))
		{
			base.CancelInvoke(new Action(this.DeductCondition));
		}
	}

	// Token: 0x0600206A RID: 8298 RVA: 0x000D6D98 File Offset: 0x000D4F98
	public void OnCassetteInserted(global::Cassette c)
	{
		if (base.baseEntity == null)
		{
			return;
		}
		base.baseEntity.ClientRPC<NetworkableId>(null, "Client_OnCassetteInserted", c.net.ID);
		this.ServerTogglePlay(false);
		this.SetFlag(global::BaseEntity.Flags.Reserved1, true);
		base.baseEntity.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x0600206B RID: 8299 RVA: 0x000D6DEF File Offset: 0x000D4FEF
	public void OnCassetteRemoved(global::Cassette c)
	{
		if (base.baseEntity == null)
		{
			return;
		}
		base.baseEntity.ClientRPC(null, "Client_OnCassetteRemoved");
		this.ServerTogglePlay(false);
		this.SetFlag(global::BaseEntity.Flags.Reserved1, false);
	}

	// Token: 0x0600206C RID: 8300 RVA: 0x000D6E24 File Offset: 0x000D5024
	private bool IsPowered()
	{
		return !(base.baseEntity == null) && (base.baseEntity.HasFlag(global::BaseEntity.Flags.Reserved8) || base.baseEntity is HeldBoomBox);
	}

	// Token: 0x0600206D RID: 8301 RVA: 0x000D6E58 File Offset: 0x000D5058
	private bool IsOn()
	{
		return !(base.baseEntity == null) && base.baseEntity.IsOn();
	}

	// Token: 0x0600206E RID: 8302 RVA: 0x000D6E75 File Offset: 0x000D5075
	private bool HasFlag(global::BaseEntity.Flags f)
	{
		return !(base.baseEntity == null) && base.baseEntity.HasFlag(f);
	}

	// Token: 0x0600206F RID: 8303 RVA: 0x000D6E93 File Offset: 0x000D5093
	private void SetFlag(global::BaseEntity.Flags f, bool state)
	{
		if (base.baseEntity != null)
		{
			base.baseEntity.SetFlag(f, state, false, true);
		}
	}

	// Token: 0x170002AF RID: 687
	// (get) Token: 0x06002070 RID: 8304 RVA: 0x000D6EB2 File Offset: 0x000D50B2
	private bool isClient
	{
		get
		{
			return base.baseEntity != null && base.baseEntity.isClient;
		}
	}
}
