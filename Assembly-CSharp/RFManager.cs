using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020004DF RID: 1247
public class RFManager
{
	// Token: 0x04002090 RID: 8336
	public static Dictionary<int, List<IRFObject>> _listeners = new Dictionary<int, List<IRFObject>>();

	// Token: 0x04002091 RID: 8337
	public static Dictionary<int, List<IRFObject>> _broadcasters = new Dictionary<int, List<IRFObject>>();

	// Token: 0x04002092 RID: 8338
	public static int minFreq = 1;

	// Token: 0x04002093 RID: 8339
	public static int maxFreq = 9999;

	// Token: 0x04002094 RID: 8340
	private static int reserveRangeMin = 4760;

	// Token: 0x04002095 RID: 8341
	private static int reserveRangeMax = 4790;

	// Token: 0x04002096 RID: 8342
	public static string reserveString = string.Concat(new object[]
	{
		"Channels ",
		RFManager.reserveRangeMin,
		" to ",
		RFManager.reserveRangeMax,
		" are restricted."
	});

	// Token: 0x06002859 RID: 10329 RVA: 0x000F94A4 File Offset: 0x000F76A4
	public static int ClampFrequency(int freq)
	{
		return Mathf.Clamp(freq, RFManager.minFreq, RFManager.maxFreq);
	}

	// Token: 0x0600285A RID: 10330 RVA: 0x000F94B8 File Offset: 0x000F76B8
	public static List<IRFObject> GetListenList(int frequency)
	{
		frequency = RFManager.ClampFrequency(frequency);
		List<IRFObject> list = null;
		if (!RFManager._listeners.TryGetValue(frequency, out list))
		{
			list = new List<IRFObject>();
			RFManager._listeners.Add(frequency, list);
		}
		return list;
	}

	// Token: 0x0600285B RID: 10331 RVA: 0x000F94F4 File Offset: 0x000F76F4
	public static List<IRFObject> GetBroadcasterList(int frequency)
	{
		frequency = RFManager.ClampFrequency(frequency);
		List<IRFObject> list = null;
		if (!RFManager._broadcasters.TryGetValue(frequency, out list))
		{
			list = new List<IRFObject>();
			RFManager._broadcasters.Add(frequency, list);
		}
		return list;
	}

	// Token: 0x0600285C RID: 10332 RVA: 0x000F9530 File Offset: 0x000F7730
	public static void AddListener(int frequency, IRFObject obj)
	{
		frequency = RFManager.ClampFrequency(frequency);
		List<IRFObject> listenList = RFManager.GetListenList(frequency);
		if (listenList.Contains(obj))
		{
			Debug.Log("adding same listener twice");
			return;
		}
		listenList.Add(obj);
		RFManager.MarkFrequencyDirty(frequency);
	}

	// Token: 0x0600285D RID: 10333 RVA: 0x000F9570 File Offset: 0x000F7770
	public static void RemoveListener(int frequency, IRFObject obj)
	{
		frequency = RFManager.ClampFrequency(frequency);
		List<IRFObject> listenList = RFManager.GetListenList(frequency);
		if (listenList.Contains(obj))
		{
			listenList.Remove(obj);
		}
		obj.RFSignalUpdate(false);
	}

	// Token: 0x0600285E RID: 10334 RVA: 0x000F95A4 File Offset: 0x000F77A4
	public static void AddBroadcaster(int frequency, IRFObject obj)
	{
		frequency = RFManager.ClampFrequency(frequency);
		List<IRFObject> broadcasterList = RFManager.GetBroadcasterList(frequency);
		if (broadcasterList.Contains(obj))
		{
			return;
		}
		broadcasterList.Add(obj);
		RFManager.MarkFrequencyDirty(frequency);
	}

	// Token: 0x0600285F RID: 10335 RVA: 0x000F95D8 File Offset: 0x000F77D8
	public static void RemoveBroadcaster(int frequency, IRFObject obj)
	{
		frequency = RFManager.ClampFrequency(frequency);
		List<IRFObject> broadcasterList = RFManager.GetBroadcasterList(frequency);
		if (broadcasterList.Contains(obj))
		{
			broadcasterList.Remove(obj);
		}
		RFManager.MarkFrequencyDirty(frequency);
	}

	// Token: 0x06002860 RID: 10336 RVA: 0x000F960B File Offset: 0x000F780B
	public static bool IsReserved(int frequency)
	{
		return frequency >= RFManager.reserveRangeMin && frequency <= RFManager.reserveRangeMax;
	}

	// Token: 0x06002861 RID: 10337 RVA: 0x000F9620 File Offset: 0x000F7820
	public static void ReserveErrorPrint(BasePlayer player)
	{
		player.ChatMessage(RFManager.reserveString);
	}

	// Token: 0x06002862 RID: 10338 RVA: 0x000F962D File Offset: 0x000F782D
	public static void ChangeFrequency(int oldFrequency, int newFrequency, IRFObject obj, bool isListener, bool isOn = true)
	{
		newFrequency = RFManager.ClampFrequency(newFrequency);
		if (isListener)
		{
			RFManager.RemoveListener(oldFrequency, obj);
			if (isOn)
			{
				RFManager.AddListener(newFrequency, obj);
				return;
			}
		}
		else
		{
			RFManager.RemoveBroadcaster(oldFrequency, obj);
			if (isOn)
			{
				RFManager.AddBroadcaster(newFrequency, obj);
			}
		}
	}

	// Token: 0x06002863 RID: 10339 RVA: 0x000F9660 File Offset: 0x000F7860
	public static void MarkFrequencyDirty(int frequency)
	{
		frequency = RFManager.ClampFrequency(frequency);
		List<IRFObject> broadcasterList = RFManager.GetBroadcasterList(frequency);
		List<IRFObject> listenList = RFManager.GetListenList(frequency);
		bool flag = broadcasterList.Count > 0;
		bool flag2 = false;
		bool flag3 = false;
		for (int i = listenList.Count - 1; i >= 0; i--)
		{
			IRFObject irfobject = listenList[i];
			if (!irfobject.IsValidEntityReference<IRFObject>())
			{
				flag2 = true;
			}
			else
			{
				if (flag)
				{
					flag = false;
					foreach (IRFObject irfobject2 in broadcasterList)
					{
						if (!irfobject2.IsValidEntityReference<IRFObject>())
						{
							flag3 = true;
						}
						else if (Vector3.Distance(irfobject2.GetPosition(), irfobject.GetPosition()) <= irfobject2.GetMaxRange())
						{
							flag = true;
							break;
						}
					}
				}
				irfobject.RFSignalUpdate(flag);
			}
		}
		if (flag2)
		{
			Debug.LogWarning("Found null entries in the RF listener list for frequency " + frequency + "... cleaning up.");
			for (int j = listenList.Count - 1; j >= 0; j--)
			{
				if (listenList[j] == null)
				{
					listenList.RemoveAt(j);
				}
			}
		}
		if (flag3)
		{
			Debug.LogWarning("Found null entries in the RF broadcaster list for frequency " + frequency + "... cleaning up.");
			for (int k = broadcasterList.Count - 1; k >= 0; k--)
			{
				if (broadcasterList[k] == null)
				{
					broadcasterList.RemoveAt(k);
				}
			}
		}
	}
}
