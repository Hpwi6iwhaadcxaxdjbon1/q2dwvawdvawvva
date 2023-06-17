using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A9D RID: 2717
	internal static class ShaderIDs
	{
		// Token: 0x040039BB RID: 14779
		internal static readonly int MainTex = Shader.PropertyToID("_MainTex");

		// Token: 0x040039BC RID: 14780
		internal static readonly int Jitter = Shader.PropertyToID("_Jitter");

		// Token: 0x040039BD RID: 14781
		internal static readonly int Sharpness = Shader.PropertyToID("_Sharpness");

		// Token: 0x040039BE RID: 14782
		internal static readonly int FinalBlendParameters = Shader.PropertyToID("_FinalBlendParameters");

		// Token: 0x040039BF RID: 14783
		internal static readonly int HistoryTex = Shader.PropertyToID("_HistoryTex");

		// Token: 0x040039C0 RID: 14784
		internal static readonly int SMAA_Flip = Shader.PropertyToID("_SMAA_Flip");

		// Token: 0x040039C1 RID: 14785
		internal static readonly int SMAA_Flop = Shader.PropertyToID("_SMAA_Flop");

		// Token: 0x040039C2 RID: 14786
		internal static readonly int AOParams = Shader.PropertyToID("_AOParams");

		// Token: 0x040039C3 RID: 14787
		internal static readonly int AOColor = Shader.PropertyToID("_AOColor");

		// Token: 0x040039C4 RID: 14788
		internal static readonly int OcclusionTexture1 = Shader.PropertyToID("_OcclusionTexture1");

		// Token: 0x040039C5 RID: 14789
		internal static readonly int OcclusionTexture2 = Shader.PropertyToID("_OcclusionTexture2");

		// Token: 0x040039C6 RID: 14790
		internal static readonly int SAOcclusionTexture = Shader.PropertyToID("_SAOcclusionTexture");

		// Token: 0x040039C7 RID: 14791
		internal static readonly int MSVOcclusionTexture = Shader.PropertyToID("_MSVOcclusionTexture");

		// Token: 0x040039C8 RID: 14792
		internal static readonly int DepthCopy = Shader.PropertyToID("DepthCopy");

		// Token: 0x040039C9 RID: 14793
		internal static readonly int LinearDepth = Shader.PropertyToID("LinearDepth");

		// Token: 0x040039CA RID: 14794
		internal static readonly int LowDepth1 = Shader.PropertyToID("LowDepth1");

		// Token: 0x040039CB RID: 14795
		internal static readonly int LowDepth2 = Shader.PropertyToID("LowDepth2");

		// Token: 0x040039CC RID: 14796
		internal static readonly int LowDepth3 = Shader.PropertyToID("LowDepth3");

		// Token: 0x040039CD RID: 14797
		internal static readonly int LowDepth4 = Shader.PropertyToID("LowDepth4");

		// Token: 0x040039CE RID: 14798
		internal static readonly int TiledDepth1 = Shader.PropertyToID("TiledDepth1");

		// Token: 0x040039CF RID: 14799
		internal static readonly int TiledDepth2 = Shader.PropertyToID("TiledDepth2");

		// Token: 0x040039D0 RID: 14800
		internal static readonly int TiledDepth3 = Shader.PropertyToID("TiledDepth3");

		// Token: 0x040039D1 RID: 14801
		internal static readonly int TiledDepth4 = Shader.PropertyToID("TiledDepth4");

		// Token: 0x040039D2 RID: 14802
		internal static readonly int Occlusion1 = Shader.PropertyToID("Occlusion1");

		// Token: 0x040039D3 RID: 14803
		internal static readonly int Occlusion2 = Shader.PropertyToID("Occlusion2");

		// Token: 0x040039D4 RID: 14804
		internal static readonly int Occlusion3 = Shader.PropertyToID("Occlusion3");

		// Token: 0x040039D5 RID: 14805
		internal static readonly int Occlusion4 = Shader.PropertyToID("Occlusion4");

		// Token: 0x040039D6 RID: 14806
		internal static readonly int Combined1 = Shader.PropertyToID("Combined1");

		// Token: 0x040039D7 RID: 14807
		internal static readonly int Combined2 = Shader.PropertyToID("Combined2");

		// Token: 0x040039D8 RID: 14808
		internal static readonly int Combined3 = Shader.PropertyToID("Combined3");

		// Token: 0x040039D9 RID: 14809
		internal static readonly int SSRResolveTemp = Shader.PropertyToID("_SSRResolveTemp");

		// Token: 0x040039DA RID: 14810
		internal static readonly int Noise = Shader.PropertyToID("_Noise");

		// Token: 0x040039DB RID: 14811
		internal static readonly int Test = Shader.PropertyToID("_Test");

		// Token: 0x040039DC RID: 14812
		internal static readonly int Resolve = Shader.PropertyToID("_Resolve");

		// Token: 0x040039DD RID: 14813
		internal static readonly int History = Shader.PropertyToID("_History");

		// Token: 0x040039DE RID: 14814
		internal static readonly int ViewMatrix = Shader.PropertyToID("_ViewMatrix");

		// Token: 0x040039DF RID: 14815
		internal static readonly int InverseViewMatrix = Shader.PropertyToID("_InverseViewMatrix");

		// Token: 0x040039E0 RID: 14816
		internal static readonly int InverseProjectionMatrix = Shader.PropertyToID("_InverseProjectionMatrix");

		// Token: 0x040039E1 RID: 14817
		internal static readonly int ScreenSpaceProjectionMatrix = Shader.PropertyToID("_ScreenSpaceProjectionMatrix");

		// Token: 0x040039E2 RID: 14818
		internal static readonly int Params2 = Shader.PropertyToID("_Params2");

		// Token: 0x040039E3 RID: 14819
		internal static readonly int FogColor = Shader.PropertyToID("_FogColor");

		// Token: 0x040039E4 RID: 14820
		internal static readonly int FogParams = Shader.PropertyToID("_FogParams");

		// Token: 0x040039E5 RID: 14821
		internal static readonly int VelocityScale = Shader.PropertyToID("_VelocityScale");

		// Token: 0x040039E6 RID: 14822
		internal static readonly int MaxBlurRadius = Shader.PropertyToID("_MaxBlurRadius");

		// Token: 0x040039E7 RID: 14823
		internal static readonly int RcpMaxBlurRadius = Shader.PropertyToID("_RcpMaxBlurRadius");

		// Token: 0x040039E8 RID: 14824
		internal static readonly int VelocityTex = Shader.PropertyToID("_VelocityTex");

		// Token: 0x040039E9 RID: 14825
		internal static readonly int Tile2RT = Shader.PropertyToID("_Tile2RT");

		// Token: 0x040039EA RID: 14826
		internal static readonly int Tile4RT = Shader.PropertyToID("_Tile4RT");

		// Token: 0x040039EB RID: 14827
		internal static readonly int Tile8RT = Shader.PropertyToID("_Tile8RT");

		// Token: 0x040039EC RID: 14828
		internal static readonly int TileMaxOffs = Shader.PropertyToID("_TileMaxOffs");

		// Token: 0x040039ED RID: 14829
		internal static readonly int TileMaxLoop = Shader.PropertyToID("_TileMaxLoop");

		// Token: 0x040039EE RID: 14830
		internal static readonly int TileVRT = Shader.PropertyToID("_TileVRT");

		// Token: 0x040039EF RID: 14831
		internal static readonly int NeighborMaxTex = Shader.PropertyToID("_NeighborMaxTex");

		// Token: 0x040039F0 RID: 14832
		internal static readonly int LoopCount = Shader.PropertyToID("_LoopCount");

		// Token: 0x040039F1 RID: 14833
		internal static readonly int DepthOfFieldTemp = Shader.PropertyToID("_DepthOfFieldTemp");

		// Token: 0x040039F2 RID: 14834
		internal static readonly int DepthOfFieldTex = Shader.PropertyToID("_DepthOfFieldTex");

		// Token: 0x040039F3 RID: 14835
		internal static readonly int Distance = Shader.PropertyToID("_Distance");

		// Token: 0x040039F4 RID: 14836
		internal static readonly int LensCoeff = Shader.PropertyToID("_LensCoeff");

		// Token: 0x040039F5 RID: 14837
		internal static readonly int MaxCoC = Shader.PropertyToID("_MaxCoC");

		// Token: 0x040039F6 RID: 14838
		internal static readonly int RcpMaxCoC = Shader.PropertyToID("_RcpMaxCoC");

		// Token: 0x040039F7 RID: 14839
		internal static readonly int RcpAspect = Shader.PropertyToID("_RcpAspect");

		// Token: 0x040039F8 RID: 14840
		internal static readonly int CoCTex = Shader.PropertyToID("_CoCTex");

		// Token: 0x040039F9 RID: 14841
		internal static readonly int TaaParams = Shader.PropertyToID("_TaaParams");

		// Token: 0x040039FA RID: 14842
		internal static readonly int AutoExposureTex = Shader.PropertyToID("_AutoExposureTex");

		// Token: 0x040039FB RID: 14843
		internal static readonly int HistogramBuffer = Shader.PropertyToID("_HistogramBuffer");

		// Token: 0x040039FC RID: 14844
		internal static readonly int Params = Shader.PropertyToID("_Params");

		// Token: 0x040039FD RID: 14845
		internal static readonly int ScaleOffsetRes = Shader.PropertyToID("_ScaleOffsetRes");

		// Token: 0x040039FE RID: 14846
		internal static readonly int BloomTex = Shader.PropertyToID("_BloomTex");

		// Token: 0x040039FF RID: 14847
		internal static readonly int SampleScale = Shader.PropertyToID("_SampleScale");

		// Token: 0x04003A00 RID: 14848
		internal static readonly int Threshold = Shader.PropertyToID("_Threshold");

		// Token: 0x04003A01 RID: 14849
		internal static readonly int ColorIntensity = Shader.PropertyToID("_ColorIntensity");

		// Token: 0x04003A02 RID: 14850
		internal static readonly int Bloom_DirtTex = Shader.PropertyToID("_Bloom_DirtTex");

		// Token: 0x04003A03 RID: 14851
		internal static readonly int Bloom_Settings = Shader.PropertyToID("_Bloom_Settings");

		// Token: 0x04003A04 RID: 14852
		internal static readonly int Bloom_Color = Shader.PropertyToID("_Bloom_Color");

		// Token: 0x04003A05 RID: 14853
		internal static readonly int Bloom_DirtTileOffset = Shader.PropertyToID("_Bloom_DirtTileOffset");

		// Token: 0x04003A06 RID: 14854
		internal static readonly int ChromaticAberration_Amount = Shader.PropertyToID("_ChromaticAberration_Amount");

		// Token: 0x04003A07 RID: 14855
		internal static readonly int ChromaticAberration_SpectralLut = Shader.PropertyToID("_ChromaticAberration_SpectralLut");

		// Token: 0x04003A08 RID: 14856
		internal static readonly int Distortion_CenterScale = Shader.PropertyToID("_Distortion_CenterScale");

		// Token: 0x04003A09 RID: 14857
		internal static readonly int Distortion_Amount = Shader.PropertyToID("_Distortion_Amount");

		// Token: 0x04003A0A RID: 14858
		internal static readonly int Lut2D = Shader.PropertyToID("_Lut2D");

		// Token: 0x04003A0B RID: 14859
		internal static readonly int Lut3D = Shader.PropertyToID("_Lut3D");

		// Token: 0x04003A0C RID: 14860
		internal static readonly int Lut3D_Params = Shader.PropertyToID("_Lut3D_Params");

		// Token: 0x04003A0D RID: 14861
		internal static readonly int Lut2D_Params = Shader.PropertyToID("_Lut2D_Params");

		// Token: 0x04003A0E RID: 14862
		internal static readonly int UserLut2D_Params = Shader.PropertyToID("_UserLut2D_Params");

		// Token: 0x04003A0F RID: 14863
		internal static readonly int PostExposure = Shader.PropertyToID("_PostExposure");

		// Token: 0x04003A10 RID: 14864
		internal static readonly int ColorBalance = Shader.PropertyToID("_ColorBalance");

		// Token: 0x04003A11 RID: 14865
		internal static readonly int ColorFilter = Shader.PropertyToID("_ColorFilter");

		// Token: 0x04003A12 RID: 14866
		internal static readonly int HueSatCon = Shader.PropertyToID("_HueSatCon");

		// Token: 0x04003A13 RID: 14867
		internal static readonly int Brightness = Shader.PropertyToID("_Brightness");

		// Token: 0x04003A14 RID: 14868
		internal static readonly int ChannelMixerRed = Shader.PropertyToID("_ChannelMixerRed");

		// Token: 0x04003A15 RID: 14869
		internal static readonly int ChannelMixerGreen = Shader.PropertyToID("_ChannelMixerGreen");

		// Token: 0x04003A16 RID: 14870
		internal static readonly int ChannelMixerBlue = Shader.PropertyToID("_ChannelMixerBlue");

		// Token: 0x04003A17 RID: 14871
		internal static readonly int Lift = Shader.PropertyToID("_Lift");

		// Token: 0x04003A18 RID: 14872
		internal static readonly int InvGamma = Shader.PropertyToID("_InvGamma");

		// Token: 0x04003A19 RID: 14873
		internal static readonly int Gain = Shader.PropertyToID("_Gain");

		// Token: 0x04003A1A RID: 14874
		internal static readonly int Curves = Shader.PropertyToID("_Curves");

		// Token: 0x04003A1B RID: 14875
		internal static readonly int CustomToneCurve = Shader.PropertyToID("_CustomToneCurve");

		// Token: 0x04003A1C RID: 14876
		internal static readonly int ToeSegmentA = Shader.PropertyToID("_ToeSegmentA");

		// Token: 0x04003A1D RID: 14877
		internal static readonly int ToeSegmentB = Shader.PropertyToID("_ToeSegmentB");

		// Token: 0x04003A1E RID: 14878
		internal static readonly int MidSegmentA = Shader.PropertyToID("_MidSegmentA");

		// Token: 0x04003A1F RID: 14879
		internal static readonly int MidSegmentB = Shader.PropertyToID("_MidSegmentB");

		// Token: 0x04003A20 RID: 14880
		internal static readonly int ShoSegmentA = Shader.PropertyToID("_ShoSegmentA");

		// Token: 0x04003A21 RID: 14881
		internal static readonly int ShoSegmentB = Shader.PropertyToID("_ShoSegmentB");

		// Token: 0x04003A22 RID: 14882
		internal static readonly int Vignette_Color = Shader.PropertyToID("_Vignette_Color");

		// Token: 0x04003A23 RID: 14883
		internal static readonly int Vignette_Center = Shader.PropertyToID("_Vignette_Center");

		// Token: 0x04003A24 RID: 14884
		internal static readonly int Vignette_Settings = Shader.PropertyToID("_Vignette_Settings");

		// Token: 0x04003A25 RID: 14885
		internal static readonly int Vignette_Mask = Shader.PropertyToID("_Vignette_Mask");

		// Token: 0x04003A26 RID: 14886
		internal static readonly int Vignette_Opacity = Shader.PropertyToID("_Vignette_Opacity");

		// Token: 0x04003A27 RID: 14887
		internal static readonly int Vignette_Mode = Shader.PropertyToID("_Vignette_Mode");

		// Token: 0x04003A28 RID: 14888
		internal static readonly int Grain_Params1 = Shader.PropertyToID("_Grain_Params1");

		// Token: 0x04003A29 RID: 14889
		internal static readonly int Grain_Params2 = Shader.PropertyToID("_Grain_Params2");

		// Token: 0x04003A2A RID: 14890
		internal static readonly int GrainTex = Shader.PropertyToID("_GrainTex");

		// Token: 0x04003A2B RID: 14891
		internal static readonly int Phase = Shader.PropertyToID("_Phase");

		// Token: 0x04003A2C RID: 14892
		internal static readonly int GrainNoiseParameters = Shader.PropertyToID("_NoiseParameters");

		// Token: 0x04003A2D RID: 14893
		internal static readonly int LumaInAlpha = Shader.PropertyToID("_LumaInAlpha");

		// Token: 0x04003A2E RID: 14894
		internal static readonly int DitheringTex = Shader.PropertyToID("_DitheringTex");

		// Token: 0x04003A2F RID: 14895
		internal static readonly int Dithering_Coords = Shader.PropertyToID("_Dithering_Coords");

		// Token: 0x04003A30 RID: 14896
		internal static readonly int From = Shader.PropertyToID("_From");

		// Token: 0x04003A31 RID: 14897
		internal static readonly int To = Shader.PropertyToID("_To");

		// Token: 0x04003A32 RID: 14898
		internal static readonly int Interp = Shader.PropertyToID("_Interp");

		// Token: 0x04003A33 RID: 14899
		internal static readonly int TargetColor = Shader.PropertyToID("_TargetColor");

		// Token: 0x04003A34 RID: 14900
		internal static readonly int HalfResFinalCopy = Shader.PropertyToID("_HalfResFinalCopy");

		// Token: 0x04003A35 RID: 14901
		internal static readonly int WaveformSource = Shader.PropertyToID("_WaveformSource");

		// Token: 0x04003A36 RID: 14902
		internal static readonly int WaveformBuffer = Shader.PropertyToID("_WaveformBuffer");

		// Token: 0x04003A37 RID: 14903
		internal static readonly int VectorscopeBuffer = Shader.PropertyToID("_VectorscopeBuffer");

		// Token: 0x04003A38 RID: 14904
		internal static readonly int RenderViewportScaleFactor = Shader.PropertyToID("_RenderViewportScaleFactor");

		// Token: 0x04003A39 RID: 14905
		internal static readonly int UVTransform = Shader.PropertyToID("_UVTransform");

		// Token: 0x04003A3A RID: 14906
		internal static readonly int DepthSlice = Shader.PropertyToID("_DepthSlice");

		// Token: 0x04003A3B RID: 14907
		internal static readonly int UVScaleOffset = Shader.PropertyToID("_UVScaleOffset");

		// Token: 0x04003A3C RID: 14908
		internal static readonly int PosScaleOffset = Shader.PropertyToID("_PosScaleOffset");
	}
}
