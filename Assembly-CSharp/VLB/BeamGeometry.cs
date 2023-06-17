using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace VLB
{
	// Token: 0x020009AA RID: 2474
	[AddComponentMenu("")]
	[ExecuteInEditMode]
	[HelpURL("http://saladgamer.com/vlb-doc/comp-lightbeam/")]
	public class BeamGeometry : MonoBehaviour
	{
		// Token: 0x04003572 RID: 13682
		private VolumetricLightBeam m_Master;

		// Token: 0x04003573 RID: 13683
		private Matrix4x4 m_ColorGradientMatrix;

		// Token: 0x04003574 RID: 13684
		private MeshType m_CurrentMeshType;

		// Token: 0x170004A9 RID: 1193
		// (get) Token: 0x06003B07 RID: 15111 RVA: 0x0015E1A9 File Offset: 0x0015C3A9
		// (set) Token: 0x06003B08 RID: 15112 RVA: 0x0015E1B1 File Offset: 0x0015C3B1
		public MeshRenderer meshRenderer { get; private set; }

		// Token: 0x170004AA RID: 1194
		// (get) Token: 0x06003B09 RID: 15113 RVA: 0x0015E1BA File Offset: 0x0015C3BA
		// (set) Token: 0x06003B0A RID: 15114 RVA: 0x0015E1C2 File Offset: 0x0015C3C2
		public MeshFilter meshFilter { get; private set; }

		// Token: 0x170004AB RID: 1195
		// (get) Token: 0x06003B0B RID: 15115 RVA: 0x0015E1CB File Offset: 0x0015C3CB
		// (set) Token: 0x06003B0C RID: 15116 RVA: 0x0015E1D3 File Offset: 0x0015C3D3
		public Material material { get; private set; }

		// Token: 0x170004AC RID: 1196
		// (get) Token: 0x06003B0D RID: 15117 RVA: 0x0015E1DC File Offset: 0x0015C3DC
		// (set) Token: 0x06003B0E RID: 15118 RVA: 0x0015E1E4 File Offset: 0x0015C3E4
		public Mesh coneMesh { get; private set; }

		// Token: 0x170004AD RID: 1197
		// (get) Token: 0x06003B0F RID: 15119 RVA: 0x0015E1ED File Offset: 0x0015C3ED
		// (set) Token: 0x06003B10 RID: 15120 RVA: 0x0015E1FA File Offset: 0x0015C3FA
		public bool visible
		{
			get
			{
				return this.meshRenderer.enabled;
			}
			set
			{
				this.meshRenderer.enabled = value;
			}
		}

		// Token: 0x170004AE RID: 1198
		// (get) Token: 0x06003B11 RID: 15121 RVA: 0x0015E208 File Offset: 0x0015C408
		// (set) Token: 0x06003B12 RID: 15122 RVA: 0x0015E215 File Offset: 0x0015C415
		public int sortingLayerID
		{
			get
			{
				return this.meshRenderer.sortingLayerID;
			}
			set
			{
				this.meshRenderer.sortingLayerID = value;
			}
		}

		// Token: 0x170004AF RID: 1199
		// (get) Token: 0x06003B13 RID: 15123 RVA: 0x0015E223 File Offset: 0x0015C423
		// (set) Token: 0x06003B14 RID: 15124 RVA: 0x0015E230 File Offset: 0x0015C430
		public int sortingOrder
		{
			get
			{
				return this.meshRenderer.sortingOrder;
			}
			set
			{
				this.meshRenderer.sortingOrder = value;
			}
		}

		// Token: 0x06003B15 RID: 15125 RVA: 0x000063A5 File Offset: 0x000045A5
		private void Start()
		{
		}

		// Token: 0x06003B16 RID: 15126 RVA: 0x0015E23E File Offset: 0x0015C43E
		private void OnDestroy()
		{
			if (this.material)
			{
				UnityEngine.Object.DestroyImmediate(this.material);
				this.material = null;
			}
		}

		// Token: 0x06003B17 RID: 15127 RVA: 0x0015E25F File Offset: 0x0015C45F
		private static bool IsUsingCustomRenderPipeline()
		{
			return RenderPipelineManager.currentPipeline != null || GraphicsSettings.renderPipelineAsset != null;
		}

		// Token: 0x06003B18 RID: 15128 RVA: 0x0015E275 File Offset: 0x0015C475
		private void OnEnable()
		{
			if (BeamGeometry.IsUsingCustomRenderPipeline())
			{
				RenderPipelineManager.beginCameraRendering += this.OnBeginCameraRendering;
			}
		}

		// Token: 0x06003B19 RID: 15129 RVA: 0x0015E28F File Offset: 0x0015C48F
		private void OnDisable()
		{
			if (BeamGeometry.IsUsingCustomRenderPipeline())
			{
				RenderPipelineManager.beginCameraRendering -= this.OnBeginCameraRendering;
			}
		}

		// Token: 0x06003B1A RID: 15130 RVA: 0x0015E2AC File Offset: 0x0015C4AC
		public void Initialize(VolumetricLightBeam master, Shader shader)
		{
			HideFlags proceduralObjectsHideFlags = Consts.ProceduralObjectsHideFlags;
			this.m_Master = master;
			base.transform.SetParent(master.transform, false);
			this.material = new Material(shader);
			this.material.hideFlags = proceduralObjectsHideFlags;
			this.meshRenderer = base.gameObject.GetOrAddComponent<MeshRenderer>();
			this.meshRenderer.hideFlags = proceduralObjectsHideFlags;
			this.meshRenderer.material = this.material;
			this.meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
			this.meshRenderer.receiveShadows = false;
			this.meshRenderer.lightProbeUsage = LightProbeUsage.Off;
			if (SortingLayer.IsValid(this.m_Master.sortingLayerID))
			{
				this.sortingLayerID = this.m_Master.sortingLayerID;
			}
			else
			{
				Debug.LogError(string.Format("Beam '{0}' has an invalid sortingLayerID ({1}). Please fix it by setting a valid layer.", Utils.GetPath(this.m_Master.transform), this.m_Master.sortingLayerID));
			}
			this.sortingOrder = this.m_Master.sortingOrder;
			this.meshFilter = base.gameObject.GetOrAddComponent<MeshFilter>();
			this.meshFilter.hideFlags = proceduralObjectsHideFlags;
			base.gameObject.hideFlags = proceduralObjectsHideFlags;
		}

		// Token: 0x06003B1B RID: 15131 RVA: 0x0015E3D0 File Offset: 0x0015C5D0
		public void RegenerateMesh()
		{
			Debug.Assert(this.m_Master);
			base.gameObject.layer = Config.Instance.geometryLayerID;
			base.gameObject.tag = Config.Instance.geometryTag;
			if (this.coneMesh && this.m_CurrentMeshType == MeshType.Custom)
			{
				UnityEngine.Object.DestroyImmediate(this.coneMesh);
			}
			this.m_CurrentMeshType = this.m_Master.geomMeshType;
			MeshType geomMeshType = this.m_Master.geomMeshType;
			if (geomMeshType != MeshType.Shared)
			{
				if (geomMeshType == MeshType.Custom)
				{
					this.coneMesh = MeshGenerator.GenerateConeZ_Radius(1f, 1f, 1f, this.m_Master.geomCustomSides, this.m_Master.geomCustomSegments, this.m_Master.geomCap);
					this.coneMesh.hideFlags = Consts.ProceduralObjectsHideFlags;
					this.meshFilter.mesh = this.coneMesh;
				}
				else
				{
					Debug.LogError("Unsupported MeshType");
				}
			}
			else
			{
				this.coneMesh = GlobalMesh.mesh;
				this.meshFilter.sharedMesh = this.coneMesh;
			}
			this.UpdateMaterialAndBounds();
		}

		// Token: 0x06003B1C RID: 15132 RVA: 0x0015E4E8 File Offset: 0x0015C6E8
		private void ComputeLocalMatrix()
		{
			float num = Mathf.Max(this.m_Master.coneRadiusStart, this.m_Master.coneRadiusEnd);
			base.transform.localScale = new Vector3(num, num, this.m_Master.fadeEnd);
		}

		// Token: 0x06003B1D RID: 15133 RVA: 0x0015E530 File Offset: 0x0015C730
		public void UpdateMaterialAndBounds()
		{
			Debug.Assert(this.m_Master);
			this.material.renderQueue = Config.Instance.geometryRenderQueue;
			float f = this.m_Master.coneAngle * 0.017453292f / 2f;
			this.material.SetVector("_ConeSlopeCosSin", new Vector2(Mathf.Cos(f), Mathf.Sin(f)));
			Vector2 v = new Vector2(Mathf.Max(this.m_Master.coneRadiusStart, 0.0001f), Mathf.Max(this.m_Master.coneRadiusEnd, 0.0001f));
			this.material.SetVector("_ConeRadius", v);
			float value = Mathf.Sign(this.m_Master.coneApexOffsetZ) * Mathf.Max(Mathf.Abs(this.m_Master.coneApexOffsetZ), 0.0001f);
			this.material.SetFloat("_ConeApexOffsetZ", value);
			if (this.m_Master.colorMode == ColorMode.Gradient)
			{
				Utils.FloatPackingPrecision floatPackingPrecision = Utils.GetFloatPackingPrecision();
				this.material.EnableKeyword((floatPackingPrecision == Utils.FloatPackingPrecision.High) ? "VLB_COLOR_GRADIENT_MATRIX_HIGH" : "VLB_COLOR_GRADIENT_MATRIX_LOW");
				this.m_ColorGradientMatrix = this.m_Master.colorGradient.SampleInMatrix((int)floatPackingPrecision);
			}
			else
			{
				this.material.DisableKeyword("VLB_COLOR_GRADIENT_MATRIX_HIGH");
				this.material.DisableKeyword("VLB_COLOR_GRADIENT_MATRIX_LOW");
				this.material.SetColor("_ColorFlat", this.m_Master.color);
			}
			if (Consts.BlendingMode_AlphaAsBlack[this.m_Master.blendingModeAsInt])
			{
				this.material.EnableKeyword("ALPHA_AS_BLACK");
			}
			else
			{
				this.material.DisableKeyword("ALPHA_AS_BLACK");
			}
			this.material.SetInt("_BlendSrcFactor", (int)Consts.BlendingMode_SrcFactor[this.m_Master.blendingModeAsInt]);
			this.material.SetInt("_BlendDstFactor", (int)Consts.BlendingMode_DstFactor[this.m_Master.blendingModeAsInt]);
			this.material.SetFloat("_AlphaInside", this.m_Master.alphaInside);
			this.material.SetFloat("_AlphaOutside", this.m_Master.alphaOutside);
			this.material.SetFloat("_AttenuationLerpLinearQuad", this.m_Master.attenuationLerpLinearQuad);
			this.material.SetFloat("_DistanceFadeStart", this.m_Master.fadeStart);
			this.material.SetFloat("_DistanceFadeEnd", this.m_Master.fadeEnd);
			this.material.SetFloat("_DistanceCamClipping", this.m_Master.cameraClippingDistance);
			this.material.SetFloat("_FresnelPow", Mathf.Max(0.001f, this.m_Master.fresnelPow));
			this.material.SetFloat("_GlareBehind", this.m_Master.glareBehind);
			this.material.SetFloat("_GlareFrontal", this.m_Master.glareFrontal);
			this.material.SetFloat("_DrawCap", (float)(this.m_Master.geomCap ? 1 : 0));
			if (this.m_Master.depthBlendDistance > 0f)
			{
				this.material.EnableKeyword("VLB_DEPTH_BLEND");
				this.material.SetFloat("_DepthBlendDistance", this.m_Master.depthBlendDistance);
			}
			else
			{
				this.material.DisableKeyword("VLB_DEPTH_BLEND");
			}
			if (this.m_Master.noiseEnabled && this.m_Master.noiseIntensity > 0f && Noise3D.isSupported)
			{
				Noise3D.LoadIfNeeded();
				this.material.EnableKeyword("VLB_NOISE_3D");
				this.material.SetVector("_NoiseLocal", new Vector4(this.m_Master.noiseVelocityLocal.x, this.m_Master.noiseVelocityLocal.y, this.m_Master.noiseVelocityLocal.z, this.m_Master.noiseScaleLocal));
				this.material.SetVector("_NoiseParam", new Vector3(this.m_Master.noiseIntensity, this.m_Master.noiseVelocityUseGlobal ? 1f : 0f, this.m_Master.noiseScaleUseGlobal ? 1f : 0f));
			}
			else
			{
				this.material.DisableKeyword("VLB_NOISE_3D");
			}
			this.ComputeLocalMatrix();
		}

		// Token: 0x06003B1E RID: 15134 RVA: 0x0015E990 File Offset: 0x0015CB90
		public void SetClippingPlane(Plane planeWS)
		{
			Vector3 normal = planeWS.normal;
			this.material.EnableKeyword("VLB_CLIPPING_PLANE");
			this.material.SetVector("_ClippingPlaneWS", new Vector4(normal.x, normal.y, normal.z, planeWS.distance));
		}

		// Token: 0x06003B1F RID: 15135 RVA: 0x0015E9E3 File Offset: 0x0015CBE3
		public void SetClippingPlaneOff()
		{
			this.material.DisableKeyword("VLB_CLIPPING_PLANE");
		}

		// Token: 0x06003B20 RID: 15136 RVA: 0x0015E9F5 File Offset: 0x0015CBF5
		private void OnBeginCameraRendering(ScriptableRenderContext context, Camera cam)
		{
			this.UpdateCameraRelatedProperties(cam);
		}

		// Token: 0x06003B21 RID: 15137 RVA: 0x0015EA00 File Offset: 0x0015CC00
		private void OnWillRenderObject()
		{
			if (!BeamGeometry.IsUsingCustomRenderPipeline())
			{
				Camera current = Camera.current;
				if (current != null)
				{
					this.UpdateCameraRelatedProperties(current);
				}
			}
		}

		// Token: 0x06003B22 RID: 15138 RVA: 0x0015EA2C File Offset: 0x0015CC2C
		private void UpdateCameraRelatedProperties(Camera cam)
		{
			if (cam && this.m_Master)
			{
				if (this.material)
				{
					Vector3 vector = this.m_Master.transform.InverseTransformPoint(cam.transform.position);
					this.material.SetVector("_CameraPosObjectSpace", vector);
					Vector3 normalized = base.transform.InverseTransformDirection(cam.transform.forward).normalized;
					float w = cam.orthographic ? -1f : this.m_Master.GetInsideBeamFactorFromObjectSpacePos(vector);
					this.material.SetVector("_CameraParams", new Vector4(normalized.x, normalized.y, normalized.z, w));
					if (this.m_Master.colorMode == ColorMode.Gradient)
					{
						this.material.SetMatrix("_ColorGradientMatrix", this.m_ColorGradientMatrix);
					}
				}
				if (this.m_Master.depthBlendDistance > 0f)
				{
					cam.depthTextureMode |= DepthTextureMode.Depth;
				}
			}
		}
	}
}
