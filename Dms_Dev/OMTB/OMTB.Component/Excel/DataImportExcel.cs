using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net;
using Aspose.Cells;

namespace OMTB.Component.Excel
{
    /// <summary>
    /// 把数据导入到Excel
    /// </summary>
    public class DataImportExcel
    {
        /// <summary>
        /// 使用Excel模板把DataTable数据写入Excel
        /// </summary>
        /// <param name="table"></param>
        /// <param name="excelPath"></param>
        /// <returns></returns>
        public static Stream DataTableToExcel(DataTable table, string excelPath)
        {
            MemoryStream memory = null;
            Workbook workbook = null;
            try
            {
                WebClient my = new WebClient();
                byte[] mybyte;
                mybyte = my.DownloadData(excelPath);
                memory = new MemoryStream(mybyte);
                workbook = new Workbook(memory);
                Worksheet sheet1 = workbook.Worksheets[0];
                Row defaultRow = sheet1.Cells.Rows[1];
                for (int rowIndex = 0; rowIndex < table.Rows.Count; rowIndex++)
                {
                    for (int columnIndex = 0; columnIndex < table.Columns.Count; columnIndex++)
                    {
                        var cell = sheet1.Cells.Rows[rowIndex + 1][columnIndex];
                        var value = table.Rows[rowIndex][columnIndex];
                        cell.PutValue(value == null ? string.Empty : value.ToString());
                        if (defaultRow.LastCell.Column > columnIndex)
                        {
                            cell.SetStyle(defaultRow[columnIndex].GetStyle());
                            //cell.SetCellType(defaultRow.GetCell(columnIndex).CellType);
                        }
                    }
                }
                //创建文件
                memory = workbook.SaveToStream();

                ////创建文件
                //FileStream files = new FileStream(@"C:\Document\Test\abc.xlsx", FileMode.Create);
                //memory.Position = 0;
                //memory.WriteTo(files);
                //files.Close();
            }
            catch (Exception)
            {
                return null;
            }

            return memory;
        }
        /// <summary>
        /// 使用Excel模板把DataTable数据写入Excel
        /// </summary>
        /// <param name="table"></param>
        /// <param name="excelTemplatePath"></param>
        /// <returns></returns>
        public static Stream DataTableToExcelByTemplate(DataTable table, string excelTemplatePath)
        {
            table.TableName = "t";
            WorkbookDesigner designer = new WorkbookDesigner();
            Workbook b = new Workbook(excelTemplatePath);
            designer.Workbook = b;
            //designer.Open(excelTemplatePath);
            //数据源 
            designer.SetDataSource(table);
            designer.Process();
            designer.Workbook.CalculateFormula(true); //计算公式
            return designer.Workbook.SaveToStream();
        }

        /// <summary>
        /// DataTable数据写入无样式的Excel
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static Stream DataTableToExcel(DataTable table)
        {
            MemoryStream memory = null;
            Workbook workbook = null;
            try
            {
                workbook = new Workbook();
                Worksheet sheet1 = workbook.Worksheets[0];
                Row defaultRow = sheet1.Cells.Rows[1];
                //表头需单独输出
                for (int columnIndex = 0; columnIndex < table.Columns.Count; columnIndex++)
                {
                    var titleCell = sheet1.Cells.Rows[0][columnIndex];
                    var titleValue = table.Columns[columnIndex];
                    titleCell.PutValue(titleValue == null ? string.Empty : titleValue.ToString());       
                }
                for (int rowIndex = 0; rowIndex < table.Rows.Count; rowIndex++)
                {
                    for (int columnIndex = 0; columnIndex < table.Columns.Count; columnIndex++)
                    {
                        var cell = sheet1.Cells.Rows[rowIndex + 1][columnIndex];
                        var value = table.Rows[rowIndex][columnIndex];
                        cell.PutValue(value == null ? string.Empty : (table.Rows[rowIndex][columnIndex] is DateTime ? DateTime.Parse(value.ToString()).Date.ToShortDateString() : value.ToString()));
                        if (defaultRow.LastCell.Column > columnIndex)
                        {
                            cell.SetStyle(defaultRow[columnIndex].GetStyle());
                            //cell.SetCellType(defaultRow.GetCell(columnIndex).CellType);
                        }
                    }
                }

                //创建文件
                memory = workbook.SaveToStream();
                // 设置当前流的位置为流的开始
                memory.Seek(0, SeekOrigin.Begin);
                ////创建文件
                //FileStream files = new FileStream(@"D:\Document\Test\abc.xlsx", FileMode.Create);
                //memory.Position = 0;
                //memory.WriteTo(files);
                //files.Close();
            }
            catch (Exception)
            {
                return null;
            }

            return memory;
        }

        /// <summary>
        /// DataTable数据写入默认样式的Excel
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static Stream DataTableToDefaultStyleExcel(DataTable table)
        {
            MemoryStream memory = null;
            Workbook workbook = null;
            try
            {
                workbook = new Workbook();
                Worksheet sheet1 = workbook.Worksheets[0];
                Row defaultRow = sheet1.Cells.Rows[1];
                ExcelStyleHelper.ExcelStyle st = new ExcelStyleHelper.ExcelStyle();
                st.FontSize = 11;
                st.TextType = TextAlignmentType.Center;
                var style = ExcelStyleHelper.GetExcelStyle(workbook, st);
                ExcelStyleHelper.ExcelStyle st2 = new ExcelStyleHelper.ExcelStyle();
                st2.FontSize = 12;
                st2.FontColor = Color.Black;
                st2.TextType = TextAlignmentType.Center;
                st2.BgColor = Color.FromArgb(230, 230, 230);
                var style2 = ExcelStyleHelper.GetExcelStyle(workbook, st2);               
                ExcelStyleHelper.ColumnRowWidthHeightStyle rowHeight = new ExcelStyleHelper.ColumnRowWidthHeightStyle();
                rowHeight.RowCount = 0;
                rowHeight.HeightCount = 20;
                ExcelStyleHelper.SetExcelRowHeight(sheet1, rowHeight);
                ExcelStyleHelper.ColumnRowWidthHeightStyle[] columnWidth = new ExcelStyleHelper.ColumnRowWidthHeightStyle[table.Columns.Count];
                
                //表头需单独输出
                for (int columnIndex = 0; columnIndex < table.Columns.Count; columnIndex++)
                {
                    var titleCell = sheet1.Cells.Rows[0][columnIndex];
                    var titleValue = table.Columns[columnIndex];
                    titleCell.PutValue(titleValue == null ? string.Empty : titleValue.ToString());
                    titleCell.SetStyle(style2);
                    columnWidth[columnIndex] = new ExcelStyleHelper.ColumnRowWidthHeightStyle { ColumnCount = columnIndex, WidthCount = 20 };
                }
                
                ExcelStyleHelper.GetExcelColumnWidth(sheet1, columnWidth);
                for (int rowIndex = 0; rowIndex < table.Rows.Count; rowIndex++)
                {
                    for (int columnIndex = 0; columnIndex < table.Columns.Count; columnIndex++)
                    {
                        var cell = sheet1.Cells.Rows[rowIndex + 1][columnIndex];
                        var value = table.Rows[rowIndex][columnIndex];
                        cell.PutValue(value == null
                            ? string.Empty
                            : (table.Rows[rowIndex][columnIndex] is DateTime
                                ? DateTime.Parse(value.ToString()).ToString("yyyy-MM-dd HH:mm:ss")
                                : value.ToString()));
                        cell.SetStyle(style);
                        if (defaultRow.LastCell.Column > columnIndex)
                        {
                            cell.SetStyle(defaultRow[columnIndex].GetStyle());
                            cell.SetStyle(style);
                            //cell.SetCellType(defaultRow.GetCell(columnIndex).CellType);
                        }
                    }
                }

                //创建文件
                memory = workbook.SaveToStream();
                // 设置当前流的位置为流的开始
                memory.Seek(0, SeekOrigin.Begin);
                ////创建文件
                //FileStream files = new FileStream(@"D:\Document\Test\abc.xlsx", FileMode.Create);
                //memory.Position = 0;
                //memory.WriteTo(files);
                //files.Close();
            }
            catch (Exception)
            {
                return null;
            }

            return memory;
        }
    }
}
