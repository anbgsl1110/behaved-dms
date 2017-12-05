using System;
using System.Collections.Generic;
using System.Data;

namespace DbDocGenerate.Data
{
    /// <summary>
    /// 自定义业务层事务管理器  added by xuc 2016-7-15
    /// </summary>
    public interface ITransactionContainer : IDisposable
    {
        List<IDbTransaction> trans { get; }

        /// <summary>
        /// 注册事务
        /// </summary>
        void ResiterTransaction(IDbTransaction trans);

        /// <summary>
        /// 提交所有事务
        /// </summary>
        void Commit();

        /// <summary>
        /// 回滚所有事务
        /// </summary>
        void Rollback();

    }


    public class TransactionContainer : ITransactionContainer
    {
        /// <summary>
        /// 初始化业务层事务管理容器
        /// </summary>
        public TransactionContainer()
        {
            trans = new List<IDbTransaction>();
        }

        public void Dispose()
        {
            foreach (var tran in trans)
            {
                if (tran.Connection != null)
                {
                    if (tran.Connection.State == ConnectionState.Open)
                        tran.Connection.Close();

                    tran.Connection.Dispose();
                }

                tran.Dispose();
            }
        }

        public List<IDbTransaction> trans { get; private set; }
        public void ResiterTransaction(IDbTransaction dbTrans)
        {
            trans.Add(dbTrans);
        }

        public void Commit()
        {
            foreach (var tran in trans)
            {
                tran.Commit();
            }
        }

        public void Rollback()
        {
            foreach (var tran in trans)
            {
                tran.Rollback();
            }
        }
    }
}