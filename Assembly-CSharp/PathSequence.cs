using System;
using System.Collections.Generic;

// Token: 0x02000680 RID: 1664
public class PathSequence : PrefabAttribute
{
	// Token: 0x06002FB3 RID: 12211 RVA: 0x0011EB40 File Offset: 0x0011CD40
	protected override Type GetIndexedType()
	{
		return typeof(PathSequence);
	}

	// Token: 0x06002FB4 RID: 12212 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void ApplySequenceReplacement(List<Prefab> sequence, ref Prefab replacement, Prefab[] possibleReplacements, int pathLength, int pathIndex)
	{
	}
}
