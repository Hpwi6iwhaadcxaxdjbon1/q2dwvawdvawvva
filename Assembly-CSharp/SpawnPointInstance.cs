using System;
using Rust;
using UnityEngine;

// Token: 0x02000577 RID: 1399
public class SpawnPointInstance : MonoBehaviour
{
	// Token: 0x040022D8 RID: 8920
	internal ISpawnPointUser parentSpawnPointUser;

	// Token: 0x040022D9 RID: 8921
	internal BaseSpawnPoint parentSpawnPoint;

	// Token: 0x06002ADB RID: 10971 RVA: 0x0010475E File Offset: 0x0010295E
	public void Notify()
	{
		if (!this.parentSpawnPointUser.IsUnityNull<ISpawnPointUser>())
		{
			this.parentSpawnPointUser.ObjectSpawned(this);
		}
		if (this.parentSpawnPoint)
		{
			this.parentSpawnPoint.ObjectSpawned(this);
		}
	}

	// Token: 0x06002ADC RID: 10972 RVA: 0x00104792 File Offset: 0x00102992
	public void Retire()
	{
		if (!this.parentSpawnPointUser.IsUnityNull<ISpawnPointUser>())
		{
			this.parentSpawnPointUser.ObjectRetired(this);
		}
		if (this.parentSpawnPoint)
		{
			this.parentSpawnPoint.ObjectRetired(this);
		}
	}

	// Token: 0x06002ADD RID: 10973 RVA: 0x001047C6 File Offset: 0x001029C6
	protected void OnDestroy()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		this.Retire();
	}
}
