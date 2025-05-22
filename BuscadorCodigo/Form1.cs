using System;
using System.Windows.Forms;
using System.IO;
using Xceed.Words.NET;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml;

namespace GeneradorInvestigacionApp
{
    public partial class Form1 : Form
    {
        private string contenidoGenerado = string.Empty;
        public Form1()
        {
            InitializeComponent();
        }

        private async void btnGenerar_Click(object sender, EventArgs e)
        {
            btnGenerar.Enabled = false;
            rtbResultado.Text = "Generando investigación, por favor espera...";
            try
            {
                var gpt = new GPT();
                string tema = txtTema.Text.Trim();
                // Guardar el tema en la base de datos
                GPT.GuardarTemaEnSql(tema);
                // Construir el prompt igual que en GPT.cs
                string promptText = $"Realiza una investigación académica profunda sobre el tema: {tema}. La investigación debe tener exactamente 3000 palabras, dividida en las siguientes secciones: " +
                    $"1. Introducción (400 palabras): Presenta el tema, su relevancia y objetivos. " +
                    $"2. Desarrollo (2000 palabras): Analiza el tema en detalle, incluyendo antecedentes, conceptos clave, avances recientes y ejemplos prácticos. Divide esta sección en al menos 4 subsecciones con subtítulos claros. " +
                    $"3. Conclusión (300 palabras): Resume los puntos principales y reflexiona sobre el impacto del tema. " +
                    $"4. Recomendaciones (200 palabras): Propón acciones o pasos futuros relacionados con el tema. " +
                    $"5. Referencias (100 palabras): Lista al menos 10 referencias en formato APA con enlaces activos. " +
                    $"Lineamientos: " +
                    $"- Evita asteriscos o identificadores de texto. " +
                    $"- No uses etiquetas como 'Desarrollo' antes de las secciones. " +
                    $"- Centra los títulos de Introducción, Conclusión, Recomendaciones y Referencias. " +
                    $"- Justifica el contenido de cada sección. " +
                    $"- Usa 10 líneas de espaciado entre títulos principales. " +
                    $"- Al final, incluye una nota confirmando que la investigación tiene 3000 palabras.";

                // Guardar el tema y el prompt en la base de datos
                GPT.GuardarBusquedaEnSql(promptText);

                string resultado = await gpt.GenerarContenido(tema);
                contenidoGenerado = resultado;
                rtbResultado.Text = resultado;

                // Crear carpeta 'Investigaciones' en el escritorio si no existe
                string carpeta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Investigaciones");
                if (!Directory.Exists(carpeta))
                    Directory.CreateDirectory(carpeta);

                // Guardar automáticamente Word y PDF en la carpeta

                string rutaWord = Path.Combine(carpeta, $"Investigacion_{tema}.docx");
                GPT.ExportarAWord(contenidoGenerado, rutaWord);

                string rutaPdf = Path.Combine(carpeta, $"Investigacion_{tema}.pdf");
                GPT.ExportarAPptx(contenidoGenerado, rutaPdf);

                MessageBox.Show($"Investigación generada y guardada en:{carpeta}", "con éxito");
            }

            catch (Exception ex)
            {
                rtbResultado.Text = $"Error: {ex.Message}";
            }
            finally
            {
                btnGenerar.Enabled = true;
            }
        }

        private void rtbResultado_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
