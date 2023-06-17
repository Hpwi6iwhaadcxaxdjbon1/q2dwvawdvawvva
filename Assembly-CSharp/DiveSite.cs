using System;
using UnityEngine;

// Token: 0x02000156 RID: 342
public class DiveSite : JunkPile
{
	// Token: 0x04000FD3 RID: 4051
	public Transform bobber;

	// Token: 0x0600171B RID: 5915 RVA: 0x000B02DD File Offset: 0x000AE4DD
	public override float TimeoutPlayerCheckRadius()
	{
		return 40f;
	}
}
