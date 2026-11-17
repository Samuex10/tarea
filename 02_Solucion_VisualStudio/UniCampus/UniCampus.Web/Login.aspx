<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="UniCampus.Web.Login" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>UniCampus | Ingreso</title>
    <meta charset="utf-8" />
    <link href="Content/site.css" rel="stylesheet" />
</head>
<body>
<form id="form1" runat="server" defaultbutton="btnIngresar">
    <div class="panel login">
        <h1>UniCampus</h1>
        <p>Sistema de gestión de actividades académicas</p>
        <asp:Label ID="lblMensaje" runat="server" CssClass="mensaje" EnableViewState="false" />
        <label for="txtCorreo">Correo</label>
        <asp:TextBox ID="txtCorreo" runat="server" CssClass="campo" MaxLength="150" />
        <label for="txtClave">Contraseña</label>
        <asp:TextBox ID="txtClave" runat="server" CssClass="campo" TextMode="Password" MaxLength="100" />
        <p><asp:Button ID="btnIngresar" runat="server" Text="Ingresar" CssClass="btn" OnClick="btnIngresar_Click" /></p>
    </div>
</form>
</body>
</html>
