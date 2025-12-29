using System.Windows;

namespace QuanLyTiecCuoi.Presentation.Helpers
{
    public interface IMessageBoxService
    {
        MessageBoxResult Show(string message);
        MessageBoxResult Show(string message, string caption);
        MessageBoxResult Show(string message, string caption, MessageBoxButton button);
        MessageBoxResult Show(string message, string caption, MessageBoxButton button, MessageBoxImage icon);
    }

    public class MessageBoxService : IMessageBoxService
    {
        public MessageBoxResult Show(string message)
        {
            return MessageBox.Show(message);
        }

        public MessageBoxResult Show(string message, string caption)
        {
            return MessageBox.Show(message, caption);
        }

        public MessageBoxResult Show(string message, string caption, MessageBoxButton button)
        {
            return MessageBox.Show(message, caption, button);
        }

        public MessageBoxResult Show(string message, string caption, MessageBoxButton button, MessageBoxImage icon)
        {
            return MessageBox.Show(message, caption, button, icon);
        }
    }
}
