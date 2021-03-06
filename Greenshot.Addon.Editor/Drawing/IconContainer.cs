﻿//  Greenshot - a free and open source screenshot tool
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

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.Serialization;
using Dapplo.Log;
using Greenshot.Addon.Editor.Interfaces.Drawing;

#endregion

namespace Greenshot.Addon.Editor.Drawing
{
	/// <summary>
	///     Description of IconContainer.
	/// </summary>
	[Serializable]
	public class IconContainer : DrawableContainer, IIconContainer
	{
		private static readonly LogSource Log = new LogSource();

		private Icon _icon;

		public IconContainer(Surface parent) : base(parent)
		{
			Init();
		}

		public IconContainer(Surface parent, string filename) : base(parent)
		{
			Load(filename);
		}

		public override Size DefaultSize
		{
			get { return _icon.Size; }
		}

		public override bool HasDefaultSize
		{
			get { return true; }
		}

		public Icon Icon
		{
			set
			{
				if (_icon != null)
				{
					_icon.Dispose();
				}
				_icon = (Icon) value.Clone();
				Width = value.Width;
				Height = value.Height;
			}
			get { return _icon; }
		}

		public void Load(string filename)
		{
			if (File.Exists(filename))
			{
				using (Icon fileIcon = new Icon(filename))
				{
					Icon = fileIcon;
					Log.Debug().WriteLine("Loaded file: {0} with resolution: {1},{2}", filename, Height, Width);
				}
			}
		}

		/**
	     * This Dispose is called from the Dispose and the Destructor.
	     * When disposing==true all non-managed resources should be freed too!
	     */

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_icon != null)
				{
					_icon.Dispose();
				}
			}
			_icon = null;
			base.Dispose(disposing);
		}

		public override void Draw(Graphics graphics, RenderMode rm)
		{
			if (_icon != null)
			{
				GraphicsState state = graphics.Save();
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
				graphics.CompositingQuality = CompositingQuality.Default;
				graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
				graphics.DrawIcon(_icon, Bounds);
				graphics.Restore(state);
			}
		}

		private void Init()
		{
			CreateDefaultAdorners();
		}

		protected override void OnDeserialized(StreamingContext streamingContext)
		{
			base.OnDeserialized(streamingContext);
			Init();
		}
	}
}