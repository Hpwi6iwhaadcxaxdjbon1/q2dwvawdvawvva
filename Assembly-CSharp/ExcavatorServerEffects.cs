using System;
using UnityEngine;

// Token: 0x02000437 RID: 1079
public class ExcavatorServerEffects : MonoBehaviour
{
	// Token: 0x04001C5F RID: 7263
	public static ExcavatorServerEffects instance;

	// Token: 0x04001C60 RID: 7264
	public TriggerBase[] miningTriggers;

	// Token: 0x06002452 RID: 9298 RVA: 0x000E712F File Offset: 0x000E532F
	public void Awake()
	{
		ExcavatorServerEffects.instance = this;
		ExcavatorServerEffects.SetMining(false, true);
	}

	// Token: 0x06002453 RID: 9299 RVA: 0x000E713E File Offset: 0x000E533E
	public void OnDestroy()
	{
		ExcavatorServerEffects.instance = null;
	}

	// Token: 0x06002454 RID: 9300 RVA: 0x000E7148 File Offset: 0x000E5348
	public static void SetMining(bool isMining, bool force = false)
	{
		if (ExcavatorServerEffects.instance == null)
		{
			return;
		}
		foreach (TriggerBase triggerBase in ExcavatorServerEffects.instance.miningTriggers)
		{
			if (!(triggerBase == null))
			{
				triggerBase.gameObject.SetActive(isMining);
			}
		}
	}
}
