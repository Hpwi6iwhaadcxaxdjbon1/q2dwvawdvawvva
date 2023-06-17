using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200005A RID: 90
public class CodeLock : BaseLock
{
	// Token: 0x0400066A RID: 1642
	public GameObjectRef keyEnterDialog;

	// Token: 0x0400066B RID: 1643
	public GameObjectRef effectUnlocked;

	// Token: 0x0400066C RID: 1644
	public GameObjectRef effectLocked;

	// Token: 0x0400066D RID: 1645
	public GameObjectRef effectDenied;

	// Token: 0x0400066E RID: 1646
	public GameObjectRef effectCodeChanged;

	// Token: 0x0400066F RID: 1647
	public GameObjectRef effectShock;

	// Token: 0x04000670 RID: 1648
	private bool hasCode;

	// Token: 0x04000671 RID: 1649
	public const global::BaseEntity.Flags Flag_CodeEntryBlocked = global::BaseEntity.Flags.Reserved11;

	// Token: 0x04000672 RID: 1650
	public static readonly Translate.Phrase blockwarning = new Translate.Phrase("codelock.blockwarning", "Further failed attempts will block code entry for some time");

	// Token: 0x04000673 RID: 1651
	[ServerVar]
	public static float maxFailedAttempts = 8f;

	// Token: 0x04000674 RID: 1652
	[ServerVar]
	public static float lockoutCooldown = 900f;

	// Token: 0x04000675 RID: 1653
	private bool hasGuestCode;

	// Token: 0x04000676 RID: 1654
	private string code = string.Empty;

	// Token: 0x04000677 RID: 1655
	private string guestCode = string.Empty;

	// Token: 0x04000678 RID: 1656
	public List<ulong> whitelistPlayers = new List<ulong>();

	// Token: 0x04000679 RID: 1657
	public List<ulong> guestPlayers = new List<ulong>();

	// Token: 0x0400067A RID: 1658
	private int wrongCodes;

	// Token: 0x0400067B RID: 1659
	private float lastWrongTime = float.NegativeInfinity;

	// Token: 0x060009AE RID: 2478 RVA: 0x0005AC78 File Offset: 0x00058E78
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("CodeLock.OnRpcMessage", 0))
		{
			if (rpc == 4013784361U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_ChangeCode ");
				}
				using (TimeWarning.New("RPC_ChangeCode", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(4013784361U, "RPC_ChangeCode", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpc2 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_ChangeCode(rpc2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_ChangeCode");
					}
				}
				return true;
			}
			if (rpc == 2626067433U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - TryLock ");
				}
				using (TimeWarning.New("TryLock", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(2626067433U, "TryLock", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpc3 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.TryLock(rpc3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in TryLock");
					}
				}
				return true;
			}
			if (rpc == 1718262U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - TryUnlock ");
				}
				using (TimeWarning.New("TryUnlock", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(1718262U, "TryUnlock", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpc4 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.TryUnlock(rpc4);
						}
					}
					catch (Exception exception3)
					{
						Debug.LogException(exception3);
						player.Kick("RPC Error in TryUnlock");
					}
				}
				return true;
			}
			if (rpc == 418605506U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - UnlockWithCode ");
				}
				using (TimeWarning.New("UnlockWithCode", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(418605506U, "UnlockWithCode", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpc5 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.UnlockWithCode(rpc5);
						}
					}
					catch (Exception exception4)
					{
						Debug.LogException(exception4);
						player.Kick("RPC Error in UnlockWithCode");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060009AF RID: 2479 RVA: 0x0005B230 File Offset: 0x00059430
	public bool IsCodeEntryBlocked()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved11);
	}

	// Token: 0x060009B0 RID: 2480 RVA: 0x0005B240 File Offset: 0x00059440
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.codeLock != null)
		{
			this.hasCode = info.msg.codeLock.hasCode;
			this.hasGuestCode = info.msg.codeLock.hasGuestCode;
			if (info.msg.codeLock.pv != null)
			{
				this.code = info.msg.codeLock.pv.code;
				this.whitelistPlayers = info.msg.codeLock.pv.users;
				this.guestCode = info.msg.codeLock.pv.guestCode;
				this.guestPlayers = info.msg.codeLock.pv.guestUsers;
			}
		}
	}

	// Token: 0x060009B1 RID: 2481 RVA: 0x0005B30E File Offset: 0x0005950E
	internal void DoEffect(string effect)
	{
		Effect.server.Run(effect, this, 0U, Vector3.zero, Vector3.forward, null, false);
	}

	// Token: 0x060009B2 RID: 2482 RVA: 0x0005B324 File Offset: 0x00059524
	public override bool OnTryToOpen(global::BasePlayer player)
	{
		if (!base.IsLocked())
		{
			return true;
		}
		if (this.whitelistPlayers.Contains(player.userID) || this.guestPlayers.Contains(player.userID))
		{
			this.DoEffect(this.effectUnlocked.resourcePath);
			return true;
		}
		this.DoEffect(this.effectDenied.resourcePath);
		return false;
	}

	// Token: 0x060009B3 RID: 2483 RVA: 0x0005B388 File Offset: 0x00059588
	public override bool OnTryToClose(global::BasePlayer player)
	{
		if (!base.IsLocked())
		{
			return true;
		}
		if (this.whitelistPlayers.Contains(player.userID) || this.guestPlayers.Contains(player.userID))
		{
			this.DoEffect(this.effectUnlocked.resourcePath);
			return true;
		}
		this.DoEffect(this.effectDenied.resourcePath);
		return false;
	}

	// Token: 0x060009B4 RID: 2484 RVA: 0x00007A3C File Offset: 0x00005C3C
	public override bool CanUseNetworkCache(Connection connection)
	{
		return false;
	}

	// Token: 0x060009B5 RID: 2485 RVA: 0x0005B3EC File Offset: 0x000595EC
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.codeLock = Facepunch.Pool.Get<ProtoBuf.CodeLock>();
		info.msg.codeLock.hasGuestCode = (this.guestCode.Length > 0);
		info.msg.codeLock.hasCode = (this.code.Length > 0);
		if (!info.forDisk && info.forConnection != null)
		{
			info.msg.codeLock.hasAuth = (this.whitelistPlayers.Contains(info.forConnection.userid) || this.guestPlayers.Contains(info.forConnection.userid));
		}
		if (info.forDisk)
		{
			info.msg.codeLock.pv = Facepunch.Pool.Get<ProtoBuf.CodeLock.Private>();
			info.msg.codeLock.pv.code = this.code;
			info.msg.codeLock.pv.users = Facepunch.Pool.Get<List<ulong>>();
			info.msg.codeLock.pv.users.AddRange(this.whitelistPlayers);
			info.msg.codeLock.pv.guestCode = this.guestCode;
			info.msg.codeLock.pv.guestUsers = Facepunch.Pool.Get<List<ulong>>();
			info.msg.codeLock.pv.guestUsers.AddRange(this.guestPlayers);
		}
	}

	// Token: 0x060009B6 RID: 2486 RVA: 0x0005B568 File Offset: 0x00059768
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RPC_ChangeCode(global::BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		string text = rpc.read.String(256);
		bool flag = rpc.read.Bit();
		if (base.IsLocked())
		{
			return;
		}
		if (text.Length != 4)
		{
			return;
		}
		if (!text.IsNumeric())
		{
			return;
		}
		if (!this.hasCode && flag)
		{
			return;
		}
		if (!this.hasCode && !flag)
		{
			base.SetFlag(global::BaseEntity.Flags.Locked, true, false, true);
		}
		Analytics.Azure.OnCodelockChanged(rpc.player, this, flag ? this.guestCode : this.code, text, flag);
		if (!flag)
		{
			this.code = text;
			this.hasCode = (this.code.Length > 0);
			this.whitelistPlayers.Clear();
			this.whitelistPlayers.Add(rpc.player.userID);
		}
		else
		{
			this.guestCode = text;
			this.hasGuestCode = (this.guestCode.Length > 0);
			this.guestPlayers.Clear();
			this.guestPlayers.Add(rpc.player.userID);
		}
		this.DoEffect(this.effectCodeChanged.resourcePath);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060009B7 RID: 2487 RVA: 0x0005B694 File Offset: 0x00059894
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void TryUnlock(global::BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		if (!base.IsLocked())
		{
			return;
		}
		if (this.IsCodeEntryBlocked())
		{
			return;
		}
		if (!this.whitelistPlayers.Contains(rpc.player.userID))
		{
			return;
		}
		this.DoEffect(this.effectUnlocked.resourcePath);
		base.SetFlag(global::BaseEntity.Flags.Locked, false, false, true);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060009B8 RID: 2488 RVA: 0x0005B700 File Offset: 0x00059900
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void TryLock(global::BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		if (base.IsLocked())
		{
			return;
		}
		if (this.code.Length != 4)
		{
			return;
		}
		if (!this.whitelistPlayers.Contains(rpc.player.userID))
		{
			return;
		}
		this.DoEffect(this.effectLocked.resourcePath);
		base.SetFlag(global::BaseEntity.Flags.Locked, true, false, true);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060009B9 RID: 2489 RVA: 0x0005B76F File Offset: 0x0005996F
	public void ClearCodeEntryBlocked()
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved11, false, false, true);
		this.wrongCodes = 0;
	}

	// Token: 0x060009BA RID: 2490 RVA: 0x0005B788 File Offset: 0x00059988
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void UnlockWithCode(global::BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		if (!base.IsLocked())
		{
			return;
		}
		if (this.IsCodeEntryBlocked())
		{
			return;
		}
		string a = rpc.read.String(256);
		bool flag = a == this.guestCode;
		bool flag2 = a == this.code;
		if (!(a == this.code) && (!this.hasGuestCode || !(a == this.guestCode)))
		{
			if (UnityEngine.Time.realtimeSinceStartup > this.lastWrongTime + 60f)
			{
				this.wrongCodes = 0;
			}
			this.DoEffect(this.effectDenied.resourcePath);
			this.DoEffect(this.effectShock.resourcePath);
			rpc.player.Hurt((float)(this.wrongCodes + 1) * 5f, DamageType.ElectricShock, this, false);
			this.wrongCodes++;
			if (this.wrongCodes > 5)
			{
				rpc.player.ShowToast(GameTip.Styles.Red_Normal, global::CodeLock.blockwarning, Array.Empty<string>());
			}
			if ((float)this.wrongCodes >= global::CodeLock.maxFailedAttempts)
			{
				base.SetFlag(global::BaseEntity.Flags.Reserved11, true, false, true);
				base.Invoke(new Action(this.ClearCodeEntryBlocked), global::CodeLock.lockoutCooldown);
			}
			this.lastWrongTime = UnityEngine.Time.realtimeSinceStartup;
			return;
		}
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		if (flag2)
		{
			if (!this.whitelistPlayers.Contains(rpc.player.userID))
			{
				this.DoEffect(this.effectCodeChanged.resourcePath);
				this.whitelistPlayers.Add(rpc.player.userID);
				this.wrongCodes = 0;
			}
			Analytics.Azure.OnCodeLockEntered(rpc.player, this, false);
			return;
		}
		if (flag && !this.guestPlayers.Contains(rpc.player.userID))
		{
			this.DoEffect(this.effectCodeChanged.resourcePath);
			this.guestPlayers.Add(rpc.player.userID);
			Analytics.Azure.OnCodeLockEntered(rpc.player, this, true);
		}
	}

	// Token: 0x060009BB RID: 2491 RVA: 0x0005B980 File Offset: 0x00059B80
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		base.SetFlag(global::BaseEntity.Flags.Reserved11, false, false, true);
	}
}
