using System;
using System.Collections.Generic;
using Facepunch;
using Rust;
using UnityEngine;

// Token: 0x02000499 RID: 1177
public class ModularCarCodeLock
{
	// Token: 0x04001EE8 RID: 7912
	private readonly bool isServer;

	// Token: 0x04001EE9 RID: 7913
	private readonly ModularCar owner;

	// Token: 0x04001EEA RID: 7914
	public const BaseEntity.Flags FLAG_CENTRAL_LOCKING = BaseEntity.Flags.Reserved2;

	// Token: 0x04001EEB RID: 7915
	public const BaseEntity.Flags FLAG_CODE_ENTRY_BLOCKED = BaseEntity.Flags.Reserved10;

	// Token: 0x04001EEC RID: 7916
	public const float LOCK_DESTROY_HEALTH = 0.2f;

	// Token: 0x04001EEF RID: 7919
	private int wrongCodes;

	// Token: 0x04001EF0 RID: 7920
	private float lastWrongTime = float.NegativeInfinity;

	// Token: 0x17000331 RID: 817
	// (get) Token: 0x0600269D RID: 9885 RVA: 0x000F2267 File Offset: 0x000F0467
	public bool HasALock
	{
		get
		{
			return this.isServer && !string.IsNullOrEmpty(this.Code);
		}
	}

	// Token: 0x17000332 RID: 818
	// (get) Token: 0x0600269E RID: 9886 RVA: 0x000F2281 File Offset: 0x000F0481
	public bool CentralLockingIsOn
	{
		get
		{
			return this.owner != null && this.owner.HasFlag(BaseEntity.Flags.Reserved2);
		}
	}

	// Token: 0x17000333 RID: 819
	// (get) Token: 0x0600269F RID: 9887 RVA: 0x000F22A3 File Offset: 0x000F04A3
	// (set) Token: 0x060026A0 RID: 9888 RVA: 0x000F22AB File Offset: 0x000F04AB
	public List<ulong> WhitelistPlayers { get; private set; } = new List<ulong>();

	// Token: 0x060026A1 RID: 9889 RVA: 0x000F22B4 File Offset: 0x000F04B4
	public ModularCarCodeLock(ModularCar owner, bool isServer)
	{
		this.owner = owner;
		this.isServer = isServer;
		if (isServer)
		{
			this.CheckEnableCentralLocking();
		}
	}

	// Token: 0x060026A2 RID: 9890 RVA: 0x000F22F4 File Offset: 0x000F04F4
	public bool PlayerCanDestroyLock(BaseVehicleModule viaModule)
	{
		return this.HasALock && viaModule.healthFraction <= 0.2f;
	}

	// Token: 0x060026A3 RID: 9891 RVA: 0x000F2310 File Offset: 0x000F0510
	public bool CodeEntryBlocked(BasePlayer player)
	{
		return !this.HasLockPermission(player) && this.owner != null && this.owner.HasFlag(BaseEntity.Flags.Reserved10);
	}

	// Token: 0x060026A4 RID: 9892 RVA: 0x000F2340 File Offset: 0x000F0540
	public void Load(BaseNetworkable.LoadInfo info)
	{
		this.Code = info.msg.modularCar.lockCode;
		if (this.Code == null)
		{
			this.Code = "";
		}
		this.WhitelistPlayers.Clear();
		this.WhitelistPlayers.AddRange(info.msg.modularCar.whitelistUsers);
	}

	// Token: 0x060026A5 RID: 9893 RVA: 0x000F239C File Offset: 0x000F059C
	public bool HasLockPermission(BasePlayer player)
	{
		return !this.HasALock || (player.IsValid() && !player.IsDead() && this.WhitelistPlayers.Contains(player.userID));
	}

	// Token: 0x060026A6 RID: 9894 RVA: 0x000F23CB File Offset: 0x000F05CB
	public bool PlayerCanUseThis(BasePlayer player, ModularCarCodeLock.LockType lockType)
	{
		return (lockType == ModularCarCodeLock.LockType.Door && !this.CentralLockingIsOn) || this.HasLockPermission(player);
	}

	// Token: 0x17000334 RID: 820
	// (get) Token: 0x060026A7 RID: 9895 RVA: 0x000F23E1 File Offset: 0x000F05E1
	// (set) Token: 0x060026A8 RID: 9896 RVA: 0x000F23E9 File Offset: 0x000F05E9
	public string Code { get; private set; } = "";

	// Token: 0x060026A9 RID: 9897 RVA: 0x000F23F2 File Offset: 0x000F05F2
	public void PostServerLoad()
	{
		this.owner.SetFlag(BaseEntity.Flags.Reserved10, false, false, true);
		this.CheckEnableCentralLocking();
	}

	// Token: 0x060026AA RID: 9898 RVA: 0x000F240D File Offset: 0x000F060D
	public bool CanHaveALock()
	{
		return !this.owner.IsDead() && this.owner.HasDriverMountPoints();
	}

	// Token: 0x060026AB RID: 9899 RVA: 0x000F2429 File Offset: 0x000F0629
	public bool TryAddALock(string code, ulong userID)
	{
		if (!this.isServer)
		{
			return false;
		}
		if (this.owner.IsDead())
		{
			return false;
		}
		this.TrySetNewCode(code, userID);
		return this.HasALock;
	}

	// Token: 0x060026AC RID: 9900 RVA: 0x000F2453 File Offset: 0x000F0653
	public bool IsValidLockCode(string code)
	{
		return code != null && code.Length == 4 && code.IsNumeric();
	}

	// Token: 0x060026AD RID: 9901 RVA: 0x000F2469 File Offset: 0x000F0669
	public bool TrySetNewCode(string newCode, ulong userID)
	{
		if (!this.IsValidLockCode(newCode))
		{
			return false;
		}
		this.Code = newCode;
		this.WhitelistPlayers.Clear();
		this.WhitelistPlayers.Add(userID);
		this.owner.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		return true;
	}

	// Token: 0x060026AE RID: 9902 RVA: 0x000F24A1 File Offset: 0x000F06A1
	public void RemoveLock()
	{
		if (!this.isServer)
		{
			return;
		}
		if (!this.HasALock)
		{
			return;
		}
		this.Code = "";
		this.owner.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060026AF RID: 9903 RVA: 0x000F24CC File Offset: 0x000F06CC
	public bool TryOpenWithCode(BasePlayer player, string codeEntered)
	{
		if (this.CodeEntryBlocked(player))
		{
			return false;
		}
		if (!(codeEntered == this.Code))
		{
			if (Time.realtimeSinceStartup > this.lastWrongTime + 60f)
			{
				this.wrongCodes = 0;
			}
			player.Hurt((float)(this.wrongCodes + 1) * 5f, DamageType.ElectricShock, this.owner, false);
			this.wrongCodes++;
			if (this.wrongCodes > 5)
			{
				player.ShowToast(GameTip.Styles.Red_Normal, CodeLock.blockwarning, Array.Empty<string>());
			}
			if ((float)this.wrongCodes >= CodeLock.maxFailedAttempts)
			{
				this.owner.SetFlag(BaseEntity.Flags.Reserved10, true, false, true);
				this.owner.Invoke(new Action(this.ClearCodeEntryBlocked), CodeLock.lockoutCooldown);
			}
			this.lastWrongTime = Time.realtimeSinceStartup;
			return false;
		}
		if (!this.WhitelistPlayers.Contains(player.userID))
		{
			this.WhitelistPlayers.Add(player.userID);
			this.wrongCodes = 0;
		}
		this.owner.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		return true;
	}

	// Token: 0x060026B0 RID: 9904 RVA: 0x000F25D7 File Offset: 0x000F07D7
	private void ClearCodeEntryBlocked()
	{
		this.owner.SetFlag(BaseEntity.Flags.Reserved10, false, false, true);
		this.wrongCodes = 0;
	}

	// Token: 0x060026B1 RID: 9905 RVA: 0x000F25F4 File Offset: 0x000F07F4
	public void CheckEnableCentralLocking()
	{
		if (this.CentralLockingIsOn)
		{
			return;
		}
		bool flag = false;
		using (List<BaseVehicleModule>.Enumerator enumerator = this.owner.AttachedModuleEntities.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				VehicleModuleSeating vehicleModuleSeating;
				if ((vehicleModuleSeating = (enumerator.Current as VehicleModuleSeating)) != null && vehicleModuleSeating.HasADriverSeat() && vehicleModuleSeating.AnyMounted())
				{
					flag = true;
					break;
				}
			}
		}
		if (!flag)
		{
			this.owner.SetFlag(BaseEntity.Flags.Reserved2, true, false, true);
		}
	}

	// Token: 0x060026B2 RID: 9906 RVA: 0x000F2684 File Offset: 0x000F0884
	public void ToggleCentralLocking()
	{
		this.owner.SetFlag(BaseEntity.Flags.Reserved2, !this.CentralLockingIsOn, false, true);
	}

	// Token: 0x060026B3 RID: 9907 RVA: 0x000F26A4 File Offset: 0x000F08A4
	public void Save(BaseNetworkable.SaveInfo info)
	{
		info.msg.modularCar.hasLock = this.HasALock;
		if (info.forDisk)
		{
			info.msg.modularCar.lockCode = this.Code;
		}
		info.msg.modularCar.whitelistUsers = Pool.Get<List<ulong>>();
		info.msg.modularCar.whitelistUsers.AddRange(this.WhitelistPlayers);
	}

	// Token: 0x02000D05 RID: 3333
	public enum LockType
	{
		// Token: 0x040045E8 RID: 17896
		Door,
		// Token: 0x040045E9 RID: 17897
		General
	}
}
