using System;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000060 RID: 96
public class ConnectedSpeaker : global::IOEntity
{
	// Token: 0x040006A7 RID: 1703
	public AudioSource SoundSource;

	// Token: 0x040006A8 RID: 1704
	private EntityRef<global::IOEntity> connectedTo;

	// Token: 0x040006A9 RID: 1705
	public VoiceProcessor VoiceProcessor;

	// Token: 0x060009F6 RID: 2550 RVA: 0x0005D69C File Offset: 0x0005B89C
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ConnectedSpeaker.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060009F7 RID: 2551 RVA: 0x0005D6DC File Offset: 0x0005B8DC
	public override void OnFlagsChanged(global::BaseEntity.Flags old, global::BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (base.isServer && old.HasFlag(global::BaseEntity.Flags.Reserved8) != next.HasFlag(global::BaseEntity.Flags.Reserved8))
		{
			if (next.HasFlag(global::BaseEntity.Flags.Reserved8))
			{
				IAudioConnectionSource connectionSource = this.GetConnectionSource(this, global::BoomBox.BacktrackLength);
				if (connectionSource != null)
				{
					base.ClientRPC<NetworkableId>(null, "Client_PlayAudioFrom", connectionSource.ToEntity().net.ID);
					this.connectedTo.Set(connectionSource.ToEntity());
					return;
				}
			}
			else if (this.connectedTo.IsSet)
			{
				base.ClientRPC<NetworkableId>(null, "Client_StopPlayingAudio", this.connectedTo.uid);
				this.connectedTo.Set(null);
			}
		}
	}

	// Token: 0x060009F8 RID: 2552 RVA: 0x0005D7B0 File Offset: 0x0005B9B0
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.connectedSpeaker != null)
		{
			this.connectedTo.uid = info.msg.connectedSpeaker.connectedTo;
		}
	}

	// Token: 0x060009F9 RID: 2553 RVA: 0x0005D7E4 File Offset: 0x0005B9E4
	private IAudioConnectionSource GetConnectionSource(global::IOEntity entity, int depth)
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
			IAudioConnectionSource result;
			if (ioentity != null && (result = (ioentity as IAudioConnectionSource)) != null)
			{
				return result;
			}
			if (ioentity != null)
			{
				IAudioConnectionSource connectionSource = this.GetConnectionSource(ioentity, depth - 1);
				if (connectionSource != null)
				{
					return connectionSource;
				}
			}
		}
		return null;
	}

	// Token: 0x060009FA RID: 2554 RVA: 0x0005D860 File Offset: 0x0005BA60
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.connectedSpeaker == null)
		{
			info.msg.connectedSpeaker = Pool.Get<ProtoBuf.ConnectedSpeaker>();
		}
		info.msg.connectedSpeaker.connectedTo = this.connectedTo.uid;
	}
}
