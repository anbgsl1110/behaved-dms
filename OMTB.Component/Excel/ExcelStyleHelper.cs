using System.Drawing;
using Aspose.Cells;

namespace OMTB.Component.Excel
{
    /// <summary>
    /// Excel样式帮助类
    /// </summary>
    public class ExcelStyleHelper
    {
        /// <summary>
        /// 设置Ecxel样式对象
        /// </summary>
        public class ExcelStyle
        {
            /// <summary>
            /// 字体大小
            /// </summary>
            public int FontSize { get; set; }
            /// <summary>
            /// 设置字体颜色
            /// </summary>
            public Color FontColor { get; set; }
            /// <summary>
            /// 设置字体类型
            /// </summary>
            public string FontName { get; set; }
            /// <summary>
            /// 设置背景颜色
            /// </summary>
            public Color BgColor { get; set; }
            /// <summary>
            /// 设置字体居中、左对齐...
            /// </summary>
            public TextAlignmentType TextType { get; set; }
        }
        
        /// <summary>
        /// 列数行宽高信息
        /// </summary>
        public class ColumnRowWidthHeightStyle
        {
            /// <summary>
            /// 列数
            /// </summary>
            public int ColumnCount { get; set; }
            /// <summary>
            /// 宽
            /// </summary>
            public double WidthCount { get; set; }
            /// <summary>
            /// 行数
            /// </summary>
            public int RowCount { get; set; }
            /// <summary>
            /// 高
            /// </summary>
            public double HeightCount { get; set; }
        }
        
        /// <summary>
        /// 获取Excel样式
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="excelStyle"></param>
        /// <returns></returns>
        public static Style GetExcelStyle(Workbook workbook,ExcelStyle excelStyle)
        {
            Style style = workbook.Styles[workbook.Styles.Add()];
            if (excelStyle.BgColor.Name != "0")
            {
                style.ForegroundColor = excelStyle.BgColor;
                style.Pattern = BackgroundType.Solid;
            }
            if (excelStyle.FontSize != 0)
            {
                style.Font.Size = excelStyle.FontSize;
            }
            if (excelStyle.FontColor.Name != "0")
            {
                style.Font.Color = excelStyle.FontColor;
            }
            if (!string.IsNullOrEmpty(excelStyle.FontName))
            {
                style.Font.Name = excelStyle.FontName;
            }
            style.HorizontalAlignment = excelStyle.TextType;
            return style;
        }  
        
        /// <summary>
        /// 设置Excel行高信息
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="rowHeight"></param>
        public static void SetExcelRowHeight(Worksheet sheet, ColumnRowWidthHeightStyle rowHeight)
        {
            sheet.Cells.SetRowHeight(rowHeight.RowCount, rowHeight.HeightCount);
        }
        
        /// <summary>
        /// 设置Excel行高信息
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="rowHeight"></param>
        public static void SetExcelRowHeight(Worksheet sheet, ColumnRowWidthHeightStyle[] rowHeight)
        {
            for (var i = 0; i < rowHeight.Length; i++)
            {
                sheet.Cells.SetRowHeight(rowHeight[i].RowCount, rowHeight[i].HeightCount);
            }
        }
        
        /// <summary>
        /// 设置Excel列宽信息
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="columnWidth"></param>
        public static void GetExcelColumnWidth(Worksheet sheet, ColumnRowWidthHeightStyle columnWidth)
        {
            sheet.Cells.SetColumnWidth(columnWidth.ColumnCount, columnWidth.WidthCount);
        }
        
        /// <summary>
        /// 设置Excel列宽信息
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="columnWidth"></param>
        public static void GetExcelColumnWidth(Worksheet sheet, ColumnRowWidthHeightStyle[] columnWidth)
        {   
            for(var i=0;i< columnWidth.Length;i++)
            {
                sheet.Cells.SetColumnWidth(columnWidth[i].ColumnCount, columnWidth[i].WidthCount);
            }        
        }  
    }
}
