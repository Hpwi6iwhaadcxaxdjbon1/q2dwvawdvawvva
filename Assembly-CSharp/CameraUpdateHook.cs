using System;
using UnityEngine;

// Token: 0x020008F4 RID: 2292
public class CameraUpdateHook : MonoBehaviour
{
	// Token: 0x040032BD RID: 12989
	public static Action PreCull;

	// Token: 0x040032BE RID: 12990
	public static Action PreRender;

	// Token: 0x040032BF RID: 12991
	public static Action PostRender;

	// Token: 0x040032C0 RID: 12992
	public static Action RustCamera_PreRender;

	// Token: 0x060037CC RID: 14284 RVA: 0x0014E1D4 File Offset: 0x0014C3D4
	private void Awake()
	{
		Camera.onPreRender = (Camera.CameraCallback)Delegate.Combine(Camera.onPreRender, new Camera.CameraCallback(delegate(Camera args)
		{
			Action preRender = CameraUpdateHook.PreRender;
			if (preRender == null)
			{
				return;
			}
			preRender();
		}));
		Camera.onPostRender = (Camera.CameraCallback)Delegate.Combine(Camera.onPostRender, new Camera.CameraCallback(delegate(Camera args)
		{
			Action postRender = CameraUpdateHook.PostRender;
			if (postRender == null)
			{
				return;
			}
			postRender();
		}));
		Camera.onPreCull = (Camera.CameraCallback)Delegate.Combine(Camera.onPreCull, new Camera.CameraCallback(delegate(Camera args)
		{
			Action preCull = CameraUpdateHook.PreCull;
			if (preCull == null)
			{
				return;
			}
			preCull();
		}));
	}
}
