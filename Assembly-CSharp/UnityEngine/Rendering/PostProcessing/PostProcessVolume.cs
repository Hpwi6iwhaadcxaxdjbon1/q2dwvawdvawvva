using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A94 RID: 2708
	[ExecuteAlways]
	[AddComponentMenu("Rendering/Post-process Volume", 1001)]
	public sealed class PostProcessVolume : MonoBehaviour
	{
		// Token: 0x0400398C RID: 14732
		public PostProcessProfile sharedProfile;

		// Token: 0x0400398D RID: 14733
		[Tooltip("Check this box to mark this volume as global. This volume's Profile will be applied to the whole Scene.")]
		public bool isGlobal;

		// Token: 0x0400398E RID: 14734
		public Bounds bounds;

		// Token: 0x0400398F RID: 14735
		[Min(0f)]
		[Tooltip("The distance (from the attached Collider) to start blending from. A value of 0 means there will be no blending and the Volume overrides will be applied immediatly upon entry to the attached Collider.")]
		public float blendDistance;

		// Token: 0x04003990 RID: 14736
		[Range(0f, 1f)]
		[Tooltip("The total weight of this Volume in the Scene. A value of 0 signifies that it will have no effect, 1 signifies full effect.")]
		public float weight = 1f;

		// Token: 0x04003991 RID: 14737
		[Tooltip("The volume priority in the stack. A higher value means higher priority. Negative values are supported.")]
		public float priority;

		// Token: 0x04003992 RID: 14738
		private int m_PreviousLayer;

		// Token: 0x04003993 RID: 14739
		private float m_PreviousPriority;

		// Token: 0x04003994 RID: 14740
		private PostProcessProfile m_InternalProfile;

		// Token: 0x17000580 RID: 1408
		// (get) Token: 0x0600407C RID: 16508 RVA: 0x0017C694 File Offset: 0x0017A894
		// (set) Token: 0x0600407D RID: 16509 RVA: 0x0017C728 File Offset: 0x0017A928
		public PostProcessProfile profile
		{
			get
			{
				if (this.m_InternalProfile == null)
				{
					this.m_InternalProfile = ScriptableObject.CreateInstance<PostProcessProfile>();
					if (this.sharedProfile != null)
					{
						foreach (PostProcessEffectSettings original in this.sharedProfile.settings)
						{
							PostProcessEffectSettings item = Object.Instantiate<PostProcessEffectSettings>(original);
							this.m_InternalProfile.settings.Add(item);
						}
					}
				}
				return this.m_InternalProfile;
			}
			set
			{
				this.m_InternalProfile = value;
			}
		}

		// Token: 0x17000581 RID: 1409
		// (get) Token: 0x0600407E RID: 16510 RVA: 0x0017C731 File Offset: 0x0017A931
		internal PostProcessProfile profileRef
		{
			get
			{
				if (!(this.m_InternalProfile == null))
				{
					return this.m_InternalProfile;
				}
				return this.sharedProfile;
			}
		}

		// Token: 0x0600407F RID: 16511 RVA: 0x0017C74E File Offset: 0x0017A94E
		public bool HasInstantiatedProfile()
		{
			return this.m_InternalProfile != null;
		}

		// Token: 0x06004080 RID: 16512 RVA: 0x0017C75C File Offset: 0x0017A95C
		private void OnEnable()
		{
			PostProcessManager.instance.Register(this);
			this.m_PreviousLayer = base.gameObject.layer;
		}

		// Token: 0x06004081 RID: 16513 RVA: 0x0017C77A File Offset: 0x0017A97A
		private void OnDisable()
		{
			PostProcessManager.instance.Unregister(this);
		}

		// Token: 0x06004082 RID: 16514 RVA: 0x0017C788 File Offset: 0x0017A988
		private void Update()
		{
			int layer = base.gameObject.layer;
			if (layer != this.m_PreviousLayer)
			{
				PostProcessManager.instance.UpdateVolumeLayer(this, this.m_PreviousLayer, layer);
				this.m_PreviousLayer = layer;
			}
			if (this.priority != this.m_PreviousPriority)
			{
				PostProcessManager.instance.SetLayerDirty(layer);
				this.m_PreviousPriority = this.priority;
			}
		}

		// Token: 0x06004083 RID: 16515 RVA: 0x0017C7E8 File Offset: 0x0017A9E8
		private void OnDrawGizmos()
		{
			if (this.isGlobal)
			{
				return;
			}
			Vector3 lossyScale = base.transform.lossyScale;
			Vector3 a = new Vector3(1f / lossyScale.x, 1f / lossyScale.y, 1f / lossyScale.z);
			Gizmos.matrix = Matrix4x4.TRS(base.transform.position, base.transform.rotation, lossyScale);
			Gizmos.DrawCube(this.bounds.center, this.bounds.size);
			Gizmos.DrawWireCube(this.bounds.center, this.bounds.size + a * this.blendDistance * 4f);
			Gizmos.matrix = Matrix4x4.identity;
		}
	}
}
