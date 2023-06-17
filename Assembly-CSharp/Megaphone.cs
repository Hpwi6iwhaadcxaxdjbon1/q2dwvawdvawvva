using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200009B RID: 155
public class Megaphone : HeldEntity
{
	// Token: 0x04000925 RID: 2341
	[Header("Megaphone")]
	public VoiceProcessor voiceProcessor;

	// Token: 0x04000926 RID: 2342
	public float VoiceDamageMinFrequency = 2f;

	// Token: 0x04000927 RID: 2343
	public float VoiceDamageAmount = 1f;

	// Token: 0x04000928 RID: 2344
	public AudioSource VoiceSource;

	// Token: 0x04000929 RID: 2345
	public SoundDefinition StartBroadcastingSfx;

	// Token: 0x0400092A RID: 2346
	public SoundDefinition StopBroadcastingSfx;

	// Token: 0x06000E10 RID: 3600 RVA: 0x000773F8 File Offset: 0x000755F8
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Megaphone.OnRpcMessage", 0))
		{
			if (rpc == 4196056309U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_ToggleBroadcasting ");
				}
				using (TimeWarning.New("Server_ToggleBroadcasting", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.FromOwner.Test(4196056309U, "Server_ToggleBroadcasting", this, player))
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
							this.Server_ToggleBroadcasting(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in Server_ToggleBroadcasting");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x17000149 RID: 329
	// (get) Token: 0x06000E11 RID: 3601 RVA: 0x0007755C File Offset: 0x0007575C
	// (set) Token: 0x06000E12 RID: 3602 RVA: 0x00077563 File Offset: 0x00075763
	[ReplicatedVar(Default = "100")]
	public static float MegaphoneVoiceRange { get; set; } = 100f;

	// Token: 0x06000E13 RID: 3603 RVA: 0x0007756C File Offset: 0x0007576C
	private void UpdateItemCondition()
	{
		Item ownerItem = base.GetOwnerItem();
		if (ownerItem == null || !ownerItem.hasCondition)
		{
			return;
		}
		ownerItem.LoseCondition(this.VoiceDamageAmount);
	}

	// Token: 0x06000E14 RID: 3604 RVA: 0x00077598 File Offset: 0x00075798
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.FromOwner]
	private void Server_ToggleBroadcasting(BaseEntity.RPCMessage msg)
	{
		bool flag = msg.read.Int8() == 1;
		base.SetFlag(BaseEntity.Flags.On, flag, false, true);
		if (flag)
		{
			if (!base.IsInvoking(new Action(this.UpdateItemCondition)))
			{
				base.InvokeRepeating(new Action(this.UpdateItemCondition), 0f, this.VoiceDamageMinFrequency);
				return;
			}
		}
		else if (base.IsInvoking(new Action(this.UpdateItemCondition)))
		{
			base.CancelInvoke(new Action(this.UpdateItemCondition));
		}
	}
}
