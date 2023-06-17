using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;

namespace CompanionServer.Cameras
{
	// Token: 0x02000A0F RID: 2575
	public class CameraRenderer : Pool.IPooled
	{
		// Token: 0x04003726 RID: 14118
		[ServerVar]
		public static bool enabled = true;

		// Token: 0x04003727 RID: 14119
		[ServerVar]
		public static float completionFrameBudgetMs = 5f;

		// Token: 0x04003728 RID: 14120
		[ServerVar]
		public static int maxRendersPerFrame = 25;

		// Token: 0x04003729 RID: 14121
		[ServerVar]
		public static int maxRaysPerFrame = 100000;

		// Token: 0x0400372A RID: 14122
		[ServerVar]
		public static int width = 160;

		// Token: 0x0400372B RID: 14123
		[ServerVar]
		public static int height = 90;

		// Token: 0x0400372C RID: 14124
		[ServerVar]
		public static float verticalFov = 65f;

		// Token: 0x0400372D RID: 14125
		[ServerVar]
		public static float nearPlane = 0f;

		// Token: 0x0400372E RID: 14126
		[ServerVar]
		public static float farPlane = 250f;

		// Token: 0x0400372F RID: 14127
		[ServerVar]
		public static int layerMask = 1218656529;

		// Token: 0x04003730 RID: 14128
		[ServerVar]
		public static float renderInterval = 0.05f;

		// Token: 0x04003731 RID: 14129
		[ServerVar]
		public static int samplesPerRender = 3000;

		// Token: 0x04003732 RID: 14130
		[ServerVar]
		public static int entityMaxAge = 5;

		// Token: 0x04003733 RID: 14131
		[ServerVar]
		public static int entityMaxDistance = 100;

		// Token: 0x04003734 RID: 14132
		[ServerVar]
		public static int playerMaxDistance = 30;

		// Token: 0x04003735 RID: 14133
		[ServerVar]
		public static int playerNameMaxDistance = 10;

		// Token: 0x04003736 RID: 14134
		private static readonly Dictionary<NetworkableId, NetworkableId> _entityIdMap = new Dictionary<NetworkableId, NetworkableId>();

		// Token: 0x04003737 RID: 14135
		[TupleElementNames(new string[]
		{
			"MaterialIndex",
			"Age"
		})]
		private readonly Dictionary<int, ValueTuple<byte, int>> _knownColliders = new Dictionary<int, ValueTuple<byte, int>>();

		// Token: 0x04003738 RID: 14136
		private readonly Dictionary<int, global::BaseEntity> _colliderToEntity = new Dictionary<int, global::BaseEntity>();

		// Token: 0x04003739 RID: 14137
		private double _lastRenderTimestamp;

		// Token: 0x0400373A RID: 14138
		private float _fieldOfView;

		// Token: 0x0400373B RID: 14139
		private int _sampleOffset;

		// Token: 0x0400373C RID: 14140
		private int _nextSampleOffset;

		// Token: 0x0400373D RID: 14141
		private int _sampleCount;

		// Token: 0x0400373E RID: 14142
		private CameraRenderTask _task;

		// Token: 0x0400373F RID: 14143
		private ulong? _cachedViewerSteamId;

		// Token: 0x04003740 RID: 14144
		private global::BasePlayer _cachedViewer;

		// Token: 0x04003741 RID: 14145
		public CameraRendererState state;

		// Token: 0x04003742 RID: 14146
		public IRemoteControllable rc;

		// Token: 0x04003743 RID: 14147
		public global::BaseEntity entity;

		// Token: 0x06003D51 RID: 15697 RVA: 0x001685B7 File Offset: 0x001667B7
		public CameraRenderer()
		{
			this.Reset();
		}

		// Token: 0x06003D52 RID: 15698 RVA: 0x001685DB File Offset: 0x001667DB
		public void EnterPool()
		{
			this.Reset();
		}

		// Token: 0x06003D53 RID: 15699 RVA: 0x000063A5 File Offset: 0x000045A5
		public void LeavePool()
		{
		}

		// Token: 0x06003D54 RID: 15700 RVA: 0x001685E4 File Offset: 0x001667E4
		public void Reset()
		{
			this._knownColliders.Clear();
			this._colliderToEntity.Clear();
			this._lastRenderTimestamp = 0.0;
			this._fieldOfView = 0f;
			this._sampleOffset = 0;
			this._nextSampleOffset = 0;
			this._sampleCount = 0;
			if (this._task != null)
			{
				CameraRendererManager instance = SingletonComponent<CameraRendererManager>.Instance;
				if (instance != null)
				{
					instance.ReturnTask(ref this._task);
				}
			}
			this._cachedViewerSteamId = null;
			this._cachedViewer = null;
			this.state = CameraRendererState.Invalid;
			this.rc = null;
			this.entity = null;
		}

		// Token: 0x06003D55 RID: 15701 RVA: 0x00168684 File Offset: 0x00166884
		public void Init(IRemoteControllable remoteControllable)
		{
			if (remoteControllable == null)
			{
				throw new ArgumentNullException("remoteControllable");
			}
			this.rc = remoteControllable;
			this.entity = remoteControllable.GetEnt();
			if (this.entity == null || !this.entity.IsValid())
			{
				throw new ArgumentException("RemoteControllable's entity is null or invalid", "rc");
			}
			this.state = CameraRendererState.WaitingToRender;
		}

		// Token: 0x06003D56 RID: 15702 RVA: 0x001686E4 File Offset: 0x001668E4
		public bool CanRender()
		{
			return this.state == CameraRendererState.WaitingToRender && TimeEx.realtimeSinceStartup - this._lastRenderTimestamp >= (double)CameraRenderer.renderInterval;
		}

		// Token: 0x06003D57 RID: 15703 RVA: 0x00168708 File Offset: 0x00166908
		public void Render(int maxSampleCount)
		{
			CameraRendererManager instance = SingletonComponent<CameraRendererManager>.Instance;
			if (instance == null)
			{
				this.state = CameraRendererState.Invalid;
				return;
			}
			if (this.state != CameraRendererState.WaitingToRender)
			{
				throw new InvalidOperationException(string.Format("CameraRenderer cannot render in state {0}", this.state));
			}
			if (this.rc.IsUnityNull<IRemoteControllable>() || !this.entity.IsValid())
			{
				this.state = CameraRendererState.Invalid;
				return;
			}
			Transform eyes = this.rc.GetEyes();
			if (eyes == null)
			{
				this.state = CameraRendererState.Invalid;
				return;
			}
			if (this._task != null)
			{
				Debug.LogError("CameraRenderer: Trying to render but a task is already allocated?", this.entity);
				instance.ReturnTask(ref this._task);
			}
			this._fieldOfView = CameraRenderer.verticalFov / Mathf.Clamp(this.rc.GetFovScale(), 1f, 8f);
			this._sampleCount = Mathf.Clamp(CameraRenderer.samplesPerRender, 1, Mathf.Min(CameraRenderer.width * CameraRenderer.height, maxSampleCount));
			this._task = instance.BorrowTask();
			this._nextSampleOffset = this._task.Start(CameraRenderer.width, CameraRenderer.height, this._fieldOfView, CameraRenderer.nearPlane, CameraRenderer.farPlane, CameraRenderer.layerMask, eyes, this._sampleCount, this._sampleOffset, this._knownColliders);
			this.state = CameraRendererState.Rendering;
		}

		// Token: 0x06003D58 RID: 15704 RVA: 0x00168854 File Offset: 0x00166A54
		public void CompleteRender()
		{
			CameraRendererManager instance = SingletonComponent<CameraRendererManager>.Instance;
			if (instance == null)
			{
				this.state = CameraRendererState.Invalid;
				return;
			}
			if (this.state != CameraRendererState.Rendering)
			{
				throw new InvalidOperationException(string.Format("CameraRenderer cannot complete render in state {0}", this.state));
			}
			if (this._task == null)
			{
				Debug.LogError("CameraRenderer: Trying to complete render but no task is allocated?", this.entity);
				this.state = CameraRendererState.Invalid;
				return;
			}
			if (this._task.keepWaiting)
			{
				return;
			}
			if (this.rc.IsUnityNull<IRemoteControllable>() || !this.entity.IsValid())
			{
				instance.ReturnTask(ref this._task);
				this.state = CameraRendererState.Invalid;
				return;
			}
			Transform eyes = this.rc.GetEyes();
			if (eyes == null)
			{
				instance.ReturnTask(ref this._task);
				this.state = CameraRendererState.Invalid;
				return;
			}
			int minimumLength = this._sampleCount * 4;
			byte[] array = System.Buffers.ArrayPool<byte>.Shared.Rent(minimumLength);
			List<int> list = Pool.GetList<int>();
			List<int> list2 = Pool.GetList<int>();
			int count = this._task.ExtractRayData(array, list, list2);
			instance.ReturnTask(ref this._task);
			this.UpdateCollidersMap(list2);
			Pool.FreeList<int>(ref list);
			Pool.FreeList<int>(ref list2);
			CameraViewerId? cameraViewerId;
			ulong num = (this.rc.ControllingViewerId != null) ? cameraViewerId.GetValueOrDefault().SteamId : 0UL;
			if (num == 0UL)
			{
				this._cachedViewerSteamId = null;
				this._cachedViewer = null;
			}
			else
			{
				ulong num2 = num;
				ulong? cachedViewerSteamId = this._cachedViewerSteamId;
				if (!(num2 == cachedViewerSteamId.GetValueOrDefault() & cachedViewerSteamId != null))
				{
					this._cachedViewerSteamId = new ulong?(num);
					this._cachedViewer = (global::BasePlayer.FindByID(num) ?? global::BasePlayer.FindSleeping(num));
				}
			}
			float distance = this._cachedViewer.IsValid() ? Mathf.Clamp01(Vector3.Distance(this._cachedViewer.transform.position, this.entity.transform.position) / this.rc.MaxRange) : 0f;
			Vector3 position = eyes.position;
			Quaternion rotation = eyes.rotation;
			Matrix4x4 worldToLocalMatrix = eyes.worldToLocalMatrix;
			NetworkableId id = this.entity.net.ID;
			CameraRenderer._entityIdMap.Clear();
			AppBroadcast appBroadcast = Pool.Get<AppBroadcast>();
			appBroadcast.cameraRays = Pool.Get<AppCameraRays>();
			appBroadcast.cameraRays.verticalFov = this._fieldOfView;
			appBroadcast.cameraRays.sampleOffset = this._sampleOffset;
			appBroadcast.cameraRays.rayData = new ArraySegment<byte>(array, 0, count);
			appBroadcast.cameraRays.distance = distance;
			appBroadcast.cameraRays.entities = Pool.GetList<AppCameraRays.Entity>();
			appBroadcast.cameraRays.timeOfDay = ((TOD_Sky.Instance != null) ? TOD_Sky.Instance.LerpValue : 1f);
			foreach (global::BaseEntity baseEntity in this._colliderToEntity.Values)
			{
				if (baseEntity.IsValid())
				{
					Vector3 position2 = baseEntity.transform.position;
					float num3 = Vector3.Distance(position2, position);
					if (num3 <= (float)CameraRenderer.entityMaxDistance)
					{
						string name = null;
						global::BasePlayer basePlayer;
						if ((basePlayer = (baseEntity as global::BasePlayer)) != null)
						{
							if (num3 > (float)CameraRenderer.playerMaxDistance)
							{
								continue;
							}
							if (num3 <= (float)CameraRenderer.playerNameMaxDistance)
							{
								name = basePlayer.displayName;
							}
						}
						AppCameraRays.Entity entity = Pool.Get<AppCameraRays.Entity>();
						entity.entityId = CameraRenderer.RandomizeEntityId(baseEntity.net.ID);
						entity.type = ((baseEntity is TreeEntity) ? AppCameraRays.EntityType.Tree : AppCameraRays.EntityType.Player);
						entity.position = worldToLocalMatrix.MultiplyPoint3x4(position2);
						entity.rotation = (Quaternion.Inverse(baseEntity.transform.rotation) * rotation).eulerAngles * 0.017453292f;
						entity.size = Vector3.Scale(baseEntity.bounds.size, baseEntity.transform.localScale);
						entity.name = name;
						appBroadcast.cameraRays.entities.Add(entity);
					}
				}
			}
			appBroadcast.cameraRays.entities.Sort((AppCameraRays.Entity x, AppCameraRays.Entity y) => x.entityId.Value.CompareTo(y.entityId.Value));
			Server.Broadcast(new CameraTarget(id), appBroadcast);
			this._sampleOffset = this._nextSampleOffset;
			if (!Server.HasAnySubscribers(new CameraTarget(id)))
			{
				this.state = CameraRendererState.Invalid;
				return;
			}
			this._lastRenderTimestamp = TimeEx.realtimeSinceStartup;
			this.state = CameraRendererState.WaitingToRender;
		}

		// Token: 0x06003D59 RID: 15705 RVA: 0x00168CF0 File Offset: 0x00166EF0
		private void UpdateCollidersMap(List<int> foundColliderIds)
		{
			List<int> list = Pool.GetList<int>();
			foreach (int item in this._knownColliders.Keys)
			{
				list.Add(item);
			}
			List<int> list2 = Pool.GetList<int>();
			foreach (int num in list)
			{
				ValueTuple<byte, int> valueTuple;
				if (this._knownColliders.TryGetValue(num, out valueTuple))
				{
					if (valueTuple.Item2 > CameraRenderer.entityMaxAge)
					{
						list2.Add(num);
					}
					else
					{
						this._knownColliders[num] = new ValueTuple<byte, int>(valueTuple.Item1, valueTuple.Item2 + 1);
					}
				}
			}
			Pool.FreeList<int>(ref list);
			foreach (int key in list2)
			{
				this._knownColliders.Remove(key);
				this._colliderToEntity.Remove(key);
			}
			Pool.FreeList<int>(ref list2);
			foreach (int num2 in foundColliderIds)
			{
				if (this._knownColliders.Count >= 512)
				{
					break;
				}
				Collider collider = BurstUtil.GetCollider(num2);
				if (!(collider == null))
				{
					byte item2;
					if (collider is TerrainCollider)
					{
						item2 = 1;
					}
					else
					{
						global::BaseEntity baseEntity = collider.ToBaseEntity();
						item2 = CameraRenderer.GetMaterialIndex(collider.sharedMaterial, baseEntity);
						if (baseEntity is TreeEntity || baseEntity is global::BasePlayer)
						{
							this._colliderToEntity[num2] = baseEntity;
						}
					}
					this._knownColliders[num2] = new ValueTuple<byte, int>(item2, 0);
				}
			}
		}

		// Token: 0x06003D5A RID: 15706 RVA: 0x00168F00 File Offset: 0x00167100
		private static NetworkableId RandomizeEntityId(NetworkableId realId)
		{
			NetworkableId result;
			if (CameraRenderer._entityIdMap.TryGetValue(realId, out result))
			{
				return result;
			}
			NetworkableId networkableId;
			do
			{
				networkableId = new NetworkableId((ulong)((long)UnityEngine.Random.Range(0, 2500)));
			}
			while (CameraRenderer._entityIdMap.ContainsKey(networkableId));
			CameraRenderer._entityIdMap.Add(realId, networkableId);
			return networkableId;
		}

		// Token: 0x06003D5B RID: 15707 RVA: 0x00168F4C File Offset: 0x0016714C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static byte GetMaterialIndex(PhysicMaterial material, global::BaseEntity entity)
		{
			string name = material.GetName();
			if (name == "Water")
			{
				return 2;
			}
			if (name == "Rock")
			{
				return 3;
			}
			if (name == "Stones")
			{
				return 4;
			}
			if (name == "Wood")
			{
				return 5;
			}
			if (name == "Metal")
			{
				return 6;
			}
			if (entity != null && entity is global::BasePlayer)
			{
				return 7;
			}
			return 0;
		}
	}
}
