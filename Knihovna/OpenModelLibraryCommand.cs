using Rhino;
using Rhino.Commands;
using Rhino.UI;

namespace Knihovna
{
    public class OpenModelLibraryCommand : Command
    {
        public override string EnglishName => "OpenModelLibrary";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            var panelId = ModelLibraryPanel.PanelId;

            // Ověření, zda je panel již otevřen
            if (Panels.IsPanelVisible(panelId))
                return Result.Success;

            // Otevření panelu
            Panels.OpenPanel(panelId);

            return Result.Success;
        }
    }
}
