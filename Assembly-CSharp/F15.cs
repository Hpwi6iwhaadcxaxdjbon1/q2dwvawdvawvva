using System;
using UnityEngine;

// Token: 0x02000412 RID: 1042
public class F15 : BaseCombatEntity
{
	// Token: 0x04001B3E RID: 6974
	public float speed = 150f;

	// Token: 0x04001B3F RID: 6975
	public float defaultAltitude = 150f;

	// Token: 0x04001B40 RID: 6976
	public float altitude = 250f;

	// Token: 0x04001B41 RID: 6977
	public float altitudeLerpSpeed = 30f;

	// Token: 0x04001B42 RID: 6978
	public float turnRate = 1f;

	// Token: 0x04001B43 RID: 6979
	public float flybySoundLengthUntilMax = 4.5f;

	// Token: 0x04001B44 RID: 6980
	public SoundPlayer flybySound;

	// Token: 0x04001B45 RID: 6981
	public GameObject body;

	// Token: 0x04001B46 RID: 6982
	public float rollSpeed = 1f;

	// Token: 0x04001B47 RID: 6983
	protected Vector3 movePosition;

	// Token: 0x04001B48 RID: 6984
	public GameObjectRef missilePrefab;

	// Token: 0x04001B49 RID: 6985
	private float nextMissileTime;

	// Token: 0x04001B4A RID: 6986
	public float blockTurningFor;

	// Token: 0x04001B4B RID: 6987
	private bool isRetiring;

	// Token: 0x04001B4C RID: 6988
	private CH47PathFinder pathFinder = new CH47PathFinder();

	// Token: 0x04001B4D RID: 6989
	private float turnSeconds;

	// Token: 0x170002F2 RID: 754
	// (get) Token: 0x06002334 RID: 9012 RVA: 0x000348EE File Offset: 0x00032AEE
	protected override float PositionTickRate
	{
		get
		{
			return 0.05f;
		}
	}

	// Token: 0x170002F3 RID: 755
	// (get) Token: 0x06002335 RID: 9013 RVA: 0x0000441C File Offset: 0x0000261C
	protected override bool PositionTickFixedTime
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002336 RID: 9014 RVA: 0x0000627E File Offset: 0x0000447E
	public override float GetNetworkTime()
	{
		return Time.fixedTime;
	}

	// Token: 0x06002337 RID: 9015 RVA: 0x000E0F1C File Offset: 0x000DF11C
	public float GetDesiredAltitude()
	{
		Vector3 vector = base.transform.position + base.transform.forward * 200f;
		return (TerrainMeta.HeightMap.GetHeight(base.transform.position) + TerrainMeta.HeightMap.GetHeight(vector) + TerrainMeta.HeightMap.GetHeight(vector + Vector3.right * 50f) + TerrainMeta.HeightMap.GetHeight(vector - Vector3.right * 50f) + TerrainMeta.HeightMap.GetHeight(vector + Vector3.forward * 50f) + TerrainMeta.HeightMap.GetHeight(vector - Vector3.forward * 50f)) / 6f + this.defaultAltitude;
	}

	// Token: 0x06002338 RID: 9016 RVA: 0x000E1000 File Offset: 0x000DF200
	public override void ServerInit()
	{
		base.ServerInit();
		base.Invoke(new Action(this.RetireToSunset), 600f);
		this.movePosition = base.transform.position;
		this.movePosition.y = this.defaultAltitude;
		base.transform.position = this.movePosition;
	}

	// Token: 0x06002339 RID: 9017 RVA: 0x000E105D File Offset: 0x000DF25D
	public void RetireToSunset()
	{
		this.isRetiring = true;
		this.movePosition = new Vector3(10000f, this.defaultAltitude, 10000f);
	}

	// Token: 0x0600233A RID: 9018 RVA: 0x000E1084 File Offset: 0x000DF284
	public void PickNewPatrolPoint()
	{
		this.movePosition = this.pathFinder.GetRandomPatrolPoint();
		float num = 0f;
		if (TerrainMeta.HeightMap != null)
		{
			num = TerrainMeta.HeightMap.GetHeight(this.movePosition);
		}
		this.movePosition.y = num + this.defaultAltitude;
	}

	// Token: 0x0600233B RID: 9019 RVA: 0x000E10DC File Offset: 0x000DF2DC
	private void FixedUpdate()
	{
		if (base.isClient)
		{
			return;
		}
		if (this.isRetiring && Vector3.Distance(base.transform.position, Vector3.zero) > 4900f)
		{
			base.Invoke(new Action(this.DelayedDestroy), 0f);
		}
		if (base.IsInvoking(new Action(this.DelayedDestroy)))
		{
			return;
		}
		this.altitude = Mathf.Lerp(this.altitude, this.GetDesiredAltitude(), Time.fixedDeltaTime * 0.25f);
		if (Vector3Ex.Distance2D(this.movePosition, base.transform.position) < 10f)
		{
			this.PickNewPatrolPoint();
			this.blockTurningFor = 6f;
		}
		this.blockTurningFor -= Time.fixedDeltaTime;
		bool flag = this.blockTurningFor > 0f;
		this.movePosition.y = this.altitude;
		Vector3 vector = Vector3Ex.Direction(this.movePosition, base.transform.position);
		if (flag)
		{
			Vector3 position = base.transform.position;
			position.y = this.altitude;
			Vector3 a = QuaternionEx.LookRotationForcedUp(base.transform.forward, Vector3.up) * Vector3.forward;
			vector = Vector3Ex.Direction(position + a * 2000f, base.transform.position);
		}
		Vector3 forward = Vector3.Lerp(base.transform.forward, vector, Time.fixedDeltaTime * this.turnRate);
		base.transform.forward = forward;
		bool flag2 = Vector3.Dot(base.transform.right, vector) > 0.55f;
		bool flag3 = Vector3.Dot(-base.transform.right, vector) > 0.55f;
		base.SetFlag(BaseEntity.Flags.Reserved1, flag2, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved2, flag3, false, true);
		if (flag3 || flag2)
		{
			this.turnSeconds += Time.fixedDeltaTime;
		}
		else
		{
			this.turnSeconds = 0f;
		}
		if (this.turnSeconds > 10f)
		{
			this.turnSeconds = 0f;
			this.blockTurningFor = 8f;
		}
		base.transform.position += base.transform.forward * this.speed * Time.fixedDeltaTime;
		this.nextMissileTime = Time.realtimeSinceStartup + 10f;
	}

	// Token: 0x0600233C RID: 9020 RVA: 0x00003384 File Offset: 0x00001584
	public void DelayedDestroy()
	{
		base.Kill(BaseNetworkable.DestroyMode.None);
	}
}
