#if UNITY_EDITOR || UNITY_IOS || UNITY_TVOS
using System;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;

namespace UnityEngine.InputSystem.iOS
{
    [InputControlLayout(stateType = typeof(ScreenKeyboardState))]
    public class iOSScreenKeyboard : ScreenKeyboard
    {
        internal delegate void OnTextChangedDelegate(int deviceId, string text);

        internal delegate void OnStatusChangedDelegate(int deviceId, ScreenKeyboardStatus status);

        internal delegate void OnSelectionChangedDelegate(int deviceId,int start, int length);

        [StructLayout(LayoutKind.Sequential)]
        private struct iOSScreenKeyboardCallbacks
        {
            internal int deviceId;
            internal OnTextChangedDelegate onTextChanged;
            internal OnStatusChangedDelegate onStatusChanged;
            internal OnSelectionChangedDelegate onSelectionChanaged;
        }

        [DllImport("__Internal")]
        private static extern void _iOSScreenKeyboardShow(ref ScreenKeyboardShowParams showParams, int sizeOfShowParams, ref iOSScreenKeyboardCallbacks callbacks, int sizeOfCallbacks);

        [DllImport("__Internal")]
        private static extern Rect _iOSScreenKeyboardOccludingArea();

        [DllImport("__Internal")]
        private static extern void _iOSScreenKeyboardSetInputFieldText(string text);

        [DllImport("__Internal")]
        private static extern string _iOSScreenKeyboardGetInputFieldText();
        
        [DllImport("__Internal")]
        private static extern void _iOSScreenKeyboardSetSelection(int start, int length);

        [DllImport("__Internal")]
        private static extern long _iOSScreenKeyboardGetSelection();

        [MonoPInvokeCallback(typeof(OnTextChangedDelegate))]
        private static void OnTextChangedCallback(int deviceId, string text)
        {
            var screenKeyboard = (iOSScreenKeyboard)InputSystem.GetDeviceById(deviceId);
            if (screenKeyboard == null)
                throw new Exception("OnTextChangedCallback: Failed to get iOSScreenKeyboard instance");

            screenKeyboard.ReportInputFieldChange(text);
        }

        [MonoPInvokeCallback(typeof(OnStatusChangedDelegate))]
        private static void OnStatusChangedCallback(int deviceId, ScreenKeyboardStatus status)
        {
            var screenKeyboard = (iOSScreenKeyboard)InputSystem.GetDeviceById(deviceId);
            if (screenKeyboard == null)
                throw new Exception("OnStatusChangedCallback: Failed to get iOSScreenKeyboard instance");

            screenKeyboard.ReportStatusChange(status);
        }
        
        [MonoPInvokeCallback(typeof(OnSelectionChangedDelegate))]
        private static void OnSelectionChangedCallback(int deviceId, int start, int length)
        {
            var screenKeyboard = (iOSScreenKeyboard)InputSystem.GetDeviceById(deviceId);
            if (screenKeyboard == null)
                throw new Exception("OnStatusChangedCallback: Failed to get iOSScreenKeyboard instance");

            screenKeyboard.ReportSelectionChange(start,length);
        }

        protected override void InternalShow()
        {
            var callbacks = new iOSScreenKeyboardCallbacks()
            {
                deviceId = deviceId,
                onTextChanged = OnTextChangedCallback,
                onStatusChanged = OnStatusChangedCallback,
                onSelectionChanaged = OnSelectionChangedCallback
            };
            _iOSScreenKeyboardShow(ref m_ShowParams, Marshal.SizeOf(m_ShowParams), ref callbacks, Marshal.SizeOf(callbacks));
        }

        protected override void InternalHide()
        {
        }

        public override string inputFieldText
        {
            get
            {
                return _iOSScreenKeyboardGetInputFieldText();
            }
            set
            {
                _iOSScreenKeyboardSetInputFieldText(value);
            }
        }
        
        public override RangeInt selection
        {
            get
            {
                var combined = _iOSScreenKeyboardGetSelection();
                unchecked
                {
                    return new RangeInt((int)(0xFFFFFFFF & combined), (int)(combined >> 32));
                }
            }
            set
            {
                _iOSScreenKeyboardSetSelection(value.start, value.length);
            }
        }

        public override Rect occludingArea => _iOSScreenKeyboardOccludingArea();
    }
}
#endif
