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

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dapplo.Windows.Enums;
using Dapplo.Windows.Native;
using Dapplo.Windows.SafeHandles;
using Dapplo.Windows.Structs;

#endregion

namespace Greenshot.Legacy.Controls
{
	/// <summary>
	///     This code was supplied by Hi-Coder as a patch for Greenshot
	///     Needed some modifications to be stable.
	/// </summary>
	public sealed class Pipette : Label, IMessageFilter, IDisposable
	{
		private const int VkEsc = 27;
		private readonly Bitmap _image;
		private Cursor _cursor;
		private bool _dragging;
		private MovableShowColorForm _movableShowColorForm;

		public Pipette()
		{
			BorderStyle = BorderStyle.FixedSingle;
			_dragging = false;
			_image = (Bitmap) new ComponentResourceManager(typeof(ColorDialog)).GetObject("pipette.Image");
			Image = _image;
			_cursor = CreateCursor(_image, 1, 14);
			_movableShowColorForm = new MovableShowColorForm();
			Application.AddMessageFilter(this);
		}

		/// <summary>
		///     The bulk of the clean-up code is implemented in Dispose(bool)
		/// </summary>
		public new void Dispose()
		{
			Dispose(true);
		}

		#region IMessageFilter Members

		public bool PreFilterMessage(ref Message m)
		{
			if (_dragging)
			{
				if (m.Msg == (int) WindowsMessages.WM_CHAR)
				{
					if ((int) m.WParam == VkEsc)
					{
						User32.ReleaseCapture();
					}
				}
			}
			return false;
		}

		#endregion

		/// <summary>
		///     Create a cursor from the supplied bitmap & hotspot coordinates
		/// </summary>
		/// <param name="bitmap">Bitmap to create an icon from</param>
		/// <param name="hotspotX">Hotspot X coordinate</param>
		/// <param name="hotspotY">Hotspot Y coordinate</param>
		/// <returns>Cursor</returns>
		private static Cursor CreateCursor(Bitmap bitmap, int hotspotX, int hotspotY)
		{
			using (SafeIconHandle iconHandle = new SafeIconHandle(bitmap.GetHicon()))
			{
				IconInfo iconInfo;
				User32.GetIconInfo(iconHandle, out iconInfo);
				iconInfo.xHotspot = hotspotX;
				iconInfo.yHotspot = hotspotY;
				iconInfo.fIcon = false;
				var icon = User32.CreateIconIndirect(ref iconInfo);
				return new Cursor(icon);
			}
		}

		/// <summary>
		///     This Dispose is called from the Dispose and the Destructor.
		/// </summary>
		/// <param name="disposing">When disposing==true all non-managed resources should be freed too!</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_cursor != null)
				{
					_cursor.Dispose();
				}
				_movableShowColorForm?.Dispose();
			}
			_movableShowColorForm = null;
			_cursor = null;
			base.Dispose(disposing);
		}

		/// <summary>
		///     Handle the MouseCaptureChanged event
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseCaptureChanged(EventArgs e)
		{
			if (Capture)
			{
				_dragging = true;
				Image = null;
				Cursor c = _cursor;
				Cursor = c;
				_movableShowColorForm.Visible = true;
			}
			else
			{
				_dragging = false;
				Image = _image;
				Cursor = Cursors.Arrow;
				_movableShowColorForm.Visible = false;
			}
			Update();
			base.OnMouseCaptureChanged(e);
		}

		/// <summary>
		///     Handle the mouse down on the Pipette "label", we take the capture and move the zoomer to the current location
		/// </summary>
		/// <param name="e">MouseEventArgs</param>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				User32.SetCapture(Handle);
				_movableShowColorForm.MoveTo(PointToScreen(new Point(e.X, e.Y)));
			}
			base.OnMouseDown(e);
		}

		/// <summary>
		///     Handle the mouse Move event, we move the ColorUnderCursor to the current location.
		/// </summary>
		/// <param name="e">MouseEventArgs</param>
		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (_dragging)
			{
				//display the form on the right side of the cursor by default;
				Point zp = PointToScreen(new Point(e.X, e.Y));
				_movableShowColorForm.MoveTo(zp);
			}
			base.OnMouseMove(e);
		}

		/// <summary>
		///     Handle the mouse up on the Pipette "label", we release the capture and fire the PipetteUsed event
		/// </summary>
		/// <param name="e">MouseEventArgs</param>
		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				//Release Capture should consume MouseUp when canceled with the escape key 
				User32.ReleaseCapture();
				PipetteUsed?.Invoke(this, new PipetteUsedArgs(_movableShowColorForm.color));
			}
			base.OnMouseUp(e);
		}

		public event EventHandler<PipetteUsedArgs> PipetteUsed;
	}

	public class PipetteUsedArgs : EventArgs
	{
		public Color Color
		{
			get; set; }

		public PipetteUsedArgs(Color c)
		{
			Color = c;
		}
	}
}