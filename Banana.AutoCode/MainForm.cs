using Banana.Core;
using Banana.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Banana.AutoCode
{
    public partial class MainForm : Form//: BaseForm
    {
        private DataTable schemaTable;//表
        private int times = 0;

        ConfigModel cfg = null;

        public MainForm()
        {
            InitializeComponent();
            LoadConfigByJson();

            this.Load += MainForm_Load;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Icon = Properties.Resources.banana_64px;

            dataGridView1.MultiSelect = true;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.Font = new Font("Courier New", 10, FontStyle.Regular);

            txtConnectionString.ReadOnly = true;
            txtConnectionString.WordWrap = true;
            txtConnectionString.Font = new Font("Courier New", 10, FontStyle.Regular);

            menuConnect.Click += MenuConnect_Click;
            menuGenerateModel.Click += MenuGenerateModel_Click;
            menuOpenOutput.Click += MenuOpenOutput_Click;
            menuExit.Click += MenuExit_Click;
            menuAbout.Click += MenuAbout_Click;

            txtConnectionString.Text = cfg.connection_string;
        }

        private void LoadConfigByJson()
        {
            if (cfg == null)
                cfg = new ConfigModel();

            cfg.connection_string = Config.GetString("connection_string");
            cfg.connection_providername = Config.GetString("connection_provider");
            cfg.connection_database = Config.GetString("connection_database");
            cfg.driver_type = Config.GetInt("driver_type");
            if (!new int[] { 0, 1, 2 }.Contains(cfg.driver_type))
                cfg.driver_type = 1;

            cfg.namespace_model = Config.GetString("namespace_model") ?? "Entity";
            cfg.using_model = Config.GetString("using_model");
            cfg.namespace_bll = Config.GetString("namespace_bll") ?? "BLL";
            cfg.using_bll = Config.GetString("using_bll");
            cfg.output = Config.GetString("output") ?? "output";
            cfg.author = Config.GetString("author");
            if (cfg.author.IsNullOrWhiteSpace())
                cfg.author = "Generate by Banana";

            cfg.constructor = Config.GetValue<bool>("constructor");
            cfg.tablePrefix = Config.GetString("table_prefix");

            if (cfg.output.StartsWith("~/"))
            {
                cfg.output = cfg.output.TrimStart("~/".ToCharArray());
            }
            if (cfg.output.StartsWith("/"))
            {
                cfg.output = cfg.output.TrimStart('/');
            }
            if (cfg.output.Substring(1, 1) != ":")
            {
                cfg.output = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, cfg.output);
            }

        }

        private void MenuAbout_Click(object sender, EventArgs e)
        {
            new AboutForm().ShowDialog();
        }

        private void MenuExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void MenuOpenOutput_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(cfg.output))
            {
                "请先生成文件".Alert(this);
                return;
            }

            System.Diagnostics.Process.Start(cfg.output);
        }

        private void MenuGenerateModel_Click(object sender, EventArgs e)
        {
            var rows = dataGridView1.SelectedRows;
            if (rows.Count > 0)
            {
                if (!Directory.Exists(cfg.output))
                {
                    Directory.CreateDirectory(cfg.output);
                }
                //else
                //{
                //    DirectoryInfo di = new DirectoryInfo(cfg_output);
                //    var files = di.GetFiles("*.cs");
                //    if (files.Length > 0)
                //    {
                //        string that_backup = Path.Combine(cfg_output, "backup." + files[0].CreationTime.ToString("yyMMddHH"));
                //        if (!Directory.Exists(that_backup))
                //        {
                //            Directory.CreateDirectory(that_backup);
                //        }
                //        foreach (FileInfo fi in files)
                //        {
                //            string path = Path.Combine(that_backup, fi.Name);
                //            if (File.Exists(path))
                //            {
                //                new FileInfo(path).Delete();
                //                //var repeatFile = new FileInfo(path);
                //                //repeatFile.MoveTo(Path.Combine(that_backup, repeatFile.Name + "." + repeatFile.CreationTime.ToString("yyMMddHHmmss")));
                //            }
                //            fi.MoveTo(path);
                //        }
                //    }
                //}

                DataTable tableSchema = null;
                Sql sql = null;
                foreach (DataGridViewRow item in rows)
                {
                    string tableName = item.Cells[0].Value.ToString();
                    switch (cfg.connection_providername)
                    {
                        case "mysql":
                            sql = new Sql("select column_name as name,data_type as datatype,column_comment as comments");
                            sql.Append("from information_schema.columns");
                            sql.Append("where table_name=@0", tableName);
                            break;
                        case "oracle":
                            sql = new Sql("select a.column_name as name,a.data_type as datatype,b.comments as comments from user_tab_columns a");
                            sql.Append("inner join user_col_comments b on a.table_name = b.table_name and b.column_name = a.column_name");
                            sql.Append("where a.table_name = @0", tableName);
                            break;
                        default:
                            break;
                    }

                    tableSchema = DBService.Instance.GetDataTable(sql, tableName);
                    GenerateEntityCode(tableSchema, item.Cells[1].Value.ToString());
                    GenerateBusinessLayerCode(tableSchema, item.Cells[1].Value.ToString());
                }

                times++;
                if (times < 2)//避免多次打开
                {
                    MenuOpenOutput_Click(e, EventArgs.Empty);
                }
            }
            else
            {
                ("请先连接数据库并选择需要操作的对象").Alert(this, icon: MessageBoxIcon.Error);
            }
        }

        private void MenuConnect_Click(object sender, EventArgs e)
        {
            try
            {
                Sql sql = null;
                switch (cfg.connection_providername)
                {
                    case "mysql":
                        sql = new Sql("select table_name as '表名',table_comment as '说明'");
                        sql.Append("from information_schema.tables");
                        sql.Append("where table_type='base table'");
                        if (!cfg.connection_database.IsNullOrWhiteSpace())
                            sql.Append("and table_schema=@0", cfg.connection_database);
                        break;
                    case "oracle":
                        sql = new Sql("select a.table_name as \"表名\",b.comments as \"说明\"");
                        sql.Append("from user_tables a inner join user_tab_comments b on a.table_name=b.table_name");
                        break;
                    default:
                        break;
                }

                schemaTable = DBService.Instance.GetDataTable(sql);
                dataGridView1.DataSource = schemaTable;
                AutoSizeColumn(dataGridView1);
            }
            catch (Exception ex)
            {
                (ex.Message).Alert(this, icon: MessageBoxIcon.Error);
            }
            AutoSizeColumn(dataGridView1);
        }

        /// <summary>
        /// 生成entity
        /// </summary>
        /// <param name="table"></param>
        /// <param name="tableComment"></param>
        void GenerateEntityCode(DataTable table, string tableComment)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");

            if (cfg.using_model_array.Length > 0)
            {
                foreach (var item in cfg.using_model_array)
                {
                    sb.AppendLine("using " + item + ";");
                }
            }

            sb.AppendLine();

            sb.AppendLine("namespace " + cfg.namespace_model);
            sb.AppendLine("{");


            sb.AppendLine("    /// <summary>");
            sb.AppendLine("    /// " + tableComment);

            sb.AppendLine("    /// 作者：" + cfg.author);
            //Generate by Banana.AutoCode
            sb.AppendLine("    /// 时间：" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            sb.AppendLine("    /// </summary>");

            sb.AppendLine("    [Serializable]");

            #region 属性

            StringBuilder sbPart = new StringBuilder();
            bool intId = false;

            sbPart.AppendLine("    public class " + table.TableName.Replace(cfg.tablePrefix, "").ToPascal());
            sbPart.AppendLine("    {");

            if (cfg.constructor)
            {
                sbPart.AppendLine("        public " + table.TableName.Replace(cfg.tablePrefix, "").ToPascal() + "() { }");
                sbPart.AppendLine();
            }

            sbPart.AppendLine("        #region 属性");
            string columnName, comment, dataType = "";
            foreach (DataRow dataRow in table.Rows)
            {
                columnName = dataRow["name"].ToString();
                comment = dataRow["comments"].ToString().Trim();
                dataType = dataRow["datatype"].ToString().ToLower();

                sbPart.AppendLine();
                if (!comment.IsNullOrWhiteSpace())
                {
                    sbPart.AppendLine("        /// <summary>");
                    sbPart.AppendLine("        /// " + comment);
                    sbPart.AppendLine("        /// </summary>");
                }
                if (cfg.driver_type == ConfigModel.DRIVER_TYPE_PETAPOCO)
                {
                    sbPart.AppendLine("        [Column(\"" + columnName.ToLower() + "\")]");
                }

                switch (dataType)
                {
                    case "int":
                    case "int32":
                        dataType = "int";
                        break;
                    case "int64":
                        dataType = "long";
                        break;
                    case "date":
                    case "datetime":
                    case "timestamp":
                        dataType = "DateTime";
                        break;
                    case "decimal":
                        dataType = "decimal";
                        break;
                    case "double":
                        dataType = "double";
                        break;
                    case "float":
                        dataType = "float";
                        break;
                    case "tinyint":
                        dataType = "bool";
                        break;
                    default:
                        dataType = "string";
                        break;
                }

                sbPart.AppendLine("        public " + (cfg.driver_type == ConfigModel.DRIVER_TYPE_NHIBERNATE ? "virtual " : "") + dataType + " " + columnName.ToPascal() + " { get; set; }");

                if (columnName.ToLower() == "id" && (dataType == "int" || dataType == "long"))
                {
                    intId = true;
                }
            }
            sbPart.AppendLine();
            sbPart.AppendLine("        #endregion");

            #endregion

            if (cfg.driver_type == ConfigModel.DRIVER_TYPE_PETAPOCO)
            {
                sb.AppendLine("    [TableName(\"" + table.TableName.ToLower() + "\")]");
                //, autoIncrement = " + (intId ? "true" : "false") + "
                sb.AppendLine("    [PrimaryKey(\"id\")]");
            }
            sb.Append(sbPart);

            sb.AppendLine("    }");
            sb.AppendLine("}");

            GenerateFile(table.TableName.Replace(cfg.tablePrefix, "").ToPascal(), sb.ToString());
            if (cfg.driver_type == ConfigModel.DRIVER_TYPE_NHIBERNATE)
                GenerateFile(table.TableName.Replace(cfg.tablePrefix, "").ToPascal() + "Mapping", GenerateMappingCode(table, intId).ToString());
        }

        /// <summary>
        /// 生成业务层
        /// </summary>
        /// <param name="table"></param>
        /// <param name="tableComment"></param>
        void GenerateBusinessLayerCode(DataTable table, string tableComment)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Linq;");
            if (cfg.using_bll_array.Length > 0)
            {
                foreach (var item in cfg.using_bll_array)
                {
                    sb.AppendLine($"using {item};");
                }
            }

            sb.AppendLine();

            sb.AppendLine("namespace " + cfg.namespace_bll);
            sb.AppendLine("{");
            sb.AppendLine("    /// <summary>");
            sb.AppendLine("    /// " + tableComment);
            sb.AppendLine("    /// 作者：" + cfg.author);
            //Generate by Banana.AutoCode
            sb.AppendLine("    /// 时间：" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            sb.AppendLine("    /// </summary>");
            //sb.AppendLine("    [Serializable]");

            #region 属性

            string modelName = table.TableName.Replace(cfg.tablePrefix, "").ToPascal();
            sb.AppendLine("    public class " + modelName + "BLL" + " : BaseService<" + modelName + ">");
            sb.AppendLine("    {");

            if (cfg.constructor)
            {
                sb.AppendLine("        public " + modelName + "BLL" + "() { }");
                sb.AppendLine();
            }

            //sb.AppendLine($"        LogFactory logger = LogFactory.GetLogger(typeof({modelName}BLL));");
            sb.AppendLine("");
            #endregion

            sb.AppendLine("    }");
            sb.AppendLine("}");

            GenerateFile($"{modelName}BLL", sb.ToString());
        }


        StringBuilder GenerateMappingCode(DataTable table, bool intId = false)
        {
            string modelName = table.TableName.Replace(cfg.tablePrefix, "").ToPascal();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("using NHibernate.Mapping.ByCode;");
            sb.AppendLine("using NHibernate.Mapping.ByCode.Conformist;");
            sb.AppendLine();
            sb.AppendLine("namespace " + cfg.namespace_model);
            sb.AppendLine("{");
            sb.AppendLine("    /// <summary>");
            sb.AppendLine("    /// " + modelName + "Mapping");
            sb.AppendLine("    /// 作者：" + cfg.author);
            sb.AppendLine("    /// 时间：" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            sb.AppendLine("    /// </summary>");
            sb.AppendLine("    public class " + modelName + "Mapping : ClassMapping<" + modelName + ">");
            sb.AppendLine("    {");
            sb.AppendLine("        public " + modelName + "Mapping ()");
            sb.AppendLine("        {");

            sb.AppendLine("            base.Lazy(false);");
            sb.AppendLine("            base.Table(\"" + table.TableName.ToLower() + "\");");

            //主键
            if (intId)
            {
                sb.AppendLine("            base.Id(x => x.Id, map => { map.Generator(Generators.Native); });");
            }
            else
            {
                sb.AppendLine("            base.Id(x => x.Id, map => { map.Generator(Generators.UUIDHex()); });");
            }
            sb.AppendLine();

            string columnName;
            foreach (DataRow dataRow in table.Rows)
            {
                columnName = dataRow["name"].ToString();
                if (columnName.ToLower() == "id") continue;

                if (columnName.ToPascal().ToLower() == columnName.ToLower())
                {
                    sb.AppendLine("            base.Property(p => p." + columnName.ToPascal() + ");");
                }
                else
                {
                    sb.AppendLine("            base.Property(p => p." + columnName.ToPascal() + ", map => { map.Column(\"" + columnName.ToLower() + "\"); });");
                }
            }
            sb.AppendLine();
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb;
        }

        void GenerateFile(string filename, string content)
        {
            FileStream fileStream = new FileStream(cfg.output + "\\" + filename + ".cs", FileMode.Create, FileAccess.Write);
            //获得字节数组
            byte[] data = System.Text.Encoding.UTF8.GetBytes(content);
            //开始写入
            fileStream.Write(data, 0, data.Length);
            //清空缓冲区、关闭流
            fileStream.Flush();
            fileStream.Close();
        }

        void AutoSizeColumn(DataGridView dgViewFiles)
        {
            int width = 0;
            //使列自使用宽度
            //对于DataGridView的每一个列都调整
            for (int i = 0; i < dgViewFiles.Columns.Count; i++)
            {
                //将每一列都调整为自动适应模式
                dgViewFiles.AutoResizeColumn(i, DataGridViewAutoSizeColumnMode.AllCells);
                //记录整个DataGridView的宽度
                width += dgViewFiles.Columns[i].Width;
            }
            //判断调整后的宽度与原来设定的宽度的关系，如果是调整后的宽度大于原来设定的宽度，
            //则将DataGridView的列自动调整模式设置为显示的列即可，
            //如果是小于原来设定的宽度，将模式改为填充。
            //if (width > dgViewFiles.Size.Width)
            //{
            //    dgViewFiles.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            //}
            //else
            //{
            //    dgViewFiles.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            //}
            //冻结某列 从左开始 0，1，2
            //dgViewFiles.Columns[1].Frozen = true;
        }

    }
}
