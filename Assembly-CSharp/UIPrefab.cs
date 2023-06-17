using System;
using Facepunch;
using UnityEngine;

// Token: 0x020008CF RID: 2255
public class UIPrefab : MonoBehaviour
{
	// Token: 0x04003278 RID: 12920
	public GameObject prefabSource;

	// Token: 0x04003279 RID: 12921
	internal GameObject createdGameObject;

	// Token: 0x0600376D RID: 14189 RVA: 0x0014D7AC File Offset: 0x0014B9AC
	private void Awake()
	{
		if (this.prefabSource == null)
		{
			return;
		}
		if (this.createdGameObject != null)
		{
			return;
		}
		this.createdGameObject = Facepunch.Instantiate.GameObject(this.prefabSource, null);
		this.createdGameObject.name = this.prefabSource.name;
		this.createdGameObject.transform.SetParent(base.transform, false);
		this.createdGameObject.Identity();
	}

	// Token: 0x0600376E RID: 14190 RVA: 0x0014D821 File Offset: 0x0014BA21
	public void SetVisible(bool visible)
	{
		if (this.createdGameObject == null)
		{
			return;
		}
		if (this.createdGameObject.activeSelf == visible)
		{
			return;
		}
		this.createdGameObject.SetActive(visible);
	}
}
