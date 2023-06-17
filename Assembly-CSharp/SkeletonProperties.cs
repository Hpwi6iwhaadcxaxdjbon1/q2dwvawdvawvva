using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x0200055C RID: 1372
[CreateAssetMenu(menuName = "Rust/Skeleton Properties")]
public class SkeletonProperties : ScriptableObject
{
	// Token: 0x04002264 RID: 8804
	public GameObject boneReference;

	// Token: 0x04002265 RID: 8805
	[BoneProperty]
	public SkeletonProperties.BoneProperty[] bones;

	// Token: 0x04002266 RID: 8806
	[NonSerialized]
	private Dictionary<uint, SkeletonProperties.BoneProperty> quickLookup;

	// Token: 0x06002A3F RID: 10815 RVA: 0x00101624 File Offset: 0x000FF824
	public void OnValidate()
	{
		if (this.boneReference == null)
		{
			Debug.LogWarning("boneReference is null on " + base.name, this);
			return;
		}
		List<SkeletonProperties.BoneProperty> list = this.bones.ToList<SkeletonProperties.BoneProperty>();
		using (List<Transform>.Enumerator enumerator = this.boneReference.transform.GetAllChildren().GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Transform child = enumerator.Current;
				if (list.All((SkeletonProperties.BoneProperty x) => x.bone != child.gameObject))
				{
					list.Add(new SkeletonProperties.BoneProperty
					{
						bone = child.gameObject,
						name = new Translate.Phrase("", "")
						{
							token = child.name.ToLower(),
							english = child.name.ToLower()
						}
					});
				}
			}
		}
		this.bones = list.ToArray();
	}

	// Token: 0x06002A40 RID: 10816 RVA: 0x0010173C File Offset: 0x000FF93C
	private void BuildDictionary()
	{
		this.quickLookup = new Dictionary<uint, SkeletonProperties.BoneProperty>();
		if (this.boneReference == null)
		{
			Debug.LogWarning("boneReference is null on " + base.name, this);
			return;
		}
		foreach (SkeletonProperties.BoneProperty boneProperty in this.bones)
		{
			if (boneProperty == null || boneProperty.bone == null || boneProperty.bone.name == null)
			{
				Debug.LogWarning("Bone error in SkeletonProperties.BuildDictionary for " + this.boneReference.name);
			}
			else
			{
				uint num = StringPool.Get(boneProperty.bone.name);
				if (!this.quickLookup.ContainsKey(num))
				{
					this.quickLookup.Add(num, boneProperty);
				}
				else
				{
					string name = boneProperty.bone.name;
					string name2 = this.quickLookup[num].bone.name;
					Debug.LogWarning(string.Concat(new object[]
					{
						"Duplicate bone id ",
						num,
						" for ",
						name,
						" and ",
						name2
					}));
				}
			}
		}
	}

	// Token: 0x06002A41 RID: 10817 RVA: 0x00101864 File Offset: 0x000FFA64
	public SkeletonProperties.BoneProperty FindBone(uint id)
	{
		if (this.quickLookup == null)
		{
			this.BuildDictionary();
		}
		SkeletonProperties.BoneProperty result = null;
		if (!this.quickLookup.TryGetValue(id, out result))
		{
			return null;
		}
		return result;
	}

	// Token: 0x02000D47 RID: 3399
	[Serializable]
	public class BoneProperty
	{
		// Token: 0x040046CF RID: 18127
		public GameObject bone;

		// Token: 0x040046D0 RID: 18128
		public Translate.Phrase name;

		// Token: 0x040046D1 RID: 18129
		public HitArea area;
	}
}
