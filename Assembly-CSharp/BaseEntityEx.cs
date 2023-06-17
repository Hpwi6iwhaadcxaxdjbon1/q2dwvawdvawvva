using System;

// Token: 0x020003A9 RID: 937
public static class BaseEntityEx
{
	// Token: 0x060020EE RID: 8430 RVA: 0x000D88C2 File Offset: 0x000D6AC2
	public static bool IsValidEntityReference<T>(this T obj) where T : class
	{
		return obj as BaseEntity != null;
	}

	// Token: 0x060020EF RID: 8431 RVA: 0x000D88D8 File Offset: 0x000D6AD8
	public static bool HasEntityInParents(this BaseEntity ent, BaseEntity toFind)
	{
		if (ent == null || toFind == null)
		{
			return false;
		}
		if (ent == toFind || ent.EqualNetID(toFind))
		{
			return true;
		}
		BaseEntity parentEntity = ent.GetParentEntity();
		while (parentEntity != null)
		{
			if (parentEntity == toFind || parentEntity.EqualNetID(toFind))
			{
				return true;
			}
			parentEntity = parentEntity.GetParentEntity();
		}
		return false;
	}
}
