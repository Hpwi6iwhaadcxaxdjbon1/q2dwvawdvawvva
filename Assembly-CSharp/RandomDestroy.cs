using System;
using UnityEngine;

// Token: 0x02000675 RID: 1653
public class RandomDestroy : MonoBehaviour
{
	// Token: 0x0400270C RID: 9996
	public uint Seed;

	// Token: 0x0400270D RID: 9997
	public float Probability = 0.5f;

	// Token: 0x06002F9E RID: 12190 RVA: 0x0011E56C File Offset: 0x0011C76C
	protected void Start()
	{
		uint num = base.transform.position.Seed(World.Seed + this.Seed);
		if (SeedRandom.Value(ref num) > this.Probability)
		{
			GameManager.Destroy(this, 0f);
			return;
		}
		GameManager.Destroy(base.gameObject, 0f);
	}
}
