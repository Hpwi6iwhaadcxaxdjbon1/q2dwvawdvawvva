using System;
using UnityEngine;

// Token: 0x020002D2 RID: 722
public class ParticleDisableOnParentDestroy : MonoBehaviour, IOnParentDestroying
{
	// Token: 0x040016C9 RID: 5833
	public float destroyAfterSeconds;

	// Token: 0x06001DA7 RID: 7591 RVA: 0x000CB5EC File Offset: 0x000C97EC
	public void OnParentDestroying()
	{
		base.transform.parent = null;
		base.GetComponent<ParticleSystem>().enableEmission = false;
		if (this.destroyAfterSeconds > 0f)
		{
			GameManager.Destroy(base.gameObject, this.destroyAfterSeconds);
		}
	}
}
