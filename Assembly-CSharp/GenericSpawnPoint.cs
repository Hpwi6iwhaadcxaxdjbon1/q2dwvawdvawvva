using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200056D RID: 1389
public class GenericSpawnPoint : BaseSpawnPoint
{
	// Token: 0x040022A3 RID: 8867
	public bool dropToGround = true;

	// Token: 0x040022A4 RID: 8868
	public bool randomRot;

	// Token: 0x040022A5 RID: 8869
	[Range(1f, 180f)]
	public float randomRotSnapDegrees = 1f;

	// Token: 0x040022A6 RID: 8870
	public GameObjectRef spawnEffect;

	// Token: 0x040022A7 RID: 8871
	public UnityEvent OnObjectSpawnedEvent = new UnityEvent();

	// Token: 0x040022A8 RID: 8872
	public UnityEvent OnObjectRetiredEvent = new UnityEvent();

	// Token: 0x06002A96 RID: 10902 RVA: 0x00103880 File Offset: 0x00101A80
	public Quaternion GetRandomRotation()
	{
		if (!this.randomRot)
		{
			return Quaternion.identity;
		}
		int max = Mathf.FloorToInt(360f / this.randomRotSnapDegrees);
		int num = UnityEngine.Random.Range(0, max);
		return Quaternion.Euler(0f, (float)num * this.randomRotSnapDegrees, 0f);
	}

	// Token: 0x06002A97 RID: 10903 RVA: 0x001038D0 File Offset: 0x00101AD0
	public override void GetLocation(out Vector3 pos, out Quaternion rot)
	{
		pos = base.transform.position;
		if (this.randomRot)
		{
			rot = base.transform.rotation * this.GetRandomRotation();
		}
		else
		{
			rot = base.transform.rotation;
		}
		if (this.dropToGround)
		{
			base.DropToGround(ref pos, ref rot);
		}
	}

	// Token: 0x06002A98 RID: 10904 RVA: 0x00103938 File Offset: 0x00101B38
	public override void ObjectSpawned(SpawnPointInstance instance)
	{
		if (this.spawnEffect.isValid)
		{
			Effect.server.Run(this.spawnEffect.resourcePath, instance.GetComponent<BaseEntity>(), 0U, Vector3.zero, Vector3.up, null, false);
		}
		this.OnObjectSpawnedEvent.Invoke();
		base.gameObject.SetActive(false);
	}

	// Token: 0x06002A99 RID: 10905 RVA: 0x0010398C File Offset: 0x00101B8C
	public override void ObjectRetired(SpawnPointInstance instance)
	{
		this.OnObjectRetiredEvent.Invoke();
		base.gameObject.SetActive(true);
	}
}
