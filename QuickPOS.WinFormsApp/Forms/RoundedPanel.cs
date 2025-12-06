using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;

namespace QuickPOS.WinFormsApp.Forms
{
    public class RoundedPanel : Panel
    {
        // --- PROPIEDADES VISUALES ---
        [Category("QuickPOS Design")]
        [Description("Radio de las esquinas.")]
        public int CornerRadius { get; set; } = 30;

        [Category("QuickPOS Design")]
        [Description("Activar o desactivar sombra.")]
        public bool Shadow { get; set; } = true;

        [Category("QuickPOS Design")]
        [Description("Qué tan lejos cae la sombra (Profundidad).")]
        public int ShadowDepth { get; set; } = 6;

        [Category("QuickPOS Design")]
        [Description("Desplazamiento vertical de la sombra (Para efecto de luz cenital).")]
        public int ShadowYOffset { get; set; } = 3;

        [Category("QuickPOS Design")]
        [Description("Opacidad inicial de la sombra (0-255). Menos es más suave.")]
        public int ShadowOpacity { get; set; } = 30;

        [Category("QuickPOS Design")]
        [Description("Color de fondo de la tarjeta.")]
        public Color CardColor { get; set; } = Color.White;

        public RoundedPanel()
        {
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.SetStyle(ControlStyles.ResizeRedraw | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            this.BackColor = Color.Transparent;
            // Margen automático seguro
            this.Padding = new Padding(10);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Calidad Máxima de Renderizado
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            // Definir el rectángulo de la tarjeta (restando el espacio de la sombra)
            // Ajustamos el alto para que la sombra de abajo no se corte
            var rect = new Rectangle(
                ShadowDepth,
                ShadowDepth,
                this.Width - (ShadowDepth * 2) - 1,
                this.Height - (ShadowDepth * 2) - ShadowYOffset - 1
            );

            // 1. DIBUJAR SOMBRA (Efecto Drop Shadow)
            if (Shadow)
            {
                int layers = ShadowDepth; // Cantidad de capas de difuminado

                for (int i = 0; i < layers; i++)
                {
                    // Calculamos la opacidad: Se desvanece hacia afuera
                    // La capa más interna es más oscura, la externa casi invisible
                    int alpha = Math.Max(0, ShadowOpacity - (i * (ShadowOpacity / layers)));

                    using (var shadowBrush = new SolidBrush(Color.FromArgb(alpha, Color.Black)))
                    {
                        // Hacemos que la sombra se desplace un poco hacia abajo en cada capa (Y + i/2)
                        // Esto crea el efecto de luz viniendo desde arriba
                        var shadowRect = new Rectangle(
                            rect.X - i + (ShadowYOffset / 3),
                            rect.Y - i + (i / 3) + ShadowYOffset,
                            rect.Width + (i * 2),
                            rect.Height + (i * 2)
                        );

                        using (var path = RoundedRect(shadowRect, CornerRadius))
                        {
                            e.Graphics.FillPath(shadowBrush, path);
                        }
                    }
                }
            }

            // 2. DIBUJAR LA TARJETA
            using (var brush = new SolidBrush(CardColor))
            using (var path = RoundedRect(rect, CornerRadius))
            {
                e.Graphics.FillPath(brush, path);

                // Borde sutil para definición (opcional, color gris muy claro)
                using (var pen = new Pen(Color.FromArgb(10, Color.Black), 1f))
                {
                    e.Graphics.DrawPath(pen, path);
                }
            }
        }

        private GraphicsPath RoundedRect(Rectangle r, int r1)
        {
            var path = new GraphicsPath();
            int d = r1 * 2;
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}