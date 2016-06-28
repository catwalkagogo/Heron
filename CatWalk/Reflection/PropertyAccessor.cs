using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace CatWalk.Reflection {
	public interface IPropertyAccessor{
		bool CanGetValue{get;}
		object GetValue(object target);
		bool CanSetValue{get;}
		void SetValue(object target, object value);
	}

	internal class PropertyAccessor<TTarget, TProperty> : IPropertyAccessor{
		private readonly Func<TTarget, TProperty> getter;
		private readonly Action<TTarget, TProperty> setter;

		public PropertyAccessor(Func<TTarget, TProperty> getter, Action<TTarget, TProperty> setter) {
			this.getter = getter;
			this.setter = setter;
		}

		public bool CanGetValue{
			get{
				return this.getter != null;
			}
		}

		public object GetValue(object target) {
			return this.getter((TTarget)target);
		}

		public bool CanSetValue{
			get{
				return this.setter != null;
			}
		}

		public void SetValue(object target, object value) {
			this.setter((TTarget)target, (TProperty)value);
		}
	}

	// PropertyInfoからIAccessorへの変換
	public static class PropertyExtension {
		public static IPropertyAccessor ToAccessor(this PropertyInfo pi) {
			Delegate getter = null;
			var getMethod = pi.GetMethod;
			if(getMethod != null){
				Type getterDelegateType = typeof(Func<,>).MakeGenericType(pi.DeclaringType, pi.PropertyType);
				getter = getMethod.CreateDelegate(getterDelegateType);
			}

			Delegate setter = null;
			var setMethod = pi.SetMethod;
			if(setMethod != null){
				Type setterDelegateType = typeof(Action<,>).MakeGenericType(pi.DeclaringType, pi.PropertyType);
				setter = setMethod.CreateDelegate(setterDelegateType);
			}

			Type accessorType = typeof(PropertyAccessor<,>).MakeGenericType(pi.DeclaringType, pi.PropertyType);
			IPropertyAccessor accessor = (IPropertyAccessor)Activator.CreateInstance(accessorType, getter, setter);

			return accessor;
		}
	}
}
