using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000056 RID: 86
public class Cassette : global::BaseEntity, IUGCBrowserEntity
{
	// Token: 0x0400062B RID: 1579
	public float MaxCassetteLength = 15f;

	// Token: 0x0400062C RID: 1580
	[ReplicatedVar]
	public static float MaxCassetteFileSizeMB = 5f;

	// Token: 0x0400062E RID: 1582
	public ulong CreatorSteamId;

	// Token: 0x0400062F RID: 1583
	public PreloadedCassetteContent.PreloadType PreloadType;

	// Token: 0x04000630 RID: 1584
	public PreloadedCassetteContent PreloadContent;

	// Token: 0x04000631 RID: 1585
	public SoundDefinition InsertCassetteSfx;

	// Token: 0x04000632 RID: 1586
	public int ViewmodelIndex;

	// Token: 0x04000633 RID: 1587
	public Sprite HudSprite;

	// Token: 0x04000634 RID: 1588
	public int MaximumVoicemailSlots = 1;

	// Token: 0x04000635 RID: 1589
	private int preloadedAudioId;

	// Token: 0x04000636 RID: 1590
	private ICassettePlayer currentCassettePlayer;

	// Token: 0x0600095B RID: 2395 RVA: 0x00058E44 File Offset: 0x00057044
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Cassette.OnRpcMessage", 0))
		{
			if (rpc == 4031457637U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_MakeNewFile ");
				}
				using (TimeWarning.New("Server_MakeNewFile", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(4031457637U, "Server_MakeNewFile", this, player, 1UL))
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
							this.Server_MakeNewFile(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in Server_MakeNewFile");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600095C RID: 2396 RVA: 0x00058FAC File Offset: 0x000571AC
	[ServerVar]
	public static void ClearCassettes(ConsoleSystem.Arg arg)
	{
		int num = 0;
		using (IEnumerator<global::BaseNetworkable> enumerator = global::BaseNetworkable.serverEntities.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				global::Cassette cassette;
				if ((cassette = (enumerator.Current as global::Cassette)) != null && cassette.ClearSavedAudio())
				{
					num++;
				}
			}
		}
		arg.ReplyWith(string.Format("Deleted the contents of {0} cassettes", num));
	}

	// Token: 0x0600095D RID: 2397 RVA: 0x00059020 File Offset: 0x00057220
	[ServerVar]
	public static void ClearCassettesByUser(ConsoleSystem.Arg arg)
	{
		ulong @uint = arg.GetUInt64(0, 0UL);
		int num = 0;
		using (IEnumerator<global::BaseNetworkable> enumerator = global::BaseNetworkable.serverEntities.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				global::Cassette cassette;
				if ((cassette = (enumerator.Current as global::Cassette)) != null && cassette.CreatorSteamId == @uint)
				{
					cassette.ClearSavedAudio();
					num++;
				}
			}
		}
		arg.ReplyWith(string.Format("Deleted {0} cassettes recorded by {1}", num, @uint));
	}

	// Token: 0x170000F6 RID: 246
	// (get) Token: 0x0600095E RID: 2398 RVA: 0x000590AC File Offset: 0x000572AC
	// (set) Token: 0x0600095F RID: 2399 RVA: 0x000590B4 File Offset: 0x000572B4
	public uint AudioId { get; private set; }

	// Token: 0x170000F7 RID: 247
	// (get) Token: 0x06000960 RID: 2400 RVA: 0x000590BD File Offset: 0x000572BD
	public SoundDefinition PreloadedAudio
	{
		get
		{
			return this.PreloadContent.GetSoundContent(this.preloadedAudioId, this.PreloadType);
		}
	}

	// Token: 0x06000961 RID: 2401 RVA: 0x000590D8 File Offset: 0x000572D8
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.cassette != null)
		{
			uint audioId = this.AudioId;
			this.AudioId = info.msg.cassette.audioId;
			this.CreatorSteamId = info.msg.cassette.creatorSteamId;
			this.preloadedAudioId = info.msg.cassette.preloadAudioId;
			if (base.isServer && info.msg.cassette.holder.IsValid)
			{
				global::BaseNetworkable baseNetworkable = global::BaseNetworkable.serverEntities.Find(info.msg.cassette.holder);
				ICassettePlayer cassettePlayer;
				if (baseNetworkable != null && (cassettePlayer = (baseNetworkable as ICassettePlayer)) != null)
				{
					this.currentCassettePlayer = cassettePlayer;
				}
			}
		}
	}

	// Token: 0x06000962 RID: 2402 RVA: 0x0005919C File Offset: 0x0005739C
	public void AssignPreloadContent()
	{
		switch (this.PreloadType)
		{
		case PreloadedCassetteContent.PreloadType.Short:
			this.preloadedAudioId = UnityEngine.Random.Range(0, this.PreloadContent.ShortTapeContent.Length);
			return;
		case PreloadedCassetteContent.PreloadType.Medium:
			this.preloadedAudioId = UnityEngine.Random.Range(0, this.PreloadContent.MediumTapeContent.Length);
			return;
		case PreloadedCassetteContent.PreloadType.Long:
			this.preloadedAudioId = UnityEngine.Random.Range(0, this.PreloadContent.LongTapeContent.Length);
			return;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x06000963 RID: 2403 RVA: 0x00059218 File Offset: 0x00057418
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.cassette = Facepunch.Pool.Get<ProtoBuf.Cassette>();
		info.msg.cassette.audioId = this.AudioId;
		info.msg.cassette.creatorSteamId = this.CreatorSteamId;
		info.msg.cassette.preloadAudioId = this.preloadedAudioId;
		if (!this.currentCassettePlayer.IsUnityNull<ICassettePlayer>() && this.currentCassettePlayer.ToBaseEntity.IsValid())
		{
			info.msg.cassette.holder = this.currentCassettePlayer.ToBaseEntity.net.ID;
		}
	}

	// Token: 0x06000964 RID: 2404 RVA: 0x000592C4 File Offset: 0x000574C4
	public override void OnParentChanging(global::BaseEntity oldParent, global::BaseEntity newParent)
	{
		base.OnParentChanging(oldParent, newParent);
		ICassettePlayer cassettePlayer = this.currentCassettePlayer;
		if (cassettePlayer != null)
		{
			cassettePlayer.OnCassetteRemoved(this);
		}
		this.currentCassettePlayer = null;
		ICassettePlayer cassettePlayer2;
		if (newParent != null && (cassettePlayer2 = (newParent as ICassettePlayer)) != null)
		{
			base.Invoke(new Action(this.DelayedCassetteInserted), 0.1f);
			this.currentCassettePlayer = cassettePlayer2;
		}
	}

	// Token: 0x06000965 RID: 2405 RVA: 0x00059323 File Offset: 0x00057523
	private void DelayedCassetteInserted()
	{
		if (this.currentCassettePlayer != null)
		{
			this.currentCassettePlayer.OnCassetteInserted(this);
		}
	}

	// Token: 0x06000966 RID: 2406 RVA: 0x00059339 File Offset: 0x00057539
	public void SetAudioId(uint id, ulong userId)
	{
		this.AudioId = id;
		this.CreatorSteamId = userId;
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000967 RID: 2407 RVA: 0x00059350 File Offset: 0x00057550
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.CallsPerSecond(1UL)]
	public void Server_MakeNewFile(global::BaseEntity.RPCMessage msg)
	{
		if (msg.player == null)
		{
			return;
		}
		global::HeldEntity heldEntity;
		if (base.GetParentEntity() != null && (heldEntity = (base.GetParentEntity() as global::HeldEntity)) != null && heldEntity.GetOwnerPlayer() != msg.player)
		{
			Debug.Log("Player mismatch!");
			return;
		}
		byte[] data = msg.read.BytesWithSize(10485760U);
		ulong userId = msg.read.UInt64();
		if (!global::Cassette.IsOggValid(data, this))
		{
			return;
		}
		FileStorage.server.RemoveAllByEntity(this.net.ID);
		uint id = FileStorage.server.Store(data, FileStorage.Type.ogg, this.net.ID, 0U);
		this.SetAudioId(id, userId);
	}

	// Token: 0x06000968 RID: 2408 RVA: 0x00059404 File Offset: 0x00057604
	private bool ClearSavedAudio()
	{
		if (this.AudioId == 0U)
		{
			return false;
		}
		FileStorage.server.RemoveAllByEntity(this.net.ID);
		this.AudioId = 0U;
		this.CreatorSteamId = 0UL;
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		return true;
	}

	// Token: 0x06000969 RID: 2409 RVA: 0x0005943C File Offset: 0x0005763C
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		this.ClearSavedAudio();
	}

	// Token: 0x170000F8 RID: 248
	// (get) Token: 0x0600096A RID: 2410 RVA: 0x0005944B File Offset: 0x0005764B
	public uint[] GetContentCRCs
	{
		get
		{
			if (this.AudioId <= 0U)
			{
				return Array.Empty<uint>();
			}
			return new uint[]
			{
				this.AudioId
			};
		}
	}

	// Token: 0x0600096B RID: 2411 RVA: 0x0005946B File Offset: 0x0005766B
	public void ClearContent()
	{
		this.AudioId = 0U;
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x170000F9 RID: 249
	// (get) Token: 0x0600096C RID: 2412 RVA: 0x0004E73F File Offset: 0x0004C93F
	public UGCType ContentType
	{
		get
		{
			return UGCType.AudioOgg;
		}
	}

	// Token: 0x170000FA RID: 250
	// (get) Token: 0x0600096D RID: 2413 RVA: 0x0005947B File Offset: 0x0005767B
	public List<ulong> EditingHistory
	{
		get
		{
			return new List<ulong>
			{
				this.CreatorSteamId
			};
		}
	}

	// Token: 0x170000FB RID: 251
	// (get) Token: 0x0600096E RID: 2414 RVA: 0x000037E7 File Offset: 0x000019E7
	public global::BaseNetworkable UgcEntity
	{
		get
		{
			return this;
		}
	}

	// Token: 0x0600096F RID: 2415 RVA: 0x0005948E File Offset: 0x0005768E
	public static bool IsOggValid(byte[] data, global::Cassette c)
	{
		return global::Cassette.IsOggValid(data, c.MaxCassetteLength);
	}

	// Token: 0x06000970 RID: 2416 RVA: 0x0005949C File Offset: 0x0005769C
	private static bool IsOggValid(byte[] data, float maxLength)
	{
		if (data == null)
		{
			return false;
		}
		if (global::Cassette.ByteToMegabyte(data.Length) >= global::Cassette.MaxCassetteFileSizeMB)
		{
			Debug.Log("Audio file is too large! Aborting");
			return false;
		}
		double oggLength = global::Cassette.GetOggLength(data);
		if (oggLength > (double)(maxLength * 1.2f))
		{
			Debug.Log(string.Format("Audio duration is longer than cassette limit! {0} > {1}", oggLength, maxLength * 1.2f));
			return false;
		}
		return true;
	}

	// Token: 0x06000971 RID: 2417 RVA: 0x000594FF File Offset: 0x000576FF
	private static float ByteToMegabyte(int byteSize)
	{
		return (float)byteSize / 1024f / 1024f;
	}

	// Token: 0x06000972 RID: 2418 RVA: 0x00059510 File Offset: 0x00057710
	private static double GetOggLength(byte[] t)
	{
		int num = t.Length;
		long num2 = -1L;
		int num3 = -1;
		for (int i = num - 1 - 8 - 2 - 4; i >= 0; i--)
		{
			if (t[i] == 79 && t[i + 1] == 103 && t[i + 2] == 103 && t[i + 3] == 83)
			{
				num2 = BitConverter.ToInt64(new byte[]
				{
					t[i + 6],
					t[i + 7],
					t[i + 8],
					t[i + 9],
					t[i + 10],
					t[i + 11],
					t[i + 12],
					t[i + 13]
				}, 0);
				break;
			}
		}
		for (int j = 0; j < num - 8 - 2 - 4; j++)
		{
			if (t[j] == 118 && t[j + 1] == 111 && t[j + 2] == 114 && t[j + 3] == 98 && t[j + 4] == 105 && t[j + 5] == 115)
			{
				num3 = BitConverter.ToInt32(new byte[]
				{
					t[j + 11],
					t[j + 12],
					t[j + 13],
					t[j + 14]
				}, 0);
				break;
			}
		}
		if (RecorderTool.debugRecording)
		{
			Debug.Log(string.Format("{0} / {1}", num2, num3));
		}
		return (double)num2 / (double)num3;
	}
}
