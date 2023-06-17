using System;

// Token: 0x02000498 RID: 1176
public interface IVehicleLockUser
{
	// Token: 0x06002699 RID: 9881
	bool PlayerCanDestroyLock(BasePlayer player, BaseVehicleModule viaModule);

	// Token: 0x0600269A RID: 9882
	bool PlayerHasUnlockPermission(BasePlayer player);

	// Token: 0x0600269B RID: 9883
	bool PlayerCanUseThis(BasePlayer player, ModularCarCodeLock.LockType lockType);

	// Token: 0x0600269C RID: 9884
	void RemoveLock();
}
