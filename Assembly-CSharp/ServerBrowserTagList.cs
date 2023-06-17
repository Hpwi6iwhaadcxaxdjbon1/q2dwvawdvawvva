using System;
using UnityEngine;

// Token: 0x02000893 RID: 2195
public class ServerBrowserTagList : MonoBehaviour
{
	// Token: 0x0400313C RID: 12604
	public int maxTagsToShow = 3;

	// Token: 0x0400313D RID: 12605
	private ServerBrowserTagGroup[] _groups;

	// Token: 0x060036DC RID: 14044 RVA: 0x0014AF74 File Offset: 0x00149174
	private void Initialize()
	{
		if (this._groups == null)
		{
			this._groups = base.GetComponentsInChildren<ServerBrowserTagGroup>(true);
		}
	}

	// Token: 0x060036DD RID: 14045 RVA: 0x0014AF8B File Offset: 0x0014918B
	public void Awake()
	{
		this.Initialize();
	}

	// Token: 0x060036DE RID: 14046 RVA: 0x0014AF94 File Offset: 0x00149194
	public bool Refresh(in ServerInfo server)
	{
		this.Initialize();
		int num = 0;
		ServerBrowserTagGroup[] groups = this._groups;
		for (int i = 0; i < groups.Length; i++)
		{
			groups[i].Refresh(server, ref num, this.maxTagsToShow);
		}
		return num > 0;
	}
}
