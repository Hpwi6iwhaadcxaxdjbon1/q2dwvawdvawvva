using System;
using UnityEngine;

// Token: 0x020008EC RID: 2284
public class BaseScriptableObject : ScriptableObject
{
	// Token: 0x040032AD RID: 12973
	[HideInInspector]
	public uint FilenameStringId;

	// Token: 0x060037B0 RID: 14256 RVA: 0x0014DEAC File Offset: 0x0014C0AC
	public string LookupFileName()
	{
		return StringPool.Get(this.FilenameStringId);
	}

	// Token: 0x060037B1 RID: 14257 RVA: 0x0014DEB9 File Offset: 0x0014C0B9
	public static bool operator ==(BaseScriptableObject a, BaseScriptableObject b)
	{
		return a == b || (a != null && b != null && a.FilenameStringId == b.FilenameStringId);
	}

	// Token: 0x060037B2 RID: 14258 RVA: 0x0014DED7 File Offset: 0x0014C0D7
	public static bool operator !=(BaseScriptableObject a, BaseScriptableObject b)
	{
		return !(a == b);
	}

	// Token: 0x060037B3 RID: 14259 RVA: 0x0014DEE3 File Offset: 0x0014C0E3
	public override int GetHashCode()
	{
		return (int)this.FilenameStringId;
	}

	// Token: 0x060037B4 RID: 14260 RVA: 0x0014DEEB File Offset: 0x0014C0EB
	public override bool Equals(object o)
	{
		return o != null && o is BaseScriptableObject && o as BaseScriptableObject == this;
	}
}
