using System.Runtime.InteropServices;

namespace PlantOps;

internal sealed class XamlMessageFilter : IMessageFilter
{
    // WinForms owns the message loop (including modal dialogs). Give the Windows
    // App SDK UI stack first refusal so island keyboard/pointer input is translated.
    public bool PreFilterMessage(ref Message m)
    {
        var message = new NativeMessage { Hwnd = m.HWnd, Message = (uint)m.Msg, WParam = m.WParam, LParam = m.LParam };
        return ContentPreTranslateMessage(ref message);
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeMessage
    {
        public nint Hwnd;
        public uint Message;
        public nint WParam;
        public nint LParam;
        public uint Time;
        public int X;
        public int Y;
        public uint Private;
    }

    [DllImport("Microsoft.UI.Windowing.Core.dll", ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool ContentPreTranslateMessage(ref NativeMessage message);
}
