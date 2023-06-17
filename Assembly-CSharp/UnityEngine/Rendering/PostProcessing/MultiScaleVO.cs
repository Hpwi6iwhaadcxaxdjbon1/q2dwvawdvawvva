using System;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A67 RID: 2663
	[Preserve]
	[Serializable]
	internal sealed class MultiScaleVO : IAmbientOcclusionMethod
	{
		// Token: 0x040038E6 RID: 14566
		private readonly float[] m_SampleThickness = new float[]
		{
			Mathf.Sqrt(0.96f),
			Mathf.Sqrt(0.84f),
			Mathf.Sqrt(0.64f),
			Mathf.Sqrt(0.35999995f),
			Mathf.Sqrt(0.91999996f),
			Mathf.Sqrt(0.79999995f),
			Mathf.Sqrt(0.59999996f),
			Mathf.Sqrt(0.31999993f),
			Mathf.Sqrt(0.67999995f),
			Mathf.Sqrt(0.47999996f),
			Mathf.Sqrt(0.19999993f),
			Mathf.Sqrt(0.27999997f)
		};

		// Token: 0x040038E7 RID: 14567
		private readonly float[] m_InvThicknessTable = new float[12];

		// Token: 0x040038E8 RID: 14568
		private readonly float[] m_SampleWeightTable = new float[12];

		// Token: 0x040038E9 RID: 14569
		private readonly int[] m_Widths = new int[7];

		// Token: 0x040038EA RID: 14570
		private readonly int[] m_Heights = new int[7];

		// Token: 0x040038EB RID: 14571
		private AmbientOcclusion m_Settings;

		// Token: 0x040038EC RID: 14572
		private PropertySheet m_PropertySheet;

		// Token: 0x040038ED RID: 14573
		private PostProcessResources m_Resources;

		// Token: 0x040038EE RID: 14574
		private RenderTexture m_AmbientOnlyAO;

		// Token: 0x040038EF RID: 14575
		private readonly RenderTargetIdentifier[] m_MRT = new RenderTargetIdentifier[]
		{
			BuiltinRenderTextureType.GBuffer0,
			BuiltinRenderTextureType.CameraTarget
		};

		// Token: 0x06003F9A RID: 16282 RVA: 0x00177414 File Offset: 0x00175614
		public MultiScaleVO(AmbientOcclusion settings)
		{
			this.m_Settings = settings;
		}

		// Token: 0x06003F9B RID: 16283 RVA: 0x0000441C File Offset: 0x0000261C
		public DepthTextureMode GetCameraFlags()
		{
			return DepthTextureMode.Depth;
		}

		// Token: 0x06003F9C RID: 16284 RVA: 0x00177533 File Offset: 0x00175733
		public void SetResources(PostProcessResources resources)
		{
			this.m_Resources = resources;
		}

		// Token: 0x06003F9D RID: 16285 RVA: 0x0017753C File Offset: 0x0017573C
		private void Alloc(CommandBuffer cmd, int id, MultiScaleVO.MipLevel size, RenderTextureFormat format, bool uav)
		{
			cmd.GetTemporaryRT(id, new RenderTextureDescriptor
			{
				width = this.m_Widths[(int)size],
				height = this.m_Heights[(int)size],
				colorFormat = format,
				depthBufferBits = 0,
				volumeDepth = 1,
				autoGenerateMips = false,
				msaaSamples = 1,
				enableRandomWrite = uav,
				dimension = TextureDimension.Tex2D,
				sRGB = false
			}, FilterMode.Point);
		}

		// Token: 0x06003F9E RID: 16286 RVA: 0x001775BC File Offset: 0x001757BC
		private void AllocArray(CommandBuffer cmd, int id, MultiScaleVO.MipLevel size, RenderTextureFormat format, bool uav)
		{
			cmd.GetTemporaryRT(id, new RenderTextureDescriptor
			{
				width = this.m_Widths[(int)size],
				height = this.m_Heights[(int)size],
				colorFormat = format,
				depthBufferBits = 0,
				volumeDepth = 16,
				autoGenerateMips = false,
				msaaSamples = 1,
				enableRandomWrite = uav,
				dimension = TextureDimension.Tex2DArray,
				sRGB = false
			}, FilterMode.Point);
		}

		// Token: 0x06003F9F RID: 16287 RVA: 0x0017763D File Offset: 0x0017583D
		private void Release(CommandBuffer cmd, int id)
		{
			cmd.ReleaseTemporaryRT(id);
		}

		// Token: 0x06003FA0 RID: 16288 RVA: 0x00177648 File Offset: 0x00175848
		private Vector4 CalculateZBufferParams(Camera camera)
		{
			float num = camera.farClipPlane / camera.nearClipPlane;
			if (SystemInfo.usesReversedZBuffer)
			{
				return new Vector4(num - 1f, 1f, 0f, 0f);
			}
			return new Vector4(1f - num, num, 0f, 0f);
		}

		// Token: 0x06003FA1 RID: 16289 RVA: 0x001776A0 File Offset: 0x001758A0
		private float CalculateTanHalfFovHeight(Camera camera)
		{
			return 1f / camera.projectionMatrix[0, 0];
		}

		// Token: 0x06003FA2 RID: 16290 RVA: 0x001776C3 File Offset: 0x001758C3
		private Vector2 GetSize(MultiScaleVO.MipLevel mip)
		{
			return new Vector2((float)this.m_Widths[(int)mip], (float)this.m_Heights[(int)mip]);
		}

		// Token: 0x06003FA3 RID: 16291 RVA: 0x001776DC File Offset: 0x001758DC
		private Vector3 GetSizeArray(MultiScaleVO.MipLevel mip)
		{
			return new Vector3((float)this.m_Widths[(int)mip], (float)this.m_Heights[(int)mip], 16f);
		}

		// Token: 0x06003FA4 RID: 16292 RVA: 0x001776FC File Offset: 0x001758FC
		public void GenerateAOMap(CommandBuffer cmd, Camera camera, RenderTargetIdentifier destination, RenderTargetIdentifier? depthMap, bool invert, bool isMSAA)
		{
			this.m_Widths[0] = camera.pixelWidth * (RuntimeUtilities.isSinglePassStereoEnabled ? 2 : 1);
			this.m_Heights[0] = camera.pixelHeight;
			for (int i = 1; i < 7; i++)
			{
				int num = 1 << i;
				this.m_Widths[i] = (this.m_Widths[0] + (num - 1)) / num;
				this.m_Heights[i] = (this.m_Heights[0] + (num - 1)) / num;
			}
			this.PushAllocCommands(cmd, isMSAA);
			this.PushDownsampleCommands(cmd, camera, depthMap, isMSAA);
			float tanHalfFovH = this.CalculateTanHalfFovHeight(camera);
			this.PushRenderCommands(cmd, ShaderIDs.TiledDepth1, ShaderIDs.Occlusion1, this.GetSizeArray(MultiScaleVO.MipLevel.L3), tanHalfFovH, isMSAA);
			this.PushRenderCommands(cmd, ShaderIDs.TiledDepth2, ShaderIDs.Occlusion2, this.GetSizeArray(MultiScaleVO.MipLevel.L4), tanHalfFovH, isMSAA);
			this.PushRenderCommands(cmd, ShaderIDs.TiledDepth3, ShaderIDs.Occlusion3, this.GetSizeArray(MultiScaleVO.MipLevel.L5), tanHalfFovH, isMSAA);
			this.PushRenderCommands(cmd, ShaderIDs.TiledDepth4, ShaderIDs.Occlusion4, this.GetSizeArray(MultiScaleVO.MipLevel.L6), tanHalfFovH, isMSAA);
			this.PushUpsampleCommands(cmd, ShaderIDs.LowDepth4, ShaderIDs.Occlusion4, ShaderIDs.LowDepth3, new int?(ShaderIDs.Occlusion3), ShaderIDs.Combined3, this.GetSize(MultiScaleVO.MipLevel.L4), this.GetSize(MultiScaleVO.MipLevel.L3), isMSAA, false);
			this.PushUpsampleCommands(cmd, ShaderIDs.LowDepth3, ShaderIDs.Combined3, ShaderIDs.LowDepth2, new int?(ShaderIDs.Occlusion2), ShaderIDs.Combined2, this.GetSize(MultiScaleVO.MipLevel.L3), this.GetSize(MultiScaleVO.MipLevel.L2), isMSAA, false);
			this.PushUpsampleCommands(cmd, ShaderIDs.LowDepth2, ShaderIDs.Combined2, ShaderIDs.LowDepth1, new int?(ShaderIDs.Occlusion1), ShaderIDs.Combined1, this.GetSize(MultiScaleVO.MipLevel.L2), this.GetSize(MultiScaleVO.MipLevel.L1), isMSAA, false);
			this.PushUpsampleCommands(cmd, ShaderIDs.LowDepth1, ShaderIDs.Combined1, ShaderIDs.LinearDepth, null, destination, this.GetSize(MultiScaleVO.MipLevel.L1), this.GetSize(MultiScaleVO.MipLevel.Original), isMSAA, invert);
			this.PushReleaseCommands(cmd);
		}

		// Token: 0x06003FA5 RID: 16293 RVA: 0x001778F8 File Offset: 0x00175AF8
		private void PushAllocCommands(CommandBuffer cmd, bool isMSAA)
		{
			if (isMSAA)
			{
				this.Alloc(cmd, ShaderIDs.LinearDepth, MultiScaleVO.MipLevel.Original, RenderTextureFormat.RGHalf, true);
				this.Alloc(cmd, ShaderIDs.LowDepth1, MultiScaleVO.MipLevel.L1, RenderTextureFormat.RGFloat, true);
				this.Alloc(cmd, ShaderIDs.LowDepth2, MultiScaleVO.MipLevel.L2, RenderTextureFormat.RGFloat, true);
				this.Alloc(cmd, ShaderIDs.LowDepth3, MultiScaleVO.MipLevel.L3, RenderTextureFormat.RGFloat, true);
				this.Alloc(cmd, ShaderIDs.LowDepth4, MultiScaleVO.MipLevel.L4, RenderTextureFormat.RGFloat, true);
				this.AllocArray(cmd, ShaderIDs.TiledDepth1, MultiScaleVO.MipLevel.L3, RenderTextureFormat.RGHalf, true);
				this.AllocArray(cmd, ShaderIDs.TiledDepth2, MultiScaleVO.MipLevel.L4, RenderTextureFormat.RGHalf, true);
				this.AllocArray(cmd, ShaderIDs.TiledDepth3, MultiScaleVO.MipLevel.L5, RenderTextureFormat.RGHalf, true);
				this.AllocArray(cmd, ShaderIDs.TiledDepth4, MultiScaleVO.MipLevel.L6, RenderTextureFormat.RGHalf, true);
				this.Alloc(cmd, ShaderIDs.Occlusion1, MultiScaleVO.MipLevel.L1, RenderTextureFormat.RG16, true);
				this.Alloc(cmd, ShaderIDs.Occlusion2, MultiScaleVO.MipLevel.L2, RenderTextureFormat.RG16, true);
				this.Alloc(cmd, ShaderIDs.Occlusion3, MultiScaleVO.MipLevel.L3, RenderTextureFormat.RG16, true);
				this.Alloc(cmd, ShaderIDs.Occlusion4, MultiScaleVO.MipLevel.L4, RenderTextureFormat.RG16, true);
				this.Alloc(cmd, ShaderIDs.Combined1, MultiScaleVO.MipLevel.L1, RenderTextureFormat.RG16, true);
				this.Alloc(cmd, ShaderIDs.Combined2, MultiScaleVO.MipLevel.L2, RenderTextureFormat.RG16, true);
				this.Alloc(cmd, ShaderIDs.Combined3, MultiScaleVO.MipLevel.L3, RenderTextureFormat.RG16, true);
				return;
			}
			this.Alloc(cmd, ShaderIDs.LinearDepth, MultiScaleVO.MipLevel.Original, RenderTextureFormat.RHalf, true);
			this.Alloc(cmd, ShaderIDs.LowDepth1, MultiScaleVO.MipLevel.L1, RenderTextureFormat.RFloat, true);
			this.Alloc(cmd, ShaderIDs.LowDepth2, MultiScaleVO.MipLevel.L2, RenderTextureFormat.RFloat, true);
			this.Alloc(cmd, ShaderIDs.LowDepth3, MultiScaleVO.MipLevel.L3, RenderTextureFormat.RFloat, true);
			this.Alloc(cmd, ShaderIDs.LowDepth4, MultiScaleVO.MipLevel.L4, RenderTextureFormat.RFloat, true);
			this.AllocArray(cmd, ShaderIDs.TiledDepth1, MultiScaleVO.MipLevel.L3, RenderTextureFormat.RHalf, true);
			this.AllocArray(cmd, ShaderIDs.TiledDepth2, MultiScaleVO.MipLevel.L4, RenderTextureFormat.RHalf, true);
			this.AllocArray(cmd, ShaderIDs.TiledDepth3, MultiScaleVO.MipLevel.L5, RenderTextureFormat.RHalf, true);
			this.AllocArray(cmd, ShaderIDs.TiledDepth4, MultiScaleVO.MipLevel.L6, RenderTextureFormat.RHalf, true);
			this.Alloc(cmd, ShaderIDs.Occlusion1, MultiScaleVO.MipLevel.L1, RenderTextureFormat.R8, true);
			this.Alloc(cmd, ShaderIDs.Occlusion2, MultiScaleVO.MipLevel.L2, RenderTextureFormat.R8, true);
			this.Alloc(cmd, ShaderIDs.Occlusion3, MultiScaleVO.MipLevel.L3, RenderTextureFormat.R8, true);
			this.Alloc(cmd, ShaderIDs.Occlusion4, MultiScaleVO.MipLevel.L4, RenderTextureFormat.R8, true);
			this.Alloc(cmd, ShaderIDs.Combined1, MultiScaleVO.MipLevel.L1, RenderTextureFormat.R8, true);
			this.Alloc(cmd, ShaderIDs.Combined2, MultiScaleVO.MipLevel.L2, RenderTextureFormat.R8, true);
			this.Alloc(cmd, ShaderIDs.Combined3, MultiScaleVO.MipLevel.L3, RenderTextureFormat.R8, true);
		}

		// Token: 0x06003FA6 RID: 16294 RVA: 0x00177B0C File Offset: 0x00175D0C
		private void PushDownsampleCommands(CommandBuffer cmd, Camera camera, RenderTargetIdentifier? depthMap, bool isMSAA)
		{
			bool flag = false;
			RenderTargetIdentifier renderTargetIdentifier;
			if (depthMap != null)
			{
				renderTargetIdentifier = depthMap.Value;
			}
			else if (!RuntimeUtilities.IsResolvedDepthAvailable(camera))
			{
				this.Alloc(cmd, ShaderIDs.DepthCopy, MultiScaleVO.MipLevel.Original, RenderTextureFormat.RFloat, false);
				renderTargetIdentifier = new RenderTargetIdentifier(ShaderIDs.DepthCopy);
				cmd.BlitFullscreenTriangle(BuiltinRenderTextureType.None, renderTargetIdentifier, this.m_PropertySheet, 0, false, null);
				flag = true;
			}
			else
			{
				renderTargetIdentifier = BuiltinRenderTextureType.ResolvedDepth;
			}
			ComputeShader computeShader = this.m_Resources.computeShaders.multiScaleAODownsample1;
			int kernelIndex = computeShader.FindKernel(isMSAA ? "MultiScaleVODownsample1_MSAA" : "MultiScaleVODownsample1");
			cmd.SetComputeTextureParam(computeShader, kernelIndex, "LinearZ", ShaderIDs.LinearDepth);
			cmd.SetComputeTextureParam(computeShader, kernelIndex, "DS2x", ShaderIDs.LowDepth1);
			cmd.SetComputeTextureParam(computeShader, kernelIndex, "DS4x", ShaderIDs.LowDepth2);
			cmd.SetComputeTextureParam(computeShader, kernelIndex, "DS2xAtlas", ShaderIDs.TiledDepth1);
			cmd.SetComputeTextureParam(computeShader, kernelIndex, "DS4xAtlas", ShaderIDs.TiledDepth2);
			cmd.SetComputeVectorParam(computeShader, "ZBufferParams", this.CalculateZBufferParams(camera));
			cmd.SetComputeTextureParam(computeShader, kernelIndex, "Depth", renderTargetIdentifier);
			cmd.DispatchCompute(computeShader, kernelIndex, this.m_Widths[4], this.m_Heights[4], 1);
			if (flag)
			{
				this.Release(cmd, ShaderIDs.DepthCopy);
			}
			computeShader = this.m_Resources.computeShaders.multiScaleAODownsample2;
			kernelIndex = (isMSAA ? computeShader.FindKernel("MultiScaleVODownsample2_MSAA") : computeShader.FindKernel("MultiScaleVODownsample2"));
			cmd.SetComputeTextureParam(computeShader, kernelIndex, "DS4x", ShaderIDs.LowDepth2);
			cmd.SetComputeTextureParam(computeShader, kernelIndex, "DS8x", ShaderIDs.LowDepth3);
			cmd.SetComputeTextureParam(computeShader, kernelIndex, "DS16x", ShaderIDs.LowDepth4);
			cmd.SetComputeTextureParam(computeShader, kernelIndex, "DS8xAtlas", ShaderIDs.TiledDepth3);
			cmd.SetComputeTextureParam(computeShader, kernelIndex, "DS16xAtlas", ShaderIDs.TiledDepth4);
			cmd.DispatchCompute(computeShader, kernelIndex, this.m_Widths[6], this.m_Heights[6], 1);
		}

		// Token: 0x06003FA7 RID: 16295 RVA: 0x00177D1C File Offset: 0x00175F1C
		private void PushRenderCommands(CommandBuffer cmd, int source, int destination, Vector3 sourceSize, float tanHalfFovH, bool isMSAA)
		{
			float num = 2f * tanHalfFovH * 10f / sourceSize.x;
			if (RuntimeUtilities.isSinglePassStereoEnabled)
			{
				num *= 2f;
			}
			float num2 = 1f / num;
			for (int i = 0; i < 12; i++)
			{
				this.m_InvThicknessTable[i] = num2 / this.m_SampleThickness[i];
			}
			this.m_SampleWeightTable[0] = 4f * this.m_SampleThickness[0];
			this.m_SampleWeightTable[1] = 4f * this.m_SampleThickness[1];
			this.m_SampleWeightTable[2] = 4f * this.m_SampleThickness[2];
			this.m_SampleWeightTable[3] = 4f * this.m_SampleThickness[3];
			this.m_SampleWeightTable[4] = 4f * this.m_SampleThickness[4];
			this.m_SampleWeightTable[5] = 8f * this.m_SampleThickness[5];
			this.m_SampleWeightTable[6] = 8f * this.m_SampleThickness[6];
			this.m_SampleWeightTable[7] = 8f * this.m_SampleThickness[7];
			this.m_SampleWeightTable[8] = 4f * this.m_SampleThickness[8];
			this.m_SampleWeightTable[9] = 8f * this.m_SampleThickness[9];
			this.m_SampleWeightTable[10] = 8f * this.m_SampleThickness[10];
			this.m_SampleWeightTable[11] = 4f * this.m_SampleThickness[11];
			this.m_SampleWeightTable[0] = 0f;
			this.m_SampleWeightTable[2] = 0f;
			this.m_SampleWeightTable[5] = 0f;
			this.m_SampleWeightTable[7] = 0f;
			this.m_SampleWeightTable[9] = 0f;
			float num3 = 0f;
			foreach (float num4 in this.m_SampleWeightTable)
			{
				num3 += num4;
			}
			for (int k = 0; k < this.m_SampleWeightTable.Length; k++)
			{
				this.m_SampleWeightTable[k] /= num3;
			}
			ComputeShader multiScaleAORender = this.m_Resources.computeShaders.multiScaleAORender;
			int kernelIndex = isMSAA ? multiScaleAORender.FindKernel("MultiScaleVORender_MSAA_interleaved") : multiScaleAORender.FindKernel("MultiScaleVORender_interleaved");
			cmd.SetComputeFloatParams(multiScaleAORender, "gInvThicknessTable", this.m_InvThicknessTable);
			cmd.SetComputeFloatParams(multiScaleAORender, "gSampleWeightTable", this.m_SampleWeightTable);
			cmd.SetComputeVectorParam(multiScaleAORender, "gInvSliceDimension", new Vector2(1f / sourceSize.x, 1f / sourceSize.y));
			cmd.SetComputeVectorParam(multiScaleAORender, "AdditionalParams", new Vector2(-1f / this.m_Settings.thicknessModifier.value, this.m_Settings.intensity.value));
			cmd.SetComputeTextureParam(multiScaleAORender, kernelIndex, "DepthTex", source);
			cmd.SetComputeTextureParam(multiScaleAORender, kernelIndex, "Occlusion", destination);
			uint num5;
			uint num6;
			uint num7;
			multiScaleAORender.GetKernelThreadGroupSizes(kernelIndex, out num5, out num6, out num7);
			cmd.DispatchCompute(multiScaleAORender, kernelIndex, ((int)sourceSize.x + (int)num5 - 1) / (int)num5, ((int)sourceSize.y + (int)num6 - 1) / (int)num6, ((int)sourceSize.z + (int)num7 - 1) / (int)num7);
		}

		// Token: 0x06003FA8 RID: 16296 RVA: 0x00178050 File Offset: 0x00176250
		private void PushUpsampleCommands(CommandBuffer cmd, int lowResDepth, int interleavedAO, int highResDepth, int? highResAO, RenderTargetIdentifier dest, Vector3 lowResDepthSize, Vector2 highResDepthSize, bool isMSAA, bool invert = false)
		{
			ComputeShader multiScaleAOUpsample = this.m_Resources.computeShaders.multiScaleAOUpsample;
			int kernelIndex;
			if (!isMSAA)
			{
				kernelIndex = multiScaleAOUpsample.FindKernel((highResAO == null) ? (invert ? "MultiScaleVOUpSample_invert" : "MultiScaleVOUpSample") : "MultiScaleVOUpSample_blendout");
			}
			else
			{
				kernelIndex = multiScaleAOUpsample.FindKernel((highResAO == null) ? (invert ? "MultiScaleVOUpSample_MSAA_invert" : "MultiScaleVOUpSample_MSAA") : "MultiScaleVOUpSample_MSAA_blendout");
			}
			float num = 1920f / lowResDepthSize.x;
			float num2 = 1f - Mathf.Pow(10f, this.m_Settings.blurTolerance.value) * num;
			num2 *= num2;
			float num3 = Mathf.Pow(10f, this.m_Settings.upsampleTolerance.value);
			float x = 1f / (Mathf.Pow(10f, this.m_Settings.noiseFilterTolerance.value) + num3);
			cmd.SetComputeVectorParam(multiScaleAOUpsample, "InvLowResolution", new Vector2(1f / lowResDepthSize.x, 1f / lowResDepthSize.y));
			cmd.SetComputeVectorParam(multiScaleAOUpsample, "InvHighResolution", new Vector2(1f / highResDepthSize.x, 1f / highResDepthSize.y));
			cmd.SetComputeVectorParam(multiScaleAOUpsample, "AdditionalParams", new Vector4(x, num, num2, num3));
			cmd.SetComputeTextureParam(multiScaleAOUpsample, kernelIndex, "LoResDB", lowResDepth);
			cmd.SetComputeTextureParam(multiScaleAOUpsample, kernelIndex, "HiResDB", highResDepth);
			cmd.SetComputeTextureParam(multiScaleAOUpsample, kernelIndex, "LoResAO1", interleavedAO);
			if (highResAO != null)
			{
				cmd.SetComputeTextureParam(multiScaleAOUpsample, kernelIndex, "HiResAO", highResAO.Value);
			}
			cmd.SetComputeTextureParam(multiScaleAOUpsample, kernelIndex, "AoResult", dest);
			int threadGroupsX = ((int)highResDepthSize.x + 17) / 16;
			int threadGroupsY = ((int)highResDepthSize.y + 17) / 16;
			cmd.DispatchCompute(multiScaleAOUpsample, kernelIndex, threadGroupsX, threadGroupsY, 1);
		}

		// Token: 0x06003FA9 RID: 16297 RVA: 0x0017824C File Offset: 0x0017644C
		private void PushReleaseCommands(CommandBuffer cmd)
		{
			this.Release(cmd, ShaderIDs.LinearDepth);
			this.Release(cmd, ShaderIDs.LowDepth1);
			this.Release(cmd, ShaderIDs.LowDepth2);
			this.Release(cmd, ShaderIDs.LowDepth3);
			this.Release(cmd, ShaderIDs.LowDepth4);
			this.Release(cmd, ShaderIDs.TiledDepth1);
			this.Release(cmd, ShaderIDs.TiledDepth2);
			this.Release(cmd, ShaderIDs.TiledDepth3);
			this.Release(cmd, ShaderIDs.TiledDepth4);
			this.Release(cmd, ShaderIDs.Occlusion1);
			this.Release(cmd, ShaderIDs.Occlusion2);
			this.Release(cmd, ShaderIDs.Occlusion3);
			this.Release(cmd, ShaderIDs.Occlusion4);
			this.Release(cmd, ShaderIDs.Combined1);
			this.Release(cmd, ShaderIDs.Combined2);
			this.Release(cmd, ShaderIDs.Combined3);
		}

		// Token: 0x06003FAA RID: 16298 RVA: 0x0017831C File Offset: 0x0017651C
		private void PreparePropertySheet(PostProcessRenderContext context)
		{
			PropertySheet propertySheet = context.propertySheets.Get(this.m_Resources.shaders.multiScaleAO);
			propertySheet.ClearKeywords();
			propertySheet.properties.SetVector(ShaderIDs.AOColor, Color.white - this.m_Settings.color.value);
			this.m_PropertySheet = propertySheet;
		}

		// Token: 0x06003FAB RID: 16299 RVA: 0x00178384 File Offset: 0x00176584
		private void CheckAOTexture(PostProcessRenderContext context)
		{
			if (this.m_AmbientOnlyAO == null || !this.m_AmbientOnlyAO.IsCreated() || this.m_AmbientOnlyAO.width != context.width || this.m_AmbientOnlyAO.height != context.height)
			{
				RuntimeUtilities.Destroy(this.m_AmbientOnlyAO);
				this.m_AmbientOnlyAO = new RenderTexture(context.width, context.height, 0, RenderTextureFormat.R8, RenderTextureReadWrite.Linear)
				{
					hideFlags = HideFlags.DontSave,
					filterMode = FilterMode.Point,
					enableRandomWrite = true
				};
				this.m_AmbientOnlyAO.Create();
			}
		}

		// Token: 0x06003FAC RID: 16300 RVA: 0x0017841A File Offset: 0x0017661A
		private void PushDebug(PostProcessRenderContext context)
		{
			if (context.IsDebugOverlayEnabled(DebugOverlay.AmbientOcclusion))
			{
				context.PushDebugOverlay(context.command, this.m_AmbientOnlyAO, this.m_PropertySheet, 3);
			}
		}

		// Token: 0x06003FAD RID: 16301 RVA: 0x00178444 File Offset: 0x00176644
		public void RenderAfterOpaque(PostProcessRenderContext context)
		{
			CommandBuffer command = context.command;
			command.BeginSample("Ambient Occlusion");
			this.SetResources(context.resources);
			this.PreparePropertySheet(context);
			this.CheckAOTexture(context);
			if (context.camera.actualRenderingPath == RenderingPath.Forward && RenderSettings.fog)
			{
				this.m_PropertySheet.EnableKeyword("APPLY_FORWARD_FOG");
				this.m_PropertySheet.properties.SetVector(ShaderIDs.FogParams, new Vector3(RenderSettings.fogDensity, RenderSettings.fogStartDistance, RenderSettings.fogEndDistance));
			}
			this.GenerateAOMap(command, context.camera, this.m_AmbientOnlyAO, null, false, false);
			this.PushDebug(context);
			command.SetGlobalTexture(ShaderIDs.MSVOcclusionTexture, this.m_AmbientOnlyAO);
			command.BlitFullscreenTriangle(BuiltinRenderTextureType.None, BuiltinRenderTextureType.CameraTarget, this.m_PropertySheet, 2, RenderBufferLoadAction.Load, null);
			command.EndSample("Ambient Occlusion");
		}

		// Token: 0x06003FAE RID: 16302 RVA: 0x00178540 File Offset: 0x00176740
		public void RenderAmbientOnly(PostProcessRenderContext context)
		{
			CommandBuffer command = context.command;
			command.BeginSample("Ambient Occlusion Render");
			this.SetResources(context.resources);
			this.PreparePropertySheet(context);
			this.CheckAOTexture(context);
			this.GenerateAOMap(command, context.camera, this.m_AmbientOnlyAO, null, false, false);
			this.PushDebug(context);
			command.EndSample("Ambient Occlusion Render");
		}

		// Token: 0x06003FAF RID: 16303 RVA: 0x001785B0 File Offset: 0x001767B0
		public void CompositeAmbientOnly(PostProcessRenderContext context)
		{
			CommandBuffer command = context.command;
			command.BeginSample("Ambient Occlusion Composite");
			command.SetGlobalTexture(ShaderIDs.MSVOcclusionTexture, this.m_AmbientOnlyAO);
			command.BlitFullscreenTriangle(BuiltinRenderTextureType.None, this.m_MRT, BuiltinRenderTextureType.CameraTarget, this.m_PropertySheet, 1, false, null);
			command.EndSample("Ambient Occlusion Composite");
		}

		// Token: 0x06003FB0 RID: 16304 RVA: 0x00178617 File Offset: 0x00176817
		public void Release()
		{
			RuntimeUtilities.Destroy(this.m_AmbientOnlyAO);
			this.m_AmbientOnlyAO = null;
		}

		// Token: 0x02000F24 RID: 3876
		internal enum MipLevel
		{
			// Token: 0x04004E8D RID: 20109
			Original,
			// Token: 0x04004E8E RID: 20110
			L1,
			// Token: 0x04004E8F RID: 20111
			L2,
			// Token: 0x04004E90 RID: 20112
			L3,
			// Token: 0x04004E91 RID: 20113
			L4,
			// Token: 0x04004E92 RID: 20114
			L5,
			// Token: 0x04004E93 RID: 20115
			L6
		}

		// Token: 0x02000F25 RID: 3877
		private enum Pass
		{
			// Token: 0x04004E95 RID: 20117
			DepthCopy,
			// Token: 0x04004E96 RID: 20118
			CompositionDeferred,
			// Token: 0x04004E97 RID: 20119
			CompositionForward,
			// Token: 0x04004E98 RID: 20120
			DebugOverlay
		}
	}
}
