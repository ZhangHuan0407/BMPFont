using HotFix.UI;

namespace HotFix
{
    public static class MessageBox
    {
        public static void Show(string content)
        {
            if (MessageWindow.ActiveMessageWindow is null)
                MessageWindow.OpenWindow();

            MessageWindow.ActiveMessageWindow.AppendLineAndRefresh(content);
        }
    }
}