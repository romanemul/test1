using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RRD
{
    [DefaultProperty("dt")]
    public class DataTableExtras : DataTable, IComparable, IRRDDataRequestable
    {
        DataGridView dgvReference;
        public BindingSource bindingSource = new BindingSource();
        public List<string> ColumnNames;
        public List<string> DataGridViewNames;
        DataTable dt = new DataTable();
        private string TableName;
        

        public DataGridView DgvReference 
        { 
            get => dgvReference; 
            set => dgvReference = value; 
        }
        public DataTable Dt 
        { 
            get => dt; 
            set => dt = value; 
        }
        
        public DataTableExtras(DataTable table)
        {
            dt = table;
            TableName = table.TableName;
            GetColumnNames();
        }

        public DataTableExtras(DataTable table, Form FormReference)
        {
            dt = table;
            TableName = table.TableName;
            GetColumnNames();
            DataGridViewNames = GetDatagridViewObjects(FormReference);
        }

        public DataTableExtras(string Schema, string Table)
        { 
            MySQLConnector mySQLConnector = new MySQLConnector(Schema, Table);
            dt = mySQLConnector.SelectAllFrom(Table);
        }


        public void DataTableToListView()
        {

        }

        public void DataTableToListBox(object[] Columns, ListBox listBox)
        {
            if(Columns == null) 
            { 
                return; 
            }
        
            listBox.Items.AddRange(Columns);
            string ColumnName = "Test";
            dt.AsEnumerable().Where(a => a[""+ ColumnName + ""].ToString().Contains("RD")).ToList().ForEach(a => listBox.Items.AddRange(Columns));
        }

        public static DataTable GetDataTable(string Schema, string Table)
        {
            DataTable dtTmp = new DataTable();
            MySQLConnector mySQLConnector = new MySQLConnector(Schema, Table);
            dtTmp = mySQLConnector.SelectAllFrom(Table);
            return dtTmp;            
        }

        
        public string SelectMAX(string Column)
        {
            string val = dt.AsEnumerable().Select(a => a["" + Column + ""]).Max().ToString();
            //if (dt.Rows[0][0] == null)
            //{ 
            //    return; 
            //}
            return val;
        }

        public DataTable FromColumn(string column, string value)
        {
            DataTable tmp = new DataTable();
            var c = dt.AsEnumerable().Where(a => a["" + column + ""].ToString() == value).ToList();
            if (c.Count > 0)
            {
                tmp = dt.AsEnumerable().Where(a => a["" + column + ""].ToString() == value).CopyToDataTable();
            }
            return tmp;
        }

        public DataTable FromColumn(string column, List<string> Values)
        {
            DataTable tmp = new DataTable();
            var c = dt.AsEnumerable().Where(a => Values.Any(b => b.ToString() == a["" + column + ""].ToString())).ToList();
            if (c.Count > 0)
            {
                tmp = dt.AsEnumerable().Where(a => Values.Any(b => b.ToString() == a["" + column + ""].ToString())).CopyToDataTable();
            }
            return tmp;
        }
        

        public DataTable FromColumn(string Column, string Operator, string Value)
        {
            DataTable tmp = new DataTable();
           
            var c = dt.AsEnumerable().Where(a => a["" + Column + ""].ToString() == Value).ToList();
            if (c.Count > 0)
            {
                tmp = dt.AsEnumerable().Where(a => a["" + Column + ""].ToString() == Value).CopyToDataTable();
            }
            return tmp;
        }

        public int CompareTo(object rightobj)
        {
            if (rightobj == null) 
            {
                return 1;
            }
            
            DataTableExtras r = rightobj as DataTableExtras;
            if (rightobj != null) 
                
                return this.TableName.CompareTo(r.TableName);
            else                        
            throw new NotImplementedException();
        }


        public List<string> GetAllValuesFromColumn(string Column)
        {
            return dt.AsEnumerable().Select(a => a["" + Column + ""].ToString()).ToList();
        }

        public List<T> GetAllValuesFromColumn<T>(string Column)
        {
            return dt.AsEnumerable().Select(a => a["" + Column + ""]).Cast<T>().ToList();
        }


        public List<string> GetDistinctValuesFromColumn(string Column)
        {
            return dt.AsEnumerable().Select(a => a["" + Column + ""].ToString()).Distinct().ToList();
        }
        

        public DataTable Contains(List<string> Values,string Column)
        {
            DataTable dataTable = new DataTable();
            var tmpValue = dt.AsEnumerable().Where(a => Values.Any(b => b.ToString() == a["" + Column + ""].ToString())).ToList();
            if(tmpValue.Count() > 0) 
            {
                dataTable = dt.AsEnumerable().Where(a => Values.Any(b => b.ToString() == a["" + Column + ""].ToString())).CopyToDataTable();
                return dataTable;
            }
            {
                dataTable.TableName = "Empty Table";
                return dataTable;
            }        
        }

        public DataTable NotContains(List<string> Values, string Column)
        {
            DataTable dataTable = new DataTable();
            var tmpValue = dt.AsEnumerable().Where(a => !Values.Any(b => b.ToString() == a["" + Column + ""].ToString())).ToList();
            if (tmpValue.Count() > 0)
            {
                dataTable = dt.AsEnumerable().Where(a => !Values.Any(b => b.ToString() == a["" + Column + ""].ToString())).CopyToDataTable();
                return dataTable;
            }
            {
                dataTable.TableName = "Empty Table";
                return dataTable;
            }
        }

        public List<TimeSpan> TimeDifferenciesToTimeSpanList(DataTable dt, string ColumnName, string ColumnName2)
        {
            List<TimeSpan> lst = new List<TimeSpan>();            
            
            DataColumn dc = new DataColumn();
            dc.ColumnName = "DateDifferencies";
            dc.DataType = typeof(TimeSpan);
            dt.Columns.Add("dc");

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if(dt.Rows[i]["" + ColumnName + ""] == DBNull.Value || dt.Rows[i]["" + ColumnName2 + ""] == DBNull.Value) 
                {
                    continue;                
                }
                TimeSpan travelTime = (DateTime)dt.Rows[i]["" + ColumnName + ""] - (DateTime)dt.Rows[i]["" + ColumnName2 + ""];
                dt.Rows[i]["dc"] = (DateTime)dt.Rows[i]["" + ColumnName + ""] - (DateTime)dt.Rows[i]["" + ColumnName2 + ""];
                
                lst.Add(travelTime);                
            }
            return lst;
        }

        public DataTable TimeDifferenciesToDataTable(DataTable dt, string ColumnName, string ColumnName2)
        {

            DataColumn dc = new DataColumn();
            dc.ColumnName = "DateDifferencies";
            dc.DataType = typeof(TimeSpan);
            dt.Columns.Add("dc");

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i]["" + ColumnName + ""] == DBNull.Value || dt.Rows[i]["" + ColumnName2 + ""] == DBNull.Value)
                {
                    continue;
                }
                TimeSpan travelTime = (DateTime)dt.Rows[i]["" + ColumnName + ""] - (DateTime)dt.Rows[i]["" + ColumnName2 + ""];
                dt.Rows[i]["dc"] = (DateTime)dt.Rows[i]["" + ColumnName + ""] - (DateTime)dt.Rows[i]["" + ColumnName2 + ""];
            }
            return dt;
        }

        public DataTable Transpose(DataTable dt, Boolean HasColumnNames)
        {
            DataTable extrasDt = new DataTable();

            if (HasColumnNames)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataColumn col = new DataColumn();
                    col.DataType = typeof(string);
                    col.ColumnName = dt.Columns[i].ColumnName;
                    extrasDt.Columns.Add(col);
                }
            }
            else 
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataColumn col = new DataColumn();
                    col.DataType = typeof(string);
                    //col.ColumnName = dt.Columns[i].ColumnName;
                    extrasDt.Columns.Add(col);
                }
            }

            for (int j = 0; j < dt.Columns.Count; j++)
            {                
                DataRow dr = extrasDt.NewRow();
                {
                    for (int i = 0; i < dt.Rows.Count; i++)

                        dr[i] = dt.Rows[i][j];
                }
                
                extrasDt.Rows.Add(dr);
            }
            return extrasDt;
        }


        public virtual List<string> GetColumnNames() 
        {
            return ColumnNames = dt.Columns.Cast<DataColumn>().AsEnumerable().Select(column => column.ColumnName.ToString()).ToList();        
        }

        public void SelectDistinct(string query)
        {
            throw new NotImplementedException();
        }

        public List<string> SelectToList(string query)
        {
            throw new NotImplementedException();
        }

        public void BindToListBox<T>(string ListboxName, List<T> values, Form FormName)
        {
            List<T> tmpValues = values.AsEnumerable().Select(a=> a.ToString()).Cast<T>().ToList();

            ListBox f = FormName.Controls.OfType<ListBox>().AsEnumerable().Where(a => a.Name == ListboxName).First();
            f.Items.Clear();
            f.Items.AddRange(tmpValues.Cast<string>().ToArray());
            
        }
        public DataRow MakeEmptyDataRow(DataRow dataRow)
        {
            DataTable tmp2 = new DataTable();
            DataRow dr = tmp2.NewRow();
            
            foreach(string columnName in ColumnNames)
            {
                tmp2.Columns.Add(columnName);
            }            
            return dr;        
        }
                

        public DataTable MakeEmptyDataTable()
        {
            DataTable tmp = new DataTable();
            DataRow dr = tmp.NewRow();
            MakeEmptyDataRow(dr);

            tmp.Rows.Add(dr);
            return tmp;
        }

        public static int BindToDataGridView(DataGridView DataGridViewName, DataTable dataTable , Form FormName)
        {
            if (dataTable == null) 
            {
                return 1;
            }
            else
            {
                DataGridViewName.DataSource = dataTable;
                return 0;
            }
        }

        private List<string> GetDatagridViewObjects(Form formReference)
        {
            List<string> f = formReference.Controls.OfType<DataGridView>().Select(a => a.Name).ToList();
            if(f.Count == 0) 
            {
                f.Add("");
            }            
            return f;        
        }

        public DataTable SumIf(string ValuesColumn, string ValueToSum, string ColumnToSum)
        {
            DataTable tmpDataTable = new DataTable();
            var tmpExpression = dt.AsEnumerable().Where(a => ValueToSum == a["" + ValuesColumn + ""].ToString()).ToList();

            if (tmpExpression.Count == 0)
            {
                tmpDataTable = MakeEmptyDataTable();
            }
            else
            {
                DataColumn KeyDataColumn = new DataColumn("Key");
                DataColumn ValueDataColumn = new DataColumn("Value");
                tmpDataTable.Columns.Add(KeyDataColumn);
                tmpDataTable.Columns.Add(ValueDataColumn);

                var dr = tmpDataTable.NewRow();

                dr["Key"] = ValueToSum;
                dr["Value"] = dt.AsEnumerable()
                    .Where(a => ValueToSum == a["" + ValuesColumn + ""].ToString())
                    .Sum(c => Convert.ToDouble(c["" + ColumnToSum + ""]));

                tmpDataTable.Rows.Add(dr);
            }
            return tmpDataTable;
        }

        public Dictionary<string,double> SumIfToDictionary(string ValuesColumn, List<string> ValuesToSum, string ColumnToSum)
        {
            DataTable tmpDataTable = new DataTable();
            Dictionary<string, double> Result = new Dictionary<string, double>();

            var tmpExpression = dt.AsEnumerable().Where(a => ValuesToSum.Any(b => b.ToString() == a["" + ValuesColumn + ""].ToString())).ToList();
            if (tmpExpression.Count == 0)
            {
                Result.DefaultIfEmpty();
            }
            else
            {
                List<string> vals = ValuesToSum.Distinct().ToList();
                            
                for (int i = 0; i < vals.Count; i++)
                {

                    Result.Add
                        (
                        vals[i].ToString(), 
                        dt.AsEnumerable().Where(a => vals[i].ToString() == a["" + ValuesColumn + ""].ToString()).Sum(c => Convert.ToDouble(c["" + ColumnToSum + ""]))
                    );

                }
            }
            return Result;
        }


        public DataTable SumIfToDataTable(string ValuesColumn, List<string> ValuesToSum, string ColumnToSum)
        {
            DataTable tmpDataTable = new DataTable();

            var tmpExpression = dt.AsEnumerable().Where(a => ValuesToSum.Any(b => b.ToString() == a["" + ValuesColumn + ""].ToString())).ToList();
            if (tmpExpression.Count == 0)
            {
                tmpDataTable = MakeEmptyDataTable();
            }
            else 
            {
                List<string> vals = ValuesToSum.Distinct().ToList();

                DataColumn KeyDataColumn = new DataColumn("Key");
                DataColumn ValueDataColumn = new DataColumn("Value");
                tmpDataTable.Columns.Add(KeyDataColumn);
                tmpDataTable.Columns.Add(ValueDataColumn);

                for (int i = 0; i < vals.Count; i++)
                {
                    var dr = tmpDataTable.NewRow();

                    dr["Key"] = vals[i].ToString();
                    dr["Value"] = dt.AsEnumerable()
                    .Where(a => vals[i].ToString() == a["" + ValuesColumn + ""].ToString())
                    .Sum(c => Convert.ToDouble(c["" + ColumnToSum + ""]));                    

                    tmpDataTable.Rows.Add(dr);
                }
            }
            
            return tmpDataTable;
            
        }

        public void MergeColumns(string Column1, string Column2) 
        {
            DataTable dataTable = new DataTable();
            var d = dt.AsEnumerable().Select(a =>
            new
            {
                MergedColumn = a["" + Column1 + ""].ToString().Trim() + a["" + Column2 + ""].ToString().Trim()
            })
                .Distinct().ToList();
            
            }

        public DataRow [] DuplicatesColumnInColumnToDataRows(string Where) 
        {            
            var duplicates = dt.AsEnumerable().GroupBy(a => a["Batch"]).Where(a => a.Count() > 2).Select(a=>a).ToArray();
            return dt.AsEnumerable().Cast<DataRow>().Where(a=> duplicates.ToList().Any(b=> b.Key.ToString() == a["Batch"].ToString())).Select(a => a).ToArray();
        }

        public DataTable DuplicatesInColumnToDataTable(string Where)
        {
            DataTable dataTable = new DataTable();
            var duplicates = dt.AsEnumerable().GroupBy(a => a["Batch"]).Where(a => a.Count() > 2).Select(a=>a).ToArray();
            dataTable = dt.AsEnumerable().Cast<DataRow>().Where(a => duplicates.ToList().Any(b => b.Key.ToString() == a["Batch"].ToString())).CopyToDataTable();//.Select(a => a).ToArray();
            return dataTable;
        }

        public static bool Compare<T>(string op, T left, T right) where T : IComparable<T>
        {
            switch (op)
            {
                case "<": return left.CompareTo(right) < 0;
                case ">": return left.CompareTo(right) > 0;
                case "<=": return left.CompareTo(right) <= 0;
                case ">=": return left.CompareTo(right) >= 0;
                case "==": return left.Equals(right);
                case "!=": return !left.Equals(right);
                default: throw new ArgumentException("Invalid comparison operator: {0}", op);
            }
        }
    }
}
