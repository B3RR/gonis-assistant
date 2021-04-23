using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gonis.Assistant.Core.Bots.Interfaces
{
    /// <summary>
    /// Interface Bot Service
    /// </summary>
    public interface IBotService
    {
        bool IsStarted { get; }

        /// <summary>
        /// Start bot
        /// </summary>
        void Start();

        /// <summary>
        /// Stop bot
        /// </summary>
        void Stop();
    }
}
