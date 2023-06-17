using System;
using UnityEngine;

// Token: 0x020005A6 RID: 1446
public class WearableReplacementByRace : MonoBehaviour
{
	// Token: 0x0400238A RID: 9098
	public GameObjectRef[] replacements;

	// Token: 0x06002BEC RID: 11244 RVA: 0x00109E78 File Offset: 0x00108078
	public GameObjectRef GetReplacement(int meshIndex)
	{
		int num = Mathf.Clamp(meshIndex, 0, this.replacements.Length - 1);
		return this.replacements[num];
	}
}
