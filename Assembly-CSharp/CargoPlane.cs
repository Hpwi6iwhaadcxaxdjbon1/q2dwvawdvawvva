using System;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x020004E1 RID: 1249
public class CargoPlane : global::BaseEntity
{
	// Token: 0x04002099 RID: 8345
	public GameObjectRef prefabDrop;

	// Token: 0x0400209A RID: 8346
	public SpawnFilter filter;

	// Token: 0x0400209B RID: 8347
	private Vector3 startPos;

	// Token: 0x0400209C RID: 8348
	private Vector3 endPos;

	// Token: 0x0400209D RID: 8349
	private float secondsToTake;

	// Token: 0x0400209E RID: 8350
	private float secondsTaken;

	// Token: 0x0400209F RID: 8351
	private bool dropped;

	// Token: 0x040020A0 RID: 8352
	private Vector3 dropPosition = Vector3.zero;

	// Token: 0x06002867 RID: 10343 RVA: 0x000F9853 File Offset: 0x000F7A53
	public override void ServerInit()
	{
		base.ServerInit();
		this.Initialize();
	}

	// Token: 0x06002868 RID: 10344 RVA: 0x000F9861 File Offset: 0x000F7A61
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (this.dropPosition == Vector3.zero)
		{
			this.Initialize();
		}
	}

	// Token: 0x06002869 RID: 10345 RVA: 0x000F9881 File Offset: 0x000F7A81
	private void Initialize()
	{
		if (this.dropPosition == Vector3.zero)
		{
			this.dropPosition = this.RandomDropPosition();
		}
		this.UpdateDropPosition(this.dropPosition);
	}

	// Token: 0x0600286A RID: 10346 RVA: 0x000F98AD File Offset: 0x000F7AAD
	public void InitDropPosition(Vector3 newDropPosition)
	{
		this.dropPosition = newDropPosition;
		this.dropPosition.y = 0f;
	}

	// Token: 0x0600286B RID: 10347 RVA: 0x000F98C8 File Offset: 0x000F7AC8
	public Vector3 RandomDropPosition()
	{
		Vector3 vector = Vector3.zero;
		float num = 100f;
		float x = TerrainMeta.Size.x;
		do
		{
			vector = Vector3Ex.Range(-(x / 3f), x / 3f);
		}
		while (this.filter.GetFactor(vector, true) == 0f && (num -= 1f) > 0f);
		vector.y = 0f;
		return vector;
	}

	// Token: 0x0600286C RID: 10348 RVA: 0x000F9934 File Offset: 0x000F7B34
	public void UpdateDropPosition(Vector3 newDropPosition)
	{
		float x = TerrainMeta.Size.x;
		float y = TerrainMeta.HighestPoint.y + 250f;
		this.startPos = Vector3Ex.Range(-1f, 1f);
		this.startPos.y = 0f;
		this.startPos.Normalize();
		this.startPos *= x * 2f;
		this.startPos.y = y;
		this.endPos = this.startPos * -1f;
		this.endPos.y = this.startPos.y;
		this.startPos += newDropPosition;
		this.endPos += newDropPosition;
		this.secondsToTake = Vector3.Distance(this.startPos, this.endPos) / 50f;
		this.secondsToTake *= UnityEngine.Random.Range(0.95f, 1.05f);
		base.transform.position = this.startPos;
		base.transform.rotation = Quaternion.LookRotation(this.endPos - this.startPos);
		this.dropPosition = newDropPosition;
	}

	// Token: 0x0600286D RID: 10349 RVA: 0x000F9A74 File Offset: 0x000F7C74
	private void Update()
	{
		if (!base.isServer)
		{
			return;
		}
		this.secondsTaken += Time.deltaTime;
		float num = Mathf.InverseLerp(0f, this.secondsToTake, this.secondsTaken);
		if (!this.dropped && num >= 0.5f)
		{
			this.dropped = true;
			global::BaseEntity baseEntity = GameManager.server.CreateEntity(this.prefabDrop.resourcePath, base.transform.position, default(Quaternion), true);
			if (baseEntity)
			{
				baseEntity.globalBroadcast = true;
				baseEntity.Spawn();
			}
		}
		base.transform.position = Vector3.Lerp(this.startPos, this.endPos, num);
		base.transform.hasChanged = true;
		if (num >= 1f)
		{
			base.Kill(global::BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x0600286E RID: 10350 RVA: 0x000F9B44 File Offset: 0x000F7D44
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (base.isServer && info.forDisk)
		{
			info.msg.cargoPlane = Pool.Get<ProtoBuf.CargoPlane>();
			info.msg.cargoPlane.startPos = this.startPos;
			info.msg.cargoPlane.endPos = this.endPos;
			info.msg.cargoPlane.secondsToTake = this.secondsToTake;
			info.msg.cargoPlane.secondsTaken = this.secondsTaken;
			info.msg.cargoPlane.dropped = this.dropped;
			info.msg.cargoPlane.dropPosition = this.dropPosition;
		}
	}

	// Token: 0x0600286F RID: 10351 RVA: 0x000F9C04 File Offset: 0x000F7E04
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (base.isServer && info.fromDisk && info.msg.cargoPlane != null)
		{
			this.startPos = info.msg.cargoPlane.startPos;
			this.endPos = info.msg.cargoPlane.endPos;
			this.secondsToTake = info.msg.cargoPlane.secondsToTake;
			this.secondsTaken = info.msg.cargoPlane.secondsTaken;
			this.dropped = info.msg.cargoPlane.dropped;
			this.dropPosition = info.msg.cargoPlane.dropPosition;
		}
	}
}
