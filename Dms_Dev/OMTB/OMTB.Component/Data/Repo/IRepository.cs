using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OMTB.Component.Data.Repo
{
    public interface IRepository<T> where T : class
    {
        void Create(T entity);

        long CreateWithIdentity(T entity);

        void Create(IEnumerable<T> entities);

        void Update(T entity);

        void UpdatePart(object entity);

        void UpdatePart(object entity, Expression<bool> condition);

        void Delete(long id);

        void Delete(IEnumerable<long> id);

        T Get(long id);
    }

}
