using System;
using UnityEngine;

// Token: 0x02000573 RID: 1395
public class RadialSpawnPoint : BaseSpawnPoint
{
	// Token: 0x040022C1 RID: 8897
	public float radius = 10f;

	// Token: 0x06002AB9 RID: 10937 RVA: 0x00103F94 File Offset: 0x00102194
	public override void GetLocation(out Vector3 pos, out Quaternion rot)
	{
		Vector2 vector = UnityEngine.Random.insideUnitCircle * this.radius;
		pos = base.transform.position + new Vector3(vector.x, 0f, vector.y);
		rot = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
		base.DropToGround(ref pos, ref rot);
	}

	// Token: 0x06002ABA RID: 10938 RVA: 0x0010400A File Offset: 0x0010220A
	public override bool HasPlayersIntersecting()
	{
		return BaseNetworkable.HasCloseConnections(base.transform.position, this.radius + 1f);
	}

	// Token: 0x06002ABB RID: 10939 RVA: 0x000063A5 File Offset: 0x000045A5
	public override void ObjectSpawned(SpawnPointInstance instance)
	{
	}

	// Token: 0x06002ABC RID: 10940 RVA: 0x000063A5 File Offset: 0x000045A5
	public override void ObjectRetired(SpawnPointInstance instance)
	{
	}
}
