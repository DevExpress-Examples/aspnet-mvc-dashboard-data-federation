<!-- default file list -->
*Files to look at*:

* [DashboardConfig.cs](./CS/MVC_DataFederationExample/App_Start/DashboardConfig.cs) (VB: [DashboardConfig.vb](./VB/MVC_DataFederationExample/App_Start/DashboardConfig.vb))
<!-- default file list end -->

# ASP.NET MVC Dashboard - How to Bind a Dashboard to a Federated Data Source Created at Runtime

This example creates the following [data sources](https://docs.devexpress.com/Dashboard/116522) at runtime:

* [DashboardSqlDataSource](https://docs.devexpress.com/Dashboard/DevExpress.DashboardCommon.DashboardSqlDataSource) connected to the [SQLite database](https://docs.devexpress.com/Dashboard/113925)
* [DashboardExcelDataSource](https://docs.devexpress.com/Dashboard/DevExpress.DashboardCommon.DashboardExcelDataSource)
* [DashboardObjectDataSource](https://docs.devexpress.com/Dashboard/16133)

Subsequently the [DashboardFederationDataSource](https://docs.devexpress.com/Dashboard/DevExpress.DashboardCommon.DashboardFederationDataSource) is created to integrate the existing data sources.

The data sources are serialized and stored in the in-memory storage ([DataSourceInMemoryStorage](https://docs.devexpress.com/Dashboard/DevExpress.DashboardWeb.DataSourceInMemoryStorage)).

See also:

* [Register Default Data Sources](https://docs.devexpress.com/Dashboard/16980)
