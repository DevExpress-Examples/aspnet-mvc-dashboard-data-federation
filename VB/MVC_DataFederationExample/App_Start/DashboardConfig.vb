Imports DevExpress.DashboardCommon
Imports DevExpress.DashboardWeb
Imports DevExpress.DashboardWeb.Mvc
Imports DevExpress.DataAccess.ConnectionParameters
Imports DevExpress.DataAccess.DataFederation
Imports DevExpress.DataAccess.Excel
Imports DevExpress.DataAccess.Sql
Imports System.Web.Hosting
Imports System.Web.Routing

Namespace MVC_DataFederationExample
	Public NotInheritable Class DashboardConfig

		Private Sub New()
		End Sub

		Public Shared Sub RegisterService(ByVal routes As RouteCollection)
			routes.MapDashboardRoute("dashboardControl")

			Dim dashboardFileStorage As New DashboardFileStorage("~/App_Data/Dashboards")
			DashboardConfigurator.Default.SetDashboardStorage(dashboardFileStorage)

			DashboardConfigurator.PassCredentials = True

			' Uncomment this string to allow end users to create new data sources based on predefined connection strings.
			'DashboardConfigurator.Default.SetConnectionStringsProvider(new DevExpress.DataAccess.Web.ConfigFileConnectionStringsProvider());

			Dim dataSourceStorage As New DataSourceInMemoryStorage()

			' Registers an SQL data source.
			Dim sqliteParams As New SQLiteConnectionParameters()
			sqliteParams.FileName = HostingEnvironment.MapPath("~/App_Data/nwind.db")

			Dim sqlDataSource As New DashboardSqlDataSource("SQLite Data Source", sqliteParams)
			Dim selectQuery As SelectQuery = SelectQueryFluentBuilder.AddTable("Orders").SelectAllColumnsFromTable().Build("SQLite Orders")
			sqlDataSource.Queries.Add(selectQuery)
			sqlDataSource.Fill()
			dataSourceStorage.RegisterDataSource("sqlDataSource", sqlDataSource.SaveToXml())

			' Registers an Object data source.
			Dim objDataSource As New DashboardObjectDataSource()
			objDataSource.Name = "ObjectDS"
			objDataSource.DataSource = DataGenerator.Data
			objDataSource.Fill()

			dataSourceStorage.RegisterDataSource("objDataSource", objDataSource.SaveToXml())

			' Registers an Excel data source.
			Dim excelDataSource As New DashboardExcelDataSource("ExcelDS")
			excelDataSource.FileName = HostingEnvironment.MapPath("~/App_Data/SalesPerson.xlsx")
			excelDataSource.SourceOptions = New ExcelSourceOptions(New ExcelWorksheetSettings("Data"))
			excelDataSource.Fill()
			dataSourceStorage.RegisterDataSource("excelDataSource", excelDataSource.SaveToXml())

			' Registers the Federated data source.
			Dim federationDataSource As New DashboardFederationDataSource("Federated Data Source")
			Dim sqlSource As New Source("sqlite", sqlDataSource, "SQLite Orders")
			Dim excelSource As New Source("excel", excelDataSource, "")
			Dim objectSource As New Source("SalesPersonDS", objDataSource, "")
			Dim mainQueryCreatedByNodeBuilder As SelectNode = sqlSource.From().Select("OrderDate", "ShipCity", "ShipCountry").Join(excelSource, "[excel.OrderID] = [sqlite.OrderID]").Select("CategoryName", "ProductName", "Extended Price").Join(objectSource, "[SalesPersonDS.SalesPerson] = [excel.Sales Person]").Select("SalesPerson", "Weight").Build("FDS")
			federationDataSource.Queries.Add(mainQueryCreatedByNodeBuilder)
			federationDataSource.CalculatedFields.Add("FDS", "[Weight] * [Extended Price] / 100", "Score")
			federationDataSource.Fill(New DevExpress.Data.IParameter(){})
			dataSourceStorage.RegisterDataSource("federatedDataSource", federationDataSource.SaveToXml())


			DashboardConfigurator.Default.SetDataSourceStorage(dataSourceStorage)
			AddHandler DashboardConfigurator.Default.DataLoading, AddressOf DataLoading
		End Sub

		Private Shared Sub DataLoading(ByVal sender As Object, ByVal e As DataLoadingWebEventArgs)
			If e.DataSourceName = "ObjectDS" Then
				e.Data = DataGenerator.CreateSourceData()
			End If
		End Sub
	End Class
End Namespace