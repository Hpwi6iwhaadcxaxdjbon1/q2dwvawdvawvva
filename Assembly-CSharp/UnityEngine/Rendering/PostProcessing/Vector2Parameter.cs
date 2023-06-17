using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A81 RID: 2689
	[Serializable]
	public sealed class Vector2Parameter : ParameterOverride<Vector2>
	{
		// Token: 0x06004012 RID: 16402 RVA: 0x0017AA38 File Offset: 0x00178C38
		public override void Interp(Vector2 from, Vector2 to, float t)
		{
			this.value.x = from.x + (to.x - from.x) * t;
			this.value.y = from.y + (to.y - from.y) * t;
		}

		// Token: 0x06004013 RID: 16403 RVA: 0x0017AA87 File Offset: 0x00178C87
		public static implicit operator Vector3(Vector2Parameter prop)
		{
			return prop.value;
		}

		// Token: 0x06004014 RID: 16404 RVA: 0x0017AA94 File Offset: 0x00178C94
		public static implicit operator Vector4(Vector2Parameter prop)
		{
			return prop.value;
		}
	}
}
