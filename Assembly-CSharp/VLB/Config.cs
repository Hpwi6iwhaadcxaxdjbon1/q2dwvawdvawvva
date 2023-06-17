using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace VLB
{
	// Token: 0x020009AB RID: 2475
	[HelpURL("http://saladgamer.com/vlb-doc/config/")]
	public class Config : ScriptableObject
	{
		// Token: 0x04003579 RID: 13689
		public int geometryLayerID = 1;

		// Token: 0x0400357A RID: 13690
		public string geometryTag = "Untagged";

		// Token: 0x0400357B RID: 13691
		public int geometryRenderQueue = 3000;

		// Token: 0x0400357C RID: 13692
		public bool forceSinglePass;

		// Token: 0x0400357D RID: 13693
		[SerializeField]
		[HighlightNull]
		private Shader beamShader1Pass;

		// Token: 0x0400357E RID: 13694
		[FormerlySerializedAs("BeamShader")]
		[FormerlySerializedAs("beamShader")]
		[SerializeField]
		[HighlightNull]
		private Shader beamShader2Pass;

		// Token: 0x0400357F RID: 13695
		public int sharedMeshSides = 24;

		// Token: 0x04003580 RID: 13696
		public int sharedMeshSegments = 5;

		// Token: 0x04003581 RID: 13697
		[Range(0.01f, 2f)]
		public float globalNoiseScale = 0.5f;

		// Token: 0x04003582 RID: 13698
		public Vector3 globalNoiseVelocity = Consts.NoiseVelocityDefault;

		// Token: 0x04003583 RID: 13699
		[HighlightNull]
		public TextAsset noise3DData;

		// Token: 0x04003584 RID: 13700
		public int noise3DSize = 64;

		// Token: 0x04003585 RID: 13701
		[HighlightNull]
		public ParticleSystem dustParticlesPrefab;

		// Token: 0x04003586 RID: 13702
		private static Config m_Instance;

		// Token: 0x170004B0 RID: 1200
		// (get) Token: 0x06003B24 RID: 15140 RVA: 0x0015EB3D File Offset: 0x0015CD3D
		public Shader beamShader
		{
			get
			{
				if (!this.forceSinglePass)
				{
					return this.beamShader2Pass;
				}
				return this.beamShader1Pass;
			}
		}

		// Token: 0x170004B1 RID: 1201
		// (get) Token: 0x06003B25 RID: 15141 RVA: 0x0015EB54 File Offset: 0x0015CD54
		public Vector4 globalNoiseParam
		{
			get
			{
				return new Vector4(this.globalNoiseVelocity.x, this.globalNoiseVelocity.y, this.globalNoiseVelocity.z, this.globalNoiseScale);
			}
		}

		// Token: 0x06003B26 RID: 15142 RVA: 0x0015EB84 File Offset: 0x0015CD84
		public void Reset()
		{
			this.geometryLayerID = 1;
			this.geometryTag = "Untagged";
			this.geometryRenderQueue = 3000;
			this.beamShader1Pass = Shader.Find("Hidden/VolumetricLightBeam1Pass");
			this.beamShader2Pass = Shader.Find("Hidden/VolumetricLightBeam2Pass");
			this.sharedMeshSides = 24;
			this.sharedMeshSegments = 5;
			this.globalNoiseScale = 0.5f;
			this.globalNoiseVelocity = Consts.NoiseVelocityDefault;
			this.noise3DData = (Resources.Load("Noise3D_64x64x64") as TextAsset);
			this.noise3DSize = 64;
			this.dustParticlesPrefab = (Resources.Load("DustParticles", typeof(ParticleSystem)) as ParticleSystem);
		}

		// Token: 0x06003B27 RID: 15143 RVA: 0x0015EC30 File Offset: 0x0015CE30
		public ParticleSystem NewVolumetricDustParticles()
		{
			if (!this.dustParticlesPrefab)
			{
				if (Application.isPlaying)
				{
					Debug.LogError("Failed to instantiate VolumetricDustParticles prefab.");
				}
				return null;
			}
			ParticleSystem particleSystem = UnityEngine.Object.Instantiate<ParticleSystem>(this.dustParticlesPrefab);
			particleSystem.useAutoRandomSeed = false;
			particleSystem.name = "Dust Particles";
			particleSystem.gameObject.hideFlags = Consts.ProceduralObjectsHideFlags;
			particleSystem.gameObject.SetActive(true);
			return particleSystem;
		}

		// Token: 0x170004B2 RID: 1202
		// (get) Token: 0x06003B28 RID: 15144 RVA: 0x0015EC98 File Offset: 0x0015CE98
		public static Config Instance
		{
			get
			{
				if (Config.m_Instance == null)
				{
					Config[] array = Resources.LoadAll<Config>("Config");
					Debug.Assert(array.Length != 0, string.Format("Can't find any resource of type '{0}'. Make sure you have a ScriptableObject of this type in a 'Resources' folder.", typeof(Config)));
					Config.m_Instance = array[0];
				}
				return Config.m_Instance;
			}
		}
	}
}
