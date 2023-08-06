using Rhino.UI;
using Eto.Forms;

namespace Knihovna
{
    [System.Runtime.InteropServices.Guid("A4B55802-6528-44C2-B0F4-E88E4C5634F2")]
    public class ModelLibraryPanel : Panel
    {
        public static System.Guid PanelId => typeof(ModelLibraryPanel).GUID;
        private ModelLibraryControl _modelLibraryControl;

        public ModelLibraryPanel(uint documentSerialNumber)
        {
            _modelLibraryControl = new ModelLibraryControl();
            Content = _modelLibraryControl;
        }

        public string PanelCaption => "Model Library";

        public System.Drawing.Bitmap PanelBitmap => null;

        public static void Register()
        {
            Panels.RegisterPanel(KnihovnaPlugin.Instance, typeof(ModelLibraryPanel), "Model Library", null);
        }
    }
}

