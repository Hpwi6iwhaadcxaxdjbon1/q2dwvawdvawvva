using System;
using System.Collections;
using System.Collections.Generic;
using Facepunch;

namespace UnityEngine
{
	// Token: 0x02000A21 RID: 2593
	public static class CoroutineEx
	{
		// Token: 0x04003760 RID: 14176
		public static WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

		// Token: 0x04003761 RID: 14177
		public static WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

		// Token: 0x04003762 RID: 14178
		private static Dictionary<float, WaitForSeconds> waitForSecondsBuffer = new Dictionary<float, WaitForSeconds>();

		// Token: 0x06003D91 RID: 15761 RVA: 0x00169924 File Offset: 0x00167B24
		public static WaitForSeconds waitForSeconds(float seconds)
		{
			WaitForSeconds waitForSeconds;
			if (!CoroutineEx.waitForSecondsBuffer.TryGetValue(seconds, out waitForSeconds))
			{
				waitForSeconds = new WaitForSeconds(seconds);
				CoroutineEx.waitForSecondsBuffer.Add(seconds, waitForSeconds);
			}
			return waitForSeconds;
		}

		// Token: 0x06003D92 RID: 15762 RVA: 0x00169954 File Offset: 0x00167B54
		public static WaitForSecondsRealtimeEx waitForSecondsRealtime(float seconds)
		{
			WaitForSecondsRealtimeEx waitForSecondsRealtimeEx = Pool.Get<WaitForSecondsRealtimeEx>();
			waitForSecondsRealtimeEx.WaitTime = seconds;
			return waitForSecondsRealtimeEx;
		}

		// Token: 0x06003D93 RID: 15763 RVA: 0x00169962 File Offset: 0x00167B62
		public static IEnumerator Combine(params IEnumerator[] coroutines)
		{
			for (;;)
			{
				bool flag = true;
				foreach (IEnumerator enumerator in coroutines)
				{
					if (enumerator != null && enumerator.MoveNext())
					{
						flag = false;
					}
				}
				if (flag)
				{
					break;
				}
				yield return CoroutineEx.waitForEndOfFrame;
			}
			yield break;
			yield break;
		}
	}
}
