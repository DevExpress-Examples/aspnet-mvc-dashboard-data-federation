using DevExpress.DashboardCommon;
using DevExpress.DashboardWeb;
using DevExpress.DashboardWeb.Mvc;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.DataFederation;
using DevExpress.DataAccess.Excel;
using DevExpress.DataAccess.Sql;
using System.Web.Hosting;
using System.Web.Routing;

namespace MVC_DataFederationExample
{
    public static class DashboardConfig
    {
        public static void RegisterService(RouteCollection routes)
        {
            routes.MapDashboardRoute("dashboardControl");

            DashboardFileStorage dashboardFileStorage = new DashboardFileStorage("~/App_Data/Dashboards");
            DashboardConfigurator.Default.SetDashboardStorage(dashboardFileStorage);

            DashboardConfigurator.PassCredentials = true;

            // Uncomment this string to allow end users to create new data sources based on predefined connection strings.
            //DashboardConfigurator.Default.SetConnectionStringsProvider(new DevExpress.DataAccess.Web.ConfigFileConnectionStringsProvider());

            DataSourceInMemoryStorage dataSourceStorage = new DataSourceInMemoryStorage();

            // Registers an SQL data source.
            SQLiteConnectionParameters sqliteParams = new SQLiteConnectionParameters();
            sqliteParams.FileName = HostingEnvironment.MapPath(@"~/App_Data/nwind.db");

            DashboardSqlDataSource sqlDataSource = new DashboardSqlDataSource("SQLite Data Source", sqliteParams);
            SelectQuery selectQuery = SelectQueryFluentBuilder
                .AddTable("Orders")
                .SelectAllColumnsFromTable()
                .Build("SQLite Orders");
            sqlDataSource.Queries.Add(selectQuery);
            sqlDataSource.Fill();
            dataSourceStorage.RegisterDataSource("sqlDataSource", sqlDataSource.SaveToXml());

            // Registers an Object data source.
            DashboardObjectDataSource objDataSource = new DashboardObjectDataSource();
            objDataSource.Name = "ObjectDS";
            objDataSource.DataSource = DataGenerator.Data;
            objDataSource.Fill();

            dataSourceStorage.RegisterDataSource("objDataSource", objDataSource.SaveToXml());

            // Registers an Excel data source.
            DashboardExcelDataSource excelDataSource = new DashboardExcelDataSource("ExcelDS");
            excelDataSource.FileName = HostingEnvironment.MapPath(@"~/App_Data/SalesPerson.xlsx");
            excelDataSource.SourceOptions = new ExcelSourceOptions(new ExcelWorksheetSettings("Data"));
            excelDataSource.Fill();
            dataSourceStorage.RegisterDataSource("excelDataSource", excelDataSource.SaveToXml());

            // Registers the Federated data source.
            DashboardFederationDataSource federationDataSource = new DashboardFederationDataSource("Federated Data Source");
            Source sqlSource = new Source("sqlite", sqlDataSource, "SQLite Orders");
            Source excelSource = new Source("excel", excelDataSource, "");
            Source objectSource = new Source("SalesPersonDS", objDataSource, "");
            SelectNode mainQueryCreatedByNodeBuilder =
                sqlSource.From()
                .Select("OrderDate", "ShipCity", "ShipCountry")
                .Join(excelSource, "[excel.OrderID] = [sqlite.OrderID]")
                    .Select("CategoryName", "ProductName", "Extended Price")
                    .Join(objectSource, "[SalesPersonDS.SalesPerson] = [excel.Sales Person]")
                        .Select("SalesPerson", "Weight")
                        .Build("FDS");
            federationDataSource.Queries.Add(mainQueryCreatedByNodeBuilder);
            federationDataSource.CalculatedFields.Add("FDS", "[Weight] * [Extended Price] / 100", "Score");
            federationDataSource.Fill(new DevExpress.Data.IParameter[0]);
            dataSourceStorage.RegisterDataSource("federatedDataSource", federationDataSource.SaveToXml());


            DashboardConfigurator.Default.SetDataSourceStorage(dataSourceStorage);
            DashboardConfigurator.Default.DataLoading += DataLoading;
        }

        private static void DataLoading(object sender, DataLoadingWebEventArgs e)
        {
            if (e.DataSourceName == "ObjectDS")
            {
                e.Data = DataGenerator.CreateSourceData();
            }
        }
    }
}