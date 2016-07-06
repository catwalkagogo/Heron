using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatWalk.Heron.ViewModel.IOSystem;
using CatWalk.IOSystem;

namespace CatWalk.Heron.IOSystem {
	internal class BuiltinSystemProvider : ISystemProvider {
		public string DisplayName {
			get {
				return "Builtin";
			}
		}

		public string Name {
			get {
				return "Builtin";
			}
		}

		public bool CanGetColumnDefinitions(ISystemEntry entry) {
			return true;
		}

		public bool CanGetGroupings(ISystemEntry entry) {
			return true;
		}

		public bool CanGetOrderDefinitions(SystemEntryViewModel entry) {
			return true;
		}

		public bool CanGetViewModel(object parent, SystemEntryViewModel entry, object previous) {
			return false;
		}

		public IEnumerable<IColumnDefinition> GetColumnDefinitions(ISystemEntry entry) {
			return (new ColumnDefinition[]{
				ColumnDefinition.NameColumn,
				ColumnDefinition.DisplayNameColumn,
			});
		}

		private static readonly NameEntryGroupDescription _DisplayNameEntryGroupDescription = new NameEntryGroupDescription();

		public IEnumerable<IGroupDefinition> GetGroupings(ISystemEntry entry) {
			return Seq.Make(_DisplayNameEntryGroupDescription);
		}

		public IEnumerable<OrderDefinition> GetOrderDefinitions(SystemEntryViewModel entry) {
			return entry.ChildrenColumns.Where(_ => _.CanSort).Select(_ => OrderDefinition.FromColumnDefinition(_));
		}

		public IEnumerable<ISystemEntry> GetRootEntries(ISystemEntry parent) {
			return new ISystemEntry[0];
		}

		public object GetViewModel(object parent, SystemEntryViewModel entry, object previous) {
			return null;
		}

		public ParsePathResult ParsePath(ISystemEntry root, string path) {
			return new ParsePathResult(false, null, false);
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
