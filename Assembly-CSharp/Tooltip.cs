using System;
using UnityEngine;

// Token: 0x020008C0 RID: 2240
public class Tooltip : BaseMonoBehaviour, IClientComponent
{
	// Token: 0x04003243 RID: 12867
	public static TooltipContainer Current;

	// Token: 0x04003244 RID: 12868
	[TextArea]
	public string Text;

	// Token: 0x04003245 RID: 12869
	public GameObject TooltipObject;

	// Token: 0x04003246 RID: 12870
	public string token = "";

	// Token: 0x17000468 RID: 1128
	// (get) Token: 0x0600374A RID: 14154 RVA: 0x0014D06C File Offset: 0x0014B26C
	public string english
	{
		get
		{
			return this.Text;
		}
	}
}
