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
			return (new ColumnDefinition[]{ColumnDefinition.NameColumn, ColumnDefinition.DisplayNameColumn}).Concat(this.GetAdditionalColumnProviders(entry));
		}
		protected virtual IEnumerable<ColumnDefinition> GetAdditionalColumnProviders(ISystemEntry entry) {
			return new ColumnDefinition[0];
		}
		public abstract bool TryParsePath(ISystemEntry root, string path, out ISystemEntry entry);
		public abstract IEnumerable<ISystemEntry> GetRootEntries(ISystemEntry parent);
		public virtual object GetEntryIcon(ISystemEntry entry, Int32Size size, CancellationToken token) {
			return null;
		}
		public abstract object GetViewModel(object parent, SystemEntryViewModel entry, object previous);

		private static readonly NameEntryGroupDescription _NameEntryGroupDescription = new NameEntryGroupDescription();

		public IEnumerable<EntryGroupDescription> GetGroupings(ISystemEntry entry) {
			return Seq.Make(_NameEntryGroupDescription).Concat(this.GetAdditionalGroupings(entry));
		}

		protected virtual IEnumerable<EntryGroupDescription> GetAdditionalGroupings(ISystemEntry entry) {
			return new EntryGroupDescription[0];
		}

		#region NameGroup

		private class NameEntryGroupDescription : EntryGroupDescription {
			/*private static readonly DelegateEntryGroup<int> AHGroup = new DelegateEntryGroup<int>(0, "A - H", name => {
				var c = Char.ToUpper(name[0]);
				return 'A' <= c && c <= 'H';
			});
			private static readonly DelegateEntryGroup<int> IPGroup = new DelegateEntryGroup<int>(0, "I - P", name => {
				var c = Char.ToUpper(name[0]);
				return 'I' <= c && c <= 'P';
			});
			private static readonly DelegateEntryGroup<int> QZGroup = new DelegateEntryGroup<int>(0, "Q - Z", name => {
				var c = Char.ToUpper(name[0]);
				return 'Q' <= c && c <= 'Z';
			});*/

			private static readonly DelegateEntryGroup<int>[] Candidates;

			static NameEntryGroupDescription() {
				Candidates =
					new[] { 
						new DelegateEntryGroup<int>(0x0001, "0 - 9", entry => entry.DisplayName[0].IsDecimalNumber())
					}
					.Concat(
						Enumerable.Range('A', 'Z')
							.Select(c => new DelegateEntryGroup<int>(0x0010 + c, "" + (char)c, entry => Char.ToUpper(entry.DisplayName[0]) == c)))
					.Concat(new[]{
						new DelegateEntryGroup<int>(0x0100, "ひらがな", entry => entry.DisplayName[0].IsHiragana()),
						new DelegateEntryGroup<int>(0x0101, "カタカナ", entry => entry.DisplayName[0].IsKatakana()),
						new DelegateEntryGroup<int>(0x0102, "漢字", entry => entry.DisplayName[0].IsKanji()),
						 new DelegateEntryGroup<int>(0, "etc.", entry => true),
					})
					.ToArray();
			}

			public override string ColumnName {
				get {
					return ColumnDefinition.DisplayNameColumn.Name;
				}
			}

			protected override IEntryGroup GroupNameFromItem(SystemEntryViewModel entry, int level, System.Globalization.CultureInfo culture) {
				return Candidates.FirstOrDefault(grp => grp.Filter(entry));
			}
		}

		#endregion
	}
}
