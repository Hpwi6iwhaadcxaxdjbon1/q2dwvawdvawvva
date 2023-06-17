using System;
using UnityEngine;

// Token: 0x02000658 RID: 1624
public static class DecorComponentEx
{
	// Token: 0x06002F5D RID: 12125 RVA: 0x0011D500 File Offset: 0x0011B700
	public static void ApplyDecorComponents(this Transform transform, DecorComponent[] components, ref Vector3 pos, ref Quaternion rot, ref Vector3 scale)
	{
		foreach (DecorComponent decorComponent in components)
		{
			if (!decorComponent.isRoot)
			{
				return;
			}
			decorComponent.Apply(ref pos, ref rot, ref scale);
		}
	}

	// Token: 0x06002F5E RID: 12126 RVA: 0x0011D534 File Offset: 0x0011B734
	public static void ApplyDecorComponents(this Transform transform, DecorComponent[] components)
	{
		Vector3 position = transform.position;
		Quaternion rotation = transform.rotation;
		Vector3 localScale = transform.localScale;
		transform.ApplyDecorComponents(components, ref position, ref rotation, ref localScale);
		transform.position = position;
		transform.rotation = rotation;
		transform.localScale = localScale;
	}

	// Token: 0x06002F5F RID: 12127 RVA: 0x0011D578 File Offset: 0x0011B778
	public static void ApplyDecorComponentsScaleOnly(this Transform transform, DecorComponent[] components)
	{
		Vector3 position = transform.position;
		Quaternion rotation = transform.rotation;
		Vector3 localScale = transform.localScale;
		transform.ApplyDecorComponents(components, ref position, ref rotation, ref localScale);
		transform.localScale = localScale;
	}
}
