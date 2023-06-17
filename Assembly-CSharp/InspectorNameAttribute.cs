using System;
using UnityEngine;

// Token: 0x020008EE RID: 2286
public class InspectorNameAttribute : PropertyAttribute
{
	// Token: 0x040032AE RID: 12974
	public string name;

	// Token: 0x060037B8 RID: 14264 RVA: 0x0014DF33 File Offset: 0x0014C133
	public InspectorNameAttribute(string name)
	{
		this.name = name;
	}
}
