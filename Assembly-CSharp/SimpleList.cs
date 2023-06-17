using System;

// Token: 0x0200094D RID: 2381
public class SimpleList<T>
{
	// Token: 0x04003376 RID: 13174
	private const int defaultCapacity = 16;

	// Token: 0x04003377 RID: 13175
	private static readonly T[] emptyArray = new T[0];

	// Token: 0x04003378 RID: 13176
	public T[] array;

	// Token: 0x04003379 RID: 13177
	public int count;

	// Token: 0x1700048D RID: 1165
	// (get) Token: 0x06003932 RID: 14642 RVA: 0x00154895 File Offset: 0x00152A95
	public T[] Array
	{
		get
		{
			return this.array;
		}
	}

	// Token: 0x1700048E RID: 1166
	// (get) Token: 0x06003933 RID: 14643 RVA: 0x0015489D File Offset: 0x00152A9D
	public int Count
	{
		get
		{
			return this.count;
		}
	}

	// Token: 0x1700048F RID: 1167
	// (get) Token: 0x06003934 RID: 14644 RVA: 0x001548A5 File Offset: 0x00152AA5
	// (set) Token: 0x06003935 RID: 14645 RVA: 0x001548B0 File Offset: 0x00152AB0
	public int Capacity
	{
		get
		{
			return this.array.Length;
		}
		set
		{
			if (value != this.array.Length)
			{
				if (value > 0)
				{
					T[] destinationArray = new T[value];
					if (this.count > 0)
					{
						System.Array.Copy(this.array, 0, destinationArray, 0, this.count);
					}
					this.array = destinationArray;
					return;
				}
				this.array = SimpleList<T>.emptyArray;
			}
		}
	}

	// Token: 0x17000490 RID: 1168
	public T this[int index]
	{
		get
		{
			return this.array[index];
		}
		set
		{
			this.array[index] = value;
		}
	}

	// Token: 0x06003938 RID: 14648 RVA: 0x00154920 File Offset: 0x00152B20
	public SimpleList()
	{
		this.array = SimpleList<T>.emptyArray;
	}

	// Token: 0x06003939 RID: 14649 RVA: 0x00154933 File Offset: 0x00152B33
	public SimpleList(int capacity)
	{
		this.array = ((capacity == 0) ? SimpleList<T>.emptyArray : new T[capacity]);
	}

	// Token: 0x0600393A RID: 14650 RVA: 0x00154954 File Offset: 0x00152B54
	public void Add(T item)
	{
		if (this.count == this.array.Length)
		{
			this.EnsureCapacity(this.count + 1);
		}
		T[] array = this.array;
		int num = this.count;
		this.count = num + 1;
		array[num] = item;
	}

	// Token: 0x0600393B RID: 14651 RVA: 0x0015499C File Offset: 0x00152B9C
	public void Clear()
	{
		if (this.count > 0)
		{
			System.Array.Clear(this.array, 0, this.count);
			this.count = 0;
		}
	}

	// Token: 0x0600393C RID: 14652 RVA: 0x001549C0 File Offset: 0x00152BC0
	public bool Contains(T item)
	{
		for (int i = 0; i < this.count; i++)
		{
			if (this.array[i].Equals(item))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600393D RID: 14653 RVA: 0x00154A02 File Offset: 0x00152C02
	public void CopyTo(T[] array)
	{
		System.Array.Copy(this.array, 0, array, 0, this.count);
	}

	// Token: 0x0600393E RID: 14654 RVA: 0x00154A18 File Offset: 0x00152C18
	public void EnsureCapacity(int min)
	{
		if (this.array.Length < min)
		{
			int num = (this.array.Length == 0) ? 16 : (this.array.Length * 2);
			num = ((num < min) ? min : num);
			this.Capacity = num;
		}
	}
}
