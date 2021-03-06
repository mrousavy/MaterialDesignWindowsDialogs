﻿using EasyHook;
using System;
using System.Runtime.Remoting;

namespace MdMsgBox {

    //The Interface being passed around on Injecting
    public class InjectorInterface : MarshalByRefObject {
        public void IsInstalled(int inClientPid) {
            Console.WriteLine($"Injected into {inClientPid}");
        }

        public void ErrorHandler(Exception ex) {
            Console.WriteLine($"Error: {ex.Message}");
        }

        public void Debug(string message) {
            Console.WriteLine(message);
        }
    }

    //The DLL Injector
    public static class Injector {
        //Inject DLL into Program
        public static void Inject(int processId, string dllPath = "MdMsgBox.dll") {
            string channelName = null;

            RemoteHooking.IpcCreateServer<InjectorInterface>(ref channelName, WellKnownObjectMode.SingleCall);

            RemoteHooking.Inject(
               processId,
               InjectionOptions.DoNotRequireStrongName,
               dllPath,
               dllPath,
                channelName);
        }

    }
}
