using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A83 RID: 2691
	[Serializable]
	public sealed class Vector4Parameter : ParameterOverride<Vector4>
	{
		// Token: 0x0600401A RID: 16410 RVA: 0x0017AB40 File Offset: 0x00178D40
		public override void Interp(Vector4 from, Vector4 to, float t)
		{
			this.value.x = from.x + (to.x - from.x) * t;
			this.value.y = from.y + (to.y - from.y) * t;
			this.value.z = from.z + (to.z - from.z) * t;
			this.value.w = from.w + (to.w - from.w) * t;
		}

		// Token: 0x0600401B RID: 16411 RVA: 0x0017ABD1 File Offset: 0x00178DD1
		public static implicit operator Vector2(Vector4Parameter prop)
		{
			return prop.value;
		}

		// Token: 0x0600401C RID: 16412 RVA: 0x0017ABDE File Offset: 0x00178DDE
		public static implicit operator Vector3(Vector4Parameter prop)
		{
			return prop.value;
		}
	}
}
