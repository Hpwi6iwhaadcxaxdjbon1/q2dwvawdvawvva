using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Facepunch
{
	// Token: 0x02000AEE RID: 2798
	public class VirtualScroll : MonoBehaviour
	{
		// Token: 0x04003C68 RID: 15464
		public int ItemHeight = 40;

		// Token: 0x04003C69 RID: 15465
		public int ItemSpacing = 10;

		// Token: 0x04003C6A RID: 15466
		public RectOffset Padding;

		// Token: 0x04003C6B RID: 15467
		[Tooltip("Optional, we'll try to GetComponent IDataSource from this object on awake")]
		public GameObject DataSourceObject;

		// Token: 0x04003C6C RID: 15468
		public GameObject SourceObject;

		// Token: 0x04003C6D RID: 15469
		public ScrollRect ScrollRect;

		// Token: 0x04003C6E RID: 15470
		public RectTransform OverrideContentRoot;

		// Token: 0x04003C6F RID: 15471
		private VirtualScroll.IDataSource dataSource;

		// Token: 0x04003C70 RID: 15472
		private Dictionary<int, GameObject> ActivePool = new Dictionary<int, GameObject>();

		// Token: 0x04003C71 RID: 15473
		private Stack<GameObject> InactivePool = new Stack<GameObject>();

		// Token: 0x0600438C RID: 17292 RVA: 0x0018ED1D File Offset: 0x0018CF1D
		public void Awake()
		{
			this.ScrollRect.onValueChanged.AddListener(new UnityAction<Vector2>(this.OnScrollChanged));
			if (this.DataSourceObject != null)
			{
				this.SetDataSource(this.DataSourceObject.GetComponent<VirtualScroll.IDataSource>(), false);
			}
		}

		// Token: 0x0600438D RID: 17293 RVA: 0x0018ED5B File Offset: 0x0018CF5B
		public void OnDestroy()
		{
			this.ScrollRect.onValueChanged.RemoveListener(new UnityAction<Vector2>(this.OnScrollChanged));
		}

		// Token: 0x0600438E RID: 17294 RVA: 0x0018ED79 File Offset: 0x0018CF79
		private void OnScrollChanged(Vector2 pos)
		{
			this.Rebuild();
		}

		// Token: 0x0600438F RID: 17295 RVA: 0x0018ED81 File Offset: 0x0018CF81
		public void SetDataSource(VirtualScroll.IDataSource source, bool forceRebuild = false)
		{
			if (this.dataSource == source && !forceRebuild)
			{
				return;
			}
			this.dataSource = source;
			this.FullRebuild();
		}

		// Token: 0x170005FA RID: 1530
		// (get) Token: 0x06004390 RID: 17296 RVA: 0x0018ED9D File Offset: 0x0018CF9D
		private int BlockHeight
		{
			get
			{
				return this.ItemHeight + this.ItemSpacing;
			}
		}

		// Token: 0x06004391 RID: 17297 RVA: 0x0018EDAC File Offset: 0x0018CFAC
		public void FullRebuild()
		{
			foreach (int key in this.ActivePool.Keys.ToArray<int>())
			{
				this.Recycle(key);
			}
			this.Rebuild();
		}

		// Token: 0x06004392 RID: 17298 RVA: 0x0018EDEC File Offset: 0x0018CFEC
		public void DataChanged()
		{
			foreach (KeyValuePair<int, GameObject> keyValuePair in this.ActivePool)
			{
				this.dataSource.SetItemData(keyValuePair.Key, keyValuePair.Value);
			}
			this.Rebuild();
		}

		// Token: 0x06004393 RID: 17299 RVA: 0x0018EE58 File Offset: 0x0018D058
		public void Rebuild()
		{
			if (this.dataSource == null)
			{
				return;
			}
			int itemCount = this.dataSource.GetItemCount();
			RectTransform rectTransform = (this.OverrideContentRoot != null) ? this.OverrideContentRoot : (this.ScrollRect.viewport.GetChild(0) as RectTransform);
			rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (float)(this.BlockHeight * itemCount - this.ItemSpacing + this.Padding.top + this.Padding.bottom));
			int num = Mathf.Max(2, Mathf.CeilToInt(this.ScrollRect.viewport.rect.height / (float)this.BlockHeight));
			int num2 = Mathf.FloorToInt((rectTransform.anchoredPosition.y - (float)this.Padding.top) / (float)this.BlockHeight);
			int num3 = num2 + num;
			this.RecycleOutOfRange(num2, (float)num3);
			for (int i = num2; i <= num3; i++)
			{
				if (i >= 0 && i < itemCount)
				{
					this.BuildItem(i);
				}
			}
		}

		// Token: 0x06004394 RID: 17300 RVA: 0x0018EF58 File Offset: 0x0018D158
		private void RecycleOutOfRange(int startVisible, float endVisible)
		{
			foreach (int key in (from x in this.ActivePool.Keys
			where x < startVisible || (float)x > endVisible
			select x).ToArray<int>())
			{
				this.Recycle(key);
			}
		}

		// Token: 0x06004395 RID: 17301 RVA: 0x0018EFD8 File Offset: 0x0018D1D8
		private void Recycle(int key)
		{
			GameObject gameObject = this.ActivePool[key];
			gameObject.SetActive(false);
			this.ActivePool.Remove(key);
			this.InactivePool.Push(gameObject);
		}

		// Token: 0x06004396 RID: 17302 RVA: 0x0018F014 File Offset: 0x0018D214
		private void BuildItem(int i)
		{
			if (i < 0)
			{
				return;
			}
			if (this.ActivePool.ContainsKey(i))
			{
				return;
			}
			GameObject item = this.GetItem();
			item.SetActive(true);
			this.dataSource.SetItemData(i, item);
			RectTransform rectTransform = item.transform as RectTransform;
			rectTransform.anchorMin = new Vector2(0f, 1f);
			rectTransform.anchorMax = new Vector2(1f, 1f);
			rectTransform.pivot = new Vector2(0.5f, 1f);
			rectTransform.offsetMin = new Vector2(0f, 0f);
			rectTransform.offsetMax = new Vector2(0f, (float)this.ItemHeight);
			rectTransform.sizeDelta = new Vector2((float)((this.Padding.left + this.Padding.right) * -1), (float)this.ItemHeight);
			rectTransform.anchoredPosition = new Vector2((float)(this.Padding.left - this.Padding.right) * 0.5f, (float)(-1 * (i * this.BlockHeight + this.Padding.top)));
			this.ActivePool[i] = item;
		}

		// Token: 0x06004397 RID: 17303 RVA: 0x0018F140 File Offset: 0x0018D340
		private GameObject GetItem()
		{
			if (this.InactivePool.Count == 0)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.SourceObject);
				gameObject.transform.SetParent((this.OverrideContentRoot != null) ? this.OverrideContentRoot : this.ScrollRect.viewport.GetChild(0), false);
				gameObject.transform.localScale = Vector3.one;
				gameObject.SetActive(false);
				this.InactivePool.Push(gameObject);
			}
			return this.InactivePool.Pop();
		}

		// Token: 0x02000F72 RID: 3954
		public interface IDataSource
		{
			// Token: 0x060054CA RID: 21706
			int GetItemCount();

			// Token: 0x060054CB RID: 21707
			void SetItemData(int i, GameObject obj);
		}
	}
}
