using System;
using Network;
using TMPro;
using UnityEngine;

// Token: 0x0200030B RID: 779
public class NetworkInfoGeneralText : MonoBehaviour
{
	// Token: 0x040017AC RID: 6060
	public TextMeshProUGUI text;

	// Token: 0x06001E9E RID: 7838 RVA: 0x000D0BB3 File Offset: 0x000CEDB3
	private void Update()
	{
		this.UpdateText();
	}

	// Token: 0x06001E9F RID: 7839 RVA: 0x000D0BBC File Offset: 0x000CEDBC
	private void UpdateText()
	{
		string str = "";
		if (Net.sv != null)
		{
			str += "Server\n";
			str += Net.sv.GetDebug(null);
			str += "\n";
		}
		this.text.text = str;
	}

	// Token: 0x06001EA0 RID: 7840 RVA: 0x000D0C0C File Offset: 0x000CEE0C
	private static string ChannelStat(int window, int left)
	{
		return string.Format("{0}/{1}", left, window);
	}
}
