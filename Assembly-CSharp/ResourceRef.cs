using System;
using UnityEngine;

// Token: 0x02000965 RID: 2405
[Serializable]
public class ResourceRef<T> where T : UnityEngine.Object
{
	// Token: 0x040033C6 RID: 13254
	public string guid;

	// Token: 0x040033C7 RID: 13255
	private T _cachedObject;

	// Token: 0x1700049A RID: 1178
	// (get) Token: 0x060039BF RID: 14783 RVA: 0x00156C91 File Offset: 0x00154E91
	public bool isValid
	{
		get
		{
			return !string.IsNullOrEmpty(this.guid);
		}
	}

	// Token: 0x060039C0 RID: 14784 RVA: 0x00156CA1 File Offset: 0x00154EA1
	public T Get()
	{
		if (this._cachedObject == null)
		{
			this._cachedObject = (GameManifest.GUIDToObject(this.guid) as T);
		}
		return this._cachedObject;
	}

	// Token: 0x1700049B RID: 1179
	// (get) Token: 0x060039C1 RID: 14785 RVA: 0x00156CD7 File Offset: 0x00154ED7
	public string resourcePath
	{
		get
		{
			return GameManifest.GUIDToPath(this.guid);
		}
	}

	// Token: 0x1700049C RID: 1180
	// (get) Token: 0x060039C2 RID: 14786 RVA: 0x00156CE4 File Offset: 0x00154EE4
	public uint resourceID
	{
		get
		{
			return StringPool.Get(this.resourcePath);
		}
	}
}
