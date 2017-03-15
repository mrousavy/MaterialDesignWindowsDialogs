using EasyHook;
using Microsoft.Win32;
using System;
using System.Runtime.Remoting;
using System.Windows;

namespace MaterialDesignWindowsDialogs {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow {
        private string _path = "MdMsgBox.dll";


        public MainWindow() {
            InitializeComponent();

            Loaded += delegate {
                try {
                    OpenFileDialog ofd = new OpenFileDialog {
                        Title = "Select MdMsgBox.dll",
                        Filter = "DLL Files|*.dll",
                        InitialDirectory = @"C:\"
                    };

                    if(ofd.ShowDialog(this) == true) {
                        _path = ofd.FileName;
                    }
                } catch(Exception ex) {
                    MessageBox.Show(ex.Message, "Error could not load DLL");
                    Close();
                }
            };
        }

        //-- ORIGINAL WINDOWS API MESSAGEBOX SIGNATURE
        //int WINAPI MessageBox(
        //  _In_opt_ HWND    hWnd,
        //  _In_opt_ LPCTSTR lpText,
        //  _In_opt_ LPCTSTR lpCaption,
        //  _In_ UINT    uType
        //);

        //Return Values of Message Box
        public enum ReturnValues {
            Abort = 3,
            Cancel = 2,
            Continue = 11,
            Ignore = 5,
            No = 7,
            Yes = 6,
            Ok = 1,
            Retry = 4,
            TryAgain = 10
        }

        //Modifiers of Message Box
        public enum Modifiers {
            AbortRetryignore = 0x00000002,
            CancelTryContinue = 0x00000006,
            Help = 0x00004000,
            Ok = 0x00000000,
            OkCancel = 0x00000001,
            RetryCancel = 0x00000005,
            YesNo = 0x00000004,
            YesNoCancel = 0x00000003
        }

        private void InjectClick(object sender, RoutedEventArgs e) {
            try {
                string channelName = null;

                RemoteHooking.IpcCreateServer<InjectorInterface>(ref channelName, WellKnownObjectMode.SingleCall);

                RemoteHooking.Inject(
                    int.Parse(pidBox.Text),
                    InjectionOptions.DoNotRequireStrongName,
                    _path,
                    _path,
                    channelName);
            } catch(Exception ex) {
                MessageBox.Show(ex.Message, "Error injecting");
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
