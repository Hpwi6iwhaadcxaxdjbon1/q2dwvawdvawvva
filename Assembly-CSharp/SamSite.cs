using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000C6 RID: 198
public class SamSite : ContainerIOEntity
{
	// Token: 0x04000B00 RID: 2816
	public Animator pitchAnimator;

	// Token: 0x04000B01 RID: 2817
	public GameObject yaw;

	// Token: 0x04000B02 RID: 2818
	public GameObject pitch;

	// Token: 0x04000B03 RID: 2819
	public GameObject gear;

	// Token: 0x04000B04 RID: 2820
	public Transform eyePoint;

	// Token: 0x04000B05 RID: 2821
	public float gearEpislonDegrees = 20f;

	// Token: 0x04000B06 RID: 2822
	public float turnSpeed = 1f;

	// Token: 0x04000B07 RID: 2823
	public float clientLerpSpeed = 1f;

	// Token: 0x04000B08 RID: 2824
	public Vector3 currentAimDir = Vector3.forward;

	// Token: 0x04000B09 RID: 2825
	public Vector3 targetAimDir = Vector3.forward;

	// Token: 0x04000B0A RID: 2826
	public float vehicleScanRadius = 350f;

	// Token: 0x04000B0B RID: 2827
	public float missileScanRadius = 500f;

	// Token: 0x04000B0C RID: 2828
	public GameObjectRef projectileTest;

	// Token: 0x04000B0D RID: 2829
	public GameObjectRef muzzleFlashTest;

	// Token: 0x04000B0E RID: 2830
	public bool staticRespawn;

	// Token: 0x04000B0F RID: 2831
	public ItemDefinition ammoType;

	// Token: 0x04000B10 RID: 2832
	public Transform[] tubes;

	// Token: 0x04000B11 RID: 2833
	[ServerVar(Help = "how long until static sam sites auto repair")]
	public static float staticrepairseconds = 1200f;

	// Token: 0x04000B12 RID: 2834
	public SoundDefinition yawMovementLoopDef;

	// Token: 0x04000B13 RID: 2835
	public float yawGainLerp = 8f;

	// Token: 0x04000B14 RID: 2836
	public float yawGainMovementSpeedMult = 0.1f;

	// Token: 0x04000B15 RID: 2837
	public SoundDefinition pitchMovementLoopDef;

	// Token: 0x04000B16 RID: 2838
	public float pitchGainLerp = 10f;

	// Token: 0x04000B17 RID: 2839
	public float pitchGainMovementSpeedMult = 0.5f;

	// Token: 0x04000B18 RID: 2840
	public int lowAmmoThreshold = 5;

	// Token: 0x04000B19 RID: 2841
	public global::BaseEntity.Flags Flag_DefenderMode = global::BaseEntity.Flags.Reserved9;

	// Token: 0x04000B1A RID: 2842
	public static SamSite.SamTargetType targetTypeUnknown;

	// Token: 0x04000B1B RID: 2843
	public static SamSite.SamTargetType targetTypeVehicle;

	// Token: 0x04000B1C RID: 2844
	public static SamSite.SamTargetType targetTypeMissile;

	// Token: 0x04000B1D RID: 2845
	private SamSite.ISamSiteTarget currentTarget;

	// Token: 0x04000B1E RID: 2846
	private SamSite.SamTargetType mostRecentTargetType;

	// Token: 0x04000B1F RID: 2847
	private global::Item ammoItem;

	// Token: 0x04000B20 RID: 2848
	private float lockOnTime;

	// Token: 0x04000B21 RID: 2849
	private float lastTargetVisibleTime;

	// Token: 0x04000B22 RID: 2850
	private int lastAmmoCount;

	// Token: 0x04000B23 RID: 2851
	private int currentTubeIndex;

	// Token: 0x04000B24 RID: 2852
	private int firedCount;

	// Token: 0x04000B25 RID: 2853
	private float nextBurstTime;

	// Token: 0x060011B9 RID: 4537 RVA: 0x0008FE88 File Offset: 0x0008E088
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("SamSite.OnRpcMessage", 0))
		{
			if (rpc == 3160662357U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ToggleDefenderMode ");
				}
				using (TimeWarning.New("ToggleDefenderMode", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(3160662357U, "ToggleDefenderMode", this, player, 1UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3160662357U, "ToggleDefenderMode", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg2 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.ToggleDefenderMode(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in ToggleDefenderMode");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060011BA RID: 4538 RVA: 0x00090048 File Offset: 0x0008E248
	public override bool IsPowered()
	{
		return this.staticRespawn || base.HasFlag(global::BaseEntity.Flags.Reserved8);
	}

	// Token: 0x060011BB RID: 4539 RVA: 0x0009005F File Offset: 0x0008E25F
	public override int ConsumptionAmount()
	{
		return 25;
	}

	// Token: 0x060011BC RID: 4540 RVA: 0x00090063 File Offset: 0x0008E263
	public bool IsInDefenderMode()
	{
		return base.HasFlag(this.Flag_DefenderMode);
	}

	// Token: 0x060011BD RID: 4541 RVA: 0x00090071 File Offset: 0x0008E271
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
	}

	// Token: 0x060011BE RID: 4542 RVA: 0x0009007A File Offset: 0x0008E27A
	private void SetTarget(SamSite.ISamSiteTarget target)
	{
		bool flag = this.currentTarget != target;
		this.currentTarget = target;
		if (!target.IsUnityNull<SamSite.ISamSiteTarget>())
		{
			this.mostRecentTargetType = target.SAMTargetType;
		}
		if (flag)
		{
			this.MarkIODirty();
		}
	}

	// Token: 0x060011BF RID: 4543 RVA: 0x000900AB File Offset: 0x0008E2AB
	private void MarkIODirty()
	{
		if (this.staticRespawn)
		{
			return;
		}
		this.lastPassthroughEnergy = -1;
		base.MarkDirtyForceUpdateOutputs();
	}

	// Token: 0x060011C0 RID: 4544 RVA: 0x000900C3 File Offset: 0x0008E2C3
	private void ClearTarget()
	{
		this.SetTarget(null);
	}

	// Token: 0x060011C1 RID: 4545 RVA: 0x000900CC File Offset: 0x0008E2CC
	public override void ServerInit()
	{
		base.ServerInit();
		SamSite.targetTypeUnknown = new SamSite.SamTargetType(this.vehicleScanRadius, 1f, 5f);
		SamSite.targetTypeVehicle = new SamSite.SamTargetType(this.vehicleScanRadius, 1f, 5f);
		SamSite.targetTypeMissile = new SamSite.SamTargetType(this.missileScanRadius, 2.25f, 3.5f);
		this.mostRecentTargetType = SamSite.targetTypeUnknown;
		this.ClearTarget();
		base.InvokeRandomized(new Action(this.TargetScan), 1f, 3f, 1f);
		this.currentAimDir = base.transform.forward;
		if (base.inventory != null && !this.staticRespawn)
		{
			base.inventory.onItemAddedRemoved = new Action<global::Item, bool>(this.OnItemAddedRemoved);
		}
	}

	// Token: 0x060011C2 RID: 4546 RVA: 0x00090197 File Offset: 0x0008E397
	private void OnItemAddedRemoved(global::Item arg1, bool arg2)
	{
		this.EnsureReloaded();
		if (this.IsPowered())
		{
			this.MarkIODirty();
		}
	}

	// Token: 0x060011C3 RID: 4547 RVA: 0x000901AD File Offset: 0x0008E3AD
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.samSite = Facepunch.Pool.Get<SAMSite>();
		info.msg.samSite.aimDir = this.GetAimDir();
	}

	// Token: 0x060011C4 RID: 4548 RVA: 0x000901DC File Offset: 0x0008E3DC
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (this.staticRespawn && base.HasFlag(global::BaseEntity.Flags.Reserved1))
		{
			base.Invoke(new Action(this.SelfHeal), SamSite.staticrepairseconds);
		}
	}

	// Token: 0x060011C5 RID: 4549 RVA: 0x00090210 File Offset: 0x0008E410
	public void SelfHeal()
	{
		this.lifestate = BaseCombatEntity.LifeState.Alive;
		base.health = this.startHealth;
		base.SetFlag(global::BaseEntity.Flags.Reserved1, false, false, true);
	}

	// Token: 0x060011C6 RID: 4550 RVA: 0x00090234 File Offset: 0x0008E434
	public override void Die(HitInfo info = null)
	{
		if (this.staticRespawn)
		{
			this.ClearTarget();
			Quaternion rotation = Quaternion.Euler(0f, Quaternion.LookRotation(this.currentAimDir, Vector3.up).eulerAngles.y, 0f);
			this.currentAimDir = rotation * Vector3.forward;
			base.Invoke(new Action(this.SelfHeal), SamSite.staticrepairseconds);
			this.lifestate = BaseCombatEntity.LifeState.Dead;
			base.health = 0f;
			base.SetFlag(global::BaseEntity.Flags.Reserved1, true, false, true);
			return;
		}
		base.Die(info);
	}

	// Token: 0x060011C7 RID: 4551 RVA: 0x000902CC File Offset: 0x0008E4CC
	public void FixedUpdate()
	{
		Vector3 rhs = this.currentAimDir;
		if (!this.currentTarget.IsUnityNull<SamSite.ISamSiteTarget>() && this.IsPowered())
		{
			float num = this.projectileTest.Get().GetComponent<ServerProjectile>().speed * this.currentTarget.SAMTargetType.speedMultiplier;
			Vector3 a = this.currentTarget.CenterPoint();
			float num2 = Vector3.Distance(a, this.eyePoint.transform.position);
			float num3 = num2 / num;
			Vector3 a2 = a + this.currentTarget.GetWorldVelocity() * num3;
			num3 = Vector3.Distance(a2, this.eyePoint.transform.position) / num;
			a2 = a + this.currentTarget.GetWorldVelocity() * num3;
			if (this.currentTarget.GetWorldVelocity().magnitude > 0.1f)
			{
				float d = Mathf.Sin(UnityEngine.Time.time * 3f) * (1f + num3 * 0.5f);
				a2 += this.currentTarget.GetWorldVelocity().normalized * d;
			}
			this.currentAimDir = (a2 - this.eyePoint.transform.position).normalized;
			if (num2 > this.currentTarget.SAMTargetType.scanRadius)
			{
				this.ClearTarget();
			}
		}
		Vector3 vector = Quaternion.LookRotation(this.currentAimDir, base.transform.up).eulerAngles;
		vector = BaseMountable.ConvertVector(vector);
		float t = Mathf.InverseLerp(0f, 90f, -vector.x);
		float z = Mathf.Lerp(15f, -75f, t);
		Quaternion localRotation = Quaternion.Euler(0f, vector.y, 0f);
		this.yaw.transform.localRotation = localRotation;
		Quaternion localRotation2 = Quaternion.Euler(this.pitch.transform.localRotation.eulerAngles.x, this.pitch.transform.localRotation.eulerAngles.y, z);
		this.pitch.transform.localRotation = localRotation2;
		if (this.currentAimDir != rhs)
		{
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x060011C8 RID: 4552 RVA: 0x00090520 File Offset: 0x0008E720
	public Vector3 GetAimDir()
	{
		return this.currentAimDir;
	}

	// Token: 0x060011C9 RID: 4553 RVA: 0x00090528 File Offset: 0x0008E728
	public bool HasValidTarget()
	{
		return !this.currentTarget.IsUnityNull<SamSite.ISamSiteTarget>();
	}

	// Token: 0x060011CA RID: 4554 RVA: 0x00090538 File Offset: 0x0008E738
	public override bool CanPickup(global::BasePlayer player)
	{
		return base.CanPickup(player) && (!base.isServer || !this.pickup.requireEmptyInv || base.inventory == null || base.inventory.itemList.Count <= 0) && !this.HasAmmo();
	}

	// Token: 0x060011CB RID: 4555 RVA: 0x0009058C File Offset: 0x0008E78C
	public void TargetScan()
	{
		if (!this.IsPowered())
		{
			this.lastTargetVisibleTime = 0f;
			return;
		}
		if (UnityEngine.Time.time > this.lastTargetVisibleTime + 3f)
		{
			this.ClearTarget();
		}
		if (!this.staticRespawn)
		{
			int num = (this.ammoItem != null && this.ammoItem.parent == base.inventory) ? this.ammoItem.amount : 0;
			bool flag = this.lastAmmoCount < this.lowAmmoThreshold;
			bool flag2 = num < this.lowAmmoThreshold;
			if (num != this.lastAmmoCount && flag != flag2)
			{
				this.MarkIODirty();
			}
			this.lastAmmoCount = num;
		}
		if (this.HasValidTarget())
		{
			return;
		}
		if (this.IsDead())
		{
			return;
		}
		List<SamSite.ISamSiteTarget> list = Facepunch.Pool.GetList<SamSite.ISamSiteTarget>();
		if (!this.IsInDefenderMode())
		{
			this.<TargetScan>g__AddTargetSet|55_0(list, 32768, SamSite.targetTypeVehicle.scanRadius);
		}
		this.<TargetScan>g__AddTargetSet|55_0(list, 1048576, SamSite.targetTypeMissile.scanRadius);
		SamSite.ISamSiteTarget samSiteTarget = null;
		foreach (SamSite.ISamSiteTarget samSiteTarget2 in list)
		{
			if (!samSiteTarget2.isClient && samSiteTarget2.CenterPoint().y >= this.eyePoint.transform.position.y && samSiteTarget2.IsVisible(this.eyePoint.transform.position, samSiteTarget2.SAMTargetType.scanRadius * 2f) && samSiteTarget2.IsValidSAMTarget(this.staticRespawn))
			{
				samSiteTarget = samSiteTarget2;
				break;
			}
		}
		if (!samSiteTarget.IsUnityNull<SamSite.ISamSiteTarget>() && this.currentTarget != samSiteTarget)
		{
			this.lockOnTime = UnityEngine.Time.time + 0.5f;
		}
		this.SetTarget(samSiteTarget);
		if (!this.currentTarget.IsUnityNull<SamSite.ISamSiteTarget>())
		{
			this.lastTargetVisibleTime = UnityEngine.Time.time;
		}
		Facepunch.Pool.FreeList<SamSite.ISamSiteTarget>(ref list);
		if (this.currentTarget.IsUnityNull<SamSite.ISamSiteTarget>())
		{
			base.CancelInvoke(new Action(this.WeaponTick));
			return;
		}
		base.InvokeRandomized(new Action(this.WeaponTick), 0f, 0.5f, 0.2f);
	}

	// Token: 0x060011CC RID: 4556 RVA: 0x000907AC File Offset: 0x0008E9AC
	public virtual bool HasAmmo()
	{
		return this.staticRespawn || (this.ammoItem != null && this.ammoItem.amount > 0 && this.ammoItem.parent == base.inventory);
	}

	// Token: 0x060011CD RID: 4557 RVA: 0x000907E4 File Offset: 0x0008E9E4
	public void Reload()
	{
		if (this.staticRespawn)
		{
			return;
		}
		for (int i = 0; i < base.inventory.itemList.Count; i++)
		{
			global::Item item = base.inventory.itemList[i];
			if (item != null && item.info.itemid == this.ammoType.itemid && item.amount > 0)
			{
				this.ammoItem = item;
				return;
			}
		}
		this.ammoItem = null;
	}

	// Token: 0x060011CE RID: 4558 RVA: 0x0009085A File Offset: 0x0008EA5A
	public void EnsureReloaded()
	{
		if (!this.HasAmmo())
		{
			this.Reload();
		}
	}

	// Token: 0x060011CF RID: 4559 RVA: 0x0009086A File Offset: 0x0008EA6A
	public bool IsReloading()
	{
		return base.IsInvoking(new Action(this.Reload));
	}

	// Token: 0x060011D0 RID: 4560 RVA: 0x00090880 File Offset: 0x0008EA80
	public void WeaponTick()
	{
		if (this.IsDead())
		{
			return;
		}
		if (UnityEngine.Time.time < this.lockOnTime)
		{
			return;
		}
		if (UnityEngine.Time.time < this.nextBurstTime)
		{
			return;
		}
		if (!this.IsPowered())
		{
			this.firedCount = 0;
			return;
		}
		if (this.firedCount >= 6)
		{
			float timeBetweenBursts = this.mostRecentTargetType.timeBetweenBursts;
			this.nextBurstTime = UnityEngine.Time.time + timeBetweenBursts;
			this.firedCount = 0;
			return;
		}
		this.EnsureReloaded();
		if (!this.HasAmmo())
		{
			return;
		}
		bool flag = this.ammoItem != null && this.ammoItem.amount == this.lowAmmoThreshold;
		if (!this.staticRespawn && this.ammoItem != null)
		{
			this.ammoItem.UseItem(1);
		}
		this.firedCount++;
		float speedMultiplier = 1f;
		if (!this.currentTarget.IsUnityNull<SamSite.ISamSiteTarget>())
		{
			speedMultiplier = this.currentTarget.SAMTargetType.speedMultiplier;
		}
		this.FireProjectile(this.tubes[this.currentTubeIndex].position, this.currentAimDir, speedMultiplier);
		Effect.server.Run(this.muzzleFlashTest.resourcePath, this, StringPool.Get("Tube " + (this.currentTubeIndex + 1).ToString()), Vector3.zero, Vector3.up, null, false);
		this.currentTubeIndex++;
		if (this.currentTubeIndex >= this.tubes.Length)
		{
			this.currentTubeIndex = 0;
		}
		if (flag)
		{
			this.MarkIODirty();
		}
	}

	// Token: 0x060011D1 RID: 4561 RVA: 0x000909F0 File Offset: 0x0008EBF0
	public void FireProjectile(Vector3 origin, Vector3 direction, float speedMultiplier)
	{
		global::BaseEntity baseEntity = GameManager.server.CreateEntity(this.projectileTest.resourcePath, origin, Quaternion.LookRotation(direction, Vector3.up), true);
		if (baseEntity == null)
		{
			return;
		}
		baseEntity.creatorEntity = this;
		ServerProjectile component = baseEntity.GetComponent<ServerProjectile>();
		if (component)
		{
			component.InitializeVelocity(this.GetInheritedProjectileVelocity(direction) + direction * component.speed * speedMultiplier);
		}
		baseEntity.Spawn();
	}

	// Token: 0x060011D2 RID: 4562 RVA: 0x00090A6C File Offset: 0x0008EC6C
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		int result = Mathf.Min(1, this.GetCurrentEnergy());
		if (outputSlot == 0)
		{
			if (this.currentTarget.IsUnityNull<SamSite.ISamSiteTarget>())
			{
				return 0;
			}
			return result;
		}
		else if (outputSlot == 1)
		{
			if (this.ammoItem == null || this.ammoItem.amount >= this.lowAmmoThreshold || this.ammoItem.parent != base.inventory)
			{
				return 0;
			}
			return result;
		}
		else
		{
			if (outputSlot != 2)
			{
				return this.GetCurrentEnergy();
			}
			if (this.HasAmmo())
			{
				return 0;
			}
			return result;
		}
	}

	// Token: 0x060011D3 RID: 4563 RVA: 0x00090AE8 File Offset: 0x0008ECE8
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(1UL)]
	private void ToggleDefenderMode(global::BaseEntity.RPCMessage msg)
	{
		if (this.staticRespawn)
		{
			return;
		}
		global::BasePlayer player = msg.player;
		if (player == null || !player.CanBuild())
		{
			return;
		}
		bool flag = msg.read.Bit();
		if (flag == this.IsInDefenderMode())
		{
			return;
		}
		base.SetFlag(this.Flag_DefenderMode, flag, false, true);
	}

	// Token: 0x060011D6 RID: 4566 RVA: 0x00090BE8 File Offset: 0x0008EDE8
	[CompilerGenerated]
	private void <TargetScan>g__AddTargetSet|55_0(List<SamSite.ISamSiteTarget> allTargets, int layerMask, float scanRadius)
	{
		List<SamSite.ISamSiteTarget> list = Facepunch.Pool.GetList<SamSite.ISamSiteTarget>();
		global::Vis.Entities<SamSite.ISamSiteTarget>(this.eyePoint.transform.position, scanRadius, list, layerMask, QueryTriggerInteraction.Ignore);
		allTargets.AddRange(list);
		Facepunch.Pool.FreeList<SamSite.ISamSiteTarget>(ref list);
	}

	// Token: 0x02000BF6 RID: 3062
	public interface ISamSiteTarget
	{
		// Token: 0x17000678 RID: 1656
		// (get) Token: 0x06004DE0 RID: 19936
		SamSite.SamTargetType SAMTargetType { get; }

		// Token: 0x17000679 RID: 1657
		// (get) Token: 0x06004DE1 RID: 19937
		bool isClient { get; }

		// Token: 0x06004DE2 RID: 19938
		bool IsValidSAMTarget(bool staticRespawn);

		// Token: 0x06004DE3 RID: 19939
		Vector3 CenterPoint();

		// Token: 0x06004DE4 RID: 19940
		Vector3 GetWorldVelocity();

		// Token: 0x06004DE5 RID: 19941
		bool IsVisible(Vector3 position, float maxDistance = float.PositiveInfinity);
	}

	// Token: 0x02000BF7 RID: 3063
	public class SamTargetType
	{
		// Token: 0x04004173 RID: 16755
		public readonly float scanRadius;

		// Token: 0x04004174 RID: 16756
		public readonly float speedMultiplier;

		// Token: 0x04004175 RID: 16757
		public readonly float timeBetweenBursts;

		// Token: 0x06004DE6 RID: 19942 RVA: 0x001A1AFA File Offset: 0x0019FCFA
		public SamTargetType(float scanRadius, float speedMultiplier, float timeBetweenBursts)
		{
			this.scanRadius = scanRadius;
			this.speedMultiplier = speedMultiplier;
			this.timeBetweenBursts = timeBetweenBursts;
		}
	}
}
