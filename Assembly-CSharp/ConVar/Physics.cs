using System;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000AD2 RID: 2770
	[ConsoleSystem.Factory("physics")]
	public class Physics : ConsoleSystem
	{
		// Token: 0x04003B88 RID: 15240
		private const float baseGravity = -9.81f;

		// Token: 0x04003B89 RID: 15241
		[ServerVar(Help = "The collision detection mode that dropped items and corpses should use")]
		public static int droppedmode = 2;

		// Token: 0x04003B8A RID: 15242
		[ServerVar(Help = "Send effects to clients when physics objects collide")]
		public static bool sendeffects = true;

		// Token: 0x04003B8B RID: 15243
		[ServerVar]
		public static bool groundwatchdebug = false;

		// Token: 0x04003B8C RID: 15244
		[ServerVar]
		public static int groundwatchfails = 1;

		// Token: 0x04003B8D RID: 15245
		[ServerVar]
		public static float groundwatchdelay = 0.1f;

		// Token: 0x04003B8E RID: 15246
		[ClientVar]
		[ServerVar]
		public static bool batchsynctransforms = true;

		// Token: 0x170005BE RID: 1470
		// (get) Token: 0x0600427D RID: 17021 RVA: 0x00189FFC File Offset: 0x001881FC
		// (set) Token: 0x0600427E RID: 17022 RVA: 0x0018A003 File Offset: 0x00188203
		[ServerVar]
		public static float bouncethreshold
		{
			get
			{
				return Physics.bounceThreshold;
			}
			set
			{
				Physics.bounceThreshold = value;
			}
		}

		// Token: 0x170005BF RID: 1471
		// (get) Token: 0x0600427F RID: 17023 RVA: 0x0018A00B File Offset: 0x0018820B
		// (set) Token: 0x06004280 RID: 17024 RVA: 0x0018A012 File Offset: 0x00188212
		[ServerVar]
		public static float sleepthreshold
		{
			get
			{
				return Physics.sleepThreshold;
			}
			set
			{
				Physics.sleepThreshold = value;
			}
		}

		// Token: 0x170005C0 RID: 1472
		// (get) Token: 0x06004281 RID: 17025 RVA: 0x0018A01A File Offset: 0x0018821A
		// (set) Token: 0x06004282 RID: 17026 RVA: 0x0018A021 File Offset: 0x00188221
		[ServerVar(Help = "The default solver iteration count permitted for any rigid bodies (default 7). Must be positive")]
		public static int solveriterationcount
		{
			get
			{
				return Physics.defaultSolverIterations;
			}
			set
			{
				Physics.defaultSolverIterations = value;
			}
		}

		// Token: 0x170005C1 RID: 1473
		// (get) Token: 0x06004283 RID: 17027 RVA: 0x0018A029 File Offset: 0x00188229
		// (set) Token: 0x06004284 RID: 17028 RVA: 0x0018A03B File Offset: 0x0018823B
		[ServerVar(Help = "Gravity multiplier")]
		public static float gravity
		{
			get
			{
				return Physics.gravity.y / -9.81f;
			}
			set
			{
				Physics.gravity = new Vector3(0f, value * -9.81f, 0f);
			}
		}

		// Token: 0x06004285 RID: 17029 RVA: 0x0018A058 File Offset: 0x00188258
		internal static void ApplyDropped(Rigidbody rigidBody)
		{
			if (Physics.droppedmode <= 0)
			{
				rigidBody.collisionDetectionMode = CollisionDetectionMode.Discrete;
			}
			if (Physics.droppedmode == 1)
			{
				rigidBody.collisionDetectionMode = CollisionDetectionMode.Continuous;
			}
			if (Physics.droppedmode == 2)
			{
				rigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
			}
			if (Physics.droppedmode >= 3)
			{
				rigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
			}
		}

		// Token: 0x170005C2 RID: 1474
		// (get) Token: 0x06004286 RID: 17030 RVA: 0x0018A096 File Offset: 0x00188296
		// (set) Token: 0x06004287 RID: 17031 RVA: 0x0018A0A3 File Offset: 0x001882A3
		[ClientVar(ClientAdmin = true)]
		[ServerVar(Help = "The amount of physics steps per second")]
		public static float steps
		{
			get
			{
				return 1f / Time.fixedDeltaTime;
			}
			set
			{
				if (value < 10f)
				{
					value = 10f;
				}
				if (value > 60f)
				{
					value = 60f;
				}
				Time.fixedDeltaTime = 1f / value;
			}
		}

		// Token: 0x170005C3 RID: 1475
		// (get) Token: 0x06004288 RID: 17032 RVA: 0x0018A0CF File Offset: 0x001882CF
		// (set) Token: 0x06004289 RID: 17033 RVA: 0x0018A0DC File Offset: 0x001882DC
		[ClientVar(ClientAdmin = true)]
		[ServerVar(Help = "The slowest physics steps will operate")]
		public static float minsteps
		{
			get
			{
				return 1f / Time.maximumDeltaTime;
			}
			set
			{
				if (value < 1f)
				{
					value = 1f;
				}
				if (value > 60f)
				{
					value = 60f;
				}
				Time.maximumDeltaTime = 1f / value;
			}
		}

		// Token: 0x170005C4 RID: 1476
		// (get) Token: 0x0600428A RID: 17034 RVA: 0x0018A108 File Offset: 0x00188308
		// (set) Token: 0x0600428B RID: 17035 RVA: 0x0018A10F File Offset: 0x0018830F
		[ClientVar]
		[ServerVar]
		public static bool autosynctransforms
		{
			get
			{
				return Physics.autoSyncTransforms;
			}
			set
			{
				Physics.autoSyncTransforms = value;
			}
		}
	}
}
