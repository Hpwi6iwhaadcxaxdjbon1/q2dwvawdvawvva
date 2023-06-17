using System;
using System.Collections.Generic;

// Token: 0x020006E0 RID: 1760
public class ProcessMonumentNodes : ProceduralComponent
{
	// Token: 0x060031EE RID: 12782 RVA: 0x0013402C File Offset: 0x0013222C
	public override void Process(uint seed)
	{
		List<MonumentNode> monumentNodes = SingletonComponent<WorldSetup>.Instance.MonumentNodes;
		if (!World.Cached)
		{
			for (int i = 0; i < monumentNodes.Count; i++)
			{
				monumentNodes[i].Process(ref seed);
			}
		}
		monumentNodes.Clear();
	}
}
