using System;
using UnityEngine;

// Token: 0x02000678 RID: 1656
public class RandomStaticObject : MonoBehaviour
{
	// Token: 0x04002716 RID: 10006
	public uint Seed;

	// Token: 0x04002717 RID: 10007
	public float Probability = 0.5f;

	// Token: 0x04002718 RID: 10008
	public GameObject[] Candidates;

	// Token: 0x06002FA2 RID: 12194 RVA: 0x0011E61C File Offset: 0x0011C81C
	protected void Start()
	{
		uint seed = base.transform.position.Seed(World.Seed + this.Seed);
		if (SeedRandom.Value(ref seed) > this.Probability)
		{
			for (int i = 0; i < this.Candidates.Length; i++)
			{
				GameManager.Destroy(this.Candidates[i], 0f);
			}
			GameManager.Destroy(this, 0f);
			return;
		}
		int num = SeedRandom.Range(seed, 0, base.transform.childCount);
		for (int j = 0; j < this.Candidates.Length; j++)
		{
			GameObject gameObject = this.Candidates[j];
			if (j == num)
			{
				gameObject.SetActive(true);
			}
			else
			{
				GameManager.Destroy(gameObject, 0f);
			}
		}
		GameManager.Destroy(this, 0f);
	}
}
