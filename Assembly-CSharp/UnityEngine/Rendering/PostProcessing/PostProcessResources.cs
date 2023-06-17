using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A93 RID: 2707
	public sealed class PostProcessResources : ScriptableObject
	{
		// Token: 0x04003987 RID: 14727
		public Texture2D[] blueNoise64;

		// Token: 0x04003988 RID: 14728
		public Texture2D[] blueNoise256;

		// Token: 0x04003989 RID: 14729
		public PostProcessResources.SMAALuts smaaLuts;

		// Token: 0x0400398A RID: 14730
		public PostProcessResources.Shaders shaders;

		// Token: 0x0400398B RID: 14731
		public PostProcessResources.ComputeShaders computeShaders;

		// Token: 0x02000F31 RID: 3889
		[Serializable]
		public sealed class Shaders
		{
			// Token: 0x04004EC6 RID: 20166
			public Shader bloom;

			// Token: 0x04004EC7 RID: 20167
			public Shader copy;

			// Token: 0x04004EC8 RID: 20168
			public Shader copyStd;

			// Token: 0x04004EC9 RID: 20169
			public Shader copyStdFromTexArray;

			// Token: 0x04004ECA RID: 20170
			public Shader copyStdFromDoubleWide;

			// Token: 0x04004ECB RID: 20171
			public Shader discardAlpha;

			// Token: 0x04004ECC RID: 20172
			public Shader depthOfField;

			// Token: 0x04004ECD RID: 20173
			public Shader finalPass;

			// Token: 0x04004ECE RID: 20174
			public Shader grainBaker;

			// Token: 0x04004ECF RID: 20175
			public Shader motionBlur;

			// Token: 0x04004ED0 RID: 20176
			public Shader temporalAntialiasing;

			// Token: 0x04004ED1 RID: 20177
			public Shader subpixelMorphologicalAntialiasing;

			// Token: 0x04004ED2 RID: 20178
			public Shader texture2dLerp;

			// Token: 0x04004ED3 RID: 20179
			public Shader uber;

			// Token: 0x04004ED4 RID: 20180
			public Shader lut2DBaker;

			// Token: 0x04004ED5 RID: 20181
			public Shader lightMeter;

			// Token: 0x04004ED6 RID: 20182
			public Shader gammaHistogram;

			// Token: 0x04004ED7 RID: 20183
			public Shader waveform;

			// Token: 0x04004ED8 RID: 20184
			public Shader vectorscope;

			// Token: 0x04004ED9 RID: 20185
			public Shader debugOverlays;

			// Token: 0x04004EDA RID: 20186
			public Shader deferredFog;

			// Token: 0x04004EDB RID: 20187
			public Shader scalableAO;

			// Token: 0x04004EDC RID: 20188
			public Shader multiScaleAO;

			// Token: 0x04004EDD RID: 20189
			public Shader screenSpaceReflections;

			// Token: 0x06005416 RID: 21526 RVA: 0x001B490F File Offset: 0x001B2B0F
			public PostProcessResources.Shaders Clone()
			{
				return (PostProcessResources.Shaders)base.MemberwiseClone();
			}
		}

		// Token: 0x02000F32 RID: 3890
		[Serializable]
		public sealed class ComputeShaders
		{
			// Token: 0x04004EDE RID: 20190
			public ComputeShader autoExposure;

			// Token: 0x04004EDF RID: 20191
			public ComputeShader exposureHistogram;

			// Token: 0x04004EE0 RID: 20192
			public ComputeShader lut3DBaker;

			// Token: 0x04004EE1 RID: 20193
			public ComputeShader texture3dLerp;

			// Token: 0x04004EE2 RID: 20194
			public ComputeShader gammaHistogram;

			// Token: 0x04004EE3 RID: 20195
			public ComputeShader waveform;

			// Token: 0x04004EE4 RID: 20196
			public ComputeShader vectorscope;

			// Token: 0x04004EE5 RID: 20197
			public ComputeShader multiScaleAODownsample1;

			// Token: 0x04004EE6 RID: 20198
			public ComputeShader multiScaleAODownsample2;

			// Token: 0x04004EE7 RID: 20199
			public ComputeShader multiScaleAORender;

			// Token: 0x04004EE8 RID: 20200
			public ComputeShader multiScaleAOUpsample;

			// Token: 0x04004EE9 RID: 20201
			public ComputeShader gaussianDownsample;

			// Token: 0x06005418 RID: 21528 RVA: 0x001B491C File Offset: 0x001B2B1C
			public PostProcessResources.ComputeShaders Clone()
			{
				return (PostProcessResources.ComputeShaders)base.MemberwiseClone();
			}
		}

		// Token: 0x02000F33 RID: 3891
		[Serializable]
		public sealed class SMAALuts
		{
			// Token: 0x04004EEA RID: 20202
			public Texture2D area;

			// Token: 0x04004EEB RID: 20203
			public Texture2D search;
		}
	}
}
