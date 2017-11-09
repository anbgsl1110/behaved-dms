using System;
using System.Data;
using System.IO;
using Aspose.Cells;

namespace OMTB.Component.Excel
{
    /// <summary>
    /// 从Excel导出数据
    /// </summary>
    public class ExcelExportData
    {
        /// <summary>
        /// 将excel中的数据导入到DataTable中
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="inputStream"></param>
        /// <param name="sheetName">excel工作薄sheet的名称</param>
        /// <param name="isFirstRowColumn">第一行是否是DataTable的列名</param>
        /// <returns>返回的DataTable</returns>
        public static DataTable ExcelToDataTable(string fileName, Stream inputStream, string sheetName, bool isFirstRowColumn)
        {
            Workbook workbook = null;
            Worksheet sheet = null;
            DataTable data = new DataTable();
            int startRow = 0;
            try
            {
                workbook = new Workbook(inputStream);

                if (sheetName != null)
                {
                    sheet = workbook.Worksheets[sheetName];
                    if (sheet == null) //如果没有找到指定的sheetName对应的sheet，则尝试获取第一个sheet
                    {
                        sheet = workbook.Worksheets[0];
                    }
                }
                else
                {
                    sheet = workbook.Worksheets[0];
                }
                if (sheet != null)
                {
                    Row firstRow = sheet.Cells.Rows[0];
                    int cellCount = firstRow.LastCell.Column; //一行最后一个cell的编号 即总的列数

                    if (isFirstRowColumn)
                    {
                        for (int i = firstRow.FirstCell.Column; i <= cellCount; ++i)
                        {
                            Cell cell = firstRow[i];
                            if (cell != null)
                            {
                                string cellValue = cell.StringValue;
                                if (cellValue != null)
                                {
                                    if (cellValue.Contains("\r"))
                                    {
                                        cellValue = cellValue.Replace("\r", "");
                                    }
                                    if (cellValue.Contains("\n"))
                                    {
                                        cellValue = cellValue.Replace("\n", "");//去除换行符
                                    }
                                    
                                    DataColumn column = new DataColumn(GetTableColumnName(data,cellValue.Trim()));
                                    
                                    data.Columns.Add(column.ColumnName);
                                }
                            }
                        }
                        startRow = 1;
                    }
                    else
                    {
                        startRow = 0;
                    }

                    //最后一列的标号
                    int rowCount = sheet.Cells.Rows.Count;
                    for (int i = startRow; i <= rowCount; ++i)
                    {
                        Row row = sheet.Cells.Rows[i];
                        if (row == null) continue; //没有数据的行默认是null　　　　　　　
                        if (IsEmptyRow(row))
                        {
                            continue; //空数据列
                        }
                        DataRow dataRow = data.NewRow();
                        for (int j = row.FirstCell.Column; j <= cellCount; ++j)
                        {
                            if (row[j] != null)
                            {
                                //同理，没有数据的单元格都默认是null
                                dataRow[j] = row[j].StringValue;

                            } 
                        }
                        data.Rows.Add(dataRow);
                    }
                }
                return data;
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        /// <summary>
        /// 验证空行
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private static bool IsEmptyRow(Row row)
        {
            var result = true;
            if (row.FirstCell == null || row.LastCell == null)
            {
                return true;
            }
            for (int j = row.FirstCell.Column; j < row.LastCell.Column; ++j)
            {
                if (row[j] != null && (!string.IsNullOrEmpty(row[j].StringValue)))
                {
                    result = false;
                    break;
                } 
            }
            return result;
        }
        
        /// <summary>
        /// 处理重复的列表
        /// </summary>
        /// <param name="table"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        private static string GetTableColumnName(DataTable table, string columnName)
        {
            while (true)
            {
                if (table.Columns.Contains(columnName))
                {
                    columnName += 1;
                }
                else
                {
                    break;
                }
            }
            return columnName;
        }
    }
}
