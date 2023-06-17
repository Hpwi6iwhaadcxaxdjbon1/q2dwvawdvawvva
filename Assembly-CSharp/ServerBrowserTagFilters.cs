using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000891 RID: 2193
public class ServerBrowserTagFilters : MonoBehaviour
{
	// Token: 0x04003137 RID: 12599
	public UnityEvent TagFiltersChanged = new UnityEvent();

	// Token: 0x04003138 RID: 12600
	private ServerBrowserTagGroup[] _groups;

	// Token: 0x04003139 RID: 12601
	private List<bool> _previousState;

	// Token: 0x060036D2 RID: 14034 RVA: 0x0014AC9C File Offset: 0x00148E9C
	public void Start()
	{
		this._groups = base.gameObject.GetComponentsInChildren<ServerBrowserTagGroup>();
		UnityAction call = delegate()
		{
			UnityEvent tagFiltersChanged = this.TagFiltersChanged;
			if (tagFiltersChanged == null)
			{
				return;
			}
			tagFiltersChanged.Invoke();
		};
		ServerBrowserTagGroup[] groups = this._groups;
		for (int i = 0; i < groups.Length; i++)
		{
			foreach (ServerBrowserTag serverBrowserTag in groups[i].tags)
			{
				serverBrowserTag.button.OnPressed.AddListener(call);
				serverBrowserTag.button.OnReleased.AddListener(call);
			}
		}
	}

	// Token: 0x060036D3 RID: 14035 RVA: 0x0014AD1C File Offset: 0x00148F1C
	public void DeselectAll()
	{
		if (this._groups == null)
		{
			return;
		}
		foreach (ServerBrowserTagGroup serverBrowserTagGroup in this._groups)
		{
			if (serverBrowserTagGroup.tags != null)
			{
				ServerBrowserTag[] tags = serverBrowserTagGroup.tags;
				for (int j = 0; j < tags.Length; j++)
				{
					tags[j].button.SetToggleFalse();
				}
			}
		}
	}

	// Token: 0x060036D4 RID: 14036 RVA: 0x0014AD7C File Offset: 0x00148F7C
	public void GetTags(out List<HashSet<string>> searchTagGroups, out HashSet<string> excludeTags)
	{
		searchTagGroups = new List<HashSet<string>>();
		excludeTags = new HashSet<string>();
		foreach (ServerBrowserTagGroup serverBrowserTagGroup in this._groups)
		{
			if (serverBrowserTagGroup.AnyActive())
			{
				if (serverBrowserTagGroup.isExclusive)
				{
					HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
					foreach (ServerBrowserTag serverBrowserTag in serverBrowserTagGroup.tags)
					{
						if (serverBrowserTag.IsActive)
						{
							hashSet.Add(serverBrowserTag.serverTag);
						}
						else if (serverBrowserTagGroup.isExclusive)
						{
							excludeTags.Add(serverBrowserTag.serverTag);
						}
					}
					if (hashSet.Count > 0)
					{
						searchTagGroups.Add(hashSet);
					}
				}
				else
				{
					foreach (ServerBrowserTag serverBrowserTag2 in serverBrowserTagGroup.tags)
					{
						if (serverBrowserTag2.IsActive)
						{
							HashSet<string> hashSet2 = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
							hashSet2.Add(serverBrowserTag2.serverTag);
							searchTagGroups.Add(hashSet2);
						}
					}
				}
			}
		}
	}
}
