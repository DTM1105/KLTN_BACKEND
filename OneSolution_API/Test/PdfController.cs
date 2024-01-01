using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using Newtonsoft.Json.Linq;
using OneSolution_API.Managers;
using OneSolution_API.Models.Utils;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;

public class InvoiceController : ApiController
{
    public class info
    {
        public string historyId { get; set; }
        public string historyCode { get; set; }
        public string bookingCode { get; set; }
        public string patientName { get; set; }
        public string ngaytao { get; set; }
        public string dtName { get; set; }
        public string chandoan { get; set; }
        public string loidan { get; set; }
        public string clinicName { get; set; }
    }
    [HttpGet]
    [Route("api/invoice/generate")]
    public IHttpActionResult GenerateInvoice(String historyId)
    {
        try
        {
            // Create a new PDF document
            Document document = new Document();

            // Add a section to the document
            Section section = document.AddSection();
            section.PageSetup.TopMargin = Unit.FromCentimeter(1); // Set to 1 centimeter, for example
            section.PageSetup.PageFormat = PageFormat.A4;
            section.PageSetup.LeftMargin = Unit.FromCentimeter(1); // Set to 2.5 centimeters, for example
            section.PageSetup.RightMargin = Unit.FromCentimeter(1); // Set to 2.5 centimeters, for example

            // Sample data - replace this with your actual invoice items
            List<InvoiceItem> items = new List<InvoiceItem>
            {
                new InvoiceItem { ItemNumber = 1, Description = "Widget A", Quantity = 2, UnitPrice = 50.00m },
                new InvoiceItem { ItemNumber = 2, Description = "Widget B", Quantity = 3, UnitPrice = 30.00m }
                // Add more items as needed
            };

            // Add invoice information and headers
            AddInvoiceInformation(section, historyId);

            // Add the table to the section

            // Save the PDF to a MemoryStream
            MemoryStream stream = new MemoryStream();
            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true, PdfFontEmbedding.Always);
            pdfRenderer.Document = document;
            pdfRenderer.RenderDocument();
            pdfRenderer.PdfDocument.Save(stream);

            // Return the PDF as a response
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(stream);
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("inline")
            {
                FileName = "Invoice.pdf"
            };

            return ResponseMessage(response);
        }
        catch (Exception ex)
        {
            var exceptionResponse = new ExceptionResponse
            {
                Message = "An error occurred while generating the invoice.",
                Details = ex.Message
            };

            var response = Request.CreateResponse(HttpStatusCode.InternalServerError, exceptionResponse);
            return ResponseMessage(response);
        }
    }

    private void AddInvoiceInformation(Section section,String historyId)
    {
        try
        {
            using (var db = new clsDB())
            {
                string query = $@"select h.pk_seq as historyId, h.code as historyCode, b.code as bookingCode
		                    , isnull(bki.patientName,u.name) as patientName, CAST(FORMAT(GETDATE(), 'HH:mm, \n\g\à\y dd \t\h\á\n\g MM \n\ă\m yyyy') AS NVARCHAR(100)) AS ngaytao
		                    ,dt.name as dtName,h.diagnostic as chandoan, h.advice as loidan
                            , cli.name as clinicName
                    from Ehistory h 
                    left join Ebooking b on h.bookingid=b.pk_seq
                    left join bkinfo bki on bki.bookingid=b.pk_seq
                    left join Esche s on s.pk_seq=b.scheduleId
                    left join Euser dt on b.doctorId=dt.pk_seq
                    left join Euser u on u.pk_seq=b.patientId
                    left join Ecli_info cli on cli.pk_seq=b.clinicId
                    where h.pk_seq='{historyId}'
                    ";
                DataTable dt = db.getDataTable(query);
                String account = XuLy.ParseDataRowToJson(dt.Rows[0]);
                info guest = JObject.Parse(account).ToObject<info>();
                var table = section.AddTable();
                table.Rows.Alignment = RowAlignment.Center;
                table.AddColumn("8cm");
                table.AddColumn("8cm");

                var row = table.AddRow();
                var paragraph = row.Cells[0].AddParagraph($@"{guest.clinicName}");

                paragraph.Format.ClearAll();
                // TabStop at column width minus inner margins and borders:
                row.Cells[1].AddParagraph($@"{guest.historyId} - {guest.historyCode}");
                row.Cells[1].Format.Alignment = ParagraphAlignment.Right;
                row.Cells[1].Format.Font.Size = 10;
                row.Cells[0].Format.Font.Size = 12;
                row.Cells[0].Format.Font.Bold = true;
                table.Borders.Width = 0;

                // Add invoice information
                Paragraph leftParagraph = section.AddParagraph();
                leftParagraph.AddLineBreak(); 
                leftParagraph.AddText($@"HÓA ĐƠN THUỐC");
                leftParagraph.Format.Font.Size = 15;
                leftParagraph.Format.Font.Bold = true;
                leftParagraph.AddLineBreak();
                leftParagraph.Format.Alignment = ParagraphAlignment.Center;

                Paragraph data = section.AddParagraph();
                data.AddLineBreak();
                data.AddText($@"{guest.ngaytao}");
                data.Format.Font.Size = 8;
                data.Format.Font.Italic = true;
                data.AddLineBreak();
                data.Format.Alignment = ParagraphAlignment.Center;
                // Add a tab to separate the left-aligned and right-aligned text


                Paragraph info = section.AddParagraph();
                info.Format.LeftIndent = Unit.FromCentimeter(2);
                info.AddLineBreak();
                info.AddText($@"Bác sĩ:");
                info.AddTab(); info.AddTab(); info.AddTab(); 
                info.AddText($"{guest.dtName}");
                info.Format.Font.Size = 10;
                info.AddLineBreak();
                info.AddLineBreak();
                info.Format.LeftIndent = Unit.FromCentimeter(1);
                info.Format.RightIndent = Unit.FromCentimeter(1); // Adjust right indent to simulate padding
                info.Format.SpaceBefore = Unit.FromPoint(2); // Adjust space before to simulate padding
                info.Format.SpaceAfter = Unit.FromPoint(2); // Adjust space after to simulate padding

                info.AddText($@"Tên bệnh nhân: ");
                info.AddTab(); info.AddTab();
                info.AddText($@"{guest.patientName}");

                info.AddLineBreak();
                info.AddLineBreak();
                info.Format.LeftIndent = Unit.FromCentimeter(1);
                info.Format.RightIndent = Unit.FromCentimeter(1); // Adjust right indent to simulate padding
                info.Format.SpaceBefore = Unit.FromPoint(2); // Adjust space before to simulate padding
                info.Format.SpaceAfter = Unit.FromPoint(2); // Adjust space after to simulate padding
                info.AddText($@"Chẩn đoán: ");
                info.AddTab(); info.AddTab();
                info.AddText($@"{guest.chandoan}");

                info.AddLineBreak();
                info.AddLineBreak();
                info.Format.LeftIndent = Unit.FromCentimeter(1);
                info.Format.RightIndent = Unit.FromCentimeter(1); // Adjust right indent to simulate padding
                info.Format.SpaceBefore = Unit.FromPoint(2); // Adjust space before to simulate padding
                info.Format.SpaceAfter = Unit.FromPoint(2); // Adjust space after to simulate padding
                info.AddText($@"Lời dặn: ");
                info.AddTab(); info.AddTab(); info.AddTab();
                info.AddText($@"{guest.loidan}");

                info.AddLineBreak();
                info.AddLineBreak();
                info.Format.LeftIndent = Unit.FromCentimeter(1);
                info.Format.RightIndent = Unit.FromCentimeter(1); // Adjust right indent to simulate padding
                info.Format.SpaceBefore = Unit.FromPoint(2); // Adjust space before to simulate padding
                info.Format.SpaceAfter = Unit.FromPoint(2); // Adjust space after to simulate padding
                info.AddText($@"Mã lịch hẹn: ");
                info.AddTab(); info.AddTab();
                info.AddText($@"{guest.bookingCode}");

                info.AddLineBreak();
                info.AddLineBreak();
                info.Format.LeftIndent = Unit.FromCentimeter(1);
                info.Format.RightIndent = Unit.FromCentimeter(1); // Adjust right indent to simulate padding
                info.Format.SpaceBefore = Unit.FromPoint(2); // Adjust space before to simulate padding
                info.Format.SpaceAfter = Unit.FromPoint(2); // Adjust space after to simulate padding
                info.AddText($@"Đơn thuốc: ");
                // Add additional headers

                query = $@"select b.code mathuoc,b.name as tenthuoc, isnull(a.amount,0) soluong, isnull(b.price,0) dongia , isnull(a.amount*b.price,0) as total
            from Ehistory_medicine a
            inner join Emedicine b on a.medicineId=b.pk_seq
            where a.historyId ='{historyId}'
        ";

                Paragraph paragraph1 = section.AddParagraph();
                paragraph1.Format.Alignment = ParagraphAlignment.Center;
                Table table1 = section.AddTable();
                table1.Rows.Alignment = RowAlignment.Center;
                DataTable dt1 = db.getDataTable(query);
                if (dt.Rows.Count > 0)
                {
                    table1.AddColumn(Unit.FromCentimeter(1));
                    table1.AddColumn(Unit.FromCentimeter(3));
                    table1.AddColumn(Unit.FromCentimeter(5));
                    table1.AddColumn(Unit.FromCentimeter(2));
                    table1.AddColumn(Unit.FromCentimeter(2));
                    table1.AddColumn(Unit.FromCentimeter(2));

                    Row headerRow = table1.AddRow();
                    headerRow.Cells[0].AddParagraph("STT");
                    headerRow.Cells[1].AddParagraph("Mã thuốc");
                    headerRow.Cells[2].AddParagraph("Tên thuốc");
                    headerRow.Cells[3].AddParagraph("Số lượng");
                    headerRow.Cells[4].AddParagraph("Đơn giá");
                    headerRow.Cells[5].AddParagraph("Thành tiền");
                }
                int i = 1;
                Double thanhtien = 0;
                foreach (DataRow row1 in dt1.Rows)
                {
                    Row dataRow = table1.AddRow();
                    dataRow.Cells[0].AddParagraph($"{i}");
                    dataRow.Cells[1].AddParagraph($"{row1["mathuoc"]}");
                    dataRow.Cells[2].AddParagraph(($"{row1["tenthuoc"]}"));
                    dataRow.Cells[3].AddParagraph($"{Double.Parse(row1["soluong"].ToString())}");
                    dataRow.Cells[4].AddParagraph($"{Double.Parse(row1["dongia"].ToString())}");
                    dataRow.Cells[5].AddParagraph($"{Double.Parse(row1["total"].ToString())}");
                    thanhtien += Double.Parse(row1["total"].ToString());
                    i++;
                }
                // Define a table with 5 columns

                // Format the table
                table1.Borders.Width = 0.5;
                Paragraph info1 = section.AddParagraph();
                info1.Format.LeftIndent = Unit.FromCentimeter(1);
                info1.AddLineBreak();
                info1.AddText($@"Thành tiền:");
                info1.AddTab(); info.AddTab(); info.AddTab();
                String tt = String.Format("{0:#,##0}", thanhtien);
                info1.AddText($@"{tt }  VNĐ");
                info1.Format.Font.Size = 10;

                Paragraph info2 = section.AddParagraph();
                info2.Format.LeftIndent = Unit.FromCentimeter(1);
                info2.AddLineBreak();
                info2.AddTab(); info2.AddTab(); info2.AddTab(); info2.AddTab(); info2.AddTab(); info2.AddTab(); info2.AddTab(); info2.AddTab(); info2.AddTab();
                info2.AddText($@"Người thanh toán");
                info2.Format.Font.Italic = true;
                info2.Format.Font.Bold = true;
            }
        }
        catch (Exception e)
        {
            
        }
        
    }



    // Example class for an invoice item
    public class InvoiceItem
    {
        public int ItemNumber { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total => Quantity * UnitPrice;
    }

    // Example class for exception response
    public class ExceptionResponse
    {
        public string Message { get; set; }
        public string Details { get; set; }
    }
    static string ConvertNumberToWords(long number)
    {
        if (number == 0)
            return "Không";

        string words = "";

        if ((number / 1000000000) > 0)
        {
            words += ConvertToWords(number / 1000000000) + " Tỷ ";
            number %= 1000000000;
        }

        if ((number / 1000000) > 0)
        {
            words += ConvertToWords(number / 1000000) + " Triệu ";
            number %= 1000000;
        }

        if ((number / 1000) > 0)
        {
            words += ConvertToWords(number / 1000) + " Nghìn ";
            number %= 1000;
        }

        if (number > 0)
        {
            if (words != "")
                words += "và ";

            words += ConvertToWords(number);
        }

        return words;
    }

    static string ConvertToWords(long num)
    {
        string[] unitsMap = { "", "Một", "Hai", "Ba", "Bốn", "Năm", "Sáu", "Bảy", "Tám", "Chín" };

        string words = "";

        if ((num / 100) > 0)
        {
            words += unitsMap[num / 100] + " Trăm ";
            num %= 100;
        }

        if (num > 0)
        {
            if (words != "")
                words += "và ";

            if (num < 10)
                words += unitsMap[num];
            else if (num < 20)
            {
                switch (num)
                {
                    case 10: words += "Mười"; break;
                    case 11: words += "Mười Một"; break;
                    case 12: words += "Mười Hai"; break;
                    case 13: words += "Mười Ba"; break;
                    case 14: words += "Mười Bốn"; break;
                    case 15: words += "Mười Lăm"; break;
                    case 16: words += "Mười Sáu"; break;
                    case 17: words += "Mười Bảy"; break;
                    case 18: words += "Mười Tám"; break;
                    case 19: words += "Mười Chín"; break;
                }
            }
            else
            {
                words += " " + unitsMap[num / 10] + " Mươi ";
                if ((num % 10) > 0)
                    words += unitsMap[num % 10];
            }
        }

        return words;
    }
}
