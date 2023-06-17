using System;
using UnityEngine;

// Token: 0x0200041D RID: 1053
public class LockedByEntCrate : LootContainer
{
	// Token: 0x04001BB3 RID: 7091
	public GameObject lockingEnt;

	// Token: 0x0600237D RID: 9085 RVA: 0x000E2CFC File Offset: 0x000E0EFC
	public void SetLockingEnt(GameObject ent)
	{
		base.CancelInvoke(new Action(this.Think));
		this.SetLocked(false);
		this.lockingEnt = ent;
		if (this.lockingEnt != null)
		{
			base.InvokeRepeating(new Action(this.Think), UnityEngine.Random.Range(0f, 1f), 1f);
			this.SetLocked(true);
		}
	}

	// Token: 0x0600237E RID: 9086 RVA: 0x000E2D64 File Offset: 0x000E0F64
	public void SetLocked(bool isLocked)
	{
		base.SetFlag(BaseEntity.Flags.OnFire, isLocked, false, true);
		base.SetFlag(BaseEntity.Flags.Locked, isLocked, false, true);
	}

	// Token: 0x0600237F RID: 9087 RVA: 0x000E2D7B File Offset: 0x000E0F7B
	public void Think()
	{
		if (this.lockingEnt == null && base.IsLocked())
		{
			this.SetLockingEnt(null);
		}
	}
}
