using System;
using UnityEngine;

// Token: 0x02000289 RID: 649
public class AnimalFootIK : MonoBehaviour
{
	// Token: 0x040015B7 RID: 5559
	public Transform[] Feet;

	// Token: 0x040015B8 RID: 5560
	public Animator animator;

	// Token: 0x040015B9 RID: 5561
	public float maxWeightDistance = 0.1f;

	// Token: 0x040015BA RID: 5562
	public float minWeightDistance = 0.025f;

	// Token: 0x040015BB RID: 5563
	public float actualFootOffset = 0.01f;

	// Token: 0x06001CFD RID: 7421 RVA: 0x000C8914 File Offset: 0x000C6B14
	public bool GroundSample(Vector3 origin, out RaycastHit hit)
	{
		return Physics.Raycast(origin + Vector3.up * 0.5f, Vector3.down, out hit, 1f, 455155969);
	}

	// Token: 0x06001CFE RID: 7422 RVA: 0x000063A5 File Offset: 0x000045A5
	public void Start()
	{
	}

	// Token: 0x06001CFF RID: 7423 RVA: 0x000C8945 File Offset: 0x000C6B45
	public AvatarIKGoal GoalFromIndex(int index)
	{
		if (index == 0)
		{
			return AvatarIKGoal.LeftHand;
		}
		if (index == 1)
		{
			return AvatarIKGoal.RightHand;
		}
		if (index == 2)
		{
			return AvatarIKGoal.LeftFoot;
		}
		if (index == 3)
		{
			return AvatarIKGoal.RightFoot;
		}
		return AvatarIKGoal.LeftHand;
	}

	// Token: 0x06001D00 RID: 7424 RVA: 0x000C8960 File Offset: 0x000C6B60
	private void OnAnimatorIK(int layerIndex)
	{
		Debug.Log("animal ik!");
		for (int i = 0; i < 4; i++)
		{
			Transform transform = this.Feet[i];
			AvatarIKGoal goal = this.GoalFromIndex(i);
			Vector3 up = Vector3.up;
			Vector3 vector = transform.transform.position;
			float value = this.animator.GetIKPositionWeight(goal);
			RaycastHit raycastHit;
			if (this.GroundSample(transform.transform.position - Vector3.down * this.actualFootOffset, out raycastHit))
			{
				Vector3 normal = raycastHit.normal;
				vector = raycastHit.point;
				float value2 = Vector3.Distance(transform.transform.position - Vector3.down * this.actualFootOffset, vector);
				value = 1f - Mathf.InverseLerp(this.minWeightDistance, this.maxWeightDistance, value2);
				this.animator.SetIKPosition(goal, vector + Vector3.up * this.actualFootOffset);
			}
			else
			{
				value = 0f;
			}
			this.animator.SetIKPositionWeight(goal, value);
		}
	}
}
