using System;
using Facepunch;
using UnityEngine;

// Token: 0x020002D1 RID: 721
public class Model : MonoBehaviour, IPrefabPreProcess
{
	// Token: 0x040016BF RID: 5823
	public SphereCollider collision;

	// Token: 0x040016C0 RID: 5824
	public Transform rootBone;

	// Token: 0x040016C1 RID: 5825
	public Transform headBone;

	// Token: 0x040016C2 RID: 5826
	public Transform eyeBone;

	// Token: 0x040016C3 RID: 5827
	public Animator animator;

	// Token: 0x040016C4 RID: 5828
	public Skeleton skeleton;

	// Token: 0x040016C5 RID: 5829
	[HideInInspector]
	public Transform[] boneTransforms;

	// Token: 0x040016C6 RID: 5830
	[HideInInspector]
	public string[] boneNames;

	// Token: 0x040016C7 RID: 5831
	internal BoneDictionary boneDict;

	// Token: 0x040016C8 RID: 5832
	internal int skin;

	// Token: 0x06001D9C RID: 7580 RVA: 0x000CB41F File Offset: 0x000C961F
	protected void OnEnable()
	{
		this.skin = -1;
	}

	// Token: 0x06001D9D RID: 7581 RVA: 0x000CB428 File Offset: 0x000C9628
	public void BuildBoneDictionary()
	{
		if (this.boneDict != null)
		{
			return;
		}
		this.boneDict = new BoneDictionary(base.transform, this.boneTransforms, this.boneNames);
	}

	// Token: 0x06001D9E RID: 7582 RVA: 0x000CB450 File Offset: 0x000C9650
	public int GetSkin()
	{
		return this.skin;
	}

	// Token: 0x06001D9F RID: 7583 RVA: 0x000CB458 File Offset: 0x000C9658
	private Transform FindBoneInternal(string name)
	{
		this.BuildBoneDictionary();
		return this.boneDict.FindBone(name, false);
	}

	// Token: 0x06001DA0 RID: 7584 RVA: 0x000CB470 File Offset: 0x000C9670
	public Transform FindBone(string name)
	{
		this.BuildBoneDictionary();
		Transform result = this.rootBone;
		if (string.IsNullOrEmpty(name))
		{
			return result;
		}
		return this.boneDict.FindBone(name, true);
	}

	// Token: 0x06001DA1 RID: 7585 RVA: 0x000CB4A4 File Offset: 0x000C96A4
	public Transform FindBone(uint hash)
	{
		this.BuildBoneDictionary();
		Transform result = this.rootBone;
		if (hash == 0U)
		{
			return result;
		}
		return this.boneDict.FindBone(hash, true);
	}

	// Token: 0x06001DA2 RID: 7586 RVA: 0x000CB4D2 File Offset: 0x000C96D2
	public uint FindBoneID(Transform transform)
	{
		this.BuildBoneDictionary();
		return this.boneDict.FindBoneID(transform);
	}

	// Token: 0x06001DA3 RID: 7587 RVA: 0x000CB4E6 File Offset: 0x000C96E6
	public Transform[] GetBones()
	{
		this.BuildBoneDictionary();
		return this.boneDict.transforms;
	}

	// Token: 0x06001DA4 RID: 7588 RVA: 0x000CB4FC File Offset: 0x000C96FC
	public Transform FindClosestBone(Vector3 worldPos)
	{
		Transform result = this.rootBone;
		float num = float.MaxValue;
		for (int i = 0; i < this.boneTransforms.Length; i++)
		{
			Transform transform = this.boneTransforms[i];
			if (!(transform == null))
			{
				float num2 = Vector3.Distance(transform.position, worldPos);
				if (num2 < num)
				{
					result = transform;
					num = num2;
				}
			}
		}
		return result;
	}

	// Token: 0x06001DA5 RID: 7589 RVA: 0x000CB554 File Offset: 0x000C9754
	public void PreProcess(IPrefabProcessor process, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if (this == null)
		{
			return;
		}
		if (this.animator == null)
		{
			this.animator = base.GetComponent<Animator>();
		}
		if (this.rootBone == null)
		{
			this.rootBone = base.transform;
		}
		this.boneTransforms = this.rootBone.GetComponentsInChildren<Transform>(true);
		this.boneNames = new string[this.boneTransforms.Length];
		for (int i = 0; i < this.boneTransforms.Length; i++)
		{
			this.boneNames[i] = this.boneTransforms[i].name;
		}
	}
}
