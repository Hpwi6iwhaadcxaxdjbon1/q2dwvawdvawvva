using System;
using System.Collections.Generic;

// Token: 0x020006E1 RID: 1761
public class ProcessProceduralObjects : ProceduralComponent
{
	// Token: 0x060031F0 RID: 12784 RVA: 0x00134070 File Offset: 0x00132270
	public override void Process(uint seed)
	{
		List<ProceduralObject> proceduralObjects = SingletonComponent<WorldSetup>.Instance.ProceduralObjects;
		if (!World.Cached)
		{
			for (int i = 0; i < proceduralObjects.Count; i++)
			{
				ProceduralObject proceduralObject = proceduralObjects[i];
				if (proceduralObject)
				{
					proceduralObject.Process();
				}
			}
		}
		proceduralObjects.Clear();
	}

	// Token: 0x17000412 RID: 1042
	// (get) Token: 0x060031F1 RID: 12785 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool RunOnCache
	{
		get
		{
			return true;
		}
	}
}
