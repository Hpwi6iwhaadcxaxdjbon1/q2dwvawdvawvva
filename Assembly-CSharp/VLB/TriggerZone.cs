using System;
using UnityEngine;

namespace VLB
{
	// Token: 0x020009B8 RID: 2488
	[DisallowMultipleComponent]
	[RequireComponent(typeof(VolumetricLightBeam))]
	[HelpURL("http://saladgamer.com/vlb-doc/comp-triggerzone/")]
	public class TriggerZone : MonoBehaviour
	{
		// Token: 0x040035EB RID: 13803
		public bool setIsTrigger = true;

		// Token: 0x040035EC RID: 13804
		public float rangeMultiplier = 1f;

		// Token: 0x040035ED RID: 13805
		private const int kMeshColliderNumSides = 8;

		// Token: 0x040035EE RID: 13806
		private Mesh m_Mesh;

		// Token: 0x06003B4E RID: 15182 RVA: 0x0015FA64 File Offset: 0x0015DC64
		private void Update()
		{
			VolumetricLightBeam component = base.GetComponent<VolumetricLightBeam>();
			if (component)
			{
				MeshCollider orAddComponent = base.gameObject.GetOrAddComponent<MeshCollider>();
				Debug.Assert(orAddComponent);
				float lengthZ = component.fadeEnd * this.rangeMultiplier;
				float radiusEnd = Mathf.LerpUnclamped(component.coneRadiusStart, component.coneRadiusEnd, this.rangeMultiplier);
				this.m_Mesh = MeshGenerator.GenerateConeZ_Radius(lengthZ, component.coneRadiusStart, radiusEnd, 8, 0, false);
				this.m_Mesh.hideFlags = Consts.ProceduralObjectsHideFlags;
				orAddComponent.sharedMesh = this.m_Mesh;
				if (this.setIsTrigger)
				{
					orAddComponent.convex = true;
					orAddComponent.isTrigger = true;
				}
				UnityEngine.Object.Destroy(this);
			}
		}
	}
}
