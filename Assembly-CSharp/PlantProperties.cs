using System;
using UnityEngine;

// Token: 0x0200043F RID: 1087
[CreateAssetMenu(menuName = "Rust/Plant Properties")]
public class PlantProperties : ScriptableObject
{
	// Token: 0x04001C83 RID: 7299
	public Translate.Phrase Description;

	// Token: 0x04001C84 RID: 7300
	public GrowableGeneProperties Genes;

	// Token: 0x04001C85 RID: 7301
	[ArrayIndexIsEnum(enumType = typeof(PlantProperties.State))]
	public PlantProperties.Stage[] stages = new PlantProperties.Stage[8];

	// Token: 0x04001C86 RID: 7302
	[Header("Metabolism")]
	public AnimationCurve timeOfDayHappiness = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(12f, 1f),
		new Keyframe(24f, 0f)
	});

	// Token: 0x04001C87 RID: 7303
	public AnimationCurve temperatureHappiness = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(-10f, -1f),
		new Keyframe(1f, 0f),
		new Keyframe(30f, 1f),
		new Keyframe(50f, 0f),
		new Keyframe(80f, -1f)
	});

	// Token: 0x04001C88 RID: 7304
	public AnimationCurve temperatureWaterRequirementMultiplier = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(-10f, 1f),
		new Keyframe(0f, 1f),
		new Keyframe(30f, 1f),
		new Keyframe(50f, 1f),
		new Keyframe(80f, 1f)
	});

	// Token: 0x04001C89 RID: 7305
	public AnimationCurve fruitVisualScaleCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(0.75f, 1f),
		new Keyframe(1f, 0f)
	});

	// Token: 0x04001C8A RID: 7306
	public int MaxSeasons = 1;

	// Token: 0x04001C8B RID: 7307
	public float WaterIntake = 20f;

	// Token: 0x04001C8C RID: 7308
	public float OptimalLightQuality = 1f;

	// Token: 0x04001C8D RID: 7309
	public float OptimalWaterQuality = 1f;

	// Token: 0x04001C8E RID: 7310
	public float OptimalGroundQuality = 1f;

	// Token: 0x04001C8F RID: 7311
	public float OptimalTemperatureQuality = 1f;

	// Token: 0x04001C90 RID: 7312
	[Header("Harvesting")]
	public BaseEntity.Menu.Option pickOption;

	// Token: 0x04001C91 RID: 7313
	public BaseEntity.Menu.Option pickAllOption;

	// Token: 0x04001C92 RID: 7314
	public BaseEntity.Menu.Option eatOption;

	// Token: 0x04001C93 RID: 7315
	public ItemDefinition pickupItem;

	// Token: 0x04001C94 RID: 7316
	public BaseEntity.Menu.Option cloneOption;

	// Token: 0x04001C95 RID: 7317
	public BaseEntity.Menu.Option cloneAllOption;

	// Token: 0x04001C96 RID: 7318
	public BaseEntity.Menu.Option removeDyingOption;

	// Token: 0x04001C97 RID: 7319
	public BaseEntity.Menu.Option removeDyingAllOption;

	// Token: 0x04001C98 RID: 7320
	public ItemDefinition removeDyingItem;

	// Token: 0x04001C99 RID: 7321
	public GameObjectRef removeDyingEffect;

	// Token: 0x04001C9A RID: 7322
	public int pickupMultiplier = 1;

	// Token: 0x04001C9B RID: 7323
	public GameObjectRef pickEffect;

	// Token: 0x04001C9C RID: 7324
	public int maxHarvests = 1;

	// Token: 0x04001C9D RID: 7325
	public bool disappearAfterHarvest;

	// Token: 0x04001C9E RID: 7326
	[Header("Seeds")]
	public GameObjectRef CrossBreedEffect;

	// Token: 0x04001C9F RID: 7327
	public ItemDefinition SeedItem;

	// Token: 0x04001CA0 RID: 7328
	public ItemDefinition CloneItem;

	// Token: 0x04001CA1 RID: 7329
	public int BaseCloneCount = 1;

	// Token: 0x04001CA2 RID: 7330
	[Header("Market")]
	public int BaseMarketValue = 10;

	// Token: 0x02000CE4 RID: 3300
	public enum State
	{
		// Token: 0x04004554 RID: 17748
		Seed,
		// Token: 0x04004555 RID: 17749
		Seedling,
		// Token: 0x04004556 RID: 17750
		Sapling,
		// Token: 0x04004557 RID: 17751
		Crossbreed,
		// Token: 0x04004558 RID: 17752
		Mature,
		// Token: 0x04004559 RID: 17753
		Fruiting,
		// Token: 0x0400455A RID: 17754
		Ripe,
		// Token: 0x0400455B RID: 17755
		Dying
	}

	// Token: 0x02000CE5 RID: 3301
	[Serializable]
	public struct Stage
	{
		// Token: 0x0400455C RID: 17756
		public PlantProperties.State nextState;

		// Token: 0x0400455D RID: 17757
		public float lifeLength;

		// Token: 0x0400455E RID: 17758
		public float health;

		// Token: 0x0400455F RID: 17759
		public float resources;

		// Token: 0x04004560 RID: 17760
		public float yield;

		// Token: 0x04004561 RID: 17761
		public GameObjectRef skinObject;

		// Token: 0x04004562 RID: 17762
		public bool IgnoreConditions;

		// Token: 0x170006A4 RID: 1700
		// (get) Token: 0x06004FCF RID: 20431 RVA: 0x001A7180 File Offset: 0x001A5380
		public float lifeLengthSeconds
		{
			get
			{
				return this.lifeLength * 60f;
			}
		}
	}
}
