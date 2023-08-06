using Rhino;
using Rhino.PlugIns;

namespace Knihovna
{
    public class KnihovnaPlugin : Rhino.PlugIns.PlugIn
    {
        public KnihovnaPlugin()
        {
            Instance = this;
        }

        ///<summary>Gets the only instance of the KnihovnaPlugin plug-in.</summary>
        public static KnihovnaPlugin Instance { get; private set; }

        protected override LoadReturnCode OnLoad(ref string errorMessage)
        {
            ModelLibraryPanel.Register();
            return base.OnLoad(ref errorMessage);
        }

        // další kód...
    }
}