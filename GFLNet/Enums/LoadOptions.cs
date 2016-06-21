/*
	$Id: LoadOptions.cs 188 2011-03-25 20:12:22Z cs6m7y@bma.biglobe.ne.jp $
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GflNet {
	[Flags]
	public enum LoadOptions : uint{
		None                        = 0x00000000,
		SkipAlpha                   = 0x00000001, /* Alpha not loaded (32bits only)                     */
		IgnoreReadError             = 0x00000002,
		RecognizeFormatByExtensionOnly = 0x00000004, /* Use only extension to recognize format. Faster     */
		ReadAllComment              = 0x00000008, /* Read Comment in GFL_FILE_DESCRIPTION               */
		ForceColorModel             = 0x00000010, /* Force to load picture in the ColorModel            */
		PreviewNoCanvasResize       = 0x00000020, /* With gflLoadPreview, width & height are the maximum box */
		BinaryAsGrey                = 0x00000040, /* Load Black&White file in greyscale                 */
		OriginalColorModel          = 0x00000080, /* If the colormodel is CMYK, keep it                 */
		OnlyFirstFrame              = 0x00000100, /* No search to check if file is multi-frame          */
		OriginalDepth               = 0x00000200, /* In the case of 10/16 bits per component            */
		ReadMetadata                = 0x00000400, /* Read all metadata                                  */
		ReadComment                 = 0x00000800, /* Read comment                                       */
		HighQualityThumbnail        = 0x00001000, /* gflLoadThumbnail                                   */
		EmbeddedThumbnail           = 0x00002000, /* gflLoadThumbnail                                   */
		OrientedThumbnail           = 0x00004000, /* gflLoadThumbnail                                   */
		OriginalEmbeddedThumbnail   = 0x00008000, /* gflLoadThumbnail                                   */
		Oriented                    = 0x00008000,
	}
}
