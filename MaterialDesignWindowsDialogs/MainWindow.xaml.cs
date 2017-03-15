using EasyHook;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace MaterialDesignWindowsDialogs {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();

            this.Loaded += delegate {
                LocalHook hook = LocalHook.Create(
                    LocalHook.GetProcAddress("user32.dll", "MessageBoxW"),
                    new DMessageBox(MessageBoxHook),
                    this);

                hook.ThreadACL.SetInclusiveACL(new[] { 0 });

                WindowInteropHelper wih = new WindowInteropHelper(this);
                IntPtr hWnd = wih.Handle;

                MessageBox(hWnd, "Hallo", "Caption", (int)Modifiers.MB_OK);
            };
        }

        //-- ORIGINAL WINDOWS API MESSAGEBOX SIGNATURE
        //int WINAPI MessageBox(
        //  _In_opt_ HWND    hWnd,
        //  _In_opt_ LPCTSTR lpText,
        //  _In_opt_ LPCTSTR lpCaption,
        //  _In_ UINT    uType
        //);


        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int MessageBox(IntPtr hWnd, string text, string caption, int options);


        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate int DMessageBox(IntPtr hWnd, string text, string caption, int options);


        private static int MessageBoxHook(IntPtr hWnd, string text, string caption, int options) {
            MessageBox(hWnd, "HOOKED MSGBOX :)", "my caption", options);
            return MessageBox(hWnd, text, caption, options);
        }


        //Return Values of Message Box
        public enum ReturnValues {
            IDABORT = 3,
            IDCANCEL = 2,
            IDCONTINUE = 11,
            IDIGNORE = 5,
            IDNO = 7,
            IDOK = 1,
            IDRETRY = 4,
            IDTRYAGAIN = 10,
            IDYES = 6
        }

        //Modifiers of Message Box
        public enum Modifiers {
            MB_ABORTRETRYIGNORE = 0x00000002,
            MB_CANCELTRYCONTINUE = 0x00000006,
            MB_HELP = 0x00004000,
            MB_OK = 0x00000000,
            MB_OKCANCEL = 0x00000001,
            MB_RETRYCANCEL = 0x00000005,
            MB_YESNO = 0x00000004,
            MB_YESNOCANCEL = 0x00000003
        }
    }
}
