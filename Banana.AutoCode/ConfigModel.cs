using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banana.AutoCode
{
    public class ConfigModel
    {
        public const int DRIVER_TYPE_ADONET = 0;
        public const int DRIVER_TYPE_PETAPOCO = 1;
        public const int DRIVER_TYPE_NHIBERNATE = 2;

        public string[] using_model_array
        {
            get
            {
                if (this.using_model.IsNullOrWhiteSpace()) return new string[0];
                return this.using_model.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToArray();
            }
        }
        public string[] using_bll_array
        {
            get
            {
                if (this.using_bll.IsNullOrWhiteSpace()) return new string[0];
                return this.using_bll.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToArray();
            }
        }


        public string output { get; set; }

        public string connection_string { get; set; }
        public string connection_providername { get; set; }
        public string connection_database { get; set; }

        public string author { get; set; }
        /// <summary>
        /// 0-none 1-petapoco 2-nhibernate
        /// </summary>
        public int driver_type { get; set; }

        public string namespace_model { get; set; }
        public string using_model { get; set; }

        public string namespace_bll { get; set; }
        public string using_bll { get; set; }

        public bool constructor { get; set; }

        public string tablePrefix { get; set; }
    }
}
