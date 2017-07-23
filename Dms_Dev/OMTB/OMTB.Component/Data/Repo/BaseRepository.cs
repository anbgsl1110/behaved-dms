using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;

namespace OMTB.Component.Data.Repo
{
    public abstract class BaseRepository<T> : IRepository<T> where T : class
    {
        public IUnitOfWork UnitOfWork { get; protected set; }

        public BaseRepository(IUnitOfWork uw)
        {
            UnitOfWork = uw;
        }

        /// <summary>
        /// 对象校验
        /// </summary>
        /// <param name="entity">待校验对象</param>
        /// <param name="mode">校验模式。Create: 用于创建， Update: 用于修改</param>
        /// <returns></returns>
        protected virtual ValidateResult Validate(T entity, ValidateMode mode)
        {
            return new ValidateResult(true, string.Empty);
        }

        /// <summary>
        /// 创建对象。只创建通过校验方法的对象
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Create(T entity)
        {
            var validResult = Validate(entity, ValidateMode.Create);
            if (validResult.Status)
            {
                this.UnitOfWork.Connection.Insert<T>(entity, this.UnitOfWork.GetLastTransaction());
                return;
            }
            throw new ArgumentException(validResult.Message);
        }

        /// <summary>
        /// 创建对象并返回自增长的值。只创建通过校验方法的对象
        /// </summary>
        /// <param name="entity"></param>
        public virtual long CreateWithIdentity(T entity)
        {
            var validResult = Validate(entity, ValidateMode.Create);
            if (validResult.Status)
            {
                return this.UnitOfWork.Connection.Insert<T>(entity, this.UnitOfWork.GetLastTransaction());
            }
            throw new ArgumentException(validResult.Message);
        }

        /// <summary>
        /// 批量创建对象。不校验对象
        /// </summary>
        /// <param name="entities"></param>
        public virtual void Create(IEnumerable<T> entities)
        {
            this.UnitOfWork.Connection.Insert<T>(entities, this.UnitOfWork.GetLastTransaction());
        }

        /// <summary>
        /// 修改对象。只修改通过校验方法的对象
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Update(T entity)
        {
            var validResult = Validate(entity, ValidateMode.Update);
            if (validResult.Status)
            {
                this.UnitOfWork.Connection.Update<T>(entity, this.UnitOfWork.GetLastTransaction());
                return;
            }
            throw new ArgumentException(validResult.Message);
        }

        /// <summary>
        /// 修改对象部分字段的值。根据传入的对象属性决定修改内容。不校验对象
        /// </summary>
        /// <param name="entity"></param>
        public virtual void UpdatePart(object entity)
        {
            this.UnitOfWork.Connection.UpdatePart<T>(entity, this.UnitOfWork.GetLastTransaction());
        }

        public virtual void UpdatePart(object entity, Expression<bool> condition)
        {
            throw new NotImplementedException();
        }

        public virtual int UpdatePart(IDictionary<string, object> source, IDictionary<string, object> condition)
        {
            return this.UnitOfWork.Connection.UpdatePart<T>(source, condition, this.UnitOfWork.GetLastTransaction());
        }

        /// <summary>
        /// 删除对象
        /// </summary>
        /// <param name="id"></param>
        public virtual void Delete(long id)
        {
            this.UnitOfWork.Connection.Execute(string.Format("delete from {0} where id=@id", typeof(T).Name), new { Id = id });
        }

        public virtual void Delete(IEnumerable<long> id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取单个对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T Get(long id)
        {
            return this.UnitOfWork.Connection.Get<T>(id);
        }

        protected int Execute(string sql, object obj)
        {
            return this.UnitOfWork.Connection.Execute(sql, obj, this.UnitOfWork.GetLastTransaction());
        }

        protected long ExecuteScalar(string sql, object obj)
        {
            return this.UnitOfWork.Connection.ExecuteScalar<long>(sql, obj, this.UnitOfWork.GetLastTransaction());
        }

    }

    public class ValidateResult
    {
        public bool Status { get; set; }

        public string Message { get; set; }

        public ValidateResult(bool status, string message)
        {
            this.Status = status;
            this.Message = message;
        }

        public ValidateResult() { }

    }

    public enum ValidateMode
    {
        Create,
        Update
    }
}
