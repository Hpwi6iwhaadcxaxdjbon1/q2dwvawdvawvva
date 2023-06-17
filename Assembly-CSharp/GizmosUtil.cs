using System;
using UnityEngine;

// Token: 0x02000936 RID: 2358
public static class GizmosUtil
{
	// Token: 0x0600389A RID: 14490 RVA: 0x0015194C File Offset: 0x0014FB4C
	public static void DrawWireCircleX(Vector3 pos, float radius)
	{
		Matrix4x4 matrix = Gizmos.matrix;
		Gizmos.matrix *= Matrix4x4.TRS(pos, Quaternion.identity, new Vector3(0f, 1f, 1f));
		Gizmos.DrawWireSphere(Vector3.zero, radius);
		Gizmos.matrix = matrix;
	}

	// Token: 0x0600389B RID: 14491 RVA: 0x0015199C File Offset: 0x0014FB9C
	public static void DrawWireCircleY(Vector3 pos, float radius)
	{
		Matrix4x4 matrix = Gizmos.matrix;
		Gizmos.matrix *= Matrix4x4.TRS(pos, Quaternion.identity, new Vector3(1f, 0f, 1f));
		Gizmos.DrawWireSphere(Vector3.zero, radius);
		Gizmos.matrix = matrix;
	}

	// Token: 0x0600389C RID: 14492 RVA: 0x001519EC File Offset: 0x0014FBEC
	public static void DrawWireCircleZ(Vector3 pos, float radius)
	{
		Matrix4x4 matrix = Gizmos.matrix;
		Gizmos.matrix *= Matrix4x4.TRS(pos, Quaternion.identity, new Vector3(1f, 1f, 0f));
		Gizmos.DrawWireSphere(Vector3.zero, radius);
		Gizmos.matrix = matrix;
	}

	// Token: 0x0600389D RID: 14493 RVA: 0x00151A3C File Offset: 0x0014FC3C
	public static void DrawCircleX(Vector3 pos, float radius)
	{
		Matrix4x4 matrix = Gizmos.matrix;
		Gizmos.matrix *= Matrix4x4.TRS(pos, Quaternion.identity, new Vector3(0f, 1f, 1f));
		Gizmos.DrawSphere(Vector3.zero, radius);
		Gizmos.matrix = matrix;
	}

	// Token: 0x0600389E RID: 14494 RVA: 0x00151A8C File Offset: 0x0014FC8C
	public static void DrawCircleY(Vector3 pos, float radius)
	{
		Matrix4x4 matrix = Gizmos.matrix;
		Gizmos.matrix *= Matrix4x4.TRS(pos, Quaternion.identity, new Vector3(1f, 0f, 1f));
		Gizmos.DrawSphere(Vector3.zero, radius);
		Gizmos.matrix = matrix;
	}

	// Token: 0x0600389F RID: 14495 RVA: 0x00151ADC File Offset: 0x0014FCDC
	public static void DrawCircleZ(Vector3 pos, float radius)
	{
		Matrix4x4 matrix = Gizmos.matrix;
		Gizmos.matrix *= Matrix4x4.TRS(pos, Quaternion.identity, new Vector3(1f, 1f, 0f));
		Gizmos.DrawSphere(Vector3.zero, radius);
		Gizmos.matrix = matrix;
	}

	// Token: 0x060038A0 RID: 14496 RVA: 0x00151B2C File Offset: 0x0014FD2C
	public static void DrawWireCylinderX(Vector3 pos, float radius, float height)
	{
		GizmosUtil.DrawWireCircleX(pos - new Vector3(0.5f * height, 0f, 0f), radius);
		GizmosUtil.DrawWireCircleX(pos + new Vector3(0.5f * height, 0f, 0f), radius);
	}

	// Token: 0x060038A1 RID: 14497 RVA: 0x00151B80 File Offset: 0x0014FD80
	public static void DrawWireCylinderY(Vector3 pos, float radius, float height)
	{
		GizmosUtil.DrawWireCircleY(pos - new Vector3(0f, 0.5f * height, 0f), radius);
		GizmosUtil.DrawWireCircleY(pos + new Vector3(0f, 0.5f * height, 0f), radius);
	}

	// Token: 0x060038A2 RID: 14498 RVA: 0x00151BD4 File Offset: 0x0014FDD4
	public static void DrawWireCylinderZ(Vector3 pos, float radius, float height)
	{
		GizmosUtil.DrawWireCircleZ(pos - new Vector3(0f, 0f, 0.5f * height), radius);
		GizmosUtil.DrawWireCircleZ(pos + new Vector3(0f, 0f, 0.5f * height), radius);
	}

	// Token: 0x060038A3 RID: 14499 RVA: 0x00151C28 File Offset: 0x0014FE28
	public static void DrawCylinderX(Vector3 pos, float radius, float height)
	{
		GizmosUtil.DrawCircleX(pos - new Vector3(0.5f * height, 0f, 0f), radius);
		GizmosUtil.DrawCircleX(pos + new Vector3(0.5f * height, 0f, 0f), radius);
	}

	// Token: 0x060038A4 RID: 14500 RVA: 0x00151C7C File Offset: 0x0014FE7C
	public static void DrawCylinderY(Vector3 pos, float radius, float height)
	{
		GizmosUtil.DrawCircleY(pos - new Vector3(0f, 0.5f * height, 0f), radius);
		GizmosUtil.DrawCircleY(pos + new Vector3(0f, 0.5f * height, 0f), radius);
	}

	// Token: 0x060038A5 RID: 14501 RVA: 0x00151CD0 File Offset: 0x0014FED0
	public static void DrawCylinderZ(Vector3 pos, float radius, float height)
	{
		GizmosUtil.DrawCircleZ(pos - new Vector3(0f, 0f, 0.5f * height), radius);
		GizmosUtil.DrawCircleZ(pos + new Vector3(0f, 0f, 0.5f * height), radius);
	}

	// Token: 0x060038A6 RID: 14502 RVA: 0x00151D24 File Offset: 0x0014FF24
	public static void DrawWireCapsuleX(Vector3 pos, float radius, float height)
	{
		Vector3 vector = pos - new Vector3(0.5f * height, 0f, 0f) + Vector3.right * radius;
		Vector3 vector2 = pos + new Vector3(0.5f * height, 0f, 0f) - Vector3.right * radius;
		Gizmos.DrawWireSphere(vector, radius);
		Gizmos.DrawWireSphere(vector2, radius);
		Gizmos.DrawLine(vector + Vector3.forward * radius, vector2 + Vector3.forward * radius);
		Gizmos.DrawLine(vector + Vector3.up * radius, vector2 + Vector3.up * radius);
		Gizmos.DrawLine(vector + Vector3.back * radius, vector2 + Vector3.back * radius);
		Gizmos.DrawLine(vector + Vector3.down * radius, vector2 + Vector3.down * radius);
	}

	// Token: 0x060038A7 RID: 14503 RVA: 0x00151E34 File Offset: 0x00150034
	public static void DrawWireCapsuleY(Vector3 pos, float radius, float height)
	{
		Vector3 vector = pos - new Vector3(0f, 0.5f * height, 0f) + Vector3.up * radius;
		Vector3 vector2 = pos + new Vector3(0f, 0.5f * height, 0f) - Vector3.up * radius;
		Gizmos.DrawWireSphere(vector, radius);
		Gizmos.DrawWireSphere(vector2, radius);
		Gizmos.DrawLine(vector + Vector3.forward * radius, vector2 + Vector3.forward * radius);
		Gizmos.DrawLine(vector + Vector3.right * radius, vector2 + Vector3.right * radius);
		Gizmos.DrawLine(vector + Vector3.back * radius, vector2 + Vector3.back * radius);
		Gizmos.DrawLine(vector + Vector3.left * radius, vector2 + Vector3.left * radius);
	}

	// Token: 0x060038A8 RID: 14504 RVA: 0x00151F44 File Offset: 0x00150144
	public static void DrawWireCapsuleZ(Vector3 pos, float radius, float height)
	{
		Vector3 vector = pos - new Vector3(0f, 0f, 0.5f * height) + Vector3.forward * radius;
		Vector3 vector2 = pos + new Vector3(0f, 0f, 0.5f * height) - Vector3.forward * radius;
		Gizmos.DrawWireSphere(vector, radius);
		Gizmos.DrawWireSphere(vector2, radius);
		Gizmos.DrawLine(vector + Vector3.up * radius, vector2 + Vector3.up * radius);
		Gizmos.DrawLine(vector + Vector3.right * radius, vector2 + Vector3.right * radius);
		Gizmos.DrawLine(vector + Vector3.down * radius, vector2 + Vector3.down * radius);
		Gizmos.DrawLine(vector + Vector3.left * radius, vector2 + Vector3.left * radius);
	}

	// Token: 0x060038A9 RID: 14505 RVA: 0x00152054 File Offset: 0x00150254
	public static void DrawCapsuleX(Vector3 pos, float radius, float height)
	{
		Vector3 center = pos - new Vector3(0.5f * height, 0f, 0f);
		Vector3 center2 = pos + new Vector3(0.5f * height, 0f, 0f);
		Gizmos.DrawSphere(center, radius);
		Gizmos.DrawSphere(center2, radius);
	}

	// Token: 0x060038AA RID: 14506 RVA: 0x001520A8 File Offset: 0x001502A8
	public static void DrawCapsuleY(Vector3 pos, float radius, float height)
	{
		Vector3 center = pos - new Vector3(0f, 0.5f * height, 0f);
		Vector3 center2 = pos + new Vector3(0f, 0.5f * height, 0f);
		Gizmos.DrawSphere(center, radius);
		Gizmos.DrawSphere(center2, radius);
	}

	// Token: 0x060038AB RID: 14507 RVA: 0x001520FC File Offset: 0x001502FC
	public static void DrawCapsuleZ(Vector3 pos, float radius, float height)
	{
		Vector3 center = pos - new Vector3(0f, 0f, 0.5f * height);
		Vector3 center2 = pos + new Vector3(0f, 0f, 0.5f * height);
		Gizmos.DrawSphere(center, radius);
		Gizmos.DrawSphere(center2, radius);
	}

	// Token: 0x060038AC RID: 14508 RVA: 0x0015214F File Offset: 0x0015034F
	public static void DrawWireCube(Vector3 pos, Vector3 size, Quaternion rot)
	{
		Matrix4x4 matrix = Gizmos.matrix;
		Gizmos.matrix = Matrix4x4.TRS(pos, rot, size);
		Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
		Gizmos.matrix = matrix;
	}

	// Token: 0x060038AD RID: 14509 RVA: 0x00152177 File Offset: 0x00150377
	public static void DrawCube(Vector3 pos, Vector3 size, Quaternion rot)
	{
		Matrix4x4 matrix = Gizmos.matrix;
		Gizmos.matrix = Matrix4x4.TRS(pos, rot, size);
		Gizmos.DrawCube(Vector3.zero, Vector3.one);
		Gizmos.matrix = matrix;
	}

	// Token: 0x060038AE RID: 14510 RVA: 0x001521A0 File Offset: 0x001503A0
	public static void DrawWirePath(Vector3 a, Vector3 b, float thickness)
	{
		GizmosUtil.DrawWireCircleY(a, thickness);
		GizmosUtil.DrawWireCircleY(b, thickness);
		Vector3 normalized = (b - a).normalized;
		Vector3 a2 = Quaternion.Euler(0f, 90f, 0f) * normalized;
		Gizmos.DrawLine(b + a2 * thickness, a + a2 * thickness);
		Gizmos.DrawLine(b - a2 * thickness, a - a2 * thickness);
	}

	// Token: 0x060038AF RID: 14511 RVA: 0x00152224 File Offset: 0x00150424
	public static void DrawSemiCircle(float radius)
	{
		float num = radius * 0.017453292f * 0.5f;
		Vector3 vector = Mathf.Cos(num) * Vector3.forward + Mathf.Sin(num) * Vector3.right;
		Gizmos.DrawLine(Vector3.zero, vector);
		Vector3 vector2 = Mathf.Cos(-num) * Vector3.forward + Mathf.Sin(-num) * Vector3.right;
		Gizmos.DrawLine(Vector3.zero, vector2);
		float num2 = Mathf.Clamp(radius / 16f, 4f, 64f);
		float num3 = num / num2;
		for (float num4 = num; num4 > 0f; num4 -= num3)
		{
			Vector3 vector3 = Mathf.Cos(num4) * Vector3.forward + Mathf.Sin(num4) * Vector3.right;
			Gizmos.DrawLine(Vector3.zero, vector3);
			if (vector != Vector3.zero)
			{
				Gizmos.DrawLine(vector3, vector);
			}
			vector = vector3;
			Vector3 vector4 = Mathf.Cos(-num4) * Vector3.forward + Mathf.Sin(-num4) * Vector3.right;
			Gizmos.DrawLine(Vector3.zero, vector4);
			if (vector2 != Vector3.zero)
			{
				Gizmos.DrawLine(vector4, vector2);
			}
			vector2 = vector4;
		}
		Gizmos.DrawLine(vector, vector2);
	}

	// Token: 0x060038B0 RID: 14512 RVA: 0x00152380 File Offset: 0x00150580
	public static void DrawMeshes(Transform transform)
	{
		foreach (MeshRenderer meshRenderer in transform.GetComponentsInChildren<MeshRenderer>())
		{
			if (meshRenderer.enabled)
			{
				MeshFilter component = meshRenderer.GetComponent<MeshFilter>();
				if (component)
				{
					Transform transform2 = meshRenderer.transform;
					if (transform2 != null && component != null && component.sharedMesh != null && component.sharedMesh.normals != null && component.sharedMesh.normals.Length != 0)
					{
						Gizmos.DrawMesh(component.sharedMesh, transform2.position, transform2.rotation, transform2.lossyScale);
					}
				}
			}
		}
	}

	// Token: 0x060038B1 RID: 14513 RVA: 0x00152428 File Offset: 0x00150628
	public static void DrawBounds(Transform transform)
	{
		Bounds bounds = transform.GetBounds(true, false, true);
		Vector3 lossyScale = transform.lossyScale;
		Quaternion rotation = transform.rotation;
		Vector3 pos = transform.position + rotation * Vector3.Scale(lossyScale, bounds.center);
		Vector3 size = Vector3.Scale(lossyScale, bounds.size);
		GizmosUtil.DrawCube(pos, size, rotation);
		GizmosUtil.DrawWireCube(pos, size, rotation);
	}
}
