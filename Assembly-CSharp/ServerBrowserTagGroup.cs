using System;
using Facepunch;
using UnityEngine;

// Token: 0x02000892 RID: 2194
public class ServerBrowserTagGroup : MonoBehaviour
{
	// Token: 0x0400313A RID: 12602
	[Tooltip("If set then queries will filter out servers matching unselected tags in the group")]
	public bool isExclusive;

	// Token: 0x0400313B RID: 12603
	[NonSerialized]
	public ServerBrowserTag[] tags;

	// Token: 0x060036D7 RID: 14039 RVA: 0x0014AEAD File Offset: 0x001490AD
	private void Initialize()
	{
		if (this.tags == null)
		{
			this.tags = base.GetComponentsInChildren<ServerBrowserTag>(true);
		}
	}

	// Token: 0x060036D8 RID: 14040 RVA: 0x0014AEC4 File Offset: 0x001490C4
	public void Awake()
	{
		this.Initialize();
	}

	// Token: 0x060036D9 RID: 14041 RVA: 0x0014AECC File Offset: 0x001490CC
	public bool AnyActive()
	{
		ServerBrowserTag[] array = this.tags;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].IsActive)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060036DA RID: 14042 RVA: 0x0014AEFC File Offset: 0x001490FC
	public void Refresh(in ServerInfo server, ref int tagsEnabled, int maxTags)
	{
		this.Initialize();
		bool flag = false;
		foreach (ServerBrowserTag serverBrowserTag in this.tags)
		{
			if ((!this.isExclusive || !flag) && tagsEnabled <= maxTags && server.Tags.Contains(serverBrowserTag.serverTag))
			{
				serverBrowserTag.SetActive(true);
				tagsEnabled++;
				flag = true;
			}
			else
			{
				serverBrowserTag.SetActive(false);
			}
		}
		base.gameObject.SetActive(flag);
	}
}
