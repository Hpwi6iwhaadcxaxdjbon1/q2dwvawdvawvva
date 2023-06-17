using System;
using UnityEngine;

// Token: 0x020006E3 RID: 1763
public abstract class ProceduralObject : MonoBehaviour
{
	// Token: 0x060031F5 RID: 12789 RVA: 0x001340C8 File Offset: 0x001322C8
	protected void Awake()
	{
		if (!(SingletonComponent<WorldSetup>.Instance == null))
		{
			if (SingletonComponent<WorldSetup>.Instance.ProceduralObjects == null)
			{
				Debug.LogError("WorldSetup.Instance.ProceduralObjects is null.", this);
				return;
			}
			SingletonComponent<WorldSetup>.Instance.ProceduralObjects.Add(this);
		}
	}

	// Token: 0x060031F6 RID: 12790
	public abstract void Process();
}
