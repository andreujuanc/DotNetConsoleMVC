using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleHVC
{
    public abstract class ConsoleHandler
    {
        public ConsoleController Controller { get; private set; }
        private Dictionary<string, ConsoleController> ControllerList = new Dictionary<string, ConsoleController>();
        private Stack<string> NavigationHistory = new Stack<string>();
        private bool _isConfigured = false;
        private bool _exit = false;
        public void OnConfigure()
        {
            if (!_isConfigured)
            {
                if (Controller == null)
                {
                    NavigateTo(ControllerList.FirstOrDefault().Key);
                }
            }
            _isConfigured = true;
        }

        public void NavigateTo(string controllerName)
        {
            if (!ControllerList.ContainsKey(controllerName))
            {
                throw new InvalidOperationException("Controller not found");
            }
            else
            {
                NavigationHistory.Push(controllerName);
                Controller = ControllerList[controllerName];
                Controller.Init();
            }
        }

        public void Run()
        {
            if (!_isConfigured) throw new InvalidOperationException("Handler is not configured.");
            while (DoLoop()) ;
        }

        private bool DoLoop()
        {
            
            if (!Controller.WaitForInput())
            {
                if (ControllerList.First().Value == Controller) return false;
                NavigationHistory.Pop();
                var last = NavigationHistory.Pop();
                NavigateTo(last);
            }
            if (_exit) return false;
            return true;
        }
        public  void AddController<ControllerType, ViewType>(string name, int? index = null) where ControllerType : ConsoleController where ViewType : ConsoleView
        {
            var controller = Activator.CreateInstance<ControllerType>();
            controller.View = Activator.CreateInstance<ViewType>();
            controller.Handler = this;
            ControllerList.Add(name, controller);
            _isConfigured = false;
        }
        internal void Exit()
        {
            _exit = true;
        }
    }
}
