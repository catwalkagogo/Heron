using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Linq.Expressions;

namespace CatWalk.Collections {
	public static class CollectionExpressions {
		public static Action<object, object> GetAddFunction(Type type) {
			type.ThrowIfNull("type");
			var method = type.GetRuntimeMethod("Add", new Type[0]);
			var obj = Expression.Parameter(typeof(object), "obj");
			var objCast = Expression.Convert(obj, type);
			var param_v = Expression.Parameter(typeof(object), "v");
			var param_v_cast = Expression.Convert(param_v, method.GetParameters()[0].ParameterType);
			var call = Expression.Call(
				objCast,
				method,
				param_v_cast);
			var lambda = Expression.Lambda<Action<object, object>>(
				call, obj, param_v).Compile();
			return lambda;
		}

		public static Action<object, object> GetRemoveFunction(Type type) {
			type.ThrowIfNull("type");
			var method = type.GetRuntimeMethod("Remove", new Type[0]);
			var obj = Expression.Parameter(typeof(object), "obj");
			var objCast = Expression.Convert(obj, type);
			var param_v = Expression.Parameter(typeof(object), "v");
			var param_v_cast = Expression.Convert(param_v, method.GetParameters()[0].ParameterType);
			var call = Expression.Call(
				objCast,
				method,
				param_v_cast);
			var lambda = Expression.Lambda<Action<object, object>>(
				call, obj, param_v).Compile();
			return lambda;
		}

		public static Action<object> GetClearFunction(Type type) {
			type.ThrowIfNull("type");
			var method = type.GetRuntimeMethod("Clear", new Type[0]);
			var obj = Expression.Parameter(typeof(object), "obj");
			var objCast = Expression.Convert(obj, type);
			//var instance = Expression.Parameter(type, "instance");
			var call = Expression.Call(
				objCast,
				method);
			var lambda = Expression.Lambda<Action<object>>(
				call, obj).Compile();
			return lambda;
		}

	}
}
