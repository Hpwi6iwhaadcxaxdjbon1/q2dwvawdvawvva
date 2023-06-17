using System;
using System.Collections.Generic;

// Token: 0x02000114 RID: 276
public class LaserDetector : BaseDetector
{
	// Token: 0x06001641 RID: 5697 RVA: 0x000AD40C File Offset: 0x000AB60C
	public override void OnObjects()
	{
		using (HashSet<BaseEntity>.Enumerator enumerator = this.myTrigger.entityContents.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.IsVisible(base.transform.position + base.transform.forward * 0.1f, 4f))
				{
					base.OnObjects();
					break;
				}
			}
		}
	}
}
