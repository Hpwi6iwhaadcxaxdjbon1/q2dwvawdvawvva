using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000987 RID: 2439
[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(Camera))]
public class CoverageQueries : MonoBehaviour
{
	// Token: 0x04003466 RID: 13414
	public bool debug;

	// Token: 0x04003467 RID: 13415
	public float depthBias = -0.1f;

	// Token: 0x02000ECD RID: 3789
	public class BufferSet
	{
		// Token: 0x04004D05 RID: 19717
		public int width;

		// Token: 0x04004D06 RID: 19718
		public int height;

		// Token: 0x04004D07 RID: 19719
		public Texture2D inputTexture;

		// Token: 0x04004D08 RID: 19720
		public RenderTexture resultTexture;

		// Token: 0x04004D09 RID: 19721
		public Color[] inputData = new Color[0];

		// Token: 0x04004D0A RID: 19722
		public Color32[] resultData = new Color32[0];

		// Token: 0x04004D0B RID: 19723
		private Material coverageMat;

		// Token: 0x04004D0C RID: 19724
		private const int MaxAsyncGPUReadbackRequests = 10;

		// Token: 0x04004D0D RID: 19725
		private Queue<AsyncGPUReadbackRequest> asyncRequests = new Queue<AsyncGPUReadbackRequest>();

		// Token: 0x06005358 RID: 21336 RVA: 0x001B2214 File Offset: 0x001B0414
		public void Attach(Material coverageMat)
		{
			this.coverageMat = coverageMat;
		}

		// Token: 0x06005359 RID: 21337 RVA: 0x001B2220 File Offset: 0x001B0420
		public void Dispose(bool data = true)
		{
			if (this.inputTexture != null)
			{
				UnityEngine.Object.DestroyImmediate(this.inputTexture);
				this.inputTexture = null;
			}
			if (this.resultTexture != null)
			{
				RenderTexture.active = null;
				this.resultTexture.Release();
				UnityEngine.Object.DestroyImmediate(this.resultTexture);
				this.resultTexture = null;
			}
			if (data)
			{
				this.inputData = new Color[0];
				this.resultData = new Color32[0];
			}
		}

		// Token: 0x0600535A RID: 21338 RVA: 0x001B229C File Offset: 0x001B049C
		public bool CheckResize(int count)
		{
			if (count > this.inputData.Length || (this.resultTexture != null && !this.resultTexture.IsCreated()))
			{
				this.Dispose(false);
				this.width = Mathf.CeilToInt(Mathf.Sqrt((float)count));
				this.height = Mathf.CeilToInt((float)count / (float)this.width);
				this.inputTexture = new Texture2D(this.width, this.height, TextureFormat.RGBAFloat, false, true);
				this.inputTexture.name = "_Input";
				this.inputTexture.filterMode = FilterMode.Point;
				this.inputTexture.wrapMode = TextureWrapMode.Clamp;
				this.resultTexture = new RenderTexture(this.width, this.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
				this.resultTexture.name = "_Result";
				this.resultTexture.filterMode = FilterMode.Point;
				this.resultTexture.wrapMode = TextureWrapMode.Clamp;
				this.resultTexture.useMipMap = false;
				this.resultTexture.Create();
				int num = this.resultData.Length;
				int num2 = this.width * this.height;
				Array.Resize<Color>(ref this.inputData, num2);
				Array.Resize<Color32>(ref this.resultData, num2);
				Color32 color = new Color32(byte.MaxValue, 0, 0, 0);
				for (int i = num; i < num2; i++)
				{
					this.resultData[i] = color;
				}
				return true;
			}
			return false;
		}

		// Token: 0x0600535B RID: 21339 RVA: 0x001B23F8 File Offset: 0x001B05F8
		public void UploadData()
		{
			if (this.inputData.Length != 0)
			{
				this.inputTexture.SetPixels(this.inputData);
				this.inputTexture.Apply();
			}
		}

		// Token: 0x0600535C RID: 21340 RVA: 0x001B2420 File Offset: 0x001B0620
		public void Dispatch(int count)
		{
			if (this.inputData.Length != 0)
			{
				RenderBuffer activeColorBuffer = Graphics.activeColorBuffer;
				RenderBuffer activeDepthBuffer = Graphics.activeDepthBuffer;
				this.coverageMat.SetTexture("_Input", this.inputTexture);
				Graphics.Blit(this.inputTexture, this.resultTexture, this.coverageMat, 0);
				Graphics.SetRenderTarget(activeColorBuffer, activeDepthBuffer);
			}
		}

		// Token: 0x0600535D RID: 21341 RVA: 0x001B2475 File Offset: 0x001B0675
		public void IssueRead()
		{
			if (this.asyncRequests.Count < 10)
			{
				this.asyncRequests.Enqueue(AsyncGPUReadback.Request(this.resultTexture, 0, null));
			}
		}

		// Token: 0x0600535E RID: 21342 RVA: 0x001B24A0 File Offset: 0x001B06A0
		public void GetResults()
		{
			if (this.resultData.Length != 0)
			{
				while (this.asyncRequests.Count > 0)
				{
					AsyncGPUReadbackRequest asyncGPUReadbackRequest = this.asyncRequests.Peek();
					if (asyncGPUReadbackRequest.hasError)
					{
						this.asyncRequests.Dequeue();
					}
					else
					{
						if (!asyncGPUReadbackRequest.done)
						{
							break;
						}
						NativeArray<Color32> data = asyncGPUReadbackRequest.GetData<Color32>(0);
						for (int i = 0; i < data.Length; i++)
						{
							this.resultData[i] = data[i];
						}
						this.asyncRequests.Dequeue();
					}
				}
			}
		}
	}

	// Token: 0x02000ECE RID: 3790
	public enum RadiusSpace
	{
		// Token: 0x04004D0F RID: 19727
		ScreenNormalized,
		// Token: 0x04004D10 RID: 19728
		World
	}

	// Token: 0x02000ECF RID: 3791
	public class Query
	{
		// Token: 0x04004D11 RID: 19729
		public CoverageQueries.Query.Input input;

		// Token: 0x04004D12 RID: 19730
		public CoverageQueries.Query.Internal intern;

		// Token: 0x04004D13 RID: 19731
		public CoverageQueries.Query.Result result;

		// Token: 0x17000705 RID: 1797
		// (get) Token: 0x06005360 RID: 21344 RVA: 0x001B2558 File Offset: 0x001B0758
		public bool IsRegistered
		{
			get
			{
				return this.intern.id >= 0;
			}
		}

		// Token: 0x02000FD8 RID: 4056
		public struct Input
		{
			// Token: 0x0400510A RID: 20746
			public Vector3 position;

			// Token: 0x0400510B RID: 20747
			public CoverageQueries.RadiusSpace radiusSpace;

			// Token: 0x0400510C RID: 20748
			public float radius;

			// Token: 0x0400510D RID: 20749
			public int sampleCount;

			// Token: 0x0400510E RID: 20750
			public float smoothingSpeed;
		}

		// Token: 0x02000FD9 RID: 4057
		public struct Internal
		{
			// Token: 0x0400510F RID: 20751
			public int id;

			// Token: 0x060055A8 RID: 21928 RVA: 0x001BA9A3 File Offset: 0x001B8BA3
			public void Reset()
			{
				this.id = -1;
			}
		}

		// Token: 0x02000FDA RID: 4058
		public struct Result
		{
			// Token: 0x04005110 RID: 20752
			public int passed;

			// Token: 0x04005111 RID: 20753
			public float coverage;

			// Token: 0x04005112 RID: 20754
			public float smoothCoverage;

			// Token: 0x04005113 RID: 20755
			public float weightedCoverage;

			// Token: 0x04005114 RID: 20756
			public float weightedSmoothCoverage;

			// Token: 0x04005115 RID: 20757
			public bool originOccluded;

			// Token: 0x04005116 RID: 20758
			public int frame;

			// Token: 0x04005117 RID: 20759
			public float originVisibility;

			// Token: 0x04005118 RID: 20760
			public float originSmoothVisibility;

			// Token: 0x060055A9 RID: 21929 RVA: 0x001BA9AC File Offset: 0x001B8BAC
			public void Reset()
			{
				this.passed = 0;
				this.coverage = 0f;
				this.smoothCoverage = 0f;
				this.weightedCoverage = 0f;
				this.weightedSmoothCoverage = 0f;
				this.originOccluded = true;
				this.frame = -1;
				this.originVisibility = 0f;
				this.originSmoothVisibility = 0f;
			}
		}
	}
}
