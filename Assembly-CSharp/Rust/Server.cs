using System;
using UnityEngine.SceneManagement;

namespace Rust
{
	// Token: 0x02000B0C RID: 2828
	public static class Server
	{
		// Token: 0x04003D24 RID: 15652
		public const float UseDistance = 3f;

		// Token: 0x04003D25 RID: 15653
		private static Scene _entityScene;

		// Token: 0x1700063D RID: 1597
		// (get) Token: 0x06004500 RID: 17664 RVA: 0x00194DD0 File Offset: 0x00192FD0
		public static Scene EntityScene
		{
			get
			{
				if (!Server._entityScene.IsValid())
				{
					Server._entityScene = SceneManager.CreateScene("Server Entities");
				}
				return Server._entityScene;
			}
		}
	}
}
