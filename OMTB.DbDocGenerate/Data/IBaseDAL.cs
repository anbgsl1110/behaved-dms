using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using DbDocGenerate.Common;
using DbDocGenerate.Plugin;

namespace DbDocGenerate.Data
{
    public class IBaseDAL
    {
        private string TableName { get; set; }

        public IBaseDAL(string tableName)
        {
            this.TableName = tableName;
        }

        public virtual int Add<T>(T t)
        {
            string sqlcommand = Lib.MakeInsertSql(TableName, t);

            int id = 0;

            DbHelper.ExecuteNonQuery(out id, sqlcommand);
            return id;
        }
        public virtual int Add<T>(SqlConnection conn, T t, bool hasUnicode = false)
        {
            string sqlcommand = Lib.MakeInsertSql(TableName, t, hasUnicode);

            int id = 0;

            DbHelper.ExecuteNonQuery(out id, conn, CommandType.Text, sqlcommand);
            return id;
        }

        public virtual bool Update<T>(T t,bool islog =false)
        {
            string sqlcommand = Lib.MakeUpdateSql(TableName, t);

            if (islog)
            {
                LogManager.WriteLog(LogFile.Data,string.Format(sqlcommand));
            }
            return DbHelper.ExecuteNonQuery(sqlcommand) > 0 ? true : false;
        }

        public virtual bool Update<T>(SqlConnection conn, T t, bool hasUnicode =false)
        {
            string sqlcommand = Lib.MakeUpdateSql(TableName, t, null, hasUnicode);

            return DbHelper.ExecuteNonQuery(conn, CommandType.Text, sqlcommand) > 0 ? true : false;
        }

        public virtual bool Delete(int id)
        {
            return DbHelper.ExecuteDelete(TableName, "id=" + id) > 0;
        }

        public virtual bool Delete(SqlConnection conn, int id)
        {
            string sql = $"delete from {TableName} where id= {id}";
            return DbHelper.ExecuteNonQuery(conn, CommandType.Text, sql) > 0;
        }

        public virtual T GetInfo<T>(int id)
        {
            if (id <= 0)
            {
                return Activator.CreateInstance<T>();
            }

            string sql = "select * from " + TableName + " where id=" + id;

            return Publics.CreateEntity<T>(sql);
        }


        protected virtual bool UpdateFiled(string sqlcommand)
        {
            return DbHelper.ExecuteNonQuery(sqlcommand) > 0 ? true : false;
        }

        /// <summary>
        /// added by lt 2016.4.7 增加另一库读法
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T GetInfo<T>(SqlConnection conn, int id)
        {
            string sql = "select * from " + TableName + " where id=" + id;

            return Publics.CreateEntity<T>(conn, sql);
        }

        public virtual List<T> GetInfoList<T>(params int[] ids)
        {
            StringBuilder idFilterBuilder = new StringBuilder();

            for (int i = 0; i < ids.Length; i++)
            {
                bool isLastIndex = i == ids.Length - 1;
                idFilterBuilder.Append(isLastIndex ? string.Format("{0}", ids[i]) : string.Format("{0},", ids[i]));
            }

            string sql = string.Format("select * from {0} where id in ({1})", TableName, idFilterBuilder.ToString());

            return Publics.GetList<T>(sql);
        }

        protected virtual T GetInfo<T>(string where)
        {
            string sql = string.Format("select top 1 * from {0} where {1}", TableName, where);

            return Publics.CreateEntity<T>(sql);
        }
        /// <summary>
        /// added by lt 2016.4.7 增加另一库读法
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        protected virtual T GetInfo<T>(SqlConnection conn, string where)
        {
            string sql = string.Format("select top 1 * from {0} where {1}", TableName, where);

            return Publics.CreateEntity<T>(conn, sql);
        }

        protected virtual int GetCount(string sql)
        {
            return Utils.Utils.StrToInt(DbHelper.ExecuteScalar(sql), 0);
        }
        protected virtual int GetCount(SqlConnection conn, string sql)
        {
            return Utils.Utils.StrToInt(DbHelper.ExecuteScalar(conn, CommandType.Text, sql), 0);
        }

        protected virtual T GetInfoByCommandText<T>(string commandText)
        {
            return Publics.CreateEntity<T>(commandText);
        }

        public virtual List<T> GetList<T>(string whereStr)
        {
            string sql = "select * from " + TableName + " where " + whereStr;

            return Publics.GetList<T>(sql);
        }

        public virtual List<T> GetList<T>(List<int> idList)
        {
            string whereStr = string.Format(" id in ({0})", string.Join(",", idList));

            return GetList<T>(whereStr);
        }


        /// <summary>
        ///  批量删除，根据ID list
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public virtual bool BatchDelete(List<int> list)
        {
            string ids = string.Join(",", list);
            return DbHelper.ExecuteDelete(TableName, string.Format(" id in ({0})", ids)) != 0;
        }

        /// <summary>
        /// 批量删除，针对有主从结构的表
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public virtual bool BatchDeleteRelated(List<int> list)
        {
            try
            {
                foreach (int id in list)
                {
                    Delete(id);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 根据传入的pagerInfo 参数获取分页后的list数据
        /// </summary>
        /// <param name="pager"></param>
        /// <returns></returns>
        public virtual DataListInfo GetPagingList(PagerInfo pager)
        {
            return null;
        }




        #region 业务层事务管理容器


        /// <summary>
        /// 添加（使用外部事务容器管理事务）  added by xuc 2016-7-18  C
        /// </summary>
        /// <param name="info"></param>
        /// <param name="container">事务管理容器，供dal中的事务注册，方便在外部如bll中统一进行commit rollback</param>
        /// <returns></returns>
        public int Add<T>(T info, ITransactionContainer container)
        {
            var conn = DbHelper.Factory.CreateConnection();
            conn.ConnectionString = DbHelper.ConnectionString;
            conn.Open();
            var trans = conn.BeginTransaction();

            container.ResiterTransaction(trans);
            var sql = Lib.MakeInsertSql(TableName, info);
            int newid = 0;
            DbHelper.ExecuteNonQuery(out newid, trans, CommandType.Text, sql);
            return newid;
        }


        public void AddList<T>(List<T> list, ITransactionContainer container)
        {
            var conn = DbHelper.Factory.CreateConnection();

            conn.ConnectionString = DbHelper.ConnectionString;
            conn.Open();

            var trans = conn.BeginTransaction();
            container.ResiterTransaction(trans);

            var sql = string.Empty;
            foreach (T info in list)
            {
                sql = Lib.MakeInsertSql(TableName, info);
                DbHelper.ExecuteNonQuery(trans, CommandType.Text, sql);
            }
        }

        /// <summary>
        /// 更新（使用外部事务容器管理事务）   added by xuc 2016-7-18  U
        /// </summary>
        /// <param name="info"></param>
        /// <param name="transContainer">事务管理容器，供dal中的事务注册，方便在外部如bll中统一进行commit rollback</param>
        /// <returns></returns>
        public bool Update<T>(T info, ITransactionContainer transContainer)
        {
            var conn = DbHelper.Factory.CreateConnection();
            conn.ConnectionString = DbHelper.ConnectionString;
            conn.Open();
            var trans = conn.BeginTransaction();

            transContainer.ResiterTransaction(trans);
            var sql = Lib.MakeUpdateSql(TableName, info);
            return DbHelper.ExecuteNonQuery(trans, CommandType.Text, sql) > 0;
        }


        /// <summary>
        /// 删除（使用外部事务容器管理事务）  added by xuc 2016-7-15  D
        /// </summary>
        /// <param name="id"></param>
        /// <param name="transContainer">事务管理容器，供dal中的事务注册，方便在外部如bll中统一进行commit rollback</param>
        /// <returns></returns>
        public bool Delete(int id, ITransactionContainer transContainer)
        {
            if (id <= 0) return false;

            var conn = DbHelper.Factory.CreateConnection();
            conn.ConnectionString = DbHelper.ConnectionString;
            conn.Open();
            var trans = conn.BeginTransaction();

            transContainer.ResiterTransaction(trans);
            var sql = string.Format("delete from {0} where ID = {1}", TableName, id);
            return DbHelper.ExecuteNonQuery(trans, CommandType.Text, sql) > 0;
        }

        /// <summary>
        ///  批量删除（使用外部事务容器管理事务） 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="transContainer">事务管理容器，供dal中的事务注册，方便在外部如bll中统一进行commit rollback</param>
        /// <returns></returns>
        public virtual bool BatchDelete(List<int> list, ITransactionContainer transContainer)
        {
            var conn = DbHelper.Factory.CreateConnection();
            conn.ConnectionString = DbHelper.ConnectionString;
            conn.Open();
            var trans = conn.BeginTransaction();

            transContainer.ResiterTransaction(trans);
            var sql = string.Empty;

            foreach (var id in list)
            {
                sql = string.Format("delete from {0} where ID = {1}", TableName, id);
                DbHelper.ExecuteNonQuery(trans, CommandType.Text, sql);
            }

            return true;
        }
        #endregion
    }



}
