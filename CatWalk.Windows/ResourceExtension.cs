/*
	$Id: ResourceExtension.cs 137 2010-12-19 10:01:30Z cs6m7y@bma.biglobe.ne.jp $
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Resources;

namespace CatWalk.Windows {
	[MarkupExtensionReturnType(typeof(string))]
	public class ResourceExtension : MarkupExtension{
		[ConstructorArgument("key")]
		public string Key{get; set;}

		public ResourceExtension(string key){
			this.Key = key;
		}

		public override object ProvideValue(IServiceProvider provider) {
			var binding = new Binding("Value");
			binding.Source = new ResourceData(this.Key);
			return binding.ProvideValue(provider);
		}

		private static ResourceManager resourceManager;
		public static ResourceManager ResourceManager{
			get{
				return resourceManager;
			}
			set{
				if(resourceManager != value){
					resourceManager = value;
					if(ResouceManagerChanged != null){
						ResouceManagerChanged(value, EventArgs.Empty);
					}
				}
			}
		}

		public static event EventHandler ResouceManagerChanged;
	}

	public class ResourceData : DisposableObject, INotifyPropertyChanged{
		public string Key{get; set;}

		public ResourceData(string key){
			this.Key = key;
			ResourceExtension.ResouceManagerChanged += this.ResourceManagerChangedHandler;
		}

		protected override void Dispose(bool disposing) {
			if(disposing){
				ResourceExtension.ResouceManagerChanged -= this.ResourceManagerChangedHandler;
			}
			base.Dispose(disposing);
		}

		public string Value{
			get{
				if(ResourceExtension.ResourceManager != null){
					return ResourceExtension.ResourceManager.GetString(this.Key);
				}
				return null;
			}
		}

		public void ResourceManagerChangedHandler(object sender, EventArgs e){
			if(this.PropertyChanged != null){
				this.PropertyChanged(this, new PropertyChangedEventArgs("Value"));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}

}
