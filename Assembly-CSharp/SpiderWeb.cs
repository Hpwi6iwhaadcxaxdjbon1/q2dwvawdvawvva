using System;
using UnityEngine;

// Token: 0x02000165 RID: 357
public class SpiderWeb : BaseCombatEntity
{
	// Token: 0x0600174B RID: 5963 RVA: 0x000B143D File Offset: 0x000AF63D
	public bool Fresh()
	{
		return !base.HasFlag(BaseEntity.Flags.Reserved1) && !base.HasFlag(BaseEntity.Flags.Reserved2) && !base.HasFlag(BaseEntity.Flags.Reserved3) && !base.HasFlag(BaseEntity.Flags.Reserved4);
	}

	// Token: 0x0600174C RID: 5964 RVA: 0x000B1478 File Offset: 0x000AF678
	public override void ServerInit()
	{
		base.ServerInit();
		if (this.Fresh())
		{
			int num = UnityEngine.Random.Range(0, 4);
			BaseEntity.Flags f = BaseEntity.Flags.Reserved1;
			if (num == 0)
			{
				f = BaseEntity.Flags.Reserved1;
			}
			else if (num == 1)
			{
				f = BaseEntity.Flags.Reserved2;
			}
			else if (num == 2)
			{
				f = BaseEntity.Flags.Reserved3;
			}
			else if (num == 3)
			{
				f = BaseEntity.Flags.Reserved4;
			}
			base.SetFlag(f, true, false, true);
		}
	}
}
