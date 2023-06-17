using System;
using Facepunch.Rust;
using Network;
using UnityEngine;

// Token: 0x020000A7 RID: 167
public class OreResourceEntity : StagedResourceEntity
{
	// Token: 0x04000A1E RID: 2590
	public GameObjectRef bonusPrefab;

	// Token: 0x04000A1F RID: 2591
	public GameObjectRef finishEffect;

	// Token: 0x04000A20 RID: 2592
	public GameObjectRef bonusFailEffect;

	// Token: 0x04000A21 RID: 2593
	public OreHotSpot _hotSpot;

	// Token: 0x04000A22 RID: 2594
	public SoundPlayer bonusSound;

	// Token: 0x04000A23 RID: 2595
	private int bonusesKilled;

	// Token: 0x04000A24 RID: 2596
	private int bonusesSpawned;

	// Token: 0x04000A25 RID: 2597
	private Vector3 lastNodeDir = Vector3.zero;

	// Token: 0x06000F58 RID: 3928 RVA: 0x000812E8 File Offset: 0x0007F4E8
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("OreResourceEntity.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000F59 RID: 3929 RVA: 0x00081328 File Offset: 0x0007F528
	protected override void UpdateNetworkStage()
	{
		int stage = this.stage;
		base.UpdateNetworkStage();
		if (this.stage != stage && this._hotSpot)
		{
			this.DelayedBonusSpawn();
		}
	}

	// Token: 0x06000F5A RID: 3930 RVA: 0x0008135E File Offset: 0x0007F55E
	public void CleanupBonus()
	{
		if (this._hotSpot)
		{
			this._hotSpot.Kill(BaseNetworkable.DestroyMode.None);
		}
		this._hotSpot = null;
	}

	// Token: 0x06000F5B RID: 3931 RVA: 0x00081380 File Offset: 0x0007F580
	public override void DestroyShared()
	{
		base.DestroyShared();
		this.CleanupBonus();
	}

	// Token: 0x06000F5C RID: 3932 RVA: 0x0008138E File Offset: 0x0007F58E
	public override void OnKilled(HitInfo info)
	{
		this.CleanupBonus();
		Analytics.Server.OreKilled(this, info);
		base.OnKilled(info);
	}

	// Token: 0x06000F5D RID: 3933 RVA: 0x000813A4 File Offset: 0x0007F5A4
	public override void ServerInit()
	{
		base.ServerInit();
		base.Invoke(new Action(this.InitialSpawnBonusSpot), 0f);
	}

	// Token: 0x06000F5E RID: 3934 RVA: 0x000813C3 File Offset: 0x0007F5C3
	private void InitialSpawnBonusSpot()
	{
		if (base.IsDestroyed)
		{
			return;
		}
		this._hotSpot = this.SpawnBonusSpot(Vector3.zero);
	}

	// Token: 0x06000F5F RID: 3935 RVA: 0x000813DF File Offset: 0x0007F5DF
	public void FinishBonusAssigned()
	{
		Effect.server.Run(this.finishEffect.resourcePath, base.transform.position, base.transform.up, null, false);
	}

	// Token: 0x06000F60 RID: 3936 RVA: 0x0008140C File Offset: 0x0007F60C
	public override void OnAttacked(HitInfo info)
	{
		if (base.isClient)
		{
			base.OnAttacked(info);
			return;
		}
		if (!info.DidGather && info.gatherScale > 0f)
		{
			Jackhammer jackhammer = info.Weapon as Jackhammer;
			if (this._hotSpot || jackhammer)
			{
				if (this._hotSpot == null)
				{
					this._hotSpot = this.SpawnBonusSpot(this.lastNodeDir);
				}
				if (Vector3.Distance(info.HitPositionWorld, this._hotSpot.transform.position) <= this._hotSpot.GetComponent<SphereCollider>().radius * 1.5f || jackhammer != null)
				{
					float num = (jackhammer == null) ? 1f : jackhammer.HotspotBonusScale;
					this.bonusesKilled++;
					info.gatherScale = 1f + Mathf.Clamp((float)this.bonusesKilled * 0.5f, 0f, 2f * num);
					this._hotSpot.FireFinishEffect();
					base.ClientRPC<int, Vector3>(null, "PlayBonusLevelSound", this.bonusesKilled, this._hotSpot.transform.position);
				}
				else if (this.bonusesKilled > 0)
				{
					this.bonusesKilled = 0;
					Effect.server.Run(this.bonusFailEffect.resourcePath, base.transform.position, base.transform.up, null, false);
				}
				if (this.bonusesKilled > 0)
				{
					this.CleanupBonus();
				}
			}
		}
		if (this._hotSpot == null)
		{
			this.DelayedBonusSpawn();
		}
		base.OnAttacked(info);
	}

	// Token: 0x06000F61 RID: 3937 RVA: 0x000815A4 File Offset: 0x0007F7A4
	public void DelayedBonusSpawn()
	{
		base.CancelInvoke(new Action(this.RespawnBonus));
		base.Invoke(new Action(this.RespawnBonus), 0.25f);
	}

	// Token: 0x06000F62 RID: 3938 RVA: 0x000815CF File Offset: 0x0007F7CF
	public void RespawnBonus()
	{
		this.CleanupBonus();
		this._hotSpot = this.SpawnBonusSpot(this.lastNodeDir);
	}

	// Token: 0x06000F63 RID: 3939 RVA: 0x000815EC File Offset: 0x0007F7EC
	public OreHotSpot SpawnBonusSpot(Vector3 lastDirection)
	{
		if (base.isClient)
		{
			return null;
		}
		if (!this.bonusPrefab.isValid)
		{
			return null;
		}
		Vector2 normalized = UnityEngine.Random.insideUnitCircle.normalized;
		Vector3 vector = Vector3.zero;
		MeshCollider stageComponent = base.GetStageComponent<MeshCollider>();
		Vector3 vector2 = base.transform.InverseTransformPoint(stageComponent.bounds.center);
		if (lastDirection == Vector3.zero)
		{
			Vector3 vector3 = this.RandomCircle(1f, false);
			this.lastNodeDir = vector3.normalized;
			Vector3 vector4 = base.transform.TransformDirection(vector3.normalized);
			vector3 = base.transform.position + base.transform.up * (vector2.y + 0.5f) + vector4.normalized * 2.5f;
			vector = vector3;
		}
		else
		{
			Vector3 a = Vector3.Cross(this.lastNodeDir, Vector3.up);
			float d = UnityEngine.Random.Range(0.25f, 0.5f);
			float d2 = (UnityEngine.Random.Range(0, 2) == 0) ? -1f : 1f;
			Vector3 normalized2 = (this.lastNodeDir + a * d * d2).normalized;
			this.lastNodeDir = normalized2;
			vector = base.transform.position + base.transform.TransformDirection(normalized2) * 2f;
			float num = UnityEngine.Random.Range(1f, 1.5f);
			vector += base.transform.up * (vector2.y + num);
		}
		this.bonusesSpawned++;
		Vector3 normalized3 = (stageComponent.bounds.center - vector).normalized;
		RaycastHit raycastHit;
		if (stageComponent.Raycast(new Ray(vector, normalized3), out raycastHit, 10f))
		{
			OreHotSpot oreHotSpot = GameManager.server.CreateEntity(this.bonusPrefab.resourcePath, raycastHit.point - normalized3 * 0.025f, Quaternion.LookRotation(raycastHit.normal, Vector3.up), true) as OreHotSpot;
			oreHotSpot.Spawn();
			oreHotSpot.SendMessage("OreOwner", this);
			return oreHotSpot;
		}
		return null;
	}

	// Token: 0x06000F64 RID: 3940 RVA: 0x0008182C File Offset: 0x0007FA2C
	public Vector3 RandomCircle(float distance = 1f, bool allowInside = false)
	{
		Vector2 vector = allowInside ? UnityEngine.Random.insideUnitCircle : UnityEngine.Random.insideUnitCircle.normalized;
		return new Vector3(vector.x, 0f, vector.y);
	}

	// Token: 0x06000F65 RID: 3941 RVA: 0x00081868 File Offset: 0x0007FA68
	public Vector3 RandomHemisphereDirection(Vector3 input, float degreesOffset, bool allowInside = true, bool changeHeight = true)
	{
		degreesOffset = Mathf.Clamp(degreesOffset / 180f, -180f, 180f);
		Vector2 vector = allowInside ? UnityEngine.Random.insideUnitCircle : UnityEngine.Random.insideUnitCircle.normalized;
		Vector3 b = new Vector3(vector.x * degreesOffset, changeHeight ? (UnityEngine.Random.Range(-1f, 1f) * degreesOffset) : 0f, vector.y * degreesOffset);
		return (input + b).normalized;
	}

	// Token: 0x06000F66 RID: 3942 RVA: 0x000818E8 File Offset: 0x0007FAE8
	public Vector3 ClampToHemisphere(Vector3 hemiInput, float degreesOffset, Vector3 inputVec)
	{
		degreesOffset = Mathf.Clamp(degreesOffset / 180f, -180f, 180f);
		Vector3 normalized = (hemiInput + Vector3.one * degreesOffset).normalized;
		Vector3 normalized2 = (hemiInput + Vector3.one * -degreesOffset).normalized;
		for (int i = 0; i < 3; i++)
		{
			inputVec[i] = Mathf.Clamp(inputVec[i], normalized2[i], normalized[i]);
		}
		return inputVec;
	}

	// Token: 0x06000F67 RID: 3943 RVA: 0x00081974 File Offset: 0x0007FB74
	public static Vector3 RandomCylinderPointAroundVector(Vector3 input, float distance, float minHeight = 0f, float maxHeight = 0f, bool allowInside = false)
	{
		Vector2 vector = allowInside ? UnityEngine.Random.insideUnitCircle : UnityEngine.Random.insideUnitCircle.normalized;
		Vector3 result = new Vector3(vector.x, 0f, vector.y).normalized * distance;
		result.y = UnityEngine.Random.Range(minHeight, maxHeight);
		return result;
	}

	// Token: 0x06000F68 RID: 3944 RVA: 0x0002BE49 File Offset: 0x0002A049
	public Vector3 ClampToCylinder(Vector3 localPos, Vector3 cylinderAxis, float cylinderDistance, float minHeight = 0f, float maxHeight = 0f)
	{
		return Vector3.zero;
	}
}
