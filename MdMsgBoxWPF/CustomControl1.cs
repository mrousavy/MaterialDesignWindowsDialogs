using System.Windows;
using System.Windows.Controls;

namespace MdMsgBoxWPF {
    public class CustomControl1 : Control {
        static CustomControl1() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomControl1), new FrameworkPropertyMetadata(typeof(CustomControl1)));
        }
    }
}
