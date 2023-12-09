using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace OctopusEnergyBillViewer.Presentation.WPF.Dialogs;

/// <summary>
/// メッセージボックスEX
/// </summary>
public static partial class MessageBoxEx
{
    /// <summary>
    /// 親ウィンドウ
    /// </summary>
    private static Window? owner_ = null;

    /// <summary>
    /// フックハンドル
    /// </summary>
    private static IntPtr hHook_ = IntPtr.Zero;

    public static Window? ActiveWindow => Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);

    /// <summary>
    /// メッセージボックスを表示する
    /// </summary>
    /// <param name="message">表示メッセージ</param>
    /// <param name="title">タイトル</param>
    /// <param name="button">ボタン</param>
    /// <param name="icon">アイコン</param>
    /// <returns>MessageBoxResult</returns>
    public static MessageBoxResult Show(
        string message,
        string title = "",
        MessageBoxButton button = MessageBoxButton.OK,
        MessageBoxImage icon = MessageBoxImage.None)
    {
        return Show(ActiveWindow, message, title, button, icon);
    }

    /// <summary>
    /// メッセージボックスを表示する
    /// </summary>
    /// <param name="owner">オーナーウィンドウ</param>
    /// <param name="message">表示メッセージ</param>
    /// <param name="title">タイトル</param>
    /// <param name="button">ボタン</param>
    /// <param name="icon">アイコン</param>
    /// <returns>MessageBoxResult</returns>
    public static MessageBoxResult Show(
        Window? owner,
        string message,
        string title = "",
        MessageBoxButton button = MessageBoxButton.OK,
        MessageBoxImage icon = MessageBoxImage.None)
    {
        if (owner is null) return MessageBox.Show(message, title, button, icon);
        if (owner.WindowState == WindowState.Minimized) return MessageBox.Show(owner, message, title, button, icon);
        owner_ = owner;
        var hwndSource = (HwndSource)PresentationSource.FromVisual(owner_);
        var hInstance = NativeMethods.GetWindowLongPtr(hwndSource.Handle, NativeMethods.GWL_HINSTANCE);
        var threadId = NativeMethods.GetCurrentThreadId();
        // フック設定
        hHook_ = NativeMethods.SetWindowsHookEx(NativeMethods.WH_CBT, new NativeMethods.HOOKPROC(HookProc), hInstance, threadId);
        return MessageBox.Show(owner_, message, title, button, icon);
    }

    private static IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode != NativeMethods.HCBT_ACTIVATE) return NativeMethods.CallNextHookEx(hHook_, nCode, wParam, lParam);
        var hwndSource = (HwndSource)PresentationSource.FromVisual(owner_);
        NativeMethods.GetWindowRect(hwndSource.Handle, out var rcForm);
        NativeMethods.GetWindowRect(wParam, out var rcMsgBox);

        // センター位置を計算、設定
        var x = rcForm.Left + (rcForm.Right - rcForm.Left) / 2 - (rcMsgBox.Right - rcMsgBox.Left) / 2;
        var y = rcForm.Top + (rcForm.Bottom - rcForm.Top) / 2 - (rcMsgBox.Bottom - rcMsgBox.Top) / 2;
        NativeMethods.SetWindowPos(wParam, 0, x, y, 0, 0, NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOZORDER | NativeMethods.SWP_NOACTIVATE);

        var result = NativeMethods.CallNextHookEx(hHook_, nCode, wParam, lParam);
        // フック解除
        NativeMethods.UnhookWindowsHookEx(hHook_);
        hHook_ = IntPtr.Zero;
        return result;
    }

    private partial class NativeMethods
    {
        [LibraryImport("user32.dll", EntryPoint = "GetWindowLongPtrA")]
        public static partial IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

        [LibraryImport("kernel32.dll")]
        public static partial IntPtr GetCurrentThreadId();

        [LibraryImport("user32.dll", EntryPoint = "SetWindowsHookExA")]
        public static partial IntPtr SetWindowsHookEx(int idHook, HOOKPROC lpfn, IntPtr hInstance, IntPtr threadId);

        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool UnhookWindowsHookEx(IntPtr hHook);

        [LibraryImport("user32.dll")]
        public static partial IntPtr CallNextHookEx(IntPtr hHook, int nCode, IntPtr wParam, IntPtr lParam);

        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        public delegate IntPtr HOOKPROC(int nCode, IntPtr wParam, IntPtr lParam);

        public const int GWL_HINSTANCE = -6;
        public const int WH_CBT = 5;
        public const int HCBT_ACTIVATE = 5;

        public const int SWP_NOSIZE = 0x0001;
        public const int SWP_NOZORDER = 0x0004;
        public const int SWP_NOACTIVATE = 0x0010;

        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
    }
}