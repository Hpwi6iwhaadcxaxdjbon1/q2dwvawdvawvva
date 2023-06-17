using System;
using UnityEngine;

// Token: 0x02000484 RID: 1156
public class MLRSAmmoIcon : MonoBehaviour
{
	// Token: 0x04001E61 RID: 7777
	[SerializeField]
	private GameObject fill;

	// Token: 0x0600262D RID: 9773 RVA: 0x000F0E18 File Offset: 0x000EF018
	protected void Awake()
	{
		this.SetState(false);
	}

	// Token: 0x0600262E RID: 9774 RVA: 0x000F0E21 File Offset: 0x000EF021
	public void SetState(bool filled)
	{
		this.fill.SetActive(filled);
	}
}
