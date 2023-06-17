using System;
using Rust;
using Rust.Registry;
using UnityEngine;

// Token: 0x020003B8 RID: 952
public class BaseEntityChild : MonoBehaviour
{
	// Token: 0x0600211D RID: 8477 RVA: 0x000D9428 File Offset: 0x000D7628
	public static void Setup(GameObject obj, BaseEntity parent)
	{
		using (TimeWarning.New("Registry.Entity.Register", 0))
		{
			Entity.Register(obj, parent);
		}
	}

	// Token: 0x0600211E RID: 8478 RVA: 0x000D9464 File Offset: 0x000D7664
	public void OnDestroy()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		using (TimeWarning.New("Registry.Entity.Unregister", 0))
		{
			Entity.Unregister(base.gameObject);
		}
	}
}
