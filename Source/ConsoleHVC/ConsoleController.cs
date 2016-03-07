using System;

namespace ConsoleHVC
{
    public abstract class ConsoleController
    {
        public ConsoleView View { get; set; }
        public ConsoleHandler Handler { get; set; }
        public void Init()
        {
            OnInit();
            this.View.OnInit();
        }
        public bool WaitForInput()
        {
            View.DrawView();
            return OnWaitForInput();
        }
        public abstract bool OnWaitForInput();
        public abstract void OnInit();

        public void Exit()
        {
            this.Handler.Exit();
        }

        public string ReadText(string promptText)
        {
            var original = Console.BackgroundColor;
            Console.SetCursorPosition(0, Console.BufferHeight - 1);
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(promptText);
            Console.CursorVisible = true;
            var result = Console.ReadLine();
            Console.BackgroundColor = original;
            return result;
        }
    }
}