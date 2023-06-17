using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A43 RID: 2627
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class PostProcessAttribute : Attribute
	{
		// Token: 0x04003853 RID: 14419
		public readonly Type renderer;

		// Token: 0x04003854 RID: 14420
		public readonly PostProcessEvent eventType;

		// Token: 0x04003855 RID: 14421
		public readonly string menuItem;

		// Token: 0x04003856 RID: 14422
		public readonly bool allowInSceneView;

		// Token: 0x04003857 RID: 14423
		internal readonly bool builtinEffect;

		// Token: 0x06003F43 RID: 16195 RVA: 0x00173690 File Offset: 0x00171890
		public PostProcessAttribute(Type renderer, PostProcessEvent eventType, string menuItem, bool allowInSceneView = true)
		{
			this.renderer = renderer;
			this.eventType = eventType;
			this.menuItem = menuItem;
			this.allowInSceneView = allowInSceneView;
			this.builtinEffect = false;
		}

		// Token: 0x06003F44 RID: 16196 RVA: 0x001736BC File Offset: 0x001718BC
		internal PostProcessAttribute(Type renderer, string menuItem, bool allowInSceneView = true)
		{
			this.renderer = renderer;
			this.menuItem = menuItem;
			this.allowInSceneView = allowInSceneView;
			this.builtinEffect = true;
		}
	}
}
