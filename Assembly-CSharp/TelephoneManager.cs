using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x020003A5 RID: 933
public static class TelephoneManager
{
	// Token: 0x040019C7 RID: 6599
	public const int MaxPhoneNumber = 99990000;

	// Token: 0x040019C8 RID: 6600
	public const int MinPhoneNumber = 10000000;

	// Token: 0x040019C9 RID: 6601
	[ServerVar]
	public static int MaxConcurrentCalls = 10;

	// Token: 0x040019CA RID: 6602
	[ServerVar]
	public static int MaxCallLength = 120;

	// Token: 0x040019CB RID: 6603
	private static Dictionary<int, PhoneController> allTelephones = new Dictionary<int, PhoneController>();

	// Token: 0x040019CC RID: 6604
	private static int maxAssignedPhoneNumber = 99990000;

	// Token: 0x060020D0 RID: 8400 RVA: 0x000D807C File Offset: 0x000D627C
	public static int GetUnusedTelephoneNumber()
	{
		int num = UnityEngine.Random.Range(10000000, 99990000);
		int num2 = 0;
		int num3 = 1000;
		while (TelephoneManager.allTelephones.ContainsKey(num) && num2 < num3)
		{
			num2++;
			num = UnityEngine.Random.Range(10000000, 99990000);
		}
		if (num2 == num3)
		{
			num = TelephoneManager.maxAssignedPhoneNumber + 1;
		}
		TelephoneManager.maxAssignedPhoneNumber = Mathf.Max(TelephoneManager.maxAssignedPhoneNumber, num);
		return num;
	}

	// Token: 0x060020D1 RID: 8401 RVA: 0x000D80E8 File Offset: 0x000D62E8
	public static void RegisterTelephone(PhoneController t, bool checkPhoneNumber = false)
	{
		if (checkPhoneNumber && TelephoneManager.allTelephones.ContainsKey(t.PhoneNumber) && TelephoneManager.allTelephones[t.PhoneNumber] != t)
		{
			t.PhoneNumber = TelephoneManager.GetUnusedTelephoneNumber();
		}
		if (!TelephoneManager.allTelephones.ContainsKey(t.PhoneNumber) && t.PhoneNumber != 0)
		{
			TelephoneManager.allTelephones.Add(t.PhoneNumber, t);
			TelephoneManager.maxAssignedPhoneNumber = Mathf.Max(TelephoneManager.maxAssignedPhoneNumber, t.PhoneNumber);
		}
	}

	// Token: 0x060020D2 RID: 8402 RVA: 0x000D816D File Offset: 0x000D636D
	public static void DeregisterTelephone(PhoneController t)
	{
		if (TelephoneManager.allTelephones.ContainsKey(t.PhoneNumber))
		{
			TelephoneManager.allTelephones.Remove(t.PhoneNumber);
		}
	}

	// Token: 0x060020D3 RID: 8403 RVA: 0x000D8192 File Offset: 0x000D6392
	public static PhoneController GetTelephone(int number)
	{
		if (TelephoneManager.allTelephones.ContainsKey(number))
		{
			return TelephoneManager.allTelephones[number];
		}
		return null;
	}

	// Token: 0x060020D4 RID: 8404 RVA: 0x000D81B0 File Offset: 0x000D63B0
	public static PhoneController GetRandomTelephone(int ignoreNumber)
	{
		foreach (KeyValuePair<int, PhoneController> keyValuePair in TelephoneManager.allTelephones)
		{
			if (keyValuePair.Value.PhoneNumber != ignoreNumber)
			{
				return keyValuePair.Value;
			}
		}
		return null;
	}

	// Token: 0x060020D5 RID: 8405 RVA: 0x000D8218 File Offset: 0x000D6418
	public static int GetCurrentActiveCalls()
	{
		int num = 0;
		foreach (KeyValuePair<int, PhoneController> keyValuePair in TelephoneManager.allTelephones)
		{
			if (keyValuePair.Value.serverState != global::Telephone.CallState.Idle)
			{
				num++;
			}
		}
		if (num == 0)
		{
			return 0;
		}
		return num / 2;
	}

	// Token: 0x060020D6 RID: 8406 RVA: 0x000D8280 File Offset: 0x000D6480
	public static void GetPhoneDirectory(int ignoreNumber, int page, int perPage, PhoneDirectory directory)
	{
		directory.entries = Pool.GetList<PhoneDirectory.DirectoryEntry>();
		int num = page * perPage;
		int num2 = 0;
		foreach (KeyValuePair<int, PhoneController> keyValuePair in TelephoneManager.allTelephones)
		{
			if (keyValuePair.Key != ignoreNumber && !string.IsNullOrEmpty(keyValuePair.Value.PhoneName))
			{
				num2++;
				if (num2 >= num)
				{
					PhoneDirectory.DirectoryEntry directoryEntry = Pool.Get<PhoneDirectory.DirectoryEntry>();
					directoryEntry.phoneName = keyValuePair.Value.GetDirectoryName();
					directoryEntry.phoneNumber = keyValuePair.Value.PhoneNumber;
					directory.entries.Add(directoryEntry);
					if (directory.entries.Count >= perPage)
					{
						directory.atEnd = false;
						return;
					}
				}
			}
		}
		directory.atEnd = true;
	}

	// Token: 0x060020D7 RID: 8407 RVA: 0x000D835C File Offset: 0x000D655C
	[ServerVar]
	public static void PrintAllPhones(ConsoleSystem.Arg arg)
	{
		TextTable textTable = new TextTable();
		textTable.AddColumns(new string[]
		{
			"Number",
			"Name",
			"Position"
		});
		foreach (KeyValuePair<int, PhoneController> keyValuePair in TelephoneManager.allTelephones)
		{
			Vector3 position = keyValuePair.Value.transform.position;
			textTable.AddRow(new string[]
			{
				keyValuePair.Key.ToString(),
				keyValuePair.Value.GetDirectoryName(),
				string.Format("{0} {1} {2}", position.x, position.y, position.z)
			});
		}
		arg.ReplyWith(textTable.ToString());
	}
}
