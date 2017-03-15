using MaterialDesignThemes.Wpf;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MaterialDesignWindowsDialogs {
    /// <summary>
    /// Interaction logic for MDMessageBox.xaml
    /// </summary>
    public partial class MDMessageBox : Window {
        public enum DialogType { YesNo, Ok, OkCancel }


        public MDMessageBox(IntPtr hWnd, string text, string caption, DialogType type) {
            InitializeComponent();

            SetWindowSize(hWnd);

            switch(type) {
                case DialogType.Ok:
                    DialogOK(text, caption);
                    break;
                case DialogType.YesNo:
                    break;
                case DialogType.OkCancel:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        //Set the Window Size
        private void SetWindowSize(IntPtr hWnd) {
            if(hWnd == IntPtr.Zero) {
                //Full Primary Screen Working Area
                Left = SystemParameters.WorkArea.Left;
                Top = SystemParameters.WorkArea.Top;
                Width = SystemParameters.WorkArea.Width;
                Height = SystemParameters.WorkArea.Height;
            } else {
                //Owner Window Size
                RECT rct;
                if(!GetWindowRect(new HandleRef(this, hWnd), out rct)) {
                    MessageBox.Show("ERROR");
                    return;
                }

                Left = rct.Left;
                Top = rct.Top;
                Width = rct.Right - rct.Left + 1;
                Height = rct.Bottom - rct.Top + 1;
            }
        }


        //Close the Material Design Dialog
        private void CloseDialog() {
            DialogHost.CloseDialogCommand.Execute(null, DialogHost);
        }

        private async void DialogOK(string text, string caption) {
            CloseDialog();

            StackPanel vPanel = new StackPanel {
                Margin = new Thickness(10),
                MinWidth = 150,
                MinHeight = 70
            };


            Label captionLabel = new Label {
                Content = caption,
                HorizontalAlignment = HorizontalAlignment.Left,
                FontWeight = FontWeights.Bold
            };

            Label contentLabel = new Label {
                Content = text,
                HorizontalAlignment = HorizontalAlignment.Left
            };

            Button ok = new Button {
                Content = "Ok",
                Foreground = Brushes.Gray,
                Width = 60,
                Margin = new Thickness(3),
                Style = (Style)FindResource("MaterialDesignFlatButton"),
                HorizontalAlignment = HorizontalAlignment.Right
            };
            ok.Click += delegate {
                CloseDialog();
            };

            vPanel.Children.Add(captionLabel);
            vPanel.Children.Add(contentLabel);
            vPanel.Children.Add(ok);

            await DialogHost.ShowDialog(vPanel);
        }


        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(HandleRef hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }
    }
}
