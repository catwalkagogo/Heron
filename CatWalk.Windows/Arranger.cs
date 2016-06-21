using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace CatWalk.Windows {
	public abstract class Arranger{
		public abstract Rect[] Arrange(Size containerSize, int count);
	}

	public class CascadeArranger : Arranger{
		public double WindowOffset{get; set;}

		public CascadeArranger() : this(25d){}
		public CascadeArranger(double windowOffset){
			this.WindowOffset = windowOffset;
		}
	
		public override Rect[] Arrange(Size containerSize, int count){
			var rects = new Rect[count];
			double newWidth = containerSize.Width * 0.58, // should be non-linear formula here
				newHeight = containerSize.Height * 0.67,
				windowOffset = 0;
			for(var i = 0; i < count; i++){
				rects[i] = new Rect(windowOffset, windowOffset, newWidth, newHeight);

				windowOffset += this.WindowOffset;
				if (windowOffset + newWidth > containerSize.Width)
					windowOffset = 0;
				if (windowOffset + newHeight > containerSize.Height)
					windowOffset = 0;
			}
			return rects;
		}
	}

	public class TileVerticalArranger : Arranger{
		public override Rect[] Arrange(Size containerSize, int count){
			var rects = new Rect[count];
			int cols = (int)Math.Sqrt(count),
				rows = count / cols;

			List<int> col_count = new List<int>(); // windows per column
			for (int i = 0; i < cols; i++)
			{
				if (count % cols > cols - i - 1)
					col_count.Add(rows + 1);
				else
					col_count.Add(rows);
			}

			double newWidth = containerSize.Width / cols,
				newHeight = containerSize.Height / col_count[0],
				offsetTop = 0,
				offsetLeft = 0;

			for (int i = 0, col_index = 0, prev_count = 0; i < count; i++)
			{
				if (i >= prev_count + col_count[col_index])
				{
					prev_count += col_count[col_index++];
					offsetLeft += newWidth;
					offsetTop = 0;
					newHeight = containerSize.Height / col_count[col_index];
				}

				rects[i] = new Rect(offsetLeft, offsetTop, newWidth, newHeight);
				offsetTop += newHeight;
			}
			return rects;
		}
	}

	public class TileHorizontalArranger : Arranger{
		public override Rect[] Arrange(Size containerSize, int count){
			var rects = new Rect[count];
			int rows = (int)Math.Sqrt(count),
				cols = count / rows;

			List<int> col_count = new List<int>(); // windows per column
			for (int i = 0; i < cols; i++)
			{
				if (count % cols > cols - i - 1)
					col_count.Add(rows + 1);
				else
					col_count.Add(rows);
			}

			double newWidth = containerSize.Width / cols,
				newHeight = containerSize.Height / col_count[0],
				offsetTop = 0,
				offsetLeft = 0;

			for (int i = 0, col_index = 0, prev_count = 0; i < count; i++)
			{
				if (i >= prev_count + col_count[col_index])
				{
					prev_count += col_count[col_index++];
					offsetLeft += newWidth;
					offsetTop = 0;
					newHeight = containerSize.Height / col_count[col_index];
				}

				rects[i] = new Rect(offsetLeft, offsetTop, newWidth, newHeight);
				offsetTop += newHeight;
			}
			return rects;
		}
	}

	public class StackVerticalArranger : Arranger{
		public override Rect[] Arrange(Size containerSize, int count) {
			var rects = new Rect[count];
			double newWidth = containerSize.Width;
			double newHeight = containerSize.Height / count;
			double offsetTop = 0;
			double offsetLeft = 0;
			for(int i = 0; i < count; i++){
				rects[i] = new Rect(offsetLeft, offsetTop, newWidth, newHeight);
				offsetTop += newHeight;
			}
			return rects;
		}
	}

	public class StackHorizontalArranger : Arranger{
		public override Rect[] Arrange(Size containerSize, int count) {
			var rects = new Rect[count];
			double newWidth = containerSize.Width / count;
			double newHeight = containerSize.Height;
			double offsetTop = 0;
			double offsetLeft = 0;
			for(int i = 0; i < count; i++){
				rects[i] = new Rect(offsetLeft, offsetTop, newWidth, newHeight);
				offsetLeft += newWidth;
			}
			return rects;
		}
	}
}
