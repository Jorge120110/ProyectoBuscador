using System;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.Net.Http;
using System.Data.SqlClient;


namespace GeneradorInvestigacionApp
{
    internal class GPT
    {
        public async Task<string> GenerarContenido(string tema)
        {
            if (string.IsNullOrEmpty(tema))
            {
                throw new ArgumentException("Agregue un tema para investigar.");
            }

            string apiKey = "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"; // Reemplaza por tu clave válida de OpenAI
            string endpoint = "https://api.openai.com/v1/chat/completions";

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            string promptText =
                $"Realiza una investigación académica profunda sobre el tema: {tema}. " +
                $"La investigación debe tener exactamente 3000 palabras, dividida en las siguientes secciones: " +
                $"1. Introducción: Presenta el tema, su relevancia y objetivos. (Aproximadamente 400 palabras) " +
                $"2. Desarrollo: Analiza el tema en detalle, incluyendo antecedentes, conceptos clave, avances recientes y ejemplos prácticos. Divide esta sección en al menos 4 subsecciones con subtítulos claros. (Aproximadamente 2000 palabras) " +
                $"3. Conclusión: Resume los puntos principales y reflexiona sobre el impacto del tema. (Aproximadamente 300 palabras) " +
                $"4. Recomendaciones: Propón acciones o pasos futuros relacionados con el tema. (Aproximadamente 200 palabras) " +
                $"5. Referencias: Lista al menos 10 referencias en formato APA con enlaces reales y activos. (Aproximadamente 100 palabras) " +
                $"Lineamientos: " +
                $"- No incluyas la cantidad de palabras en los títulos ni en el texto. " +
                $"- Evita asteriscos o identificadores de texto en los títulos. ";

            var requestBody = new
            {
                
                model = "o4-mini",
                messages = new[] {
                    new { role = "system", content = "Eres un experto en redacción académica profesional." },
                    new { role = "user", content = promptText }
                },
                max_completion_tokens = 10000,
            };

            string json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync(endpoint, content);
                if (response.IsSuccessStatusCode)
                {
                    string responseJson = await response.Content.ReadAsStringAsync();
                    using JsonDocument doc = JsonDocument.Parse(responseJson);
                    return doc.RootElement
                        .GetProperty("choices")[0]
                        .GetProperty("message")
                        .GetProperty("content")
                        .GetString();
                }
                else
                {
                    string errorDetails = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Error API: {response.StatusCode} - {errorDetails}");
                }
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Error de conexión: {ex.Message}");
            }
        }

        //Word
        public static void ExportarAWord(string contenido, string rutaArchivo)
        {
            using (var wordDoc = WordprocessingDocument.Create(rutaArchivo, DocumentFormat.OpenXml.WordprocessingDocumentType.Document))
            {
                var mainPart = wordDoc.AddMainDocumentPart();
                mainPart.Document = new DocumentFormat.OpenXml.Wordprocessing.Document();
                var body = mainPart.Document.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Body());
                var paragraphs = contenido.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var parrafo in paragraphs)
                {
                    var p = new DocumentFormat.OpenXml.Wordprocessing.Paragraph();
                   
                    var props = new DocumentFormat.OpenXml.Wordprocessing.ParagraphProperties();
                    props.Justification = new DocumentFormat.OpenXml.Wordprocessing.Justification()
                    {
                        Val = DocumentFormat.OpenXml.Wordprocessing.JustificationValues.Both
                    };
                    p.Append(props);

                    var runProps = new DocumentFormat.OpenXml.Wordprocessing.RunProperties();
                    runProps.RunFonts = new DocumentFormat.OpenXml.Wordprocessing.RunFonts() { Ascii = "Arial", HighAnsi = "Arial" };
                    runProps.FontSize = new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "24" }; 

                    var r = new DocumentFormat.OpenXml.Wordprocessing.Run();
                    r.Append(new DocumentFormat.OpenXml.Wordprocessing.Text(parrafo));
                    p.Append(r);
                    body.Append(p);
                }
                mainPart.Document.Save();
            }
        }
        
        // "PowerPoint" PDF
        public static void ExportarAPptx(string contenido, string rutaArchivo)
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            // Dimensiones de PowerPoint estándar (16:9) en puntos (1 punto = 1/72 pulgadas)

            float widthPt = 1280 * 72f / 96f;
            float heightPt = 720 * 72f / 96f;

            var doc = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(widthPt, heightPt);
                    page.Margin(40);
                    page.Content()
                        .Text(contenido)
                        .FontFamily("Arial")
                        .FontSize(18)
                        .LineHeight(1.3f);
                });
            });
            doc.GeneratePdf(rutaArchivo);
        }

       

        public static void GuardarBusquedaEnSql(string promptCompleto)
        {
            string connectionString = "Server=PC1\\SQLEXPRESS;Database=Buscador;Trusted_Connection=True;";
            string query = "INSERT INTO dbo.[Prompts] ([Prompt]) VALUES (@prompt)";
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@prompt", promptCompleto);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public static void GuardarTemaEnSql(string tema)
        {
            string connectionString = "Server=PC1\\SQLEXPRESS;Database=Buscador;Trusted_Connection=True;";
            string query = "INSERT INTO dbo.[Tabla busquedaa] ([Busqueda ingresada]) VALUES (@tema)";
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@tema", tema);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}
