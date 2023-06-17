using System;
using UnityEngine;

namespace VLB
{
	// Token: 0x020009BB RID: 2491
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(VolumetricLightBeam))]
	[HelpURL("http://saladgamer.com/vlb-doc/comp-dustparticles/")]
	public class VolumetricDustParticles : MonoBehaviour
	{
		// Token: 0x040035F2 RID: 13810
		[Range(0f, 1f)]
		public float alpha = 0.5f;

		// Token: 0x040035F3 RID: 13811
		[Range(0.0001f, 0.1f)]
		public float size = 0.01f;

		// Token: 0x040035F4 RID: 13812
		public VolumetricDustParticles.Direction direction = VolumetricDustParticles.Direction.Random;

		// Token: 0x040035F5 RID: 13813
		public float speed = 0.03f;

		// Token: 0x040035F6 RID: 13814
		public float density = 5f;

		// Token: 0x040035F7 RID: 13815
		[Range(0f, 1f)]
		public float spawnMaxDistance = 0.7f;

		// Token: 0x040035F8 RID: 13816
		public bool cullingEnabled = true;

		// Token: 0x040035F9 RID: 13817
		public float cullingMaxDistance = 10f;

		// Token: 0x040035FB RID: 13819
		public static bool isFeatureSupported = true;

		// Token: 0x040035FC RID: 13820
		private ParticleSystem m_Particles;

		// Token: 0x040035FD RID: 13821
		private ParticleSystemRenderer m_Renderer;

		// Token: 0x040035FE RID: 13822
		private static bool ms_NoMainCameraLogged = false;

		// Token: 0x040035FF RID: 13823
		private static Camera ms_MainCamera = null;

		// Token: 0x04003600 RID: 13824
		private VolumetricLightBeam m_Master;

		// Token: 0x170004B9 RID: 1209
		// (get) Token: 0x06003B68 RID: 15208 RVA: 0x0015FF22 File Offset: 0x0015E122
		// (set) Token: 0x06003B69 RID: 15209 RVA: 0x0015FF2A File Offset: 0x0015E12A
		public bool isCulled { get; private set; }

		// Token: 0x170004BA RID: 1210
		// (get) Token: 0x06003B6A RID: 15210 RVA: 0x0015FF33 File Offset: 0x0015E133
		public bool particlesAreInstantiated
		{
			get
			{
				return this.m_Particles;
			}
		}

		// Token: 0x170004BB RID: 1211
		// (get) Token: 0x06003B6B RID: 15211 RVA: 0x0015FF40 File Offset: 0x0015E140
		public int particlesCurrentCount
		{
			get
			{
				if (!this.m_Particles)
				{
					return 0;
				}
				return this.m_Particles.particleCount;
			}
		}

		// Token: 0x170004BC RID: 1212
		// (get) Token: 0x06003B6C RID: 15212 RVA: 0x0015FF5C File Offset: 0x0015E15C
		public int particlesMaxCount
		{
			get
			{
				if (!this.m_Particles)
				{
					return 0;
				}
				return this.m_Particles.main.maxParticles;
			}
		}

		// Token: 0x170004BD RID: 1213
		// (get) Token: 0x06003B6D RID: 15213 RVA: 0x0015FF8C File Offset: 0x0015E18C
		public Camera mainCamera
		{
			get
			{
				if (!VolumetricDustParticles.ms_MainCamera)
				{
					VolumetricDustParticles.ms_MainCamera = Camera.main;
					if (!VolumetricDustParticles.ms_MainCamera && !VolumetricDustParticles.ms_NoMainCameraLogged)
					{
						Debug.LogErrorFormat(base.gameObject, "In order to use 'VolumetricDustParticles' culling, you must have a MainCamera defined in your scene.", Array.Empty<object>());
						VolumetricDustParticles.ms_NoMainCameraLogged = true;
					}
				}
				return VolumetricDustParticles.ms_MainCamera;
			}
		}

		// Token: 0x06003B6E RID: 15214 RVA: 0x0015FFE2 File Offset: 0x0015E1E2
		private void Start()
		{
			this.isCulled = false;
			this.m_Master = base.GetComponent<VolumetricLightBeam>();
			Debug.Assert(this.m_Master);
			this.InstantiateParticleSystem();
			this.SetActiveAndPlay();
		}

		// Token: 0x06003B6F RID: 15215 RVA: 0x00160014 File Offset: 0x0015E214
		private void InstantiateParticleSystem()
		{
			ParticleSystem[] componentsInChildren = base.GetComponentsInChildren<ParticleSystem>(true);
			for (int i = componentsInChildren.Length - 1; i >= 0; i--)
			{
				UnityEngine.Object.DestroyImmediate(componentsInChildren[i].gameObject);
			}
			this.m_Particles = Config.Instance.NewVolumetricDustParticles();
			if (this.m_Particles)
			{
				this.m_Particles.transform.SetParent(base.transform, false);
				this.m_Renderer = this.m_Particles.GetComponent<ParticleSystemRenderer>();
			}
		}

		// Token: 0x06003B70 RID: 15216 RVA: 0x0016008B File Offset: 0x0015E28B
		private void OnEnable()
		{
			this.SetActiveAndPlay();
		}

		// Token: 0x06003B71 RID: 15217 RVA: 0x00160093 File Offset: 0x0015E293
		private void SetActiveAndPlay()
		{
			if (this.m_Particles)
			{
				this.m_Particles.gameObject.SetActive(true);
				this.SetParticleProperties();
				this.m_Particles.Play(true);
			}
		}

		// Token: 0x06003B72 RID: 15218 RVA: 0x001600C5 File Offset: 0x0015E2C5
		private void OnDisable()
		{
			if (this.m_Particles)
			{
				this.m_Particles.gameObject.SetActive(false);
			}
		}

		// Token: 0x06003B73 RID: 15219 RVA: 0x001600E5 File Offset: 0x0015E2E5
		private void OnDestroy()
		{
			if (this.m_Particles)
			{
				UnityEngine.Object.DestroyImmediate(this.m_Particles.gameObject);
			}
			this.m_Particles = null;
		}

		// Token: 0x06003B74 RID: 15220 RVA: 0x0016010B File Offset: 0x0015E30B
		private void Update()
		{
			if (Application.isPlaying)
			{
				this.UpdateCulling();
			}
			this.SetParticleProperties();
		}

		// Token: 0x06003B75 RID: 15221 RVA: 0x00160120 File Offset: 0x0015E320
		private void SetParticleProperties()
		{
			if (this.m_Particles && this.m_Particles.gameObject.activeSelf)
			{
				float t = Mathf.Clamp01(1f - this.m_Master.fresnelPow / 10f);
				float num = this.m_Master.fadeEnd * this.spawnMaxDistance;
				float num2 = num * this.density;
				int maxParticles = (int)(num2 * 4f);
				ParticleSystem.MainModule main = this.m_Particles.main;
				ParticleSystem.MinMaxCurve startLifetime = main.startLifetime;
				startLifetime.mode = ParticleSystemCurveMode.TwoConstants;
				startLifetime.constantMin = 4f;
				startLifetime.constantMax = 6f;
				main.startLifetime = startLifetime;
				ParticleSystem.MinMaxCurve startSize = main.startSize;
				startSize.mode = ParticleSystemCurveMode.TwoConstants;
				startSize.constantMin = this.size * 0.9f;
				startSize.constantMax = this.size * 1.1f;
				main.startSize = startSize;
				ParticleSystem.MinMaxGradient startColor = main.startColor;
				if (this.m_Master.colorMode == ColorMode.Flat)
				{
					startColor.mode = ParticleSystemGradientMode.Color;
					Color color = this.m_Master.color;
					color.a *= this.alpha;
					startColor.color = color;
				}
				else
				{
					startColor.mode = ParticleSystemGradientMode.Gradient;
					Gradient colorGradient = this.m_Master.colorGradient;
					GradientColorKey[] colorKeys = colorGradient.colorKeys;
					GradientAlphaKey[] alphaKeys = colorGradient.alphaKeys;
					for (int i = 0; i < alphaKeys.Length; i++)
					{
						GradientAlphaKey[] array = alphaKeys;
						int num3 = i;
						array[num3].alpha = array[num3].alpha * this.alpha;
					}
					Gradient gradient = new Gradient();
					gradient.SetKeys(colorKeys, alphaKeys);
					startColor.gradient = gradient;
				}
				main.startColor = startColor;
				ParticleSystem.MinMaxCurve startSpeed = main.startSpeed;
				startSpeed.constant = this.speed;
				main.startSpeed = startSpeed;
				main.maxParticles = maxParticles;
				ParticleSystem.ShapeModule shape = this.m_Particles.shape;
				shape.shapeType = ParticleSystemShapeType.ConeVolume;
				shape.radius = this.m_Master.coneRadiusStart * Mathf.Lerp(0.3f, 1f, t);
				shape.angle = this.m_Master.coneAngle * 0.5f * Mathf.Lerp(0.7f, 1f, t);
				shape.length = num;
				shape.arc = 360f;
				shape.randomDirectionAmount = ((this.direction == VolumetricDustParticles.Direction.Random) ? 1f : 0f);
				ParticleSystem.EmissionModule emission = this.m_Particles.emission;
				ParticleSystem.MinMaxCurve rateOverTime = emission.rateOverTime;
				rateOverTime.constant = num2;
				emission.rateOverTime = rateOverTime;
				if (this.m_Renderer)
				{
					this.m_Renderer.sortingLayerID = this.m_Master.sortingLayerID;
					this.m_Renderer.sortingOrder = this.m_Master.sortingOrder;
				}
			}
		}

		// Token: 0x06003B76 RID: 15222 RVA: 0x001603E4 File Offset: 0x0015E5E4
		private void UpdateCulling()
		{
			if (this.m_Particles)
			{
				bool flag = true;
				if (this.cullingEnabled && this.m_Master.hasGeometry)
				{
					if (this.mainCamera)
					{
						float num = this.cullingMaxDistance * this.cullingMaxDistance;
						flag = (this.m_Master.bounds.SqrDistance(this.mainCamera.transform.position) <= num);
					}
					else
					{
						this.cullingEnabled = false;
					}
				}
				if (this.m_Particles.gameObject.activeSelf != flag)
				{
					this.m_Particles.gameObject.SetActive(flag);
					this.isCulled = !flag;
				}
				if (flag && !this.m_Particles.isPlaying)
				{
					this.m_Particles.Play();
				}
			}
		}

		// Token: 0x02000EE2 RID: 3810
		public enum Direction
		{
			// Token: 0x04004D77 RID: 19831
			Beam,
			// Token: 0x04004D78 RID: 19832
			Random
		}
	}
}
