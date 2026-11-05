namespace RutasXela.UI
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            panelEncabezado = new Panel();
            lblTitulo = new Label();
            lblSubtitulo = new Label();
            btnCsv = new RoundedButton();
            panelContenido = new Panel();
            panelDerecho = new Panel();
            panelMapa = new Panel();
            _map = new MapPanel();
            espaciador = new Panel();
            logCard = new RoundedPanel();
            lblLogTitulo = new Label();
            txtRegistroActividad = new TextBox();
            panelIzquierdo = new FlowLayoutPanel();
            cardBuscar = new RoundedPanel();
            txtBuscar = new TextBox();
            lblTituloBuscar = new Label();
            _sug = new ListBox();
            _info = new Label();
            cardRutas = new RoundedPanel();
            cmbDestino = new ComboBox();
            cmbOrigen = new ComboBox();
            lblTituloRutas = new Label();
            lblOrigen = new Label();
            lblDestino = new Label();
            _rRapida = new RadioButton();
            _rEco = new RadioButton();
            btnCalcular = new RoundedButton();
            btnZona = new RoundedButton();
            btnBfs = new RoundedButton();
            btnDfs = new RoundedButton();
            _resultado = new Label();
            cardParada = new RoundedPanel();
            txtCodigoParada = new TextBox();
            txtNombreParada = new TextBox();
            txtLatitud = new TextBox();
            txtLongitud = new TextBox();
            lblTituloParada = new Label();
            btnAgregarParada = new RoundedButton();
            cardRuta = new RoundedPanel();
            txtOrigenRuta = new TextBox();
            txtDestinoRuta = new TextBox();
            txtMinutos = new TextBox();
            txtCosto = new TextBox();
            txtLinea = new TextBox();
            lblTituloRuta = new Label();
            _rUno = new CheckBox();
            btnAgregarRuta = new RoundedButton();
            panelEncabezado.SuspendLayout();
            panelContenido.SuspendLayout();
            panelDerecho.SuspendLayout();
            panelMapa.SuspendLayout();
            logCard.SuspendLayout();
            panelIzquierdo.SuspendLayout();
            cardBuscar.SuspendLayout();
            cardRutas.SuspendLayout();
            cardParada.SuspendLayout();
            cardRuta.SuspendLayout();
            SuspendLayout();
            // 
            // panelEncabezado
            // 
            panelEncabezado.BackColor = Color.FromArgb(31, 42, 68);
            panelEncabezado.Controls.Add(lblTitulo);
            panelEncabezado.Controls.Add(lblSubtitulo);
            panelEncabezado.Controls.Add(btnCsv);
            panelEncabezado.Dock = DockStyle.Top;
            panelEncabezado.Location = new Point(0, 0);
            panelEncabezado.Name = "panelEncabezado";
            panelEncabezado.Size = new Size(1302, 74);
            panelEncabezado.TabIndex = 1;
            panelEncabezado.Paint += Header_Paint;
            panelEncabezado.Resize += Header_Resize;
            // 
            // lblTitulo
            // 
            lblTitulo.AutoSize = true;
            lblTitulo.BackColor = Color.Transparent;
            lblTitulo.Font = new Font("Segoe UI Semibold", 20F);
            lblTitulo.ForeColor = Color.White;
            lblTitulo.Location = new Point(28, 0);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(181, 46);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "Rutas Xela";
            // 
            // lblSubtitulo
            // 
            lblSubtitulo.AutoSize = true;
            lblSubtitulo.BackColor = Color.Transparent;
            lblSubtitulo.Font = new Font("Segoe UI", 9.5F);
            lblSubtitulo.ForeColor = Color.FromArgb(190, 198, 215);
            lblSubtitulo.Location = new Point(34, 44);
            lblSubtitulo.Name = "lblSubtitulo";
            lblSubtitulo.Size = new Size(309, 21);
            lblSubtitulo.TabIndex = 1;
            lblSubtitulo.Text = "Red de transporte urbano · Quetzaltenango";
            lblSubtitulo.Click += lblSubtitulo_Click;
            // 
            // btnCsv
            // 
            btnCsv.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCsv.FlatStyle = FlatStyle.Flat;
            btnCsv.Font = new Font("Segoe UI Semibold", 9.5F);
            btnCsv.ForeColor = Color.FromArgb(31, 42, 68);
            btnCsv.Normal = Theme.Amarillo;
            btnCsv.Location = new Point(1130, 17);
            btnCsv.Name = "btnCsv";
            btnCsv.Size = new Size(150, 38);
            btnCsv.TabIndex = 2;
            btnCsv.Text = "Cargar CSV";
            btnCsv.Click += BtnCsv_Click;
            // 
            // panelContenido
            // 
            panelContenido.Controls.Add(panelDerecho);
            panelContenido.Controls.Add(panelIzquierdo);
            panelContenido.Dock = DockStyle.Fill;
            panelContenido.Location = new Point(0, 74);
            panelContenido.Name = "panelContenido";
            panelContenido.Padding = new Padding(16);
            panelContenido.Size = new Size(1302, 719);
            panelContenido.TabIndex = 0;
            // 
            // panelDerecho
            // 
            panelDerecho.Controls.Add(panelMapa);
            panelDerecho.Controls.Add(espaciador);
            panelDerecho.Controls.Add(logCard);
            panelDerecho.Dock = DockStyle.Fill;
            panelDerecho.Location = new Point(412, 16);
            panelDerecho.Name = "panelDerecho";
            panelDerecho.Padding = new Padding(16, 0, 0, 0);
            panelDerecho.Size = new Size(874, 687);
            panelDerecho.TabIndex = 0;
            //
            // panelMapa
            //
            panelMapa.AutoScroll = true;
            panelMapa.AutoScrollMinSize = new Size(2400, 1800);
            panelMapa.BackColor = Color.FromArgb(244, 245, 250);
            panelMapa.Location = new Point(16, 0);
            panelMapa.Name = "panelMapa";
            panelMapa.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panelMapa.Size = new Size(858, 503);
            panelMapa.TabIndex = 0;
            panelMapa.Controls.Add(_map);
            //
            // _map
            //
            _map.BackColor = Color.FromArgb(244, 245, 250);
            _map.Location = new Point(0, 0);
            _map.Name = "_map";
            _map.Size = new Size(2400, 1800);
            _map.TabIndex = 0;
            _map.Paint += _map_Paint;
            // 
            // espaciador
            // 
            espaciador.Dock = DockStyle.Bottom;
            espaciador.Location = new Point(16, 503);
            espaciador.Name = "espaciador";
            espaciador.Size = new Size(858, 14);
            espaciador.TabIndex = 1;
            // 
            // logCard
            // 
            logCard.BackColor = Color.FromArgb(244, 245, 250);
            logCard.Controls.Add(lblLogTitulo);
            logCard.Controls.Add(txtRegistroActividad);
            logCard.Dock = DockStyle.Bottom;
            logCard.Location = new Point(16, 517);
            logCard.Name = "logCard";
            logCard.Padding = new Padding(16, 40, 16, 14);
            logCard.Size = new Size(858, 170);
            logCard.TabIndex = 2;
            // 
            // lblLogTitulo
            // 
            lblLogTitulo.AutoSize = true;
            lblLogTitulo.BackColor = Color.White;
            lblLogTitulo.Font = new Font("Segoe UI Semibold", 11F);
            lblLogTitulo.ForeColor = Color.FromArgb(31, 42, 68);
            lblLogTitulo.Location = new Point(18, 12);
            lblLogTitulo.Name = "lblLogTitulo";
            lblLogTitulo.Size = new Size(191, 25);
            lblLogTitulo.TabIndex = 0;
            lblLogTitulo.Text = "Registro de actividad";
            // 
            // txtRegistroActividad
            // 
            txtRegistroActividad.BackColor = Color.White;
            txtRegistroActividad.BorderStyle = BorderStyle.None;
            txtRegistroActividad.Dock = DockStyle.Fill;
            txtRegistroActividad.Font = new Font("Consolas", 9.5F);
            txtRegistroActividad.ForeColor = Color.FromArgb(43, 50, 69);
            txtRegistroActividad.Location = new Point(16, 40);
            txtRegistroActividad.Multiline = true;
            txtRegistroActividad.Name = "txtRegistroActividad";
            txtRegistroActividad.ReadOnly = true;
            txtRegistroActividad.ScrollBars = ScrollBars.Vertical;
            txtRegistroActividad.Size = new Size(826, 116);
            txtRegistroActividad.TabIndex = 1;
            // 
            // panelIzquierdo
            // 
            panelIzquierdo.AutoScroll = true;
            panelIzquierdo.Controls.Add(cardBuscar);
            panelIzquierdo.Controls.Add(cardRutas);
            panelIzquierdo.Controls.Add(cardParada);
            panelIzquierdo.Controls.Add(cardRuta);
            panelIzquierdo.Dock = DockStyle.Left;
            panelIzquierdo.FlowDirection = FlowDirection.TopDown;
            panelIzquierdo.Location = new Point(16, 16);
            panelIzquierdo.Name = "panelIzquierdo";
            panelIzquierdo.Size = new Size(396, 687);
            panelIzquierdo.TabIndex = 1;
            panelIzquierdo.WrapContents = false;
            // 
            // cardBuscar
            // 
            cardBuscar.BackColor = Color.FromArgb(244, 245, 250);
            cardBuscar.Controls.Add(txtBuscar);
            cardBuscar.Controls.Add(lblTituloBuscar);
            cardBuscar.Controls.Add(_sug);
            cardBuscar.Controls.Add(_info);
            cardBuscar.Location = new Point(0, 0);
            cardBuscar.Margin = new Padding(0, 0, 0, 14);
            cardBuscar.Name = "cardBuscar";
            cardBuscar.Size = new Size(360, 275);
            cardBuscar.TabIndex = 0;
            // 
            // txtBuscar
            // 
            txtBuscar.Location = new Point(18, 54);
            txtBuscar.Name = "txtBuscar";
            txtBuscar.PlaceholderText = "Escribe un código o nombre…";
            txtBuscar.Size = new Size(324, 29);
            txtBuscar.TabIndex = 3;
            // 
            // lblTituloBuscar
            // 
            lblTituloBuscar.AutoSize = true;
            lblTituloBuscar.BackColor = Color.White;
            lblTituloBuscar.Font = new Font("Segoe UI Semibold", 11.5F);
            lblTituloBuscar.ForeColor = Color.FromArgb(31, 42, 68);
            lblTituloBuscar.Location = new Point(18, 14);
            lblTituloBuscar.Name = "lblTituloBuscar";
            lblTituloBuscar.Size = new Size(138, 28);
            lblTituloBuscar.TabIndex = 0;
            lblTituloBuscar.Text = "Buscar parada";
            // 
            // _sug
            // 
            _sug.BackColor = Color.White;
            _sug.BorderStyle = BorderStyle.None;
            _sug.Font = new Font("Segoe UI", 10F);
            _sug.ForeColor = Color.FromArgb(43, 50, 69);
            _sug.IntegralHeight = false;
            _sug.Location = new Point(22, 98);
            _sug.Name = "_sug";
            _sug.Size = new Size(316, 96);
            _sug.TabIndex = 1;
            _sug.SelectedIndexChanged += Sug_SelectedIndexChanged;
            // 
            // _info
            // 
            _info.BackColor = Color.White;
            _info.ForeColor = Color.FromArgb(43, 50, 69);
            _info.Location = new Point(18, 206);
            _info.Name = "_info";
            _info.Size = new Size(324, 56);
            _info.TabIndex = 2;
            _info.Text = "Las sugerencias aparecerán mientras escribes.";
            // 
            // cardRutas
            // 
            cardRutas.BackColor = Color.FromArgb(244, 245, 250);
            cardRutas.Controls.Add(cmbDestino);
            cardRutas.Controls.Add(cmbOrigen);
            cardRutas.Controls.Add(lblTituloRutas);
            cardRutas.Controls.Add(lblOrigen);
            cardRutas.Controls.Add(lblDestino);
            cardRutas.Controls.Add(_rRapida);
            cardRutas.Controls.Add(_rEco);
            cardRutas.Controls.Add(btnCalcular);
            cardRutas.Controls.Add(btnZona);
            cardRutas.Controls.Add(btnBfs);
            cardRutas.Controls.Add(btnDfs);
            cardRutas.Controls.Add(_resultado);
            cardRutas.Location = new Point(0, 289);
            cardRutas.Margin = new Padding(0, 0, 0, 14);
            cardRutas.Name = "cardRutas";
            cardRutas.Size = new Size(360, 395);
            cardRutas.TabIndex = 1;
            // 
            // cmbDestino
            // 
            cmbDestino.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDestino.FormattingEnabled = true;
            cmbDestino.Location = new Point(18, 139);
            cmbDestino.Name = "cmbDestino";
            cmbDestino.Size = new Size(324, 29);
            cmbDestino.TabIndex = 11;
            // 
            // cmbOrigen
            // 
            cmbOrigen.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbOrigen.FormattingEnabled = true;
            cmbOrigen.Location = new Point(18, 71);
            cmbOrigen.Name = "cmbOrigen";
            cmbOrigen.Size = new Size(324, 29);
            cmbOrigen.TabIndex = 10;
            // 
            // lblTituloRutas
            // 
            lblTituloRutas.AutoSize = true;
            lblTituloRutas.BackColor = Color.White;
            lblTituloRutas.Font = new Font("Segoe UI Semibold", 11.5F);
            lblTituloRutas.ForeColor = Color.FromArgb(31, 42, 68);
            lblTituloRutas.Location = new Point(18, 14);
            lblTituloRutas.Name = "lblTituloRutas";
            lblTituloRutas.Size = new Size(175, 28);
            lblTituloRutas.TabIndex = 0;
            lblTituloRutas.Text = "Rutas y recorridos";
            // 
            // lblOrigen
            // 
            lblOrigen.AutoSize = true;
            lblOrigen.BackColor = Color.White;
            lblOrigen.Font = new Font("Segoe UI", 8.5F);
            lblOrigen.ForeColor = Color.FromArgb(123, 132, 153);
            lblOrigen.Location = new Point(20, 46);
            lblOrigen.Name = "lblOrigen";
            lblOrigen.Size = new Size(62, 20);
            lblOrigen.TabIndex = 1;
            lblOrigen.Text = "ORIGEN";
            // 
            // lblDestino
            // 
            lblDestino.AutoSize = true;
            lblDestino.BackColor = Color.White;
            lblDestino.Font = new Font("Segoe UI", 8.5F);
            lblDestino.ForeColor = Color.FromArgb(123, 132, 153);
            lblDestino.Location = new Point(20, 110);
            lblDestino.Name = "lblDestino";
            lblDestino.Size = new Size(70, 20);
            lblDestino.TabIndex = 2;
            lblDestino.Text = "DESTINO";
            // 
            // _rRapida
            // 
            _rRapida.AutoSize = true;
            _rRapida.BackColor = Color.White;
            _rRapida.Checked = true;
            _rRapida.ForeColor = Color.FromArgb(43, 50, 69);
            _rRapida.Location = new Point(20, 182);
            _rRapida.Name = "_rRapida";
            _rRapida.Size = new Size(108, 25);
            _rRapida.TabIndex = 3;
            _rRapida.TabStop = true;
            _rRapida.Text = "Más rápida";
            _rRapida.UseVisualStyleBackColor = false;
            // 
            // _rEco
            // 
            _rEco.AutoSize = true;
            _rEco.BackColor = Color.White;
            _rEco.ForeColor = Color.FromArgb(43, 50, 69);
            _rEco.Location = new Point(150, 182);
            _rEco.Name = "_rEco";
            _rEco.Size = new Size(139, 25);
            _rEco.TabIndex = 4;
            _rEco.Text = "Más económica";
            _rEco.UseVisualStyleBackColor = false;
            // 
            // btnCalcular
            // 
            btnCalcular.FlatStyle = FlatStyle.Flat;
            btnCalcular.Font = new Font("Segoe UI Semibold", 9.5F);
            btnCalcular.ForeColor = Color.White;
            btnCalcular.Normal = Theme.Navy;
            btnCalcular.Location = new Point(18, 214);
            btnCalcular.Name = "btnCalcular";
            btnCalcular.Size = new Size(158, 38);
            btnCalcular.TabIndex = 5;
            btnCalcular.Text = "Calcular ruta";
            btnCalcular.Click += BtnCalcular_Click;
            // 
            // btnZona
            // 
            btnZona.FlatStyle = FlatStyle.Flat;
            btnZona.Font = new Font("Segoe UI Semibold", 9.5F);
            btnZona.ForeColor = Color.White;
            btnZona.Normal = Theme.Verde;
            btnZona.Location = new Point(184, 214);
            btnZona.Name = "btnZona";
            btnZona.Size = new Size(158, 38);
            btnZona.TabIndex = 6;
            btnZona.Text = "¿Misma zona?";
            btnZona.Click += BtnZona_Click;
            // 
            // btnBfs
            // 
            btnBfs.FlatStyle = FlatStyle.Flat;
            btnBfs.Font = new Font("Segoe UI Semibold", 9.5F);
            btnBfs.ForeColor = Color.White;
            btnBfs.Normal = Theme.Linea(4);
            btnBfs.Location = new Point(18, 258);
            btnBfs.Name = "btnBfs";
            btnBfs.Size = new Size(158, 38);
            btnBfs.TabIndex = 7;
            btnBfs.Text = "Recorrido BFS";
            btnBfs.Click += BtnBfs_Click;
            // 
            // btnDfs
            // 
            btnDfs.FlatStyle = FlatStyle.Flat;
            btnDfs.Font = new Font("Segoe UI Semibold", 9.5F);
            btnDfs.ForeColor = Color.White;
            btnDfs.Normal = Theme.Linea(6);
            btnDfs.Location = new Point(184, 258);
            btnDfs.Name = "btnDfs";
            btnDfs.Size = new Size(158, 38);
            btnDfs.TabIndex = 8;
            btnDfs.Text = "Recorrido DFS";
            btnDfs.Click += BtnDfs_Click;
            // 
            // _resultado
            // 
            _resultado.BackColor = Color.White;
            _resultado.ForeColor = Color.FromArgb(43, 50, 69);
            _resultado.Location = new Point(18, 308);
            _resultado.Name = "_resultado";
            _resultado.Size = new Size(324, 76);
            _resultado.TabIndex = 9;
            _resultado.Text = "Elige origen y destino para calcular.";
            // 
            // cardParada
            // 
            cardParada.BackColor = Color.FromArgb(244, 245, 250);
            cardParada.Controls.Add(txtCodigoParada);
            cardParada.Controls.Add(txtNombreParada);
            cardParada.Controls.Add(txtLatitud);
            cardParada.Controls.Add(txtLongitud);
            cardParada.Controls.Add(lblTituloParada);
            cardParada.Controls.Add(btnAgregarParada);
            cardParada.Location = new Point(0, 698);
            cardParada.Margin = new Padding(0, 0, 0, 14);
            cardParada.Name = "cardParada";
            cardParada.Size = new Size(360, 215);
            cardParada.TabIndex = 2;
            // 
            // txtCodigoParada
            // 
            txtCodigoParada.Location = new Point(18, 56);
            txtCodigoParada.Name = "txtCodigoParada";
            txtCodigoParada.PlaceholderText = "Código";
            txtCodigoParada.Size = new Size(100, 29);
            txtCodigoParada.TabIndex = 18;
            // 
            // txtNombreParada
            // 
            txtNombreParada.Location = new Point(126, 56);
            txtNombreParada.Name = "txtNombreParada";
            txtNombreParada.PlaceholderText = "Nombre";
            txtNombreParada.Size = new Size(216, 29);
            txtNombreParada.TabIndex = 19;
            // 
            // txtLatitud
            // 
            txtLatitud.Location = new Point(18, 104);
            txtLatitud.Name = "txtLatitud";
            txtLatitud.PlaceholderText = "Coordenada Y";
            txtLatitud.Size = new Size(162, 29);
            txtLatitud.TabIndex = 20;
            // 
            // txtLongitud
            // 
            txtLongitud.Location = new Point(186, 104);
            txtLongitud.Name = "txtLongitud";
            txtLongitud.PlaceholderText = "Coordenada X";
            txtLongitud.Size = new Size(156, 29);
            txtLongitud.TabIndex = 21;
            // 
            // lblTituloParada
            // 
            lblTituloParada.AutoSize = true;
            lblTituloParada.BackColor = Color.White;
            lblTituloParada.Font = new Font("Segoe UI Semibold", 11.5F);
            lblTituloParada.ForeColor = Color.FromArgb(31, 42, 68);
            lblTituloParada.Location = new Point(18, 14);
            lblTituloParada.Name = "lblTituloParada";
            lblTituloParada.Size = new Size(158, 28);
            lblTituloParada.TabIndex = 0;
            lblTituloParada.Text = "Registrar parada";
            // 
            // btnAgregarParada
            // 
            btnAgregarParada.FlatStyle = FlatStyle.Flat;
            btnAgregarParada.Font = new Font("Segoe UI Semibold", 9.5F);
            btnAgregarParada.ForeColor = Color.White;
            btnAgregarParada.Normal = Theme.Navy;
            btnAgregarParada.Location = new Point(18, 152);
            btnAgregarParada.Name = "btnAgregarParada";
            btnAgregarParada.Size = new Size(324, 38);
            btnAgregarParada.TabIndex = 1;
            btnAgregarParada.Text = "Agregar parada";
            btnAgregarParada.Click += BtnAgregarParada_Click;
            // 
            // cardRuta
            // 
            cardRuta.BackColor = Color.FromArgb(244, 245, 250);
            cardRuta.Controls.Add(txtOrigenRuta);
            cardRuta.Controls.Add(txtDestinoRuta);
            cardRuta.Controls.Add(txtMinutos);
            cardRuta.Controls.Add(txtCosto);
            cardRuta.Controls.Add(txtLinea);
            cardRuta.Controls.Add(lblTituloRuta);
            cardRuta.Controls.Add(_rUno);
            cardRuta.Controls.Add(btnAgregarRuta);
            cardRuta.Location = new Point(0, 927);
            cardRuta.Margin = new Padding(0, 0, 0, 14);
            cardRuta.Name = "cardRuta";
            cardRuta.Size = new Size(360, 245);
            cardRuta.TabIndex = 3;
            // 
            // txtOrigenRuta
            // 
            txtOrigenRuta.Location = new Point(18, 54);
            txtOrigenRuta.Name = "txtOrigenRuta";
            txtOrigenRuta.PlaceholderText = "Origen (P001)";
            txtOrigenRuta.Size = new Size(158, 29);
            txtOrigenRuta.TabIndex = 25;
            // 
            // txtDestinoRuta
            // 
            txtDestinoRuta.Location = new Point(184, 54);
            txtDestinoRuta.Name = "txtDestinoRuta";
            txtDestinoRuta.PlaceholderText = "Destino (P002)";
            txtDestinoRuta.Size = new Size(158, 29);
            txtDestinoRuta.TabIndex = 26;
            // 
            // txtMinutos
            // 
            txtMinutos.Location = new Point(18, 102);
            txtMinutos.Name = "txtMinutos";
            txtMinutos.PlaceholderText = "Minutos";
            txtMinutos.Size = new Size(100, 29);
            txtMinutos.TabIndex = 27;
            // 
            // txtCosto
            // 
            txtCosto.Location = new Point(126, 102);
            txtCosto.Name = "txtCosto";
            txtCosto.PlaceholderText = "Costo Q";
            txtCosto.Size = new Size(100, 29);
            txtCosto.TabIndex = 28;
            // 
            // txtLinea
            // 
            txtLinea.Location = new Point(234, 102);
            txtLinea.Name = "txtLinea";
            txtLinea.PlaceholderText = "Línea (1-7)";
            txtLinea.Size = new Size(108, 29);
            txtLinea.TabIndex = 29;
            // 
            // lblTituloRuta
            // 
            lblTituloRuta.AutoSize = true;
            lblTituloRuta.BackColor = Color.White;
            lblTituloRuta.Font = new Font("Segoe UI Semibold", 11.5F);
            lblTituloRuta.ForeColor = Color.FromArgb(31, 42, 68);
            lblTituloRuta.Location = new Point(18, 14);
            lblTituloRuta.Name = "lblTituloRuta";
            lblTituloRuta.Size = new Size(133, 28);
            lblTituloRuta.TabIndex = 0;
            lblTituloRuta.Text = "Registrar ruta";
            // 
            // _rUno
            // 
            _rUno.AutoSize = true;
            _rUno.BackColor = Color.White;
            _rUno.ForeColor = Color.FromArgb(43, 50, 69);
            _rUno.Location = new Point(20, 148);
            _rUno.Name = "_rUno";
            _rUno.Size = new Size(140, 25);
            _rUno.TabIndex = 1;
            _rUno.Text = "Solo un sentido";
            _rUno.UseVisualStyleBackColor = false;
            // 
            // btnAgregarRuta
            // 
            btnAgregarRuta.FlatStyle = FlatStyle.Flat;
            btnAgregarRuta.Font = new Font("Segoe UI Semibold", 9.5F);
            btnAgregarRuta.ForeColor = Color.FromArgb(31, 42, 68);
            btnAgregarRuta.Normal = Theme.Amarillo;
            btnAgregarRuta.Location = new Point(18, 182);
            btnAgregarRuta.Name = "btnAgregarRuta";
            btnAgregarRuta.Size = new Size(324, 38);
            btnAgregarRuta.TabIndex = 2;
            btnAgregarRuta.Text = "Agregar ruta";
            btnAgregarRuta.Click += BtnAgregarRuta_Click;
            // 
            // MainForm
            // 
            BackColor = Color.FromArgb(244, 245, 250);
            ClientSize = new Size(1302, 793);
            Controls.Add(panelContenido);
            Controls.Add(panelEncabezado);
            Font = new Font("Segoe UI", 9.5F);
            MinimumSize = new Size(1150, 720);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Rutas Xela";
            panelEncabezado.ResumeLayout(false);
            panelEncabezado.PerformLayout();
            panelContenido.ResumeLayout(false);
            panelMapa.ResumeLayout(false);
            panelDerecho.ResumeLayout(false);
            logCard.ResumeLayout(false);
            logCard.PerformLayout();
            panelIzquierdo.ResumeLayout(false);
            cardBuscar.ResumeLayout(false);
            cardBuscar.PerformLayout();
            cardRutas.ResumeLayout(false);
            cardRutas.PerformLayout();
            cardParada.ResumeLayout(false);
            cardParada.PerformLayout();
            cardRuta.ResumeLayout(false);
            cardRuta.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelEncabezado;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblSubtitulo;
        private RutasXela.UI.RoundedButton btnCsv;
        private System.Windows.Forms.Panel panelContenido;
        private System.Windows.Forms.Panel panelDerecho;
        private System.Windows.Forms.Panel panelMapa;
        private RutasXela.UI.MapPanel _map;
        private System.Windows.Forms.Panel espaciador;
        private RutasXela.UI.RoundedPanel logCard;
        private System.Windows.Forms.Label lblLogTitulo;
        private System.Windows.Forms.TextBox txtRegistroActividad;
        private System.Windows.Forms.FlowLayoutPanel panelIzquierdo;
        private RutasXela.UI.RoundedPanel cardBuscar;
        private System.Windows.Forms.Label lblTituloBuscar;
        private RutasXela.UI.RoundedInput _buscar;
        private System.Windows.Forms.ListBox _sug;
        private System.Windows.Forms.Label _info;
        private RutasXela.UI.RoundedPanel cardRutas;
        private System.Windows.Forms.Label lblTituloRutas;
        private System.Windows.Forms.Label lblOrigen;
        private System.Windows.Forms.Label lblDestino;
        private System.Windows.Forms.RadioButton _rRapida;
        private System.Windows.Forms.RadioButton _rEco;
        private RutasXela.UI.RoundedButton btnCalcular;
        private RutasXela.UI.RoundedButton btnZona;
        private RutasXela.UI.RoundedButton btnBfs;
        private RutasXela.UI.RoundedButton btnDfs;
        private System.Windows.Forms.Label _resultado;
        private RutasXela.UI.RoundedPanel cardParada;
        private System.Windows.Forms.Label lblTituloParada;
        private RutasXela.UI.RoundedInput _pCod;
        private RutasXela.UI.RoundedInput _pNom;
        private RutasXela.UI.RoundedInput _pLat;
        private RutasXela.UI.RoundedInput _pLon;
        private RutasXela.UI.RoundedButton btnAgregarParada;
        private RutasXela.UI.RoundedPanel cardRuta;
        private System.Windows.Forms.Label lblTituloRuta;
        private RutasXela.UI.RoundedInput _rO;
        private RutasXela.UI.RoundedInput _rD;
        private RutasXela.UI.RoundedInput _rT;
        private RutasXela.UI.RoundedInput _rC;
        private RutasXela.UI.RoundedInput _rL;
        private System.Windows.Forms.CheckBox _rUno;
        private RutasXela.UI.RoundedButton btnAgregarRuta;
        private TextBox txtBuscar;
        private ComboBox cmbOrigen;
        private ComboBox cmbDestino;
        private TextBox txtCodigoParada;
        private TextBox txtNombreParada;
        private TextBox txtLatitud;
        private TextBox txtLongitud;
        private TextBox txtOrigenRuta;
        private TextBox txtDestinoRuta;
        private TextBox txtMinutos;
        private TextBox txtCosto;
        private TextBox txtLinea;
    }
}
