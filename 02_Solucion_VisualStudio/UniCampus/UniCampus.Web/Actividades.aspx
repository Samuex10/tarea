<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Actividades.aspx.cs" Inherits="UniCampus.Web.Actividades" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>UniCampus | Actividades académicas</title>
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

        <asp:Panel ID="pnlFormulario" runat="server" CssClass="panel">
            <h2><asp:Label ID="lblModo" runat="server" Text="Nueva actividad" /></h2>
            <asp:HiddenField ID="hfActividadId" runat="server" />
            <div class="grid">
                <div class="full">
                    <label for="txtTitulo">Título *</label>
                    <asp:TextBox ID="txtTitulo" runat="server" CssClass="campo" MaxLength="120" />
                </div>
                <div class="full">
                    <label for="txtDescripcion">Descripción</label>
                    <asp:TextBox ID="txtDescripcion" runat="server" CssClass="campo" TextMode="MultiLine" Rows="3" MaxLength="1000" />
                </div>
                <div>
                    <label for="txtFechaInicio">Fecha y hora de inicio *</label>
                    <asp:TextBox ID="txtFechaInicio" runat="server" CssClass="campo" TextMode="DateTimeLocal" />
                </div>
                <div>
                    <label for="txtFechaFin">Fecha y hora de fin *</label>
                    <asp:TextBox ID="txtFechaFin" runat="server" CssClass="campo" TextMode="DateTimeLocal" />
                </div>
                <div>
                    <label for="txtLugar">Lugar</label>
                    <asp:TextBox ID="txtLugar" runat="server" CssClass="campo" MaxLength="150" />
                </div>
                <div>
                    <label for="txtCupos">Cupos *</label>
                    <asp:TextBox ID="txtCupos" runat="server" CssClass="campo" />
                </div>
                <div>
                    <label for="ddlTipo">Tipo *</label>
                    <asp:DropDownList ID="ddlTipo" runat="server" CssClass="campo" />
                </div>
                <div>
                    <label for="ddlEstado">Estado *</label>
                    <asp:DropDownList ID="ddlEstado" runat="server" CssClass="campo">
                        <asp:ListItem Text="Publicada" Value="Publicada" Selected="True" />
                        <asp:ListItem Text="Borrador" Value="Borrador" />
                        <asp:ListItem Text="Cancelada" Value="Cancelada" />
                        <asp:ListItem Text="Finalizada" Value="Finalizada" />
                    </asp:DropDownList>
                </div>
            </div>
            <asp:Button ID="btnGuardar" runat="server" Text="Guardar" CssClass="btn" OnClick="btnGuardar_Click" />
            <asp:Button ID="btnCancelarEdicion" runat="server" Text="Cancelar edición" CssClass="btn sec" OnClick="btnCancelarEdicion_Click" Visible="false" />
        </asp:Panel>

        <div class="panel">
            <h2>Actividades</h2>
            <div class="grid3">
                <div>
                    <label for="txtFiltroTexto">Buscar por nombre o descripción</label>
                    <asp:TextBox ID="txtFiltroTexto" runat="server" CssClass="campo" MaxLength="120" />
                </div>
                <div>
                    <label for="txtFiltroDesde">Desde</label>
                    <asp:TextBox ID="txtFiltroDesde" runat="server" CssClass="campo" TextMode="Date" />
                </div>
                <div>
                    <label for="txtFiltroHasta">Hasta</label>
                    <asp:TextBox ID="txtFiltroHasta" runat="server" CssClass="campo" TextMode="Date" />
                </div>
                <div>
                    <label for="ddlFiltroTipo">Tipo</label>
                    <asp:DropDownList ID="ddlFiltroTipo" runat="server" CssClass="campo" />
                </div>
                <asp:Panel ID="pnlFiltroEstado" runat="server">
                    <label for="ddlFiltroEstado">Estado</label>
                    <asp:DropDownList ID="ddlFiltroEstado" runat="server" CssClass="campo">
                        <asp:ListItem Text="Todos" Value="" />
                        <asp:ListItem Text="Publicada" Value="Publicada" />
                        <asp:ListItem Text="Borrador" Value="Borrador" />
                        <asp:ListItem Text="Cancelada" Value="Cancelada" />
                        <asp:ListItem Text="Finalizada" Value="Finalizada" />
                    </asp:DropDownList>
                </asp:Panel>
            </div>
            <asp:Button ID="btnBuscar" runat="server" Text="Buscar" CssClass="btn" OnClick="btnBuscar_Click" />
            <asp:Button ID="btnLimpiarFiltros" runat="server" Text="Limpiar filtros" CssClass="btn sec" OnClick="btnLimpiarFiltros_Click" />
            <br /><br />
            <asp:GridView ID="gvActividades" runat="server" AutoGenerateColumns="False" CssClass="tabla"
                EmptyDataText="No se encontraron actividades." OnRowCommand="gvActividades_RowCommand">
                <Columns>
                    <asp:BoundField DataField="ActividadId" HeaderText="#" />
                    <asp:BoundField DataField="Titulo" HeaderText="Actividad" />
                    <asp:BoundField DataField="TipoNombre" HeaderText="Tipo" />
                    <asp:BoundField DataField="FechaInicio" HeaderText="Inicio" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                    <asp:BoundField DataField="Lugar" HeaderText="Lugar" />
                    <asp:BoundField DataField="Cupos" HeaderText="Cupos" />
                    <asp:BoundField DataField="CuposDisponibles" HeaderText="Disponibles" />
                    <asp:BoundField DataField="Estado" HeaderText="Estado" />
                    <asp:HyperLinkField DataNavigateUrlFields="ActividadId" DataNavigateUrlFormatString="Detalle.aspx?id={0}" Text="Ver detalle" HeaderText="Detalle" />
                    <asp:TemplateField HeaderText="Acciones">
                        <ItemTemplate>
                            <asp:LinkButton runat="server" Text="Editar" CommandName="Editar" CommandArgument='<%# Eval("ActividadId") %>' CausesValidation="false" />
                            <asp:LinkButton runat="server" Text="Desactivar" CommandName="Desactivar" CommandArgument='<%# Eval("ActividadId") %>' CausesValidation="false" OnClientClick="return confirm('¿Desactivar esta actividad?');" />
                            <asp:LinkButton runat="server" Text="Eliminar" CommandName="Eliminar" CommandArgument='<%# Eval("ActividadId") %>' CausesValidation="false" OnClientClick="return confirm('¿Eliminar definitivamente esta actividad?');" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
</form>
</body>
</html>
