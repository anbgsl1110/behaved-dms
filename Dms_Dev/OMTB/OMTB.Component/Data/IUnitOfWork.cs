using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMTB.Component.Data
{
    public interface IUnitOfWork : IDisposable
    {
        IDbConnection Connection { get; }

        void BeginTran();

        void Commit();

        void Rollback();

        IDbTransaction GetLastTransaction();
    }
}
