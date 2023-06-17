using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch;
using UnityEngine;

// Token: 0x0200057B RID: 1403
[CreateAssetMenu(fileName = "NewTechTree", menuName = "Rust/Tech Tree", order = 2)]
public class TechTreeData : ScriptableObject
{
	// Token: 0x040022F1 RID: 8945
	public string shortname;

	// Token: 0x040022F2 RID: 8946
	public int nextID;

	// Token: 0x040022F3 RID: 8947
	private Dictionary<int, TechTreeData.NodeInstance> _idToNode;

	// Token: 0x040022F4 RID: 8948
	private TechTreeData.NodeInstance _entryNode;

	// Token: 0x040022F5 RID: 8949
	public List<TechTreeData.NodeInstance> nodes = new List<TechTreeData.NodeInstance>();

	// Token: 0x06002B07 RID: 11015 RVA: 0x001057EC File Offset: 0x001039EC
	public TechTreeData.NodeInstance GetByID(int id)
	{
		if (UnityEngine.Application.isPlaying)
		{
			if (this._idToNode == null)
			{
				this._idToNode = this.nodes.ToDictionary((TechTreeData.NodeInstance n) => n.id, (TechTreeData.NodeInstance n) => n);
			}
			TechTreeData.NodeInstance result;
			this._idToNode.TryGetValue(id, out result);
			return result;
		}
		this._idToNode = null;
		foreach (TechTreeData.NodeInstance nodeInstance in this.nodes)
		{
			if (nodeInstance.id == id)
			{
				return nodeInstance;
			}
		}
		return null;
	}

	// Token: 0x06002B08 RID: 11016 RVA: 0x001058C0 File Offset: 0x00103AC0
	public TechTreeData.NodeInstance GetEntryNode()
	{
		if (UnityEngine.Application.isPlaying && this._entryNode != null && this._entryNode.groupName == "Entry")
		{
			return this._entryNode;
		}
		this._entryNode = null;
		foreach (TechTreeData.NodeInstance nodeInstance in this.nodes)
		{
			if (nodeInstance.groupName == "Entry")
			{
				this._entryNode = nodeInstance;
				return nodeInstance;
			}
		}
		Debug.LogError("NO ENTRY NODE FOR TECH TREE, This will Fail hard");
		return null;
	}

	// Token: 0x06002B09 RID: 11017 RVA: 0x0010596C File Offset: 0x00103B6C
	public void ClearInputs(TechTreeData.NodeInstance node)
	{
		foreach (int id in node.outputs)
		{
			TechTreeData.NodeInstance byID = this.GetByID(id);
			byID.inputs.Clear();
			this.ClearInputs(byID);
		}
	}

	// Token: 0x06002B0A RID: 11018 RVA: 0x001059D4 File Offset: 0x00103BD4
	public void SetupInputs(TechTreeData.NodeInstance node)
	{
		foreach (int id in node.outputs)
		{
			TechTreeData.NodeInstance byID = this.GetByID(id);
			if (!byID.inputs.Contains(node.id))
			{
				byID.inputs.Add(node.id);
			}
			this.SetupInputs(byID);
		}
	}

	// Token: 0x06002B0B RID: 11019 RVA: 0x00105A54 File Offset: 0x00103C54
	public bool PlayerHasPathForUnlock(BasePlayer player, TechTreeData.NodeInstance node)
	{
		TechTreeData.NodeInstance entryNode = this.GetEntryNode();
		return entryNode != null && this.CheckChainRecursive(player, entryNode, node);
	}

	// Token: 0x06002B0C RID: 11020 RVA: 0x00105A78 File Offset: 0x00103C78
	public bool CheckChainRecursive(BasePlayer player, TechTreeData.NodeInstance start, TechTreeData.NodeInstance target)
	{
		if (start.groupName != "Entry")
		{
			if (start.IsGroup())
			{
				using (List<int>.Enumerator enumerator = start.inputs.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						int id = enumerator.Current;
						if (!this.PlayerHasPathForUnlock(player, this.GetByID(id)))
						{
							return false;
						}
					}
					goto IL_69;
				}
			}
			if (!this.HasPlayerUnlocked(player, start))
			{
				return false;
			}
		}
		IL_69:
		bool result = false;
		foreach (int num in start.outputs)
		{
			if (num == target.id)
			{
				return true;
			}
			if (this.CheckChainRecursive(player, this.GetByID(num), target))
			{
				result = true;
			}
		}
		return result;
	}

	// Token: 0x06002B0D RID: 11021 RVA: 0x00105B64 File Offset: 0x00103D64
	public bool PlayerCanUnlock(BasePlayer player, TechTreeData.NodeInstance node)
	{
		return this.PlayerHasPathForUnlock(player, node) && !this.HasPlayerUnlocked(player, node);
	}

	// Token: 0x06002B0E RID: 11022 RVA: 0x00105B80 File Offset: 0x00103D80
	public bool HasPlayerUnlocked(BasePlayer player, TechTreeData.NodeInstance node)
	{
		if (node.IsGroup())
		{
			bool result = true;
			foreach (int id in node.outputs)
			{
				TechTreeData.NodeInstance byID = this.GetByID(id);
				if (!this.HasPlayerUnlocked(player, byID))
				{
					result = false;
				}
			}
			return result;
		}
		return player.blueprints.HasUnlocked(node.itemDef);
	}

	// Token: 0x06002B0F RID: 11023 RVA: 0x00105C00 File Offset: 0x00103E00
	public void GetNodesRequiredToUnlock(BasePlayer player, TechTreeData.NodeInstance node, List<TechTreeData.NodeInstance> foundNodes)
	{
		foundNodes.Add(node);
		if (node == this.GetEntryNode())
		{
			return;
		}
		if (node.inputs.Count == 1)
		{
			this.GetNodesRequiredToUnlock(player, this.GetByID(node.inputs[0]), foundNodes);
			return;
		}
		List<TechTreeData.NodeInstance> list = Pool.GetList<TechTreeData.NodeInstance>();
		int num = int.MaxValue;
		foreach (int id in node.inputs)
		{
			List<TechTreeData.NodeInstance> list2 = Pool.GetList<TechTreeData.NodeInstance>();
			this.GetNodesRequiredToUnlock(player, this.GetByID(id), list2);
			int num2 = 0;
			foreach (TechTreeData.NodeInstance nodeInstance in list2)
			{
				if (!(nodeInstance.itemDef == null) && !this.HasPlayerUnlocked(player, nodeInstance))
				{
					num2 += ResearchTable.ScrapForResearch(nodeInstance.itemDef, ResearchTable.ResearchType.TechTree);
				}
			}
			if (num2 < num)
			{
				list.Clear();
				list.AddRange(list2);
				num = num2;
			}
			Pool.FreeList<TechTreeData.NodeInstance>(ref list2);
		}
		foundNodes.AddRange(list);
		Pool.FreeList<TechTreeData.NodeInstance>(ref list);
	}

	// Token: 0x02000D59 RID: 3417
	[Serializable]
	public class NodeInstance
	{
		// Token: 0x04004711 RID: 18193
		public int id;

		// Token: 0x04004712 RID: 18194
		public ItemDefinition itemDef;

		// Token: 0x04004713 RID: 18195
		public Vector2 graphPosition;

		// Token: 0x04004714 RID: 18196
		public List<int> outputs = new List<int>();

		// Token: 0x04004715 RID: 18197
		public List<int> inputs = new List<int>();

		// Token: 0x04004716 RID: 18198
		public string groupName;

		// Token: 0x04004717 RID: 18199
		public int costOverride = -1;

		// Token: 0x060050C5 RID: 20677 RVA: 0x001AA582 File Offset: 0x001A8782
		public bool IsGroup()
		{
			return this.itemDef == null && this.groupName != "Entry" && !string.IsNullOrEmpty(this.groupName);
		}
	}
}
