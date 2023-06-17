using System;
using UnityEngine.Events;

// Token: 0x020008C9 RID: 2249
public class UIEscapeCapture : ListComponent<UIEscapeCapture>
{
	// Token: 0x04003268 RID: 12904
	public UnityEvent onEscape = new UnityEvent();

	// Token: 0x0600375B RID: 14171 RVA: 0x0014D254 File Offset: 0x0014B454
	public static bool EscapePressed()
	{
		using (ListHashSet<UIEscapeCapture>.Enumerator enumerator = ListComponent<UIEscapeCapture>.InstanceList.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				enumerator.Current.onEscape.Invoke();
				return true;
			}
		}
		return false;
	}
}
