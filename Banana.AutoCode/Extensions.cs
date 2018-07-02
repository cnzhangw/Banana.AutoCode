using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Banana.AutoCode
{
    public static class Extensions
    {
        
        public static string Clear(this string that)
        {
            if (that.IsNullOrWhiteSpace())
            {
                return string.Empty;
            }
            return that.Trim();
        }

        delegate DialogResult AlertDelegate(string message, MessageBoxButtons button, MessageBoxIcon icon, Action action);
        private static DialogResult _Alert(string message, MessageBoxButtons button, MessageBoxIcon icon, Action action)
        {
            var r = MessageBox.Show(message, "系统提示", button, icon);
            if (action != null && (r == DialogResult.OK || r == DialogResult.Yes))
            {
                action.Invoke();
            }
            return r;
        }
        public static DialogResult Alert(this string that, Control control,
            MessageBoxButtons button = MessageBoxButtons.OK,
            MessageBoxIcon icon = MessageBoxIcon.Information, Action action = null)
        {
            DialogResult result;
            if (control.InvokeRequired)
            {
                result = (DialogResult)control.Invoke(new AlertDelegate(_Alert), that, button, icon, action);
            }
            else
            {
                result = _Alert(that, button, icon, action);
            }
            return result;
        }
    }
}
