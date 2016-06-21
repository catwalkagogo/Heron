using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CatWalk.Windows.Extensions {
	public static class HoldingKeys {
		public static readonly RoutedEvent HoldingKeysReleasedEvent = EventManager.RegisterRoutedEvent("HoldingKeysReleased", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(HoldingKeys));

		public static void AddHoldingKeysReleasedHandler(DependencyObject d, RoutedEventHandler handler) {
			d.ThrowIfNull("d");
			handler.ThrowIfNull("handler");
			var uie = d as UIElement;
			if(uie != null) {
				uie.AddHandler(HoldingKeysReleasedEvent, handler);
			} else {
				var ce = d as ContentElement;
				if(ce != null) {
					ce.AddHandler(HoldingKeysReleasedEvent, handler);
				} else {
					var uie3 = d as UIElement3D;
					if(uie3 != null) {
						uie3.AddHandler(HoldingKeysReleasedEvent, handler);
					} else {
						throw new ArgumentException("d");
					}
				}
			}
		}

		public static void RemoveHoldingKeysReleasedHandler(DependencyObject d, RoutedEventHandler handler) {
			d.ThrowIfNull("d");
			handler.ThrowIfNull("handler");
			var uie = d as UIElement;
			if(uie != null) {
				uie.RemoveHandler(HoldingKeysReleasedEvent, handler);
			} else {
				var ce = d as ContentElement;
				if(ce != null) {
					ce.RemoveHandler(HoldingKeysReleasedEvent, handler);
				} else {
					var uie3 = d as UIElement3D;
					if(uie3 != null) {
						uie3.RemoveHandler(HoldingKeysReleasedEvent, handler);
					} else {
						throw new ArgumentException("d");
					}
				}
			}
		}

		public static readonly DependencyProperty HoldingKeysProperty =
			DependencyProperty.RegisterAttached("HoldingKeys", typeof(IReadOnlyCollection<Key>), typeof(HoldingKeys), new PropertyMetadata(null, OnHoldingModifiersChanged));

		[AttachedPropertyBrowsableForType(typeof(DependencyObject))]
		public static IReadOnlyCollection<Key> GetHoldingKeys(DependencyObject d) {
			return (IReadOnlyCollection<Key>)d.GetValue(HoldingKeysProperty);
		}

		[AttachedPropertyBrowsableForType(typeof(DependencyObject))]
		public static void SetHoldingKeys(DependencyObject d, IReadOnlyCollection<Key> keys) {
			d.SetValue(HoldingKeysProperty, keys);
		}

		private static void OnHoldingModifiersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var oldKeys = (IReadOnlyCollection<Key>)e.OldValue;
			var newKeys = (IReadOnlyCollection<Key>)e.NewValue;

			if(newKeys != null || newKeys.Count > 0) {
				var uie = d as UIElement;
				if(uie != null) {
					uie.PreviewKeyUp += OnPreviewKeyUp;
				} else {
					var ce = d as ContentElement;
					if(ce != null) {
						ce.PreviewKeyUp += OnPreviewKeyUp;
					} else {
						var uie3 = d as UIElement3D;
						if(uie3 != null) {
							uie3.PreviewKeyUp += OnPreviewKeyUp;
						} else {
							throw new ArgumentException("d");
						}
					}
				}
			}
		}

		private static void OnPreviewKeyUp(object sender, KeyEventArgs e) {
			if(e.Handled) {
				return;
			}

			var d = (DependencyObject)sender;
			var keys = GetHoldingKeys((DependencyObject)sender);
			var releasedKey = keys
				.Where(key => e.KeyboardDevice.GetKeyStates(key) == KeyStates.None)
				.Select(key => new Nullable<Key>(key))
				.FirstOrDefault();

			if(releasedKey != null) {
				// Released
				var e2 = new RoutedEventArgs(HoldingKeysReleasedEvent);
				var uie = d as UIElement;
				if(uie != null) {
					uie.RaiseEvent(e2);
				} else {
					var ce = d as ContentElement;
					if(ce != null) {
						ce.RaiseEvent(e2);
					} else {
						var uie3 = d as UIElement3D;
						if(uie3 != null) {
							uie3.RaiseEvent(e2);
						} else {
							throw new ArgumentException("d");
						}
					}
				}
			}
		}
	}
}
