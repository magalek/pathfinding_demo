using System.Collections.Generic;

namespace Game
{
    public class Options
    {
        public readonly Option<bool> PathDrawingOption = new Option<bool>("Path Drawing Enabled", true);
    }
}