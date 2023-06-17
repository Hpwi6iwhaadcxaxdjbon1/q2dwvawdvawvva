using System;
using UnityEngine;

// Token: 0x020002C1 RID: 705
public class FollowCamera : MonoBehaviour, IClientComponent
{
	// Token: 0x06001D6E RID: 7534 RVA: 0x000CAB8C File Offset: 0x000C8D8C
	private void LateUpdate()
	{
		if (MainCamera.mainCamera == null)
		{
			return;
		}
		base.transform.position = MainCamera.position;
	}
}
