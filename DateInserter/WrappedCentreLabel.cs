using System;
using Gtk;

namespace DateInserter
{
	public class WrappedCentreLabel : Gtk.Widget
	{
		string text;
		Pango.Layout layout;

		public WrappedCentreLabel()
		{
			WidgetFlags |= Gtk.WidgetFlags.NoWindow;
		}

		public WrappedCentreLabel(string text)
			: this()
		{
			this.Text = text;
		}

		public string Text
		{
			set
			{
				text = value;
				UpdateLayout();
			}
			get { return text; }
		}

		private void CreateLayout()
		{
			if (layout != null)
			{
				layout.Dispose();
			}

			layout = new Pango.Layout(PangoContext);
			layout.Wrap = Pango.WrapMode.Word;
		}


		void UpdateLayout()
		{
			if (layout == null)
				CreateLayout();
			layout.Alignment = Pango.Alignment.Center;
			layout.SetText(text);
		}

		protected override bool OnExposeEvent(Gdk.EventExpose evnt)
		{
			if (evnt.Window != GdkWindow || layout == null)
			{
				return base.OnExposeEvent(evnt);
			}
			layout.Width = (int)(Allocation.Width * 2 / 3 * Pango.Scale.PangoScale);
			Gtk.Style.PaintLayout(Style, GdkWindow, State, false, evnt.Area,
				this, null, Allocation.Width * 1 / 6 + Allocation.X, 12 + Allocation.Y, layout);
			return true;
		}

		protected override void OnStyleSet(Gtk.Style previous_style)
		{
			CreateLayout();
			UpdateLayout();
			base.OnStyleSet(previous_style);
		}

		public override void Dispose()
		{
			if (layout != null)
			{
				layout.Dispose();
				layout = null;
			}
			base.Dispose();
		}
	}
}
