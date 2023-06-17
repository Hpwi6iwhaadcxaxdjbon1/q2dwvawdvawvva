using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000804 RID: 2052
public class UIBuffs : SingletonComponent<UIBuffs>
{
	// Token: 0x04002E09 RID: 11785
	public bool Enabled = true;

	// Token: 0x04002E0A RID: 11786
	public Transform PrefabBuffIcon;

	// Token: 0x060035B9 RID: 13753 RVA: 0x00148100 File Offset: 0x00146300
	public void Refresh(PlayerModifiers modifiers)
	{
		if (!this.Enabled)
		{
			return;
		}
		this.RemoveAll();
		if (modifiers == null)
		{
			return;
		}
		using (List<Modifier>.Enumerator enumerator = modifiers.All.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current != null)
				{
					UnityEngine.Object.Instantiate<Transform>(this.PrefabBuffIcon).SetParent(base.transform);
				}
			}
		}
	}

	// Token: 0x060035BA RID: 13754 RVA: 0x0014817C File Offset: 0x0014637C
	private void RemoveAll()
	{
		foreach (object obj in base.transform)
		{
			UnityEngine.Object.Destroy(((Transform)obj).gameObject);
		}
	}
}
