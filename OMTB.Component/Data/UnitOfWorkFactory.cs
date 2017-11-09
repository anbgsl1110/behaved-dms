using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace OMTB.Component.Data
{
    public static class UnitOfWorkFactory
    {
        private static ConnectionOption ConnectionOption;

        static UnitOfWorkFactory()
        {
            ConnectionOption = new ConnectionOption();
            foreach (ConnectionStringSettings con in ConfigurationManager.ConnectionStrings)
            {
                if (con.ElementInformation.IsPresent)
                {
                    if (con.Name.ToLower().Contains("write"))
                    {
                        ConnectionOption.Masters.Add(con.Name, 0);
                    }
                    else
                    {
                        ConnectionOption.Slaves.Add(con.Name, 0);
                    }
                }
            }
         }

        public static IUnitOfWork GetUnitOfWork(UnitOfWorkType unitOfWorkType = UnitOfWorkType.ReadOnly)
        {
            IUnitOfWork unitOfWork;
            if (unitOfWorkType == UnitOfWorkType.ReadOnly)
            {
                var con = ConnectionOption.Slaves.OrderBy(s => s.Value).First();
                ConnectionOption.Slaves[con.Key] = con.Value + 1;
                unitOfWork = new DefaultUnitOfWork(ConfigurationManager.ConnectionStrings[con.Key].ConnectionString, con.Key);
            }
            else
            {
                var con = ConnectionOption.Masters.OrderBy(s => s.Value).First();
                ConnectionOption.Masters[con.Key] = con.Value + 1;
                unitOfWork = new DefaultUnitOfWork(ConfigurationManager.ConnectionStrings[con.Key].ConnectionString, con.Key);
            }

            return unitOfWork;
        }

        public static void RebackUnitOfWork(string connectionName)
        {
            if (ConnectionOption.Masters.ContainsKey(connectionName))
            {
                ConnectionOption.Masters[connectionName] = ConnectionOption.Masters[connectionName] - 1;
            }
            else if (ConnectionOption.Slaves.ContainsKey(connectionName))
            {
                ConnectionOption.Slaves[connectionName] = ConnectionOption.Slaves[connectionName] - 1;
            }
        }
    }

    public enum UnitOfWorkType
    {
        ReadOnly,
        ReadWrite
    }

    public class ConnectionOption
    {
        public IDictionary<string, int> Masters { get; set; }
        public IDictionary<string, int> Slaves { get; set; }

        public ConnectionOption()
        {
            Masters = new Dictionary<string, int>();
            Slaves = new Dictionary<string, int>();
        }
    }
}