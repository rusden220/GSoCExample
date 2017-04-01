using MonoDevelop.Components.Commands;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui;

using System;
using System.Collections.Generic;
using Gtk;
using MonoDevelop.Components;
using MonoDevelop.Ide.Gui.Components;

namespace DateInserter
{
	class InsertDateHandler : CommandHandler
	{
		private void AddPad()
		{
			DocumentOutlinePad dop = new DocumentOutlinePad();
		
			try
			{
				IdeApp.Workbench.AddPad(dop, "DateInserter.DocumentOutlinePad", "Code Metrix", "", null);
			}
			catch (Exception ex)
			{
				IdeApp.Workbench.ActiveDocument.Editor.InsertAtCaret(ex.Message);
			}
		}
		protected override void Run()
		{
			AddPad();
			var editor = IdeApp.Workbench.ActiveDocument.Editor;

			var t = IdeApp.Workbench.Pads.DocumentOutlinePad.Content;

			var tt = IdeApp.Workbench.ActiveDocument.GetContent<MonoDevelop.Ide.Gui.Content.IPathedDocument>();

			foreach (var item in tt.CurrentPath)
			{
				editor.InsertAtCaret("//" + item.Markup);
			}

		}
		protected override void Update(CommandInfo info)
		{
			
			info.Enabled = IdeApp.Workbench.ActiveDocument?.Editor != null;
		}
	}
}