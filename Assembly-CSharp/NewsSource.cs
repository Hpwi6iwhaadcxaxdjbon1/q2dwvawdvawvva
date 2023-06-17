using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Facepunch;
using Facepunch.Extend;
using Facepunch.Math;
using Facepunch.Models;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200087C RID: 2172
public class NewsSource : MonoBehaviour
{
	// Token: 0x040030E1 RID: 12513
	private static readonly Regex BbcodeParse = new Regex("([^\\[]*)(?:\\[(\\w+)(?:=([^\\]]+))?\\](.*?)\\[\\/\\2\\])?", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);

	// Token: 0x040030E2 RID: 12514
	public RustText title;

	// Token: 0x040030E3 RID: 12515
	public RustText date;

	// Token: 0x040030E4 RID: 12516
	public RustText authorName;

	// Token: 0x040030E5 RID: 12517
	public HttpImage coverImage;

	// Token: 0x040030E6 RID: 12518
	public RectTransform container;

	// Token: 0x040030E7 RID: 12519
	public Button button;

	// Token: 0x040030E8 RID: 12520
	public RustText paragraphTemplate;

	// Token: 0x040030E9 RID: 12521
	public HttpImage imageTemplate;

	// Token: 0x040030EA RID: 12522
	public HttpImage youtubeTemplate;

	// Token: 0x040030EB RID: 12523
	private static readonly string[] BulletSeparators = new string[]
	{
		"[*]"
	};

	// Token: 0x0600367A RID: 13946 RVA: 0x0014999C File Offset: 0x00147B9C
	public void Awake()
	{
		GA.DesignEvent("news:view");
	}

	// Token: 0x0600367B RID: 13947 RVA: 0x001499A8 File Offset: 0x00147BA8
	public void OnEnable()
	{
		if (SteamNewsSource.Stories == null || SteamNewsSource.Stories.Length == 0)
		{
			return;
		}
		this.SetStory(SteamNewsSource.Stories[0]);
	}

	// Token: 0x0600367C RID: 13948 RVA: 0x001499CC File Offset: 0x00147BCC
	public void SetStory(SteamNewsSource.Story story)
	{
		NewsSource.<>c__DisplayClass12_0 CS$<>8__locals1 = new NewsSource.<>c__DisplayClass12_0();
		CS$<>8__locals1.story = story;
		PlayerPrefs.SetInt("lastNewsDate", CS$<>8__locals1.story.date);
		this.container.DestroyAllChildren(false);
		this.title.text = CS$<>8__locals1.story.name;
		this.authorName.text = "by " + CS$<>8__locals1.story.author;
		string str = ((long)(Epoch.Current - CS$<>8__locals1.story.date)).FormatSecondsLong();
		this.date.text = "Posted " + str + " ago";
		this.button.onClick.RemoveAllListeners();
		this.button.onClick.AddListener(delegate()
		{
			Facepunch.Models.Manifest.NewsInfo.BlogInfo blogInfo2 = base.<SetStory>g__GetBlogPost|1();
			string text2 = ((blogInfo2 != null) ? blogInfo2.Url : null) ?? CS$<>8__locals1.story.url;
			Debug.Log("Opening URL: " + text2);
			UnityEngine.Application.OpenURL(text2);
		});
		Facepunch.Models.Manifest.NewsInfo.BlogInfo blogInfo = CS$<>8__locals1.<SetStory>g__GetBlogPost|1();
		string text = (blogInfo != null) ? blogInfo.HeaderImage : null;
		NewsSource.ParagraphBuilder paragraphBuilder = NewsSource.ParagraphBuilder.New();
		this.ParseBbcode(ref paragraphBuilder, CS$<>8__locals1.story.text, ref text, 0);
		this.AppendParagraph(ref paragraphBuilder);
		if (text != null)
		{
			this.coverImage.Load(text);
		}
		RustText[] componentsInChildren = this.container.GetComponentsInChildren<RustText>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].DoAutoSize();
		}
	}

	// Token: 0x0600367D RID: 13949 RVA: 0x00149B10 File Offset: 0x00147D10
	private void ParseBbcode(ref NewsSource.ParagraphBuilder currentParagraph, string bbcode, ref string firstImage, int depth = 0)
	{
		foreach (object obj in NewsSource.BbcodeParse.Matches(bbcode))
		{
			Match match = (Match)obj;
			string value = match.Groups[1].Value;
			string value2 = match.Groups[2].Value;
			string value3 = match.Groups[3].Value;
			string value4 = match.Groups[4].Value;
			currentParagraph.Append(value);
			string text = value2.ToLowerInvariant();
			uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
			if (num <= 2369466585U)
			{
				if (num <= 706259085U)
				{
					if (num != 217798785U)
					{
						if (num != 632598351U)
						{
							if (num == 706259085U)
							{
								if (text == "noparse")
								{
									currentParagraph.Append(value4);
								}
							}
						}
						else if (text == "strike")
						{
							currentParagraph.Append("<s>");
							this.ParseBbcode(ref currentParagraph, value4, ref firstImage, depth + 1);
							currentParagraph.Append("</s>");
						}
					}
					else if (text == "list")
					{
						currentParagraph.AppendLine();
						foreach (string text2 in NewsSource.GetBulletPoints(value4))
						{
							if (!string.IsNullOrWhiteSpace(text2))
							{
								currentParagraph.Append("\t• ");
								currentParagraph.Append(text2.Trim());
								currentParagraph.AppendLine();
							}
						}
					}
				}
				else if (num <= 1624406948U)
				{
					if (num != 848251934U)
					{
						if (num == 1624406948U)
						{
							if (text == "previewyoutube")
							{
								if (depth == 0)
								{
									string[] array2 = value3.Split(new char[]
									{
										';'
									});
									this.AppendYouTube(ref currentParagraph, array2[0]);
								}
							}
						}
					}
					else if (text == "url")
					{
						if (value4.Contains("[img]", StringComparison.InvariantCultureIgnoreCase))
						{
							this.ParseBbcode(ref currentParagraph, value4, ref firstImage, depth);
						}
						else
						{
							int count = currentParagraph.Links.Count;
							currentParagraph.Links.Add(value3);
							currentParagraph.Append(string.Format("<link={0}><u>", count));
							this.ParseBbcode(ref currentParagraph, value4, ref firstImage, depth + 1);
							currentParagraph.Append("</u></link>");
						}
					}
				}
				else if (num != 2229740804U)
				{
					if (num == 2369466585U)
					{
						if (text == "h4")
						{
							currentParagraph.Append("<size=150%>");
							this.ParseBbcode(ref currentParagraph, value4, ref firstImage, depth + 1);
							currentParagraph.Append("</size>");
						}
					}
				}
				else if (text == "img")
				{
					if (depth == 0)
					{
						string text3 = value4.Trim();
						if (firstImage == null)
						{
							firstImage = text3;
						}
						this.AppendImage(ref currentParagraph, text3);
					}
				}
			}
			else if (num <= 2419799442U)
			{
				if (num != 2386244204U)
				{
					if (num != 2403021823U)
					{
						if (num != 2419799442U)
						{
							continue;
						}
						if (!(text == "h1"))
						{
							continue;
						}
					}
					else if (!(text == "h2"))
					{
						continue;
					}
					currentParagraph.Append("<size=200%>");
					this.ParseBbcode(ref currentParagraph, value4, ref firstImage, depth + 1);
					currentParagraph.Append("</size>");
				}
				else if (text == "h3")
				{
					currentParagraph.Append("<size=175%>");
					this.ParseBbcode(ref currentParagraph, value4, ref firstImage, depth + 1);
					currentParagraph.Append("</size>");
				}
			}
			else if (num <= 3876335077U)
			{
				if (num != 2791659946U)
				{
					if (num == 3876335077U)
					{
						if (text == "b")
						{
							currentParagraph.Append("<b>");
							this.ParseBbcode(ref currentParagraph, value4, ref firstImage, depth + 1);
							currentParagraph.Append("</b>");
						}
					}
				}
				else if (text == "olist")
				{
					currentParagraph.AppendLine();
					string[] bulletPoints = NewsSource.GetBulletPoints(value4);
					int num2 = 1;
					foreach (string text4 in bulletPoints)
					{
						if (!string.IsNullOrWhiteSpace(text4))
						{
							currentParagraph.Append(string.Format("\t{0} ", num2++));
							currentParagraph.Append(text4.Trim());
							currentParagraph.AppendLine();
						}
					}
				}
			}
			else if (num != 3960223172U)
			{
				if (num == 4027333648U)
				{
					if (text == "u")
					{
						currentParagraph.Append("<u>");
						this.ParseBbcode(ref currentParagraph, value4, ref firstImage, depth + 1);
						currentParagraph.Append("</u>");
					}
				}
			}
			else if (text == "i")
			{
				currentParagraph.Append("<i>");
				this.ParseBbcode(ref currentParagraph, value4, ref firstImage, depth + 1);
				currentParagraph.Append("</i>");
			}
		}
	}

	// Token: 0x0600367E RID: 13950 RVA: 0x0014A09C File Offset: 0x0014829C
	private static string[] GetBulletPoints(string listContent)
	{
		return ((listContent != null) ? listContent.Split(NewsSource.BulletSeparators, StringSplitOptions.RemoveEmptyEntries) : null) ?? Array.Empty<string>();
	}

	// Token: 0x0600367F RID: 13951 RVA: 0x0014A0BC File Offset: 0x001482BC
	private void AppendParagraph(ref NewsSource.ParagraphBuilder currentParagraph)
	{
		if (currentParagraph.StringBuilder.Length > 0)
		{
			string text = currentParagraph.StringBuilder.ToString();
			RustText rustText = UnityEngine.Object.Instantiate<RustText>(this.paragraphTemplate, this.container);
			rustText.SetActive(true);
			rustText.SetText(text);
			NewsParagraph newsParagraph;
			if (rustText.TryGetComponent<NewsParagraph>(out newsParagraph))
			{
				newsParagraph.Links = currentParagraph.Links;
			}
		}
		currentParagraph = NewsSource.ParagraphBuilder.New();
	}

	// Token: 0x06003680 RID: 13952 RVA: 0x0014A122 File Offset: 0x00148322
	private void AppendImage(ref NewsSource.ParagraphBuilder currentParagraph, string url)
	{
		this.AppendParagraph(ref currentParagraph);
		HttpImage httpImage = UnityEngine.Object.Instantiate<HttpImage>(this.imageTemplate, this.container);
		httpImage.SetActive(true);
		httpImage.Load(url);
	}

	// Token: 0x06003681 RID: 13953 RVA: 0x0014A14C File Offset: 0x0014834C
	private void AppendYouTube(ref NewsSource.ParagraphBuilder currentParagraph, string videoId)
	{
		this.AppendParagraph(ref currentParagraph);
		HttpImage httpImage = UnityEngine.Object.Instantiate<HttpImage>(this.youtubeTemplate, this.container);
		httpImage.SetActive(true);
		httpImage.Load("https://img.youtube.com/vi/" + videoId + "/maxresdefault.jpg");
		RustButton component = httpImage.GetComponent<RustButton>();
		if (component != null)
		{
			string videoUrl = "https://www.youtube.com/watch?v=" + videoId;
			component.OnReleased.AddListener(delegate()
			{
				Debug.Log("Opening URL: " + videoUrl);
				UnityEngine.Application.OpenURL(videoUrl);
			});
		}
	}

	// Token: 0x02000E92 RID: 3730
	private struct ParagraphBuilder
	{
		// Token: 0x04004C30 RID: 19504
		public StringBuilder StringBuilder;

		// Token: 0x04004C31 RID: 19505
		public List<string> Links;

		// Token: 0x060052DB RID: 21211 RVA: 0x001B1170 File Offset: 0x001AF370
		public static NewsSource.ParagraphBuilder New()
		{
			return new NewsSource.ParagraphBuilder
			{
				StringBuilder = new StringBuilder(),
				Links = new List<string>()
			};
		}

		// Token: 0x060052DC RID: 21212 RVA: 0x001B119E File Offset: 0x001AF39E
		public void AppendLine()
		{
			this.StringBuilder.AppendLine();
		}

		// Token: 0x060052DD RID: 21213 RVA: 0x001B11AC File Offset: 0x001AF3AC
		public void Append(string text)
		{
			this.StringBuilder.Append(text);
		}
	}
}
