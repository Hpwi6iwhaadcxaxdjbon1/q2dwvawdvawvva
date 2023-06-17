using System;
using UnityEngine;

// Token: 0x0200085E RID: 2142
public class InventoryUI : MonoBehaviour
{
	// Token: 0x04003022 RID: 12322
	public GameObject ContactsButton;

	// Token: 0x06003637 RID: 13879 RVA: 0x001491CC File Offset: 0x001473CC
	private void Update()
	{
		if (this.ContactsButton != null && RelationshipManager.contacts != this.ContactsButton.activeSelf)
		{
			this.ContactsButton.SetActive(RelationshipManager.contacts);
		}
	}
}
