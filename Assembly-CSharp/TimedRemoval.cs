using System;
using UnityEngine;

// Token: 0x020002EA RID: 746
public class TimedRemoval : MonoBehaviour
{
	// Token: 0x0400175C RID: 5980
	public UnityEngine.Object objectToDestroy;

	// Token: 0x0400175D RID: 5981
	public float removeDelay = 1f;

	// Token: 0x06001DFF RID: 7679 RVA: 0x000CCBA1 File Offset: 0x000CADA1
	private void OnEnable()
	{
		UnityEngine.Object.Destroy(this.objectToDestroy, this.removeDelay);
	}
}
