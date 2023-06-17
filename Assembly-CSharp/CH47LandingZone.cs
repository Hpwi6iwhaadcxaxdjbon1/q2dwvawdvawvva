using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000480 RID: 1152
public class CH47LandingZone : MonoBehaviour
{
	// Token: 0x04001E5A RID: 7770
	public float lastDropTime;

	// Token: 0x04001E5B RID: 7771
	private static List<CH47LandingZone> landingZones = new List<CH47LandingZone>();

	// Token: 0x04001E5C RID: 7772
	public float dropoffScale = 1f;

	// Token: 0x0600261F RID: 9759 RVA: 0x000F08D0 File Offset: 0x000EEAD0
	public void Awake()
	{
		if (!CH47LandingZone.landingZones.Contains(this))
		{
			CH47LandingZone.landingZones.Add(this);
		}
	}

	// Token: 0x06002620 RID: 9760 RVA: 0x000F08EC File Offset: 0x000EEAEC
	public static CH47LandingZone GetClosest(Vector3 pos)
	{
		float num = float.PositiveInfinity;
		CH47LandingZone result = null;
		foreach (CH47LandingZone ch47LandingZone in CH47LandingZone.landingZones)
		{
			float num2 = Vector3Ex.Distance2D(pos, ch47LandingZone.transform.position);
			if (num2 < num)
			{
				num = num2;
				result = ch47LandingZone;
			}
		}
		return result;
	}

	// Token: 0x06002621 RID: 9761 RVA: 0x000F0960 File Offset: 0x000EEB60
	public void OnDestroy()
	{
		if (CH47LandingZone.landingZones.Contains(this))
		{
			CH47LandingZone.landingZones.Remove(this);
		}
	}

	// Token: 0x06002622 RID: 9762 RVA: 0x000F097B File Offset: 0x000EEB7B
	public float TimeSinceLastDrop()
	{
		return Time.time - this.lastDropTime;
	}

	// Token: 0x06002623 RID: 9763 RVA: 0x000F0989 File Offset: 0x000EEB89
	public void Used()
	{
		this.lastDropTime = Time.time;
	}

	// Token: 0x06002624 RID: 9764 RVA: 0x000F0998 File Offset: 0x000EEB98
	public void OnDrawGizmos()
	{
		Color magenta = Color.magenta;
		magenta.a = 0.25f;
		Gizmos.color = magenta;
		GizmosUtil.DrawCircleY(base.transform.position, 6f);
		magenta.a = 1f;
		Gizmos.color = magenta;
		GizmosUtil.DrawWireCircleY(base.transform.position, 6f);
	}
}
