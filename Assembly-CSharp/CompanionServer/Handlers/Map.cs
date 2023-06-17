using System;
using Facepunch;
using ProtoBuf;
using UnityEngine;

namespace CompanionServer.Handlers
{
	// Token: 0x020009FD RID: 2557
	public class Map : BaseHandler<AppEmpty>
	{
		// Token: 0x040036EE RID: 14062
		private static int _width;

		// Token: 0x040036EF RID: 14063
		private static int _height;

		// Token: 0x040036F0 RID: 14064
		private static byte[] _imageData;

		// Token: 0x040036F1 RID: 14065
		private static string _background;

		// Token: 0x17000507 RID: 1287
		// (get) Token: 0x06003D1E RID: 15646 RVA: 0x00166C80 File Offset: 0x00164E80
		protected override double TokenCost
		{
			get
			{
				return 5.0;
			}
		}

		// Token: 0x06003D1F RID: 15647 RVA: 0x00166C8C File Offset: 0x00164E8C
		public override void Execute()
		{
			if (Map._imageData == null)
			{
				base.SendError("no_map");
				return;
			}
			AppMap appMap = Pool.Get<AppMap>();
			appMap.width = (uint)Map._width;
			appMap.height = (uint)Map._height;
			appMap.oceanMargin = 500;
			appMap.jpgImage = Map._imageData;
			appMap.background = Map._background;
			appMap.monuments = Pool.GetList<AppMap.Monument>();
			if (TerrainMeta.Path != null && TerrainMeta.Path.Landmarks != null)
			{
				foreach (LandmarkInfo landmarkInfo in TerrainMeta.Path.Landmarks)
				{
					if (landmarkInfo.shouldDisplayOnMap)
					{
						Vector2 vector = Util.WorldToMap(landmarkInfo.transform.position);
						AppMap.Monument monument = Pool.Get<AppMap.Monument>();
						monument.token = (landmarkInfo.displayPhrase.IsValid() ? landmarkInfo.displayPhrase.token : landmarkInfo.transform.root.name);
						monument.x = vector.x;
						monument.y = vector.y;
						appMap.monuments.Add(monument);
					}
				}
			}
			AppResponse appResponse = Pool.Get<AppResponse>();
			appResponse.map = appMap;
			base.Send(appResponse);
		}

		// Token: 0x06003D20 RID: 15648 RVA: 0x00166DEC File Offset: 0x00164FEC
		public static void PopulateCache()
		{
			Map.RenderToCache();
		}

		// Token: 0x06003D21 RID: 15649 RVA: 0x00166DF4 File Offset: 0x00164FF4
		private static void RenderToCache()
		{
			Map._imageData = null;
			Map._width = 0;
			Map._height = 0;
			try
			{
				Color color;
				Map._imageData = MapImageRenderer.Render(out Map._width, out Map._height, out color, 0.5f, true);
				Map._background = "#" + ColorUtility.ToHtmlStringRGB(color);
			}
			catch (Exception arg)
			{
				Debug.LogError(string.Format("Exception thrown when rendering map for the app: {0}", arg));
			}
			if (Map._imageData == null)
			{
				Debug.LogError("Map image is null! App users will not be able to see the map.");
			}
		}
	}
}
