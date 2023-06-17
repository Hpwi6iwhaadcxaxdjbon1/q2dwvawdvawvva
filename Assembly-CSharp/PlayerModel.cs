using System;
using UnityEngine;

// Token: 0x02000447 RID: 1095
public class PlayerModel : ListComponent<PlayerModel>
{
	// Token: 0x04001CB4 RID: 7348
	public Transform[] Shoulders;

	// Token: 0x04001CB5 RID: 7349
	public Transform[] AdditionalSpineBones;

	// Token: 0x04001CB6 RID: 7350
	protected static int speed = Animator.StringToHash("speed");

	// Token: 0x04001CB7 RID: 7351
	protected static int acceleration = Animator.StringToHash("acceleration");

	// Token: 0x04001CB8 RID: 7352
	protected static int rotationYaw = Animator.StringToHash("rotationYaw");

	// Token: 0x04001CB9 RID: 7353
	protected static int forward = Animator.StringToHash("forward");

	// Token: 0x04001CBA RID: 7354
	protected static int right = Animator.StringToHash("right");

	// Token: 0x04001CBB RID: 7355
	protected static int up = Animator.StringToHash("up");

	// Token: 0x04001CBC RID: 7356
	protected static int ducked = Animator.StringToHash("ducked");

	// Token: 0x04001CBD RID: 7357
	protected static int grounded = Animator.StringToHash("grounded");

	// Token: 0x04001CBE RID: 7358
	protected static int crawling = Animator.StringToHash("crawling");

	// Token: 0x04001CBF RID: 7359
	protected static int waterlevel = Animator.StringToHash("waterlevel");

	// Token: 0x04001CC0 RID: 7360
	protected static int attack = Animator.StringToHash("attack");

	// Token: 0x04001CC1 RID: 7361
	protected static int attack_alt = Animator.StringToHash("attack_alt");

	// Token: 0x04001CC2 RID: 7362
	protected static int deploy = Animator.StringToHash("deploy");

	// Token: 0x04001CC3 RID: 7363
	protected static int reload = Animator.StringToHash("reload");

	// Token: 0x04001CC4 RID: 7364
	protected static int throwWeapon = Animator.StringToHash("throw");

	// Token: 0x04001CC5 RID: 7365
	protected static int holster = Animator.StringToHash("holster");

	// Token: 0x04001CC6 RID: 7366
	protected static int aiming = Animator.StringToHash("aiming");

	// Token: 0x04001CC7 RID: 7367
	protected static int onLadder = Animator.StringToHash("onLadder");

	// Token: 0x04001CC8 RID: 7368
	protected static int posing = Animator.StringToHash("posing");

	// Token: 0x04001CC9 RID: 7369
	protected static int poseType = Animator.StringToHash("poseType");

	// Token: 0x04001CCA RID: 7370
	protected static int relaxGunPose = Animator.StringToHash("relaxGunPose");

	// Token: 0x04001CCB RID: 7371
	protected static int vehicle_aim_yaw = Animator.StringToHash("vehicleAimYaw");

	// Token: 0x04001CCC RID: 7372
	protected static int vehicle_aim_speed = Animator.StringToHash("vehicleAimYawSpeed");

	// Token: 0x04001CCD RID: 7373
	protected static int onPhone = Animator.StringToHash("onPhone");

	// Token: 0x04001CCE RID: 7374
	protected static int usePoseTransition = Animator.StringToHash("usePoseTransition");

	// Token: 0x04001CCF RID: 7375
	protected static int leftFootIK = Animator.StringToHash("leftFootIK");

	// Token: 0x04001CD0 RID: 7376
	protected static int rightFootIK = Animator.StringToHash("rightFootIK");

	// Token: 0x04001CD1 RID: 7377
	protected static int vehicleSteering = Animator.StringToHash("vehicleSteering");

	// Token: 0x04001CD2 RID: 7378
	protected static int sitReaction = Animator.StringToHash("sitReaction");

	// Token: 0x04001CD3 RID: 7379
	protected static int forwardReaction = Animator.StringToHash("forwardReaction");

	// Token: 0x04001CD4 RID: 7380
	protected static int rightReaction = Animator.StringToHash("rightReaction");

	// Token: 0x04001CD5 RID: 7381
	public BoxCollider collision;

	// Token: 0x04001CD6 RID: 7382
	public GameObject censorshipCube;

	// Token: 0x04001CD7 RID: 7383
	public GameObject censorshipCubeBreasts;

	// Token: 0x04001CD8 RID: 7384
	public GameObject jawBone;

	// Token: 0x04001CD9 RID: 7385
	public GameObject neckBone;

	// Token: 0x04001CDA RID: 7386
	public GameObject headBone;

	// Token: 0x04001CDB RID: 7387
	public EyeController eyeController;

	// Token: 0x04001CDC RID: 7388
	public EyeBlink blinkController;

	// Token: 0x04001CDD RID: 7389
	public Transform[] SpineBones;

	// Token: 0x04001CDE RID: 7390
	public Transform leftFootBone;

	// Token: 0x04001CDF RID: 7391
	public Transform rightFootBone;

	// Token: 0x04001CE0 RID: 7392
	public Transform leftHandPropBone;

	// Token: 0x04001CE1 RID: 7393
	public Transform rightHandPropBone;

	// Token: 0x04001CE2 RID: 7394
	public Vector3 rightHandTarget;

	// Token: 0x04001CE3 RID: 7395
	[Header("IK")]
	public Vector3 leftHandTargetPosition;

	// Token: 0x04001CE4 RID: 7396
	public Quaternion leftHandTargetRotation;

	// Token: 0x04001CE5 RID: 7397
	public Vector3 rightHandTargetPosition;

	// Token: 0x04001CE6 RID: 7398
	public Quaternion rightHandTargetRotation;

	// Token: 0x04001CE7 RID: 7399
	public float steeringTargetDegrees;

	// Token: 0x04001CE8 RID: 7400
	public Vector3 rightFootTargetPosition;

	// Token: 0x04001CE9 RID: 7401
	public Quaternion rightFootTargetRotation;

	// Token: 0x04001CEA RID: 7402
	public Vector3 leftFootTargetPosition;

	// Token: 0x04001CEB RID: 7403
	public Quaternion leftFootTargetRotation;

	// Token: 0x04001CEC RID: 7404
	public RuntimeAnimatorController CinematicAnimationController;

	// Token: 0x04001CED RID: 7405
	public Avatar DefaultAvatar;

	// Token: 0x04001CEE RID: 7406
	public Avatar CinematicAvatar;

	// Token: 0x04001CEF RID: 7407
	public RuntimeAnimatorController DefaultHoldType;

	// Token: 0x04001CF0 RID: 7408
	public RuntimeAnimatorController SleepGesture;

	// Token: 0x04001CF1 RID: 7409
	public RuntimeAnimatorController CrawlToIncapacitatedGesture;

	// Token: 0x04001CF2 RID: 7410
	public RuntimeAnimatorController StandToIncapacitatedGesture;

	// Token: 0x04001CF3 RID: 7411
	[NonSerialized]
	public RuntimeAnimatorController CurrentGesture;

	// Token: 0x04001CF4 RID: 7412
	[Header("Skin")]
	public SkinSetCollection MaleSkin;

	// Token: 0x04001CF5 RID: 7413
	public SkinSetCollection FemaleSkin;

	// Token: 0x04001CF6 RID: 7414
	public SubsurfaceProfile subsurfaceProfile;

	// Token: 0x04001CF7 RID: 7415
	[Header("Parameters")]
	[Range(0f, 1f)]
	public float voiceVolume;

	// Token: 0x04001CF8 RID: 7416
	[Range(0f, 1f)]
	public float skinColor = 1f;

	// Token: 0x04001CF9 RID: 7417
	[Range(0f, 1f)]
	public float skinNumber = 1f;

	// Token: 0x04001CFA RID: 7418
	[Range(0f, 1f)]
	public float meshNumber;

	// Token: 0x04001CFB RID: 7419
	[Range(0f, 1f)]
	public float hairNumber;

	// Token: 0x04001CFC RID: 7420
	[Range(0f, 1f)]
	public int skinType;

	// Token: 0x04001CFD RID: 7421
	public MovementSounds movementSounds;

	// Token: 0x04001CFE RID: 7422
	public bool showSash;

	// Token: 0x04001CFF RID: 7423
	public int tempPoseType;

	// Token: 0x04001D00 RID: 7424
	public uint underwearSkin;

	// Token: 0x0600249F RID: 9375 RVA: 0x000E8787 File Offset: 0x000E6987
	private static Vector3 GetFlat(Vector3 dir)
	{
		dir.y = 0f;
		return dir.normalized;
	}

	// Token: 0x060024A0 RID: 9376 RVA: 0x000063A5 File Offset: 0x000045A5
	public static void RebuildAll()
	{
	}

	// Token: 0x1700030A RID: 778
	// (get) Token: 0x060024A1 RID: 9377 RVA: 0x000E879C File Offset: 0x000E699C
	// (set) Token: 0x060024A2 RID: 9378 RVA: 0x000E87A4 File Offset: 0x000E69A4
	public ulong overrideSkinSeed { get; private set; }

	// Token: 0x1700030B RID: 779
	// (get) Token: 0x060024A3 RID: 9379 RVA: 0x000E87AD File Offset: 0x000E69AD
	public bool IsFemale
	{
		get
		{
			return this.skinType == 1;
		}
	}

	// Token: 0x1700030C RID: 780
	// (get) Token: 0x060024A4 RID: 9380 RVA: 0x000E87B8 File Offset: 0x000E69B8
	public SkinSetCollection SkinSet
	{
		get
		{
			if (!this.IsFemale)
			{
				return this.MaleSkin;
			}
			return this.FemaleSkin;
		}
	}

	// Token: 0x1700030D RID: 781
	// (get) Token: 0x060024A5 RID: 9381 RVA: 0x000E87CF File Offset: 0x000E69CF
	// (set) Token: 0x060024A6 RID: 9382 RVA: 0x000E87D7 File Offset: 0x000E69D7
	public Quaternion AimAngles { get; set; }

	// Token: 0x1700030E RID: 782
	// (get) Token: 0x060024A7 RID: 9383 RVA: 0x000E87E0 File Offset: 0x000E69E0
	// (set) Token: 0x060024A8 RID: 9384 RVA: 0x000E87E8 File Offset: 0x000E69E8
	public Quaternion LookAngles { get; set; }

	// Token: 0x02000CE6 RID: 3302
	public enum MountPoses
	{
		// Token: 0x04004564 RID: 17764
		Chair,
		// Token: 0x04004565 RID: 17765
		Driving,
		// Token: 0x04004566 RID: 17766
		Horseback,
		// Token: 0x04004567 RID: 17767
		HeliUnarmed,
		// Token: 0x04004568 RID: 17768
		HeliArmed,
		// Token: 0x04004569 RID: 17769
		HandMotorBoat,
		// Token: 0x0400456A RID: 17770
		MotorBoatPassenger,
		// Token: 0x0400456B RID: 17771
		SitGeneric,
		// Token: 0x0400456C RID: 17772
		SitRaft,
		// Token: 0x0400456D RID: 17773
		StandDrive,
		// Token: 0x0400456E RID: 17774
		SitShootingGeneric,
		// Token: 0x0400456F RID: 17775
		SitMinicopter_Pilot,
		// Token: 0x04004570 RID: 17776
		SitMinicopter_Passenger,
		// Token: 0x04004571 RID: 17777
		ArcadeLeft,
		// Token: 0x04004572 RID: 17778
		ArcadeRight,
		// Token: 0x04004573 RID: 17779
		SitSummer_Ring,
		// Token: 0x04004574 RID: 17780
		SitSummer_BoogieBoard,
		// Token: 0x04004575 RID: 17781
		SitCarPassenger,
		// Token: 0x04004576 RID: 17782
		SitSummer_Chair,
		// Token: 0x04004577 RID: 17783
		SitRaft_NoPaddle,
		// Token: 0x04004578 RID: 17784
		Sit_SecretLab,
		// Token: 0x04004579 RID: 17785
		Sit_Workcart,
		// Token: 0x0400457A RID: 17786
		Sit_Cardgame,
		// Token: 0x0400457B RID: 17787
		Sit_Crane,
		// Token: 0x0400457C RID: 17788
		Sit_Snowmobile_Shooting,
		// Token: 0x0400457D RID: 17789
		Sit_RetroSnowmobile_Shooting,
		// Token: 0x0400457E RID: 17790
		Driving_Snowmobile,
		// Token: 0x0400457F RID: 17791
		ZiplineHold,
		// Token: 0x04004580 RID: 17792
		Sit_Locomotive,
		// Token: 0x04004581 RID: 17793
		Sit_Throne,
		// Token: 0x04004582 RID: 17794
		Standing = 128
	}
}
