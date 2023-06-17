using System;
using UnityEngine;

// Token: 0x0200052F RID: 1327
public class CopyLODValues : MonoBehaviour, IEditorComponent
{
	// Token: 0x040021E5 RID: 8677
	[SerializeField]
	private LODGroup source;

	// Token: 0x040021E6 RID: 8678
	[SerializeField]
	private LODGroup destination;

	// Token: 0x040021E7 RID: 8679
	[Tooltip("Is false, exact values are copied. If true, values are scaled based on LODGroup size, so the changeover point will match.")]
	[SerializeField]
	private bool scale = true;

	// Token: 0x060029C3 RID: 10691 RVA: 0x000FF9E7 File Offset: 0x000FDBE7
	public bool CanCopy()
	{
		return this.source != null && this.destination != null;
	}

	// Token: 0x060029C4 RID: 10692 RVA: 0x000FFA08 File Offset: 0x000FDC08
	public void Copy()
	{
		if (!this.CanCopy())
		{
			return;
		}
		LOD[] lods = this.source.GetLODs();
		if (this.scale)
		{
			float num = this.destination.size / this.source.size;
			for (int i = 0; i < lods.Length; i++)
			{
				LOD[] array = lods;
				int num2 = i;
				array[num2].screenRelativeTransitionHeight = array[num2].screenRelativeTransitionHeight * num;
			}
		}
		LOD[] lods2 = this.destination.GetLODs();
		int num3 = 0;
		while (num3 < lods2.Length && num3 < lods.Length)
		{
			int num4 = (num3 == lods2.Length - 1) ? (lods.Length - 1) : num3;
			lods2[num3].screenRelativeTransitionHeight = lods[num4].screenRelativeTransitionHeight;
			Debug.Log(string.Format("Set destination LOD {0} to {1}", num3, lods2[num3].screenRelativeTransitionHeight));
			num3++;
		}
		this.destination.SetLODs(lods2);
	}
}
