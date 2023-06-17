using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200090A RID: 2314
public class ParticleSystemContainer : MonoBehaviour, IPrefabPreProcess
{
	// Token: 0x040032FF RID: 13055
	public bool precached;

	// Token: 0x04003300 RID: 13056
	[HideInInspector]
	public ParticleSystemContainer.ParticleSystemGroup[] particleGroups;

	// Token: 0x06003801 RID: 14337 RVA: 0x000063A5 File Offset: 0x000045A5
	public void Play()
	{
	}

	// Token: 0x06003802 RID: 14338 RVA: 0x000063A5 File Offset: 0x000045A5
	public void Pause()
	{
	}

	// Token: 0x06003803 RID: 14339 RVA: 0x000063A5 File Offset: 0x000045A5
	public void Stop()
	{
	}

	// Token: 0x06003804 RID: 14340 RVA: 0x000063A5 File Offset: 0x000045A5
	public void Clear()
	{
	}

	// Token: 0x06003805 RID: 14341 RVA: 0x0014ECF0 File Offset: 0x0014CEF0
	public void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if (this.precached && clientside)
		{
			List<ParticleSystemContainer.ParticleSystemGroup> list = new List<ParticleSystemContainer.ParticleSystemGroup>();
			foreach (ParticleSystem particleSystem in base.GetComponentsInChildren<ParticleSystem>())
			{
				LODComponentParticleSystem[] components = particleSystem.GetComponents<LODComponentParticleSystem>();
				ParticleSystemContainer.ParticleSystemGroup item = new ParticleSystemContainer.ParticleSystemGroup
				{
					system = particleSystem,
					lodComponents = components
				};
				list.Add(item);
			}
			this.particleGroups = list.ToArray();
		}
	}

	// Token: 0x02000EB4 RID: 3764
	[Serializable]
	public struct ParticleSystemGroup
	{
		// Token: 0x04004CB0 RID: 19632
		public ParticleSystem system;

		// Token: 0x04004CB1 RID: 19633
		public LODComponentParticleSystem[] lodComponents;
	}
}
