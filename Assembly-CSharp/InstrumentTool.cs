using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000087 RID: 135
public class InstrumentTool : HeldEntity
{
	// Token: 0x04000837 RID: 2103
	public InstrumentKeyController KeyController;

	// Token: 0x04000838 RID: 2104
	public SoundDefinition DeploySound;

	// Token: 0x04000839 RID: 2105
	public Vector2 PitchClamp = new Vector2(-90f, 90f);

	// Token: 0x0400083A RID: 2106
	public bool UseAnimationSlotEvents;

	// Token: 0x0400083B RID: 2107
	public Transform MuzzleT;

	// Token: 0x0400083C RID: 2108
	public bool UsableByAutoTurrets;

	// Token: 0x0400083D RID: 2109
	private NoteBindingCollection.NoteData lastPlayedTurretData;

	// Token: 0x06000CB2 RID: 3250 RVA: 0x0006DFA0 File Offset: 0x0006C1A0
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("InstrumentTool.OnRpcMessage", 0))
		{
			if (rpc == 1625188589U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_PlayNote ");
				}
				using (TimeWarning.New("Server_PlayNote", 0))
				{
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
							this.Server_PlayNote(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in Server_PlayNote");
					}
				}
				return true;
			}
			if (rpc == 705843933U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_StopNote ");
				}
				using (TimeWarning.New("Server_StopNote", 0))
				{
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage msg3 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_StopNote(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in Server_StopNote");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000CB3 RID: 3251 RVA: 0x0006E200 File Offset: 0x0006C400
	[BaseEntity.RPC_Server]
	private void Server_PlayNote(BaseEntity.RPCMessage msg)
	{
		int arg = msg.read.Int32();
		int arg2 = msg.read.Int32();
		int arg3 = msg.read.Int32();
		float arg4 = msg.read.Float();
		this.KeyController.ProcessServerPlayedNote(base.GetOwnerPlayer());
		base.ClientRPC<int, int, int, float>(null, "Client_PlayNote", arg, arg2, arg3, arg4);
	}

	// Token: 0x06000CB4 RID: 3252 RVA: 0x0006E260 File Offset: 0x0006C460
	[BaseEntity.RPC_Server]
	private void Server_StopNote(BaseEntity.RPCMessage msg)
	{
		int arg = msg.read.Int32();
		int arg2 = msg.read.Int32();
		int arg3 = msg.read.Int32();
		base.ClientRPC<int, int, int>(null, "Client_StopNote", arg, arg2, arg3);
	}

	// Token: 0x06000CB5 RID: 3253 RVA: 0x0006E2A0 File Offset: 0x0006C4A0
	public override void ServerUse()
	{
		base.ServerUse();
		if (base.IsInvoking(new Action(this.StopAfterTime)))
		{
			return;
		}
		this.lastPlayedTurretData = this.KeyController.Bindings.BaseBindings[UnityEngine.Random.Range(0, this.KeyController.Bindings.BaseBindings.Length)];
		base.ClientRPC<int, int, int, float>(null, "Client_PlayNote", (int)this.lastPlayedTurretData.Note, (int)this.lastPlayedTurretData.Type, this.lastPlayedTurretData.NoteOctave, 1f);
		base.Invoke(new Action(this.StopAfterTime), 0.2f);
	}

	// Token: 0x06000CB6 RID: 3254 RVA: 0x0006E344 File Offset: 0x0006C544
	private void StopAfterTime()
	{
		base.ClientRPC<int, int, int>(null, "Client_StopNote", (int)this.lastPlayedTurretData.Note, (int)this.lastPlayedTurretData.Type, this.lastPlayedTurretData.NoteOctave);
	}

	// Token: 0x17000134 RID: 308
	// (get) Token: 0x06000CB7 RID: 3255 RVA: 0x0006E373 File Offset: 0x0006C573
	public override bool IsUsableByTurret
	{
		get
		{
			return this.UsableByAutoTurrets;
		}
	}

	// Token: 0x17000135 RID: 309
	// (get) Token: 0x06000CB8 RID: 3256 RVA: 0x0006E37B File Offset: 0x0006C57B
	public override Transform MuzzleTransform
	{
		get
		{
			return this.MuzzleT;
		}
	}

	// Token: 0x06000CB9 RID: 3257 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool IsInstrument()
	{
		return true;
	}
}
