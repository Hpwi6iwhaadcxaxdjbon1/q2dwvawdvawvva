using System;
using UnityEngine;

namespace Painting
{
	// Token: 0x020009DB RID: 2523
	[Serializable]
	public class Brush
	{
		// Token: 0x04003681 RID: 13953
		public float spacing;

		// Token: 0x04003682 RID: 13954
		public Vector2 brushSize;

		// Token: 0x04003683 RID: 13955
		public Texture2D texture;

		// Token: 0x04003684 RID: 13956
		public Color color;

		// Token: 0x04003685 RID: 13957
		public bool erase;
	}
}
