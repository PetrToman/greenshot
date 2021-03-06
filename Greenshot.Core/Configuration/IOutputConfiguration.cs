//  Greenshot - a free and open source screenshot tool
//  Copyright (C) 2007-2017 Thomas Braun, Jens Klingen, Robin Krom
// 
//  For more information see: http://getgreenshot.org/
//  The Greenshot project is hosted on GitHub: https://github.com/greenshot
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 1 of the License, or
//  (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
// 
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

#region Usings

using System.Collections.Generic;
using System.ComponentModel;
using Dapplo.Config.Ini;

#endregion

namespace Greenshot.Core.Configuration
{
	/// <summary>
	///     This interface represents all the output settings
	/// </summary>
	public interface IOutputConfiguration : IIniSubSection
	{
		[Description("Which destinations? Possible options (more might be added by plugins) are: Editor, FileDefault, FileWithDialog, Clipboard, Printer, EMail, Picker")]
		[DefaultValue("Picker")]
		IList<string> OutputDestinations { get; set; }

		[Description("If the target file already exists True will make Greenshot always overwrite and False will display a 'Save-As' dialog.")]
		[DefaultValue(true)]
		bool OutputFileAllowOverwrite { get; set; }

		[Description("SaveAs Full path?")]
		string OutputFileAsFullpath { get; set; }

		[Description("When saving a screenshot, copy the path to the clipboard?")]
		[DefaultValue(true)]
		bool OutputFileCopyPathToClipboard { get; set; }

		[Description("Filename pattern for screenshot.")]
		[DefaultValue("${capturetime:d\"yyyy-MM-dd HH_mm_ss\"}-${title}")]
		string OutputFileFilenamePattern { get; set; }

		[Description("The number for the ${NUM} in the filename pattern, is increased automatically after each save.")]
		[DefaultValue(1)]
		uint OutputFileIncrementingNumber { get; set; }

		[Description("Output file path.")]
		string OutputFilePath { get; set; }

		[Description("Optional command to execute on a temporary PNG file, the command should overwrite the file and Greenshot will read it back. Note: this command is also executed when uploading PNG's!")]
		[DefaultValue("")]
		string OptimizePNGCommand { get; set; }

		[Description("Arguments for the optional command to execute on a PNG, {0} is replaced by the temp-filename from Greenshot. Note: Temp-file is deleted afterwards by Greenshot.")]
		[DefaultValue("\"{0}\"")]
		string OptimizePNGCommandArguments { get; set; }

		[Description("If set to true the amount of colors is counted and if smaller than 256 the color reduction is automatically used.")]
		[DefaultValue(false)]
		bool OutputFileAutoReduceColors { get; set; }

		[Description("Default file type for writing screenshots. (bmp, gif, jpg, png, tiff)")]
		[DefaultValue(OutputFormat.png)]
		OutputFormat OutputFileFormat { get; set; }

		[Description("JPEG file save quality in %.")]
		[DefaultValue(80)]
		int OutputFileJpegQuality { get; set; }

		[Description("Ask for the quality before saving?")]
		[DefaultValue(false)]
		bool OutputFilePromptQuality { get; set; }

		[Description("If set to true, than the colors of the output file are reduced to 256 (8-bit) colors")]
		[DefaultValue(false)]
		bool OutputFileReduceColors { get; set; }

		[Description("Amount of colors to reduce to, when reducing")]
		[DefaultValue(256)]
		int OutputFileReduceColorsTo { get; set; }
	}
}