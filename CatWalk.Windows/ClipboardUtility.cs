using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;

namespace CatWalk.Windows {
	public static class ClipboardUtility {
		public static DropEffect GetDropEffect() {
			var data = Clipboard.GetDataObject();
			var obj = data.GetData("Preferred DropEffect");
			var stream = obj as MemoryStream;
			if(stream != null) {
				return (DropEffect)BitConverter.ToInt32(stream.ToArray(), 0);
			} else {
				return DropEffect.None;
			}
		}

		public static void CopyFiles(string[] files) {
			SetFileDropList(DropEffect.Copy | DropEffect.Link, files);
		}

		public static void CutFiles(string[] files) {
			SetFileDropList(DropEffect.Move | DropEffect.Link, files);
		}

		public static void SetFileDropList(DropEffect effect, string[] files) {
			files.ThrowIfNull("files");

			IDataObject iDataObj = new DataObject(DataFormats.FileDrop, files);

			MemoryStream dropEffect = new MemoryStream();
			byte[] bytes = new byte[] { (byte)effect, 0, 0, 0 };
			dropEffect.Write(bytes, 0, bytes.Length);
			dropEffect.SetLength(bytes.Length);

			iDataObj.SetData("Preferred DropEffect", dropEffect);
			Clipboard.SetDataObject(iDataObj);
		}

		public static bool IsFilesInClipboard {
			get {
				return Clipboard.GetFileDropList() != null;
			}
		}

		public static IEnumerable<string> FileDropList {
			get {
				return IsFilesInClipboard ? Clipboard.GetFileDropList().Cast<string>() : new string[0];
			}
		}

	}

	[Flags]
	public enum DropEffect : int {
		None = 0,
		Copy = 1,
		Move = 2,
		Link = 4
	}
}
