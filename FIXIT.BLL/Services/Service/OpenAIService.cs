//using System.Text;
//using System.Text.Json;
//using System.Text.RegularExpressions;
//using FIXIT.BLL.DTOs.CraftsmanDTOs;
//using FIXIT.BLL.Services.Intrfaces;
//using FIXIT.BLL.Services.IService;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;

//namespace FIXIT.BLL.Services.Service
//{
//    public class OpenAIService : IOpenAIService
//    {
//        private readonly HttpClient _httpClient;
//        private readonly string _apiKey;
//        private readonly ILogger<OpenAIService> _logger;

//        public OpenAIService(
//            IHttpClientFactory httpClientFactory,
//            IConfiguration configuration,
//            ILogger<OpenAIService> logger)
//        {
//            _httpClient = httpClientFactory.CreateClient("OpenAI");
//            _apiKey = configuration["OpenAI:ApiKey"]
//                ?? throw new InvalidOperationException("OpenAI API key not configured");
//            _logger = logger;

//            _httpClient.BaseAddress = new Uri("https://api.openai.com/v1/");
//            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
//        }

//        public async Task<IdVerificationResponseDto> VerifyEgyptianNationalIdAsync(
//      IFormFile frontImage,
//      IFormFile backImage)
//        {
//            try
//            {
//                // Convert images to base64
//                var frontBase64 = await ConvertToBase64(frontImage);
//                var backBase64 = await ConvertToBase64(backImage);

//                // Prepare request with response_format to enforce JSON
//                var requestBody = new
//                {
//                    model = "gpt-4o",
//                    messages = new[]
//                    {
//                new
//                {
//                    role = "user",
//                    content = new object[]
//                    {
//                        new { type = "text", text = GetVerificationPrompt() },
//                        new { type = "image_url", image_url = new { url = $"data:image/jpeg;base64,{frontBase64}" } },
//                        new { type = "image_url", image_url = new { url = $"data:image/jpeg;base64,{backBase64}" } }
//                    }
//                }
//            },
//                    max_tokens = 1000,
//                    temperature = 0.1,
//                    // 👇 ADD THIS LINE - Forces JSON response
//                    response_format = new { type = "json_object" }
//                };

//                var json = JsonSerializer.Serialize(requestBody);
//                var content = new StringContent(json, Encoding.UTF8, "application/json");

//                var response = await _httpClient.PostAsync("chat/completions", content);
//                response.EnsureSuccessStatusCode();

//                var responseBody = await response.Content.ReadAsStringAsync();
//                var openAiResponse = JsonSerializer.Deserialize<JsonElement>(responseBody);

//                var aiContent = openAiResponse
//                    .GetProperty("choices")[0]
//                    .GetProperty("message")
//                    .GetProperty("content")
//                    .GetString();

//                _logger.LogInformation($"OpenAI Response: {aiContent}");

//                // 👇 IMPROVED JSON PARSING with better error handling
//                // Clean the response - remove markdown code blocks if present
//                var cleanedContent = aiContent?.Trim() ?? string.Empty;

//                if (cleanedContent.StartsWith("```json"))
//                {
//                    cleanedContent = cleanedContent.Substring(7);
//                }
//                if (cleanedContent.StartsWith("```"))
//                {
//                    cleanedContent = cleanedContent.Substring(3);
//                }
//                if (cleanedContent.EndsWith("```"))
//                {
//                    cleanedContent = cleanedContent.Substring(0, cleanedContent.Length - 3);
//                }
//                cleanedContent = cleanedContent.Trim();

//                // Try to extract JSON if it's embedded in text
//                if (!cleanedContent.StartsWith("{"))
//                {
//                    var jsonMatch = Regex.Match(cleanedContent, @"\{[\s\S]*\}", RegexOptions.Multiline);
//                    if (jsonMatch.Success)
//                    {
//                        cleanedContent = jsonMatch.Value;
//                    }
//                    else
//                    {
//                        _logger.LogError($"Failed to extract JSON from response: {aiContent}");
//                        return new IdVerificationResponseDto
//                        {
//                            IsValid = false,
//                            Message = "AI service returned an invalid response format.",
//                            Errors = new List<string> { "Unable to parse verification result. Response did not contain valid JSON." }
//                        };
//                    }
//                }

//                try
//                {
//                    var verificationResult = JsonSerializer.Deserialize<IdVerificationResponseDto>(
//                        cleanedContent,
//                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

//                    return verificationResult ?? new IdVerificationResponseDto
//                    {
//                        IsValid = false,
//                        Message = "Failed to parse verification result",
//                        Errors = new List<string> { "Invalid response format from AI" }
//                    };
//                }
//                catch (JsonException jsonEx)
//                {
//                    _logger.LogError(jsonEx, $"JSON parsing error. Content: {cleanedContent}");
//                    return new IdVerificationResponseDto
//                    {
//                        IsValid = false,
//                        Message = "Failed to parse AI response",
//                        Errors = new List<string> { $"JSON parsing error: {jsonEx.Message}" }
//                    };
//                }
//            }
//            catch (HttpRequestException httpEx)
//            {
//                _logger.LogError(httpEx, "HTTP error calling OpenAI API");
//                return new IdVerificationResponseDto
//                {
//                    IsValid = false,
//                    Message = "Network error connecting to verification service.",
//                    Errors = new List<string> { httpEx.Message }
//                };
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error verifying national ID");
//                return new IdVerificationResponseDto
//                {
//                    IsValid = false,
//                    Message = "Verification service error. Please try again later.",
//                    Errors = new List<string> { ex.Message }
//                };
//            }
//        }

//        private async Task<string> ConvertToBase64(IFormFile file)
//        {
//            using var memoryStream = new MemoryStream();
//            await file.CopyToAsync(memoryStream);
//            return Convert.ToBase64String(memoryStream.ToArray());
//        }

//        private string GetVerificationPrompt()
//        {
//            return @"You are an expert Egyptian National ID verification system with ADVANCED OCR capabilities. Analyze the two provided images (front and back of an Egyptian National ID card) with EXTREME ATTENTION to date reading accuracy.

//CRITICAL: You MUST respond with ONLY valid JSON. No markdown, no explanations, just pure JSON.

//⚠️ SPECIAL INSTRUCTIONS FOR DATE READING:
//Egyptian National IDs use DD/MM/YYYY format. The card shows TWO dates:
//1. ISSUE DATE (تاريخ الإصدار) - When the card was issued
//2. EXPIRY DATE (تاريخ الانتهاء) - When the card expires (usually 7 years after issue date)

//CAREFUL NUMBER RECOGNITION:
//- Take EXTRA CARE distinguishing between: 2, 3, 5, 8
//- The digit '2' (٢) in Arabic looks like '٢'
//- The digit '3' (٣) in Arabic looks like '٣'
//- DO NOT confuse 2023 with 2033 or 2025 with 2035
//- Egyptian IDs issued in 2016 expire in 2023
//- Egyptian IDs issued in 2017 expire in 2024
//- Egyptian IDs issued in 2023 expire in 2030
//- Egyptian IDs issued after 2023 expire 7 years later

//DATE VERIFICATION PROCESS:
//1. Locate BOTH dates on the ID (usually at the bottom of the front side)
//2. The EXPIRY DATE is typically on the right side
//3. The ISSUE DATE is typically on the left side  
//4. Read each digit CAREFULLY and INDIVIDUALLY
//5. Verify the expiry date is 7 years after the issue date
//6. Double-check that the expiry date makes logical sense
//7. Today's date is " + DateTime.Now.ToString("yyyy-MM-dd") + @"
//8. If the expiry year is 2023 or earlier, the card IS expired
//9. If the expiry year is 2024 or later, check the full date

//VALIDATION RULES:

//1. IMAGE QUALITY REQUIREMENTS:
//   - Both images must be clear and well-lit
//   - Text must be fully readable without blurriness
//   - ALL NUMBERS must be clearly visible
//   - Images must not be cropped or cut off
//   - The ID card must not be covered, damaged, or rotated
//   - No glare or shadows obscuring text or numbers
//   - Both photos must show a complete ID card
//   - The DATE AREA must be especially clear and readable

//2. DOCUMENT TYPE VERIFICATION:
//   - Must be a valid Egyptian National ID (البطاقة الشخصية المصرية)
//   - Not a passport, driver's license, or any other document
//   - Must contain the Egyptian national emblem/logo

//3. REQUIRED FIELDS VERIFICATION (Front):
//   - Full Name (in Arabic)
//   - National ID Number (14 digits)
//   - Date of Birth
//   - Gender
//   - Governorate
//   - Marital Status
//   - Religion
//   - Photo of the holder
//   - Issue Date (تاريخ الإصدار)
//   - Expiry Date (تاريخ الانتهاء) - READ THIS VERY CAREFULLY

//4. REQUIRED FIELDS VERIFICATION (Back):
//   - Address details (in Arabic)
//   - Signature or fingerprint
//   - Issue date
//   - Machine-readable zone (MRZ) if applicable

//5. EXPIRY DATE VALIDATION:
//   - Extract the EXPIRY date (NOT the issue date)
//   - Format: DD/MM/YYYY
//   - Compare against today's date: " + DateTime.Now.ToString("yyyy-MM-dd") + @"
//   - If expired, set isExpired=true and add to errors
//   - If the date seems illogical (e.g., expires in 1990), RE-READ IT CAREFULLY

//6. CONSISTENCY CHECKS:
//   - Front and back must belong to the same person
//   - National ID number must match on both sides (if present on both)
//   - Information must be consistent between both sides
//   - Issue date + 7 years should equal expiry date (approximately)

//RESPONSE FORMAT (JSON ONLY):
//{
//  ""isValid"": boolean,
//  ""message"": ""Brief summary message"",
//  ""errors"": [""List of validation errors if any""],
//  ""warnings"": [""List of warnings if any""],
//  ""extractedData"": {
//    ""fullName"": ""string or null"",
//    ""nationalIdNumber"": ""string or null"",
//    ""expiryDate"": ""YYYY-MM-DD or null"",
//    ""isExpired"": boolean
//  }
//}

//IMPORTANT RULES:
//- If the ID is valid with no issues: isValid=true, errors=[], message=""National ID verified successfully""
//- If the ID has any issues: isValid=false, errors=[list of specific issues]
//- Always extract fullName, nationalIdNumber, expiryDate if visible
//- If ID is expired: isValid=false, isExpired=true, add ""National ID card has expired"" to errors
//- If images are too blurry to read DATES: isValid=false, add ""Date area is too blurry to verify - please upload a clearer image"" to errors
//- If not an Egyptian National ID: isValid=false, add ""Document is not an Egyptian National ID"" to errors
//- If you cannot read the expiry date with confidence: Add a warning ""Unable to read expiry date clearly"" and request better images

//⚠️ CRITICAL: If the expiry date you read seems unlikely (e.g., 2023 when it should be 2033), STOP and RE-READ it digit by digit.

//Remember: Respond with ONLY the JSON object. No other text.";
//        }
//        private string GetDateValidationPrompt(string suspiciousDate)
//        {
//            return $@"CRITICAL TASK: Verify this date reading from an Egyptian National ID card.

//The system initially read the EXPIRY DATE as: {suspiciousDate}

//Please RE-READ the expiry date (تاريخ الانتهاء) from the provided ID images with EXTREME CARE.

//INSTRUCTIONS:
//1. Locate the expiry date area (usually bottom right of front side)
//2. Read each digit INDIVIDUALLY
//3. Egyptian date format is DD/MM/YYYY
//4. Common confusions: 2 vs 3, 2023 vs 2033, 2025 vs 2035
//5. Today's date is {DateTime.Now:yyyy-MM-dd}

//Respond ONLY with a JSON 
//  ""correctedDate"": ""YYYY-MM-DD"",
//  ""confidence"": ""high/medium/low"",
//  ""reasoning"": ""Brief explanation of what you see""
//}}";
//        }
//    }
//}


using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using FIXIT.BLL.DTOs.CraftsmanDTOs;
using FIXIT.BLL.Services.Intrfaces;
using FIXIT.BLL.Services.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FIXIT.BLL.Services.Service
{
    public class OpenAIService : IOpenAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly ILogger<OpenAIService> _logger;

        public OpenAIService(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<OpenAIService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("OpenAI");
            _apiKey = configuration["OpenAI:ApiKey"]
                ?? throw new InvalidOperationException("OpenAI API key not configured");
            _logger = logger;

            _httpClient.BaseAddress = new Uri("https://api.openai.com/v1/");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        }

        public async Task<IdVerificationResponseDto> VerifyEgyptianNationalIdAsync(
            IFormFile frontImage,
            IFormFile backImage)
        {
            try
            {
                var frontBase64 = await ConvertToBase64(frontImage);
                var backBase64 = await ConvertToBase64(backImage);

                var requestBody = new
                {
                    model = "gpt-4o",
                    messages = new[]
                    {
                        new
                        {
                            role = "user",
                            content = new object[]
                            {
                                new { type = "text", text = GetVerificationPrompt() },
                                new { type = "image_url", image_url = new { url = $"data:image/jpeg;base64,{frontBase64}" } },
                                new { type = "image_url", image_url = new { url = $"data:image/jpeg;base64,{backBase64}" } }
                            }
                        }
                    },
                    max_tokens = 1000,
                    temperature = 0.05,
                    // NOTE: ensure API supports response_format; if not, keep but handle free-text fallback
                    response_format = new { type = "json_object" }
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("chat/completions", content);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                var openAiResponse = JsonSerializer.Deserialize<JsonElement>(responseBody);

                var aiContent = openAiResponse
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

                _logger.LogInformation($"OpenAI Response (raw): {aiContent}");

                var cleanedContent = aiContent?.Trim() ?? string.Empty;

                // Remove code fences if present
                if (cleanedContent.StartsWith("```json"))
                    cleanedContent = cleanedContent.Substring(7);
                if (cleanedContent.StartsWith("```"))
                    cleanedContent = cleanedContent.Substring(3);
                if (cleanedContent.EndsWith("```"))
                    cleanedContent = cleanedContent.Substring(0, cleanedContent.Length - 3);
                cleanedContent = cleanedContent.Trim();

                // Try to extract JSON object if embedded
                if (!cleanedContent.StartsWith("{"))
                {
                    var jsonMatch = Regex.Match(cleanedContent, @"\{[\s\S]*\}", RegexOptions.Multiline);
                    if (jsonMatch.Success)
                    {
                        cleanedContent = jsonMatch.Value;
                    }
                    else
                    {
                        _logger.LogError($"Failed to extract JSON from response: {aiContent}");
                        return new IdVerificationResponseDto
                        {
                            IsValid = false,
                            Message = "AI service returned an invalid response format.",
                            Errors = new List<string> { "Unable to parse verification result. Response did not contain valid JSON." }
                        };
                    }
                }

                // Normalize numerals inside the JSON string (Arabic-Indic -> ASCII)
                cleanedContent = NormalizeArabicIndicDigitsInText(cleanedContent);

                // Parse into JsonDocument for post-processing
                JsonDocument doc;
                try
                {
                    doc = JsonDocument.Parse(cleanedContent);
                }
                catch (JsonException je)
                {
                    _logger.LogError(je, $"JSON parse failed after normalization. Content: {cleanedContent}");
                    return new IdVerificationResponseDto
                    {
                        IsValid = false,
                        Message = "Failed to parse AI response after normalization",
                        Errors = new List<string> { $"JSON parsing error: {je.Message}" }
                    };
                }

                // Map to DTO but with corrections
                var root = doc.RootElement;

                var resultDto = new IdVerificationResponseDto
                {
                    IsValid = root.GetPropertyOrDefaultBool("isValid"),
                    Message = root.GetPropertyOrDefaultString("message"),
                    Errors = root.GetPropertyOrDefaultStringArray("errors"),
                    Warnings = root.GetPropertyOrDefaultStringArray("warnings")
                    
                };

                // Extracted data normalization & corrections
                if (root.TryGetProperty("extractedData", out var extracted))
                {
                    resultDto.ExtractedData.FullName = extracted.GetPropertyOrDefaultString("fullName");
                    resultDto.ExtractedData.NationalIdNumber = NormalizeArabicIndicDigitsInText(extracted.GetPropertyOrDefaultString("nationalIdNumber"));
                    var expiryRaw = NormalizeArabicIndicDigitsInText(extracted.GetPropertyOrDefaultString("expiryDate"));
                    resultDto.ExtractedData.IsExpired = extracted.GetPropertyOrDefaultBool("isExpired");

                    // Try parse expiry date robustly with correction attempts
                    if (!string.IsNullOrWhiteSpace(expiryRaw))
                    {
                        if (TryParseEgyptianDateWithCorrections(expiryRaw, out DateTime expiryParsed, out string correctedIso, out string parseConfidence))
                        {
                            resultDto.ExtractedData.ExpiryDate = correctedIso; // YYYY-MM-DD
                            // determine isExpired based on parsed date
                            resultDto.ExtractedData.IsExpired = expiryParsed.Date < DateTime.Now.Date;
                        }
                        else
                        {
                            // Could not parse expiry — add warning/error
                            resultDto.Warnings ??= new List<string>();
                            resultDto.Warnings.Add("Unable to reliably parse expiry date from AI result.");
                        }
                    }
                }

                // Final logical consistency checks: if AI claimed valid but expiry says expired -> override
                if (resultDto.ExtractedData?.ExpiryDate != null)
                {
                    if (resultDto.ExtractedData.IsExpired)
                    {
                        resultDto.IsValid = false;
                        resultDto.Errors ??= new List<string>();
                        if (!resultDto.Errors.Contains("National ID card has expired"))
                            resultDto.Errors.Add("National ID card has expired");
                    }
                }

                return resultDto;
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "HTTP error calling OpenAI API");
                return new IdVerificationResponseDto
                {
                    IsValid = false,
                    Message = "Network error connecting to verification service.",
                    Errors = new List<string> { httpEx.Message }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying national ID");
                return new IdVerificationResponseDto
                {
                    IsValid = false,
                    Message = "Verification service error. Please try again later.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        private async Task<string> ConvertToBase64(IFormFile file)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            return Convert.ToBase64String(memoryStream.ToArray());
        }

        private string GetVerificationPrompt()
        {
            // **IMPORTANT**: force the model to output digits using ASCII digits 0-9 in JSON
            return @"You are an expert Egyptian National ID verification system with OCR capabilities.
You will receive two images (front and back). Analyze carefully and return ONLY valid JSON (no markdown).
**VERY IMPORTANT**: In the returned JSON, ALL numeric characters (dates, ID numbers, years) MUST use ASCII digits 0-9 (e.g., 2023 not ٢٠٢٣).
If you read Arabic-Indic numerals, convert them to ASCII digits before returning JSON.

Follow the same verification rules as before (date formats DD/MM/YYYY, expiry = issue +7 years, verify fields).
If uncertain about digits, include a 'warnings' array and provide confidence (high/medium/low) for dates.

Response format (JSON ONLY):
{
  ""isValid"": boolean,
  ""message"": ""Brief summary"",
  ""errors"": [],
  ""warnings"": [],
  ""extractedData"": {
    ""fullName"": ""string or null"",
    ""nationalIdNumber"": ""14-digit string or null (ASCII digits only)"",
    ""expiryDate"": ""YYYY-MM-DD or null (ASCII digits only)"",
    ""isExpired"": boolean
  }
}";
        }

        // ---------------------------
        // Helper functions
        // ---------------------------

        // Normalize Arabic-Indic digits (٠١٢٣٤٥٦٧٨٩) and Arabic Eastern forms to ASCII digits
        private static string NormalizeArabicIndicDigitsInText(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            var sb = new StringBuilder(input.Length);
            foreach (var ch in input)
            {
                sb.Append(ch switch
                {
                    '\u0660' => '0', // Arabic-Indic ٠
                    '\u0661' => '1',
                    '\u0662' => '2',
                    '\u0663' => '3',
                    '\u0664' => '4',
                    '\u0665' => '5',
                    '\u0666' => '6',
                    '\u0667' => '7',
                    '\u0668' => '8',
                    '\u0669' => '9',
                    '\u06F0' => '0', // Extended Arabic-Indic (Persian) ۰
                    '\u06F1' => '1',
                    '\u06F2' => '2',
                    '\u06F3' => '3',
                    '\u06F4' => '4',
                    '\u06F5' => '5',
                    '\u06F6' => '6',
                    '\u06F7' => '7',
                    '\u06F8' => '8',
                    '\u06F9' => '9',
                    var c => c
                });
            }
            return sb.ToString();
        }

        // Try parse an Egyptian date with robust correction attempts
        private static bool TryParseEgyptianDateWithCorrections(string rawDate, out DateTime parsedDate, out string isoDate, out string confidence)
        {
            parsedDate = default;
            isoDate = null;
            confidence = "low";

            if (string.IsNullOrWhiteSpace(rawDate))
                return false;

            // Clean common separators and whitespace
            var cleaned = rawDate.Trim();
            cleaned = cleaned.Replace('-', '/').Replace('.', '/').Replace('\\', '/');

            // If already ISO (YYYY-MM-DD), try parse first
            if (DateTime.TryParseExact(cleaned, new[] { "yyyy-MM-dd", "yyyy/MM/dd" }, null, System.Globalization.DateTimeStyles.None, out parsedDate))
            {
                isoDate = parsedDate.ToString("yyyy-MM-dd");
                confidence = "high";
                return true;
            }

            // Try DD/MM/YYYY or D/M/YYYY
            var formats = new[] { "dd/MM/yyyy", "d/M/yyyy", "d/MM/yyyy", "dd/M/yyyy" };
            if (DateTime.TryParseExact(cleaned, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out parsedDate))
            {
                isoDate = parsedDate.ToString("yyyy-MM-dd");
                confidence = "high";
                return true;
            }

            // If parsing failed, attempt digit-correction heuristics (common confusions)
            // 1) Try mapping 2<->3, 5<->8 and also common OCR flips
            var confusionPairs = new List<(char a, char b)> { ('2', '3'), ('3', '2'), ('5', '8'), ('8', '5') };

            // Normalize all to ASCII digits already expected by caller
            var digitsOnly = Regex.Replace(cleaned, @"[^\d/]", "");

            // Try replacing digits at each position with confusion candidates
            for (int pos = 0; pos < digitsOnly.Length; pos++)
            {
                foreach (var (a, b) in confusionPairs)
                {
                    var arr = digitsOnly.ToCharArray();
                    if (arr[pos] == a) arr[pos] = b;
                    var candidate = new string(arr);

                    if (DateTime.TryParseExact(candidate, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out parsedDate))
                    {
                        // Additional logical check: year plausible
                        var year = parsedDate.Year;
                        var now = DateTime.Now.Year;
                        if (year >= 1900 && year <= now + 20) // plausible range
                        {
                            isoDate = parsedDate.ToString("yyyy-MM-dd");
                            confidence = "medium";
                            return true;
                        }
                    }
                }
            }

            // As last resort, generate a few full-year-correction candidates (e.g., 2033 -> 2023)
            var yearMatch = Regex.Match(digitsOnly, @"(\d{1,2})/(\d{1,2})/(\d{4})");
            if (yearMatch.Success)
            {
                var dd = yearMatch.Groups[1].Value;
                var mm = yearMatch.Groups[2].Value;
                var yy = yearMatch.Groups[3].Value;

                // try common mistaken years: replace first occurrence of '3' with '2' in year if year obviously > now+10
                if (yy.Length == 4 && int.TryParse(yy, out int yval))
                {
                    if (yval > DateTime.Now.Year + 10)
                    {
                        var altYear = yy.Replace('3', '2'); // e.g., 2033 -> 2022 (careful)
                        var candidate = $"{dd}/{mm}/{altYear}";
                        if (DateTime.TryParseExact(candidate, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out parsedDate))
                        {
                            isoDate = parsedDate.ToString("yyyy-MM-dd");
                            confidence = "medium";
                            return true;
                        }
                    }
                }
            }

            // give up
            return false;
        }
    }

    // Extension helpers for safe JsonElement reading
    internal static class JsonElementExtensions
    {
        public static string GetPropertyOrDefaultString(this JsonElement element, string propName)
        {
            if (element.TryGetProperty(propName, out var prop) && prop.ValueKind != JsonValueKind.Null)
                return prop.GetString();
            return null;
        }

        public static bool GetPropertyOrDefaultBool(this JsonElement element, string propName)
        {
            if (element.TryGetProperty(propName, out var prop) && prop.ValueKind != JsonValueKind.Null)
            {
                if (prop.ValueKind == JsonValueKind.True) return true;
                if (prop.ValueKind == JsonValueKind.False) return false;
                if (prop.ValueKind == JsonValueKind.String && bool.TryParse(prop.GetString(), out var b)) return b;
            }
            return false;
        }

        public static List<string> GetPropertyOrDefaultStringArray(this JsonElement element, string propName)
        {
            var list = new List<string>();
            if (element.TryGetProperty(propName, out var prop) && prop.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in prop.EnumerateArray())
                {
                    if (item.ValueKind == JsonValueKind.String) list.Add(item.GetString());
                }
            }
            return list.Count > 0 ? list : null;
        }
    }
}
