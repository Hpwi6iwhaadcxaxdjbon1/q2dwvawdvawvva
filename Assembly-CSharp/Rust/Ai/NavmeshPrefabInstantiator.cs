using System;
using UnityEngine;

namespace Rust.Ai
{
	// Token: 0x02000B3E RID: 2878
	public class NavmeshPrefabInstantiator : MonoBehaviour
	{
		// Token: 0x04003E3B RID: 15931
		public GameObjectRef NavmeshPrefab;

		// Token: 0x060045D7 RID: 17879 RVA: 0x00197D59 File Offset: 0x00195F59
		private void Start()
		{
			if (this.NavmeshPrefab != null)
			{
				this.NavmeshPrefab.Instantiate(base.transform).SetActive(true);
				UnityEngine.Object.Destroy(this);
			}
		}
	}
}
