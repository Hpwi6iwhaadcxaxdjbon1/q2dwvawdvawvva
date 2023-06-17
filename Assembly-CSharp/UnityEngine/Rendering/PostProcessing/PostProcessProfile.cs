using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A92 RID: 2706
	public sealed class PostProcessProfile : ScriptableObject
	{
		// Token: 0x04003985 RID: 14725
		[Tooltip("A list of all settings currently stored in this profile.")]
		public List<PostProcessEffectSettings> settings = new List<PostProcessEffectSettings>();

		// Token: 0x04003986 RID: 14726
		[NonSerialized]
		public bool isDirty = true;

		// Token: 0x06004070 RID: 16496 RVA: 0x0017C3C1 File Offset: 0x0017A5C1
		private void OnEnable()
		{
			this.settings.RemoveAll((PostProcessEffectSettings x) => x == null);
		}

		// Token: 0x06004071 RID: 16497 RVA: 0x0017C3EE File Offset: 0x0017A5EE
		public T AddSettings<T>() where T : PostProcessEffectSettings
		{
			return (T)((object)this.AddSettings(typeof(T)));
		}

		// Token: 0x06004072 RID: 16498 RVA: 0x0017C408 File Offset: 0x0017A608
		public PostProcessEffectSettings AddSettings(Type type)
		{
			if (this.HasSettings(type))
			{
				throw new InvalidOperationException("Effect already exists in the stack");
			}
			PostProcessEffectSettings postProcessEffectSettings = (PostProcessEffectSettings)ScriptableObject.CreateInstance(type);
			postProcessEffectSettings.hideFlags = (HideFlags.HideInHierarchy | HideFlags.HideInInspector);
			postProcessEffectSettings.name = type.Name;
			postProcessEffectSettings.enabled.value = true;
			this.settings.Add(postProcessEffectSettings);
			this.isDirty = true;
			return postProcessEffectSettings;
		}

		// Token: 0x06004073 RID: 16499 RVA: 0x0017C468 File Offset: 0x0017A668
		public PostProcessEffectSettings AddSettings(PostProcessEffectSettings effect)
		{
			if (this.HasSettings(this.settings.GetType()))
			{
				throw new InvalidOperationException("Effect already exists in the stack");
			}
			this.settings.Add(effect);
			this.isDirty = true;
			return effect;
		}

		// Token: 0x06004074 RID: 16500 RVA: 0x0017C49C File Offset: 0x0017A69C
		public void RemoveSettings<T>() where T : PostProcessEffectSettings
		{
			this.RemoveSettings(typeof(T));
		}

		// Token: 0x06004075 RID: 16501 RVA: 0x0017C4B0 File Offset: 0x0017A6B0
		public void RemoveSettings(Type type)
		{
			int num = -1;
			for (int i = 0; i < this.settings.Count; i++)
			{
				if (this.settings[i].GetType() == type)
				{
					num = i;
					break;
				}
			}
			if (num < 0)
			{
				throw new InvalidOperationException("Effect doesn't exist in the profile");
			}
			this.settings.RemoveAt(num);
			this.isDirty = true;
		}

		// Token: 0x06004076 RID: 16502 RVA: 0x0017C514 File Offset: 0x0017A714
		public bool HasSettings<T>() where T : PostProcessEffectSettings
		{
			return this.HasSettings(typeof(T));
		}

		// Token: 0x06004077 RID: 16503 RVA: 0x0017C528 File Offset: 0x0017A728
		public bool HasSettings(Type type)
		{
			using (List<PostProcessEffectSettings>.Enumerator enumerator = this.settings.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.GetType() == type)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06004078 RID: 16504 RVA: 0x0017C588 File Offset: 0x0017A788
		public T GetSetting<T>() where T : PostProcessEffectSettings
		{
			foreach (PostProcessEffectSettings postProcessEffectSettings in this.settings)
			{
				if (postProcessEffectSettings is T)
				{
					return postProcessEffectSettings as T;
				}
			}
			return default(T);
		}

		// Token: 0x06004079 RID: 16505 RVA: 0x0017C5F8 File Offset: 0x0017A7F8
		public bool TryGetSettings<T>(out T outSetting) where T : PostProcessEffectSettings
		{
			Type typeFromHandle = typeof(T);
			outSetting = default(T);
			foreach (PostProcessEffectSettings postProcessEffectSettings in this.settings)
			{
				if (postProcessEffectSettings.GetType() == typeFromHandle)
				{
					outSetting = (T)((object)postProcessEffectSettings);
					return true;
				}
			}
			return false;
		}
	}
}
