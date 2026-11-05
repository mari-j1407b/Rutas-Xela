using System.Globalization;

namespace RutasXela.UI;

// ─────────────────────────────────────────────────────────────
//  LÓGICA de la ventana. El DISEÑO VISUAL está en MainForm.Designer.cs
//  (ábrelo con el diseñador de Visual Studio para editarlo).
// ─────────────────────────────────────────────────────────────
public partial class MainForm : Form
{
    private readonly IRutasService _svc = new RutasServiceReal();

    // Los ComboBox viven dentro de los RoundedComboInput del diseñador
    private ComboBox _cO, _cD;
    private RoundedPanel _detalleParadaPanel;
    private Label _detalleParadaTexto;
    private Parada _paradaSeleccionada;

    public MainForm()
    {
        InitializeComponent();   // ← construye todo el diseño (MainForm.Designer.cs)

        // Conexiones a controles internos (el diseñador no puede guardarlas)
        _cO = cmbOrigen;
        _cD = cmbDestino;
        CrearPanelDetalleParada();
        panelDerecho.Resize += (_, _) => PosicionarPanelDetalle();
        _map.ParadaSeleccionada += SeleccionarParada;
        _map.RutasSeleccionadas += SeleccionarRuta;
        txtBuscar.TextChanged += (_, _) => Sugerir();
        txtBuscar.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter && _sug.Items.Count > 0) { _sug.SelectedIndex = 0; e.SuppressKeyPress = true; }
        };

        Refrescar();
        Log("Listo. Datos cargados desde Rutas-Xela.");
    }

    // ───────────────────────── EVENTOS (enlazados desde el diseñador) ─────────────────────────
    private void Header_Paint(object sender, PaintEventArgs e)
    {
        int w = panelEncabezado.Width / Theme.Lineas.Length + 1;
        for (int i = 0; i < Theme.Lineas.Length; i++)
        {
            using var b = new SolidBrush(Theme.Lineas[i]);
            e.Graphics.FillRectangle(b, i * w, panelEncabezado.Height - 5, w, 5);
        }
    }

    private void Header_Resize(object sender, EventArgs e)
    {
        btnCsv.Left = panelEncabezado.Width - btnCsv.Width - 24;
        panelEncabezado.Invalidate();
    }

    private void BtnCsv_Click(object sender, EventArgs e) => CargarCsv();
    private void BtnCalcular_Click(object sender, EventArgs e) => Calcular();
    private void BtnZona_Click(object sender, EventArgs e) => VerificarZona();
    private void BtnBfs_Click(object sender, EventArgs e) => Recorrer(true);
    private void BtnDfs_Click(object sender, EventArgs e) => Recorrer(false);
    private void BtnAgregarParada_Click(object sender, EventArgs e) => AgregarParada();
    private void BtnAgregarRuta_Click(object sender, EventArgs e) => AgregarRuta();
    private void Sug_SelectedIndexChanged(object sender, EventArgs e) => MostrarSeleccion();

    // ───────────────────────── LÓGICA DE LA UI ─────────────────────────
    private void Log(string msg) => txtRegistroActividad.AppendText($"[{DateTime.Now:HH:mm:ss}] {msg}{Environment.NewLine}");

    private string Nombre(string codigo) => _svc.BuscarParada(codigo)?.Nombre ?? codigo;

    private void Refrescar()
    {
        var ps = _svc.ObtenerParadas();
        _map.Paradas = ps;
        _map.Rutas = _svc.ObtenerRutas();
        _map.Invalidate();
        if (_paradaSeleccionada != null)
        {
            Parada actualizada = _svc.BuscarParada(_paradaSeleccionada.Codigo);
            if (actualizada == null)
            {
                SeleccionarParada(null);
            }
            else
            {
                _paradaSeleccionada = actualizada;
                ActualizarDetalleParada();
            }
        }
        LlenarCombo(_cO, ps, 0);
        LlenarCombo(_cD, ps, 1);
    }

    private static void LlenarCombo(ComboBox cb, Parada[] ps, int defecto)
    {
        string prev = (cb.SelectedItem as Parada)?.Codigo;
        cb.Items.Clear();
        cb.Items.AddRange(ps);
        int idx = prev == null ? -1 : Array.FindIndex(ps, p => p.Codigo == prev);
        if (idx < 0) idx = Math.Min(defecto, ps.Length - 1);
        if (idx >= 0) cb.SelectedIndex = idx;
    }

    private bool Seleccion(out string o, out string d)
    {
        o = (_cO.SelectedItem as Parada)?.Codigo;
        d = (_cD.SelectedItem as Parada)?.Codigo;
        if (o == null || d == null) { MessageBox.Show("Selecciona origen y destino."); return false; }
        return true;
    }

    private static bool Dbl(string s, out double v) =>
        double.TryParse(s.Trim().Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out v) &&
        double.IsFinite(v);

    // — Buscar / autocompletado (Trie + Tabla Hash) —
    private void Sugerir()
    {
        _sug.Items.Clear();
        string t = txtBuscar.Text.Trim();
        if (t.Length == 0) { _info.Text = "Las sugerencias aparecerán mientras escribes."; return; }
        _sug.Items.AddRange(_svc.Autocompletar(t));
    }

    private void MostrarSeleccion()
    {
        if (_sug.SelectedItem is not Parada sel) return;
        SeleccionarParada(sel.Codigo);
        Log($"Parada localizada: {sel}");
    }

    private void CrearPanelDetalleParada()
    {
        _detalleParadaPanel = new RoundedPanel
        {
            Size = new Size(270, 166),
            Radius = 16,
            FillColor = Color.White,
            BorderColor = Theme.Borde,
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Visible = false
        };
        _detalleParadaTexto = new Label
        {
            Location = new Point(14, 12),
            Size = new Size(242, 82),
            ForeColor = Theme.Navy,
            BackColor = Color.White,
            Font = new Font("Segoe UI", 9F)
        };
        var modificar = new RoundedButton
        {
            Location = new Point(14, 112),
            Size = new Size(112, 36),
            Radius = 12,
            Normal = Theme.Navy,
            ForeColor = Color.White,
            Text = "Modificar"
        };
        modificar.Click += (_, _) => ModificarParadaSeleccionada();
        var eliminar = new RoundedButton
        {
            Location = new Point(136, 112),
            Size = new Size(112, 36),
            Radius = 12,
            Normal = Theme.Rojo,
            ForeColor = Color.White,
            Text = "Eliminar"
        };
        eliminar.Click += (_, _) => EliminarParadaSeleccionada();
        _detalleParadaPanel.Controls.Add(_detalleParadaTexto);
        _detalleParadaPanel.Controls.Add(modificar);
        _detalleParadaPanel.Controls.Add(eliminar);
        panelDerecho.Controls.Add(_detalleParadaPanel);
        PosicionarPanelDetalle();
    }

    private void PosicionarPanelDetalle()
    {
        if (_detalleParadaPanel != null)
        {
            _detalleParadaPanel.Location = new Point(
                Math.Max(16, panelDerecho.ClientSize.Width - _detalleParadaPanel.Width - 16), 12);
            _detalleParadaPanel.BringToFront();
        }
    }

    private void SeleccionarParada(string codigo)
    {
        _paradaSeleccionada = codigo == null ? null : _svc.BuscarParada(codigo);
        _map.Seleccion = _paradaSeleccionada?.Codigo;
        _map.Invalidate();
        ActualizarDetalleParada();
    }

    private void ActualizarDetalleParada()
    {
        _detalleParadaPanel.Visible = _paradaSeleccionada != null;
        if (_paradaSeleccionada != null)
        {
            _detalleParadaTexto.Text = $"Código: {_paradaSeleccionada.Codigo}\nNombre: {_paradaSeleccionada.Nombre}\nY: {_paradaSeleccionada.Latitud.ToString("F2", CultureInfo.InvariantCulture)}\nX: {_paradaSeleccionada.Longitud.ToString("F2", CultureInfo.InvariantCulture)}";
            _detalleParadaPanel.BringToFront();
        }
    }

    private void ModificarParadaSeleccionada()
    {
        if (_paradaSeleccionada == null) return;

        using var dialogo = new Form
        {
            Text = $"Modificar parada {_paradaSeleccionada.Codigo}",
            FormBorderStyle = FormBorderStyle.FixedDialog,
            StartPosition = FormStartPosition.CenterParent,
            ClientSize = new Size(350, 220),
            MinimizeBox = false,
            MaximizeBox = false
        };
        var etiquetaCodigo = new Label { Text = $"Código: {_paradaSeleccionada.Codigo}", Location = new Point(16, 14), AutoSize = true };
        var entradaNombre = new TextBox { Text = _paradaSeleccionada.Nombre, Location = new Point(16, 48), Size = new Size(318, 27) };
        var entradaY = new TextBox { Text = _paradaSeleccionada.Latitud.ToString("R", CultureInfo.InvariantCulture), Location = new Point(16, 94), Size = new Size(150, 27), PlaceholderText = "Coordenada Y" };
        var entradaX = new TextBox { Text = _paradaSeleccionada.Longitud.ToString("R", CultureInfo.InvariantCulture), Location = new Point(184, 94), Size = new Size(150, 27), PlaceholderText = "Coordenada X" };
        var guardar = new Button { Text = "Guardar", DialogResult = DialogResult.OK, Location = new Point(164, 158), Size = new Size(80, 32) };
        var cancelar = new Button { Text = "Cancelar", DialogResult = DialogResult.Cancel, Location = new Point(254, 158), Size = new Size(80, 32) };
        dialogo.Controls.AddRange(new Control[] { etiquetaCodigo, entradaNombre, entradaY, entradaX, guardar, cancelar });
        dialogo.AcceptButton = guardar;
        dialogo.CancelButton = cancelar;

        if (dialogo.ShowDialog(this) != DialogResult.OK) return;
        if (string.IsNullOrWhiteSpace(entradaNombre.Text) || !Dbl(entradaY.Text, out double y) ||
            !Dbl(entradaX.Text, out double x) || !AreaGeograficaXela.Contiene(y, x))
        {
            MessageBox.Show(this, "Ingresa un nombre y coordenadas X/Y válidas dentro de los límites permitidos.", "Datos inválidos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            if (!_svc.ModificarParada(new Parada { Codigo = _paradaSeleccionada.Codigo, Nombre = entradaNombre.Text.Trim(), Latitud = y, Longitud = x }))
            {
                MessageBox.Show(this, "No se pudo modificar la parada. Verifica que el nombre no esté repetido y sea compatible con el CSV.", "No se pudo modificar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Log($"Parada {_paradaSeleccionada.Codigo} modificada.");
            Refrescar();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidDataException or InvalidOperationException or ArgumentException or System.Security.SecurityException)
        {
            MessageBox.Show(this, "No se guardaron los cambios de la parada:\n" + ex.Message, "Error al guardar", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void EliminarParadaSeleccionada()
    {
        if (_paradaSeleccionada == null) return;
        Parada seleccionada = _paradaSeleccionada;
        if (MessageBox.Show(this, $"¿Eliminar la parada {seleccionada.Codigo} · {seleccionada.Nombre}?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
        {
            return;
        }

        int cantidadRutas = _svc.ContarRutasConectadas(seleccionada.Codigo);
        if (cantidadRutas > 0 && MessageBox.Show(this,
            $"La parada tiene {cantidadRutas} ruta(s) conectada(s), que también se eliminarán. ¿Continuar?",
            "Confirmar eliminación de rutas", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
        {
            return;
        }

        try
        {
            if (!_svc.EliminarParada(seleccionada.Codigo))
            {
                MessageBox.Show(this, "No se pudo eliminar la parada.", "No se pudo eliminar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Log($"Parada {seleccionada.Codigo} eliminada junto con {cantidadRutas} ruta(s).");
            SeleccionarParada(null);
            Refrescar();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidDataException or InvalidOperationException or ArgumentException or System.Security.SecurityException)
        {
            MessageBox.Show(this, "No se eliminó la parada:\n" + ex.Message, "Error al guardar", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void SeleccionarRuta(Ruta[] candidatas)
    {
        if (candidatas == null || candidatas.Length == 0)
        {
            return;
        }

        Ruta seleccionada = candidatas[0];
        if (candidatas.Length > 1)
        {
            using var selector = new Form
            {
                Text = "Seleccionar conexión",
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                ClientSize = new Size(480, 150),
                MinimizeBox = false,
                MaximizeBox = false
            };
            var etiqueta = new Label { Text = "Hay varias rutas muy cercanas. Selecciona la conexión:", Location = new Point(14, 14), AutoSize = true };
            var opciones = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(14, 42),
                Size = new Size(452, 28)
            };
            opciones.Items.AddRange(candidatas.Select(DescribirRuta).Cast<object>().ToArray());
            opciones.SelectedIndex = 0;
            var aceptar = new Button { Text = "Seleccionar", DialogResult = DialogResult.OK, Location = new Point(286, 96), Size = new Size(88, 32) };
            var cancelar = new Button { Text = "Cancelar", DialogResult = DialogResult.Cancel, Location = new Point(378, 96), Size = new Size(88, 32) };
            selector.Controls.AddRange(new Control[] { etiqueta, opciones, aceptar, cancelar });
            selector.AcceptButton = aceptar;
            selector.CancelButton = cancelar;
            if (selector.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }
            seleccionada = candidatas[opciones.SelectedIndex];
        }

        EditarRutaSeleccionada(seleccionada);
    }

    private string DescribirRuta(Ruta ruta)
    {
        string sentido = ruta.UnSentido ? "un sentido" : "bidireccional";
        return $"{ruta.Origen} ({Nombre(ruta.Origen)}) → {ruta.Destino} ({Nombre(ruta.Destino)}) · Línea {ruta.Linea} · {ruta.Tiempo} min · Q{ruta.Costo.ToString("F2", CultureInfo.InvariantCulture)} · {sentido}";
    }

    private void EditarRutaSeleccionada(Ruta rutaActual)
    {
        Parada[] paradas = _svc.ObtenerParadas();
        using var dialogo = new Form
        {
            Text = $"Editar ruta {rutaActual.Origen} → {rutaActual.Destino}",
            FormBorderStyle = FormBorderStyle.FixedDialog,
            StartPosition = FormStartPosition.CenterParent,
            ClientSize = new Size(460, 315),
            MinimizeBox = false,
            MaximizeBox = false
        };

        var etiquetaOrigen = new Label { Text = "Origen", Location = new Point(16, 16), AutoSize = true };
        var entradaOrigen = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(16, 40), Size = new Size(204, 28) };
        entradaOrigen.Items.AddRange(paradas);
        var etiquetaDestino = new Label { Text = "Destino", Location = new Point(236, 16), AutoSize = true };
        var entradaDestino = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(236, 40), Size = new Size(204, 28) };
        entradaDestino.Items.AddRange(paradas);
        entradaOrigen.SelectedIndex = Array.FindIndex(paradas, parada => string.Equals(parada.Codigo, rutaActual.Origen, StringComparison.OrdinalIgnoreCase));
        entradaDestino.SelectedIndex = Array.FindIndex(paradas, parada => string.Equals(parada.Codigo, rutaActual.Destino, StringComparison.OrdinalIgnoreCase));

        var etiquetaTiempo = new Label { Text = "Tiempo (minutos)", Location = new Point(16, 86), AutoSize = true };
        var entradaTiempo = new TextBox { Text = rutaActual.Tiempo.ToString(CultureInfo.InvariantCulture), Location = new Point(16, 110), Size = new Size(120, 27) };
        var etiquetaCosto = new Label { Text = "Costo (Q)", Location = new Point(158, 86), AutoSize = true };
        var entradaCosto = new TextBox { Text = rutaActual.Costo.ToString("R", CultureInfo.InvariantCulture), Location = new Point(158, 110), Size = new Size(120, 27) };
        var etiquetaLinea = new Label { Text = "Número de línea", Location = new Point(300, 86), AutoSize = true };
        var entradaLinea = new TextBox { Text = rutaActual.Linea.ToString(CultureInfo.InvariantCulture), Location = new Point(300, 110), Size = new Size(140, 27) };
        var unSentido = new CheckBox { Text = "Ruta de un solo sentido", Checked = rutaActual.UnSentido, Location = new Point(16, 158), AutoSize = true };

        var guardar = new Button { Text = "Guardar", Location = new Point(184, 258), Size = new Size(80, 32) };
        var eliminar = new Button { Text = "Eliminar ruta", Location = new Point(270, 258), Size = new Size(88, 32) };
        var cancelar = new Button { Text = "Cancelar", DialogResult = DialogResult.Cancel, Location = new Point(364, 258), Size = new Size(76, 32) };
        guardar.Click += (_, _) =>
        {
            if (entradaOrigen.SelectedItem is not Parada origen || entradaDestino.SelectedItem is not Parada destino ||
                string.Equals(origen.Codigo, destino.Codigo, StringComparison.OrdinalIgnoreCase) ||
                !int.TryParse(entradaTiempo.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int tiempo) || tiempo < 0 ||
                !Dbl(entradaCosto.Text, out double costo) || costo < 0 ||
                !int.TryParse(entradaLinea.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int linea) || linea < 1)
            {
                MessageBox.Show(dialogo, "Selecciona paradas distintas e ingresa tiempo, costo y línea válidos.", "Datos inválidos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var rutaModificada = new Ruta
            {
                Origen = origen.Codigo,
                Destino = destino.Codigo,
                Tiempo = tiempo,
                Costo = costo,
                UnSentido = unSentido.Checked,
                Linea = linea
            };
            try
            {
                if (!_svc.ModificarRuta(rutaActual, rutaModificada))
                {
                    MessageBox.Show(dialogo, "No se pudo modificar la ruta. Verifica que las paradas existan y que no se duplique una conexión.", "No se pudo modificar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _map.Camino = null;
                _map.Recorrido = null;
                Log($"Ruta modificada: {DescribirRuta(rutaModificada)}");
                Refrescar();
                dialogo.DialogResult = DialogResult.OK;
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidDataException or InvalidOperationException or ArgumentException or System.Security.SecurityException)
            {
                MessageBox.Show(dialogo, "No se guardaron los cambios de la ruta:\n" + ex.Message, "Error al guardar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        };
        eliminar.Click += (_, _) =>
        {
            if (MessageBox.Show(dialogo, $"¿Eliminar únicamente la conexión {rutaActual.Origen} → {rutaActual.Destino}?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                if (!_svc.EliminarRuta(rutaActual))
                {
                    MessageBox.Show(dialogo, "No se pudo eliminar la ruta seleccionada.", "No se pudo eliminar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _map.Camino = null;
                _map.Recorrido = null;
                Log($"Ruta eliminada: {rutaActual.Origen} → {rutaActual.Destino}. Las paradas se conservaron.");
                Refrescar();
                dialogo.DialogResult = DialogResult.Abort;
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidDataException or InvalidOperationException or ArgumentException or System.Security.SecurityException)
            {
                MessageBox.Show(dialogo, "No se eliminó la ruta:\n" + ex.Message, "Error al eliminar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        };

        dialogo.Controls.AddRange(new Control[]
        {
            etiquetaOrigen, entradaOrigen, etiquetaDestino, entradaDestino,
            etiquetaTiempo, entradaTiempo, etiquetaCosto, entradaCosto, etiquetaLinea, entradaLinea,
            unSentido, guardar, eliminar, cancelar
        });
        dialogo.AcceptButton = guardar;
        dialogo.CancelButton = cancelar;
        dialogo.ShowDialog(this);
    }

    // — Dijkstra
    private void Calcular()
    {
        if (!Seleccion(out var o, out var d)) return;
        _map.Recorrido = null;

        if (!_svc.MismaZona(o, d))
        {
            _map.Camino = null; _map.Invalidate();
            _resultado.Text = "Las paradas no están en la misma zona conectada.";
            Log($"{Nombre(o)} y {Nombre(d)} NO están conectadas.");
            return;
        }

        var crit = _rEco.Checked ? Criterio.Economica : Criterio.Rapida;
        var r = _svc.CalcularRuta(o, d, crit);
        if (!r.Exito)
        {
            _map.Camino = null; _map.Invalidate();
            _resultado.Text = r.Mensaje;
            Log(r.Mensaje);
            return;
        }

        _map.Camino = r.Camino;
        _map.Invalidate();
        string lineas = string.Join(", ", r.DetalleConexiones.Select(detalle => detalle.Linea).Distinct());
        _resultado.Text = "Ruta: " + string.Join(" → ", r.Camino.Select(Nombre)) +
                          $"\nTiempo: {r.TiempoTotal} min   ·   Costo: Q{r.CostoTotal.ToString("F2", CultureInfo.InvariantCulture)}" +
                          $"\nLíneas: {(lineas.Length == 0 ? "ninguna" : lineas)}";
        Log($"Ruta {(crit == Criterio.Rapida ? "rápida" : "económica")}: {string.Join(" → ", r.Camino)}");
        foreach (DetalleConexionRuta detalle in r.DetalleConexiones)
        {
            Log($"{detalle.Origen} → {detalle.Destino} · Línea {detalle.Linea} · Q{detalle.Costo.ToString("F2", CultureInfo.InvariantCulture)} · {detalle.Tiempo} min");
        }
    }

    // — Union-Find —
    private void VerificarZona()
    {
        if (!Seleccion(out var o, out var d)) return;
        bool ok = _svc.MismaZona(o, d);
        string msg = ok ? $"{Nombre(o)} y {Nombre(d)} están en la MISMA zona conectada."
                        : $"{Nombre(o)} y {Nombre(d)} están en zonas DIFERENTES.";
        _resultado.Text = msg;
        Log(msg);
    }

    // — BFS / DFS —
    private void Recorrer(bool bfs)
    {
        if (!Seleccion(out var o, out _)) return;
        var orden = bfs ? _svc.RecorridoBFS(o) : _svc.RecorridoDFS(o);
        _map.Camino = null;
        _map.Recorrido = orden;
        _map.Invalidate();
        string txt = string.Join(" → ", orden.Select(Nombre));
        _resultado.Text = (bfs ? "BFS: " : "DFS: ") + txt;
        Log((bfs ? "BFS" : "DFS") + " desde " + Nombre(o) + ": " + string.Join(", ", orden));
    }

    // — Registrar —
    private void AgregarParada()
    {
        if (!Dbl(txtLatitud.Text, out var lat) || !Dbl(txtLongitud.Text, out var lon))
        {
            MessageBox.Show("Las coordenadas ficticias X e Y deben ser números (ej. 15 o 30).");
            return;
        }

        if (!AreaGeograficaXela.Contiene(lat, lon))
        {
            MessageBox.Show("No se puede registrar: las coordenadas ficticias X e Y deben estar entre 0 y 30.");
            return;
        }

        var p = new Parada
        {
            Codigo = txtCodigoParada.Text.Trim(),
            Nombre = txtNombreParada.Text.Trim(),
            Latitud = lat,
            Longitud = lon
        };

        if (_svc.AgregarParada(p))
        {
            Log($"Parada agregada: {p}");

            txtCodigoParada.Clear();
            txtNombreParada.Clear();
            txtLatitud.Clear();
            txtLongitud.Clear();

            Refrescar();
        }
        else
        {
            MessageBox.Show("No se pudo agregar (¿código vacío o repetido?).");
        }
    }

    private Parada[] BuscarCandidatosParadaRuta(string identificador)
    {
        string texto = identificador.Trim();
        if (texto.Length == 0)
        {
            return Array.Empty<Parada>();
        }

        Parada porCodigo = _svc.BuscarParada(texto);
        if (porCodigo != null)
        {
            return new[] { porCodigo };
        }

        bool existeEnTrie = _svc.Autocompletar(texto).Any(parada =>
            string.Equals(parada.Nombre, texto, StringComparison.OrdinalIgnoreCase));
        if (!existeEnTrie)
        {
            return Array.Empty<Parada>();
        }

        return _svc.ObtenerParadas()
            .Where(parada => string.Equals(parada.Nombre, texto, StringComparison.OrdinalIgnoreCase))
            .ToArray();
    }

    private Parada ElegirParadaRuta(string identificador, string campo)
    {
        Parada[] candidatos = BuscarCandidatosParadaRuta(identificador);
        if (candidatos.Length == 0)
        {
            MessageBox.Show(this, $"No se encontró una parada para {campo}: ‘{identificador.Trim()}’. Ingresa un código o nombre exacto.",
                "Parada no encontrada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return null;
        }

        if (candidatos.Length == 1)
        {
            return candidatos[0];
        }

        using var dialogo = new Form
        {
            Text = $"Seleccionar parada de {campo}",
            FormBorderStyle = FormBorderStyle.FixedDialog,
            StartPosition = FormStartPosition.CenterParent,
            ClientSize = new Size(340, 125),
            MinimizeBox = false,
            MaximizeBox = false
        };
        var etiqueta = new Label { Text = "Hay varias paradas con ese nombre. Elige el código:", Location = new Point(14, 14), AutoSize = true };
        var opciones = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Location = new Point(14, 42),
            Size = new Size(312, 28)
        };
        opciones.Items.AddRange(candidatos);
        opciones.SelectedIndex = 0;
        var aceptar = new Button { Text = "Seleccionar", DialogResult = DialogResult.OK, Location = new Point(150, 82), Size = new Size(86, 30) };
        var cancelar = new Button { Text = "Cancelar", DialogResult = DialogResult.Cancel, Location = new Point(240, 82), Size = new Size(86, 30) };
        dialogo.Controls.AddRange(new Control[] { etiqueta, opciones, aceptar, cancelar });
        dialogo.AcceptButton = aceptar;
        dialogo.CancelButton = cancelar;
        return dialogo.ShowDialog(this) == DialogResult.OK ? opciones.SelectedItem as Parada : null;
    }

    private void AgregarRuta()
    {
        if (!int.TryParse(txtMinutos.Text, out var t) || !Dbl(txtCosto.Text, out var costo))
        { MessageBox.Show("Minutos debe ser entero y costo un número."); return; }
        int.TryParse(txtLinea.Text, out var linea);
        if (linea < 1) linea = 1;

        Parada origen = ElegirParadaRuta(txtOrigenRuta.Text, "origen");
        if (origen == null) return;
        Parada destino = ElegirParadaRuta(txtDestinoRuta.Text, "destino");
        if (destino == null) return;
        if (string.Equals(origen.Codigo, destino.Codigo, StringComparison.OrdinalIgnoreCase))
        {
            MessageBox.Show(this, "El origen y el destino deben ser paradas distintas.", "Ruta inválida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (_svc.MismaZona(origen.Codigo, destino.Codigo))
            Log($"Aviso: {origen.Codigo} y {destino.Codigo} ya están en la misma zona; la nueva ruta crea una alternativa.");
        else
            Log($"Aviso: {origen.Codigo} y {destino.Codigo} estaban en zonas distintas; la nueva ruta las une.");

        var r = new Ruta { Origen = origen.Codigo, Destino = destino.Codigo, Tiempo = t, Costo = costo, UnSentido = _rUno.Checked, Linea = linea };
        if (_svc.AgregarRuta(r))
        {
            Log($"Ruta agregada: {origen.Codigo} ({origen.Nombre}) → {destino.Codigo} ({destino.Nombre}) ({t} min, Q{costo.ToString("F2", CultureInfo.InvariantCulture)})");
            Refrescar();
        }
        else MessageBox.Show("No se pudo agregar la ruta (verifica que no exista ya una conexión entre esas paradas).");
    }

    // — CSV —
    private void CargarCsv()
    {
        using var dlg = new OpenFileDialog { Filter = "Archivos CSV/TXT|*.csv;*.txt|Todos|*.*", Title = "Selecciona el archivo de datos" };
        if (dlg.ShowDialog() != DialogResult.OK) return;
        try
        {
            Log(_svc.CargarCSV(dlg.FileName));
            Refrescar();
        }
        catch (Exception ex) { MessageBox.Show("Error al leer el archivo:\n" + ex.Message); }
    }

    private void lblSubtitulo_Click(object sender, EventArgs e)
    {

    }

    private void _map_Paint(object sender, PaintEventArgs e)
    {

    }

    private void MainForm_Load_1(object sender, EventArgs e)
    {

    }
}
