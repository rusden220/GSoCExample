using System;
using Gtk;

using MonoDevelop.Components.Docking;
using MonoDevelop.Core;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide;
using System.Collections.Generic;
using MonoDevelop.Components;
using MonoDevelop.Ide.Gui.Content;
using MonoDevelop.Ide.Gui.Components;


namespace DateInserter
{
	public class DocumentOutlinePad : PadContent
	{
		Alignment box;
		//IOutlinedDocument currentOutlineDoc;
		Document currentDoc;
		DockItemToolbar toolbar;

		public DocumentOutlinePad()
		{
			box = new Gtk.Alignment(0, 0, 1, 1);
			box.BorderWidth = 0;
			SetWidget(null);
			box.ShowAll();
		}

		protected override void Initialize(IPadWindow window)
		{
			base.Initialize(window);
			IdeApp.Workbench.ActiveDocumentChanged += DocumentChangedHandler;
			CurrentDoc = IdeApp.Workbench.ActiveDocument;
			toolbar = window.GetToolbar(DockPositionType.Top);
			toolbar.Visible = false;
			Update();
		}

		public override void Dispose()
		{
			IdeApp.Workbench.ActiveDocumentChanged -= DocumentChangedHandler;
			CurrentDoc = null;
			ReleaseDoc();
			base.Dispose();
		}

		Document CurrentDoc
		{
			get { return currentDoc; }
			set
			{
				if (value == currentDoc)
					return;
				if (currentDoc != null)
					currentDoc.ViewChanged -= ViewChangedHandler;
				currentDoc = value;
				if (currentDoc != null)
					currentDoc.ViewChanged += ViewChangedHandler;
			}
		}

		public override Control Control
		{
			get { return box; }
		}

		void ViewChangedHandler(object sender, EventArgs args)
		{
			Update();
		}

		void DocumentChangedHandler(object sender, EventArgs args)
		{
			CurrentDoc = IdeApp.Workbench.ActiveDocument;
			Update();
		}


		void Update()
		{
			ReleaseDoc();
			SetWidget(TreeView());
		}
		private TreeView TreeView()
		{
			IPathedDocument padDoc = IdeApp.Workbench.ActiveDocument.GetContent<IPathedDocument>();
			Gtk.TreeView tree = new Gtk.TreeView();

			Gtk.TreeViewColumn objectColumn = new Gtk.TreeViewColumn();
			objectColumn.Title = "object";
			Gtk.TreeViewColumn metrixColumn = new Gtk.TreeViewColumn();
			metrixColumn.Title = "metrix";

			// Add the columns to the TreeView
			tree.AppendColumn(objectColumn);
			tree.AppendColumn(metrixColumn);

			Gtk.ListStore listStore = new Gtk.ListStore(typeof(string), typeof(string));
			foreach (var item in padDoc.CurrentPath)
			{
				listStore.AppendValues(item.Markup, "0");
			}

			tree.Model = listStore;
			Gtk.CellRendererText artistNameCell = new Gtk.CellRendererText();
			objectColumn.PackStart(artistNameCell, true);
			Gtk.CellRendererText songTitleCell = new Gtk.CellRendererText();
			metrixColumn.PackStart(songTitleCell, true);
			 objectColumn.AddAttribute(artistNameCell, "text", 0);
			metrixColumn.AddAttribute(songTitleCell, "text", 1);
			return tree;
		}
		void ReleaseDoc()
		{
			RemoveBoxChild();
			//if (currentOutlineDoc != null)
			//	currentOutlineDoc.ReleaseOutlineWidget();
			//currentOutlineDoc = null;
		}

		void SetWidget(Gtk.Widget widget)
		{
			if (widget == null)
				widget = new WrappedCentreLabel(MonoDevelop.Core.GettextCatalog.GetString(
					"An outline is not available for the current document."));
			RemoveBoxChild();
			box.Add(widget);
			widget.Show();
			box.Show();
		}

		void SetToolbarWidgets(IEnumerable<Widget> toolbarWidgets)
		{
			foreach (var old in toolbar.Children)
				toolbar.Remove(old);
			bool any = false;
			if (toolbarWidgets != null)
			{
				foreach (var w in toolbarWidgets)
				{
					w.Show();
					toolbar.Add(w);
					any = true;
				}
			}
			toolbar.Visible = any;
		}

		void RemoveBoxChild()
		{
			Gtk.Widget curChild = box.Child;
			if (curChild != null)
				box.Remove(curChild);
		}

		private class WrappedCentreLabel : Gtk.Widget
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
}
