using System;
using UnityEngine;

// Token: 0x02000177 RID: 375
public class PlacementTest : MonoBehaviour
{
	// Token: 0x04001068 RID: 4200
	public MeshCollider myMeshCollider;

	// Token: 0x04001069 RID: 4201
	public Transform testTransform;

	// Token: 0x0400106A RID: 4202
	public Transform visualTest;

	// Token: 0x0400106B RID: 4203
	public float hemisphere = 45f;

	// Token: 0x0400106C RID: 4204
	public float clampTest = 45f;

	// Token: 0x0400106D RID: 4205
	public float testDist = 2f;

	// Token: 0x0400106E RID: 4206
	private float nextTest;

	// Token: 0x06001788 RID: 6024 RVA: 0x000B2990 File Offset: 0x000B0B90
	public Vector3 RandomHemisphereDirection(Vector3 input, float degreesOffset)
	{
		degreesOffset = Mathf.Clamp(degreesOffset / 180f, -180f, 180f);
		Vector2 insideUnitCircle = UnityEngine.Random.insideUnitCircle;
		Vector3 b = new Vector3(insideUnitCircle.x * degreesOffset, UnityEngine.Random.Range(-1f, 1f) * degreesOffset, insideUnitCircle.y * degreesOffset);
		return (input + b).normalized;
	}

	// Token: 0x06001789 RID: 6025 RVA: 0x000B29F4 File Offset: 0x000B0BF4
	public Vector3 RandomCylinderPointAroundVector(Vector3 input, float distance, float minHeight = 0f, float maxHeight = 0f)
	{
		Vector2 insideUnitCircle = UnityEngine.Random.insideUnitCircle;
		Vector3 normalized = new Vector3(insideUnitCircle.x, 0f, insideUnitCircle.y).normalized;
		return new Vector3(normalized.x * distance, UnityEngine.Random.Range(minHeight, maxHeight), normalized.z * distance);
	}

	// Token: 0x0600178A RID: 6026 RVA: 0x000B2A44 File Offset: 0x000B0C44
	public Vector3 ClampToHemisphere(Vector3 hemiInput, float degreesOffset, Vector3 inputVec)
	{
		degreesOffset = Mathf.Clamp(degreesOffset / 180f, -180f, 180f);
		Vector3 normalized = (hemiInput + Vector3.one * degreesOffset).normalized;
		Vector3 normalized2 = (hemiInput + Vector3.one * -degreesOffset).normalized;
		for (int i = 0; i < 3; i++)
		{
			inputVec[i] = Mathf.Clamp(inputVec[i], normalized2[i], normalized[i]);
		}
		return inputVec.normalized;
	}

	// Token: 0x0600178B RID: 6027 RVA: 0x000B2AD8 File Offset: 0x000B0CD8
	private void Update()
	{
		if (Time.realtimeSinceStartup < this.nextTest)
		{
			return;
		}
		this.nextTest = Time.realtimeSinceStartup + 0f;
		Vector3 position = this.RandomCylinderPointAroundVector(Vector3.up, 0.5f, 0.25f, 0.5f);
		position = base.transform.TransformPoint(position);
		this.testTransform.transform.position = position;
		if (this.testTransform != null && this.visualTest != null)
		{
			Vector3 position2 = base.transform.position;
			RaycastHit raycastHit;
			if (this.myMeshCollider.Raycast(new Ray(this.testTransform.position, (base.transform.position - this.testTransform.position).normalized), out raycastHit, 5f))
			{
				position2 = raycastHit.point;
			}
			else
			{
				Debug.LogError("Missed");
			}
			this.visualTest.transform.position = position2;
		}
	}

	// Token: 0x0600178C RID: 6028 RVA: 0x000063A5 File Offset: 0x000045A5
	public void OnDrawGizmos()
	{
	}
}
