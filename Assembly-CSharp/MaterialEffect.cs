using System;
using UnityEngine;

// Token: 0x02000609 RID: 1545
[CreateAssetMenu(menuName = "Rust/MaterialEffect")]
public class MaterialEffect : ScriptableObject
{
	// Token: 0x04002559 RID: 9561
	public GameObjectRef DefaultEffect;

	// Token: 0x0400255A RID: 9562
	public SoundDefinition DefaultSoundDefinition;

	// Token: 0x0400255B RID: 9563
	public MaterialEffect.Entry[] Entries;

	// Token: 0x0400255C RID: 9564
	public int waterFootstepIndex = -1;

	// Token: 0x0400255D RID: 9565
	public MaterialEffect.Entry deepWaterEntry;

	// Token: 0x0400255E RID: 9566
	public float deepWaterDepth = -1f;

	// Token: 0x0400255F RID: 9567
	public MaterialEffect.Entry submergedWaterEntry;

	// Token: 0x04002560 RID: 9568
	public float submergedWaterDepth = -1f;

	// Token: 0x04002561 RID: 9569
	public bool ScaleVolumeWithSpeed;

	// Token: 0x04002562 RID: 9570
	public AnimationCurve SpeedGainCurve;

	// Token: 0x06002DBE RID: 11710 RVA: 0x00113018 File Offset: 0x00111218
	public MaterialEffect.Entry GetEntryFromMaterial(PhysicMaterial mat)
	{
		foreach (MaterialEffect.Entry entry in this.Entries)
		{
			if (entry.Material == mat)
			{
				return entry;
			}
		}
		return null;
	}

	// Token: 0x06002DBF RID: 11711 RVA: 0x00113050 File Offset: 0x00111250
	public MaterialEffect.Entry GetWaterEntry()
	{
		if (this.waterFootstepIndex == -1)
		{
			for (int i = 0; i < this.Entries.Length; i++)
			{
				if (this.Entries[i].Material.name == "Water")
				{
					this.waterFootstepIndex = i;
					break;
				}
			}
		}
		if (this.waterFootstepIndex != -1)
		{
			return this.Entries[this.waterFootstepIndex];
		}
		Debug.LogWarning("Unable to find water effect for :" + base.name);
		return null;
	}

	// Token: 0x06002DC0 RID: 11712 RVA: 0x001130D0 File Offset: 0x001112D0
	public void SpawnOnRay(Ray ray, int mask, float length = 0.5f, Vector3 forward = default(Vector3), float speed = 0f)
	{
		RaycastHit raycastHit;
		if (!GamePhysics.Trace(ray, 0f, out raycastHit, length, mask, QueryTriggerInteraction.UseGlobal, null))
		{
			Effect.client.Run(this.DefaultEffect.resourcePath, ray.origin, ray.direction * -1f, forward, Effect.Type.Generic);
			if (this.DefaultSoundDefinition != null)
			{
				this.PlaySound(this.DefaultSoundDefinition, raycastHit.point, speed);
			}
			return;
		}
		WaterLevel.WaterInfo waterInfo = WaterLevel.GetWaterInfo(ray.origin, true, null, false);
		if (!waterInfo.isValid)
		{
			PhysicMaterial materialAt = raycastHit.collider.GetMaterialAt(raycastHit.point);
			MaterialEffect.Entry entryFromMaterial = this.GetEntryFromMaterial(materialAt);
			if (entryFromMaterial == null)
			{
				Effect.client.Run(this.DefaultEffect.resourcePath, raycastHit.point, raycastHit.normal, forward, Effect.Type.Generic);
				if (this.DefaultSoundDefinition != null)
				{
					this.PlaySound(this.DefaultSoundDefinition, raycastHit.point, speed);
					return;
				}
			}
			else
			{
				Effect.client.Run(entryFromMaterial.Effect.resourcePath, raycastHit.point, raycastHit.normal, forward, Effect.Type.Generic);
				if (entryFromMaterial.SoundDefinition != null)
				{
					this.PlaySound(entryFromMaterial.SoundDefinition, raycastHit.point, speed);
				}
			}
			return;
		}
		Vector3 vector = new Vector3(ray.origin.x, WaterSystem.GetHeight(ray.origin), ray.origin.z);
		MaterialEffect.Entry waterEntry = this.GetWaterEntry();
		if (this.submergedWaterDepth > 0f && waterInfo.currentDepth >= this.submergedWaterDepth)
		{
			waterEntry = this.submergedWaterEntry;
		}
		else if (this.deepWaterDepth > 0f && waterInfo.currentDepth >= this.deepWaterDepth)
		{
			waterEntry = this.deepWaterEntry;
		}
		if (waterEntry == null)
		{
			return;
		}
		Effect.client.Run(waterEntry.Effect.resourcePath, vector, Vector3.up, default(Vector3), Effect.Type.Generic);
		if (waterEntry.SoundDefinition != null)
		{
			this.PlaySound(waterEntry.SoundDefinition, vector, speed);
		}
	}

	// Token: 0x06002DC1 RID: 11713 RVA: 0x000063A5 File Offset: 0x000045A5
	public void PlaySound(SoundDefinition definition, Vector3 position, float velocity = 0f)
	{
	}

	// Token: 0x02000D87 RID: 3463
	[Serializable]
	public class Entry
	{
		// Token: 0x040047CB RID: 18379
		public PhysicMaterial Material;

		// Token: 0x040047CC RID: 18380
		public GameObjectRef Effect;

		// Token: 0x040047CD RID: 18381
		public SoundDefinition SoundDefinition;
	}
}
