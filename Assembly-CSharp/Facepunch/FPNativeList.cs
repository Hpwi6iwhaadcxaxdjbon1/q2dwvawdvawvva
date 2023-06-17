using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using UnityEngine;

namespace Facepunch
{
	// Token: 0x02000AEF RID: 2799
	public class FPNativeList<[IsUnmanaged] T> : Pool.IPooled where T : struct, ValueType
	{
		// Token: 0x04003C72 RID: 15474
		private NativeArray<T> _array;

		// Token: 0x04003C73 RID: 15475
		private int _length;

		// Token: 0x170005FB RID: 1531
		// (get) Token: 0x06004399 RID: 17305 RVA: 0x0018F1F5 File Offset: 0x0018D3F5
		public NativeArray<T> Array
		{
			get
			{
				return this._array;
			}
		}

		// Token: 0x170005FC RID: 1532
		// (get) Token: 0x0600439A RID: 17306 RVA: 0x0018F1FD File Offset: 0x0018D3FD
		public int Count
		{
			get
			{
				return this._length;
			}
		}

		// Token: 0x170005FD RID: 1533
		public T this[int index]
		{
			get
			{
				return this._array[index];
			}
			set
			{
				this._array[index] = value;
			}
		}

		// Token: 0x0600439D RID: 17309 RVA: 0x0018F224 File Offset: 0x0018D424
		public void Add(T item)
		{
			this.EnsureCapacity(this._length + 1);
			int length = this._length;
			this._length = length + 1;
			this._array[length] = item;
		}

		// Token: 0x0600439E RID: 17310 RVA: 0x0018F25C File Offset: 0x0018D45C
		public void Clear()
		{
			for (int i = 0; i < this._array.Length; i++)
			{
				this._array[i] = default(T);
			}
			this._length = 0;
		}

		// Token: 0x0600439F RID: 17311 RVA: 0x0018F29B File Offset: 0x0018D49B
		public void Resize(int count)
		{
			if (this._array.IsCreated)
			{
				this._array.Dispose();
			}
			this._array = new NativeArray<T>(count, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this._length = count;
		}

		// Token: 0x060043A0 RID: 17312 RVA: 0x0018F2CC File Offset: 0x0018D4CC
		public void EnsureCapacity(int requiredCapacity)
		{
			if (!this._array.IsCreated || this._array.Length < requiredCapacity)
			{
				int length = Mathf.Max(this._array.Length * 2, requiredCapacity);
				NativeArray<T> array = new NativeArray<T>(length, Allocator.Persistent, NativeArrayOptions.ClearMemory);
				if (this._array.IsCreated)
				{
					this._array.CopyTo(array.GetSubArray(0, this._array.Length));
					this._array.Dispose();
				}
				this._array = array;
			}
		}

		// Token: 0x060043A1 RID: 17313 RVA: 0x0018F34F File Offset: 0x0018D54F
		public void EnterPool()
		{
			if (this._array.IsCreated)
			{
				this._array.Dispose();
			}
			this._array = default(NativeArray<T>);
			this._length = 0;
		}

		// Token: 0x060043A2 RID: 17314 RVA: 0x000063A5 File Offset: 0x000045A5
		public void LeavePool()
		{
		}
	}
}
