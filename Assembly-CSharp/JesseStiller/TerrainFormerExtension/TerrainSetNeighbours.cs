using System;
using UnityEngine;

namespace JesseStiller.TerrainFormerExtension
{
	// Token: 0x020009DA RID: 2522
	public class TerrainSetNeighbours : MonoBehaviour
	{
		// Token: 0x0400367D RID: 13949
		[SerializeField]
		private Terrain leftTerrain;

		// Token: 0x0400367E RID: 13950
		[SerializeField]
		private Terrain topTerrain;

		// Token: 0x0400367F RID: 13951
		[SerializeField]
		private Terrain rightTerrain;

		// Token: 0x04003680 RID: 13952
		[SerializeField]
		private Terrain bottomTerrain;

		// Token: 0x06003C4A RID: 15434 RVA: 0x00163196 File Offset: 0x00161396
		private void Awake()
		{
			base.GetComponent<Terrain>().SetNeighbors(this.leftTerrain, this.topTerrain, this.rightTerrain, this.bottomTerrain);
			UnityEngine.Object.Destroy(this);
		}

		// Token: 0x06003C4B RID: 15435 RVA: 0x001631C1 File Offset: 0x001613C1
		public void SetNeighbours(Terrain leftTerrain, Terrain topTerrain, Terrain rightTerrain, Terrain bottomTerrain)
		{
			this.leftTerrain = leftTerrain;
			this.topTerrain = topTerrain;
			this.rightTerrain = rightTerrain;
			this.bottomTerrain = bottomTerrain;
		}
	}
}
