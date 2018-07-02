using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Banana.AutoCode
{
    public class BaseForm : Form
    {
        public BaseForm()
        {
            
        }

        delegate void MsgDelegate(string msg);
        protected void Alert(string msg)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MsgDelegate(InnerAlert), msg);
            }
            else
            {
                InnerAlert(msg);
            }
        }
        protected void Error(string msg)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MsgDelegate(InnerError), msg);
            }
            else
            {
                InnerError(msg);
            }
        }
        private void InnerError(string msg)
        {
            MessageBox.Show(msg, "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void InnerAlert(string msg)
        {
            MessageBox.Show(msg, "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
