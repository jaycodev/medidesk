using System.Text.Json;

namespace Web.Helpers
{
    public static class HttpHelper
    {
        public static string ExtractErrorMessage(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return "No se pudo procesar la solicitud.";

            try
            {
                if (content.Contains("\"message\""))
                {
                    var start = content.IndexOf("\"message\"", StringComparison.OrdinalIgnoreCase);
                    var colon = content.IndexOf(':', start);
                    var trimmed = content.Substring(colon + 1).Trim().Trim('"', ' ', '}');
                    return trimmed;
                }

                if (content.Contains("\"error\""))
                {
                    var start = content.IndexOf("\"error\"", StringComparison.OrdinalIgnoreCase);
                    var colon = content.IndexOf(':', start);
                    var trimmed = content.Substring(colon + 1).Trim().Trim('"', ' ', '}');
                    return trimmed;
                }
            }
            catch { }

            return content.Length > 300 ? content[..300] + "..." : content;
        }

        public static async Task<string> ExtractErrorMessageAsync(HttpResponseMessage resp)
        {
            try
            {
                var content = await resp.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(content)) return "Ocurrió un error en la petición.";

                try
                {
                    using var doc = JsonDocument.Parse(content);
                    if (doc.RootElement.ValueKind == JsonValueKind.Object && doc.RootElement.TryGetProperty("message", out var msgProp))
                    {
                        return msgProp.GetString() ?? content;
                    }
                }
                catch { }

                return content;
            }
            catch
            {
                return "Ocurrió un error al leer la respuesta del servidor.";
            }
        }
    }
}
