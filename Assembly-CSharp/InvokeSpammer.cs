using System;
using System.Threading;
using UnityEngine;

// Token: 0x02000307 RID: 775
public class InvokeSpammer : MonoBehaviour
{
	// Token: 0x0400179E RID: 6046
	public int InvokeMilliseconds = 1;

	// Token: 0x0400179F RID: 6047
	public float RepeatTime = 0.6f;

	// Token: 0x06001E8E RID: 7822 RVA: 0x000D06CD File Offset: 0x000CE8CD
	private void Start()
	{
		SingletonComponent<InvokeHandler>.Instance.InvokeRepeating(new Action(this.TestInvoke), this.RepeatTime, this.RepeatTime);
	}

	// Token: 0x06001E8F RID: 7823 RVA: 0x000D06F1 File Offset: 0x000CE8F1
	private void TestInvoke()
	{
		Thread.Sleep(this.InvokeMilliseconds);
	}
}
