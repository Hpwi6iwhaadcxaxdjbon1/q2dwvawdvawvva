using System;
using UnityEngine;

// Token: 0x020008E9 RID: 2281
public class VitalRadial : MonoBehaviour
{
	// Token: 0x060037A3 RID: 14243 RVA: 0x0014DB82 File Offset: 0x0014BD82
	private void Awake()
	{
		Debug.LogWarning("VitalRadial is obsolete " + base.transform.GetRecursiveName(""), base.gameObject);
	}
}
