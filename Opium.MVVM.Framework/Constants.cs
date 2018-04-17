using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opium.MVVM.Framework
{
    /// <summary>
    ///     Application-wide constants
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Generic message showing something is happening for a busy overlay
        /// </summary>
        public const string BEGIN_BUSY = "Opium.Begin.Busy";

        /// <summary>
        /// Generic message to end the busy overlay 
        /// </summary>
        public const string END_BUSY = "Opium.End.Busy";

        /// <summary>
        /// Initialization parameter name for specifying the logging level
        /// </summary>
        public const string INIT_PARAM_LOGLEVEL = "Opium.LogLevel";

        /// <summary>
        /// Parameter to set a view as a window
        /// </summary>
        public const string AS_WINDOW = "Opium.OOBWindow";

        /// <summary>
        /// Window width
        /// </summary>
        public const string WINDOW_WIDTH = "Opium.WindowWidth";

        /// <summary>
        /// Window height
        /// </summary>
        public const string WINDOW_HEIGHT = "Opium.WindowHeight";

        /// <summary>
        /// Set window title
        /// </summary>
        public const string WINDOW_TITLE = "Opium.Title";

        /// <summary>
        /// Set a reference to the window
        /// </summary>
        public const string WINDOW_REFERENCE = "Opium.WindowReference";
    }
}
