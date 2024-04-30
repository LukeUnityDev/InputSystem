#if UNITY_EDITOR || UNITY_ANDROID

using System;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;

namespace UnityEngine.InputSystem.Android.LowLevel
{
    // <summary>
    /// Input device that represents an Android handheld device.
    /// </summary>
    [InputControlLayout(displayName = "Android Device", hideInUI = true)]
    public class AndroidDevice : InputDevice
    {
        [InputControl(synthetic = true)]
        public ButtonControl button { get; protected set; }
        
        /// <inheritdoc />
        protected override void OnAdded()
        {
            base.OnAdded();
            InputSystem.onSettingsChange += OnSettingsChange;
        }

        /// <inheritdoc />
        protected override void OnRemoved()
        {
            InputSystem.onSettingsChange -= OnSettingsChange;
            base.OnRemoved();
        }
        
        private void OnSettingsChange()
        {
            backButtonLeavesApp = InputSystem.settings.android.backButtonLeavesApp;
        }
        
        private bool backButtonLeavesApp
        {
            get 
            {
                var command = GetCustomCommand.Create();
                if (ExecuteCommand(ref command) >= 0)
                {
                    return command.value >= 1;
                }
                return false;

            }
            set
            {
                var command = SetCustomCommand.Create((uint)AndroidCustomCommand.BackButtonLeavesApp, Convert.ToUInt32(value));
                ExecuteCommand(ref command);
            }
        }

        private enum AndroidCustomCommand
        {
            BackButtonLeavesApp = 0,
        }
    }
}
#endif