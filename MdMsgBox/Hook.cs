using EasyHook;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace MdMsgBox {
    public class Hook : IEntryPoint {
        private InjectorInterface _interface;


        public Hook(RemoteHooking.IContext inContext, string inChannelName) {
            _interface = RemoteHooking.IpcConnectClient<InjectorInterface>(inChannelName);
        }

        public void Run(RemoteHooking.IContext inContext, string inChannelName) {
            LocalHook hook = LocalHook.Create(
                LocalHook.GetProcAddress("user32.dll", "MessageBoxW"),
                new DMessageBox(MessageBoxHook),
                this);

            IEnumerable<int> pidsEnum = Process.GetProcesses().Select(p => p.Id);
            int[] pids = pidsEnum.ToArray();

            hook.ThreadACL.SetExclusiveACL(new[] { 0 });

            MessageBox(IntPtr.Zero, "Hook Initialized", "Success", (int)Modifiers.MB_OK);
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
            new MDMessageBox(IntPtr.Zero, text, caption, MDMessageBox.DialogType.Ok).Show();
            //return MessageBox(hWnd, text, caption, options);
            return (int)ReturnValues.Ok;
        }


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





    public class InjectorInterface : MarshalByRefObject {
        public void IsInstalled() {
            Console.WriteLine("Injected ;).\r\n");
        }

        public void debug(String msg) {
            Console.Out.WriteLine("{0}\r\n", msg);
        }

        public void ReportException(Exception InInfo) {
            Console.WriteLine("Error:\r\n" + InInfo.ToString());
        }
    }

}
