using System;
using System.IO;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000AEA RID: 2794
	[ConsoleSystem.Factory("world")]
	public class World : ConsoleSystem
	{
		// Token: 0x04003C4B RID: 15435
		[ServerVar]
		[ClientVar]
		public static bool cache = true;

		// Token: 0x04003C4C RID: 15436
		[ClientVar]
		public static bool streaming = true;

		// Token: 0x06004372 RID: 17266 RVA: 0x0018E1C8 File Offset: 0x0018C3C8
		[ClientVar]
		[ServerVar]
		public static void monuments(ConsoleSystem.Arg arg)
		{
			if (!TerrainMeta.Path)
			{
				return;
			}
			TextTable textTable = new TextTable();
			textTable.AddColumn("type");
			textTable.AddColumn("name");
			textTable.AddColumn("prefab");
			textTable.AddColumn("pos");
			foreach (MonumentInfo monumentInfo in TerrainMeta.Path.Monuments)
			{
				textTable.AddRow(new string[]
				{
					monumentInfo.Type.ToString(),
					monumentInfo.displayPhrase.translated,
					monumentInfo.name,
					monumentInfo.transform.position.ToString()
				});
			}
			arg.ReplyWith(textTable.ToString());
		}

		// Token: 0x06004373 RID: 17267 RVA: 0x0018E2B8 File Offset: 0x0018C4B8
		[ServerVar(Clientside = true, Help = "Renders a high resolution PNG of the current map")]
		public static void rendermap(ConsoleSystem.Arg arg)
		{
			float @float = arg.GetFloat(0, 1f);
			int num;
			int num2;
			Color color;
			byte[] array = MapImageRenderer.Render(out num, out num2, out color, @float, false);
			if (array == null)
			{
				arg.ReplyWith("Failed to render the map (is a map loaded now?)");
				return;
			}
			string fullPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, string.Format("map_{0}_{1}.png", World.Size, World.Seed)));
			File.WriteAllBytes(fullPath, array);
			arg.ReplyWith("Saved map render to: " + fullPath);
		}

		// Token: 0x06004374 RID: 17268 RVA: 0x0018E335 File Offset: 0x0018C535
		[ServerVar(Clientside = true, Help = "Renders a PNG of the current map's tunnel network")]
		public static void rendertunnels(ConsoleSystem.Arg arg)
		{
			World.RenderMapLayerToFile(arg, "tunnels", MapLayer.TrainTunnels);
		}

		// Token: 0x06004375 RID: 17269 RVA: 0x0018E344 File Offset: 0x0018C544
		[ServerVar(Clientside = true, Help = "Renders a PNG of the current map's underwater labs, for a specific floor")]
		public static void renderlabs(ConsoleSystem.Arg arg)
		{
			int underwaterLabFloorCount = MapLayerRenderer.GetOrCreate().GetUnderwaterLabFloorCount();
			int @int = arg.GetInt(0, 0);
			if (@int < 0 || @int >= underwaterLabFloorCount)
			{
				arg.ReplyWith(string.Format("Floor number must be between 0 and {0}", underwaterLabFloorCount));
				return;
			}
			World.RenderMapLayerToFile(arg, string.Format("labs_{0}", @int), MapLayer.Underwater1 + @int);
		}

		// Token: 0x06004376 RID: 17270 RVA: 0x0018E3A0 File Offset: 0x0018C5A0
		private static void RenderMapLayerToFile(ConsoleSystem.Arg arg, string name, MapLayer layer)
		{
			try
			{
				MapLayerRenderer orCreate = MapLayerRenderer.GetOrCreate();
				orCreate.Render(layer);
				string fullPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, string.Format("{0}_{1}_{2}.png", name, World.Size, World.Seed)));
				RenderTexture targetTexture = orCreate.renderCamera.targetTexture;
				Texture2D texture2D = new Texture2D(targetTexture.width, targetTexture.height);
				RenderTexture active = RenderTexture.active;
				try
				{
					RenderTexture.active = targetTexture;
					texture2D.ReadPixels(new Rect(0f, 0f, (float)targetTexture.width, (float)targetTexture.height), 0, 0);
					texture2D.Apply();
					File.WriteAllBytes(fullPath, texture2D.EncodeToPNG());
				}
				finally
				{
					RenderTexture.active = active;
					UnityEngine.Object.DestroyImmediate(texture2D);
				}
				arg.ReplyWith("Saved " + name + " render to: " + fullPath);
			}
			catch (Exception message)
			{
				Debug.LogWarning(message);
				arg.ReplyWith("Failed to render " + name);
			}
		}
	}
}
