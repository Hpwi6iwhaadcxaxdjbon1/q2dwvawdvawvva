using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch;
using UnityEngine;

// Token: 0x020005A2 RID: 1442
public class Wearable : MonoBehaviour, IItemSetup, IPrefabPreProcess
{
	// Token: 0x04002365 RID: 9061
	[global::InspectorFlags]
	public Wearable.RemoveSkin removeSkin;

	// Token: 0x04002366 RID: 9062
	[global::InspectorFlags]
	public Wearable.RemoveSkin removeSkinFirstPerson;

	// Token: 0x04002367 RID: 9063
	[global::InspectorFlags]
	public Wearable.RemoveHair removeHair;

	// Token: 0x04002368 RID: 9064
	[global::InspectorFlags]
	public Wearable.DeformHair deformHair;

	// Token: 0x04002369 RID: 9065
	[global::InspectorFlags]
	public Wearable.OccupationSlots occupationUnder;

	// Token: 0x0400236A RID: 9066
	[global::InspectorFlags]
	public Wearable.OccupationSlots occupationOver;

	// Token: 0x0400236B RID: 9067
	public bool showCensorshipCube;

	// Token: 0x0400236C RID: 9068
	public bool showCensorshipCubeBreasts;

	// Token: 0x0400236D RID: 9069
	public bool forceHideCensorshipBreasts;

	// Token: 0x0400236E RID: 9070
	public string followBone;

	// Token: 0x0400236F RID: 9071
	public bool disableRigStripping;

	// Token: 0x04002370 RID: 9072
	public bool overrideDownLimit;

	// Token: 0x04002371 RID: 9073
	public float downLimit = 70f;

	// Token: 0x04002372 RID: 9074
	[HideInInspector]
	public PlayerModelHair playerModelHair;

	// Token: 0x04002373 RID: 9075
	[HideInInspector]
	public PlayerModelHairCap playerModelHairCap;

	// Token: 0x04002374 RID: 9076
	[HideInInspector]
	public WearableReplacementByRace wearableReplacementByRace;

	// Token: 0x04002375 RID: 9077
	[HideInInspector]
	public WearableShadowLod wearableShadowLod;

	// Token: 0x04002376 RID: 9078
	[HideInInspector]
	public List<Renderer> renderers = new List<Renderer>();

	// Token: 0x04002377 RID: 9079
	[HideInInspector]
	public List<PlayerModelSkin> playerModelSkins = new List<PlayerModelSkin>();

	// Token: 0x04002378 RID: 9080
	[HideInInspector]
	public List<BoneRetarget> boneRetargets = new List<BoneRetarget>();

	// Token: 0x04002379 RID: 9081
	[HideInInspector]
	public List<SkinnedMeshRenderer> skinnedRenderers = new List<SkinnedMeshRenderer>();

	// Token: 0x0400237A RID: 9082
	[HideInInspector]
	public List<SkeletonSkin> skeletonSkins = new List<SkeletonSkin>();

	// Token: 0x0400237B RID: 9083
	[HideInInspector]
	public List<ComponentInfo> componentInfos = new List<ComponentInfo>();

	// Token: 0x0400237C RID: 9084
	public bool HideInEyesView;

	// Token: 0x0400237D RID: 9085
	[Header("First Person Legs")]
	[Tooltip("If this is true, we'll hide this item in the first person view. Usually done for items that you definitely won't see in first person view, like facemasks and hats.")]
	public bool HideInFirstPerson;

	// Token: 0x0400237E RID: 9086
	[Tooltip("Use this if the clothing item clips into the player view. It'll push the chest legs model backwards.")]
	[Range(0f, 5f)]
	public float ExtraLeanBack;

	// Token: 0x0400237F RID: 9087
	[Tooltip("Enable this to check for BoneRetargets which need to be preserved in first person view")]
	public bool PreserveBones;

	// Token: 0x04002380 RID: 9088
	public Renderer[] RenderersLod0;

	// Token: 0x04002381 RID: 9089
	public Renderer[] RenderersLod1;

	// Token: 0x04002382 RID: 9090
	public Renderer[] RenderersLod2;

	// Token: 0x04002383 RID: 9091
	public Renderer[] RenderersLod3;

	// Token: 0x04002384 RID: 9092
	public Renderer[] SkipInFirstPersonLegs;

	// Token: 0x04002385 RID: 9093
	private static LOD[] emptyLOD = new LOD[1];

	// Token: 0x06002BE2 RID: 11234 RVA: 0x000063A5 File Offset: 0x000045A5
	public void OnItemSetup(Item item)
	{
	}

	// Token: 0x06002BE3 RID: 11235 RVA: 0x00109B9C File Offset: 0x00107D9C
	public virtual void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		foreach (LODGroup lodgroup in base.GetComponentsInChildren<LODGroup>(true))
		{
			lodgroup.SetLODs(Wearable.emptyLOD);
			preProcess.RemoveComponent(lodgroup);
		}
	}

	// Token: 0x06002BE4 RID: 11236 RVA: 0x00109BD8 File Offset: 0x00107DD8
	public void CacheComponents()
	{
		this.playerModelHairCap = base.GetComponent<PlayerModelHairCap>();
		this.playerModelHair = base.GetComponent<PlayerModelHair>();
		this.wearableReplacementByRace = base.GetComponent<WearableReplacementByRace>();
		this.wearableShadowLod = base.GetComponent<WearableShadowLod>();
		base.GetComponentsInChildren<Renderer>(true, this.renderers);
		base.GetComponentsInChildren<PlayerModelSkin>(true, this.playerModelSkins);
		base.GetComponentsInChildren<BoneRetarget>(true, this.boneRetargets);
		base.GetComponentsInChildren<SkinnedMeshRenderer>(true, this.skinnedRenderers);
		base.GetComponentsInChildren<SkeletonSkin>(true, this.skeletonSkins);
		base.GetComponentsInChildren<ComponentInfo>(true, this.componentInfos);
		this.RenderersLod0 = (from x in this.renderers
		where x.gameObject.name.EndsWith("0")
		select x).ToArray<Renderer>();
		this.RenderersLod1 = (from x in this.renderers
		where x.gameObject.name.EndsWith("1")
		select x).ToArray<Renderer>();
		this.RenderersLod2 = (from x in this.renderers
		where x.gameObject.name.EndsWith("2")
		select x).ToArray<Renderer>();
		this.RenderersLod3 = (from x in this.renderers
		where x.gameObject.name.EndsWith("3")
		select x).ToArray<Renderer>();
		foreach (Renderer renderer in this.renderers)
		{
			renderer.gameObject.AddComponent<ObjectMotionVectorFix>();
			renderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
		}
	}

	// Token: 0x06002BE5 RID: 11237 RVA: 0x00109D88 File Offset: 0x00107F88
	public void StripRig(IPrefabProcessor preProcess, SkinnedMeshRenderer skinnedMeshRenderer)
	{
		if (this.disableRigStripping)
		{
			return;
		}
		Transform transform = skinnedMeshRenderer.FindRig();
		if (transform != null)
		{
			List<Transform> list = Pool.GetList<Transform>();
			transform.GetComponentsInChildren<Transform>(list);
			for (int i = list.Count - 1; i >= 0; i--)
			{
				if (preProcess != null)
				{
					preProcess.NominateForDeletion(list[i].gameObject);
				}
				else
				{
					UnityEngine.Object.DestroyImmediate(list[i].gameObject);
				}
			}
			Pool.FreeList<Transform>(ref list);
		}
	}

	// Token: 0x06002BE6 RID: 11238 RVA: 0x000063A5 File Offset: 0x000045A5
	public void SetupRendererCache(IPrefabProcessor preProcess)
	{
	}

	// Token: 0x02000D61 RID: 3425
	[Flags]
	public enum RemoveSkin
	{
		// Token: 0x0400472C RID: 18220
		Torso = 1,
		// Token: 0x0400472D RID: 18221
		Feet = 2,
		// Token: 0x0400472E RID: 18222
		Hands = 4,
		// Token: 0x0400472F RID: 18223
		Legs = 8,
		// Token: 0x04004730 RID: 18224
		Head = 16
	}

	// Token: 0x02000D62 RID: 3426
	[Flags]
	public enum RemoveHair
	{
		// Token: 0x04004732 RID: 18226
		Head = 1,
		// Token: 0x04004733 RID: 18227
		Eyebrow = 2,
		// Token: 0x04004734 RID: 18228
		Facial = 4,
		// Token: 0x04004735 RID: 18229
		Armpit = 8,
		// Token: 0x04004736 RID: 18230
		Pubic = 16
	}

	// Token: 0x02000D63 RID: 3427
	[Flags]
	public enum DeformHair
	{
		// Token: 0x04004738 RID: 18232
		None = 0,
		// Token: 0x04004739 RID: 18233
		BaseballCap = 1,
		// Token: 0x0400473A RID: 18234
		BoonieHat = 2,
		// Token: 0x0400473B RID: 18235
		CandleHat = 3,
		// Token: 0x0400473C RID: 18236
		MinersHat = 4,
		// Token: 0x0400473D RID: 18237
		WoodHelmet = 5
	}

	// Token: 0x02000D64 RID: 3428
	[Flags]
	public enum OccupationSlots
	{
		// Token: 0x0400473F RID: 18239
		HeadTop = 1,
		// Token: 0x04004740 RID: 18240
		Face = 2,
		// Token: 0x04004741 RID: 18241
		HeadBack = 4,
		// Token: 0x04004742 RID: 18242
		TorsoFront = 8,
		// Token: 0x04004743 RID: 18243
		TorsoBack = 16,
		// Token: 0x04004744 RID: 18244
		LeftShoulder = 32,
		// Token: 0x04004745 RID: 18245
		RightShoulder = 64,
		// Token: 0x04004746 RID: 18246
		LeftArm = 128,
		// Token: 0x04004747 RID: 18247
		RightArm = 256,
		// Token: 0x04004748 RID: 18248
		LeftHand = 512,
		// Token: 0x04004749 RID: 18249
		RightHand = 1024,
		// Token: 0x0400474A RID: 18250
		Groin = 2048,
		// Token: 0x0400474B RID: 18251
		Bum = 4096,
		// Token: 0x0400474C RID: 18252
		LeftKnee = 8192,
		// Token: 0x0400474D RID: 18253
		RightKnee = 16384,
		// Token: 0x0400474E RID: 18254
		LeftLeg = 32768,
		// Token: 0x0400474F RID: 18255
		RightLeg = 65536,
		// Token: 0x04004750 RID: 18256
		LeftFoot = 131072,
		// Token: 0x04004751 RID: 18257
		RightFoot = 262144,
		// Token: 0x04004752 RID: 18258
		Mouth = 524288,
		// Token: 0x04004753 RID: 18259
		Eyes = 1048576
	}
}
