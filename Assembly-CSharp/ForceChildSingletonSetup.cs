using System;
using UnityEngine;

// Token: 0x020008ED RID: 2285
public class ForceChildSingletonSetup : MonoBehaviour
{
	// Token: 0x060037B6 RID: 14262 RVA: 0x0014DF08 File Offset: 0x0014C108
	[ComponentHelp("Any child objects of this object that contain SingletonComponents will be registered - even if they're not enabled")]
	private void Awake()
	{
		SingletonComponent[] componentsInChildren = base.GetComponentsInChildren<SingletonComponent>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].SingletonSetup();
		}
	}
}
