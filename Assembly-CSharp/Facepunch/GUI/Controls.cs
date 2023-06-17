using System;
using UnityEngine;

namespace Facepunch.GUI
{
	// Token: 0x02000AFC RID: 2812
	public static class Controls
	{
		// Token: 0x04003CD1 RID: 15569
		public static float labelWidth = 100f;

		// Token: 0x060044A5 RID: 17573 RVA: 0x00193184 File Offset: 0x00191384
		public static float FloatSlider(string strLabel, float value, float low, float high, string format = "0.00")
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label(strLabel, new GUILayoutOption[]
			{
				GUILayout.Width(Controls.labelWidth)
			});
			float value2 = float.Parse(GUILayout.TextField(value.ToString(format), new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true)
			}));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			float result = GUILayout.HorizontalSlider(value2, low, high, Array.Empty<GUILayoutOption>());
			GUILayout.EndHorizontal();
			return result;
		}

		// Token: 0x060044A6 RID: 17574 RVA: 0x001931F8 File Offset: 0x001913F8
		public static int IntSlider(string strLabel, int value, int low, int high, string format = "0")
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label(strLabel, new GUILayoutOption[]
			{
				GUILayout.Width(Controls.labelWidth)
			});
			float num = (float)int.Parse(GUILayout.TextField(value.ToString(format), new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true)
			}));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			int result = (int)GUILayout.HorizontalSlider(num, (float)low, (float)high, Array.Empty<GUILayoutOption>());
			GUILayout.EndHorizontal();
			return result;
		}

		// Token: 0x060044A7 RID: 17575 RVA: 0x0019326E File Offset: 0x0019146E
		public static string TextArea(string strName, string value)
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label(strName, new GUILayoutOption[]
			{
				GUILayout.Width(Controls.labelWidth)
			});
			string result = GUILayout.TextArea(value, Array.Empty<GUILayoutOption>());
			GUILayout.EndHorizontal();
			return result;
		}

		// Token: 0x060044A8 RID: 17576 RVA: 0x001932A3 File Offset: 0x001914A3
		public static bool Checkbox(string strName, bool value)
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label(strName, new GUILayoutOption[]
			{
				GUILayout.Width(Controls.labelWidth)
			});
			bool result = GUILayout.Toggle(value, "", Array.Empty<GUILayoutOption>());
			GUILayout.EndHorizontal();
			return result;
		}

		// Token: 0x060044A9 RID: 17577 RVA: 0x001932DD File Offset: 0x001914DD
		public static bool Button(string strName)
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			bool result = GUILayout.Button(strName, Array.Empty<GUILayoutOption>());
			GUILayout.EndHorizontal();
			return result;
		}
	}
}
