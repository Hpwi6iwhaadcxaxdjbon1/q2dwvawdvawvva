using System;

// Token: 0x020007A3 RID: 1955
public class NeedsMouseWheel : ListComponent<NeedsMouseWheel>
{
	// Token: 0x06003511 RID: 13585 RVA: 0x001465DD File Offset: 0x001447DD
	public static bool AnyActive()
	{
		return ListComponent<NeedsMouseWheel>.InstanceList.Count > 0;
	}
}
