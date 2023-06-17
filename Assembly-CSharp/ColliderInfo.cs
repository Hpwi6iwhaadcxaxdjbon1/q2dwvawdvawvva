using System;
using UnityEngine;

// Token: 0x020004EC RID: 1260
public class ColliderInfo : MonoBehaviour
{
	// Token: 0x040020D6 RID: 8406
	public const ColliderInfo.Flags FlagsNone = (ColliderInfo.Flags)0;

	// Token: 0x040020D7 RID: 8407
	public const ColliderInfo.Flags FlagsEverything = (ColliderInfo.Flags)(-1);

	// Token: 0x040020D8 RID: 8408
	public const ColliderInfo.Flags FlagsDefault = ColliderInfo.Flags.Usable | ColliderInfo.Flags.Shootable | ColliderInfo.Flags.Melee | ColliderInfo.Flags.Opaque;

	// Token: 0x040020D9 RID: 8409
	[InspectorFlags]
	public ColliderInfo.Flags flags = ColliderInfo.Flags.Usable | ColliderInfo.Flags.Shootable | ColliderInfo.Flags.Melee | ColliderInfo.Flags.Opaque;

	// Token: 0x060028D7 RID: 10455 RVA: 0x000FB92A File Offset: 0x000F9B2A
	public bool HasFlag(ColliderInfo.Flags f)
	{
		return (this.flags & f) == f;
	}

	// Token: 0x060028D8 RID: 10456 RVA: 0x000FB937 File Offset: 0x000F9B37
	public void SetFlag(ColliderInfo.Flags f, bool b)
	{
		if (b)
		{
			this.flags |= f;
			return;
		}
		this.flags &= ~f;
	}

	// Token: 0x060028D9 RID: 10457 RVA: 0x000FB95C File Offset: 0x000F9B5C
	public bool Filter(HitTest info)
	{
		switch (info.type)
		{
		case HitTest.Type.ProjectileEffect:
		case HitTest.Type.Projectile:
			if ((this.flags & ColliderInfo.Flags.Shootable) == (ColliderInfo.Flags)0)
			{
				return false;
			}
			break;
		case HitTest.Type.MeleeAttack:
			if ((this.flags & ColliderInfo.Flags.Melee) == (ColliderInfo.Flags)0)
			{
				return false;
			}
			break;
		case HitTest.Type.Use:
			if ((this.flags & ColliderInfo.Flags.Usable) == (ColliderInfo.Flags)0)
			{
				return false;
			}
			break;
		}
		return true;
	}

	// Token: 0x02000D2C RID: 3372
	[Flags]
	public enum Flags
	{
		// Token: 0x0400466A RID: 18026
		Usable = 1,
		// Token: 0x0400466B RID: 18027
		Shootable = 2,
		// Token: 0x0400466C RID: 18028
		Melee = 4,
		// Token: 0x0400466D RID: 18029
		Opaque = 8,
		// Token: 0x0400466E RID: 18030
		Airflow = 16,
		// Token: 0x0400466F RID: 18031
		OnlyBlockBuildingBlock = 32
	}
}
