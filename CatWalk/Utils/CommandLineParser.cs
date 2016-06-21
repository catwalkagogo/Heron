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
	/// �R�}���h���C����������͂���N���X�B
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
		/// �R�}���h���C����͂����s����B
		/// </summary>
		/// <param name="option">�R�}���h���C���̖��O�ƒl���`�����I�u�W�F�N�g</param>
		/// <param name="arguments">�R�}���h���C��</param>
		/// <remarks>
		/// option�����̃I�u�W�F�N�g�ɂ͕����̔C�ӂ�string�^��bool?�^�v���p�e�B���`���܂��B
		/// �����̃v���p�e�B�̃��j�[�N�Ȑ擪���������R�}���h���C���I�v�V�������ɂȂ�܂��B
		/// bool?�^�̃v���p�e�B�̓I�v�V������On/Off/�ȗ����擾�ł��܂�(/name(+|-))�B
		/// string�^�̃v���p�e�B�̓I�v�V�����̕�������擾�ł��܂�(/name:value)�B
		/// 
		/// �܂��A���string[]�^�̃v���p�e�B���`���邱�ƂŁA�I�v�V�����ȊO�̕�����(�t�@�C�����Ȃ�)���擾�ł��܂��B
		/// 
		/// <code>
		/// class CommandLineOption{
		/// 	public string[] Files{get; set;}
		/// 	public string Mask{get; set;} // /m:�t�@�C���}�X�N
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

			// �v���p�e�B�擾
			foreach(var prop in option.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
			                          .Where(prop => prop.CanWrite && prop.CanRead)){
				var action = GetAction(prop, option, ref listProp);
				if(action != null){
					// dicOptions�ɒǉ�
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

					// �v���C�I���e�B�ǉ�
					var prioAttr = prop.GetCustomAttributes(typeof(CommandLineParemeterPriorityAttribute), true)
						.Cast<CommandLineParemeterPriorityAttribute>().FirstOrDefault();
					if(prioAttr != null){
						priorities.Add(new Tuple<int, PropertyInfo, Action<string>>(prioAttr.Priority, prop, action));
					}else{
						priorities.Add(new Tuple<int, PropertyInfo, Action<string>>(0, prop, action));
					}

					// �f�t�H���g�p�����[�^
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

			// ���
			for(var i = 0; i < arguments.Count; i++){
				var arg = arguments[i];
				// �X�C�b�`�t��
				if(arg.StartsWith(this.SwitchPrefix)){
					// �L�[�ƒl���擾
					string value;
					string key;
					{
						string[] a;
						// ��؂肪�󔒂Ȃ��ǂ�
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
						// �l�����݂��Ȃ��A�t���O�̏ꍇ
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

					// �O����v����
					var founds = dicOption.Search(key).ToArray();
					// �����q�b�g�����ꍇ
					if(founds.Length > 1){
						var foundsProps = new HashSet<PropertyInfo>(founds.Select(pair => pair.Value.Item1));
						// �v���C�I���e�B���画��
						var props = priorities
							.Where(pair => foundsProps.Contains(pair.Item2))
							.GroupBy(pair => pair.Item1)
							.FirstOrDefault().ToArray();
						if(props.Length == 1){
							props[0].Item3(value);
						}
					// ������q�b�g�����ꍇ
					} else if(founds.Length == 1) {
						founds[0].Value.Item2(value);
					} else if(dict != null){
						dict[key] = value;
					}
				}else{ // �X�C�b�`�����̎�
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
				// �t���O�I�v�V�����̏ꍇ
				return new Action<string>(delegate(string arg){
					thisProp.SetValue(option, (arg.IsNullOrEmpty() || arg == "+"), null);
				});
			}else if(prop.PropertyType.Equals(typeof(string[]))){
				// ���X�g�̏ꍇ
				if(listProp != null){
					throw new ArgumentException("option");
				}
				listProp = thisProp;
				return null;
			}else{
				var conv = TypeDescriptor.GetConverter(prop.PropertyType);
				// �l�t���I�v�V�����̏ꍇ
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
		/// �R�}���h���C�������Ɏg�p���镶������G�X�P�[�v����B
		/// </summary>
		/// <param name="text">�G�X�P�[�v���镶����</param>
		/// <returns>�G�X�P�[�v�ς݂̕�����</returns>
		/// <remarks>
		/// " �� \" �ɁA% �� ^% �ɒu�������܂��B
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