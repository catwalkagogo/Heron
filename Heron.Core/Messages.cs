using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Linq.Expressions;

namespace CatWalk.Heron {
	public abstract class MessageBase {
	}

	public static class Messages {
		public class CancelMessage : MessageBase {
			public bool Cancel { get; set; }

			public CancelMessage() { }
		}

		public class RequestViewMessage : MessageBase {
			public object View { get; set; }

			public RequestViewMessage(){
			}
		}

		public class RequestPropertyMessage<T> : MessageBase{
			private static Dictionary<string, Func<object, T>> _Cache = new Dictionary<string, Func<object, T>>();

			public T Value{get; set;}
			public string PropertyName{get; private set;}

			public RequestPropertyMessage(string propName){
				propName.ThrowIfNullOrEmpty("propName");
			}

			public void AssignToMessage(object obj) {
				obj.ThrowIfNull("obj");

				var type = obj.GetType();
				Func<object, T> call;

				var fullName = type.FullName + "#" + this.PropertyName;
				if(!_Cache.TryGetValue(fullName, out call)) {
					var expObj = Expression.Parameter(typeof(object));
					var expGet = Expression.Lambda<Func<object, T>>(
						Expression.Property(
							Expression.Convert(expObj, type),
							this.PropertyName),
						expObj
					);
					call = expGet.Compile();
					_Cache[fullName] = call;
				}
				this.Value = call(obj);
			}
		}

		public class SetPropertyMessage<T> : MessageBase {
			private static Dictionary<Tuple<Type, string>, Action<object, T>> _Cache = new Dictionary<Tuple<Type, string>, Action<object, T>>();

			public T Value {
				get;
				set;
			}
			public string PropertyName {
				get;
				private set;
			}

			public SetPropertyMessage(string propName){
				propName.ThrowIfNullOrEmpty("propName");
			}

			public void AssignToObject(object obj) {
				obj.ThrowIfNull("obj");
				var type = obj.GetType();
				Action<object, T> call;
				if(!_Cache.TryGetValue(Tuple.Create(type, this.PropertyName), out call)) {
					var expObj = Expression.Parameter(type);
					var expProp = Expression.Property(expObj, this.PropertyName);
					var expValue = Expression.Parameter(typeof(T), this.PropertyName);
					var expSet = Expression.Lambda<Action<object, T>>(
						Expression.Assign(expProp, expValue),
						expObj,
						expValue
					);
					call = expSet.Compile();
					_Cache[Tuple.Create(type, this.PropertyName)] = call;
				}
				call(obj, this.Value);
			}
		}

		public class SelectItemsMessage : MessageBase {
			public IEnumerable Items { get; private set; }
			public IEnumerable SelectedItems { get; set; }

			public SelectItemsMessage(IEnumerable items) {
				items.ThrowIfNull("items");
				this.Items = items;
			}
		}

		public class DataContextAttachedMessage : MessageBase {
			public DataContextAttachedMessage() {

			}
		}

	}
}
