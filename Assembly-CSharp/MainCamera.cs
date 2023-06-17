using System;
using UnityEngine;

// Token: 0x020002C9 RID: 713
[ExecuteInEditMode]
public class MainCamera : RustCamera<MainCamera>
{
	// Token: 0x04001686 RID: 5766
	public static Camera mainCamera;

	// Token: 0x04001687 RID: 5767
	public static Transform mainCameraTransform;

	// Token: 0x1700026B RID: 619
	// (get) Token: 0x06001D7C RID: 7548 RVA: 0x000CAE10 File Offset: 0x000C9010
	public static bool isValid
	{
		get
		{
			return MainCamera.mainCamera != null && MainCamera.mainCamera.enabled;
		}
	}

	// Token: 0x1700026C RID: 620
	// (get) Token: 0x06001D7D RID: 7549 RVA: 0x000CAE2B File Offset: 0x000C902B
	// (set) Token: 0x06001D7E RID: 7550 RVA: 0x000CAE32 File Offset: 0x000C9032
	public static Vector3 velocity { get; private set; }

	// Token: 0x1700026D RID: 621
	// (get) Token: 0x06001D7F RID: 7551 RVA: 0x000CAE3A File Offset: 0x000C903A
	// (set) Token: 0x06001D80 RID: 7552 RVA: 0x000CAE46 File Offset: 0x000C9046
	public static Vector3 position
	{
		get
		{
			return MainCamera.mainCameraTransform.position;
		}
		set
		{
			MainCamera.mainCameraTransform.position = value;
		}
	}

	// Token: 0x1700026E RID: 622
	// (get) Token: 0x06001D81 RID: 7553 RVA: 0x000CAE53 File Offset: 0x000C9053
	// (set) Token: 0x06001D82 RID: 7554 RVA: 0x000CAE5F File Offset: 0x000C905F
	public static Vector3 forward
	{
		get
		{
			return MainCamera.mainCameraTransform.forward;
		}
		set
		{
			if (value.sqrMagnitude > 0f)
			{
				MainCamera.mainCameraTransform.forward = value;
			}
		}
	}

	// Token: 0x1700026F RID: 623
	// (get) Token: 0x06001D83 RID: 7555 RVA: 0x000CAE7A File Offset: 0x000C907A
	// (set) Token: 0x06001D84 RID: 7556 RVA: 0x000CAE86 File Offset: 0x000C9086
	public static Vector3 right
	{
		get
		{
			return MainCamera.mainCameraTransform.right;
		}
		set
		{
			if (value.sqrMagnitude > 0f)
			{
				MainCamera.mainCameraTransform.right = value;
			}
		}
	}

	// Token: 0x17000270 RID: 624
	// (get) Token: 0x06001D85 RID: 7557 RVA: 0x000CAEA1 File Offset: 0x000C90A1
	// (set) Token: 0x06001D86 RID: 7558 RVA: 0x000CAEAD File Offset: 0x000C90AD
	public static Vector3 up
	{
		get
		{
			return MainCamera.mainCameraTransform.up;
		}
		set
		{
			if (value.sqrMagnitude > 0f)
			{
				MainCamera.mainCamera.transform.up = value;
			}
		}
	}

	// Token: 0x17000271 RID: 625
	// (get) Token: 0x06001D87 RID: 7559 RVA: 0x000CAECD File Offset: 0x000C90CD
	// (set) Token: 0x06001D88 RID: 7560 RVA: 0x000CAED9 File Offset: 0x000C90D9
	public static Quaternion rotation
	{
		get
		{
			return MainCamera.mainCameraTransform.rotation;
		}
		set
		{
			MainCamera.mainCameraTransform.rotation = value;
		}
	}

	// Token: 0x17000272 RID: 626
	// (get) Token: 0x06001D89 RID: 7561 RVA: 0x000CAEE6 File Offset: 0x000C90E6
	public static Ray Ray
	{
		get
		{
			return new Ray(MainCamera.position, MainCamera.forward);
		}
	}
}
