using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace VLB
{
	// Token: 0x020009BC RID: 2492
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[SelectionBase]
	[HelpURL("http://saladgamer.com/vlb-doc/comp-lightbeam/")]
	public class VolumetricLightBeam : MonoBehaviour
	{
		// Token: 0x04003601 RID: 13825
		public bool colorFromLight = true;

		// Token: 0x04003602 RID: 13826
		public ColorMode colorMode;

		// Token: 0x04003603 RID: 13827
		[ColorUsage(true, true)]
		[FormerlySerializedAs("colorValue")]
		public Color color = Consts.FlatColor;

		// Token: 0x04003604 RID: 13828
		public Gradient colorGradient;

		// Token: 0x04003605 RID: 13829
		[Range(0f, 1f)]
		public float alphaInside = 1f;

		// Token: 0x04003606 RID: 13830
		[FormerlySerializedAs("alpha")]
		[Range(0f, 1f)]
		public float alphaOutside = 1f;

		// Token: 0x04003607 RID: 13831
		public BlendingMode blendingMode;

		// Token: 0x04003608 RID: 13832
		[FormerlySerializedAs("angleFromLight")]
		public bool spotAngleFromLight = true;

		// Token: 0x04003609 RID: 13833
		[Range(0.1f, 179.9f)]
		public float spotAngle = 35f;

		// Token: 0x0400360A RID: 13834
		[FormerlySerializedAs("radiusStart")]
		public float coneRadiusStart = 0.1f;

		// Token: 0x0400360B RID: 13835
		public MeshType geomMeshType;

		// Token: 0x0400360C RID: 13836
		[FormerlySerializedAs("geomSides")]
		public int geomCustomSides = 18;

		// Token: 0x0400360D RID: 13837
		public int geomCustomSegments = 5;

		// Token: 0x0400360E RID: 13838
		public bool geomCap;

		// Token: 0x0400360F RID: 13839
		public bool fadeEndFromLight = true;

		// Token: 0x04003610 RID: 13840
		public AttenuationEquation attenuationEquation = AttenuationEquation.Quadratic;

		// Token: 0x04003611 RID: 13841
		[Range(0f, 1f)]
		public float attenuationCustomBlending = 0.5f;

		// Token: 0x04003612 RID: 13842
		public float fadeStart;

		// Token: 0x04003613 RID: 13843
		public float fadeEnd = 3f;

		// Token: 0x04003614 RID: 13844
		public float depthBlendDistance = 2f;

		// Token: 0x04003615 RID: 13845
		public float cameraClippingDistance = 0.5f;

		// Token: 0x04003616 RID: 13846
		[Range(0f, 1f)]
		public float glareFrontal = 0.5f;

		// Token: 0x04003617 RID: 13847
		[Range(0f, 1f)]
		public float glareBehind = 0.5f;

		// Token: 0x04003618 RID: 13848
		[Obsolete("Use 'glareFrontal' instead")]
		public float boostDistanceInside = 0.5f;

		// Token: 0x04003619 RID: 13849
		[Obsolete("This property has been merged with 'fresnelPow'")]
		public float fresnelPowInside = 6f;

		// Token: 0x0400361A RID: 13850
		[FormerlySerializedAs("fresnelPowOutside")]
		public float fresnelPow = 8f;

		// Token: 0x0400361B RID: 13851
		public bool noiseEnabled;

		// Token: 0x0400361C RID: 13852
		[Range(0f, 1f)]
		public float noiseIntensity = 0.5f;

		// Token: 0x0400361D RID: 13853
		public bool noiseScaleUseGlobal = true;

		// Token: 0x0400361E RID: 13854
		[Range(0.01f, 2f)]
		public float noiseScaleLocal = 0.5f;

		// Token: 0x0400361F RID: 13855
		public bool noiseVelocityUseGlobal = true;

		// Token: 0x04003620 RID: 13856
		public Vector3 noiseVelocityLocal = Consts.NoiseVelocityDefault;

		// Token: 0x04003621 RID: 13857
		private Plane m_PlaneWS;

		// Token: 0x04003622 RID: 13858
		[SerializeField]
		private int pluginVersion = -1;

		// Token: 0x04003623 RID: 13859
		[FormerlySerializedAs("trackChangesDuringPlaytime")]
		[SerializeField]
		private bool _TrackChangesDuringPlaytime;

		// Token: 0x04003624 RID: 13860
		[SerializeField]
		private int _SortingLayerID;

		// Token: 0x04003625 RID: 13861
		[SerializeField]
		private int _SortingOrder;

		// Token: 0x04003626 RID: 13862
		private BeamGeometry m_BeamGeom;

		// Token: 0x04003627 RID: 13863
		private Coroutine m_CoPlaytimeUpdate;

		// Token: 0x04003628 RID: 13864
		private Light _CachedLight;

		// Token: 0x170004BE RID: 1214
		// (get) Token: 0x06003B79 RID: 15225 RVA: 0x00160527 File Offset: 0x0015E727
		public float coneAngle
		{
			get
			{
				return Mathf.Atan2(this.coneRadiusEnd - this.coneRadiusStart, this.fadeEnd) * 57.29578f * 2f;
			}
		}

		// Token: 0x170004BF RID: 1215
		// (get) Token: 0x06003B7A RID: 15226 RVA: 0x0016054D File Offset: 0x0015E74D
		public float coneRadiusEnd
		{
			get
			{
				return this.fadeEnd * Mathf.Tan(this.spotAngle * 0.017453292f * 0.5f);
			}
		}

		// Token: 0x170004C0 RID: 1216
		// (get) Token: 0x06003B7B RID: 15227 RVA: 0x00160570 File Offset: 0x0015E770
		public float coneVolume
		{
			get
			{
				float num = this.coneRadiusStart;
				float coneRadiusEnd = this.coneRadiusEnd;
				return 1.0471976f * (num * num + num * coneRadiusEnd + coneRadiusEnd * coneRadiusEnd) * this.fadeEnd;
			}
		}

		// Token: 0x170004C1 RID: 1217
		// (get) Token: 0x06003B7C RID: 15228 RVA: 0x001605A4 File Offset: 0x0015E7A4
		public float coneApexOffsetZ
		{
			get
			{
				float num = this.coneRadiusStart / this.coneRadiusEnd;
				if (num != 1f)
				{
					return this.fadeEnd * num / (1f - num);
				}
				return float.MaxValue;
			}
		}

		// Token: 0x170004C2 RID: 1218
		// (get) Token: 0x06003B7D RID: 15229 RVA: 0x001605DD File Offset: 0x0015E7DD
		// (set) Token: 0x06003B7E RID: 15230 RVA: 0x001605F9 File Offset: 0x0015E7F9
		public int geomSides
		{
			get
			{
				if (this.geomMeshType != MeshType.Custom)
				{
					return Config.Instance.sharedMeshSides;
				}
				return this.geomCustomSides;
			}
			set
			{
				this.geomCustomSides = value;
				Debug.LogWarning("The setter VLB.VolumetricLightBeam.geomSides is OBSOLETE and has been renamed to geomCustomSides.");
			}
		}

		// Token: 0x170004C3 RID: 1219
		// (get) Token: 0x06003B7F RID: 15231 RVA: 0x0016060C File Offset: 0x0015E80C
		// (set) Token: 0x06003B80 RID: 15232 RVA: 0x00160628 File Offset: 0x0015E828
		public int geomSegments
		{
			get
			{
				if (this.geomMeshType != MeshType.Custom)
				{
					return Config.Instance.sharedMeshSegments;
				}
				return this.geomCustomSegments;
			}
			set
			{
				this.geomCustomSegments = value;
				Debug.LogWarning("The setter VLB.VolumetricLightBeam.geomSegments is OBSOLETE and has been renamed to geomCustomSegments.");
			}
		}

		// Token: 0x170004C4 RID: 1220
		// (get) Token: 0x06003B81 RID: 15233 RVA: 0x0016063B File Offset: 0x0015E83B
		public float attenuationLerpLinearQuad
		{
			get
			{
				if (this.attenuationEquation == AttenuationEquation.Linear)
				{
					return 0f;
				}
				if (this.attenuationEquation == AttenuationEquation.Quadratic)
				{
					return 1f;
				}
				return this.attenuationCustomBlending;
			}
		}

		// Token: 0x170004C5 RID: 1221
		// (get) Token: 0x06003B82 RID: 15234 RVA: 0x00160660 File Offset: 0x0015E860
		// (set) Token: 0x06003B83 RID: 15235 RVA: 0x00160668 File Offset: 0x0015E868
		public int sortingLayerID
		{
			get
			{
				return this._SortingLayerID;
			}
			set
			{
				this._SortingLayerID = value;
				if (this.m_BeamGeom)
				{
					this.m_BeamGeom.sortingLayerID = value;
				}
			}
		}

		// Token: 0x170004C6 RID: 1222
		// (get) Token: 0x06003B84 RID: 15236 RVA: 0x0016068A File Offset: 0x0015E88A
		// (set) Token: 0x06003B85 RID: 15237 RVA: 0x00160697 File Offset: 0x0015E897
		public string sortingLayerName
		{
			get
			{
				return SortingLayer.IDToName(this.sortingLayerID);
			}
			set
			{
				this.sortingLayerID = SortingLayer.NameToID(value);
			}
		}

		// Token: 0x170004C7 RID: 1223
		// (get) Token: 0x06003B86 RID: 15238 RVA: 0x001606A5 File Offset: 0x0015E8A5
		// (set) Token: 0x06003B87 RID: 15239 RVA: 0x001606AD File Offset: 0x0015E8AD
		public int sortingOrder
		{
			get
			{
				return this._SortingOrder;
			}
			set
			{
				this._SortingOrder = value;
				if (this.m_BeamGeom)
				{
					this.m_BeamGeom.sortingOrder = value;
				}
			}
		}

		// Token: 0x170004C8 RID: 1224
		// (get) Token: 0x06003B88 RID: 15240 RVA: 0x001606CF File Offset: 0x0015E8CF
		// (set) Token: 0x06003B89 RID: 15241 RVA: 0x001606D7 File Offset: 0x0015E8D7
		public bool trackChangesDuringPlaytime
		{
			get
			{
				return this._TrackChangesDuringPlaytime;
			}
			set
			{
				this._TrackChangesDuringPlaytime = value;
				this.StartPlaytimeUpdateIfNeeded();
			}
		}

		// Token: 0x170004C9 RID: 1225
		// (get) Token: 0x06003B8A RID: 15242 RVA: 0x001606E6 File Offset: 0x0015E8E6
		public bool isCurrentlyTrackingChanges
		{
			get
			{
				return this.m_CoPlaytimeUpdate != null;
			}
		}

		// Token: 0x170004CA RID: 1226
		// (get) Token: 0x06003B8B RID: 15243 RVA: 0x001606F1 File Offset: 0x0015E8F1
		public bool hasGeometry
		{
			get
			{
				return this.m_BeamGeom != null;
			}
		}

		// Token: 0x170004CB RID: 1227
		// (get) Token: 0x06003B8C RID: 15244 RVA: 0x001606FF File Offset: 0x0015E8FF
		public Bounds bounds
		{
			get
			{
				if (!(this.m_BeamGeom != null))
				{
					return new Bounds(Vector3.zero, Vector3.zero);
				}
				return this.m_BeamGeom.meshRenderer.bounds;
			}
		}

		// Token: 0x06003B8D RID: 15245 RVA: 0x0016072F File Offset: 0x0015E92F
		public void SetClippingPlane(Plane planeWS)
		{
			if (this.m_BeamGeom)
			{
				this.m_BeamGeom.SetClippingPlane(planeWS);
			}
			this.m_PlaneWS = planeWS;
		}

		// Token: 0x06003B8E RID: 15246 RVA: 0x00160751 File Offset: 0x0015E951
		public void SetClippingPlaneOff()
		{
			if (this.m_BeamGeom)
			{
				this.m_BeamGeom.SetClippingPlaneOff();
			}
			this.m_PlaneWS = default(Plane);
		}

		// Token: 0x06003B8F RID: 15247 RVA: 0x00160778 File Offset: 0x0015E978
		public bool IsColliderHiddenByDynamicOccluder(Collider collider)
		{
			Debug.Assert(collider, "You should pass a valid Collider to VLB.VolumetricLightBeam.IsColliderHiddenByDynamicOccluder");
			return this.m_PlaneWS.IsValid() && !GeometryUtility.TestPlanesAABB(new Plane[]
			{
				this.m_PlaneWS
			}, collider.bounds);
		}

		// Token: 0x170004CC RID: 1228
		// (get) Token: 0x06003B90 RID: 15248 RVA: 0x001607C5 File Offset: 0x0015E9C5
		public int blendingModeAsInt
		{
			get
			{
				return Mathf.Clamp((int)this.blendingMode, 0, Enum.GetValues(typeof(BlendingMode)).Length);
			}
		}

		// Token: 0x170004CD RID: 1229
		// (get) Token: 0x06003B91 RID: 15249 RVA: 0x001607E7 File Offset: 0x0015E9E7
		public MeshRenderer Renderer
		{
			get
			{
				if (!(this.m_BeamGeom != null))
				{
					return null;
				}
				return this.m_BeamGeom.meshRenderer;
			}
		}

		// Token: 0x170004CE RID: 1230
		// (get) Token: 0x06003B92 RID: 15250 RVA: 0x00160804 File Offset: 0x0015EA04
		public string meshStats
		{
			get
			{
				Mesh mesh = this.m_BeamGeom ? this.m_BeamGeom.coneMesh : null;
				if (mesh)
				{
					return string.Format("Cone angle: {0:0.0} degrees\nMesh: {1} vertices, {2} triangles", this.coneAngle, mesh.vertexCount, mesh.triangles.Length / 3);
				}
				return "no mesh available";
			}
		}

		// Token: 0x170004CF RID: 1231
		// (get) Token: 0x06003B93 RID: 15251 RVA: 0x0016086A File Offset: 0x0015EA6A
		public int meshVerticesCount
		{
			get
			{
				if (!this.m_BeamGeom || !this.m_BeamGeom.coneMesh)
				{
					return 0;
				}
				return this.m_BeamGeom.coneMesh.vertexCount;
			}
		}

		// Token: 0x170004D0 RID: 1232
		// (get) Token: 0x06003B94 RID: 15252 RVA: 0x0016089D File Offset: 0x0015EA9D
		public int meshTrianglesCount
		{
			get
			{
				if (!this.m_BeamGeom || !this.m_BeamGeom.coneMesh)
				{
					return 0;
				}
				return this.m_BeamGeom.coneMesh.triangles.Length / 3;
			}
		}

		// Token: 0x170004D1 RID: 1233
		// (get) Token: 0x06003B95 RID: 15253 RVA: 0x001608D4 File Offset: 0x0015EAD4
		private Light lightSpotAttached
		{
			get
			{
				if (this._CachedLight == null)
				{
					this._CachedLight = base.GetComponent<Light>();
				}
				if (this._CachedLight && this._CachedLight.type == LightType.Spot)
				{
					return this._CachedLight;
				}
				return null;
			}
		}

		// Token: 0x06003B96 RID: 15254 RVA: 0x00160912 File Offset: 0x0015EB12
		public float GetInsideBeamFactor(Vector3 posWS)
		{
			return this.GetInsideBeamFactorFromObjectSpacePos(base.transform.InverseTransformPoint(posWS));
		}

		// Token: 0x06003B97 RID: 15255 RVA: 0x00160928 File Offset: 0x0015EB28
		public float GetInsideBeamFactorFromObjectSpacePos(Vector3 posOS)
		{
			if (posOS.z < 0f)
			{
				return -1f;
			}
			Vector2 normalized = new Vector2(posOS.xy().magnitude, posOS.z + this.coneApexOffsetZ).normalized;
			return Mathf.Clamp((Mathf.Abs(Mathf.Sin(this.coneAngle * 0.017453292f / 2f)) - Mathf.Abs(normalized.x)) / 0.1f, -1f, 1f);
		}

		// Token: 0x06003B98 RID: 15256 RVA: 0x001609AE File Offset: 0x0015EBAE
		[Obsolete("Use 'GenerateGeometry()' instead")]
		public void Generate()
		{
			this.GenerateGeometry();
		}

		// Token: 0x06003B99 RID: 15257 RVA: 0x001609B8 File Offset: 0x0015EBB8
		public virtual void GenerateGeometry()
		{
			this.HandleBackwardCompatibility(this.pluginVersion, 1510);
			this.pluginVersion = 1510;
			this.ValidateProperties();
			if (this.m_BeamGeom == null)
			{
				Shader beamShader = Config.Instance.beamShader;
				if (!beamShader)
				{
					Debug.LogError("Invalid BeamShader set in VLB Config");
					return;
				}
				this.m_BeamGeom = Utils.NewWithComponent<BeamGeometry>("Beam Geometry");
				this.m_BeamGeom.Initialize(this, beamShader);
			}
			this.m_BeamGeom.RegenerateMesh();
			this.m_BeamGeom.visible = base.enabled;
		}

		// Token: 0x06003B9A RID: 15258 RVA: 0x00160A4C File Offset: 0x0015EC4C
		public virtual void UpdateAfterManualPropertyChange()
		{
			this.ValidateProperties();
			if (this.m_BeamGeom)
			{
				this.m_BeamGeom.UpdateMaterialAndBounds();
			}
		}

		// Token: 0x06003B9B RID: 15259 RVA: 0x001609AE File Offset: 0x0015EBAE
		private void Start()
		{
			this.GenerateGeometry();
		}

		// Token: 0x06003B9C RID: 15260 RVA: 0x00160A6C File Offset: 0x0015EC6C
		private void OnEnable()
		{
			if (this.m_BeamGeom)
			{
				this.m_BeamGeom.visible = true;
			}
			this.StartPlaytimeUpdateIfNeeded();
		}

		// Token: 0x06003B9D RID: 15261 RVA: 0x00160A8D File Offset: 0x0015EC8D
		private void OnDisable()
		{
			if (this.m_BeamGeom)
			{
				this.m_BeamGeom.visible = false;
			}
			this.m_CoPlaytimeUpdate = null;
		}

		// Token: 0x06003B9E RID: 15262 RVA: 0x000063A5 File Offset: 0x000045A5
		private void StartPlaytimeUpdateIfNeeded()
		{
		}

		// Token: 0x06003B9F RID: 15263 RVA: 0x00160AAF File Offset: 0x0015ECAF
		private IEnumerator CoPlaytimeUpdate()
		{
			while (this.trackChangesDuringPlaytime && base.enabled)
			{
				this.UpdateAfterManualPropertyChange();
				yield return null;
			}
			this.m_CoPlaytimeUpdate = null;
			yield break;
		}

		// Token: 0x06003BA0 RID: 15264 RVA: 0x00160ABE File Offset: 0x0015ECBE
		private void OnDestroy()
		{
			this.DestroyBeam();
		}

		// Token: 0x06003BA1 RID: 15265 RVA: 0x00160AC6 File Offset: 0x0015ECC6
		private void DestroyBeam()
		{
			if (this.m_BeamGeom)
			{
				UnityEngine.Object.DestroyImmediate(this.m_BeamGeom.gameObject);
			}
			this.m_BeamGeom = null;
		}

		// Token: 0x06003BA2 RID: 15266 RVA: 0x00160AEC File Offset: 0x0015ECEC
		private void AssignPropertiesFromSpotLight(Light lightSpot)
		{
			if (lightSpot && lightSpot.type == LightType.Spot)
			{
				if (this.fadeEndFromLight)
				{
					this.fadeEnd = lightSpot.range;
				}
				if (this.spotAngleFromLight)
				{
					this.spotAngle = lightSpot.spotAngle;
				}
				if (this.colorFromLight)
				{
					this.colorMode = ColorMode.Flat;
					this.color = lightSpot.color;
				}
			}
		}

		// Token: 0x06003BA3 RID: 15267 RVA: 0x00160B4C File Offset: 0x0015ED4C
		private void ClampProperties()
		{
			this.alphaInside = Mathf.Clamp01(this.alphaInside);
			this.alphaOutside = Mathf.Clamp01(this.alphaOutside);
			this.attenuationCustomBlending = Mathf.Clamp01(this.attenuationCustomBlending);
			this.fadeEnd = Mathf.Max(0.01f, this.fadeEnd);
			this.fadeStart = Mathf.Clamp(this.fadeStart, 0f, this.fadeEnd - 0.01f);
			this.spotAngle = Mathf.Clamp(this.spotAngle, 0.1f, 179.9f);
			this.coneRadiusStart = Mathf.Max(this.coneRadiusStart, 0f);
			this.depthBlendDistance = Mathf.Max(this.depthBlendDistance, 0f);
			this.cameraClippingDistance = Mathf.Max(this.cameraClippingDistance, 0f);
			this.geomCustomSides = Mathf.Clamp(this.geomCustomSides, 3, 256);
			this.geomCustomSegments = Mathf.Clamp(this.geomCustomSegments, 0, 64);
			this.fresnelPow = Mathf.Max(0f, this.fresnelPow);
			this.glareBehind = Mathf.Clamp01(this.glareBehind);
			this.glareFrontal = Mathf.Clamp01(this.glareFrontal);
			this.noiseIntensity = Mathf.Clamp(this.noiseIntensity, 0f, 1f);
		}

		// Token: 0x06003BA4 RID: 15268 RVA: 0x00160C9F File Offset: 0x0015EE9F
		private void ValidateProperties()
		{
			this.AssignPropertiesFromSpotLight(this.lightSpotAttached);
			this.ClampProperties();
		}

		// Token: 0x06003BA5 RID: 15269 RVA: 0x00160CB3 File Offset: 0x0015EEB3
		private void HandleBackwardCompatibility(int serializedVersion, int newVersion)
		{
			if (serializedVersion == -1)
			{
				return;
			}
			if (serializedVersion == newVersion)
			{
				return;
			}
			if (serializedVersion < 1301)
			{
				this.attenuationEquation = AttenuationEquation.Linear;
			}
			if (serializedVersion < 1501)
			{
				this.geomMeshType = MeshType.Custom;
				this.geomCustomSegments = 5;
			}
			Utils.MarkCurrentSceneDirty();
		}
	}
}
