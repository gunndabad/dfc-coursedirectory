using System;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using Microsoft.SqlServer.Dac;

namespace Dfc.CourseDirectory.Core
{
    public class SqlDeployHelper
    {
        public void Deploy(string connectionString, Action<string> writeMessage)
        {
            var dacServices = new DacServices(connectionString);

            dacServices.ProgressChanged += DacServices_ProgressChanged;

            try
            {
                var binFolder = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                const string dacpacFile = "Dfc.CourseDirectory.Database.dacpac";
                var fullDacpacPath = Path.Combine(binFolder, dacpacFile);
                using var dacpac = DacPackage.Load(fullDacpacPath);

                var databaseName = GetDatabaseNameFromConnectionString();

                dacServices.Deploy(
                    dacpac,
                    databaseName,
                    upgradeExisting: true,
                    options: new DacDeployOptions()
                    {
                        BlockOnPossibleDataLoss = false
                    });
            }
            finally
            {
                dacServices.ProgressChanged -= DacServices_ProgressChanged;
            }

            void DacServices_ProgressChanged(object sender, DacProgressEventArgs e) => writeMessage?.Invoke(e.Message);

            string GetDatabaseNameFromConnectionString() =>
                new SqlConnectionStringBuilder(connectionString).InitialCatalog;
        }
    }
}
