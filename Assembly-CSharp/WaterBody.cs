using System;
using UnityEngine;

// Token: 0x020006F8 RID: 1784
[ExecuteInEditMode]
public class WaterBody : MonoBehaviour
{
	// Token: 0x040028ED RID: 10477
	public WaterBodyType Type = WaterBodyType.Lake;

	// Token: 0x040028EE RID: 10478
	public Renderer Renderer;

	// Token: 0x040028EF RID: 10479
	public Collider[] Triggers;

	// Token: 0x040028F0 RID: 10480
	public bool IsOcean;

	// Token: 0x040028F2 RID: 10482
	public WaterBody.FishingTag FishingType;

	// Token: 0x17000413 RID: 1043
	// (get) Token: 0x06003247 RID: 12871 RVA: 0x00135E48 File Offset: 0x00134048
	// (set) Token: 0x06003246 RID: 12870 RVA: 0x00135E3F File Offset: 0x0013403F
	public Transform Transform { get; private set; }

	// Token: 0x06003248 RID: 12872 RVA: 0x00135E50 File Offset: 0x00134050
	private void Awake()
	{
		this.Transform = base.transform;
	}

	// Token: 0x06003249 RID: 12873 RVA: 0x00135E5E File Offset: 0x0013405E
	private void OnEnable()
	{
		WaterSystem.RegisterBody(this);
	}

	// Token: 0x0600324A RID: 12874 RVA: 0x00135E66 File Offset: 0x00134066
	private void OnDisable()
	{
		WaterSystem.UnregisterBody(this);
	}

	// Token: 0x0600324B RID: 12875 RVA: 0x00135E70 File Offset: 0x00134070
	public void OnOceanLevelChanged(float newLevel)
	{
		if (!this.IsOcean)
		{
			return;
		}
		foreach (Collider collider in this.Triggers)
		{
			Vector3 position = collider.transform.position;
			position.y = newLevel;
			collider.transform.position = position;
		}
	}

	// Token: 0x02000E2B RID: 3627
	[Flags]
	public enum FishingTag
	{
		// Token: 0x04004A5E RID: 19038
		MoonPool = 1,
		// Token: 0x04004A5F RID: 19039
		River = 2,
		// Token: 0x04004A60 RID: 19040
		Ocean = 4,
		// Token: 0x04004A61 RID: 19041
		Swamp = 8
	}
}
