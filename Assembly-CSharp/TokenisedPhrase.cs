using System;
using Facepunch;
using UnityEngine;

// Token: 0x020002EB RID: 747
[Serializable]
public class TokenisedPhrase : Translate.Phrase
{
	// Token: 0x17000277 RID: 631
	// (get) Token: 0x06001E01 RID: 7681 RVA: 0x000CCBC7 File Offset: 0x000CADC7
	public override string translated
	{
		get
		{
			return TokenisedPhrase.ReplaceTokens(base.translated);
		}
	}

	// Token: 0x06001E02 RID: 7682 RVA: 0x000CCBD4 File Offset: 0x000CADD4
	public static string ReplaceTokens(string str)
	{
		if (!str.Contains("["))
		{
			return str;
		}
		str = str.Replace("[inventory.toggle]", string.Format("[{0}]", Facepunch.Input.GetButtonWithBind("inventory.toggle").ToUpper()));
		str = str.Replace("[inventory.togglecrafting]", string.Format("[{0}]", Facepunch.Input.GetButtonWithBind("inventory.togglecrafting").ToUpper()));
		str = str.Replace("[+map]", string.Format("[{0}]", Facepunch.Input.GetButtonWithBind("+map").ToUpper()));
		str = str.Replace("[inventory.examineheld]", string.Format("[{0}]", Facepunch.Input.GetButtonWithBind("inventory.examineheld").ToUpper()));
		str = str.Replace("[slot2]", string.Format("[{0}]", Facepunch.Input.GetButtonWithBind("+slot2").ToUpper()));
		str = str.Replace("[attack]", string.Format("[{0}]", TokenisedPhrase.TranslateMouseButton(Facepunch.Input.GetButtonWithBind("+attack")).ToUpper()));
		str = str.Replace("[attack2]", string.Format("[{0}]", TokenisedPhrase.TranslateMouseButton(Facepunch.Input.GetButtonWithBind("+attack2")).ToUpper()));
		str = str.Replace("[+use]", string.Format("[{0}]", TokenisedPhrase.TranslateMouseButton(Facepunch.Input.GetButtonWithBind("+use")).ToUpper()));
		str = str.Replace("[+altlook]", string.Format("[{0}]", TokenisedPhrase.TranslateMouseButton(Facepunch.Input.GetButtonWithBind("+altlook")).ToUpper()));
		str = str.Replace("[+reload]", string.Format("[{0}]", TokenisedPhrase.TranslateMouseButton(Facepunch.Input.GetButtonWithBind("+reload")).ToUpper()));
		str = str.Replace("[+voice]", string.Format("[{0}]", TokenisedPhrase.TranslateMouseButton(Facepunch.Input.GetButtonWithBind("+voice")).ToUpper()));
		str = str.Replace("[+lockBreakHealthPercent]", string.Format("{0:0%}", 0.2f));
		str = str.Replace("[+gestures]", string.Format("[{0}]", TokenisedPhrase.TranslateMouseButton(Facepunch.Input.GetButtonWithBind("+gestures")).ToUpper()));
		str = str.Replace("[+left]", string.Format("[{0}]", TokenisedPhrase.TranslateMouseButton(Facepunch.Input.GetButtonWithBind("+left")).ToUpper()));
		str = str.Replace("[+right]", string.Format("[{0}]", TokenisedPhrase.TranslateMouseButton(Facepunch.Input.GetButtonWithBind("+right")).ToUpper()));
		str = str.Replace("[+backward]", string.Format("[{0}]", TokenisedPhrase.TranslateMouseButton(Facepunch.Input.GetButtonWithBind("+backward")).ToUpper()));
		str = str.Replace("[+forward]", string.Format("[{0}]", TokenisedPhrase.TranslateMouseButton(Facepunch.Input.GetButtonWithBind("+forward")).ToUpper()));
		str = str.Replace("[+sprint]", string.Format("[{0}]", Facepunch.Input.GetButtonWithBind("+sprint")).ToUpper());
		str = str.Replace("[+duck]", string.Format("[{0}]", Facepunch.Input.GetButtonWithBind("+duck")).ToUpper());
		str = str.Replace("[+pets]", string.Format("[{0}]", Facepunch.Input.GetButtonWithBind("+pets")).ToUpper());
		str = str.Replace("[lighttoggle]", string.Format("[{0}]", Facepunch.Input.GetButtonWithBind("lighttoggle")).ToUpper());
		str = str.Replace("[+ping]", string.Format("[{0}]", Facepunch.Input.GetButtonWithBind("+ping")).ToUpper());
		return str;
	}

	// Token: 0x06001E03 RID: 7683 RVA: 0x000CCF67 File Offset: 0x000CB167
	public TokenisedPhrase(string t = "", string eng = "") : base(t, eng)
	{
	}

	// Token: 0x06001E04 RID: 7684 RVA: 0x000CCF71 File Offset: 0x000CB171
	public static string TranslateMouseButton(string mouseButton)
	{
		if (mouseButton == "mouse0")
		{
			return "Left Mouse";
		}
		if (mouseButton == "mouse1")
		{
			return "Right Mouse";
		}
		if (mouseButton == "mouse2")
		{
			return "Center Mouse";
		}
		return mouseButton;
	}

	// Token: 0x06001E05 RID: 7685 RVA: 0x000CCFB0 File Offset: 0x000CB1B0
	private static string GetButtonWithBind(string s)
	{
		if (!UnityEngine.Application.isPlaying)
		{
			uint num = <PrivateImplementationDetails>.ComputeStringHash(s);
			if (num <= 2356994644U)
			{
				if (num <= 929483559U)
				{
					if (num != 853535541U)
					{
						if (num == 929483559U)
						{
							if (s == "inventory.toggle")
							{
								return "tab";
							}
						}
					}
					else if (s == "inventory.togglecrafting")
					{
						return "q";
					}
				}
				else if (num != 1163060404U)
				{
					if (num != 2051247900U)
					{
						if (num == 2356994644U)
						{
							if (s == "+map")
							{
								return "g";
							}
						}
					}
					else if (s == "+attack2")
					{
						return "mouse1";
					}
				}
				else if (s == "+slot2")
				{
					return "2";
				}
			}
			else if (num <= 3646378636U)
			{
				if (num != 3369192399U)
				{
					if (num != 3565620013U)
					{
						if (num == 3646378636U)
						{
							if (s == "+voice")
							{
								return "v";
							}
						}
					}
					else if (s == "+use")
					{
						return "e";
					}
				}
				else if (s == "+reload")
				{
					return "r";
				}
			}
			else if (num != 3724475309U)
			{
				if (num != 4160601298U)
				{
					if (num == 4193981894U)
					{
						if (s == "+attack")
						{
							return "mouse0";
						}
					}
				}
				else if (s == "+altlook")
				{
					return "leftalt";
				}
			}
			else if (s == "inventory.examineheld")
			{
				return "n";
			}
		}
		return Facepunch.Input.GetButtonWithBind(s);
	}
}
