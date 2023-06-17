using System;
using UnityEngine;

namespace Smaa
{
	// Token: 0x020009C3 RID: 2499
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	[AddComponentMenu("Image Effects/Subpixel Morphological Antialiasing")]
	public class SMAA : MonoBehaviour
	{
		// Token: 0x04003643 RID: 13891
		public DebugPass DebugPass;

		// Token: 0x04003644 RID: 13892
		public QualityPreset Quality = QualityPreset.High;

		// Token: 0x04003645 RID: 13893
		public EdgeDetectionMethod DetectionMethod = EdgeDetectionMethod.Luma;

		// Token: 0x04003646 RID: 13894
		public bool UsePredication;

		// Token: 0x04003647 RID: 13895
		public Preset CustomPreset;

		// Token: 0x04003648 RID: 13896
		public PredicationPreset CustomPredicationPreset;

		// Token: 0x04003649 RID: 13897
		public Shader Shader;

		// Token: 0x0400364A RID: 13898
		public Texture2D AreaTex;

		// Token: 0x0400364B RID: 13899
		public Texture2D SearchTex;

		// Token: 0x0400364C RID: 13900
		protected Camera m_Camera;

		// Token: 0x0400364D RID: 13901
		protected Preset m_LowPreset;

		// Token: 0x0400364E RID: 13902
		protected Preset m_MediumPreset;

		// Token: 0x0400364F RID: 13903
		protected Preset m_HighPreset;

		// Token: 0x04003650 RID: 13904
		protected Preset m_UltraPreset;

		// Token: 0x04003651 RID: 13905
		protected Material m_Material;

		// Token: 0x170004D2 RID: 1234
		// (get) Token: 0x06003BAA RID: 15274 RVA: 0x00160E8D File Offset: 0x0015F08D
		public Material Material
		{
			get
			{
				if (this.m_Material == null)
				{
					this.m_Material = new Material(this.Shader);
					this.m_Material.hideFlags = HideFlags.HideAndDontSave;
				}
				return this.m_Material;
			}
		}
	}
}
