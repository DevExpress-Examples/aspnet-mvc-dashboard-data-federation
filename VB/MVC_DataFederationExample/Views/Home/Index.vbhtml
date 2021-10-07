<script type="text/javascript">
    function onBeforeRender(sender) {
        var control = sender.GetDashboardControl();
        control.registerExtension(new DevExpress.Dashboard.DashboardPanelExtension(control, { dashboardThumbnail: "./Content/DashboardThumbnail/{0}.png" }));
    }
</script>
@Html.DevExpress().Dashboard(
                Sub(settings)
                    settings.Name = "Dashboard"
                    settings.ControllerName = "DefaultDashboard"
                    settings.Width = Unit.Percentage(100)
                    settings.Height = Unit.Percentage(100)
                    settings.ClientSideEvents.BeforeRender = "onBeforeRender"
                End Sub).GetHtml()