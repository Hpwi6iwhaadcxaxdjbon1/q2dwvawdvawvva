using System;
using Rust;
using UnityEngine;

// Token: 0x02000601 RID: 1537
public class ItemModWearable : ItemMod
{
	// Token: 0x04002535 RID: 9525
	public GameObjectRef entityPrefab = new GameObjectRef();

	// Token: 0x04002536 RID: 9526
	public GameObjectRef entityPrefabFemale = new GameObjectRef();

	// Token: 0x04002537 RID: 9527
	public ProtectionProperties protectionProperties;

	// Token: 0x04002538 RID: 9528
	public ArmorProperties armorProperties;

	// Token: 0x04002539 RID: 9529
	public ClothingMovementProperties movementProperties;

	// Token: 0x0400253A RID: 9530
	public UIBlackoutOverlay.blackoutType occlusionType = UIBlackoutOverlay.blackoutType.NONE;

	// Token: 0x0400253B RID: 9531
	public bool blocksAiming;

	// Token: 0x0400253C RID: 9532
	public bool emissive;

	// Token: 0x0400253D RID: 9533
	public float accuracyBonus;

	// Token: 0x0400253E RID: 9534
	public bool blocksEquipping;

	// Token: 0x0400253F RID: 9535
	public float eggVision;

	// Token: 0x04002540 RID: 9536
	public float weight;

	// Token: 0x04002541 RID: 9537
	public bool equipOnRightClick = true;

	// Token: 0x04002542 RID: 9538
	public bool npcOnly;

	// Token: 0x04002543 RID: 9539
	public GameObjectRef breakEffect = new GameObjectRef();

	// Token: 0x04002544 RID: 9540
	public GameObjectRef viewmodelAddition;

	// Token: 0x170003C4 RID: 964
	// (get) Token: 0x06002D8C RID: 11660 RVA: 0x0011223F File Offset: 0x0011043F
	public Wearable targetWearable
	{
		get
		{
			if (this.entityPrefab.isValid)
			{
				return this.entityPrefab.Get().GetComponent<Wearable>();
			}
			return null;
		}
	}

	// Token: 0x06002D8D RID: 11661 RVA: 0x00112260 File Offset: 0x00110460
	private void DoPrepare()
	{
		if (!this.entityPrefab.isValid)
		{
			Debug.LogWarning("ItemModWearable: entityPrefab is null! " + base.gameObject, base.gameObject);
		}
		if (this.entityPrefab.isValid && this.targetWearable == null)
		{
			Debug.LogWarning("ItemModWearable: entityPrefab doesn't have a Wearable component! " + base.gameObject, this.entityPrefab.Get());
		}
	}

	// Token: 0x06002D8E RID: 11662 RVA: 0x001122D0 File Offset: 0x001104D0
	public override void ModInit()
	{
		if (string.IsNullOrEmpty(this.entityPrefab.resourcePath))
		{
			Debug.LogWarning(this + " - entityPrefab is null or something.. - " + this.entityPrefab.guid);
		}
	}

	// Token: 0x06002D8F RID: 11663 RVA: 0x001122FF File Offset: 0x001104FF
	public bool ProtectsArea(HitArea area)
	{
		return !(this.armorProperties == null) && this.armorProperties.Contains(area);
	}

	// Token: 0x06002D90 RID: 11664 RVA: 0x0011231D File Offset: 0x0011051D
	public bool HasProtections()
	{
		return this.protectionProperties != null;
	}

	// Token: 0x06002D91 RID: 11665 RVA: 0x0011232B File Offset: 0x0011052B
	internal float GetProtection(Item item, DamageType damageType)
	{
		if (this.protectionProperties == null)
		{
			return 0f;
		}
		return this.protectionProperties.Get(damageType) * this.ConditionProtectionScale(item);
	}

	// Token: 0x06002D92 RID: 11666 RVA: 0x00112355 File Offset: 0x00110555
	public float ConditionProtectionScale(Item item)
	{
		if (!item.isBroken)
		{
			return 1f;
		}
		return 0.25f;
	}

	// Token: 0x06002D93 RID: 11667 RVA: 0x0011236A File Offset: 0x0011056A
	public void CollectProtection(Item item, ProtectionProperties protection)
	{
		if (this.protectionProperties == null)
		{
			return;
		}
		protection.Add(this.protectionProperties, this.ConditionProtectionScale(item));
	}

	// Token: 0x06002D94 RID: 11668 RVA: 0x00112390 File Offset: 0x00110590
	private bool IsHeadgear()
	{
		Wearable component = this.entityPrefab.Get().GetComponent<Wearable>();
		return component != null && (component.occupationOver & (Wearable.OccupationSlots.HeadTop | Wearable.OccupationSlots.Face | Wearable.OccupationSlots.HeadBack)) != (Wearable.OccupationSlots)0;
	}

	// Token: 0x06002D95 RID: 11669 RVA: 0x001123C4 File Offset: 0x001105C4
	public bool IsFootwear()
	{
		Wearable component = this.entityPrefab.Get().GetComponent<Wearable>();
		return component != null && (component.occupationOver & (Wearable.OccupationSlots.LeftFoot | Wearable.OccupationSlots.RightFoot)) != (Wearable.OccupationSlots)0;
	}

	// Token: 0x06002D96 RID: 11670 RVA: 0x001123FC File Offset: 0x001105FC
	public override void OnAttacked(Item item, HitInfo info)
	{
		if (!item.hasCondition)
		{
			return;
		}
		float num = 0f;
		for (int i = 0; i < 25; i++)
		{
			DamageType damageType = (DamageType)i;
			if (info.damageTypes.Has(damageType))
			{
				num += Mathf.Clamp(info.damageTypes.types[i] * this.GetProtection(item, damageType), 0f, item.condition);
				if (num >= item.condition)
				{
					break;
				}
			}
		}
		item.LoseCondition(num);
		if (item != null && item.isBroken && item.GetOwnerPlayer() && this.IsHeadgear() && info.damageTypes.Total() >= item.GetOwnerPlayer().health)
		{
			item.Drop(item.GetOwnerPlayer().transform.position + new Vector3(0f, 1.8f, 0f), item.GetOwnerPlayer().GetInheritedDropVelocity() + Vector3.up * 3f, default(Quaternion)).SetAngularVelocity(UnityEngine.Random.rotation.eulerAngles * 5f);
		}
	}

	// Token: 0x06002D97 RID: 11671 RVA: 0x00112528 File Offset: 0x00110728
	public bool CanExistWith(ItemModWearable wearable)
	{
		if (wearable == null)
		{
			return true;
		}
		Wearable targetWearable = this.targetWearable;
		Wearable targetWearable2 = wearable.targetWearable;
		return (targetWearable.occupationOver & targetWearable2.occupationOver) == (Wearable.OccupationSlots)0 && (targetWearable.occupationUnder & targetWearable2.occupationUnder) == (Wearable.OccupationSlots)0;
	}
}
