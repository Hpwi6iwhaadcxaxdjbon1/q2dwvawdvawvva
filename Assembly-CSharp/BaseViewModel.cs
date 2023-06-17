using System;
using UnityEngine;

// Token: 0x02000969 RID: 2409
public class BaseViewModel : MonoBehaviour
{
	// Token: 0x040033CB RID: 13259
	[Header("BaseViewModel")]
	public LazyAimProperties lazyaimRegular;

	// Token: 0x040033CC RID: 13260
	public LazyAimProperties lazyaimIronsights;

	// Token: 0x040033CD RID: 13261
	public Transform pivot;

	// Token: 0x040033CE RID: 13262
	public bool useViewModelCamera = true;

	// Token: 0x040033CF RID: 13263
	public bool wantsHeldItemFlags;

	// Token: 0x040033D0 RID: 13264
	public GameObject[] hideSightMeshes;

	// Token: 0x040033D1 RID: 13265
	public bool isGestureViewModel;

	// Token: 0x040033D2 RID: 13266
	public Transform MuzzlePoint;

	// Token: 0x040033D3 RID: 13267
	[Header("Skin")]
	public SubsurfaceProfile subsurfaceProfile;
}
