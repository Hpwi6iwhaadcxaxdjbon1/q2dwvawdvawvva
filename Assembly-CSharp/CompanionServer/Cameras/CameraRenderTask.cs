using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace CompanionServer.Cameras
{
	// Token: 0x02000A06 RID: 2566
	public class CameraRenderTask : CustomYieldInstruction, IDisposable
	{
		// Token: 0x040036F2 RID: 14066
		public const int MaxSamplesPerRender = 10000;

		// Token: 0x040036F3 RID: 14067
		public const int MaxColliders = 512;

		// Token: 0x040036F4 RID: 14068
		private static readonly Dictionary<ValueTuple<int, int>, NativeArray<int2>> _samplePositions = new Dictionary<ValueTuple<int, int>, NativeArray<int2>>();

		// Token: 0x040036F5 RID: 14069
		private NativeArray<RaycastCommand> _raycastCommands;

		// Token: 0x040036F6 RID: 14070
		private NativeArray<RaycastHit> _raycastHits;

		// Token: 0x040036F7 RID: 14071
		private NativeArray<int> _colliderIds;

		// Token: 0x040036F8 RID: 14072
		private NativeArray<byte> _colliderMaterials;

		// Token: 0x040036F9 RID: 14073
		private NativeArray<int> _colliderHits;

		// Token: 0x040036FA RID: 14074
		private NativeArray<int> _raycastOutput;

		// Token: 0x040036FB RID: 14075
		private NativeArray<int> _foundCollidersLength;

		// Token: 0x040036FC RID: 14076
		private NativeArray<int> _foundColliders;

		// Token: 0x040036FD RID: 14077
		private NativeArray<int> _outputDataLength;

		// Token: 0x040036FE RID: 14078
		private NativeArray<byte> _outputData;

		// Token: 0x040036FF RID: 14079
		private JobHandle? _pendingJob;

		// Token: 0x04003700 RID: 14080
		private int _sampleCount;

		// Token: 0x04003701 RID: 14081
		private int _colliderLength;

		// Token: 0x06003D35 RID: 15669 RVA: 0x001673F8 File Offset: 0x001655F8
		public CameraRenderTask()
		{
			this._raycastCommands = new NativeArray<RaycastCommand>(10000, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this._raycastHits = new NativeArray<RaycastHit>(10000, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
			this._colliderIds = new NativeArray<int>(512, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
			this._colliderMaterials = new NativeArray<byte>(512, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
			this._colliderHits = new NativeArray<int>(512, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
			this._raycastOutput = new NativeArray<int>(10000, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
			this._foundCollidersLength = new NativeArray<int>(1, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
			this._foundColliders = new NativeArray<int>(10000, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
			this._outputDataLength = new NativeArray<int>(1, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
			this._outputData = new NativeArray<byte>(40000, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
		}

		// Token: 0x06003D36 RID: 15670 RVA: 0x001674B8 File Offset: 0x001656B8
		~CameraRenderTask()
		{
			this.Dispose();
		}

		// Token: 0x06003D37 RID: 15671 RVA: 0x001674E4 File Offset: 0x001656E4
		public void Dispose()
		{
			this._raycastCommands.Dispose();
			this._raycastHits.Dispose();
			this._colliderIds.Dispose();
			this._colliderMaterials.Dispose();
			this._colliderHits.Dispose();
			this._raycastOutput.Dispose();
			this._foundCollidersLength.Dispose();
			this._foundColliders.Dispose();
			this._outputDataLength.Dispose();
			this._outputData.Dispose();
		}

		// Token: 0x06003D38 RID: 15672 RVA: 0x00167560 File Offset: 0x00165760
		public new void Reset()
		{
			if (this._pendingJob != null)
			{
				if (!this._pendingJob.Value.IsCompleted)
				{
					Debug.LogWarning("CameraRenderTask is resetting before completion! This will cause it to synchronously block for completion.");
				}
				this._pendingJob.Value.Complete();
			}
			this._pendingJob = null;
			this._sampleCount = 0;
		}

		// Token: 0x17000509 RID: 1289
		// (get) Token: 0x06003D39 RID: 15673 RVA: 0x001675C0 File Offset: 0x001657C0
		public override bool keepWaiting
		{
			get
			{
				return this._pendingJob != null && !this._pendingJob.Value.IsCompleted;
			}
		}

		// Token: 0x06003D3A RID: 15674 RVA: 0x001675F4 File Offset: 0x001657F4
		public int Start(int width, int height, float verticalFov, float nearPlane, float farPlane, int layerMask, Transform cameraTransform, int sampleCount, int sampleOffset, [TupleElementNames(new string[]
		{
			"MaterialIndex",
			"Age"
		})] Dictionary<int, ValueTuple<byte, int>> knownColliders)
		{
			if (cameraTransform == null)
			{
				throw new ArgumentNullException("cameraTransform");
			}
			if (sampleCount <= 0 || sampleCount > 10000)
			{
				throw new ArgumentOutOfRangeException("sampleCount");
			}
			if (sampleOffset < 0)
			{
				throw new ArgumentOutOfRangeException("sampleOffset");
			}
			if (knownColliders == null)
			{
				throw new ArgumentNullException("knownColliders");
			}
			if (knownColliders.Count > 512)
			{
				throw new ArgumentException("Too many colliders", "knownColliders");
			}
			if (this._pendingJob != null)
			{
				throw new InvalidOperationException("A render job was already started for this instance.");
			}
			this._sampleCount = sampleCount;
			this._colliderLength = knownColliders.Count;
			int num = 0;
			foreach (KeyValuePair<int, ValueTuple<byte, int>> keyValuePair in knownColliders)
			{
				this._colliderIds[num] = keyValuePair.Key;
				this._colliderMaterials[num] = keyValuePair.Value.Item1;
				num++;
			}
			NativeArray<int2> samplePositions = CameraRenderTask.GetSamplePositions(width, height);
			this._foundCollidersLength[0] = 0;
			RaycastBufferSetupJob jobData = new RaycastBufferSetupJob
			{
				colliderIds = this._colliderIds.GetSubArray(0, this._colliderLength),
				colliderMaterials = this._colliderMaterials.GetSubArray(0, this._colliderLength),
				colliderHits = this._colliderHits.GetSubArray(0, this._colliderLength)
			};
			RaycastRaySetupJob jobData2 = new RaycastRaySetupJob
			{
				res = new float2((float)width, (float)height),
				halfRes = new float2((float)width / 2f, (float)height / 2f),
				aspectRatio = (float)width / (float)height,
				worldHeight = 2f * Mathf.Tan(0.008726646f * verticalFov),
				cameraPos = cameraTransform.position,
				cameraRot = cameraTransform.rotation,
				nearPlane = nearPlane,
				farPlane = farPlane,
				layerMask = layerMask,
				samplePositions = samplePositions,
				sampleOffset = sampleOffset % samplePositions.Length,
				raycastCommands = this._raycastCommands.GetSubArray(0, sampleCount)
			};
			RaycastRayProcessingJob jobData3 = new RaycastRayProcessingJob
			{
				cameraForward = -cameraTransform.forward,
				farPlane = farPlane,
				raycastHits = this._raycastHits.GetSubArray(0, sampleCount),
				colliderIds = this._colliderIds.GetSubArray(0, this._colliderLength),
				colliderMaterials = this._colliderMaterials.GetSubArray(0, this._colliderLength),
				colliderHits = this._colliderHits.GetSubArray(0, this._colliderLength),
				outputs = this._raycastOutput.GetSubArray(0, sampleCount),
				foundCollidersIndex = this._foundCollidersLength,
				foundColliders = this._foundColliders
			};
			RaycastColliderProcessingJob jobData4 = new RaycastColliderProcessingJob
			{
				foundCollidersLength = this._foundCollidersLength,
				foundColliders = this._foundColliders
			};
			RaycastOutputCompressJob jobData5 = new RaycastOutputCompressJob
			{
				rayOutputs = this._raycastOutput.GetSubArray(0, sampleCount),
				dataLength = this._outputDataLength,
				data = this._outputData
			};
			JobHandle job = jobData.Schedule(default(JobHandle));
			JobHandle dependsOn = jobData2.Schedule(sampleCount, 100, default(JobHandle));
			JobHandle job2 = RaycastCommand.ScheduleBatch(this._raycastCommands.GetSubArray(0, sampleCount), this._raycastHits.GetSubArray(0, sampleCount), 100, dependsOn);
			JobHandle dependsOn2 = jobData3.Schedule(sampleCount, 100, JobHandle.CombineDependencies(job, job2));
			JobHandle job3 = jobData4.Schedule(dependsOn2);
			JobHandle job4 = jobData5.Schedule(dependsOn2);
			this._pendingJob = new JobHandle?(JobHandle.CombineDependencies(job4, job3));
			return sampleOffset + sampleCount;
		}

		// Token: 0x06003D3B RID: 15675 RVA: 0x001679EC File Offset: 0x00165BEC
		public int ExtractRayData(byte[] buffer, List<int> hitColliderIds = null, List<int> foundColliderIds = null)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			int num = this._sampleCount * 4;
			if (buffer.Length < num)
			{
				throw new ArgumentException("Output buffer is not large enough to hold all the ray data", "buffer");
			}
			if (this._pendingJob == null)
			{
				throw new InvalidOperationException("Job was not started for this CameraRenderTask");
			}
			if (!this._pendingJob.Value.IsCompleted)
			{
				Debug.LogWarning("Trying to extract ray data from CameraRenderTask before completion! This will cause it to synchronously block for completion.");
			}
			this._pendingJob.Value.Complete();
			int num2 = this._outputDataLength[0];
			NativeArray<byte>.Copy(this._outputData.GetSubArray(0, num2), buffer, num2);
			if (hitColliderIds != null)
			{
				hitColliderIds.Clear();
				for (int i = 0; i < this._colliderLength; i++)
				{
					if (this._colliderHits[i] > 0)
					{
						hitColliderIds.Add(this._colliderIds[i]);
					}
				}
			}
			if (foundColliderIds != null)
			{
				foundColliderIds.Clear();
				int num3 = this._foundCollidersLength[0];
				for (int j = 0; j < num3; j++)
				{
					foundColliderIds.Add(this._foundColliders[j]);
				}
			}
			return num2;
		}

		// Token: 0x06003D3C RID: 15676 RVA: 0x00167B08 File Offset: 0x00165D08
		private static NativeArray<int2> GetSamplePositions(int width, int height)
		{
			if (width <= 0)
			{
				throw new ArgumentOutOfRangeException("width");
			}
			if (height <= 0)
			{
				throw new ArgumentOutOfRangeException("height");
			}
			ValueTuple<int, int> key = new ValueTuple<int, int>(width, height);
			NativeArray<int2> nativeArray;
			if (CameraRenderTask._samplePositions.TryGetValue(key, out nativeArray))
			{
				return nativeArray;
			}
			nativeArray = new NativeArray<int2>(width * height, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
			new RaycastSamplePositionsJob
			{
				res = new int2(width, height),
				random = new Unity.Mathematics.Random(1337U),
				positions = nativeArray
			}.Run<RaycastSamplePositionsJob>();
			CameraRenderTask._samplePositions.Add(key, nativeArray);
			return nativeArray;
		}

		// Token: 0x06003D3D RID: 15677 RVA: 0x00167B9C File Offset: 0x00165D9C
		public static void FreeCachedSamplePositions()
		{
			foreach (KeyValuePair<ValueTuple<int, int>, NativeArray<int2>> keyValuePair in CameraRenderTask._samplePositions)
			{
				keyValuePair.Value.Dispose();
			}
			CameraRenderTask._samplePositions.Clear();
		}
	}
}
