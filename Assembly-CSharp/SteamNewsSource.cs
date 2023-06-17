using System;
using System.Collections;
using System.Collections.Generic;
using JSON;
using UnityEngine;

// Token: 0x0200087D RID: 2173
public static class SteamNewsSource
{
	// Token: 0x040030EC RID: 12524
	public static SteamNewsSource.Story[] Stories;

	// Token: 0x06003684 RID: 13956 RVA: 0x0014A1F2 File Offset: 0x001483F2
	public static IEnumerator GetStories()
	{
		WWW www = new WWW("http://api.steampowered.com/ISteamNews/GetNewsForApp/v0002/?appid=252490&count=8&format=json&feeds=steam_community_announcements");
		yield return www;
		JSON.Object @object = JSON.Object.Parse(www.text);
		www.Dispose();
		if (@object == null)
		{
			yield break;
		}
		JSON.Array array = @object.GetObject("appnews").GetArray("newsitems");
		List<SteamNewsSource.Story> list = new List<SteamNewsSource.Story>();
		foreach (Value value in array)
		{
			string text = value.Obj.GetString("contents", "Missing Contents");
			text = text.Replace("\\n", "\n").Replace("\\r", "").Replace("\\\"", "\"");
			list.Add(new SteamNewsSource.Story
			{
				name = value.Obj.GetString("title", "Missing Title"),
				url = value.Obj.GetString("url", "Missing URL"),
				date = value.Obj.GetInt("date", 0),
				text = text,
				author = value.Obj.GetString("author", "Missing Author")
			});
		}
		SteamNewsSource.Stories = list.ToArray();
		yield break;
	}

	// Token: 0x02000E96 RID: 3734
	public struct Story
	{
		// Token: 0x04004C36 RID: 19510
		public string name;

		// Token: 0x04004C37 RID: 19511
		public string url;

		// Token: 0x04004C38 RID: 19512
		public int date;

		// Token: 0x04004C39 RID: 19513
		public string text;

		// Token: 0x04004C3A RID: 19514
		public string author;
	}
}
