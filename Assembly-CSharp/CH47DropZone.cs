using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200047D RID: 1149
public class CH47DropZone : MonoBehaviour
{
	// Token: 0x04001E41 RID: 7745
	public float lastDropTime;

	// Token: 0x04001E42 RID: 7746
	private static List<CH47DropZone> dropZones = new List<CH47DropZone>();

	// Token: 0x060025EC RID: 9708 RVA: 0x000EF8F4 File Offset: 0x000EDAF4
	public void Awake()
	{
		if (!CH47DropZone.dropZones.Contains(this))
		{
			CH47DropZone.dropZones.Add(this);
		}
	}

	// Token: 0x060025ED RID: 9709 RVA: 0x000EF910 File Offset: 0x000EDB10
	public static CH47DropZone GetClosest(Vector3 pos)
	{
		float num = float.PositiveInfinity;
		CH47DropZone result = null;
		foreach (CH47DropZone ch47DropZone in CH47DropZone.dropZones)
		{
			float num2 = Vector3Ex.Distance2D(pos, ch47DropZone.transform.position);
			if (num2 < num)
			{
				num = num2;
				result = ch47DropZone;
			}
		}
		return result;
	}

	// Token: 0x060025EE RID: 9710 RVA: 0x000EF984 File Offset: 0x000EDB84
	public void OnDestroy()
	{
		if (CH47DropZone.dropZones.Contains(this))
		{
			CH47DropZone.dropZones.Remove(this);
		}
	}

	// Token: 0x060025EF RID: 9711 RVA: 0x000EF99F File Offset: 0x000EDB9F
	public float TimeSinceLastDrop()
	{
		return Time.time - this.lastDropTime;
	}

	// Token: 0x060025F0 RID: 9712 RVA: 0x000EF9AD File Offset: 0x000EDBAD
	public void Used()
	{
		this.lastDropTime = Time.time;
	}

	// Token: 0x060025F1 RID: 9713 RVA: 0x000EF9BA File Offset: 0x000EDBBA
	public void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(base.transform.position, 5f);
	}
}
