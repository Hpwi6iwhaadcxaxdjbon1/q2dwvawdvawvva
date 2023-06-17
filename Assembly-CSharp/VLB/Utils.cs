using System;
using UnityEngine;

namespace VLB
{
	// Token: 0x020009B9 RID: 2489
	public static class Utils
	{
		// Token: 0x040035EF RID: 13807
		private static Utils.FloatPackingPrecision ms_FloatPackingPrecision;

		// Token: 0x040035F0 RID: 13808
		private const int kFloatPackingHighMinShaderLevel = 35;

		// Token: 0x06003B50 RID: 15184 RVA: 0x0015FB28 File Offset: 0x0015DD28
		public static string GetPath(Transform current)
		{
			if (current.parent == null)
			{
				return "/" + current.name;
			}
			return Utils.GetPath(current.parent) + "/" + current.name;
		}

		// Token: 0x06003B51 RID: 15185 RVA: 0x0015FB64 File Offset: 0x0015DD64
		public static T NewWithComponent<T>(string name) where T : Component
		{
			return new GameObject(name, new Type[]
			{
				typeof(T)
			}).GetComponent<T>();
		}

		// Token: 0x06003B52 RID: 15186 RVA: 0x0015FB84 File Offset: 0x0015DD84
		public static T GetOrAddComponent<T>(this GameObject self) where T : Component
		{
			T t = self.GetComponent<T>();
			if (t == null)
			{
				t = self.AddComponent<T>();
			}
			return t;
		}

		// Token: 0x06003B53 RID: 15187 RVA: 0x0015FBAE File Offset: 0x0015DDAE
		public static T GetOrAddComponent<T>(this MonoBehaviour self) where T : Component
		{
			return self.gameObject.GetOrAddComponent<T>();
		}

		// Token: 0x06003B54 RID: 15188 RVA: 0x0015FBBB File Offset: 0x0015DDBB
		public static bool HasFlag(this Enum mask, Enum flags)
		{
			return ((int)mask & (int)flags) == (int)flags;
		}

		// Token: 0x06003B55 RID: 15189 RVA: 0x0015FBD2 File Offset: 0x0015DDD2
		public static Vector2 xy(this Vector3 aVector)
		{
			return new Vector2(aVector.x, aVector.y);
		}

		// Token: 0x06003B56 RID: 15190 RVA: 0x0015FBE5 File Offset: 0x0015DDE5
		public static Vector2 xz(this Vector3 aVector)
		{
			return new Vector2(aVector.x, aVector.z);
		}

		// Token: 0x06003B57 RID: 15191 RVA: 0x0015FBF8 File Offset: 0x0015DDF8
		public static Vector2 yz(this Vector3 aVector)
		{
			return new Vector2(aVector.y, aVector.z);
		}

		// Token: 0x06003B58 RID: 15192 RVA: 0x0015FC0B File Offset: 0x0015DE0B
		public static Vector2 yx(this Vector3 aVector)
		{
			return new Vector2(aVector.y, aVector.x);
		}

		// Token: 0x06003B59 RID: 15193 RVA: 0x0015FC1E File Offset: 0x0015DE1E
		public static Vector2 zx(this Vector3 aVector)
		{
			return new Vector2(aVector.z, aVector.x);
		}

		// Token: 0x06003B5A RID: 15194 RVA: 0x0015FC31 File Offset: 0x0015DE31
		public static Vector2 zy(this Vector3 aVector)
		{
			return new Vector2(aVector.z, aVector.y);
		}

		// Token: 0x06003B5B RID: 15195 RVA: 0x0015FC44 File Offset: 0x0015DE44
		public static float GetVolumeCubic(this Bounds self)
		{
			return self.size.x * self.size.y * self.size.z;
		}

		// Token: 0x06003B5C RID: 15196 RVA: 0x0015FC6C File Offset: 0x0015DE6C
		public static float GetMaxArea2D(this Bounds self)
		{
			return Mathf.Max(Mathf.Max(self.size.x * self.size.y, self.size.y * self.size.z), self.size.x * self.size.z);
		}

		// Token: 0x06003B5D RID: 15197 RVA: 0x0015FCCE File Offset: 0x0015DECE
		public static Color Opaque(this Color self)
		{
			return new Color(self.r, self.g, self.b, 1f);
		}

		// Token: 0x06003B5E RID: 15198 RVA: 0x0015FCEC File Offset: 0x0015DEEC
		public static void GizmosDrawPlane(Vector3 normal, Vector3 position, Color color, float size = 1f)
		{
			Vector3 vector = Vector3.Cross(normal, (Mathf.Abs(Vector3.Dot(normal, Vector3.forward)) < 0.999f) ? Vector3.forward : Vector3.up).normalized * size;
			Vector3 vector2 = position + vector;
			Vector3 vector3 = position - vector;
			vector = Quaternion.AngleAxis(90f, normal) * vector;
			Vector3 vector4 = position + vector;
			Vector3 vector5 = position - vector;
			Gizmos.matrix = Matrix4x4.identity;
			Gizmos.color = color;
			Gizmos.DrawLine(vector2, vector3);
			Gizmos.DrawLine(vector4, vector5);
			Gizmos.DrawLine(vector2, vector4);
			Gizmos.DrawLine(vector4, vector3);
			Gizmos.DrawLine(vector3, vector5);
			Gizmos.DrawLine(vector5, vector2);
		}

		// Token: 0x06003B5F RID: 15199 RVA: 0x0015FDA2 File Offset: 0x0015DFA2
		public static Plane TranslateCustom(this Plane plane, Vector3 translation)
		{
			plane.distance += Vector3.Dot(translation.normalized, plane.normal) * translation.magnitude;
			return plane;
		}

		// Token: 0x06003B60 RID: 15200 RVA: 0x0015FDD0 File Offset: 0x0015DFD0
		public static bool IsValid(this Plane plane)
		{
			return plane.normal.sqrMagnitude > 0.5f;
		}

		// Token: 0x06003B61 RID: 15201 RVA: 0x0015FDF4 File Offset: 0x0015DFF4
		public static Matrix4x4 SampleInMatrix(this Gradient self, int floatPackingPrecision)
		{
			Matrix4x4 result = default(Matrix4x4);
			for (int i = 0; i < 16; i++)
			{
				Color color = self.Evaluate(Mathf.Clamp01((float)i / 15f));
				result[i] = color.PackToFloat(floatPackingPrecision);
			}
			return result;
		}

		// Token: 0x06003B62 RID: 15202 RVA: 0x0015FE3C File Offset: 0x0015E03C
		public static Color[] SampleInArray(this Gradient self, int samplesCount)
		{
			Color[] array = new Color[samplesCount];
			for (int i = 0; i < samplesCount; i++)
			{
				array[i] = self.Evaluate(Mathf.Clamp01((float)i / (float)(samplesCount - 1)));
			}
			return array;
		}

		// Token: 0x06003B63 RID: 15203 RVA: 0x0015FE76 File Offset: 0x0015E076
		private static Vector4 Vector4_Floor(Vector4 vec)
		{
			return new Vector4(Mathf.Floor(vec.x), Mathf.Floor(vec.y), Mathf.Floor(vec.z), Mathf.Floor(vec.w));
		}

		// Token: 0x06003B64 RID: 15204 RVA: 0x0015FEAC File Offset: 0x0015E0AC
		public static float PackToFloat(this Color color, int floatPackingPrecision)
		{
			Vector4 vector = Utils.Vector4_Floor(color * (float)(floatPackingPrecision - 1));
			return 0f + vector.x * (float)floatPackingPrecision * (float)floatPackingPrecision * (float)floatPackingPrecision + vector.y * (float)floatPackingPrecision * (float)floatPackingPrecision + vector.z * (float)floatPackingPrecision + vector.w;
		}

		// Token: 0x06003B65 RID: 15205 RVA: 0x0015FF01 File Offset: 0x0015E101
		public static Utils.FloatPackingPrecision GetFloatPackingPrecision()
		{
			if (Utils.ms_FloatPackingPrecision == Utils.FloatPackingPrecision.Undef)
			{
				Utils.ms_FloatPackingPrecision = ((SystemInfo.graphicsShaderLevel >= 35) ? Utils.FloatPackingPrecision.High : Utils.FloatPackingPrecision.Low);
			}
			return Utils.ms_FloatPackingPrecision;
		}

		// Token: 0x06003B66 RID: 15206 RVA: 0x000063A5 File Offset: 0x000045A5
		public static void MarkCurrentSceneDirty()
		{
		}

		// Token: 0x02000EE1 RID: 3809
		public enum FloatPackingPrecision
		{
			// Token: 0x04004D73 RID: 19827
			High = 64,
			// Token: 0x04004D74 RID: 19828
			Low = 8,
			// Token: 0x04004D75 RID: 19829
			Undef = 0
		}
	}
}
