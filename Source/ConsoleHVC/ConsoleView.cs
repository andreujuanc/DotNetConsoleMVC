using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleHVC
{
    public abstract class ConsoleView
    {
        public dynamic Model { get; set; }
        public List<ConsoleControl> Controls { get; set; } = new List<ConsoleControl>();
        public void DrawView()
        {
            Console.Clear();
            OnDrawView();
            DrawControls();
        }
        private void DrawControls()
        {
            foreach (var control in Controls)
            {
                control.OnRenderControl();
            }
        }
        public abstract void OnDrawView();
        public abstract void OnInit();
    }
}
