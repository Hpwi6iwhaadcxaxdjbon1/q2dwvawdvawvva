using System;
using System.Collections.Generic;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;

// Token: 0x0200003D RID: 61
public class BaseHelicopter : BaseCombatEntity
{
	// Token: 0x040002D4 RID: 724
	public BaseHelicopter.weakspot[] weakspots;

	// Token: 0x040002D5 RID: 725
	public GameObject rotorPivot;

	// Token: 0x040002D6 RID: 726
	public GameObject mainRotor;

	// Token: 0x040002D7 RID: 727
	public GameObject mainRotor_blades;

	// Token: 0x040002D8 RID: 728
	public GameObject mainRotor_blur;

	// Token: 0x040002D9 RID: 729
	public GameObject tailRotor;

	// Token: 0x040002DA RID: 730
	public GameObject tailRotor_blades;

	// Token: 0x040002DB RID: 731
	public GameObject tailRotor_blur;

	// Token: 0x040002DC RID: 732
	public GameObject rocket_tube_left;

	// Token: 0x040002DD RID: 733
	public GameObject rocket_tube_right;

	// Token: 0x040002DE RID: 734
	public GameObject left_gun_yaw;

	// Token: 0x040002DF RID: 735
	public GameObject left_gun_pitch;

	// Token: 0x040002E0 RID: 736
	public GameObject left_gun_muzzle;

	// Token: 0x040002E1 RID: 737
	public GameObject right_gun_yaw;

	// Token: 0x040002E2 RID: 738
	public GameObject right_gun_pitch;

	// Token: 0x040002E3 RID: 739
	public GameObject right_gun_muzzle;

	// Token: 0x040002E4 RID: 740
	public GameObject spotlight_rotation;

	// Token: 0x040002E5 RID: 741
	public GameObjectRef rocket_fire_effect;

	// Token: 0x040002E6 RID: 742
	public GameObjectRef gun_fire_effect;

	// Token: 0x040002E7 RID: 743
	public GameObjectRef bulletEffect;

	// Token: 0x040002E8 RID: 744
	public GameObjectRef explosionEffect;

	// Token: 0x040002E9 RID: 745
	public GameObjectRef fireBall;

	// Token: 0x040002EA RID: 746
	public GameObjectRef crateToDrop;

	// Token: 0x040002EB RID: 747
	public int maxCratesToSpawn = 4;

	// Token: 0x040002EC RID: 748
	public float bulletSpeed = 250f;

	// Token: 0x040002ED RID: 749
	public float bulletDamage = 20f;

	// Token: 0x040002EE RID: 750
	public GameObjectRef servergibs;

	// Token: 0x040002EF RID: 751
	public GameObjectRef debrisFieldMarker;

	// Token: 0x040002F0 RID: 752
	public SoundDefinition rotorWashSoundDef;

	// Token: 0x040002F1 RID: 753
	private Sound _rotorWashSound;

	// Token: 0x040002F2 RID: 754
	public SoundDefinition flightEngineSoundDef;

	// Token: 0x040002F3 RID: 755
	public SoundDefinition flightThwopsSoundDef;

	// Token: 0x040002F4 RID: 756
	private Sound flightEngineSound;

	// Token: 0x040002F5 RID: 757
	private Sound flightThwopsSound;

	// Token: 0x040002F6 RID: 758
	private SoundModulation.Modulator flightEngineGainMod;

	// Token: 0x040002F7 RID: 759
	private SoundModulation.Modulator flightThwopsGainMod;

	// Token: 0x040002F8 RID: 760
	public float rotorGainModSmoothing = 0.25f;

	// Token: 0x040002F9 RID: 761
	public float engineGainMin = 0.5f;

	// Token: 0x040002FA RID: 762
	public float engineGainMax = 1f;

	// Token: 0x040002FB RID: 763
	public float thwopGainMin = 0.5f;

	// Token: 0x040002FC RID: 764
	public float thwopGainMax = 1f;

	// Token: 0x040002FD RID: 765
	public float spotlightJitterAmount = 5f;

	// Token: 0x040002FE RID: 766
	public float spotlightJitterSpeed = 5f;

	// Token: 0x040002FF RID: 767
	public GameObject[] nightLights;

	// Token: 0x04000300 RID: 768
	public Vector3 spotlightTarget;

	// Token: 0x04000301 RID: 769
	public float engineSpeed = 1f;

	// Token: 0x04000302 RID: 770
	public float targetEngineSpeed = 1f;

	// Token: 0x04000303 RID: 771
	public float blur_rotationScale = 0.05f;

	// Token: 0x04000304 RID: 772
	public ParticleSystem[] _rotorWashParticles;

	// Token: 0x04000305 RID: 773
	private PatrolHelicopterAI myAI;

	// Token: 0x04000306 RID: 774
	public GameObjectRef mapMarkerEntityPrefab;

	// Token: 0x04000307 RID: 775
	private float lastNetworkUpdate = float.NegativeInfinity;

	// Token: 0x04000308 RID: 776
	private const float networkUpdateRate = 0.25f;

	// Token: 0x04000309 RID: 777
	private global::BaseEntity mapMarkerInstance;

	// Token: 0x060003DB RID: 987 RVA: 0x00030FCC File Offset: 0x0002F1CC
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BaseHelicopter.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060003DC RID: 988 RVA: 0x0003100C File Offset: 0x0002F20C
	public void InitalizeWeakspots()
	{
		BaseHelicopter.weakspot[] array = this.weakspots;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].body = this;
		}
	}

	// Token: 0x060003DD RID: 989 RVA: 0x00031037 File Offset: 0x0002F237
	public override void OnAttacked(HitInfo info)
	{
		base.OnAttacked(info);
		if (base.isServer)
		{
			this.myAI.WasAttacked(info);
		}
	}

	// Token: 0x060003DE RID: 990 RVA: 0x00031054 File Offset: 0x0002F254
	public override void Hurt(HitInfo info)
	{
		bool flag = false;
		if (info.damageTypes.Total() >= base.health)
		{
			base.health = 1000000f;
			this.myAI.CriticalDamage();
			flag = true;
		}
		base.Hurt(info);
		if (!flag)
		{
			foreach (BaseHelicopter.weakspot weakspot in this.weakspots)
			{
				foreach (string str in weakspot.bonenames)
				{
					if (info.HitBone == StringPool.Get(str))
					{
						weakspot.Hurt(info.damageTypes.Total(), info);
						this.myAI.WeakspotDamaged(weakspot, info);
					}
				}
			}
		}
	}

	// Token: 0x060003DF RID: 991 RVA: 0x0002A2EC File Offset: 0x000284EC
	public override float MaxVelocity()
	{
		return 100f;
	}

	// Token: 0x060003E0 RID: 992 RVA: 0x00031101 File Offset: 0x0002F301
	public override void InitShared()
	{
		base.InitShared();
		this.InitalizeWeakspots();
	}

	// Token: 0x060003E1 RID: 993 RVA: 0x0003110F File Offset: 0x0002F30F
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.helicopter != null)
		{
			this.spotlightTarget = info.msg.helicopter.spotlightVec;
		}
	}

	// Token: 0x060003E2 RID: 994 RVA: 0x0003113C File Offset: 0x0002F33C
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.helicopter = Pool.Get<Helicopter>();
		info.msg.helicopter.tiltRot = this.rotorPivot.transform.localRotation.eulerAngles;
		info.msg.helicopter.spotlightVec = this.spotlightTarget;
		info.msg.helicopter.weakspothealths = Pool.Get<List<float>>();
		for (int i = 0; i < this.weakspots.Length; i++)
		{
			info.msg.helicopter.weakspothealths.Add(this.weakspots[i].health);
		}
	}

	// Token: 0x060003E3 RID: 995 RVA: 0x000311E8 File Offset: 0x0002F3E8
	public override void ServerInit()
	{
		base.ServerInit();
		this.myAI = base.GetComponent<PatrolHelicopterAI>();
		if (!this.myAI.hasInterestZone)
		{
			this.myAI.SetInitialDestination(Vector3.zero, 1.25f);
			this.myAI.targetThrottleSpeed = 1f;
			this.myAI.ExitCurrentState();
			this.myAI.State_Patrol_Enter();
		}
		this.CreateMapMarker();
	}

	// Token: 0x060003E4 RID: 996 RVA: 0x00031258 File Offset: 0x0002F458
	public void CreateMapMarker()
	{
		if (this.mapMarkerInstance)
		{
			this.mapMarkerInstance.Kill(global::BaseNetworkable.DestroyMode.None);
		}
		global::BaseEntity baseEntity = GameManager.server.CreateEntity(this.mapMarkerEntityPrefab.resourcePath, Vector3.zero, Quaternion.identity, true);
		baseEntity.SetParent(this, false, false);
		baseEntity.Spawn();
		this.mapMarkerInstance = baseEntity;
	}

	// Token: 0x060003E5 RID: 997 RVA: 0x000312B5 File Offset: 0x0002F4B5
	public override void OnPositionalNetworkUpdate()
	{
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		base.OnPositionalNetworkUpdate();
	}

	// Token: 0x060003E6 RID: 998 RVA: 0x000312C4 File Offset: 0x0002F4C4
	public void CreateExplosionMarker(float durationMinutes)
	{
		global::BaseEntity baseEntity = GameManager.server.CreateEntity(this.debrisFieldMarker.resourcePath, base.transform.position, Quaternion.identity, true);
		baseEntity.Spawn();
		baseEntity.SendMessage("SetDuration", durationMinutes, SendMessageOptions.DontRequireReceiver);
	}

	// Token: 0x060003E7 RID: 999 RVA: 0x00031304 File Offset: 0x0002F504
	public override void OnKilled(HitInfo info)
	{
		if (base.isClient)
		{
			return;
		}
		this.CreateExplosionMarker(10f);
		Effect.server.Run(this.explosionEffect.resourcePath, base.transform.position, Vector3.up, null, true);
		Vector3 vector = this.myAI.GetLastMoveDir() * this.myAI.GetMoveSpeed() * 0.75f;
		GameObject gibSource = this.servergibs.Get().GetComponent<global::ServerGib>()._gibSource;
		List<global::ServerGib> list = global::ServerGib.CreateGibs(this.servergibs.resourcePath, base.gameObject, gibSource, vector, 3f);
		if (info.damageTypes.GetMajorityDamageType() != DamageType.Decay)
		{
			for (int i = 0; i < 12 - this.maxCratesToSpawn; i++)
			{
				global::BaseEntity baseEntity = GameManager.server.CreateEntity(this.fireBall.resourcePath, base.transform.position, base.transform.rotation, true);
				if (baseEntity)
				{
					float min = 3f;
					float max = 10f;
					Vector3 onUnitSphere = UnityEngine.Random.onUnitSphere;
					baseEntity.transform.position = base.transform.position + new Vector3(0f, 1.5f, 0f) + onUnitSphere * UnityEngine.Random.Range(-4f, 4f);
					Collider component = baseEntity.GetComponent<Collider>();
					baseEntity.Spawn();
					baseEntity.SetVelocity(vector + onUnitSphere * UnityEngine.Random.Range(min, max));
					foreach (global::ServerGib serverGib in list)
					{
						Physics.IgnoreCollision(component, serverGib.GetCollider(), true);
					}
				}
			}
		}
		for (int j = 0; j < this.maxCratesToSpawn; j++)
		{
			Vector3 onUnitSphere2 = UnityEngine.Random.onUnitSphere;
			Vector3 pos = base.transform.position + new Vector3(0f, 1.5f, 0f) + onUnitSphere2 * UnityEngine.Random.Range(2f, 3f);
			global::BaseEntity baseEntity2 = GameManager.server.CreateEntity(this.crateToDrop.resourcePath, pos, Quaternion.LookRotation(onUnitSphere2), true);
			baseEntity2.Spawn();
			LootContainer lootContainer = baseEntity2 as LootContainer;
			if (lootContainer)
			{
				lootContainer.Invoke(new Action(lootContainer.RemoveMe), 1800f);
			}
			Collider component2 = baseEntity2.GetComponent<Collider>();
			Rigidbody rigidbody = baseEntity2.gameObject.AddComponent<Rigidbody>();
			rigidbody.useGravity = true;
			rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
			rigidbody.mass = 2f;
			rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
			rigidbody.velocity = vector + onUnitSphere2 * UnityEngine.Random.Range(1f, 3f);
			rigidbody.angularVelocity = Vector3Ex.Range(-1.75f, 1.75f);
			rigidbody.drag = 0.5f * (rigidbody.mass / 5f);
			rigidbody.angularDrag = 0.2f * (rigidbody.mass / 5f);
			FireBall fireBall = GameManager.server.CreateEntity(this.fireBall.resourcePath, default(Vector3), default(Quaternion), true) as FireBall;
			if (fireBall)
			{
				fireBall.SetParent(baseEntity2, false, false);
				fireBall.Spawn();
				fireBall.GetComponent<Rigidbody>().isKinematic = true;
				fireBall.GetComponent<Collider>().enabled = false;
			}
			baseEntity2.SendMessage("SetLockingEnt", fireBall.gameObject, SendMessageOptions.DontRequireReceiver);
			foreach (global::ServerGib serverGib2 in list)
			{
				Physics.IgnoreCollision(component2, serverGib2.GetCollider(), true);
			}
		}
		base.OnKilled(info);
	}

	// Token: 0x060003E8 RID: 1000 RVA: 0x00031708 File Offset: 0x0002F908
	public void Update()
	{
		if (base.isServer && Time.realtimeSinceStartup - this.lastNetworkUpdate >= 0.25f)
		{
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			this.lastNetworkUpdate = Time.realtimeSinceStartup;
		}
	}

	// Token: 0x02000B8B RID: 2955
	[Serializable]
	public class weakspot
	{
		// Token: 0x04003F7C RID: 16252
		[NonSerialized]
		public BaseHelicopter body;

		// Token: 0x04003F7D RID: 16253
		public string[] bonenames;

		// Token: 0x04003F7E RID: 16254
		public float maxHealth;

		// Token: 0x04003F7F RID: 16255
		public float health;

		// Token: 0x04003F80 RID: 16256
		public float healthFractionOnDestroyed = 0.5f;

		// Token: 0x04003F81 RID: 16257
		public GameObjectRef destroyedParticles;

		// Token: 0x04003F82 RID: 16258
		public GameObjectRef damagedParticles;

		// Token: 0x04003F83 RID: 16259
		public GameObject damagedEffect;

		// Token: 0x04003F84 RID: 16260
		public GameObject destroyedEffect;

		// Token: 0x04003F85 RID: 16261
		public List<global::BasePlayer> attackers;

		// Token: 0x04003F86 RID: 16262
		private bool isDestroyed;

		// Token: 0x06004D09 RID: 19721 RVA: 0x0019FE0A File Offset: 0x0019E00A
		public float HealthFraction()
		{
			return this.health / this.maxHealth;
		}

		// Token: 0x06004D0A RID: 19722 RVA: 0x0019FE1C File Offset: 0x0019E01C
		public void Hurt(float amount, HitInfo info)
		{
			if (this.isDestroyed)
			{
				return;
			}
			this.health -= amount;
			Effect.server.Run(this.damagedParticles.resourcePath, this.body, StringPool.Get(this.bonenames[UnityEngine.Random.Range(0, this.bonenames.Length)]), Vector3.zero, Vector3.up, null, true);
			if (this.health <= 0f)
			{
				this.health = 0f;
				this.WeakspotDestroyed();
			}
		}

		// Token: 0x06004D0B RID: 19723 RVA: 0x0019FE9A File Offset: 0x0019E09A
		public void Heal(float amount)
		{
			this.health += amount;
		}

		// Token: 0x06004D0C RID: 19724 RVA: 0x0019FEAC File Offset: 0x0019E0AC
		public void WeakspotDestroyed()
		{
			this.isDestroyed = true;
			Effect.server.Run(this.destroyedParticles.resourcePath, this.body, StringPool.Get(this.bonenames[UnityEngine.Random.Range(0, this.bonenames.Length)]), Vector3.zero, Vector3.up, null, true);
			this.body.Hurt(this.body.MaxHealth() * this.healthFractionOnDestroyed, DamageType.Generic, null, false);
		}
	}
}
