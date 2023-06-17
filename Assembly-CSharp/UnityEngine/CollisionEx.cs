using System;

namespace UnityEngine
{
	// Token: 0x02000A1F RID: 2591
	public static class CollisionEx
	{
		// Token: 0x06003D8E RID: 15758 RVA: 0x001698EB File Offset: 0x00167AEB
		public static BaseEntity GetEntity(this Collision col)
		{
			return col.transform.ToBaseEntity();
		}
	}
}
