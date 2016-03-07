using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleHVC
{
    public abstract class ConsoleControl
    {
        public List<ConsoleControl> Controls { get; set; }
        internal void Render()
        {
            foreach (var control in Controls)
            {
                control.Render();
            }
        }
        public abstract void OnRenderControl();
    }
}
