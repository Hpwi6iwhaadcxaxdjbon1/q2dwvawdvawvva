using System;
using UnityEngine;

// Token: 0x020004DB RID: 1243
public class NoPlayersIOReset : FacepunchBehaviour
{
	// Token: 0x04002078 RID: 8312
	[SerializeField]
	private IOEntity[] entitiesToReset;

	// Token: 0x04002079 RID: 8313
	[SerializeField]
	private float radius;

	// Token: 0x0400207A RID: 8314
	[SerializeField]
	private float timeBetweenChecks;

	// Token: 0x06002843 RID: 10307 RVA: 0x000F8EC3 File Offset: 0x000F70C3
	protected void OnEnable()
	{
		base.InvokeRandomized(new Action(this.Check), this.timeBetweenChecks, this.timeBetweenChecks, this.timeBetweenChecks * 0.1f);
	}

	// Token: 0x06002844 RID: 10308 RVA: 0x000F8EEF File Offset: 0x000F70EF
	protected void OnDisable()
	{
		base.CancelInvoke(new Action(this.Check));
	}

	// Token: 0x06002845 RID: 10309 RVA: 0x000F8F03 File Offset: 0x000F7103
	private void Check()
	{
		if (!PuzzleReset.AnyPlayersWithinDistance(base.transform, this.radius))
		{
			this.Reset();
		}
	}

	// Token: 0x06002846 RID: 10310 RVA: 0x000F8F20 File Offset: 0x000F7120
	private void Reset()
	{
		foreach (IOEntity ioentity in this.entitiesToReset)
		{
			if (ioentity.IsValid() && ioentity.isServer)
			{
				ioentity.ResetIOState();
				ioentity.MarkDirty();
			}
		}
	}
}
