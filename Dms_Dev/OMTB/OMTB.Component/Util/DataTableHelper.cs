using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace OMTB.Component.Util
{
    /// <summary>
    /// DataTable扩展类
    /// </summary>
    public class DataTableHelper
    {
        /// <summary>
        /// Enumerable转化为DataTable 
        /// </summary>   
        /// <typeparam name="T"></typeparam>    
        /// <param name="list"></param>    
        /// <returns></returns>    
        public static DataTable ConvertEnumerableToDataTable<T>(IEnumerable<T> list)
        {
            //创建属性的集合    
            List<PropertyInfo> pList = new List<PropertyInfo>();
            //获得反射的入口    
            Type type = typeof(T);
            DataTable dt = new DataTable();
            //把所有的public属性加入到集合 并添加DataTable的列    
            Array.ForEach<PropertyInfo>(type.GetProperties(), p => { pList.Add(p); dt.Columns.Add(p.Name, p.PropertyType); });
            foreach (var item in list)
            {
                //创建一个DataRow实例    
                DataRow row = dt.NewRow();
                //给row 赋值    
                pList.ForEach(p => row[p.Name] = p.GetValue(item, null));
                //加入到DataTable    
                dt.Rows.Add(row);
            }
            return dt;
        }
        
        /// <summary>
        /// 将DataReader转换为DataTable
        /// </summary>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static DataTable ConvertDataReaderToDataTable(IDataReader dataReader)  
        {  
            //定义DataTable  
            DataTable datatable = new DataTable();  
  
            try  
            {    //动态添加表的数据列  
                for (int i = 0; i < dataReader.FieldCount; i++)  
                {  
                    DataColumn myDataColumn = new DataColumn();  
                    myDataColumn.DataType = dataReader.GetFieldType(i);  
                    myDataColumn.ColumnName = dataReader.GetName(i);  
                    datatable.Columns.Add(myDataColumn);  
                }  
  
                //添加表的数据  
                while (dataReader.Read())  
                {  
                    DataRow myDataRow = datatable.NewRow();  
                    for (int i = 0; i < dataReader.FieldCount; i++)  
                    {  
                        myDataRow[i] = dataReader[i].ToString();  
                    }  
                    datatable.Rows.Add(myDataRow);
                }  
                //关闭数据读取器  
                dataReader.Close();  
                return datatable;  
            }  
            catch (Exception ex)  
            {  
                //抛出类型转换错误  
                //SystemError.CreateErrorLog(ex.Message);  
                throw new Exception(ex.Message, ex);  
            }  
        }
    }
}
