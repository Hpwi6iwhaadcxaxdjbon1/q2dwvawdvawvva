using System;
using UnityEngine;

// Token: 0x02000972 RID: 2418
public class SwapKeycard : MonoBehaviour
{
	// Token: 0x04003400 RID: 13312
	public GameObject[] accessLevels;

	// Token: 0x060039E2 RID: 14818 RVA: 0x00157490 File Offset: 0x00155690
	public void UpdateAccessLevel(int level)
	{
		GameObject[] array = this.accessLevels;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(false);
		}
		this.accessLevels[level - 1].SetActive(true);
	}

	// Token: 0x060039E3 RID: 14819 RVA: 0x001574CC File Offset: 0x001556CC
	public void SetRootActive(int index)
	{
		for (int i = 0; i < this.accessLevels.Length; i++)
		{
			this.accessLevels[i].SetActive(i == index);
		}
	}
}
