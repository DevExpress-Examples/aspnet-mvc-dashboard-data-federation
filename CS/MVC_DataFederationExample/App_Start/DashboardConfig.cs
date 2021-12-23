using DevExpress.DashboardCommon;
using DevExpress.DashboardWeb;
using DevExpress.DashboardWeb.Mvc;
using DevExpress.DataAccess.DataFederation;
using DevExpress.DataAccess.Excel;
using DevExpress.DataAccess.Json;
using DevExpress.DataAccess.Sql;
using System;
using System.Web.Hosting;
using System.Web.Routing;

namespace MVC_DataFederationExample {
    public static class DashboardConfig {
        public static void RegisterService(RouteCollection routes) {
            routes.MapDashboardRoute("dashboardControl", "DefaultDashboard");

            DashboardFileStorage dashboardFileStorage = new DashboardFileStorage("~/App_Data/Dashboards");
            DashboardConfigurator.Default.SetDashboardStorage(dashboardFileStorage);

            DashboardConfigurator.PassCredentials = true;

            // Uncomment this string to allow end users to create new data sources based on predefined connection strings.
            //DashboardConfigurator.Default.SetConnectionStringsProvider(new DevExpress.DataAccess.Web.ConfigFileConnectionStringsProvider());

            DataSourceInMemoryStorage dataSourceStorage = new DataSourceInMemoryStorage();

            // Configures an SQL data source.
            DashboardSqlDataSource sqlDataSource = new DashboardSqlDataSource("SQL Data Source", "NWindConnectionString");
            SelectQuery query = SelectQueryFluentBuilder
                .AddTable("Orders")
                .SelectAllColumnsFromTable()
                .Build("SQL Orders");
            sqlDataSource.Queries.Add(query);

            // Configures an Object data source.
            DashboardObjectDataSource objDataSource = new DashboardObjectDataSource("Object Data Source");
            objDataSource.DataId = "odsInvoices";

            // Configures an Excel data source.
            DashboardExcelDataSource excelDataSource = new DashboardExcelDataSource("Excel Data Source");
            excelDataSource.ConnectionName = "excelSales";
            excelDataSource.FileName = HostingEnvironment.MapPath(@"~/App_Data/SalesPerson.xlsx");
            excelDataSource.SourceOptions = new ExcelSourceOptions(new ExcelWorksheetSettings("Data"));

            // Configures a JSON data source.
            DashboardJsonDataSource jsonDataSource = new DashboardJsonDataSource("JSON Data Source");
            jsonDataSource.ConnectionName = "jsonCategories";
            Uri fileUri = new Uri(HostingEnvironment.MapPath(@"~/App_Data/Categories.json"), UriKind.RelativeOrAbsolute);
            jsonDataSource.JsonSource = new UriJsonSource(fileUri);

            // Registers a Federated data source.
            dataSourceStorage.RegisterDataSource("federatedDataSource", CreateFederatedDataSource(sqlDataSource,
                excelDataSource, objDataSource, jsonDataSource).SaveToXml());

            DashboardConfigurator.Default.SetDataSourceStorage(dataSourceStorage);
            DashboardConfigurator.Default.DataLoading += DataLoading;
            DashboardConfigurator.Default.ConfigureDataConnection += Default_ConfigureDataConnection;
        }

        private static void Default_ConfigureDataConnection(object sender, ConfigureDataConnectionWebEventArgs e) {
            if (e.ConnectionName == "excelSales") {
                (e.ConnectionParameters as ExcelDataSourceConnectionParameters).FileName = HostingEnvironment.MapPath(@"~/App_Data/SalesPerson.xlsx");
            }
            else if (e.ConnectionName == "jsonCategories") {
                UriJsonSource uriSource = (e.ConnectionParameters as JsonSourceConnectionParameters).JsonSource as UriJsonSource;
                uriSource.Uri = new Uri(HostingEnvironment.MapPath(@"~/App_Data/Categories.json"), UriKind.RelativeOrAbsolute);
            }
        }

        private static void DataLoading(object sender, DataLoadingWebEventArgs e) {
            if (e.DataId == "odsInvoices") {
                e.Data = Invoices.CreateData();
            }
        }

        private static DashboardFederationDataSource CreateFederatedDataSource(DashboardSqlDataSource sqlDS,
            DashboardExcelDataSource excelDS, DashboardObjectDataSource objDS, DashboardJsonDataSource jsonDS) {

            DashboardFederationDataSource federationDataSource = new DashboardFederationDataSource("Federated Data Source");

            Source sqlSource = new Source("sqlSource", sqlDS, "SQL Orders");
            Source excelSource = new Source("excelSource", excelDS, "");
            Source objectSource = new Source("objectSource", objDS, "");
            SourceNode jsonSourceNode = new SourceNode(new Source("json", jsonDS, ""));

            // Join
            SelectNode joinQuery =
            sqlSource.From()
            .Select("OrderDate", "ShipCity", "ShipCountry")
            .Join(excelSource, "[excelSource.OrderID] = [sqlSource.OrderID]")
                .Select("CategoryName", "ProductName", "Extended Price")
                .Join(objectSource, "[objectSource.Country] = [excelSource.Country]")
                    .Select("Country", "UnitPrice")
                    .Build("Join query");
            federationDataSource.Queries.Add(joinQuery);

            // Union and UnionAll
            UnionNode queryUnionAll = sqlSource.From().Select("OrderID", "OrderDate").Build("OrdersSqlite")
                .UnionAll(excelSource.From().Select("OrderID", "OrderDate").Build("OrdersExcel"))
                .Build("OrdersUnionAll");
            queryUnionAll.Alias = "Union query";

            UnionNode queryUnion = sqlSource.From().Select("OrderID", "OrderDate").Build("OrdersSqlite")
                .Union(excelSource.From().Select("OrderID", "OrderDate").Build("OrdersExcel"))
                .Build("OrdersUnion");
            queryUnion.Alias = "UnionAll query";

            federationDataSource.Queries.Add(queryUnionAll);
            federationDataSource.Queries.Add(queryUnion);

            // Transformation
            TransformationNode unfoldNode = new TransformationNode(jsonSourceNode) {
                Alias = "Unfold",
                Rules = { new TransformationRule { ColumnName = "Products", Alias = "Product", Unfold = true, Flatten = false } }
            };

            TransformationNode unfoldFlattenNode = new TransformationNode(jsonSourceNode) {
                Alias = "Unfold and Flatten",
                Rules = { new TransformationRule { ColumnName = "Products", Unfold = true, Flatten = true } }
            };

            federationDataSource.Queries.Add(unfoldNode);
            federationDataSource.Queries.Add(unfoldFlattenNode);

            return federationDataSource;
        }
    }
}