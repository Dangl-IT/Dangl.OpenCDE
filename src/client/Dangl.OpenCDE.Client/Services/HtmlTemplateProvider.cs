namespace Dangl.OpenCDE.Client.Services
{
    public static class HtmlTemplateProvider
    {
        public static string GetHtmlContent(string scriptSection, string bodyHtml, string title)
        {
            var template = $@"﻿<!DOCTYPE html><!DOCTYPE html>

<html lang=""en"" xmlns=""http://www.w3.org/1999/xhtml"">
<head>
    <meta charset=""utf-8"" />
    <title>
        {title}
    </title>
    <style tyle=""text/css"">
        body {{
            text-align: center;
            padding: 150px;
            font: 20px Helvetica, sans-serif;
            color: #333;
        }}
        h1 {{
            font-size: 50px;
        }}
        div {{
            display: block;
            text-align: left;
            width: 650px;
            margin: 0 auto;
        }}
    </style>
    <script>
        {scriptSection}
    </script>
</head>
<body>
<h1>{title}</h1>
<div>
    {bodyHtml}
</div>
</body>
</html>";

            return template;
        }
    }
}
