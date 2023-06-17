using System;
using System.Collections.Generic;
using ConVar;
using Epic.OnlineServices.Reports;
using Facepunch;
using Facepunch.Rust;
using UnityEngine;

// Token: 0x0200073B RID: 1851
public static class AntiHack
{
	// Token: 0x04002A03 RID: 10755
	private const int movement_mask = 429990145;

	// Token: 0x04002A04 RID: 10756
	private const int vehicle_mask = 8192;

	// Token: 0x04002A05 RID: 10757
	private const int grounded_mask = 1503731969;

	// Token: 0x04002A06 RID: 10758
	private const int player_mask = 131072;

	// Token: 0x04002A07 RID: 10759
	private static Collider[] buffer = new Collider[4];

	// Token: 0x04002A08 RID: 10760
	private static Dictionary<ulong, int> kicks = new Dictionary<ulong, int>();

	// Token: 0x04002A09 RID: 10761
	private static Dictionary<ulong, int> bans = new Dictionary<ulong, int>();

	// Token: 0x04002A0A RID: 10762
	private const float LOG_GROUP_SECONDS = 60f;

	// Token: 0x04002A0B RID: 10763
	private static Queue<global::AntiHack.GroupedLog> groupedLogs = new Queue<global::AntiHack.GroupedLog>();

	// Token: 0x04002A0C RID: 10764
	public static RaycastHit isInsideRayHit;

	// Token: 0x0600337B RID: 13179 RVA: 0x0013C300 File Offset: 0x0013A500
	public static bool TestNoClipping(Vector3 oldPos, Vector3 newPos, float radius, float backtracking, bool sphereCast, out Collider collider, bool vehicleLayer = false, BaseEntity ignoreEntity = null)
	{
		int num = 429990145;
		if (!vehicleLayer)
		{
			num &= -8193;
		}
		Vector3 normalized = (newPos - oldPos).normalized;
		Vector3 vector = oldPos - normalized * backtracking;
		float magnitude = (newPos - vector).magnitude;
		Ray ray = new Ray(vector, normalized);
		RaycastHit hitInfo;
		bool flag = (ignoreEntity == null) ? UnityEngine.Physics.Raycast(ray, out hitInfo, magnitude + radius, num, QueryTriggerInteraction.Ignore) : GamePhysics.Trace(ray, 0f, out hitInfo, magnitude + radius, num, QueryTriggerInteraction.Ignore, ignoreEntity);
		if (!flag && sphereCast)
		{
			flag = ((ignoreEntity == null) ? UnityEngine.Physics.SphereCast(ray, radius, out hitInfo, magnitude, num, QueryTriggerInteraction.Ignore) : GamePhysics.Trace(ray, radius, out hitInfo, magnitude, num, QueryTriggerInteraction.Ignore, ignoreEntity));
		}
		collider = hitInfo.collider;
		return flag && GamePhysics.Verify(hitInfo, null);
	}

	// Token: 0x0600337C RID: 13180 RVA: 0x0013C3D8 File Offset: 0x0013A5D8
	public static void Cycle()
	{
		float num = UnityEngine.Time.unscaledTime - 60f;
		if (global::AntiHack.groupedLogs.Count > 0)
		{
			global::AntiHack.GroupedLog groupedLog = global::AntiHack.groupedLogs.Peek();
			while (groupedLog.firstLogTime <= num)
			{
				global::AntiHack.GroupedLog groupedLog2 = global::AntiHack.groupedLogs.Dequeue();
				global::AntiHack.LogToConsole(groupedLog2.playerName, groupedLog2.antiHackType, string.Format("{0} (x{1})", groupedLog2.message, groupedLog2.num), groupedLog2.averagePos);
				Facepunch.Pool.Free<global::AntiHack.GroupedLog>(ref groupedLog2);
				if (global::AntiHack.groupedLogs.Count == 0)
				{
					break;
				}
				groupedLog = global::AntiHack.groupedLogs.Peek();
			}
		}
	}

	// Token: 0x0600337D RID: 13181 RVA: 0x0013C46F File Offset: 0x0013A66F
	public static void ResetTimer(BasePlayer ply)
	{
		ply.lastViolationTime = UnityEngine.Time.realtimeSinceStartup;
	}

	// Token: 0x0600337E RID: 13182 RVA: 0x0013C47C File Offset: 0x0013A67C
	public static bool ShouldIgnore(BasePlayer ply)
	{
		bool result;
		using (TimeWarning.New("AntiHack.ShouldIgnore", 0))
		{
			if (ply.IsFlying)
			{
				ply.lastAdminCheatTime = UnityEngine.Time.realtimeSinceStartup;
			}
			else if ((ply.IsAdmin || ply.IsDeveloper) && ply.lastAdminCheatTime == 0f)
			{
				ply.lastAdminCheatTime = UnityEngine.Time.realtimeSinceStartup;
			}
			if (ply.IsAdmin)
			{
				if (ConVar.AntiHack.userlevel < 1)
				{
					return true;
				}
				if (ConVar.AntiHack.admincheat && ply.UsedAdminCheat(2f))
				{
					return true;
				}
			}
			if (ply.IsDeveloper)
			{
				if (ConVar.AntiHack.userlevel < 2)
				{
					return true;
				}
				if (ConVar.AntiHack.admincheat && ply.UsedAdminCheat(2f))
				{
					return true;
				}
			}
			if (ply.IsSpectating())
			{
				result = true;
			}
			else
			{
				result = false;
			}
		}
		return result;
	}

	// Token: 0x0600337F RID: 13183 RVA: 0x0013C558 File Offset: 0x0013A758
	public static bool ValidateMove(BasePlayer ply, TickInterpolator ticks, float deltaTime)
	{
		bool result;
		using (TimeWarning.New("AntiHack.ValidateMove", 0))
		{
			if (global::AntiHack.ShouldIgnore(ply))
			{
				result = true;
			}
			else
			{
				bool flag = deltaTime > ConVar.AntiHack.maxdeltatime;
				Collider collider;
				if (global::AntiHack.IsNoClipping(ply, ticks, deltaTime, out collider))
				{
					if (flag)
					{
						return false;
					}
					Analytics.Azure.OnNoclipViolation(ply, ticks.CurrentPoint, ticks.EndPoint, ticks.Count, collider);
					global::AntiHack.AddViolation(ply, AntiHackType.NoClip, ConVar.AntiHack.noclip_penalty * ticks.Length);
					if (ConVar.AntiHack.noclip_reject)
					{
						return false;
					}
				}
				if (global::AntiHack.IsSpeeding(ply, ticks, deltaTime))
				{
					if (flag)
					{
						return false;
					}
					Analytics.Azure.OnSpeedhackViolation(ply, ticks.CurrentPoint, ticks.EndPoint, ticks.Count);
					global::AntiHack.AddViolation(ply, AntiHackType.SpeedHack, ConVar.AntiHack.speedhack_penalty * ticks.Length);
					if (ConVar.AntiHack.speedhack_reject)
					{
						return false;
					}
				}
				if (global::AntiHack.IsFlying(ply, ticks, deltaTime))
				{
					if (flag)
					{
						return false;
					}
					Analytics.Azure.OnFlyhackViolation(ply, ticks.CurrentPoint, ticks.EndPoint, ticks.Count);
					global::AntiHack.AddViolation(ply, AntiHackType.FlyHack, ConVar.AntiHack.flyhack_penalty * ticks.Length);
					if (ConVar.AntiHack.flyhack_reject)
					{
						return false;
					}
				}
				result = true;
			}
		}
		return result;
	}

	// Token: 0x06003380 RID: 13184 RVA: 0x0013C68C File Offset: 0x0013A88C
	public static void ValidateEyeHistory(BasePlayer ply)
	{
		using (TimeWarning.New("AntiHack.ValidateEyeHistory", 0))
		{
			for (int i = 0; i < ply.eyeHistory.Count; i++)
			{
				Vector3 vector = ply.eyeHistory[i];
				if (ply.tickHistory.Distance(ply, vector) > ConVar.AntiHack.eye_history_forgiveness)
				{
					global::AntiHack.AddViolation(ply, AntiHackType.EyeHack, ConVar.AntiHack.eye_history_penalty);
					Analytics.Azure.OnEyehackViolation(ply, vector);
				}
			}
			ply.eyeHistory.Clear();
		}
	}

	// Token: 0x06003381 RID: 13185 RVA: 0x0013C718 File Offset: 0x0013A918
	public static bool IsInsideTerrain(BasePlayer ply)
	{
		return global::AntiHack.TestInsideTerrain(ply.transform.position);
	}

	// Token: 0x06003382 RID: 13186 RVA: 0x0013C72C File Offset: 0x0013A92C
	public static bool TestInsideTerrain(Vector3 pos)
	{
		bool result;
		using (TimeWarning.New("AntiHack.TestInsideTerrain", 0))
		{
			if (!TerrainMeta.Terrain)
			{
				result = false;
			}
			else if (!TerrainMeta.HeightMap)
			{
				result = false;
			}
			else if (!TerrainMeta.Collision)
			{
				result = false;
			}
			else
			{
				float terrain_padding = ConVar.AntiHack.terrain_padding;
				float height = TerrainMeta.HeightMap.GetHeight(pos);
				if (pos.y > height - terrain_padding)
				{
					result = false;
				}
				else
				{
					float num = TerrainMeta.Position.y + TerrainMeta.Terrain.SampleHeight(pos);
					if (pos.y > num - terrain_padding)
					{
						result = false;
					}
					else if (TerrainMeta.Collision.GetIgnore(pos, 0.01f))
					{
						result = false;
					}
					else
					{
						result = true;
					}
				}
			}
		}
		return result;
	}

	// Token: 0x06003383 RID: 13187 RVA: 0x0013C800 File Offset: 0x0013AA00
	public static bool IsInsideMesh(Vector3 pos)
	{
		bool queriesHitBackfaces = UnityEngine.Physics.queriesHitBackfaces;
		UnityEngine.Physics.queriesHitBackfaces = true;
		if (UnityEngine.Physics.Raycast(pos, Vector3.up, out global::AntiHack.isInsideRayHit, 50f, 65537))
		{
			UnityEngine.Physics.queriesHitBackfaces = queriesHitBackfaces;
			return Vector3.Dot(Vector3.up, global::AntiHack.isInsideRayHit.normal) > 0f;
		}
		UnityEngine.Physics.queriesHitBackfaces = queriesHitBackfaces;
		return false;
	}

	// Token: 0x06003384 RID: 13188 RVA: 0x0013C860 File Offset: 0x0013AA60
	public static bool IsNoClipping(BasePlayer ply, TickInterpolator ticks, float deltaTime, out Collider collider)
	{
		collider = null;
		bool result;
		using (TimeWarning.New("AntiHack.IsNoClipping", 0))
		{
			ply.vehiclePauseTime = Mathf.Max(0f, ply.vehiclePauseTime - deltaTime);
			if (ConVar.AntiHack.noclip_protection <= 0)
			{
				result = false;
			}
			else
			{
				ticks.Reset();
				if (!ticks.HasNext())
				{
					result = false;
				}
				else
				{
					bool flag = ply.transform.parent == null;
					Matrix4x4 matrix4x = flag ? Matrix4x4.identity : ply.transform.parent.localToWorldMatrix;
					Vector3 a = flag ? ticks.StartPoint : matrix4x.MultiplyPoint3x4(ticks.StartPoint);
					Vector3 vector = flag ? ticks.EndPoint : matrix4x.MultiplyPoint3x4(ticks.EndPoint);
					Vector3 b = ply.NoClipOffset();
					float radius = ply.NoClipRadius(ConVar.AntiHack.noclip_margin);
					float noclip_backtracking = ConVar.AntiHack.noclip_backtracking;
					bool vehicleLayer = ply.vehiclePauseTime <= 0f;
					if (ConVar.AntiHack.noclip_protection >= 3)
					{
						float num = Mathf.Max(ConVar.AntiHack.noclip_stepsize, 0.1f);
						int num2 = Mathf.Max(ConVar.AntiHack.noclip_maxsteps, 1);
						num = Mathf.Max(ticks.Length / (float)num2, num);
						while (ticks.MoveNext(num))
						{
							vector = (flag ? ticks.CurrentPoint : matrix4x.MultiplyPoint3x4(ticks.CurrentPoint));
							if (global::AntiHack.TestNoClipping(a + b, vector + b, radius, noclip_backtracking, true, out collider, vehicleLayer, null))
							{
								return true;
							}
							a = vector;
						}
					}
					else if (ConVar.AntiHack.noclip_protection >= 2)
					{
						if (global::AntiHack.TestNoClipping(a + b, vector + b, radius, noclip_backtracking, true, out collider, vehicleLayer, null))
						{
							return true;
						}
					}
					else if (global::AntiHack.TestNoClipping(a + b, vector + b, radius, noclip_backtracking, false, out collider, vehicleLayer, null))
					{
						return true;
					}
					result = false;
				}
			}
		}
		return result;
	}

	// Token: 0x06003385 RID: 13189 RVA: 0x0013CA58 File Offset: 0x0013AC58
	public static bool IsSpeeding(BasePlayer ply, TickInterpolator ticks, float deltaTime)
	{
		bool result;
		using (TimeWarning.New("AntiHack.IsSpeeding", 0))
		{
			ply.speedhackPauseTime = Mathf.Max(0f, ply.speedhackPauseTime - deltaTime);
			if (ConVar.AntiHack.speedhack_protection <= 0)
			{
				result = false;
			}
			else
			{
				bool flag = ply.transform.parent == null;
				Matrix4x4 matrix4x = flag ? Matrix4x4.identity : ply.transform.parent.localToWorldMatrix;
				Vector3 vector = flag ? ticks.StartPoint : matrix4x.MultiplyPoint3x4(ticks.StartPoint);
				Vector3 a = flag ? ticks.EndPoint : matrix4x.MultiplyPoint3x4(ticks.EndPoint);
				float running = 1f;
				float ducking = 0f;
				float crawling = 0f;
				if (ConVar.AntiHack.speedhack_protection >= 2)
				{
					bool flag2 = ply.IsRunning();
					bool flag3 = ply.IsDucked();
					bool flag4 = ply.IsSwimming();
					bool flag5 = ply.IsCrawling();
					running = (flag2 ? 1f : 0f);
					ducking = ((flag3 || flag4) ? 1f : 0f);
					crawling = (flag5 ? 1f : 0f);
				}
				float speed = ply.GetSpeed(running, ducking, crawling);
				Vector3 v = a - vector;
				float num = v.Magnitude2D();
				float num2 = deltaTime * speed;
				if (num > num2)
				{
					Vector3 v2 = TerrainMeta.HeightMap ? TerrainMeta.HeightMap.GetNormal(vector) : Vector3.up;
					float num3 = Mathf.Max(0f, Vector3.Dot(v2.XZ3D(), v.XZ3D())) * ConVar.AntiHack.speedhack_slopespeed * deltaTime;
					num = Mathf.Max(0f, num - num3);
				}
				float num4 = Mathf.Max((ply.speedhackPauseTime > 0f) ? ConVar.AntiHack.speedhack_forgiveness_inertia : ConVar.AntiHack.speedhack_forgiveness, 0.1f);
				float num5 = num4 + Mathf.Max(ConVar.AntiHack.speedhack_forgiveness, 0.1f);
				ply.speedhackDistance = Mathf.Clamp(ply.speedhackDistance, -num5, num5);
				ply.speedhackDistance = Mathf.Clamp(ply.speedhackDistance - num2, -num5, num5);
				if (ply.speedhackDistance > num4)
				{
					result = true;
				}
				else
				{
					ply.speedhackDistance = Mathf.Clamp(ply.speedhackDistance + num, -num5, num5);
					if (ply.speedhackDistance > num4)
					{
						result = true;
					}
					else
					{
						result = false;
					}
				}
			}
		}
		return result;
	}

	// Token: 0x06003386 RID: 13190 RVA: 0x0013CCB4 File Offset: 0x0013AEB4
	public static bool IsFlying(BasePlayer ply, TickInterpolator ticks, float deltaTime)
	{
		bool result;
		using (TimeWarning.New("AntiHack.IsFlying", 0))
		{
			ply.flyhackPauseTime = Mathf.Max(0f, ply.flyhackPauseTime - deltaTime);
			if (ConVar.AntiHack.flyhack_protection <= 0)
			{
				result = false;
			}
			else
			{
				ticks.Reset();
				if (!ticks.HasNext())
				{
					result = false;
				}
				else
				{
					bool flag = ply.transform.parent == null;
					Matrix4x4 matrix4x = flag ? Matrix4x4.identity : ply.transform.parent.localToWorldMatrix;
					Vector3 oldPos = flag ? ticks.StartPoint : matrix4x.MultiplyPoint3x4(ticks.StartPoint);
					Vector3 vector = flag ? ticks.EndPoint : matrix4x.MultiplyPoint3x4(ticks.EndPoint);
					if (ConVar.AntiHack.flyhack_protection >= 3)
					{
						float num = Mathf.Max(ConVar.AntiHack.flyhack_stepsize, 0.1f);
						int num2 = Mathf.Max(ConVar.AntiHack.flyhack_maxsteps, 1);
						num = Mathf.Max(ticks.Length / (float)num2, num);
						while (ticks.MoveNext(num))
						{
							vector = (flag ? ticks.CurrentPoint : matrix4x.MultiplyPoint3x4(ticks.CurrentPoint));
							if (global::AntiHack.TestFlying(ply, oldPos, vector, true))
							{
								return true;
							}
							oldPos = vector;
						}
					}
					else if (ConVar.AntiHack.flyhack_protection >= 2)
					{
						if (global::AntiHack.TestFlying(ply, oldPos, vector, true))
						{
							return true;
						}
					}
					else if (global::AntiHack.TestFlying(ply, oldPos, vector, false))
					{
						return true;
					}
					result = false;
				}
			}
		}
		return result;
	}

	// Token: 0x06003387 RID: 13191 RVA: 0x0013CE38 File Offset: 0x0013B038
	public static bool TestFlying(BasePlayer ply, Vector3 oldPos, Vector3 newPos, bool verifyGrounded)
	{
		ply.isInAir = false;
		ply.isOnPlayer = false;
		if (verifyGrounded)
		{
			float flyhack_extrusion = ConVar.AntiHack.flyhack_extrusion;
			Vector3 vector = (oldPos + newPos) * 0.5f;
			if (!ply.OnLadder() && !WaterLevel.Test(vector - new Vector3(0f, flyhack_extrusion, 0f), true, ply) && (EnvironmentManager.Get(vector) & EnvironmentType.Elevator) == (EnvironmentType)0)
			{
				float flyhack_margin = ConVar.AntiHack.flyhack_margin;
				float radius = ply.GetRadius();
				float height = ply.GetHeight(false);
				Vector3 vector2 = vector + new Vector3(0f, radius - flyhack_extrusion, 0f);
				Vector3 vector3 = vector + new Vector3(0f, height - radius, 0f);
				float radius2 = radius - flyhack_margin;
				ply.isInAir = !UnityEngine.Physics.CheckCapsule(vector2, vector3, radius2, 1503731969, QueryTriggerInteraction.Ignore);
				if (ply.isInAir)
				{
					int num = UnityEngine.Physics.OverlapCapsuleNonAlloc(vector2, vector3, radius2, global::AntiHack.buffer, 131072, QueryTriggerInteraction.Ignore);
					for (int i = 0; i < num; i++)
					{
						BasePlayer basePlayer = global::AntiHack.buffer[i].gameObject.ToBaseEntity() as BasePlayer;
						if (!(basePlayer == null) && !(basePlayer == ply) && !basePlayer.isInAir && !basePlayer.isOnPlayer && !basePlayer.TriggeredAntiHack(1f, float.PositiveInfinity) && !basePlayer.IsSleeping())
						{
							ply.isOnPlayer = true;
							ply.isInAir = false;
							break;
						}
					}
					for (int j = 0; j < global::AntiHack.buffer.Length; j++)
					{
						global::AntiHack.buffer[j] = null;
					}
				}
			}
		}
		else
		{
			ply.isInAir = (!ply.OnLadder() && !ply.IsSwimming() && !ply.IsOnGround());
		}
		if (ply.isInAir)
		{
			bool flag = false;
			Vector3 vector4 = newPos - oldPos;
			float num2 = Mathf.Abs(vector4.y);
			float num3 = vector4.Magnitude2D();
			if (vector4.y >= 0f)
			{
				ply.flyhackDistanceVertical += vector4.y;
				flag = true;
			}
			if (num2 < num3)
			{
				ply.flyhackDistanceHorizontal += num3;
				flag = true;
			}
			if (flag)
			{
				float num4 = Mathf.Max((ply.flyhackPauseTime > 0f) ? ConVar.AntiHack.flyhack_forgiveness_vertical_inertia : ConVar.AntiHack.flyhack_forgiveness_vertical, 0f);
				float num5 = ply.GetJumpHeight() + num4;
				if (ply.flyhackDistanceVertical > num5)
				{
					return true;
				}
				float num6 = Mathf.Max((ply.flyhackPauseTime > 0f) ? ConVar.AntiHack.flyhack_forgiveness_horizontal_inertia : ConVar.AntiHack.flyhack_forgiveness_horizontal, 0f);
				float num7 = 5f + num6;
				if (ply.flyhackDistanceHorizontal > num7)
				{
					return true;
				}
			}
		}
		else
		{
			ply.flyhackDistanceVertical = 0f;
			ply.flyhackDistanceHorizontal = 0f;
		}
		return false;
	}

	// Token: 0x06003388 RID: 13192 RVA: 0x0013D0F4 File Offset: 0x0013B2F4
	public static bool TestIsBuildingInsideSomething(Construction.Target target, Vector3 deployPos)
	{
		if (ConVar.AntiHack.build_inside_check <= 0)
		{
			return false;
		}
		using (List<MonumentInfo>.Enumerator enumerator = TerrainMeta.Path.Monuments.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.IsInBounds(deployPos))
				{
					return false;
				}
			}
		}
		if (global::AntiHack.IsInsideMesh(deployPos) && global::AntiHack.IsInsideMesh(target.ray.origin))
		{
			global::AntiHack.LogToConsoleBatched(target.player, AntiHackType.InsideGeometry, "Tried to build while clipped inside " + global::AntiHack.isInsideRayHit.collider.name, 25f);
			if (ConVar.AntiHack.build_inside_check > 1)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06003389 RID: 13193 RVA: 0x0013D1AC File Offset: 0x0013B3AC
	public static void NoteAdminHack(BasePlayer ply)
	{
		global::AntiHack.Ban(ply, "Cheat Detected!");
	}

	// Token: 0x0600338A RID: 13194 RVA: 0x0013D1B9 File Offset: 0x0013B3B9
	public static void FadeViolations(BasePlayer ply, float deltaTime)
	{
		if (UnityEngine.Time.realtimeSinceStartup - ply.lastViolationTime > ConVar.AntiHack.relaxationpause)
		{
			ply.violationLevel = Mathf.Max(0f, ply.violationLevel - ConVar.AntiHack.relaxationrate * deltaTime);
		}
	}

	// Token: 0x0600338B RID: 13195 RVA: 0x0013D1EC File Offset: 0x0013B3EC
	public static void EnforceViolations(BasePlayer ply)
	{
		if (ConVar.AntiHack.enforcementlevel <= 0)
		{
			return;
		}
		if (ply.violationLevel > ConVar.AntiHack.maxviolation)
		{
			if (ConVar.AntiHack.debuglevel >= 1)
			{
				global::AntiHack.LogToConsole(ply, ply.lastViolationType, "Enforcing (violation of " + ply.violationLevel + ")");
			}
			string reason = ply.lastViolationType + " Violation Level " + ply.violationLevel;
			if (ConVar.AntiHack.enforcementlevel > 1)
			{
				global::AntiHack.Kick(ply, reason);
				return;
			}
			global::AntiHack.Kick(ply, reason);
		}
	}

	// Token: 0x0600338C RID: 13196 RVA: 0x0013D275 File Offset: 0x0013B475
	public static void Log(BasePlayer ply, AntiHackType type, string message)
	{
		if (ConVar.AntiHack.debuglevel > 1)
		{
			global::AntiHack.LogToConsole(ply, type, message);
		}
		Analytics.Azure.OnAntihackViolation(ply, (int)type, message);
		global::AntiHack.LogToEAC(ply, type, message);
	}

	// Token: 0x0600338D RID: 13197 RVA: 0x0013D298 File Offset: 0x0013B498
	public static void LogToConsoleBatched(BasePlayer ply, AntiHackType type, string message, float maxDistance)
	{
		string playerName = ply.ToString();
		Vector3 position = ply.transform.position;
		using (Queue<global::AntiHack.GroupedLog>.Enumerator enumerator = global::AntiHack.groupedLogs.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.TryGroup(playerName, type, message, position, maxDistance))
				{
					return;
				}
			}
		}
		global::AntiHack.GroupedLog groupedLog = Facepunch.Pool.Get<global::AntiHack.GroupedLog>();
		groupedLog.SetInitial(playerName, type, message, position);
		global::AntiHack.groupedLogs.Enqueue(groupedLog);
	}

	// Token: 0x0600338E RID: 13198 RVA: 0x0013D320 File Offset: 0x0013B520
	private static void LogToConsole(BasePlayer ply, AntiHackType type, string message)
	{
		Debug.LogWarning(string.Concat(new object[]
		{
			ply,
			" ",
			type,
			": ",
			message,
			" at ",
			ply.transform.position
		}));
	}

	// Token: 0x0600338F RID: 13199 RVA: 0x0013D37C File Offset: 0x0013B57C
	private static void LogToConsole(string plyName, AntiHackType type, string message, Vector3 pos)
	{
		Debug.LogWarning(string.Concat(new object[]
		{
			plyName,
			" ",
			type,
			": ",
			message,
			" at ",
			pos
		}));
	}

	// Token: 0x06003390 RID: 13200 RVA: 0x0013D3CB File Offset: 0x0013B5CB
	private static void LogToEAC(BasePlayer ply, AntiHackType type, string message)
	{
		if (ConVar.AntiHack.reporting)
		{
			EACServer.SendPlayerBehaviorReport(PlayerReportsCategory.Exploiting, ply.UserIDString, type + ": " + message);
		}
	}

	// Token: 0x06003391 RID: 13201 RVA: 0x0013D3F4 File Offset: 0x0013B5F4
	public static void AddViolation(BasePlayer ply, AntiHackType type, float amount)
	{
		using (TimeWarning.New("AntiHack.AddViolation", 0))
		{
			ply.lastViolationType = type;
			ply.lastViolationTime = UnityEngine.Time.realtimeSinceStartup;
			ply.violationLevel += amount;
			if ((ConVar.AntiHack.debuglevel >= 2 && amount > 0f) || (ConVar.AntiHack.debuglevel >= 3 && type != AntiHackType.NoClip) || ConVar.AntiHack.debuglevel >= 4)
			{
				global::AntiHack.LogToConsole(ply, type, string.Concat(new object[]
				{
					"Added violation of ",
					amount,
					" in frame ",
					UnityEngine.Time.frameCount,
					" (now has ",
					ply.violationLevel,
					")"
				}));
			}
			global::AntiHack.EnforceViolations(ply);
		}
	}

	// Token: 0x06003392 RID: 13202 RVA: 0x0013D4CC File Offset: 0x0013B6CC
	public static void Kick(BasePlayer ply, string reason)
	{
		global::AntiHack.AddRecord(ply, global::AntiHack.kicks);
		ConsoleSystem.Run(ConsoleSystem.Option.Server, "kick", new object[]
		{
			ply.userID,
			reason
		});
	}

	// Token: 0x06003393 RID: 13203 RVA: 0x0013D501 File Offset: 0x0013B701
	public static void Ban(BasePlayer ply, string reason)
	{
		global::AntiHack.AddRecord(ply, global::AntiHack.bans);
		ConsoleSystem.Run(ConsoleSystem.Option.Server, "ban", new object[]
		{
			ply.userID,
			reason
		});
	}

	// Token: 0x06003394 RID: 13204 RVA: 0x0013D538 File Offset: 0x0013B738
	private static void AddRecord(BasePlayer ply, Dictionary<ulong, int> records)
	{
		if (records.ContainsKey(ply.userID))
		{
			ulong userID = ply.userID;
			records[userID]++;
			return;
		}
		records.Add(ply.userID, 1);
	}

	// Token: 0x06003395 RID: 13205 RVA: 0x0013D57A File Offset: 0x0013B77A
	public static int GetKickRecord(BasePlayer ply)
	{
		return global::AntiHack.GetRecord(ply, global::AntiHack.kicks);
	}

	// Token: 0x06003396 RID: 13206 RVA: 0x0013D587 File Offset: 0x0013B787
	public static int GetBanRecord(BasePlayer ply)
	{
		return global::AntiHack.GetRecord(ply, global::AntiHack.bans);
	}

	// Token: 0x06003397 RID: 13207 RVA: 0x0013D594 File Offset: 0x0013B794
	private static int GetRecord(BasePlayer ply, Dictionary<ulong, int> records)
	{
		if (!records.ContainsKey(ply.userID))
		{
			return 0;
		}
		return records[ply.userID];
	}

	// Token: 0x02000E43 RID: 3651
	private class GroupedLog : Facepunch.Pool.IPooled
	{
		// Token: 0x04004ADC RID: 19164
		public float firstLogTime;

		// Token: 0x04004ADD RID: 19165
		public string playerName;

		// Token: 0x04004ADE RID: 19166
		public AntiHackType antiHackType;

		// Token: 0x04004ADF RID: 19167
		public string message;

		// Token: 0x04004AE0 RID: 19168
		public Vector3 averagePos;

		// Token: 0x04004AE1 RID: 19169
		public int num;

		// Token: 0x0600524B RID: 21067 RVA: 0x00008747 File Offset: 0x00006947
		public GroupedLog()
		{
		}

		// Token: 0x0600524C RID: 21068 RVA: 0x001AFD52 File Offset: 0x001ADF52
		public GroupedLog(string playerName, AntiHackType antiHackType, string message, Vector3 pos)
		{
			this.SetInitial(playerName, antiHackType, message, pos);
		}

		// Token: 0x0600524D RID: 21069 RVA: 0x001AFD65 File Offset: 0x001ADF65
		public void EnterPool()
		{
			this.firstLogTime = 0f;
			this.playerName = string.Empty;
			this.antiHackType = AntiHackType.None;
			this.averagePos = Vector3.zero;
			this.num = 0;
		}

		// Token: 0x0600524E RID: 21070 RVA: 0x000063A5 File Offset: 0x000045A5
		public void LeavePool()
		{
		}

		// Token: 0x0600524F RID: 21071 RVA: 0x001AFD96 File Offset: 0x001ADF96
		public void SetInitial(string playerName, AntiHackType antiHackType, string message, Vector3 pos)
		{
			this.firstLogTime = UnityEngine.Time.unscaledTime;
			this.playerName = playerName;
			this.antiHackType = antiHackType;
			this.message = message;
			this.averagePos = pos;
			this.num = 1;
		}

		// Token: 0x06005250 RID: 21072 RVA: 0x001AFDC8 File Offset: 0x001ADFC8
		public bool TryGroup(string playerName, AntiHackType antiHackType, string message, Vector3 pos, float maxDistance)
		{
			if (antiHackType != this.antiHackType || playerName != this.playerName || message != this.message)
			{
				return false;
			}
			if (Vector3.SqrMagnitude(this.averagePos - pos) > maxDistance * maxDistance)
			{
				return false;
			}
			Vector3 a = this.averagePos * (float)this.num;
			this.averagePos = (a + pos) / (float)(this.num + 1);
			this.num++;
			return true;
		}
	}
}
