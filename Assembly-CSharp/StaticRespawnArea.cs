using System;
using UnityEngine;

// Token: 0x02000424 RID: 1060
public class StaticRespawnArea : SleepingBag
{
	// Token: 0x04001C03 RID: 7171
	public Transform[] spawnAreas;

	// Token: 0x04001C04 RID: 7172
	public bool allowHostileSpawns;

	// Token: 0x060023DB RID: 9179 RVA: 0x000E52D1 File Offset: 0x000E34D1
	public override bool ValidForPlayer(ulong playerID, bool ignoreTimers)
	{
		return ignoreTimers || this.allowHostileSpawns || BasePlayer.FindByID(playerID).GetHostileDuration() <= 0f;
	}

	// Token: 0x060023DC RID: 9180 RVA: 0x000E52F8 File Offset: 0x000E34F8
	public override void GetSpawnPos(out Vector3 pos, out Quaternion rot)
	{
		Transform transform = this.spawnAreas[UnityEngine.Random.Range(0, this.spawnAreas.Length)];
		pos = transform.transform.position + this.spawnOffset;
		rot = Quaternion.Euler(0f, transform.transform.rotation.eulerAngles.y, 0f);
	}

	// Token: 0x060023DD RID: 9181 RVA: 0x000E5364 File Offset: 0x000E3564
	public override void SetUnlockTime(float newTime)
	{
		this.unlockTime = 0f;
	}

	// Token: 0x060023DE RID: 9182 RVA: 0x000E5374 File Offset: 0x000E3574
	public override float GetUnlockSeconds(ulong playerID)
	{
		BasePlayer basePlayer = BasePlayer.FindByID(playerID);
		if (basePlayer == null || this.allowHostileSpawns)
		{
			return base.unlockSeconds;
		}
		return Mathf.Max(basePlayer.GetHostileDuration(), base.unlockSeconds);
	}
}
