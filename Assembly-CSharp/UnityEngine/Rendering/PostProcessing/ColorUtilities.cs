using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A95 RID: 2709
	public static class ColorUtilities
	{
		// Token: 0x04003995 RID: 14741
		private const float logC_cut = 0.011361f;

		// Token: 0x04003996 RID: 14742
		private const float logC_a = 5.555556f;

		// Token: 0x04003997 RID: 14743
		private const float logC_b = 0.047996f;

		// Token: 0x04003998 RID: 14744
		private const float logC_c = 0.244161f;

		// Token: 0x04003999 RID: 14745
		private const float logC_d = 0.386036f;

		// Token: 0x0400399A RID: 14746
		private const float logC_e = 5.301883f;

		// Token: 0x0400399B RID: 14747
		private const float logC_f = 0.092819f;

		// Token: 0x06004085 RID: 16517 RVA: 0x0017C8C4 File Offset: 0x0017AAC4
		public static float StandardIlluminantY(float x)
		{
			return 2.87f * x - 3f * x * x - 0.27509508f;
		}

		// Token: 0x06004086 RID: 16518 RVA: 0x0017C8E0 File Offset: 0x0017AAE0
		public static Vector3 CIExyToLMS(float x, float y)
		{
			float num = 1f;
			float num2 = num * x / y;
			float num3 = num * (1f - x - y) / y;
			float x2 = 0.7328f * num2 + 0.4296f * num - 0.1624f * num3;
			float y2 = -0.7036f * num2 + 1.6975f * num + 0.0061f * num3;
			float z = 0.003f * num2 + 0.0136f * num + 0.9834f * num3;
			return new Vector3(x2, y2, z);
		}

		// Token: 0x06004087 RID: 16519 RVA: 0x0017C958 File Offset: 0x0017AB58
		public static Vector3 ComputeColorBalance(float temperature, float tint)
		{
			float num = temperature / 60f;
			float num2 = tint / 60f;
			float x = 0.31271f - num * ((num < 0f) ? 0.1f : 0.05f);
			float y = ColorUtilities.StandardIlluminantY(x) + num2 * 0.05f;
			Vector3 vector = new Vector3(0.949237f, 1.03542f, 1.08728f);
			Vector3 vector2 = ColorUtilities.CIExyToLMS(x, y);
			return new Vector3(vector.x / vector2.x, vector.y / vector2.y, vector.z / vector2.z);
		}

		// Token: 0x06004088 RID: 16520 RVA: 0x0017C9F0 File Offset: 0x0017ABF0
		public static Vector3 ColorToLift(Vector4 color)
		{
			Vector3 vector = new Vector3(color.x, color.y, color.z);
			float num = vector.x * 0.2126f + vector.y * 0.7152f + vector.z * 0.0722f;
			vector = new Vector3(vector.x - num, vector.y - num, vector.z - num);
			float w = color.w;
			return new Vector3(vector.x + w, vector.y + w, vector.z + w);
		}

		// Token: 0x06004089 RID: 16521 RVA: 0x0017CA80 File Offset: 0x0017AC80
		public static Vector3 ColorToInverseGamma(Vector4 color)
		{
			Vector3 vector = new Vector3(color.x, color.y, color.z);
			float num = vector.x * 0.2126f + vector.y * 0.7152f + vector.z * 0.0722f;
			vector = new Vector3(vector.x - num, vector.y - num, vector.z - num);
			float num2 = color.w + 1f;
			return new Vector3(1f / Mathf.Max(vector.x + num2, 0.001f), 1f / Mathf.Max(vector.y + num2, 0.001f), 1f / Mathf.Max(vector.z + num2, 0.001f));
		}

		// Token: 0x0600408A RID: 16522 RVA: 0x0017CB48 File Offset: 0x0017AD48
		public static Vector3 ColorToGain(Vector4 color)
		{
			Vector3 vector = new Vector3(color.x, color.y, color.z);
			float num = vector.x * 0.2126f + vector.y * 0.7152f + vector.z * 0.0722f;
			vector = new Vector3(vector.x - num, vector.y - num, vector.z - num);
			float num2 = color.w + 1f;
			return new Vector3(vector.x + num2, vector.y + num2, vector.z + num2);
		}

		// Token: 0x0600408B RID: 16523 RVA: 0x0017CBDE File Offset: 0x0017ADDE
		public static float LogCToLinear(float x)
		{
			if (x <= 0.1530537f)
			{
				return (x - 0.092819f) / 5.301883f;
			}
			return (Mathf.Pow(10f, (x - 0.386036f) / 0.244161f) - 0.047996f) / 5.555556f;
		}

		// Token: 0x0600408C RID: 16524 RVA: 0x0017CC19 File Offset: 0x0017AE19
		public static float LinearToLogC(float x)
		{
			if (x <= 0.011361f)
			{
				return 5.301883f * x + 0.092819f;
			}
			return 0.244161f * Mathf.Log10(5.555556f * x + 0.047996f) + 0.386036f;
		}

		// Token: 0x0600408D RID: 16525 RVA: 0x0017CC50 File Offset: 0x0017AE50
		public static uint ToHex(Color c)
		{
			return (uint)(c.a * 255f) << 24 | (uint)(c.r * 255f) << 16 | (uint)(c.g * 255f) << 8 | (uint)(c.b * 255f);
		}

		// Token: 0x0600408E RID: 16526 RVA: 0x0017CC9C File Offset: 0x0017AE9C
		public static Color ToRGBA(uint hex)
		{
			return new Color((hex >> 16 & 255U) / 255f, (hex >> 8 & 255U) / 255f, (hex & 255U) / 255f, (hex >> 24 & 255U) / 255f);
		}
	}
}
