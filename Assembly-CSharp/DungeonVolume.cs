using System;
using UnityEngine;

// Token: 0x02000673 RID: 1651
public class DungeonVolume : MonoBehaviour
{
	// Token: 0x04002707 RID: 9991
	public Bounds bounds = new Bounds(Vector3.zero, Vector3.one);

	// Token: 0x06002F97 RID: 12183 RVA: 0x0011E358 File Offset: 0x0011C558
	public OBB GetBounds(Vector3 position, Quaternion rotation)
	{
		position += rotation * (base.transform.localRotation * this.bounds.center + base.transform.localPosition);
		return new OBB(position, this.bounds.size, rotation * base.transform.localRotation);
	}

	// Token: 0x06002F98 RID: 12184 RVA: 0x0011E3C0 File Offset: 0x0011C5C0
	public OBB GetBounds(Vector3 position, Quaternion rotation, Vector3 extrude)
	{
		position += rotation * (base.transform.localRotation * this.bounds.center + base.transform.localPosition);
		return new OBB(position, this.bounds.size + extrude, rotation * base.transform.localRotation);
	}
}
