using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A82 RID: 2690
	[Serializable]
	public sealed class Vector3Parameter : ParameterOverride<Vector3>
	{
		// Token: 0x06004016 RID: 16406 RVA: 0x0017AAAC File Offset: 0x00178CAC
		public override void Interp(Vector3 from, Vector3 to, float t)
		{
			this.value.x = from.x + (to.x - from.x) * t;
			this.value.y = from.y + (to.y - from.y) * t;
			this.value.z = from.z + (to.z - from.z) * t;
		}

		// Token: 0x06004017 RID: 16407 RVA: 0x0017AB1C File Offset: 0x00178D1C
		public static implicit operator Vector2(Vector3Parameter prop)
		{
			return prop.value;
		}

		// Token: 0x06004018 RID: 16408 RVA: 0x0017AB29 File Offset: 0x00178D29
		public static implicit operator Vector4(Vector3Parameter prop)
		{
			return prop.value;
		}
	}
}
