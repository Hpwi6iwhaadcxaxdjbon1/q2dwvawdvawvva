using System;
using UnityEngine;

// Token: 0x0200070F RID: 1807
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class AtmosphereVolumeRenderer : MonoBehaviour
{
	// Token: 0x04002963 RID: 10595
	public FogMode Mode = FogMode.ExponentialSquared;

	// Token: 0x04002964 RID: 10596
	public bool DistanceFog = true;

	// Token: 0x04002965 RID: 10597
	public bool HeightFog = true;

	// Token: 0x04002966 RID: 10598
	public AtmosphereVolume Volume;

	// Token: 0x1700042D RID: 1069
	// (get) Token: 0x060032F3 RID: 13043 RVA: 0x00139B39 File Offset: 0x00137D39
	private static bool isSupported
	{
		get
		{
			return Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.OSXPlayer;
		}
	}
}
