using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatWalk.IOSystem;
using CatWalk.Collections;

namespace CatWalk.Heron.Configuration.IOSystem {
	public class ConfigurationWatcher : IIOSystemWatcher {
		public ConfigurationDirectory Directory { get; private set; }
		public bool IsEnabled { get; set; }

		public ConfigurationWatcher(ConfigurationDirectory dir) {
			dir.ThrowIfNull("dir");

			this.Directory = dir;
			this.Directory.Storage.PropertyChanged += Storage_PropertyChanged;
		}

		private void Storage_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
			if (!this.IsEnabled) {
				return;
			}

			var entry = new ConfigurationEntry(this.Directory, e.PropertyName);

			var handler = this.CollectionChanged;
			if(handler != null) {
				handler(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, entry, entry));
			}
		}

		public ISystemEntry Target {
			get {
				return this.Directory;
			}
		}

		public event NotifyCollectionChangedEventHandler CollectionChanged;
	}
}
