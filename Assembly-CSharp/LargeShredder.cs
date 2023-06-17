using System;
using Rust;
using UnityEngine;

// Token: 0x02000439 RID: 1081
public class LargeShredder : BaseEntity
{
	// Token: 0x04001C62 RID: 7266
	public Transform shredRail;

	// Token: 0x04001C63 RID: 7267
	public Transform shredRailStartPos;

	// Token: 0x04001C64 RID: 7268
	public Transform shredRailEndPos;

	// Token: 0x04001C65 RID: 7269
	public Vector3 shredRailStartRotation;

	// Token: 0x04001C66 RID: 7270
	public Vector3 shredRailEndRotation;

	// Token: 0x04001C67 RID: 7271
	public LargeShredderTrigger trigger;

	// Token: 0x04001C68 RID: 7272
	public float shredDurationRotation = 2f;

	// Token: 0x04001C69 RID: 7273
	public float shredDurationPosition = 5f;

	// Token: 0x04001C6A RID: 7274
	public float shredSwayAmount = 1f;

	// Token: 0x04001C6B RID: 7275
	public float shredSwaySpeed = 3f;

	// Token: 0x04001C6C RID: 7276
	public BaseEntity currentlyShredding;

	// Token: 0x04001C6D RID: 7277
	public GameObject[] shreddingWheels;

	// Token: 0x04001C6E RID: 7278
	public float shredRotorSpeed = 1f;

	// Token: 0x04001C6F RID: 7279
	public GameObjectRef shredSoundEffect;

	// Token: 0x04001C70 RID: 7280
	public Transform resourceSpawnPoint;

	// Token: 0x04001C71 RID: 7281
	private Quaternion entryRotation;

	// Token: 0x04001C72 RID: 7282
	public const string SHRED_STAT = "cars_shredded";

	// Token: 0x04001C73 RID: 7283
	private bool isShredding;

	// Token: 0x04001C74 RID: 7284
	private float shredStartTime;

	// Token: 0x06002458 RID: 9304 RVA: 0x000E7198 File Offset: 0x000E5398
	public virtual void OnEntityEnteredTrigger(BaseEntity ent)
	{
		if (ent.IsDestroyed)
		{
			return;
		}
		Rigidbody component = ent.GetComponent<Rigidbody>();
		if (this.isShredding || this.currentlyShredding != null)
		{
			component.velocity = -component.velocity * 3f;
			return;
		}
		this.shredRail.transform.position = this.shredRailStartPos.position;
		this.shredRail.transform.rotation = Quaternion.LookRotation(this.shredRailStartRotation);
		this.entryRotation = ent.transform.rotation;
		Quaternion rotation = ent.transform.rotation;
		component.isKinematic = true;
		this.currentlyShredding = ent;
		ent.transform.rotation = rotation;
		this.isShredding = true;
		this.SetShredding(true);
		this.shredStartTime = Time.realtimeSinceStartup;
	}

	// Token: 0x06002459 RID: 9305 RVA: 0x000E7270 File Offset: 0x000E5470
	public void CreateShredResources()
	{
		if (this.currentlyShredding == null)
		{
			return;
		}
		MagnetLiftable component = this.currentlyShredding.GetComponent<MagnetLiftable>();
		if (component == null)
		{
			return;
		}
		if (component.associatedPlayer != null && GameInfo.HasAchievements)
		{
			component.associatedPlayer.stats.Add("cars_shredded", 1, Stats.Steam);
			component.associatedPlayer.stats.Save(true);
		}
		foreach (ItemAmount itemAmount in component.shredResources)
		{
			Item item = ItemManager.Create(itemAmount.itemDef, (int)itemAmount.amount, 0UL);
			float num = 0.5f;
			if (item.CreateWorldObject(this.resourceSpawnPoint.transform.position + new Vector3(UnityEngine.Random.Range(-num, num), 1f, UnityEngine.Random.Range(-num, num)), default(Quaternion), null, 0U) == null)
			{
				item.Remove(0f);
			}
		}
		BaseModularVehicle component2 = this.currentlyShredding.GetComponent<BaseModularVehicle>();
		if (component2)
		{
			foreach (BaseVehicleModule baseVehicleModule in component2.AttachedModuleEntities)
			{
				if (baseVehicleModule.AssociatedItemDef && baseVehicleModule.AssociatedItemDef.Blueprint)
				{
					foreach (ItemAmount itemAmount2 in baseVehicleModule.AssociatedItemDef.Blueprint.ingredients)
					{
						int num2 = Mathf.FloorToInt(itemAmount2.amount * 0.5f);
						if (num2 != 0)
						{
							Item item2 = ItemManager.Create(itemAmount2.itemDef, num2, 0UL);
							float num3 = 0.5f;
							if (item2.CreateWorldObject(this.resourceSpawnPoint.transform.position + new Vector3(UnityEngine.Random.Range(-num3, num3), 1f, UnityEngine.Random.Range(-num3, num3)), default(Quaternion), null, 0U) == null)
							{
								item2.Remove(0f);
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x0600245A RID: 9306 RVA: 0x000E74F0 File Offset: 0x000E56F0
	public void UpdateBonePosition(float delta)
	{
		float t = delta / this.shredDurationPosition;
		float t2 = delta / this.shredDurationRotation;
		this.shredRail.transform.localPosition = Vector3.Lerp(this.shredRailStartPos.localPosition, this.shredRailEndPos.localPosition, t);
		this.shredRail.transform.rotation = Quaternion.LookRotation(Vector3.Lerp(this.shredRailStartRotation, this.shredRailEndRotation, t2));
	}

	// Token: 0x0600245B RID: 9307 RVA: 0x000E7562 File Offset: 0x000E5762
	public void SetShredding(bool isShredding)
	{
		if (isShredding)
		{
			base.InvokeRandomized(new Action(this.FireShredEffect), 0.25f, 0.75f, 0.25f);
			return;
		}
		base.CancelInvoke(new Action(this.FireShredEffect));
	}

	// Token: 0x0600245C RID: 9308 RVA: 0x000E759B File Offset: 0x000E579B
	public void FireShredEffect()
	{
		Effect.server.Run(this.shredSoundEffect.resourcePath, base.transform.position + Vector3.up * 3f, Vector3.up, null, false);
	}

	// Token: 0x0600245D RID: 9309 RVA: 0x000E75D4 File Offset: 0x000E57D4
	public void ServerUpdate()
	{
		if (base.isClient)
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.Reserved10, this.isShredding, false, true);
		if (this.isShredding)
		{
			float num = Time.realtimeSinceStartup - this.shredStartTime;
			float t = num / this.shredDurationPosition;
			float t2 = num / this.shredDurationRotation;
			this.shredRail.transform.localPosition = Vector3.Lerp(this.shredRailStartPos.localPosition, this.shredRailEndPos.localPosition, t);
			this.shredRail.transform.rotation = Quaternion.LookRotation(Vector3.Lerp(this.shredRailStartRotation, this.shredRailEndRotation, t2));
			MagnetLiftable component = this.currentlyShredding.GetComponent<MagnetLiftable>();
			this.currentlyShredding.transform.position = this.shredRail.transform.position;
			Vector3 vector = base.transform.TransformDirection(component.shredDirection);
			if (Vector3.Dot(-vector, this.currentlyShredding.transform.forward) > Vector3.Dot(vector, this.currentlyShredding.transform.forward))
			{
				vector = base.transform.TransformDirection(-component.shredDirection);
			}
			bool flag = Vector3.Dot(component.transform.up, Vector3.up) >= -0.95f;
			Quaternion quaternion = QuaternionEx.LookRotationForcedUp(vector, flag ? (-base.transform.right) : base.transform.right);
			float num2 = Time.time * this.shredSwaySpeed;
			float num3 = Mathf.PerlinNoise(num2, 0f);
			float num4 = Mathf.PerlinNoise(0f, num2 + 150f);
			quaternion *= Quaternion.Euler(num3 * this.shredSwayAmount, 0f, num4 * this.shredSwayAmount);
			this.currentlyShredding.transform.rotation = Quaternion.Lerp(this.entryRotation, quaternion, t2);
			if (num > 5f)
			{
				this.CreateShredResources();
				this.currentlyShredding.Kill(BaseNetworkable.DestroyMode.None);
				this.currentlyShredding = null;
				this.isShredding = false;
				this.SetShredding(false);
			}
		}
	}

	// Token: 0x0600245E RID: 9310 RVA: 0x000E77EC File Offset: 0x000E59EC
	private void Update()
	{
		this.ServerUpdate();
	}
}
