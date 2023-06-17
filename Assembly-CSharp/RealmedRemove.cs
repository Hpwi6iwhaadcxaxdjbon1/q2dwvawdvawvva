using System;
using System.Linq;
using UnityEngine;

// Token: 0x0200055F RID: 1375
public class RealmedRemove : MonoBehaviour, IPrefabPreProcess
{
	// Token: 0x04002269 RID: 8809
	public GameObject[] removedFromClient;

	// Token: 0x0400226A RID: 8810
	public Component[] removedComponentFromClient;

	// Token: 0x0400226B RID: 8811
	public GameObject[] removedFromServer;

	// Token: 0x0400226C RID: 8812
	public Component[] removedComponentFromServer;

	// Token: 0x0400226D RID: 8813
	public Component[] doNotRemoveFromServer;

	// Token: 0x0400226E RID: 8814
	public Component[] doNotRemoveFromClient;

	// Token: 0x06002A46 RID: 10822 RVA: 0x00101910 File Offset: 0x000FFB10
	public void PreProcess(IPrefabProcessor process, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if (clientside)
		{
			GameObject[] array = this.removedFromClient;
			for (int i = 0; i < array.Length; i++)
			{
				UnityEngine.Object.DestroyImmediate(array[i], true);
			}
			Component[] array2 = this.removedComponentFromClient;
			for (int i = 0; i < array2.Length; i++)
			{
				UnityEngine.Object.DestroyImmediate(array2[i], true);
			}
		}
		if (serverside)
		{
			GameObject[] array = this.removedFromServer;
			for (int i = 0; i < array.Length; i++)
			{
				UnityEngine.Object.DestroyImmediate(array[i], true);
			}
			Component[] array2 = this.removedComponentFromServer;
			for (int i = 0; i < array2.Length; i++)
			{
				UnityEngine.Object.DestroyImmediate(array2[i], true);
			}
		}
		if (!bundling)
		{
			process.RemoveComponent(this);
		}
	}

	// Token: 0x06002A47 RID: 10823 RVA: 0x001019A8 File Offset: 0x000FFBA8
	public bool ShouldDelete(Component comp, bool client, bool server)
	{
		return (!client || this.doNotRemoveFromClient == null || !this.doNotRemoveFromClient.Contains(comp)) && (!server || this.doNotRemoveFromServer == null || !this.doNotRemoveFromServer.Contains(comp));
	}
}
