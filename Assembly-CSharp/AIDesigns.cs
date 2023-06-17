using System;
using System.Collections.Generic;
using System.IO;
using ProtoBuf;
using UnityEngine;

// Token: 0x0200035F RID: 863
public static class AIDesigns
{
	// Token: 0x040018E7 RID: 6375
	public const string DesignFolderPath = "cfg/ai/";

	// Token: 0x040018E8 RID: 6376
	private static Dictionary<string, ProtoBuf.AIDesign> designs = new Dictionary<string, ProtoBuf.AIDesign>();

	// Token: 0x06001F7D RID: 8061 RVA: 0x000D4B2C File Offset: 0x000D2D2C
	public static ProtoBuf.AIDesign GetByNameOrInstance(string designName, ProtoBuf.AIDesign entityDesign)
	{
		if (entityDesign != null)
		{
			return entityDesign;
		}
		ProtoBuf.AIDesign byName = AIDesigns.GetByName(designName + "_custom");
		if (byName != null)
		{
			return byName;
		}
		return AIDesigns.GetByName(designName);
	}

	// Token: 0x06001F7E RID: 8062 RVA: 0x000D4B5A File Offset: 0x000D2D5A
	public static void RefreshCache(string designName, ProtoBuf.AIDesign design)
	{
		if (!AIDesigns.designs.ContainsKey(designName))
		{
			return;
		}
		AIDesigns.designs[designName] = design;
	}

	// Token: 0x06001F7F RID: 8063 RVA: 0x000D4B78 File Offset: 0x000D2D78
	private static ProtoBuf.AIDesign GetByName(string designName)
	{
		ProtoBuf.AIDesign aidesign;
		AIDesigns.designs.TryGetValue(designName, out aidesign);
		if (aidesign != null)
		{
			return aidesign;
		}
		string text = "cfg/ai/" + designName;
		if (!File.Exists(text))
		{
			return null;
		}
		try
		{
			using (FileStream fileStream = File.Open(text, FileMode.Open))
			{
				aidesign = ProtoBuf.AIDesign.Deserialize(fileStream);
				if (aidesign == null)
				{
					return null;
				}
				AIDesigns.designs.Add(designName, aidesign);
			}
		}
		catch (Exception)
		{
			Debug.LogWarning("Error trying to find AI design by name: " + text);
			return null;
		}
		return aidesign;
	}
}
