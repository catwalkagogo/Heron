/*
	$Id: CommandLine.cs 250 2011-07-13 10:14:13Z catwalkagogo@gmail.com $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ComponentModel;
using CatWalk.Collections;

namespace CatWalk.Utils{
	/// <summary>
	/// コマンドライン引数を解析するクラス。
	/// </summary>
	public class CommandLineParser{
		public CommandLineParser() : this("/", ":", StringComparer.Ordinal){}
		public CommandLineParser(string prefix, string separator) : this(prefix, separator, StringComparer.Ordinal){}
		public CommandLineParser(string prefix, string separator, StringComparer comparer){
			prefix.ThrowIfNull("prefix");
			separator.ThrowIfNull("separator");
			this.SwitchPrefix = prefix;
			this.ParameterSeparator = separator;
			this.StringComparer = comparer;
		}

		#region Property

		private static WeakReference<CommandLineParser> _Default;
		public static CommandLineParser Default{
			get{
				CommandLineParser parser;
				if(!_Default.TryGetTarget(out parser)) {
					parser = new CommandLineParser();
					_Default = new WeakReference<CommandLineParser>(parser);
				}
				return parser;
			}
		}

		private string _SwitchPrefix;
		public string SwitchPrefix{
			get{
				return this._SwitchPrefix;
			}
			set{
				value.ThrowIfNull();
				if(value != "" && value.IsNullOrWhitespace()){
					throw new ArgumentException();
				}
				this._SwitchPrefix = value;
			}
		}

		private string _ParameterSeparator;
		public string ParameterSeparator{
			get{
				return this._ParameterSeparator;
			}
			set{
				value.ThrowIfNull();
				if(value.IsNullOrEmpty()){
					throw new ArgumentException();
				}
				this._ParameterSeparator = value;
			}
		}

		private StringComparer _StringComparer;
		public StringComparer StringComparer{
			get{
				return this._StringComparer;
			}
			set{
				value.ThrowIfNull();
				this._StringComparer = value;
			}
		}

		#endregion
	
		public void Parse(object option){
			Parse(option, GetArguments());
		}

		/// <summary>
		/// コマンドライン解析を実行する。
		/// </summary>
		/// <param name="option">コマンドラインの名前と値を定義したオブジェクト</param>
		/// <param name="arguments">コマンドライン</param>
		/// <remarks>
		/// option引数のオブジェクトには複数の任意のstring型とbool?型プロパティを定義します。
		/// これらのプロパティのユニークな先頭数文字がコマンドラインオプション名になります。
		/// bool?型のプロパティはオプションのOn/Off/省略を取得できます(/name(+|-))。
		/// string型のプロパティはオプションの文字列を取得できます(/name:value)。
		/// 
		/// また、一つのstring[]型のプロパティを定義することで、オプション以外の文字列(ファイル名など)を取得できます。
		/// 
		/// <code>
		/// class CommandLineOption{
		/// 	public string[] Files{get; set;}
		/// 	public string Mask{get; set;} // /m:ファイルマスク
		/// 	public bool? Recursive{get; set;} // /rec(+|-)
		/// 	public bool? Regex{get; set;} // /reg(+|-)
		/// }
		/// </code>
		/// <code>
		/// var option = new CommandLineOption();
		/// CommandLineParser.Parse(option, args, StringComparer.OrdinalIgnoreCase);
		/// </code>
		/// </remarks>
		public void Parse(object option, IReadOnlyList<string> arguments) {
			option.ThrowIfNull("option");
			arguments.ThrowIfNull("arguments");
			var comparer = this._StringComparer;
			
			var comp = new LambdaComparer<char>((x, y) => comparer.Compare(x.ToString(), y.ToString()));
			var dicOption = new PrefixDictionary<Tuple<PropertyInfo, Action<string>>>(comp);
			var defaultActions = new List<Tuple<CommandLineParemeterOrderAttribute, Action<string>>>();
			var priorities = new List<Tuple<int, PropertyInfo, Action<string>>>();
			var list = new List<string>();
			var dict = option as IDictionary<string, string>;
			PropertyInfo listProp = null;

			// プロパティ取得
			foreach(var prop in option.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
			                          .Where(prop => prop.CanWrite && prop.CanRead)){
				var action = GetAction(prop, option, ref listProp);
				if(action != null){
					// dicOptionsに追加
					var entry = new Tuple<PropertyInfo, Action<string>>(prop, action);
					var nameAttrs = prop.GetCustomAttributes(typeof(CommandLineOptionNameAttribute), true)
						.Cast<CommandLineOptionNameAttribute>().ToArray();
					if(nameAttrs.Length > 0){
						foreach(var attr in nameAttrs){
							dicOption.Add(attr.Name, entry);
						}
					}else{
						dicOption.Add(prop.Name, entry);
					}

					// プライオリティ追加
					var prioAttr = prop.GetCustomAttributes(typeof(CommandLineParemeterPriorityAttribute), true)
						.Cast<CommandLineParemeterPriorityAttribute>().FirstOrDefault();
					if(prioAttr != null){
						priorities.Add(new Tuple<int, PropertyInfo, Action<string>>(prioAttr.Priority, prop, action));
					}else{
						priorities.Add(new Tuple<int, PropertyInfo, Action<string>>(0, prop, action));
					}

					// デフォルトパラメータ
					var orderAttr = prop.GetCustomAttributes(typeof(CommandLineParemeterOrderAttribute), true)
						.Cast<CommandLineParemeterOrderAttribute>().FirstOrDefault();
					if(orderAttr != null){
						defaultActions.Add(new Tuple<CommandLineParemeterOrderAttribute, Action<string>>(orderAttr, action));
					}
				}
			}
			defaultActions.Sort(new LambdaComparer<Tuple<CommandLineParemeterOrderAttribute, Action<string>>>(
				(a, b) => a.Item1.Index.CompareTo(b.Item1.Index)));
			priorities.Sort(new LambdaComparer<Tuple<int, PropertyInfo, Action<string>>>(
				(a, b) => a.Item1.CompareTo(b.Item1)));

			// 解析
			for(var i = 0; i < arguments.Count; i++){
				var arg = arguments[i];
				// スイッチ付き
				if(arg.StartsWith(this.SwitchPrefix)){
					// キーと値を取得
					string value;
					string key;
					{
						string[] a;
						// 区切りが空白なら先読み
						if(String.IsNullOrWhiteSpace(this.ParameterSeparator)){
							var j = i + 1;
							if(j < arguments.Count){
								a = new[]{arguments[i], arguments[j]};
								i = j;
							}else{
								a = new[]{arguments[j]};
							}
						}else{
							a = arg.Substring(this.SwitchPrefix.Length)
								.Split(new string[]{this.ParameterSeparator}, StringSplitOptions.None);
						}
						// 値が存在しない、フラグの場合
						if(a.Length == 1){
							int last = arg.Length - 1;
							if(arg[last] == '+' || arg[last] == '-'){
								key = arg.Substring(0, last);
								value = arg[last].ToString();
							}else{
								key = arg;
								value = "";
							}
						}else{
							key = a[0];
							value = a[1];
						}
					}

					// 前方一致検索
					var founds = dicOption.Search(key).ToArray();
					// 複数ヒットした場合
					if(founds.Length > 1){
						var foundsProps = new HashSet<PropertyInfo>(founds.Select(pair => pair.Value.Item1));
						// プライオリティから判定
						var props = priorities
							.Where(pair => foundsProps.Contains(pair.Item2))
							.GroupBy(pair => pair.Item1)
							.FirstOrDefault().ToArray();
						if(props.Length == 1){
							props[0].Item3(value);
						}
					// 一個だけヒットした場合
					} else if(founds.Length == 1) {
						founds[0].Value.Item2(value);
					} else if(dict != null){
						dict[key] = value;
					}
				}else{ // スイッチ無しの時
					if(defaultActions.Count > 0){
						defaultActions[0].Item2(arg);
						defaultActions.RemoveAt(0);
					}else{
						if(listProp != null){
							list.Add(arg);
						}
					}
				}
			}

			if(listProp != null){
				listProp.SetValue(option, list.ToArray(), null);
			}
		}
		
		private Action<string> GetAction(PropertyInfo prop, object option, ref PropertyInfo listProp){
			var thisProp = prop;
			if(prop.PropertyType.Equals(typeof(Nullable<bool>))){
				// フラグオプションの場合
				return new Action<string>(delegate(string arg){
					thisProp.SetValue(option, (arg.IsNullOrEmpty() || arg == "+"), null);
				});
			}else if(prop.PropertyType.Equals(typeof(string[]))){
				// リストの場合
				if(listProp != null){
					throw new ArgumentException("option");
				}
				listProp = thisProp;
				return null;
			}else{
				var conv = TypeDescriptor.GetConverter(prop.PropertyType);
				// 値付きオプションの場合
				return new Action<string>(delegate(string arg){
					try{
						thisProp.SetValue(option, conv.ConvertFromString(arg), null);
					}catch(Exception){
					}
				});
			}
		}

		public T Parse<T>() where T : new(){
			return this.Parse<T>(GetArguments());
		}
		public T Parse<T>(IReadOnlyList<string> arguments) where T : new() {
			T option = new T();
			this.Parse(option, arguments);
			return option;
		}

		private static IReadOnlyList<string> GetArguments() {
			return Environment.GetCommandLineArgs().Skip(1).ToArray();
		}

		/// <summary>
		/// コマンドライン引数に使用する文字列をエスケープする。
		/// </summary>
		/// <param name="text">エスケープする文字列</param>
		/// <returns>エスケープ済みの文字列</returns>
		/// <remarks>
		/// " を \" に、% を ^% に置き換えます。
		/// </remarks>
		public static string Escape(string text){
			return text.Replace("\"", "\\\"").Replace("%", "^%");
		}
	}

	[AttributeUsage(AttributeTargets.Property)]
	public class CommandLineOptionNameAttribute : Attribute{
		public string Name{get; set;}

		public CommandLineOptionNameAttribute(string name){
			this.Name = name;
		}
	}

	[AttributeUsage(AttributeTargets.Property)]
	public class CommandLineParemeterOrderAttribute : Attribute{
		public int Index{get; set;}
		public CommandLineParemeterOrderAttribute(int index){
			this.Index = index;
		}
	}

	[AttributeUsage(AttributeTargets.Property)]
	public class CommandLineParemeterPriorityAttribute : Attribute{
		public int Priority{get; set;}
		public CommandLineParemeterPriorityAttribute(int prior){
			this.Priority = prior;
		}
	}

}