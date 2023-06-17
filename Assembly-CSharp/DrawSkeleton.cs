using System;
using UnityEngine;

// Token: 0x02000306 RID: 774
public class DrawSkeleton : MonoBehaviour
{
	// Token: 0x06001E8B RID: 7819 RVA: 0x000D0674 File Offset: 0x000CE874
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		DrawSkeleton.DrawTransform(base.transform);
	}

	// Token: 0x06001E8C RID: 7820 RVA: 0x000D068C File Offset: 0x000CE88C
	private static void DrawTransform(Transform t)
	{
		for (int i = 0; i < t.childCount; i++)
		{
			Gizmos.DrawLine(t.position, t.GetChild(i).position);
			DrawSkeleton.DrawTransform(t.GetChild(i));
		}
	}
}
