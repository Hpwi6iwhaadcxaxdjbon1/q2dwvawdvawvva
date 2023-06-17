using System;
using System.Collections;
using System.IO;
using System.Linq;
using Rust;
using UnityEngine;
using UnityEngine.Video;

// Token: 0x02000879 RID: 2169
public class MenuBackgroundVideo : SingletonComponent<MenuBackgroundVideo>
{
	// Token: 0x040030DC RID: 12508
	private string[] videos;

	// Token: 0x040030DD RID: 12509
	private int index;

	// Token: 0x040030DE RID: 12510
	private bool errored;

	// Token: 0x0600366E RID: 13934 RVA: 0x00149753 File Offset: 0x00147953
	protected override void Awake()
	{
		base.Awake();
		this.LoadVideoList();
		this.NextVideo();
		base.GetComponent<VideoPlayer>().errorReceived += this.OnVideoError;
	}

	// Token: 0x0600366F RID: 13935 RVA: 0x0014977E File Offset: 0x0014797E
	private void OnVideoError(VideoPlayer source, string message)
	{
		this.errored = true;
	}

	// Token: 0x06003670 RID: 13936 RVA: 0x00149788 File Offset: 0x00147988
	public void LoadVideoList()
	{
		this.videos = (from x in Directory.EnumerateFiles(UnityEngine.Application.streamingAssetsPath + "/MenuVideo/")
		where x.EndsWith(".mp4") || x.EndsWith(".webm")
		orderby Guid.NewGuid()
		select x).ToArray<string>();
	}

	// Token: 0x06003671 RID: 13937 RVA: 0x001497FC File Offset: 0x001479FC
	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.Keypad2))
		{
			this.LoadVideoList();
		}
		if (Input.GetKeyDown(KeyCode.Keypad1))
		{
			this.NextVideo();
		}
	}

	// Token: 0x06003672 RID: 13938 RVA: 0x00149824 File Offset: 0x00147A24
	private void NextVideo()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		string[] array = this.videos;
		int num = this.index;
		this.index = num + 1;
		string text = array[num % this.videos.Length];
		this.errored = false;
		if (Global.LaunchCountThisVersion <= 3)
		{
			string text2 = (from x in this.videos
			where x.EndsWith("whatsnew.mp4")
			select x).FirstOrDefault<string>();
			if (!string.IsNullOrEmpty(text2))
			{
				text = text2;
			}
		}
		Debug.Log("Playing Video " + text);
		VideoPlayer component = base.GetComponent<VideoPlayer>();
		component.url = text;
		component.Play();
	}

	// Token: 0x06003673 RID: 13939 RVA: 0x001498C6 File Offset: 0x00147AC6
	internal IEnumerator ReadyVideo()
	{
		if (this.errored)
		{
			yield break;
		}
		VideoPlayer player = base.GetComponent<VideoPlayer>();
		while (!player.isPrepared)
		{
			if (this.errored)
			{
				yield break;
			}
			yield return null;
		}
		yield break;
	}
}
