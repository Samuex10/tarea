<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MisInscripciones.aspx.cs" Inherits="UniCampus.Web.MisInscripciones" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>UniCampus | Mis inscripciones</title>
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
            <h1>Mis inscripciones</h1>
            <asp:GridView ID="gvInscripciones" runat="server" AutoGenerateColumns="False" CssClass="tabla"
                EmptyDataText="Aún no tiene inscripciones." OnRowCommand="gvInscripciones_RowCommand">
                <Columns>
                    <asp:BoundField DataField="ActividadTitulo" HeaderText="Actividad" />
                    <asp:BoundField DataField="FechaInicio" HeaderText="Inicio" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                    <asp:BoundField DataField="Lugar" HeaderText="Lugar" />
                    <asp:BoundField DataField="FechaInscripcion" HeaderText="Inscrito el" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                    <asp:BoundField DataField="Estado" HeaderText="Estado" />
                    <asp:HyperLinkField DataNavigateUrlFields="ActividadId" DataNavigateUrlFormatString="Detalle.aspx?id={0}" Text="Ver detalle" HeaderText="Detalle" />
                    <asp:TemplateField HeaderText="Acciones">
                        <ItemTemplate>
                            <asp:LinkButton runat="server" Text="Cancelar inscripción" CommandName="Cancelar"
                                CommandArgument='<%# Eval("ActividadId") %>' CausesValidation="false"
                                Visible='<%# Convert.ToString(Eval("Estado")) == "Activa" %>'
                                OnClientClick="return confirm('¿Cancelar su inscripción?');" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
</form>
</body>
</html>
