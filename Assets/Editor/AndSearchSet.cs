using System;

namespace JAlgorithm
{
	public class AndSearchSet
	{
		private int[] root;
		public AndSearchSet(int count){
			root = new int[count];
			for (int i = 0; i < count; i++) {
				root [i] = i;
			}
		}

		/// <summary>
		/// 查找一个数据所属集合
		/// </summary>
		public int Search(int index){
			return root[index];
		}

		private int findFather(int x){
			return x == root[x] ? x : root[x] = findFather(root[x]);
		}

		/// <summary>
		/// 合并集合
		/// </summary>
		public void And(int index1, int index2){
			int fa = findFather(index1);
			int fb = findFather(index2);
			root [fa] = fb;
		}
	}
}

