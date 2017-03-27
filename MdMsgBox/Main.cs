using EasyHook;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace MdMsgBox {
    public class Main : IEntryPoint {
        private readonly InjectorInterface _interface;
        private Stack<string> _queue = new Stack<string>();


        public Main(RemoteHooking.IContext inContext, string inChannelName) {
            _interface = RemoteHooking.IpcConnectClient<InjectorInterface>(inChannelName);
        }

        public void Run(RemoteHooking.IContext inContext, string inChannelName) {
            LocalHook hook = LocalHook.Create(
                    LocalHook.GetProcAddress("user32.dll", "MessageBoxW"),
                    new DMessageBox(MessageBoxHook),
                    this);

            hook.ThreadACL.SetExclusiveACL(new Int32[] { 0 });

            //IEnumerable<int> pidsEnum = Process.GetProcesses().Select(p => p.Id);
            //int[] pids = pidsEnum.ToArray();

            MessageBox(IntPtr.Zero, "Hook Initialized", "Success", (int)Modifiers.Ok);

            try {
                while(true) {
                    System.Threading.Thread.Sleep(100);

                    if(_queue.Count > 0) {
                        string[] package = null;
                        lock(_queue) {
                            package = _queue.ToArray();
                            _queue.Clear();
                        }
                        _interface.Debug(package[0]);
                    }
                }
            } catch {
                // ignored
            }
        }


















        //-- ORIGINAL WINDOWS API MESSAGEBOX SIGNATURE
        //int WINAPI MessageBox(
        //  _In_opt_ HWND    hWnd,
        //  _In_opt_ LPCTSTR lpText,
        //  _In_opt_ LPCTSTR lpCaption,
        //  _In_ UINT    uType
        //);


        //user32 Import -- WinAPI
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int MessageBox(IntPtr hWnd, string text, string caption, int options);

        //Delegate Signature for user32 Import
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate int DMessageBox(IntPtr hWnd, string text, string caption, int options);

        //Delegate Signature implementation
        public static int MessageBoxHook(IntPtr hWnd, string text, string caption, int options) {
            Main This = (Main)HookRuntimeInfo.Callback;
            lock(This._queue) {
                This._queue.Push($"HWND: {hWnd} | Text: {text} | Caption: {caption} | Options: {options}");
            }

            Thread thread = new Thread(() => new MDMessageBox(IntPtr.Zero, text, caption, MDMessageBox.DialogType.Ok).Show());
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

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
            AbortRetryignore = 0x00000002,
            CancelTryContinue = 0x00000006,
            Help = 0x00004000,
            Ok = 0x00000000,
            OkCancel = 0x00000001,
            RetryCancel = 0x00000005,
            YesNo = 0x00000004,
            YesNoCancel = 0x00000003
        }
    }
}