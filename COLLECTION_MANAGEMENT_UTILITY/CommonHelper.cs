using iText.Html2pdf;
using iText.Html2pdf.Resolver.Font;
using iText.Layout.Font;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COLLECTION_MANAGEMENT_UTILITY
{
    public static class CommonHelper
    {
        public static string FormatKey(string key)
        {
            return string.Join(" ", Enumerable.Range(0, key.Length / 4).Select(i => key.Substring(i * 4, 4)));
        }

        public static string GenerateQrCodeUri(string email, string unformattedKey)
        {
            var appName = "ITAXS";
            return $"otpauth://totp/{appName}:{email}?secret={unformattedKey}&issuer={appName}&digits=6";
        }

        public static string GenerateRandomDigits(int length)
        {
            Random random = new Random();
            return new string(Enumerable.Range(0, length).Select(_ => (char)('0' + random.Next(10))).ToArray());
        }

        public static string GenerateRandomAlphabets(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            Random random = new Random();
            return new string(Enumerable.Range(0, length).Select(_ => chars[random.Next(chars.Length)]).ToArray());
        }

        public static string GenerateRandomAlphanumeric(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            return new string(Enumerable.Range(0, length)
                .Select(_ => chars[random.Next(chars.Length)]).ToArray());
        }

        //public static string GeneratePDFFromHTML(string fileName, string fileSavePath, StringBuilder templatesBody)
        //{
        //    string result = string.Empty;
        //    using (FileStream pdfDest = File.Open(Path.Combine(fileSavePath, fileName), FileMode.Create))
        //    {
        //        ConverterProperties properties = new();

        //        // Updated font handling
        //        FontSet fontSet = new FontSet();
        //        fontSet.AddSystemFonts(); // Adds system fonts

        //        FontProvider fontProvider = new FontProvider(fontSet);
        //        properties.SetFontProvider(fontProvider);

        //        HtmlConverter.ConvertToPdf(templatesBody.ToString(), pdfDest, properties);
        //        result = Path.Combine(fileSavePath, fileName);
        //    }
        //    return result;
        //}

        public static string GeneratePDFFromHTML(string fileName, string fileSavePath, StringBuilder templatesBody)
        {
            string result = string.Empty;
            using (FileStream pdfDest = File.Open(Path.Combine(fileSavePath, fileName), FileMode.Create))
            {
                ConverterProperties properties = new();
                FontProvider fontProvider = new DefaultFontProvider(true, true, true);
                properties.SetFontProvider(fontProvider);
                HtmlConverter.ConvertToPdf(templatesBody.ToString(), pdfDest, properties);
                result = Path.Combine(fileSavePath, fileName);
            }
            return result;
        }
        public static string GeneratePDFFromHTMLWithCustomFont(string fileName, string fileSavePath, StringBuilder templatesBody, string web_root_path, string font_path)
        {
            string result = string.Empty;
            using (FileStream pdfDest = File.Open(Path.Combine(fileSavePath, fileName), FileMode.Create))
            {
                ConverterProperties properties = new();
                FontProvider fontProvider = new DefaultFontProvider();
                fontProvider.AddDirectory(string.Concat(web_root_path, font_path));
                properties.SetFontProvider(fontProvider);
                HtmlConverter.ConvertToPdf(templatesBody.ToString(), pdfDest, properties);
                result = Path.Combine(fileSavePath, fileName);
            }
            return result;
        }

        public static byte[] GeneratePDFFromHTMLWithCustomFontAsBytes(StringBuilder templatesBody)
        {
            using var memoryStream = new MemoryStream();

            ConverterProperties properties = new();
            FontProvider fontProvider = new DefaultFontProvider(true, true, true);
            properties.SetFontProvider(fontProvider);
            HtmlConverter.ConvertToPdf(templatesBody.ToString(), memoryStream, properties);
            return memoryStream.ToArray();
        }
        public static string ConvertDataTableToHTML(DataTable dt, bool is_header_required = false, int table_width_percentage = 100, int font_size = 12, string font_family = "helvetica")
        {
            //string html = "<table style='border:1px solid #b3adad;border-collapse:collapse;padding:5px;width:" + table_width_percentage + "%; font-family:" + font_family + ";font-size:" + font_size + "px'>";
            string html = "<table>";

            var amountColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                                {
                                    "quantity",
                                    "total_price"
                                };

            //add header row
            if (is_header_required == true)
            {
                html += "<tr>";
                for (int i = 0; i < dt.Columns.Count; i++)
                    html += "<td style='text-align:right'>" + dt.Columns[i].ColumnName + "</td>";
                html += "</tr>";
            }
            //add rows
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                html += "<tr>";
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    var colName = dt.Columns[j].ColumnName;
                    var cellValue = dt.Rows[i][j].ToString();
                    var amountClass = amountColumns.Contains(colName) ? " class='amount'" : "";
                    html += $"<td {amountClass}>" + dt.Rows[i][j].ToString() + "</td>";
                }
                    
                html += "</tr>";
            }
            html += "</table>";
            return html;
        }
        public static string ConvertDataTableToHTMLWithClass(DataTable dt, bool is_header_required = false, string table_class = "")
        {
            //string html = "<table style='border:1px solid #b3adad;border-collapse:collapse;padding:5px;width:" + table_width_percentage + "%; font-family:" + font_family + ";font-size:" + font_size + "px'>";
            string html = "<table class='" + table_class + "'>";

            //add header row
            if (is_header_required == true)
            {
                html += "<tr>";
                for (int i = 0; i < dt.Columns.Count; i++)
                    html += "<td'>" + dt.Columns[i].ColumnName + "</td>";
                html += "</tr>";
            }
            //add rows
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                html += "<tr>";
                for (int j = 0; j < dt.Columns.Count; j++)
                    html += "<td>" + dt.Rows[i][j].ToString() + "</td>";
                html += "</tr>";
            }
            html += "</table>";
            return html;
        }

        public static string GetImageBase64(string imagePath)
        {
            byte[] imageBytes = File.ReadAllBytes(imagePath);
            string base64String = Convert.ToBase64String(imageBytes);
            string ext = Path.GetExtension(imagePath).ToLower().Replace(".", "");
            return $"data:image/{ext};base64,{base64String}";
        }
    }
}
