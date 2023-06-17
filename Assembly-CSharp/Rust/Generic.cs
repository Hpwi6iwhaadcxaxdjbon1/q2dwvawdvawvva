using System;
using UnityEngine.SceneManagement;

namespace Rust
{
	// Token: 0x02000B0B RID: 2827
	public static class Generic
	{
		// Token: 0x04003D23 RID: 15651
		private static Scene _batchingScene;

		// Token: 0x1700063C RID: 1596
		// (get) Token: 0x060044FF RID: 17663 RVA: 0x00194DAE File Offset: 0x00192FAE
		public static Scene BatchingScene
		{
			get
			{
				if (!Generic._batchingScene.IsValid())
				{
					Generic._batchingScene = SceneManager.CreateScene("Batching");
				}
				return Generic._batchingScene;
			}
		}
	}
}
