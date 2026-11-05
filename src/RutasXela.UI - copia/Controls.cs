using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace RutasXela.UI;

// ─────────────────────────────────────────────────────────────
//  TEMA + CONTROLES PERSONALIZADOS (esquinas redondeadas)
// ─────────────────────────────────────────────────────────────
public static class Theme
{
    public static readonly Color Fondo = ColorTranslator.FromHtml("#F4F5FA");
    public static readonly Color Navy = ColorTranslator.FromHtml("#1F2A44");
    public static readonly Color Amarillo = ColorTranslator.FromHtml("#F7C400");
    public static readonly Color Texto = ColorTranslator.FromHtml("#2B3245");
    public static readonly Color Gris = ColorTranslator.FromHtml("#7B8499");
    public static readonly Color Campo = ColorTranslator.FromHtml("#F1F3F8");
    public static readonly Color Borde = ColorTranslator.FromHtml("#E1E5EE");
    public static readonly Color Verde = ColorTranslator.FromHtml("#2FB170");
    public static readonly Color Rojo = ColorTranslator.FromHtml("#E5484D");

    // Colores de las líneas (inspirados en tu imagen)
    public static readonly Color[] Lineas =
    {
        ColorTranslator.FromHtml("#F7C400"), // 1 amarillo
        ColorTranslator.FromHtml("#0B8F43"), // 2 verde
        ColorTranslator.FromHtml("#F58220"), // 3 naranja
        ColorTranslator.FromHtml("#7B5EA7"), // 4 morado
        ColorTranslator.FromHtml("#7FA3C7"), // 5 azul
        ColorTranslator.FromHtml("#C2158C"), // 6 magenta
        ColorTranslator.FromHtml("#8A8A8A"), // 7 gris
    };

    public static Color Linea(int n) => Lineas[(Math.Max(n, 1) - 1) % Lineas.Length];

    public static GraphicsPath Round(RectangleF r, float radius)
    {
        float d = Math.Min(radius * 2, Math.Min(r.Width, r.Height));
        var p = new GraphicsPath();
        p.AddArc(r.X, r.Y, d, d, 180, 90);
        p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
        p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
        p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
        p.CloseFigure();
        return p;
    }

    public static Color FondoDe(Control c) =>
        c.Parent is RoundedPanel rp ? rp.FillColor : (c.Parent?.BackColor ?? Color.White);
}

// Tarjeta con esquinas redondeadas
public class RoundedPanel : Panel
{
    [DefaultValue(18)]
    public int Radius { get; set; } = 18;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Color FillColor { get; set; } = Color.White;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Color BorderColor { get; set; } = Theme.Borde;

    public RoundedPanel()
    {
        DoubleBuffered = true;
        ResizeRedraw = true;
        BackColor = Theme.Fondo;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        using var path = Theme.Round(new RectangleF(0.5f, 0.5f, Width - 1.5f, Height - 1.5f), Radius);
        using var br = new SolidBrush(FillColor);
        using var pen = new Pen(BorderColor, 1);
        e.Graphics.FillPath(br, path);
        e.Graphics.DrawPath(pen, path);
    }
}

// Botón redondeado con efecto hover
public class RoundedButton : Button
{
    [DefaultValue(14)]
    public int Radius { get; set; } = 14;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Color Normal { get; set; } = Theme.Navy;
    private bool _hover, _down;

    public RoundedButton()
    {
        FlatStyle = FlatStyle.Flat;
        FlatAppearance.BorderSize = 0;
        Cursor = Cursors.Hand;
        Height = 38;
        ForeColor = Color.White;
        Font = new Font("Segoe UI Semibold", 9.5f);
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
    }

    protected override void OnMouseEnter(EventArgs e) { _hover = true; Invalidate(); base.OnMouseEnter(e); }
    protected override void OnMouseLeave(EventArgs e) { _hover = false; _down = false; Invalidate(); base.OnMouseLeave(e); }
    protected override void OnMouseDown(MouseEventArgs e) { _down = true; Invalidate(); base.OnMouseDown(e); }
    protected override void OnMouseUp(MouseEventArgs e) { _down = false; Invalidate(); base.OnMouseUp(e); }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.Clear(Theme.FondoDe(this));
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

        Color c = !Enabled ? Color.FromArgb(190, 195, 205)
                : _down ? ControlPaint.Dark(Normal, .15f)
                : _hover ? ControlPaint.Dark(Normal, .08f)
                : Normal;

        using var path = Theme.Round(new RectangleF(0, 0, Width - 1, Height - 1), Radius);
        using var br = new SolidBrush(c);
        using var tb = new SolidBrush(ForeColor);
        using var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        g.FillPath(br, path);
        g.DrawString(Text, Font, tb, new RectangleF(0, 0, Width, Height), sf);
    }
}

// Campo de entrada redondeado (envuelve un TextBox o un ComboBox)
public class RoundedInput : Control
{
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Control Inner { get; }

    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Valor { get => Inner.Text; set => Inner.Text = value; }

    // Texto gris de ayuda (solo aplica cuando el campo interno es un TextBox)
    [Category("Appearance"), DefaultValue("")]
    public string Placeholder
    {
        get => (Inner as TextBox)?.PlaceholderText ?? "";
        set { if (Inner is TextBox tb) tb.PlaceholderText = value; }
    }

    private bool _focus;

    // Constructor sin parámetros: necesario para que el diseñador pueda crear el control
    public RoundedInput() : this(new TextBox()) { }

    protected RoundedInput(Control inner)
    {
        Inner = inner;
        Height = 40;
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);

        inner.Font = new Font("Segoe UI", 10f);
        inner.BackColor = Theme.Campo;
        inner.ForeColor = Theme.Texto;
        if (inner is TextBox tb) tb.BorderStyle = BorderStyle.None;
        if (inner is ComboBox cb) { cb.FlatStyle = FlatStyle.Flat; cb.DropDownStyle = ComboBoxStyle.DropDownList; }
        inner.GotFocus += (_, _) => { _focus = true; Invalidate(); };
        inner.LostFocus += (_, _) => { _focus = false; Invalidate(); };
        Controls.Add(inner);
        Acomodar();
    }

    private void Acomodar()
    {
        Inner.Left = 12;
        Inner.Width = Math.Max(10, Width - 24);
        Inner.Top = Math.Max(0, (Height - Inner.Height) / 2);
    }

    protected override void OnResize(EventArgs e) { base.OnResize(e); Acomodar(); }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.Clear(Theme.FondoDe(this));
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var path = Theme.Round(new RectangleF(0.5f, 0.5f, Width - 2, Height - 2), 12);
        using var br = new SolidBrush(Theme.Campo);
        using var pen = new Pen(_focus ? Theme.Navy : Theme.Borde, _focus ? 1.6f : 1f);
        g.FillPath(br, path);
        g.DrawPath(pen, path);
    }
}

// Versión del campo redondeado que contiene un ComboBox
public class RoundedComboInput : RoundedInput
{
    public RoundedComboInput() : base(new ComboBox()) { }

    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ComboBox Combo => (ComboBox)Inner;
}

// ─────────────────────────────────────────────────────────────
//  MAPA ESTILO METRO: dibuja paradas, rutas y la ruta calculada
// ─────────────────────────────────────────────────────────────
public class MapPanel : RoundedPanel
{
    private Parada[] _paradas = Array.Empty<Parada>();

    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Parada[] Paradas
    {
        get => _paradas;
        set
        {
            _paradas = value ?? Array.Empty<Parada>();
            Invalidate();
        }
    }

    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Ruta[] Rutas { get; set; } = Array.Empty<Ruta>();
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string[] Camino { get; set; }       // ruta resaltada (Dijkstra)
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string[] Recorrido { get; set; }    // orden de visita (BFS / DFS)
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Seleccion { get; set; }      // parada seleccionada en el buscador
    public event Action<string> ParadaSeleccionada;

    private Dictionary<string, PointF> CalcularPosiciones()
    {
        var pos = new Dictionary<string, PointF>();
        if (Paradas.Length == 0) return pos;

        double anchoGeografico = AreaGeograficaXela.CoordenadaXMaxima - AreaGeograficaXela.CoordenadaXMinima;
        double altoGeografico = AreaGeograficaXela.CoordenadaYMaxima - AreaGeograficaXela.CoordenadaYMinima;
        float padX = 60, padTop = 60, padBottom = 60;
        float anchoDisponible = Math.Max(1, Width - padX * 2);
        float altoDisponible = Math.Max(1, Height - padTop - padBottom);
        double escala = Math.Min(anchoDisponible / anchoGeografico, altoDisponible / altoGeografico) * 1.16;
        double anchoMapa = anchoGeografico * escala;
        double altoMapa = altoGeografico * escala;
        double inicioX = (Width - anchoMapa) / 2;
        double inicioY = padTop + (altoDisponible - altoMapa) / 2;

        foreach (var p in Paradas)
        {
            float x = (float)(inicioX + (p.Longitud - AreaGeograficaXela.CoordenadaXMinima) * escala);
            float y = (float)(inicioY + (AreaGeograficaXela.CoordenadaYMaxima - p.Latitud) * escala);
            pos[p.Codigo] = new PointF(x, y);
        }
        return pos;
    }

    protected override void OnMouseClick(MouseEventArgs e)
    {
        base.OnMouseClick(e);
        var posiciones = CalcularPosiciones();
        string seleccionada = null;
        double distanciaMinima = 18 * 18;

        foreach (var parada in Paradas)
        {
            if (!posiciones.TryGetValue(parada.Codigo, out PointF posicion))
            {
                continue;
            }

            double dx = e.X - posicion.X;
            double dy = e.Y - posicion.Y;
            double distancia = dx * dx + dy * dy;
            if (distancia <= distanciaMinima)
            {
                distanciaMinima = distancia;
                seleccionada = parada.Codigo;
            }
        }

        Seleccion = seleccionada;
        Invalidate();
        ParadaSeleccionada?.Invoke(seleccionada);
    }

    // Línea con un codo a 45° (look de mapa de metro)
    private static PointF[] Elbow(PointF a, PointF b)
    {
        float dx = b.X - a.X, dy = b.Y - a.Y, adx = Math.Abs(dx), ady = Math.Abs(dy);
        if (adx < 1 || ady < 1) return new[] { a, b };
        float d = Math.Min(adx, ady);
        PointF m = adx > ady
            ? new PointF(a.X + Math.Sign(dx) * (adx - d), a.Y)
            : new PointF(a.X, a.Y + Math.Sign(dy) * (ady - d));
        return new[] { a, m, b };
    }

    private bool EnCamino(Ruta r)
    {
        if (Camino == null) return false;
        for (int i = 0; i < Camino.Length - 1; i++)
            if ((Camino[i] == r.Origen && Camino[i + 1] == r.Destino) ||
                (!r.UnSentido && Camino[i] == r.Destino && Camino[i + 1] == r.Origen)) return true;
        return false;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

        if (Paradas.Length == 0)
        {
            using var f0 = new Font("Segoe UI", 12f);
            g.DrawString("Sin datos. Carga un CSV o agrega paradas.", f0, Brushes.Gray, 24, 24);
            return;
        }

        var pos = CalcularPosiciones();
        bool hayCamino = Camino != null && Camino.Length > 1;

        // 1) Rutas (primero las normales, después las resaltadas)
        for (int pasada = 0; pasada < 2; pasada++)
            foreach (var r in Rutas)
            {
                if (!pos.TryGetValue(r.Origen, out var a) || !pos.TryGetValue(r.Destino, out var b)) continue;
                bool resaltada = hayCamino && EnCamino(r);
                if ((pasada == 0) == resaltada) continue;

                var col = Theme.Linea(r.Linea);
                if (hayCamino && !resaltada) col = Color.FromArgb(70, col);
                var pts = Elbow(a, b);

                if (resaltada)
                {
                    using var halo = new Pen(Theme.Navy, 16) { StartCap = LineCap.Round, EndCap = LineCap.Round, LineJoin = LineJoin.Round };
                    g.DrawLines(halo, pts);
                }
                using var pen = new Pen(col, 9) { StartCap = LineCap.Round, EndCap = LineCap.Round, LineJoin = LineJoin.Round };
                g.DrawLines(pen, pts);
            }

        // 2) Paradas
        using var fNombre = new Font("Segoe UI Semibold", 9f);
        using var fNum = new Font("Segoe UI", 8f, FontStyle.Bold);
        using var sf = new StringFormat { Alignment = StringAlignment.Center };
        using var sfC = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        using var brTexto = new SolidBrush(Theme.Navy);

        foreach (var p in Paradas)
        {
            var c = pos[p.Codigo];
            Color relleno = Color.White;
            if (hayCamino && p.Codigo == Camino[0]) relleno = Theme.Verde;
            else if (hayCamino && p.Codigo == Camino[^1]) relleno = Theme.Rojo;

            if (p.Codigo == Seleccion)
            {
                using var ring = new Pen(Theme.Amarillo, 4);
                g.DrawEllipse(ring, c.X - 17, c.Y - 17, 34, 34);
            }
            using var brR = new SolidBrush(relleno);
            using var penB = new Pen(Theme.Navy, 3.5f);
            g.FillEllipse(brR, c.X - 11, c.Y - 11, 22, 22);
            g.DrawEllipse(penB, c.X - 11, c.Y - 11, 22, 22);
            g.DrawString(p.Nombre, fNombre, brTexto, c.X, c.Y + 16, sf);
        }

        // 3) Orden de recorrido BFS/DFS
        if (Recorrido != null)
            for (int i = 0; i < Recorrido.Length; i++)
            {
                if (!pos.TryGetValue(Recorrido[i], out var c)) continue;
                var rect = new RectangleF(c.X + 8, c.Y - 28, 20, 20);
                using var brB = new SolidBrush(Theme.Navy);
                g.FillEllipse(brB, rect);
                g.DrawString((i + 1).ToString(), fNum, Brushes.White, rect, sfC);
            }

        // 4) Leyenda de líneas (círculos numerados como en tu imagen)
        int x = 22;
        foreach (int l in Rutas.Select(r => r.Linea).Distinct().OrderBy(n => n))
        {
            using var brL = new SolidBrush(Theme.Linea(l));
            g.FillEllipse(brL, x, 20, 26, 26);
            g.DrawString(l.ToString(), fNum, l == 1 ? brTexto : Brushes.White, new RectangleF(x, 20, 26, 26), sfC);
            x += 34;
        }
    }
}
