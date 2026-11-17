<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Detalle.aspx.cs" Inherits="UniCampus.Web.Detalle" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>UniCampus | Detalle de actividad</title>
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

        <asp:Panel ID="pnlContenido" runat="server" Visible="false">
            <div class="panel">
                <h1><asp:Label ID="lblTitulo" runat="server" /></h1>
                <p><span class="etiqueta"><asp:Label ID="lblTipo" runat="server" /></span>
                   <span class="etiqueta"><asp:Label ID="lblEstado" runat="server" /></span></p>
                <p class="dato"><b>Fecha y hora:</b> <asp:Label ID="lblFechas" runat="server" /></p>
                <p class="dato"><b>Lugar:</b> <asp:Label ID="lblLugar" runat="server" /></p>
                <p class="dato"><b>Cupos:</b> <asp:Label ID="lblCupos" runat="server" /></p>
                <p class="dato"><b>Descripción:</b><br /><asp:Label ID="lblDescripcion" runat="server" /></p>
                <asp:Button ID="btnInscribirse" runat="server" Text="Inscribirme" CssClass="btn verde" OnClick="btnInscribirse_Click" Visible="false" />
                <asp:Button ID="btnCancelarInscripcion" runat="server" Text="Cancelar mi inscripción" CssClass="btn rojo" OnClick="btnCancelarInscripcion_Click" Visible="false" OnClientClick="return confirm('¿Cancelar su inscripción?');" />
            </div>

            <asp:Panel ID="pnlInscritos" runat="server" CssClass="panel" Visible="false">
                <h2>Inscritos</h2>
                <asp:GridView ID="gvInscritos" runat="server" AutoGenerateColumns="False" CssClass="tabla" EmptyDataText="Aún no hay inscritos.">
                    <Columns>
                        <asp:BoundField DataField="UsuarioNombre" HeaderText="Estudiante" />
                        <asp:BoundField DataField="UsuarioCorreo" HeaderText="Correo" />
                        <asp:BoundField DataField="FechaInscripcion" HeaderText="Fecha de inscripción" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                        <asp:BoundField DataField="Estado" HeaderText="Estado" />
                    </Columns>
                </asp:GridView>
            </asp:Panel>

            <div class="panel">
                <h2>Reseñas</h2>
                <p><asp:Label ID="lblPromedio" runat="server" /></p>
                <asp:Panel ID="pnlResena" runat="server" Visible="false">
                    <div class="grid">
                        <div>
                            <label for="ddlCalificacion">Calificación (1 a 5)</label>
                            <asp:DropDownList ID="ddlCalificacion" runat="server" CssClass="campo">
                                <asp:ListItem Text="Seleccione..." Value="" />
                                <asp:ListItem Text="1" Value="1" />
                                <asp:ListItem Text="2" Value="2" />
                                <asp:ListItem Text="3" Value="3" />
                                <asp:ListItem Text="4" Value="4" />
                                <asp:ListItem Text="5" Value="5" />
                            </asp:DropDownList>
                        </div>
                        <div class="full">
                            <label for="txtComentario">Comentario</label>
                            <asp:TextBox ID="txtComentario" runat="server" CssClass="campo" TextMode="MultiLine" Rows="3" MaxLength="1000" />
                        </div>
                    </div>
                    <asp:Button ID="btnPublicarResena" runat="server" Text="Publicar reseña" CssClass="btn" OnClick="btnPublicarResena_Click" />
                </asp:Panel>
                <asp:GridView ID="gvResenas" runat="server" AutoGenerateColumns="False" CssClass="tabla" EmptyDataText="Esta actividad aún no tiene reseñas.">
                    <Columns>
                        <asp:BoundField DataField="UsuarioNombre" HeaderText="Estudiante" />
                        <asp:BoundField DataField="Calificacion" HeaderText="Calificación" />
                        <asp:BoundField DataField="Comentario" HeaderText="Comentario" />
                        <asp:BoundField DataField="Fecha" HeaderText="Fecha" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                    </Columns>
                </asp:GridView>
            </div>
        </asp:Panel>
        <p><a href="Actividades.aspx">&larr; Volver a actividades</a></p>
    </div>
</form>
</body>
</html>
