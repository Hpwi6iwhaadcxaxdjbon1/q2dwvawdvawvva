using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Audio;

// Token: 0x0200009C RID: 156
public class MicrophoneStand : BaseMountable
{
	// Token: 0x0400092C RID: 2348
	public VoiceProcessor VoiceProcessor;

	// Token: 0x0400092D RID: 2349
	public AudioSource VoiceSource;

	// Token: 0x0400092E RID: 2350
	private global::MicrophoneStand.SpeechMode currentSpeechMode;

	// Token: 0x0400092F RID: 2351
	public AudioMixerGroup NormalMix;

	// Token: 0x04000930 RID: 2352
	public AudioMixerGroup HighPitchMix;

	// Token: 0x04000931 RID: 2353
	public AudioMixerGroup LowPitchMix;

	// Token: 0x04000932 RID: 2354
	public Translate.Phrase NormalPhrase = new Translate.Phrase("microphone_normal", "Normal");

	// Token: 0x04000933 RID: 2355
	public Translate.Phrase NormalDescPhrase = new Translate.Phrase("microphone_normal_desc", "No voice effect");

	// Token: 0x04000934 RID: 2356
	public Translate.Phrase HighPitchPhrase = new Translate.Phrase("microphone_high", "High Pitch");

	// Token: 0x04000935 RID: 2357
	public Translate.Phrase HighPitchDescPhrase = new Translate.Phrase("microphone_high_desc", "High pitch voice");

	// Token: 0x04000936 RID: 2358
	public Translate.Phrase LowPitchPhrase = new Translate.Phrase("microphone_low", "Low");

	// Token: 0x04000937 RID: 2359
	public Translate.Phrase LowPitchDescPhrase = new Translate.Phrase("microphone_low_desc", "Low pitch voice");

	// Token: 0x04000938 RID: 2360
	public GameObjectRef IOSubEntity;

	// Token: 0x04000939 RID: 2361
	public Transform IOSubEntitySpawnPos;

	// Token: 0x0400093A RID: 2362
	public bool IsStatic;

	// Token: 0x0400093B RID: 2363
	public EntityRef<global::IOEntity> ioEntity;

	// Token: 0x06000E17 RID: 3607 RVA: 0x00077644 File Offset: 0x00075844
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("MicrophoneStand.OnRpcMessage", 0))
		{
			if (rpc == 1420522459U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SetMode ");
				}
				using (TimeWarning.New("SetMode", 0))
				{
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage mode = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SetMode(mode);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in SetMode");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000E18 RID: 3608 RVA: 0x00077768 File Offset: 0x00075968
	[global::BaseEntity.RPC_Server]
	public void SetMode(global::BaseEntity.RPCMessage msg)
	{
		if (msg.player != this._mounted)
		{
			return;
		}
		global::MicrophoneStand.SpeechMode speechMode = (global::MicrophoneStand.SpeechMode)msg.read.Int32();
		if (speechMode != this.currentSpeechMode)
		{
			this.currentSpeechMode = speechMode;
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06000E19 RID: 3609 RVA: 0x000777AC File Offset: 0x000759AC
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.microphoneStand == null)
		{
			info.msg.microphoneStand = Facepunch.Pool.Get<ProtoBuf.MicrophoneStand>();
		}
		info.msg.microphoneStand.microphoneMode = (int)this.currentSpeechMode;
		info.msg.microphoneStand.IORef = this.ioEntity.uid;
	}

	// Token: 0x06000E1A RID: 3610 RVA: 0x00077810 File Offset: 0x00075A10
	public void SpawnChildEntity()
	{
		MicrophoneStandIOEntity microphoneStandIOEntity = GameManager.server.CreateEntity(this.IOSubEntity.resourcePath, this.IOSubEntitySpawnPos.localPosition, this.IOSubEntitySpawnPos.localRotation, true) as MicrophoneStandIOEntity;
		microphoneStandIOEntity.enableSaving = this.enableSaving;
		microphoneStandIOEntity.SetParent(this, false, false);
		microphoneStandIOEntity.Spawn();
		this.ioEntity.Set(microphoneStandIOEntity);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000E1B RID: 3611 RVA: 0x0007787D File Offset: 0x00075A7D
	public override void OnDeployed(global::BaseEntity parent, global::BasePlayer deployedBy, global::Item fromItem)
	{
		base.OnDeployed(parent, deployedBy, fromItem);
		this.SpawnChildEntity();
	}

	// Token: 0x06000E1C RID: 3612 RVA: 0x00077890 File Offset: 0x00075A90
	public override void PostMapEntitySpawn()
	{
		base.PostMapEntitySpawn();
		if (this.IsStatic)
		{
			this.SpawnChildEntity();
			int num = 128;
			List<global::ConnectedSpeaker> list = Facepunch.Pool.GetList<global::ConnectedSpeaker>();
			GamePhysics.OverlapSphere<global::ConnectedSpeaker>(base.transform.position, (float)num, list, 256, QueryTriggerInteraction.Ignore);
			global::IOEntity ioentity = this.ioEntity.Get(true);
			List<global::MicrophoneStand> list2 = Facepunch.Pool.GetList<global::MicrophoneStand>();
			int num2 = 0;
			foreach (global::ConnectedSpeaker connectedSpeaker in list)
			{
				bool flag = true;
				list2.Clear();
				GamePhysics.OverlapSphere<global::MicrophoneStand>(connectedSpeaker.transform.position, (float)num, list2, 256, QueryTriggerInteraction.Ignore);
				if (list2.Count > 1)
				{
					float num3 = base.Distance(connectedSpeaker);
					foreach (global::MicrophoneStand microphoneStand in list2)
					{
						if (!microphoneStand.isClient && microphoneStand.Distance(connectedSpeaker) < num3)
						{
							flag = false;
							break;
						}
					}
				}
				if (flag)
				{
					ioentity.outputs[0].connectedTo.Set(connectedSpeaker);
					connectedSpeaker.inputs[0].connectedTo.Set(ioentity);
					ioentity = connectedSpeaker;
					num2++;
				}
			}
			Facepunch.Pool.FreeList<global::ConnectedSpeaker>(ref list);
			Facepunch.Pool.FreeList<global::MicrophoneStand>(ref list2);
		}
	}

	// Token: 0x06000E1D RID: 3613 RVA: 0x00077A00 File Offset: 0x00075C00
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.microphoneStand != null)
		{
			this.currentSpeechMode = (global::MicrophoneStand.SpeechMode)info.msg.microphoneStand.microphoneMode;
			this.ioEntity.uid = info.msg.microphoneStand.IORef;
		}
	}

	// Token: 0x02000BE1 RID: 3041
	public enum SpeechMode
	{
		// Token: 0x0400412F RID: 16687
		Normal,
		// Token: 0x04004130 RID: 16688
		HighPitch,
		// Token: 0x04004131 RID: 16689
		LowPitch
	}
}
