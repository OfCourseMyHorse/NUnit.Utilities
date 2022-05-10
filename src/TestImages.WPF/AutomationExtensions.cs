using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Automation.Provider;

namespace System.Windows
{
    // none of these methods work on an isolated control/window,
    // TODO: Figure out how to inject input into an isolated control.
    
    static class AutomationExtensions
    {
        public static void RaiseMouseClick(this UIElement element)
        {
            // https://stackoverflow.com/questions/6325956/how-can-i-raise-a-mouse-event-in-wpf-c-sharp-with-specific-coordinates

            // Copied the snippet of code from the link above, just to help future readers
            var e = new Input.MouseEventArgs(Input.Mouse.PrimaryDevice, 0);
            e.RoutedEvent = Input.Mouse.MouseEnterEvent;

            element.RaiseEvent(e);
            // Or
            // Input.InputManager.Current.ProcessInput(e);
        }

        public static void AutomationInvoke(this UIElement element)
        {
            var peer = Automation.Peers.UIElementAutomationPeer.CreatePeerForElement(element);

            if (peer.GetPattern(Automation.Peers.PatternInterface.Invoke) is IInvokeProvider invokeProvider)
            {
                invokeProvider.Invoke();
            }
        }

        public static IRawElementProviderSimple[] AutomationGetSelection(this UIElement element)
        {
            var peer = Automation.Peers.UIElementAutomationPeer.CreatePeerForElement(element);

            if (peer.GetPattern(Automation.Peers.PatternInterface.Selection) is ISelectionProvider selectProvider)
            {
                return selectProvider.GetSelection();
            }

            return null;
        }

    }
}
