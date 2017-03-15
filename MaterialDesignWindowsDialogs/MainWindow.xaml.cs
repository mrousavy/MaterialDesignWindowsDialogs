using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;

namespace MaterialDesignWindowsDialogs {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow {
        private string _path = Path.Combine(System.Reflection.Assembly.GetEntryAssembly().Location, "MdMsgBox.dll");


        public MainWindow() {
            InitializeComponent();

            dllBox.Text = _path;
        }

        //-- ORIGINAL WINDOWS API MESSAGEBOX SIGNATURE
        //int WINAPI MessageBox(
        //  _In_opt_ HWND    hWnd,
        //  _In_opt_ LPCTSTR lpText,
        //  _In_opt_ LPCTSTR lpCaption,
        //  _In_ UINT    uType
        //);

        private void InjectClick(object sender, RoutedEventArgs e) {
            try {
                MdMsgBox.Injector.Inject(int.Parse(pidBox.Text), _path);
            } catch(Exception ex) {
                MessageBox.Show(ex.Message, "Error injecting");
            }
        }
        private void OpenClick(object sender, RoutedEventArgs e) {
            try {
                OpenFileDialog ofd = new OpenFileDialog {
                    Title = "Select MdMsgBox.dll",
                    Filter = "DLL Files|*.dll",
                    InitialDirectory = Path.GetDirectoryName(_path)
                };

                if(ofd.ShowDialog(this) == true) {
                    _path = ofd.FileName;
                    dllBox.Text = _path;
                }
            } catch(Exception ex) {
                MessageBox.Show(ex.Message, "Error could not load DLL");
                Close();
            }
        }


    }


    public class InjectorInterface : MarshalByRefObject {
        public void IsInstalled() {
            Console.WriteLine(@"Injected");
        }

        public void Debug(string msg) {
            Console.WriteLine(msg);
        }

        public void ReportException(Exception inInfo) {
            Console.WriteLine(inInfo.ToString());
        }
    }

}
