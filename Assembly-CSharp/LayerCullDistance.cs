using System;
using UnityEngine;

// Token: 0x020008FD RID: 2301
public class LayerCullDistance : MonoBehaviour
{
	// Token: 0x040032D2 RID: 13010
	public string Layer = "Default";

	// Token: 0x040032D3 RID: 13011
	public float Distance = 1000f;

	// Token: 0x060037DD RID: 14301 RVA: 0x0014E714 File Offset: 0x0014C914
	protected void OnEnable()
	{
		Camera component = base.GetComponent<Camera>();
		float[] layerCullDistances = component.layerCullDistances;
		layerCullDistances[LayerMask.NameToLayer(this.Layer)] = this.Distance;
		component.layerCullDistances = layerCullDistances;
	}
}
