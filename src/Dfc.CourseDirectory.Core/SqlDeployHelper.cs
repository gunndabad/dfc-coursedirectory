using System;
using System.Data.SqlClient;
using System.IO;
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
                using var dacpac = DacPackage.Load("Dfc.CourseDirectory.Database.dacpac");

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
