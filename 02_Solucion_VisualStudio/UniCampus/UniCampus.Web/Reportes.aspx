<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Reportes.aspx.cs" Inherits="UniCampus.Web.Reportes" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>UniCampus | Reportes</title>
    <meta charset="utf-8" />
    <link href="Content/site.css" rel="stylesheet" />
</head>
<body>
<form id="form1" runat="server">
    <div class="nav">
    <span class="marca">UniCampus</span>
    <a href="Actividades.aspx">Actividades</a>
    <% if (EsEstudiante) { %><a href="MisInscripciones.aspx">Mis inscripciones</a><% } %>
    <% if (EsAdmin) { %><a href="Reportes.aspx">Reportes</a><% } %>
    <span class="usuario"><%: NombreUsuario %> (<%: RolUsuario %>) &middot; <a href="Logout.aspx">Salir</a></span>
</div>
    <div class="contenedor">
        <asp:Label ID="lblMensaje" runat="server" CssClass="mensaje" EnableViewState="false" />
        <div class="panel">
            <h1>Resumen de actividades</h1>
            <p><b><asp:Label ID="lblTotales" runat="server" /></b></p>
            <asp:GridView ID="gvResumen" runat="server" AutoGenerateColumns="False" CssClass="tabla" EmptyDataText="No hay actividades registradas.">
                <Columns>
                    <asp:BoundField DataField="Titulo" HeaderText="Actividad" />
                    <asp:BoundField DataField="Estado" HeaderText="Estado" />
                    <asp:BoundField DataField="Cupos" HeaderText="Cupos" />
                    <asp:BoundField DataField="Inscritos" HeaderText="Inscritos" />
                    <asp:BoundField DataField="CuposDisponibles" HeaderText="Disponibles" />
                    <asp:BoundField DataField="TotalResenas" HeaderText="Reseñas" />
                    <asp:BoundField DataField="PromedioTexto" HeaderText="Promedio" />
                </Columns>
            </asp:GridView>
        </div>
    </div>
</form>
</body>
</html>
