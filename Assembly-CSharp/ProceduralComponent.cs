using System;
using UnityEngine;

// Token: 0x020006B1 RID: 1713
public abstract class ProceduralComponent : MonoBehaviour
{
	// Token: 0x040027E1 RID: 10209
	[InspectorFlags]
	public ProceduralComponent.Realm Mode = (ProceduralComponent.Realm)(-1);

	// Token: 0x040027E2 RID: 10210
	public string Description = "Procedural Component";

	// Token: 0x17000409 RID: 1033
	// (get) Token: 0x0600315A RID: 12634 RVA: 0x00007A3C File Offset: 0x00005C3C
	public virtual bool RunOnCache
	{
		get
		{
			return false;
		}
	}

	// Token: 0x0600315B RID: 12635 RVA: 0x001274AA File Offset: 0x001256AA
	public bool ShouldRun()
	{
		return (!World.Cached || this.RunOnCache) && (this.Mode & ProceduralComponent.Realm.Server) != (ProceduralComponent.Realm)0;
	}

	// Token: 0x0600315C RID: 12636
	public abstract void Process(uint seed);

	// Token: 0x02000DD9 RID: 3545
	public enum Realm
	{
		// Token: 0x04004943 RID: 18755
		Client = 1,
		// Token: 0x04004944 RID: 18756
		Server
	}
}
