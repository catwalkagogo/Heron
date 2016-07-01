using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CatWalk.Heron.Windows.Controls {
	public class FactoryDataTemplateSelector<TSource> : DataTemplateSelector {
		public Factory<TSource, DataTemplate> DataTemplateFactory { get; private set; }
		public virtual string DefaultTemplateKey { get; private set; }

		public FactoryDataTemplateSelector() : this(new Factory<TSource, DataTemplate>(), null) { }

		public FactoryDataTemplateSelector(Factory<TSource, DataTemplate> factory) : this(factory, null) { }

		public FactoryDataTemplateSelector(Factory<TSource, DataTemplate> factory, string defaultTemplateKey) {
			factory.ThrowIfNull(nameof(factory));
			this.DataTemplateFactory = factory;
			this.DefaultTemplateKey = defaultTemplateKey;
		}

		protected virtual TSource ItemDataSelector(object item) {
			return (TSource)item;
		}

		public override DataTemplate SelectTemplate(object item, DependencyObject container) {
			var element = (FrameworkElement)container;

			var view = this.DataTemplateFactory.Create(this.ItemDataSelector(item));

			return view ?? (DataTemplate)element.FindResource(this.DefaultTemplateKey);
		}
	}
}
