using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000034 RID: 52
public class AudioVisualisationEntity : global::IOEntity
{
	// Token: 0x040001A0 RID: 416
	private EntityRef<global::BaseEntity> connectedTo;

	// Token: 0x040001A4 RID: 420
	public GameObjectRef SettingsDialog;

	// Token: 0x0600015C RID: 348 RVA: 0x00022114 File Offset: 0x00020314
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("AudioVisualisationEntity.OnRpcMessage", 0))
		{
			if (rpc == 4002266471U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ServerUpdateSettings ");
				}
				using (TimeWarning.New("ServerUpdateSettings", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(4002266471U, "ServerUpdateSettings", this, player, 5UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(4002266471U, "ServerUpdateSettings", this, player, 3f))
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
							this.ServerUpdateSettings(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in ServerUpdateSettings");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x1700002E RID: 46
	// (get) Token: 0x0600015D RID: 349 RVA: 0x000222D4 File Offset: 0x000204D4
	// (set) Token: 0x0600015E RID: 350 RVA: 0x000222DC File Offset: 0x000204DC
	public AudioVisualisationEntity.LightColour currentColour { get; private set; }

	// Token: 0x1700002F RID: 47
	// (get) Token: 0x0600015F RID: 351 RVA: 0x000222E5 File Offset: 0x000204E5
	// (set) Token: 0x06000160 RID: 352 RVA: 0x000222ED File Offset: 0x000204ED
	public AudioVisualisationEntity.VolumeSensitivity currentVolumeSensitivity { get; private set; } = AudioVisualisationEntity.VolumeSensitivity.Medium;

	// Token: 0x17000030 RID: 48
	// (get) Token: 0x06000161 RID: 353 RVA: 0x000222F6 File Offset: 0x000204F6
	// (set) Token: 0x06000162 RID: 354 RVA: 0x000222FE File Offset: 0x000204FE
	public AudioVisualisationEntity.Speed currentSpeed { get; private set; } = AudioVisualisationEntity.Speed.Medium;

	// Token: 0x17000031 RID: 49
	// (get) Token: 0x06000163 RID: 355 RVA: 0x00022307 File Offset: 0x00020507
	// (set) Token: 0x06000164 RID: 356 RVA: 0x0002230F File Offset: 0x0002050F
	public int currentGradient { get; private set; }

	// Token: 0x06000165 RID: 357 RVA: 0x00022318 File Offset: 0x00020518
	public override void OnFlagsChanged(global::BaseEntity.Flags old, global::BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (base.isServer && old.HasFlag(global::BaseEntity.Flags.Reserved8) != next.HasFlag(global::BaseEntity.Flags.Reserved8) && next.HasFlag(global::BaseEntity.Flags.Reserved8))
		{
			int num = global::BoomBox.BacktrackLength * 4;
			global::IOEntity audioSource = this.GetAudioSource(this, ref num);
			if (audioSource != null)
			{
				base.ClientRPC<NetworkableId>(null, "Client_PlayAudioFrom", audioSource.net.ID);
			}
			this.connectedTo.Set(audioSource);
		}
	}

	// Token: 0x06000166 RID: 358 RVA: 0x000223BC File Offset: 0x000205BC
	private global::IOEntity GetAudioSource(global::IOEntity entity, ref int depth)
	{
		if (depth <= 0)
		{
			return null;
		}
		global::IOEntity.IOSlot[] inputs = entity.inputs;
		for (int i = 0; i < inputs.Length; i++)
		{
			global::IOEntity ioentity = inputs[i].connectedTo.Get(base.isServer);
			if (ioentity == this)
			{
				return null;
			}
			IAudioConnectionSource audioConnectionSource;
			if (ioentity != null && ioentity.TryGetComponent<IAudioConnectionSource>(out audioConnectionSource))
			{
				return ioentity;
			}
			AudioVisualisationEntity audioVisualisationEntity;
			if (ioentity != null && ioentity.TryGetComponent<AudioVisualisationEntity>(out audioVisualisationEntity) && audioVisualisationEntity.connectedTo.IsSet)
			{
				return audioVisualisationEntity.connectedTo.Get(base.isServer) as global::IOEntity;
			}
			if (ioentity != null)
			{
				depth--;
				ioentity = this.GetAudioSource(ioentity, ref depth);
				IAudioConnectionSource audioConnectionSource2;
				if (ioentity != null && ioentity.TryGetComponent<IAudioConnectionSource>(out audioConnectionSource2))
				{
					return ioentity;
				}
			}
		}
		return null;
	}

	// Token: 0x06000167 RID: 359 RVA: 0x00022488 File Offset: 0x00020688
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.connectedSpeaker == null)
		{
			info.msg.connectedSpeaker = Facepunch.Pool.Get<ProtoBuf.ConnectedSpeaker>();
		}
		info.msg.connectedSpeaker.connectedTo = this.connectedTo.uid;
		if (info.msg.audioEntity == null)
		{
			info.msg.audioEntity = Facepunch.Pool.Get<AudioEntity>();
		}
		info.msg.audioEntity.colourMode = (int)this.currentColour;
		info.msg.audioEntity.volumeRange = (int)this.currentVolumeSensitivity;
		info.msg.audioEntity.speed = (int)this.currentSpeed;
		info.msg.audioEntity.gradient = this.currentGradient;
	}

	// Token: 0x06000168 RID: 360 RVA: 0x0002254C File Offset: 0x0002074C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void ServerUpdateSettings(global::BaseEntity.RPCMessage msg)
	{
		int num = msg.read.Int32();
		int num2 = msg.read.Int32();
		int num3 = msg.read.Int32();
		int num4 = msg.read.Int32();
		if (this.currentColour != (AudioVisualisationEntity.LightColour)num || this.currentVolumeSensitivity != (AudioVisualisationEntity.VolumeSensitivity)num2 || this.currentSpeed != (AudioVisualisationEntity.Speed)num3 || this.currentGradient != num4)
		{
			this.currentColour = (AudioVisualisationEntity.LightColour)num;
			this.currentVolumeSensitivity = (AudioVisualisationEntity.VolumeSensitivity)num2;
			this.currentSpeed = (AudioVisualisationEntity.Speed)num3;
			this.currentGradient = num4;
			this.MarkDirty();
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06000169 RID: 361 RVA: 0x000225D8 File Offset: 0x000207D8
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.audioEntity != null)
		{
			this.currentColour = (AudioVisualisationEntity.LightColour)info.msg.audioEntity.colourMode;
			this.currentVolumeSensitivity = (AudioVisualisationEntity.VolumeSensitivity)info.msg.audioEntity.volumeRange;
			this.currentSpeed = (AudioVisualisationEntity.Speed)info.msg.audioEntity.speed;
			this.currentGradient = info.msg.audioEntity.gradient;
		}
		if (info.msg.connectedSpeaker != null)
		{
			this.connectedTo.uid = info.msg.connectedSpeaker.connectedTo;
		}
	}

	// Token: 0x02000B54 RID: 2900
	public enum LightColour
	{
		// Token: 0x04003EB5 RID: 16053
		Red,
		// Token: 0x04003EB6 RID: 16054
		Green,
		// Token: 0x04003EB7 RID: 16055
		Blue,
		// Token: 0x04003EB8 RID: 16056
		Yellow,
		// Token: 0x04003EB9 RID: 16057
		Pink
	}

	// Token: 0x02000B55 RID: 2901
	public enum VolumeSensitivity
	{
		// Token: 0x04003EBB RID: 16059
		Small,
		// Token: 0x04003EBC RID: 16060
		Medium,
		// Token: 0x04003EBD RID: 16061
		Large
	}

	// Token: 0x02000B56 RID: 2902
	public enum Speed
	{
		// Token: 0x04003EBF RID: 16063
		Low,
		// Token: 0x04003EC0 RID: 16064
		Medium,
		// Token: 0x04003EC1 RID: 16065
		High
	}
}
