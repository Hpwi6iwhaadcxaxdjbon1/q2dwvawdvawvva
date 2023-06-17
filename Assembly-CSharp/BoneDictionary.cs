using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000293 RID: 659
public class BoneDictionary
{
	// Token: 0x040015E5 RID: 5605
	public Transform transform;

	// Token: 0x040015E6 RID: 5606
	public Transform[] transforms;

	// Token: 0x040015E7 RID: 5607
	public string[] names;

	// Token: 0x040015E8 RID: 5608
	private Dictionary<string, Transform> nameDict = new Dictionary<string, Transform>(StringComparer.OrdinalIgnoreCase);

	// Token: 0x040015E9 RID: 5609
	private Dictionary<uint, Transform> hashDict = new Dictionary<uint, Transform>();

	// Token: 0x040015EA RID: 5610
	private Dictionary<Transform, uint> transformDict = new Dictionary<Transform, uint>();

	// Token: 0x17000267 RID: 615
	// (get) Token: 0x06001D15 RID: 7445 RVA: 0x000C9163 File Offset: 0x000C7363
	public int Count
	{
		get
		{
			return this.transforms.Length;
		}
	}

	// Token: 0x06001D16 RID: 7446 RVA: 0x000C9170 File Offset: 0x000C7370
	public BoneDictionary(Transform rootBone)
	{
		this.transform = rootBone;
		this.transforms = rootBone.GetComponentsInChildren<Transform>(true);
		this.names = new string[this.transforms.Length];
		for (int i = 0; i < this.transforms.Length; i++)
		{
			Transform transform = this.transforms[i];
			if (transform != null)
			{
				this.names[i] = transform.name;
			}
		}
		this.BuildBoneDictionary();
	}

	// Token: 0x06001D17 RID: 7447 RVA: 0x000C920C File Offset: 0x000C740C
	public BoneDictionary(Transform rootBone, Transform[] boneTransforms, string[] boneNames)
	{
		this.transform = rootBone;
		this.transforms = boneTransforms;
		this.names = boneNames;
		this.BuildBoneDictionary();
	}

	// Token: 0x06001D18 RID: 7448 RVA: 0x000C9260 File Offset: 0x000C7460
	private void BuildBoneDictionary()
	{
		for (int i = 0; i < this.transforms.Length; i++)
		{
			Transform transform = this.transforms[i];
			string text = this.names[i];
			uint num = StringPool.Get(text);
			if (!this.nameDict.ContainsKey(text))
			{
				this.nameDict.Add(text, transform);
			}
			if (!this.hashDict.ContainsKey(num))
			{
				this.hashDict.Add(num, transform);
			}
			if (transform != null && !this.transformDict.ContainsKey(transform))
			{
				this.transformDict.Add(transform, num);
			}
		}
	}

	// Token: 0x06001D19 RID: 7449 RVA: 0x000C92F8 File Offset: 0x000C74F8
	public Transform FindBone(string name, bool defaultToRoot = true)
	{
		Transform result = null;
		if (this.nameDict.TryGetValue(name, out result))
		{
			return result;
		}
		if (!defaultToRoot)
		{
			return null;
		}
		return this.transform;
	}

	// Token: 0x06001D1A RID: 7450 RVA: 0x000C9324 File Offset: 0x000C7524
	public Transform FindBone(uint hash, bool defaultToRoot = true)
	{
		Transform result = null;
		if (this.hashDict.TryGetValue(hash, out result))
		{
			return result;
		}
		if (!defaultToRoot)
		{
			return null;
		}
		return this.transform;
	}

	// Token: 0x06001D1B RID: 7451 RVA: 0x000C9350 File Offset: 0x000C7550
	public uint FindBoneID(Transform transform)
	{
		uint result;
		if (!this.transformDict.TryGetValue(transform, out result))
		{
			return StringPool.closest;
		}
		return result;
	}
}
