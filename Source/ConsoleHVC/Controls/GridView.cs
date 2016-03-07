using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleHVC.Controls
{
    public class GridView : ConsoleControl
    {
        public event GridViewDataSourceEventHandler OnNeedDataSource;
        public bool AutoGenerateColumns { get; set; } = true;
        public List<GridViewColumn> Columns { get; set; } = new List<GridViewColumn>();
        public object DataSource { get; set; }
        public int Left { get; set; }
        public int Top { get; set; }
        public override void OnRenderControl()
        {
            if (OnNeedDataSource != null)
            {
                OnNeedDataSource(this, new GridViewDataSourceEventArgs());
            }
            if (DataSource == null)
                throw new InvalidOperationException("Datasource is null");
            if (AutoGenerateColumns)
            {
                GetColumnsFromDataSource();
            }
            var table = BuildTable();
            DrawTable(table);
        }

        private void DrawTable(List<GridDataRow> table)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            int x = Left, y = Top;
            int rowIndex = -1;
            foreach (var item in table)
            {                
                x = Left;
                foreach (var col in Columns)
                {
                    Console.SetCursorPosition(x += 1, y);
                    Console.Write("|");
                    Console.SetCursorPosition(x += 1, y);

                    if (rowIndex == -1)
                        Console.Write(col.Name);
                    else
                        Console.Write(item.GetColumnString(col.Name));
                    Console.SetCursorPosition(x += col.Size.Value-1, y);
                    //Console.SetCursorPosition(x += col.Name, y);
                    //for (int i = 0; i < col.Size; i++)
                    //{
                   
                }
                Console.SetCursorPosition(x += 1, y);
                Console.Write("|");

                x = Left;
                if (rowIndex == -1 || rowIndex == table.Count-1)
                {
                    y++;
                    foreach (var col in Columns)
                    {
                        Console.SetCursorPosition(x += 1, y);
                        Console.Write("|");
                        for (int i = 0; i < col.Size; i++)
                        {
                            Console.SetCursorPosition(x += 1, y);
                            Console.Write("-");
                        }
                        
                        //Console.SetCursorPosition(x += col.Size.Value - 1, y);
                    }
                    Console.SetCursorPosition(x += 1, y);
                    Console.Write("|");
                }
               
                y ++;
                rowIndex++;
            }
        }

        private List<GridDataRow> BuildTable()
        {
            var table = new List<GridDataRow>();
          
            table.Add(new GridDataRow(this));

            foreach (var item in GetDataSourceEnumerable())
            {
                table.Add(new GridDataRow(this, item));
            }
            return table;
        }

        private IEnumerable GetDataSourceEnumerable()
        {
            return DataSource as IEnumerable;
        }

        private void GetColumnsFromDataSource()
        {
            Columns.Clear();

            var props = GetDataSourceEnumerable().GetType().GetGenericArguments().First().GetProperties();
            foreach (var prop in props)
            {
                Columns.Add(new GridViewColumn() { Name=prop.Name });
            }
        }


        private class GridDataRow
        {
            public GridView ParentGrid { get; set; }
            public object DataItem;
            

            public GridDataRow(GridView parent)
            {
                ParentGrid = parent;
                CalculateWidth();
            }

            public GridDataRow(GridView parent, object dataItem) : this(parent)
            {
                this.DataItem = dataItem;
                CalculateWidth();
            }
            private void CalculateWidth()
            {
                foreach (var col in ParentGrid.Columns)
                {
                    if (DataItem != null) CalculateMax(col, GetPropertyValue(DataItem, col.Name));
                    else CalculateMax(col, col.Name.Length);
                }                
            }

            private int GetPropertyValue(object dataItem, string name)
            {
                return dataItem.GetType().GetProperty(name).GetValue(dataItem).ToString().Length;
            }

            private void CalculateMax(GridViewColumn col, int size)
            {
                if (!col.Size.HasValue) col.Size = size;
                else if (size > col.Size) col.Size = size;
            }
            public string GetColumnString(string columnName)
            {
                return DataItem.GetType().GetProperty(columnName).GetValue(DataItem).ToString();
            }
        }
    }
    public delegate void GridViewDataSourceEventHandler(object sender, GridViewDataSourceEventArgs e);
    public class GridViewDataSourceEventArgs : EventArgs
    {

    }

    public class GridViewColumn : ConsoleControl
    {
        public string Name { get; internal set; }
        /// <summary>
        /// NULL SETS IT TO AUTO
        /// </summary>
        public int? Size { get; set; }
        public override void OnRenderControl()
        {
            throw new NotImplementedException();
        }
    }

    
}
