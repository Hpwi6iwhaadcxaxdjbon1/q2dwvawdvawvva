using System;
using UnityEngine;

// Token: 0x020008EF RID: 2287
[Serializable]
public class MinMax
{
	// Token: 0x040032AF RID: 12975
	public float x;

	// Token: 0x040032B0 RID: 12976
	public float y = 1f;

	// Token: 0x060037B9 RID: 14265 RVA: 0x0014DF42 File Offset: 0x0014C142
	public MinMax(float x, float y)
	{
		this.x = x;
		this.y = y;
	}

	// Token: 0x060037BA RID: 14266 RVA: 0x0014DF63 File Offset: 0x0014C163
	public float Random()
	{
		return UnityEngine.Random.Range(this.x, this.y);
	}

	// Token: 0x060037BB RID: 14267 RVA: 0x0014DF76 File Offset: 0x0014C176
	public float Lerp(float t)
	{
		return Mathf.Lerp(this.x, this.y, t);
	}

	// Token: 0x060037BC RID: 14268 RVA: 0x0014DF8A File Offset: 0x0014C18A
	public float Lerp(float a, float b, float t)
	{
		return Mathf.Lerp(this.x, this.y, Mathf.InverseLerp(a, b, t));
	}
}
