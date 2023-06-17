using System;
using Facepunch;

namespace UnityEngine
{
	// Token: 0x02000A20 RID: 2592
	public static class ComponentEx
	{
		// Token: 0x06003D8F RID: 15759 RVA: 0x001698F8 File Offset: 0x00167AF8
		public static T Instantiate<T>(this T component) where T : Component
		{
			return Facepunch.Instantiate.GameObject(component.gameObject, null).GetComponent<T>();
		}

		// Token: 0x06003D90 RID: 15760 RVA: 0x00169910 File Offset: 0x00167B10
		public static bool HasComponent<T>(this Component component) where T : Component
		{
			return component.GetComponent<T>() != null;
		}
	}
}
