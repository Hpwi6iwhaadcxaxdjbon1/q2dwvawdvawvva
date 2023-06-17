using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

namespace VLB
{
	// Token: 0x020009AC RID: 2476
	public static class Consts
	{
		// Token: 0x04003587 RID: 13703
		private const string HelpUrlBase = "http://saladgamer.com/vlb-doc/";

		// Token: 0x04003588 RID: 13704
		public const string HelpUrlBeam = "http://saladgamer.com/vlb-doc/comp-lightbeam/";

		// Token: 0x04003589 RID: 13705
		public const string HelpUrlDustParticles = "http://saladgamer.com/vlb-doc/comp-dustparticles/";

		// Token: 0x0400358A RID: 13706
		public const string HelpUrlDynamicOcclusion = "http://saladgamer.com/vlb-doc/comp-dynocclusion/";

		// Token: 0x0400358B RID: 13707
		public const string HelpUrlTriggerZone = "http://saladgamer.com/vlb-doc/comp-triggerzone/";

		// Token: 0x0400358C RID: 13708
		public const string HelpUrlConfig = "http://saladgamer.com/vlb-doc/config/";

		// Token: 0x0400358D RID: 13709
		public static readonly bool ProceduralObjectsVisibleInEditor = true;

		// Token: 0x0400358E RID: 13710
		public static readonly Color FlatColor = Color.white;

		// Token: 0x0400358F RID: 13711
		public const ColorMode ColorModeDefault = ColorMode.Flat;

		// Token: 0x04003590 RID: 13712
		public const float Alpha = 1f;

		// Token: 0x04003591 RID: 13713
		public const float SpotAngleDefault = 35f;

		// Token: 0x04003592 RID: 13714
		public const float SpotAngleMin = 0.1f;

		// Token: 0x04003593 RID: 13715
		public const float SpotAngleMax = 179.9f;

		// Token: 0x04003594 RID: 13716
		public const float ConeRadiusStart = 0.1f;

		// Token: 0x04003595 RID: 13717
		public const MeshType GeomMeshType = MeshType.Shared;

		// Token: 0x04003596 RID: 13718
		public const int GeomSidesDefault = 18;

		// Token: 0x04003597 RID: 13719
		public const int GeomSidesMin = 3;

		// Token: 0x04003598 RID: 13720
		public const int GeomSidesMax = 256;

		// Token: 0x04003599 RID: 13721
		public const int GeomSegmentsDefault = 5;

		// Token: 0x0400359A RID: 13722
		public const int GeomSegmentsMin = 0;

		// Token: 0x0400359B RID: 13723
		public const int GeomSegmentsMax = 64;

		// Token: 0x0400359C RID: 13724
		public const bool GeomCap = false;

		// Token: 0x0400359D RID: 13725
		public const AttenuationEquation AttenuationEquationDefault = AttenuationEquation.Quadratic;

		// Token: 0x0400359E RID: 13726
		public const float AttenuationCustomBlending = 0.5f;

		// Token: 0x0400359F RID: 13727
		public const float FadeStart = 0f;

		// Token: 0x040035A0 RID: 13728
		public const float FadeEnd = 3f;

		// Token: 0x040035A1 RID: 13729
		public const float FadeMinThreshold = 0.01f;

		// Token: 0x040035A2 RID: 13730
		public const float DepthBlendDistance = 2f;

		// Token: 0x040035A3 RID: 13731
		public const float CameraClippingDistance = 0.5f;

		// Token: 0x040035A4 RID: 13732
		public const float FresnelPowMaxValue = 10f;

		// Token: 0x040035A5 RID: 13733
		public const float FresnelPow = 8f;

		// Token: 0x040035A6 RID: 13734
		public const float GlareFrontal = 0.5f;

		// Token: 0x040035A7 RID: 13735
		public const float GlareBehind = 0.5f;

		// Token: 0x040035A8 RID: 13736
		public const float NoiseIntensityMin = 0f;

		// Token: 0x040035A9 RID: 13737
		public const float NoiseIntensityMax = 1f;

		// Token: 0x040035AA RID: 13738
		public const float NoiseIntensityDefault = 0.5f;

		// Token: 0x040035AB RID: 13739
		public const float NoiseScaleMin = 0.01f;

		// Token: 0x040035AC RID: 13740
		public const float NoiseScaleMax = 2f;

		// Token: 0x040035AD RID: 13741
		public const float NoiseScaleDefault = 0.5f;

		// Token: 0x040035AE RID: 13742
		public static readonly Vector3 NoiseVelocityDefault = new Vector3(0.07f, 0.18f, 0.05f);

		// Token: 0x040035AF RID: 13743
		public const BlendingMode BlendingModeDefault = BlendingMode.Additive;

		// Token: 0x040035B0 RID: 13744
		public static readonly BlendMode[] BlendingMode_SrcFactor;

		// Token: 0x040035B1 RID: 13745
		public static readonly BlendMode[] BlendingMode_DstFactor;

		// Token: 0x040035B2 RID: 13746
		public static readonly bool[] BlendingMode_AlphaAsBlack;

		// Token: 0x040035B3 RID: 13747
		public const float DynOcclusionMinSurfaceRatioDefault = 0.5f;

		// Token: 0x040035B4 RID: 13748
		public const float DynOcclusionMinSurfaceRatioMin = 50f;

		// Token: 0x040035B5 RID: 13749
		public const float DynOcclusionMinSurfaceRatioMax = 100f;

		// Token: 0x040035B6 RID: 13750
		public const float DynOcclusionMaxSurfaceDotDefault = 0.25f;

		// Token: 0x040035B7 RID: 13751
		public const float DynOcclusionMaxSurfaceAngleMin = 45f;

		// Token: 0x040035B8 RID: 13752
		public const float DynOcclusionMaxSurfaceAngleMax = 90f;

		// Token: 0x040035B9 RID: 13753
		public const int ConfigGeometryLayerIDDefault = 1;

		// Token: 0x040035BA RID: 13754
		public const string ConfigGeometryTagDefault = "Untagged";

		// Token: 0x040035BB RID: 13755
		public const RenderQueue ConfigGeometryRenderQueueDefault = RenderQueue.Transparent;

		// Token: 0x040035BC RID: 13756
		public const bool ConfigGeometryForceSinglePassDefault = false;

		// Token: 0x040035BD RID: 13757
		public const int ConfigNoise3DSizeDefault = 64;

		// Token: 0x040035BE RID: 13758
		public const int ConfigSharedMeshSides = 24;

		// Token: 0x040035BF RID: 13759
		public const int ConfigSharedMeshSegments = 5;

		// Token: 0x170004B3 RID: 1203
		// (get) Token: 0x06003B2B RID: 15147 RVA: 0x0015ED45 File Offset: 0x0015CF45
		public static HideFlags ProceduralObjectsHideFlags
		{
			get
			{
				if (!Consts.ProceduralObjectsVisibleInEditor)
				{
					return HideFlags.HideAndDontSave;
				}
				return HideFlags.DontSaveInEditor | HideFlags.NotEditable | HideFlags.DontSaveInBuild | HideFlags.DontUnloadUnusedAsset;
			}
		}

		// Token: 0x06003B2C RID: 15148 RVA: 0x0015ED54 File Offset: 0x0015CF54
		// Note: this type is marked as 'beforefieldinit'.
		static Consts()
		{
			BlendMode[] array = new BlendMode[3];
			RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.42A4001D1CFDC98C761C0CFE5497A75F739D92F8).FieldHandle);
			Consts.BlendingMode_SrcFactor = array;
			BlendMode[] array2 = new BlendMode[3];
			RuntimeHelpers.InitializeArray(array2, fieldof(<PrivateImplementationDetails>.D950356082C70AD4018410AD313BA99D655D4D4A).FieldHandle);
			Consts.BlendingMode_DstFactor = array2;
			bool[] array3 = new bool[3];
			array3[0] = true;
			array3[1] = true;
			Consts.BlendingMode_AlphaAsBlack = array3;
		}
	}
}
