using System;
using UnityEngine;

// Token: 0x020004FE RID: 1278
public class DetachMonumentChildren : MonoBehaviour
{
	// Token: 0x0600291D RID: 10525 RVA: 0x000FCB5D File Offset: 0x000FAD5D
	private void Awake()
	{
		base.transform.DetachChildren();
	}
}
