using System;

// Token: 0x020004AE RID: 1198
public class MountableParentCombatEntity : BaseCombatEntity
{
	// Token: 0x04001F99 RID: 8089
	private BaseMountable mountable;

	// Token: 0x1700034A RID: 842
	// (get) Token: 0x0600272B RID: 10027 RVA: 0x000F4E79 File Offset: 0x000F3079
	private BaseMountable Mountable
	{
		get
		{
			if (this.mountable == null)
			{
				this.mountable = base.GetComponentInParent<BaseMountable>();
			}
			return this.mountable;
		}
	}
}
