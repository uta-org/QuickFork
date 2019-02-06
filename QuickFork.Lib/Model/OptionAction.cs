using System;

namespace QuickFork.Lib.Model
{
    public class OptionAction
    {
        public Action<int> Action { get; set; }
        public string Caption { get; set; }

        private OptionAction()
        {
        }

        public OptionAction(string caption, Action<int> action)
        {
            Caption = caption;
            Action = action;
        }
    }

    public class OptionAction<T>
    {
        public Action<T> Action { get; set; }
        public string Caption { get; set; }

        private OptionAction()
        {
        }

        public OptionAction(string caption, Action<T> action)
        {
            Caption = caption;
            Action = action;
        }
    }
}