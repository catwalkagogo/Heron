using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CatWalk.IOSystem;
using CatWalk;
using CatWalk.Heron.ViewModel.IOSystem;

namespace CatWalk.Heron.IOSystem {
	public abstract class SystemProvider : ISystemProvider {
		public virtual string Name {
			get {
				return this.GetType().Name;
			}
		}
		public virtual string DisplayName {
			get {
				return this.Name;
			}
		}

		public IEnumerable<IColumnDefinition> GetColumnDefinitions(ISystemEntry entry) {
			return (new ColumnDefinition[]{ColumnDefinition.NameColumn}).Concat(this.GetAdditionalColumnProviders(entry));
		}
		protected virtual IEnumerable<ColumnDefinition> GetAdditionalColumnProviders(ISystemEntry entry) {
			return new ColumnDefinition[0];
		}
		public abstract ParsePathResult ParsePath(ISystemEntry root, string path);

		public abstract IEnumerable<ISystemEntry> GetRootEntries(ISystemEntry parent);
		public virtual object GetEntryIcon(ISystemEntry entry, Size<int> size, CancellationToken token) {
			return null;
		}
		public abstract object GetViewModel(object parent, SystemEntryViewModel entry, object previous);
		
		private static readonly NameEntryGroupDescription _DisplayNameEntryGroupDescription = new NameEntryGroupDescription();
		
		public IEnumerable<IGroupDefinition> GetGroupings(ISystemEntry entry) {
			return Seq.Make(_DisplayNameEntryGroupDescription).Concat(this.GetAdditionalGroupings(entry));
		}

		protected virtual IEnumerable<IGroupDefinition> GetAdditionalGroupings(ISystemEntry entry) {
			return new IGroupDefinition[0];
		}
		
		public IEnumerable<OrderDefinition> GetOrderDefinitions(SystemEntryViewModel entry) {
			return entry.Columns.Values.Select(vm => OrderDefinition.FromColumnDefinition(vm.Definition));
		}

		#region NameGroup
		
		private class NameEntryGroupDescription : IGroupDefinition {
			private static readonly DelegateEntryGroup<int>[] Candidates;

			static NameEntryGroupDescription() {
				Candidates =
					new[] { 
						new DelegateEntryGroup<int>(0x0001, "0 - 9", entry => entry.Name[0].IsDecimalNumber())
					}
					.Concat(
						Enumerable.Range('A', 'Z')
							.Select(c => new DelegateEntryGroup<int>(0x0010 + c, "" + (char)c, entry => Char.ToUpper(entry.Name[0]) == c)))
					.Concat(new[]{
						new DelegateEntryGroup<int>(0x0100, "ひらがな", entry => entry.Name[0].IsHiragana()),
						new DelegateEntryGroup<int>(0x0101, "カタカナ", entry => entry.Name[0].IsKatakana()),
						new DelegateEntryGroup<int>(0x0102, "漢字", entry => entry.Name[0].IsKanji()),
						 new DelegateEntryGroup<int>(0, "etc.", entry => true),
					})
					.ToArray();
			}

			public IGroup GetGroupName(SystemEntryViewModel entry) {
				return Candidates.FirstOrDefault(grp => grp.Filter(entry));
			}
		}
		
		#endregion
	}
}
