using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A91 RID: 2705
	public sealed class PostProcessManager
	{
		// Token: 0x0400397E RID: 14718
		private static PostProcessManager s_Instance;

		// Token: 0x0400397F RID: 14719
		private const int k_MaxLayerCount = 32;

		// Token: 0x04003980 RID: 14720
		private readonly Dictionary<int, List<PostProcessVolume>> m_SortedVolumes;

		// Token: 0x04003981 RID: 14721
		private readonly List<PostProcessVolume> m_Volumes;

		// Token: 0x04003982 RID: 14722
		private readonly Dictionary<int, bool> m_SortNeeded;

		// Token: 0x04003983 RID: 14723
		private readonly List<PostProcessEffectSettings> m_BaseSettings;

		// Token: 0x04003984 RID: 14724
		public readonly Dictionary<Type, PostProcessAttribute> settingsTypes;

		// Token: 0x1700057F RID: 1407
		// (get) Token: 0x0600405D RID: 16477 RVA: 0x0017BAC4 File Offset: 0x00179CC4
		public static PostProcessManager instance
		{
			get
			{
				if (PostProcessManager.s_Instance == null)
				{
					PostProcessManager.s_Instance = new PostProcessManager();
				}
				return PostProcessManager.s_Instance;
			}
		}

		// Token: 0x0600405E RID: 16478 RVA: 0x0017BADC File Offset: 0x00179CDC
		private PostProcessManager()
		{
			this.m_SortedVolumes = new Dictionary<int, List<PostProcessVolume>>();
			this.m_Volumes = new List<PostProcessVolume>();
			this.m_SortNeeded = new Dictionary<int, bool>();
			this.m_BaseSettings = new List<PostProcessEffectSettings>();
			this.settingsTypes = new Dictionary<Type, PostProcessAttribute>();
			this.ReloadBaseTypes();
		}

		// Token: 0x0600405F RID: 16479 RVA: 0x0017BB2C File Offset: 0x00179D2C
		private void CleanBaseTypes()
		{
			this.settingsTypes.Clear();
			foreach (PostProcessEffectSettings obj in this.m_BaseSettings)
			{
				RuntimeUtilities.Destroy(obj);
			}
			this.m_BaseSettings.Clear();
		}

		// Token: 0x06004060 RID: 16480 RVA: 0x0017BB94 File Offset: 0x00179D94
		private void ReloadBaseTypes()
		{
			this.CleanBaseTypes();
			foreach (Type type in from t in RuntimeUtilities.GetAllAssemblyTypes()
			where t.IsSubclassOf(typeof(PostProcessEffectSettings)) && t.IsDefined(typeof(PostProcessAttribute), false) && !t.IsAbstract
			select t)
			{
				this.settingsTypes.Add(type, type.GetAttribute<PostProcessAttribute>());
				PostProcessEffectSettings postProcessEffectSettings = (PostProcessEffectSettings)ScriptableObject.CreateInstance(type);
				postProcessEffectSettings.SetAllOverridesTo(true, false);
				this.m_BaseSettings.Add(postProcessEffectSettings);
			}
		}

		// Token: 0x06004061 RID: 16481 RVA: 0x0017BC38 File Offset: 0x00179E38
		public void GetActiveVolumes(PostProcessLayer layer, List<PostProcessVolume> results, bool skipDisabled = true, bool skipZeroWeight = true)
		{
			int value = layer.volumeLayer.value;
			Transform volumeTrigger = layer.volumeTrigger;
			bool flag = volumeTrigger == null;
			Vector3 vector = flag ? Vector3.zero : volumeTrigger.position;
			foreach (PostProcessVolume postProcessVolume in this.GrabVolumes(value))
			{
				if ((!skipDisabled || postProcessVolume.enabled) && !(postProcessVolume.profileRef == null) && (!skipZeroWeight || postProcessVolume.weight > 0f))
				{
					if (postProcessVolume.isGlobal)
					{
						results.Add(postProcessVolume);
					}
					else if (!flag)
					{
						OBB obb = new OBB(postProcessVolume.transform, postProcessVolume.bounds);
						float sqrMagnitude = ((obb.ClosestPoint(vector) - vector) / 2f).sqrMagnitude;
						float num = postProcessVolume.blendDistance * postProcessVolume.blendDistance;
						if (sqrMagnitude <= num)
						{
							results.Add(postProcessVolume);
						}
					}
				}
			}
		}

		// Token: 0x06004062 RID: 16482 RVA: 0x0017BD58 File Offset: 0x00179F58
		public PostProcessVolume GetHighestPriorityVolume(PostProcessLayer layer)
		{
			if (layer == null)
			{
				throw new ArgumentNullException("layer");
			}
			return this.GetHighestPriorityVolume(layer.volumeLayer);
		}

		// Token: 0x06004063 RID: 16483 RVA: 0x0017BD7C File Offset: 0x00179F7C
		public PostProcessVolume GetHighestPriorityVolume(LayerMask mask)
		{
			float num = float.NegativeInfinity;
			PostProcessVolume result = null;
			List<PostProcessVolume> list;
			if (this.m_SortedVolumes.TryGetValue(mask, out list))
			{
				foreach (PostProcessVolume postProcessVolume in list)
				{
					if (postProcessVolume.priority > num)
					{
						num = postProcessVolume.priority;
						result = postProcessVolume;
					}
				}
			}
			return result;
		}

		// Token: 0x06004064 RID: 16484 RVA: 0x0017BDF8 File Offset: 0x00179FF8
		public PostProcessVolume QuickVolume(int layer, float priority, params PostProcessEffectSettings[] settings)
		{
			PostProcessVolume postProcessVolume = new GameObject
			{
				name = "Quick Volume",
				layer = layer,
				hideFlags = HideFlags.HideAndDontSave
			}.AddComponent<PostProcessVolume>();
			postProcessVolume.priority = priority;
			postProcessVolume.isGlobal = true;
			PostProcessProfile profile = postProcessVolume.profile;
			foreach (PostProcessEffectSettings postProcessEffectSettings in settings)
			{
				Assert.IsNotNull<PostProcessEffectSettings>(postProcessEffectSettings, "Trying to create a volume with null effects");
				profile.AddSettings(postProcessEffectSettings);
			}
			return postProcessVolume;
		}

		// Token: 0x06004065 RID: 16485 RVA: 0x0017BE6C File Offset: 0x0017A06C
		internal void SetLayerDirty(int layer)
		{
			Assert.IsTrue(layer >= 0 && layer <= 32, "Invalid layer bit");
			foreach (KeyValuePair<int, List<PostProcessVolume>> keyValuePair in this.m_SortedVolumes)
			{
				int key = keyValuePair.Key;
				if ((key & 1 << layer) != 0)
				{
					this.m_SortNeeded[key] = true;
				}
			}
		}

		// Token: 0x06004066 RID: 16486 RVA: 0x0017BEF0 File Offset: 0x0017A0F0
		internal void UpdateVolumeLayer(PostProcessVolume volume, int prevLayer, int newLayer)
		{
			Assert.IsTrue(prevLayer >= 0 && prevLayer <= 32, "Invalid layer bit");
			this.Unregister(volume, prevLayer);
			this.Register(volume, newLayer);
		}

		// Token: 0x06004067 RID: 16487 RVA: 0x0017BF1C File Offset: 0x0017A11C
		private void Register(PostProcessVolume volume, int layer)
		{
			this.m_Volumes.Add(volume);
			foreach (KeyValuePair<int, List<PostProcessVolume>> keyValuePair in this.m_SortedVolumes)
			{
				if ((keyValuePair.Key & 1 << layer) != 0)
				{
					keyValuePair.Value.Add(volume);
				}
			}
			this.SetLayerDirty(layer);
		}

		// Token: 0x06004068 RID: 16488 RVA: 0x0017BF98 File Offset: 0x0017A198
		internal void Register(PostProcessVolume volume)
		{
			int layer = volume.gameObject.layer;
			this.Register(volume, layer);
		}

		// Token: 0x06004069 RID: 16489 RVA: 0x0017BFBC File Offset: 0x0017A1BC
		private void Unregister(PostProcessVolume volume, int layer)
		{
			this.m_Volumes.Remove(volume);
			foreach (KeyValuePair<int, List<PostProcessVolume>> keyValuePair in this.m_SortedVolumes)
			{
				if ((keyValuePair.Key & 1 << layer) != 0)
				{
					keyValuePair.Value.Remove(volume);
				}
			}
		}

		// Token: 0x0600406A RID: 16490 RVA: 0x0017C034 File Offset: 0x0017A234
		internal void Unregister(PostProcessVolume volume)
		{
			int layer = volume.gameObject.layer;
			this.Unregister(volume, layer);
		}

		// Token: 0x0600406B RID: 16491 RVA: 0x0017C058 File Offset: 0x0017A258
		private void ReplaceData(PostProcessLayer postProcessLayer)
		{
			foreach (PostProcessEffectSettings postProcessEffectSettings in this.m_BaseSettings)
			{
				PostProcessEffectSettings settings = postProcessLayer.GetBundle(postProcessEffectSettings.GetType()).settings;
				int count = postProcessEffectSettings.parameters.Count;
				for (int i = 0; i < count; i++)
				{
					settings.parameters[i].SetValue(postProcessEffectSettings.parameters[i]);
				}
			}
		}

		// Token: 0x0600406C RID: 16492 RVA: 0x0017C0F4 File Offset: 0x0017A2F4
		internal void UpdateSettings(PostProcessLayer postProcessLayer, Camera camera)
		{
			this.ReplaceData(postProcessLayer);
			int value = postProcessLayer.volumeLayer.value;
			Transform volumeTrigger = postProcessLayer.volumeTrigger;
			bool flag = volumeTrigger == null;
			Vector3 vector = flag ? Vector3.zero : volumeTrigger.position;
			foreach (PostProcessVolume postProcessVolume in this.GrabVolumes(value))
			{
				if (postProcessVolume.enabled && !(postProcessVolume.profileRef == null) && postProcessVolume.weight > 0f)
				{
					List<PostProcessEffectSettings> settings = postProcessVolume.profileRef.settings;
					if (postProcessVolume.isGlobal)
					{
						postProcessLayer.OverrideSettings(settings, Mathf.Clamp01(postProcessVolume.weight));
					}
					else if (!flag)
					{
						OBB obb = new OBB(postProcessVolume.transform, postProcessVolume.bounds);
						float sqrMagnitude = ((obb.ClosestPoint(vector) - vector) / 2f).sqrMagnitude;
						float num = postProcessVolume.blendDistance * postProcessVolume.blendDistance;
						if (sqrMagnitude <= num)
						{
							float num2 = 1f;
							if (num > 0f)
							{
								num2 = 1f - sqrMagnitude / num;
							}
							postProcessLayer.OverrideSettings(settings, num2 * Mathf.Clamp01(postProcessVolume.weight));
						}
					}
				}
			}
		}

		// Token: 0x0600406D RID: 16493 RVA: 0x0017C278 File Offset: 0x0017A478
		private List<PostProcessVolume> GrabVolumes(LayerMask mask)
		{
			List<PostProcessVolume> list;
			if (!this.m_SortedVolumes.TryGetValue(mask, out list))
			{
				list = new List<PostProcessVolume>();
				foreach (PostProcessVolume postProcessVolume in this.m_Volumes)
				{
					if ((mask & 1 << postProcessVolume.gameObject.layer) != 0)
					{
						list.Add(postProcessVolume);
						this.m_SortNeeded[mask] = true;
					}
				}
				this.m_SortedVolumes.Add(mask, list);
			}
			bool flag;
			if (this.m_SortNeeded.TryGetValue(mask, out flag) && flag)
			{
				this.m_SortNeeded[mask] = false;
				PostProcessManager.SortByPriority(list);
			}
			return list;
		}

		// Token: 0x0600406E RID: 16494 RVA: 0x0017C354 File Offset: 0x0017A554
		private static void SortByPriority(List<PostProcessVolume> volumes)
		{
			Assert.IsNotNull<List<PostProcessVolume>>(volumes, "Trying to sort volumes of non-initialized layer");
			for (int i = 1; i < volumes.Count; i++)
			{
				PostProcessVolume postProcessVolume = volumes[i];
				int num = i - 1;
				while (num >= 0 && volumes[num].priority > postProcessVolume.priority)
				{
					volumes[num + 1] = volumes[num];
					num--;
				}
				volumes[num + 1] = postProcessVolume;
			}
		}

		// Token: 0x0600406F RID: 16495 RVA: 0x0000441C File Offset: 0x0000261C
		private static bool IsVolumeRenderedByCamera(PostProcessVolume volume, Camera camera)
		{
			return true;
		}
	}
}
