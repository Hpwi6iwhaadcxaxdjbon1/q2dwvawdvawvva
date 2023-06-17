using System;
using UnityEngine;

// Token: 0x02000935 RID: 2357
public static class GameObjectUtil
{
	// Token: 0x06003899 RID: 14489 RVA: 0x00151920 File Offset: 0x0014FB20
	public static void GlobalBroadcast(string messageName, object param = null)
	{
		Transform[] rootObjects = TransformUtil.GetRootObjects();
		for (int i = 0; i < rootObjects.Length; i++)
		{
			rootObjects[i].BroadcastMessage(messageName, param, SendMessageOptions.DontRequireReceiver);
		}
	}
}
