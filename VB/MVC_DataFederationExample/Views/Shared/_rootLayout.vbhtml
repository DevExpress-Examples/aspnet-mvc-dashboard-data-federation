<!DOCTYPE html>

<html>
<head>
    <meta charset="UTF-8" />
    <title>Dashboard Web Application</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no" />
    
    @Html.DevExpress().GetStyleSheets(
        New StyleSheet With {.ExtensionSuite = ExtensionSuite.Dashboard }
    )
    @Html.DevExpress().GetScripts(
        New Script With {.ExtensionSuite = ExtensionSuite.Dashboard }
    )
    <link href="@Url.Content("~/Content/Site.css")" rel="stylesheet" type="text/css" />
</head>
<body>
    @Html.DevExpress().Splitter(Sub(settings)
        settings.Name = "MainSplitter"
        settings.AllowResize = False
        settings.Orientation = System.Web.UI.WebControls.Orientation.Vertical
        settings.FullscreenMode = True
        settings.SeparatorVisible = False
        settings.Styles.Pane.Border.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(0)
        settings.Styles.Pane.Paddings.Padding = System.Web.UI.WebControls.Unit.Pixel(0)

        settings.Panes.Add(Sub(pane)
            pane.Name = "Content"
            pane.PaneStyle.CssClass = "mainContentPane"
            pane.MinSize = System.Web.UI.WebControls.Unit.Pixel(375)
            pane.PaneStyle.BackColor = System.Drawing.Color.White
            pane.PaneStyle.BorderBottom.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(1)
            pane.SetContent(RenderBody().ToHtmlString())
        End Sub)

    End Sub).GetHtml()
</body>
</html>