using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000861 RID: 2145
public class LifeInfographicStat : MonoBehaviour
{
	// Token: 0x04003030 RID: 12336
	public LifeInfographicStat.DataType dataSource;

	// Token: 0x04003031 RID: 12337
	[Header("Generic Stats")]
	public string genericStatKey;

	// Token: 0x04003032 RID: 12338
	[Header("Weapon Info")]
	public string targetWeaponName;

	// Token: 0x04003033 RID: 12339
	public LifeInfographicStat.WeaponInfoType weaponInfoType;

	// Token: 0x04003034 RID: 12340
	public TextMeshProUGUI targetText;

	// Token: 0x04003035 RID: 12341
	public Image StatImage;

	// Token: 0x02000E8C RID: 3724
	public enum DataType
	{
		// Token: 0x04004BFE RID: 19454
		None,
		// Token: 0x04004BFF RID: 19455
		AliveTime_Short,
		// Token: 0x04004C00 RID: 19456
		SleepingTime_Short,
		// Token: 0x04004C01 RID: 19457
		KillerName,
		// Token: 0x04004C02 RID: 19458
		KillerWeapon,
		// Token: 0x04004C03 RID: 19459
		AliveTime_Long,
		// Token: 0x04004C04 RID: 19460
		KillerDistance,
		// Token: 0x04004C05 RID: 19461
		GenericStat,
		// Token: 0x04004C06 RID: 19462
		DistanceTravelledWalk,
		// Token: 0x04004C07 RID: 19463
		DistanceTravelledRun,
		// Token: 0x04004C08 RID: 19464
		DamageTaken,
		// Token: 0x04004C09 RID: 19465
		DamageHealed,
		// Token: 0x04004C0A RID: 19466
		WeaponInfo,
		// Token: 0x04004C0B RID: 19467
		SecondsWilderness,
		// Token: 0x04004C0C RID: 19468
		SecondsSwimming,
		// Token: 0x04004C0D RID: 19469
		SecondsInBase,
		// Token: 0x04004C0E RID: 19470
		SecondsInMonument,
		// Token: 0x04004C0F RID: 19471
		SecondsFlying,
		// Token: 0x04004C10 RID: 19472
		SecondsBoating,
		// Token: 0x04004C11 RID: 19473
		PlayersKilled,
		// Token: 0x04004C12 RID: 19474
		ScientistsKilled,
		// Token: 0x04004C13 RID: 19475
		AnimalsKilled,
		// Token: 0x04004C14 RID: 19476
		SecondsDriving
	}

	// Token: 0x02000E8D RID: 3725
	public enum WeaponInfoType
	{
		// Token: 0x04004C16 RID: 19478
		TotalShots,
		// Token: 0x04004C17 RID: 19479
		ShotsHit,
		// Token: 0x04004C18 RID: 19480
		ShotsMissed,
		// Token: 0x04004C19 RID: 19481
		AccuracyPercentage
	}
}
