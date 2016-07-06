using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CatWalk.Heron.Configuration {
	public interface IStorage : IDictionary<string, object>, INotifyPropertyChanged, IDisposable{
		T Get<T>(string key, T def);
	}
}
