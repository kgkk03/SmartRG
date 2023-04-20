using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace smartrg.Helpers
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public class PrintPDF
    {
        //Font Setting
        iTextSharp.text.Color ColorBlack = iTextSharp.text.Color.BLACK;
        iTextSharp.text.Color ColorBlue = iTextSharp.text.Color.BLUE;
        iTextSharp.text.Color ColorRed = iTextSharp.text.Color.RED;
        int Cell_ALIGN_CENTER = iTextSharp.text.Cell.ALIGN_CENTER;
        int Cell_ALIGN_LEFT = iTextSharp.text.Cell.ALIGN_LEFT;
        int Cell_ALIGN_RIGHT = iTextSharp.text.Cell.ALIGN_RIGHT;
        int Cell_ALIGN_MIDDLE = iTextSharp.text.Cell.ALIGN_MIDDLE;

        int FontNormal = iTextSharp.text.Font.NORMAL;
        int FontBold = iTextSharp.text.Font.BOLD;
        int FontItalic = iTextSharp.text.Font.ITALIC;

        int AlignRight = iTextSharp.text.Element.ALIGN_RIGHT;
        int AlignCenter = iTextSharp.text.Element.ALIGN_CENTER;
        int AlignLeft = iTextSharp.text.Element.ALIGN_LEFT;
        int ALIGNJUSTIFIED = iTextSharp.text.Element.ALIGN_JUSTIFIED;

        BaseFont PrintFont = BaseFont.CreateFont();
        iTextSharp.text.Font TitleFont = new iTextSharp.text.Font();
        iTextSharp.text.Font CompanyFont = new iTextSharp.text.Font();
        iTextSharp.text.Font CompanyDetailsFont = new iTextSharp.text.Font();
        iTextSharp.text.Font CaptionFont = new iTextSharp.text.Font();
        iTextSharp.text.Font DataFont = new iTextSharp.text.Font();
        iTextSharp.text.Font SpaceFont = new iTextSharp.text.Font();

        public PrintPDF() {
            SetParameter("THSarabun.ttf");
        }
        public async void SetParameter(string fontname)
        {
            //PrintFont = await GetFonts("THSarabun.ttf");
            PrintFont = await GetFonts(fontname);
            TitleFont = new iTextSharp.text.Font(PrintFont, 18.0f, FontBold, ColorBlack);
            CompanyFont = new iTextSharp.text.Font(PrintFont, 18.0f, FontBold, ColorBlue);
            CompanyDetailsFont = new iTextSharp.text.Font(PrintFont, 12.0f, FontNormal, ColorBlue);
            CaptionFont = new iTextSharp.text.Font(PrintFont, 14.0f, FontNormal, ColorBlack);
            DataFont = new iTextSharp.text.Font(PrintFont, 14.0f, FontItalic, ColorBlue);
            SpaceFont = new iTextSharp.text.Font(PrintFont, 14.0f, FontItalic, iTextSharp.text.Color.WHITE);
        }
        public void ExamplePdf()
        {
            string pdfPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "sample.pdf");
            System.IO.FileStream fs = new FileStream(pdfPath, FileMode.Create);
            Document document = new Document(PageSize.A4);
            document.AddCreationDate();
            document.AddAuthor("Software Maker");
            document.AddCreator("Borworn Thepsanga");
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            document.Open();

            
            AddNewItem(document, "รายการสั่งขาย", AlignCenter, TitleFont);


            //Addmore
            AddNewItem(document, "Order No:", AlignLeft, TitleFont);

            AddNewItem(document, "#717171", AlignLeft, TitleFont);

            AddLineSeparato(document, ColorRed);

            AddNewItem(document, "Order Date", AlignLeft, CaptionFont);
            AddNewItem(document, "3/8/2019", AlignLeft, DataFont);

            AddLineSeparato(document, ColorRed);

            AddNewItem(document, "Account Name", AlignLeft, CaptionFont);
            AddNewItem(document, "#Bworn Thepsanga", AlignLeft, DataFont);

            AddLineSeparato(document, ColorRed);

            //Add Product order detail
            AddLineSpace(document);
            AddNewItem(document, "Product Detail", AlignLeft, CaptionFont);
            AddLineSeparato(document, ColorRed);

            //Item 1
            AddNewItemWhithLeftAndRight(document, "Pizza 25", "(0.0%)", CaptionFont, DataFont);
            AddNewItemWhithLeftAndRight(document, "12.0*1000", "12000.0", CaptionFont, DataFont);
            AddLineSeparato(document, ColorRed);

            //Item 2
            AddNewItemWhithLeftAndRight(document, "Pizza 26", "(0.0%)", CaptionFont, DataFont);
            AddNewItemWhithLeftAndRight(document, "12.0*1000", "12000.0", CaptionFont, DataFont);
            AddLineSeparato(document, ColorRed);

            //Total
            AddLineSpace(document);
            AddLineSpace(document);

            AddNewItemWhithLeftAndRight(document, "Total", "24000.0", CaptionFont, DataFont);

            document.Close();
            writer.Close();
            fs.Close();
            DependencyService.Get<Helpers.ICallService>().PrintPDF(pdfPath, "MyPDF.pdf");
        }
        public void ExampleColumn()
        {
            string pdfpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Columns.pdf");
            Document doc = new Document();
            try

            {

                PdfWriter.GetInstance(doc, new FileStream(pdfpath, FileMode.Create));
                //PdfWriter.GetInstance(doc, new FileStream(pdfpath + "/Columns.pdf", FileMode.Create));
                doc.Open();
               
                Paragraph heading = new Paragraph("Page Heading", TitleFont);

                heading.SpacingAfter = 18f;

                doc.Add(heading);

                string text = @"Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Suspendisse blandit blandit turpis. Nam in lectus ut dolor consectetuer bibendum. Morbi neque ipsum, laoreet id; dignissim et, viverra id, mauris. Nulla mauris elit, consectetuer sit amet, accumsan eget, congue ac, libero. Vivamus suscipit. Nunc dignissim consectetuer lectus. Fusce elit nisi; commodo non, facilisis quis, hendrerit eu, dolor? Suspendisse eleifend nisi ut magna. Phasellus id lectus! Vivamus laoreet enim et dolor. Integer arcu mauris, ultricies vel, porta quis, venenatis at, libero. Donec nibh est, adipiscing et, ullamcorper vitae, placerat at, diam. Integer ac turpis vel ligula rutrum auctor! Morbi egestas erat sit amet diam. Ut ut ipsum? Aliquam non sem. Nulla risus eros, mollis quis, blandit ut; luctus eget, urna. Vestibulum vestibulum dapibus erat. Proin egestas leo a metus?";

                MultiColumnText columns = new MultiColumnText();

                columns.AddSimpleColumn(36f, 336f);

                columns.AddSimpleColumn(360f, doc.PageSize.Width - 36f);



                Paragraph para = new Paragraph(text, DataFont);

                para.SpacingAfter = 9f;
                para.Alignment = ALIGNJUSTIFIED;


                #region Create Table
                PdfPTable table = new PdfPTable(3);

                float[] widths = new float[] { 1f, 1f, 1f };

                table.TotalWidth = 300f;

                table.LockedWidth = true;

                table.SetWidths(widths);


                PdfPCell cell = new PdfPCell(new Phrase("Header spanning 3 columns"));

                cell.Colspan = 3;

                cell.HorizontalAlignment = 0;

                table.AddCell(cell);

                table.AddCell("Col 1 Row 1");

                table.AddCell("Col 2 Row 1");

                table.AddCell("Col 3 Row 1");

                table.AddCell("Col 1 Row 2");

                table.AddCell("Col 2 Row 2");

                table.AddCell("Col 3 Row 2");

                #endregion

                //iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(imagepath);
                iTextSharp.text.Image jpg = GetImage("printlogo.png");

                jpg.ScaleToFit(200f, 200f);

                jpg.SpacingAfter = 12f;

                jpg.SpacingBefore = 12f;



                columns.AddElement(para);

                columns.AddElement(table);

                columns.AddElement(jpg);

                columns.AddElement(para);

                columns.AddElement(para);

                columns.AddElement(para);

                columns.AddElement(para);

                doc.Add(columns);



            }

            catch (Exception ex)

            {

                var a = ex.Message;

            }

            finally

            {

                doc.Close();
                DependencyService.Get<Helpers.ICallService>().PrintPDF(pdfpath, "MyPDF.pdf");
            }
        }


        #region Image & Font
        async Task<BaseFont> GetFonts(string fontname)
        {
            try
            {
                string font = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), fontname);
                BaseFont result = BaseFont.CreateFont(font, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                return result;
            }
            catch(Exception ex)
            {
                var a = ex.Message;
                return await SetFonts(fontname);
            }
        }
        async Task<BaseFont> SetFonts(string fontname)
        {
            BaseFont result;
            try
            {
                if (await SaveFonts(fontname))
                {
                    string font = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), fontname);
                    result = BaseFont.CreateFont(font, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    return await Task.FromResult(result);
                }
            }
            catch { }
            return BaseFont.CreateFont();
        }
        async public Task<bool> SaveFonts(string fontname)
        {
            var filPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), fontname);
            if (!File.Exists(filPath))
            {
                try
                {
                    DependencyService.Get<Helpers.ICallService>().SaveAssetsFile(fontname);
                    return await Task.FromResult(true);
                }
                catch (Exception ex)
                {

                }
            }
            return await Task.FromResult(false);
        }
        public iTextSharp.text.Image GetImage(string imagename)
        {
            string imagepath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), imagename);
            try
            {
                return iTextSharp.text.Image.GetInstance(imagepath);
            } catch {
               
                return SetImage(imagepath);
            }
        }
        public iTextSharp.text.Image SetImage(string imagename)
        {
            string imagepath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), imagename);
            try
            {
                DependencyService.Get<Helpers.ICallService>().SaveAssetsFile(imagename);
                return iTextSharp.text.Image.GetInstance(imagepath);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Write PDF Data
        private void AddNewItemWhithLeftAndRight(Document document, string leftText, string rightText, iTextSharp.text.Font leftFont, iTextSharp.text.Font rightFont)
        {
            Chunk chunkLeft = new Chunk(leftText, leftFont);
            Chunk chuckRight = new Chunk(rightText, rightFont);
            Paragraph p = new Paragraph(chunkLeft);
            p.Add(new Chunk(new VerticalPositionMark()));
            p.Add(chuckRight);
            document.Add(p);
        }
        private void AddLineSeparato(Document document, iTextSharp.text.Color linecolor)
        {
            LineSeparator lineSeparator = new LineSeparator();
            lineSeparator.LineColor = linecolor;
            AddLineSpace(document);
            document.Add(new Chunk(lineSeparator));
            AddLineSpace(document);
        }
        private void AddLineSpace(Document document)
        {
            Chunk chunk = new Chunk("-", SpaceFont);
            Paragraph p = new Paragraph(chunk);
            document.Add(p);
        }
        private void AddNewItem(Document document, string text, int align, iTextSharp.text.Font font)
        {
            Chunk chunk = new Chunk(text, font);
            Paragraph p = new Paragraph(chunk);
            p.Alignment = align;
            document.Add(p);

        }

        #endregion

        #region  Print CashSale 
        public async Task<bool> PrintCashSale(Models.BillCompany company, Models.CashSaleData data, List<Models.CashSalelineData> details, List<Models.PaymentData> payments)
        {
            string filename = data.Key;
            string pdfPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "cashsale.pdf");
            FileStream fs = new FileStream(pdfPath, FileMode.Create);
            Document document = new Document(PageSize.A4);
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            try
            {
                document.AddCreationDate();
                document.AddAuthor("Software Maker");
                document.AddCreator("Borworn Thepsanga");
                document.Open();

                // Document Name
                AddNewItem(document, "ใบเสร็จรับเงิน", AlignCenter, TitleFont);
                AddNewItem(document, "", AlignCenter, CompanyDetailsFont);

                await PrintCompany(document,company);
                await PrintCashSaleHeader(document,data);
                await PrintCashSaleData(document, details);
                await PrintCashSalePayment(document, data, payments);
                

            }
            catch { }
            finally
            {
                document.Close();
                writer.Close();
                fs.Close();
            }
            DependencyService.Get<ICallService>().PrintPDF(pdfPath, filename);
            return await Task.FromResult(false);
        }
        async Task<bool> PrintCompany(Document doc, Models.BillCompany company)
        {
            try {

                #region Image

                iTextSharp.text.Image imgheader = GetImage("printlogo.png");
                imgheader.ScaleToFit(70f, 70f);
                imgheader.SpacingAfter = 5f;
                imgheader.SpacingBefore = 5f;

                #endregion


                #region Company Details

                PdfPTable table = new PdfPTable(2);
                float[] widths = new float[] { 1f, 6f };
                table.TotalWidth = doc.PageSize.Width-20;
                table.LockedWidth = true;
                table.SetWidths(widths);
                table.HorizontalAlignment = AlignCenter;

                PdfPCell imageCell = new PdfPCell(imgheader);
                imageCell.Rowspan = 3;
                imageCell.Border = 0;
                imageCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                imageCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                table.AddCell(imageCell);


                // Company Name
                PdfPCell Companycell = new PdfPCell(new Phrase(company.Compname, CompanyFont));
                Companycell.HorizontalAlignment = PdfCell.ALIGN_LEFT;
                Companycell.Border = 0;
                table.AddCell(Companycell);

                // Company Address
                PdfPCell Addresscell = new PdfPCell(new Phrase(company.Compaddress, CompanyDetailsFont));
                Addresscell.HorizontalAlignment = PdfCell.ALIGN_LEFT;
                Addresscell.Border = 0;
                table.AddCell(Addresscell);

                //// Company Address 2
                //PdfPCell Address2cell = new PdfPCell(new Phrase("ตำบล.....อำเภอ............จังหวัด................รหัสไปรณีย์.......", CaptionFont));
                //Address2cell.HorizontalAlignment = PdfCell.ALIGN_LEFT;
                //Address2cell.Border = 0;
                //table.AddCell(Address2cell);

                // Company Contract
                PdfPCell Contractcell = new PdfPCell(new Phrase(company.Taxid, CompanyDetailsFont));
                Contractcell.HorizontalAlignment = PdfCell.ALIGN_LEFT;
                Contractcell.Border = 0;
                table.AddCell(Contractcell);

                doc.Add(table);



                #endregion

                AddNewItem(doc, "", AlignCenter, CompanyDetailsFont);
                return await Task.FromResult(true);

            }
            catch { }
            return await Task.FromResult(false);
        }
        async Task<bool> PrintCashSaleHeader(Document doc, Models.CashSaleData data)
        {
            try
            {
                //Font Setting
               
                #region Customer Details

                PdfPTable table = new PdfPTable(5);
                float[] widths = new float[] { 1f,2f,3f,1f,5f };
                table.TotalWidth = doc.PageSize.Width - 20;
                table.LockedWidth = true;
                table.SetWidths(widths);
                table.HorizontalAlignment = AlignCenter;

                // Bill Date & Bill No.
                PdfPCell B1 = new PdfPCell(new Phrase("วันที่ :" , CaptionFont));
                B1.HorizontalAlignment = PdfCell.ALIGN_RIGHT;
                B1.Border = 0;
                table.AddCell(B1);

                PdfPCell B2 = new PdfPCell(new Phrase(Helpers.Controls.Date2ThaiString(data.Saledate, "dd-MMM-yyyy") , DataFont));
                B2.HorizontalAlignment = PdfCell.ALIGN_LEFT;
                B2.Border = 0;
                B2.Colspan = 2;
                table.AddCell(B2);

                PdfPCell B3 = new PdfPCell(new Phrase("เลขที่ :", CaptionFont));
                B3.HorizontalAlignment = PdfCell.ALIGN_RIGHT;
                B3.Border = 0;
                table.AddCell(B3);

                PdfPCell B4 = new PdfPCell(new Phrase(data.Key, DataFont));
                B4.HorizontalAlignment = PdfCell.ALIGN_LEFT;
                B4.Border = 0;
                table.AddCell(B4);


                // Customer Name
                PdfPCell C1 = new PdfPCell(new Phrase("ลูกค้า :", CaptionFont));
                C1.HorizontalAlignment = PdfCell.ALIGN_RIGHT;
                C1.Border = 0;
                table.AddCell(C1);
                PdfPCell C2 = new PdfPCell(new Phrase(data.Custname, DataFont));
                C2.HorizontalAlignment = PdfCell.ALIGN_LEFT;
                C2.Border = 0;
                C2.Colspan = 4;
                table.AddCell(C2);

                // Customer Address 1
                PdfPCell D1 = new PdfPCell(new Phrase("ที่อยู่ :", CaptionFont));
                D1.HorizontalAlignment = PdfCell.ALIGN_RIGHT;
                D1.Border = 0;
                table.AddCell(D1);
                PdfPCell D2 = new PdfPCell(new Phrase(data.Custaddress , DataFont));
                D2.HorizontalAlignment = PdfCell.ALIGN_LEFT;
                D2.VerticalAlignment = PdfCell.ALIGN_TOP;
                D2.NoWrap = false;
                D2.Border = 0;
                D2.Colspan = 4;
                D2.Rowspan = 2;
                table.AddCell(D2);


                // Tax ID
                PdfPCell E1 = new PdfPCell(new Phrase("เลขประจำตัวผู้เสียภาษี :", CaptionFont));
                E1.HorizontalAlignment = PdfCell.ALIGN_RIGHT;
                E1.Border = 0;
                E1.Colspan = 2;
                table.AddCell(E1);
                PdfPCell E2 = new PdfPCell(new Phrase(data.Custax, DataFont));
                E2.HorizontalAlignment = PdfCell.ALIGN_LEFT;
                E2.Border = 0;
                E2.Colspan = 3;
                table.AddCell(E2);
                doc.Add(table);

                AddLineSeparato(doc, ColorRed);



                #endregion

                return await Task.FromResult(true);

            }
            catch { }
            return await Task.FromResult(false);
        }
        async Task<bool> PrintCashSaleData(Document doc, List<Models.CashSalelineData> details)
        {
            try
            {

                #region Table Header
                PdfPTable table = new PdfPTable(4);
                float[] widths = new float[] { 8f, 1.2f, 1.4f, 1.4f };
                table.TotalWidth = doc.PageSize.Width - 40;
                table.LockedWidth = true;
                table.SetWidths(widths);
                table.HorizontalAlignment = AlignCenter;
               
                for (int i = 0; i<4; i++)
                {
                    string text = "";
                    if (i == 0) { text = "รายการขายสินค้า"; }
                    else if (i == 1) { text = "จำนวน\n(หน่วย)"; }
                    else if (i == 2) { text = "ราคา/หน่วย\n(บาท)"; }
                    else { text = "รวมเงิน\n(บาท)"; }
                    PdfPCell Header = new PdfPCell(new Phrase(text, CaptionFont));
                    Header.Rowspan = 2;
                    Header.HorizontalAlignment = PdfCell.ALIGN_CENTER;
                    Header.VerticalAlignment = PdfCell.ALIGN_CENTER;
                    table.AddCell(Header);
                }
                #endregion


                #region Table Data

                for (int i = 0; i < 20; i++)
                {
                    Models.CashSalelineData dr = null;
                    if (i< details.Count) { dr = details[i]; }
                    for (int j = 0; j < 4; j++)
                    {
                        string data = "";
                        string space = "     ";
                        int HAlign = PdfCell.ALIGN_RIGHT;
                        iTextSharp.text.Font df = DataFont;
                        if (dr != null)
                        {
                            if (j == 0)
                            {
                                data = space + dr.Item.ToString() + ")" + dr.Productname;
                                HAlign = PdfCell.ALIGN_LEFT;
                            }
                            else if (j == 1)
                            {
                                data = dr.Qty.ToString("#,##0");
                                HAlign = PdfCell.ALIGN_CENTER;
                            }
                            else if (j == 2) { data = dr.Price.ToString("#,##0.00") + space; }
                            else { data = dr.Amount.ToString("#,##0.00") + space; }

                        }
                        else { if (j == 3) { data = "-"; df = SpaceFont; } }
                        PdfPCell celldata = new PdfPCell(new Phrase(data, df));
                        celldata.HorizontalAlignment = HAlign;
                        celldata.VerticalAlignment = PdfCell.ALIGN_MIDDLE;
                        //celldata.Border = 0;
                        table.AddCell(celldata);
                    }
                }

                #endregion
                
                doc.Add(table);
                return await Task.FromResult(true);

            }
            catch { }
            return await Task.FromResult(false);
        }
        async Task<bool> PrintCashSalePayment(Document doc, Models.CashSaleData bill, List<Models.PaymentData> payment)
        {
            try
            {

                #region Table Header
                PdfPTable table = new PdfPTable(4);
                float[] widths = new float[] { 8f, 1.2f, 1.4f, 1.4f };
                table.TotalWidth = doc.PageSize.Width - 40;
                table.LockedWidth = true;
                table.SetWidths(widths);
                table.HorizontalAlignment = AlignCenter;

                #endregion

                #region Table Footer

                // รวมเงิน

                PdfPCell F1 = new PdfPCell(new Phrase(Helpers.Controls.BathText(bill.Amount) , CaptionFont));
                F1.HorizontalAlignment = PdfCell.ALIGN_CENTER;
                F1.VerticalAlignment = PdfCell.ALIGN_CENTER;
                F1.BackgroundColor = iTextSharp.text.Color.LIGHT_GRAY;
                F1.Rowspan = 3;
                table.AddCell(F1);
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        string data = "";
                        if (j == 0)
                        {
                            if (i == 0) { data = "รวมเงิน"; }
                            else if (i == 1) { data = "ภาษีมูลค่าเพิ่ม"; }
                            else { data = "รวมเป็นเงินทั้งสิ้น"; }
                            PdfPCell Footer = new PdfPCell(new Phrase(data, CaptionFont));
                            Footer.VerticalAlignment = PdfCell.ALIGN_CENTER;
                            Footer.HorizontalAlignment = PdfCell.ALIGN_RIGHT;
                            Footer.Colspan = 2;
                            table.AddCell(Footer);
                        }
                        else
                        {
                            data = "0.00 บาท  ";
                            if (i == 0) { data = bill.Amount.ToString("#,##0.00") ; }
                            else if (i == 1) { bill.Vat.ToString("#,##0.00"); }
                            else { data = bill.Total.ToString("#,##0.00"); }
                            PdfPCell Footer = new PdfPCell(new Phrase(data, DataFont));
                            Footer.VerticalAlignment = PdfCell.ALIGN_CENTER;
                            Footer.HorizontalAlignment = PdfCell.ALIGN_RIGHT;
                            table.AddCell(Footer);
                        }
                    }
                }

                PdfPCell spaceline = new PdfPCell(new Phrase("", CaptionFont));
                spaceline.Colspan = 4;
                table.AddCell(spaceline);

                // ชำระเงิน
                
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        // รายการชำระเงิน
                        string data = "";
                        if (j == 0)
                        {
                            if(payment!=null && i < payment.Count){ data = payment[i].Item.ToString() + ") ชำระเงินโดย: "+ payment[i].Paytypename + " จำนวนเงิน: " + payment[i].Total.ToString("#,##0.00") + " ธนาคาร: " + payment[i].Bank + "เลขที่อ้างอิง : " + payment[i].Ref1 ; }
                            PdfPCell Footer = new PdfPCell(new Phrase(data, DataFont));
                            Footer.HorizontalAlignment = PdfCell.ALIGN_LEFT;
                            Footer.VerticalAlignment = PdfCell.ALIGN_CENTER;
                            Footer.Border = 0;
                            table.AddCell(Footer);
                        }
                        else
                        {
                            // ผู้ซื้อ ผู้ขาย
                            iTextSharp.text.Font df = DataFont;
                            if (i == 0) { data = "ได้รับสินค้าไว้ครบตามจำนวนแล้ว"; }
                            else if (i == 1) { data = "-"; df = SpaceFont; }
                            else if (i == 2) { data = "..................................................ผู้รับสินค้า"; }
                            else { data = bill.Salename + "  ผู้ขาย"; }

                            PdfPCell Footer = new PdfPCell(new Phrase(data, df));
                            Footer.VerticalAlignment = PdfCell.ALIGN_CENTER;
                            Footer.HorizontalAlignment = PdfCell.ALIGN_RIGHT;
                            Footer.Border = 0;
                            Footer.Colspan = 3;
                            table.AddCell(Footer);
                        }
                    }
                }



                #endregion




                doc.Add(table);

                return await Task.FromResult(true);

            }
            catch { }
            return await Task.FromResult(false);
        }

        #endregion

        #region  Print Clearstock 
        public async Task<bool> PrintClearstock(Models.BillCompany company, Models.PickingData data, List<Models.PickingLineData> details)
        {
            string filename = data.Key;
            string pdfPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "clearstock.pdf");
            FileStream fs = new FileStream(pdfPath, FileMode.Create);
            Document document = new Document(PageSize.A4);
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            try
            {
                document.AddCreationDate();
                document.AddAuthor("Software Maker");
                document.AddCreator("Borworn Thepsanga");
                document.Open();

                // Group Data for print
                var listproduct = details.GroupBy(x => x.Productid)
                         .Select(group => new {
                             Productid = group.Key,
                             Pick = group.Where(x => x.Pickingtype.Equals("เบิก")).Sum(x => x.Qty),
                             Sale = group.Where(x => x.Pickingtype.Equals("ขาย")).Sum(x => x.Qty),
                             Return = group.Where(x => x.Pickingtype.Equals("คืน")).Sum(x => x.Qty),
                             Amount = group.Where(x => x.Pickingtype.Equals("ขาย")).Sum(x => x.Amount),
                             Items = group.ToList()
                         }).ToList();

                PrintPdfData ReturnData = new PrintPdfData()
                {
                    Header = new string[] { "รายการ", "เบิก", "ขาย", "คืน", "ยอดขาย" },
                    Colwidths = new float[] { 7f, 1.2f, 1.2f, 1.2f, 1.4f },
                    Colalign = new int[] { Cell_ALIGN_LEFT, Cell_ALIGN_RIGHT, Cell_ALIGN_RIGHT, Cell_ALIGN_RIGHT, Cell_ALIGN_RIGHT, Cell_ALIGN_RIGHT },
                    Items = new List<PdfData>(),
                    DocumentName ="ใบคืนสินค้า",
                    Totalline=20,
                };

                PrintPdfData DetailsData = new PrintPdfData()
                {
                    Header = new string[] { "รายการ", "เบิก", "ขาย", "คืน", "ยอดขาย" },
                    Colwidths = new float[] { 7f, 1.2f, 1.2f, 1.2f, 1.4f },
                    Colalign = new int[] { Cell_ALIGN_LEFT, Cell_ALIGN_RIGHT, Cell_ALIGN_RIGHT, Cell_ALIGN_RIGHT, Cell_ALIGN_RIGHT, Cell_ALIGN_RIGHT },
                    Items = new List<PdfData>(),
                    DocumentName = "รายละเอียดประกอบการคืนสินค้า",
                    Totalline = 25,
                };

                int lineid = 0;
                int linereturn = 0;
                foreach (var dr in listproduct)
                {
                    string space = "     ";
                    ReturnData.Items.Add(new PdfData()
                    {
                        Linenumber = linereturn,
                        Data = new string[] {
                            space + dr.Items[0].Productname + " (" + dr.Items[0].Productcode + ")",
                            dr.Pick.ToString("#,##0") + space,
                            dr.Sale.ToString("#,##0") + space,
                            dr.Return.ToString("#,##0") + space,
                            dr.Amount.ToString("#,##0.00") + space
                        }
                    });
                    linereturn += 1;
                    DetailsData.Items.Add(new PdfData()
                    {
                        Linenumber = lineid,
                        Data = new string[] {
                            space + dr.Items[0].Productname + " (" + dr.Items[0].Productcode + ")",
                            dr.Pick.ToString("#,##0") + space,
                            dr.Sale.ToString("#,##0") + space,
                            dr.Return.ToString("#,##0") + space,
                            dr.Amount.ToString("#,##0.00") + space
                        }
                    });
                    lineid += 1;
                    foreach (var rec in dr.Items)
                    {
                        string itemname = space + Helpers.Controls.Date2ThaiString(rec.Pickingdate, "dd-MMM") + "  " + rec.Pickingtype;
                        if (rec.Pickingtype.Equals("เบิก")) {
                            DetailsData.Items.Add(new PdfData()
                            {
                                Linenumber = lineid,
                                Data = new string[] { itemname, rec.Qty.ToString("#,##0") + space,"","","" }
                            });
                        }
                        else if (rec.Pickingtype.Equals("ขาย"))
                        {
                            DetailsData.Items.Add(new PdfData()
                            {
                                Linenumber = lineid,
                                Data = new string[] {itemname + " ( " + rec.Custname + " )", "",
                                    rec.Qty.ToString("#,##0") + space,"", dr.Amount.ToString("#,##0.00") + space
                                }
                            });
                        }
                        else {
                            DetailsData.Items.Add(new PdfData()
                            {
                                Linenumber = lineid,
                                Data = new string[] { itemname,"", "", rec.Qty.ToString("#,##0") + space, "" }
                            });
                        }
                        lineid += 1;
                    }
                }

                //========== พิมพ์ใบคืนสินค้า ===============
                do
                {
                    ReturnData.Page += 1;
                    await PrintReturn(document, company, data, ReturnData);
                } while (ReturnData.index< ReturnData.Items.Count);

                double Amount = details.Where(x => x.Pickingtype.Equals("ขาย")).Sum(x => x.Amount);
                // ใส่ท้ายตาราง
                await PrintReturnFooter(document, Amount, ReturnData);


                //========== พิมพ์รายละเอียดประกอบใบคืนสินค้า ===============
                do
                {
                    ReturnData.Page += 1;
                    await PrintReturn(document, company, data, DetailsData);
                } while (DetailsData.index < DetailsData.Items.Count);

            }
            catch { }
            finally
            {
                document.Close();
                writer.Close();
                fs.Close();
            }
            DependencyService.Get<ICallService>().PrintPDF(pdfPath, filename);
            return await Task.FromResult(false);
        }
        async Task<bool> PrintReturn(Document doc, Models.BillCompany company, Models.PickingData data, PrintPdfData details)
        {
            try
            {
                //Font Setting
                string docname = details.DocumentName + (details.Page == 1 ? "" : " ( แผ่นที่ " + details.Page.ToString()+")");
                AddNewItem(doc, docname, AlignCenter, TitleFont);
                AddNewItem(doc, "", AlignCenter, CompanyDetailsFont);
                await PrintCompany(doc, company);

                #region Header

                PdfPTable headertable = new PdfPTable(5);
                headertable.TotalWidth = doc.PageSize.Width - 20;
                headertable.LockedWidth = true;
                headertable.SetWidths(new float[] { 1f, 2f, 3f, 1f, 5f });
                headertable.HorizontalAlignment = AlignCenter;

                // Bill Date & Bill No.
                PdfPCell B1 = new PdfPCell(new Phrase("วันที่ :", CaptionFont));
                B1.HorizontalAlignment = PdfCell.ALIGN_RIGHT;
                B1.Border = 0;
                headertable.AddCell(B1);

                PdfPCell B2 = new PdfPCell(new Phrase(Helpers.Controls.Date2ThaiString(data.Pickingdate, "dd-MMM-yyyy"), DataFont));
                B2.HorizontalAlignment = PdfCell.ALIGN_LEFT;
                B2.Border = 0;
                B2.Colspan = 2;
                headertable.AddCell(B2);

                PdfPCell B3 = new PdfPCell(new Phrase("เลขที่ :", CaptionFont));
                B3.HorizontalAlignment = PdfCell.ALIGN_RIGHT;
                B3.Border = 0;
                headertable.AddCell(B3);

                PdfPCell B4 = new PdfPCell(new Phrase(data.Key, DataFont));
                B4.HorizontalAlignment = PdfCell.ALIGN_LEFT;
                B4.Border = 0;
                headertable.AddCell(B4);


                // Customer Name
                PdfPCell C1 = new PdfPCell(new Phrase("ร้านค้า :", CaptionFont));
                C1.HorizontalAlignment = PdfCell.ALIGN_RIGHT;
                C1.Border = 0;
                headertable.AddCell(C1);
                PdfPCell C2 = new PdfPCell(new Phrase(data.Agentname + " (" + data.Agentcode + ")", DataFont));
                C2.HorizontalAlignment = PdfCell.ALIGN_LEFT;
                C2.Border = 0;
                C2.Colspan = 4;
                headertable.AddCell(C2);

                doc.Add(headertable);
                AddLineSeparato(doc, ColorRed);


                #endregion

                #region Table Header
                PdfPTable table = new PdfPTable(details.Colwidths.Length);
                table.TotalWidth = doc.PageSize.Width - 40;
                table.LockedWidth = true;
                table.SetWidths(details.Colwidths);
                table.HorizontalAlignment = AlignCenter;

                for (int i = 0; i < 5; i++)
                {
                    PdfPCell Header = new PdfPCell(new Phrase(details.Header[i], CaptionFont));
                    Header.Rowspan = 2;
                    Header.HorizontalAlignment = PdfCell.ALIGN_CENTER;
                    Header.VerticalAlignment = PdfCell.ALIGN_CENTER;
                    table.AddCell(Header);
                }
                #endregion

                #region Data intable
                int line = 0;
                foreach (var dr in details.Items)
                {
                    for (int i = 0; i < details.Colwidths.Length; i++)
                    {
                        PdfPCell celldata = new PdfPCell(new Phrase(dr.Data[i], DataFont));
                        celldata.HorizontalAlignment = details.Colalign[i];
                        celldata.VerticalAlignment = PdfCell.ALIGN_MIDDLE;
                        //celldata.Border = 0;
                        table.AddCell(celldata);
                    }
                    details.index += 1;
                    line += 1;
                    if (line == details.Totalline) {
                        doc.Add(table);
                        // ขึ้นหน้าใหม่
                        AddNewItem(doc, "หน้า " + details.Page.ToString(), AlignRight, CaptionFont);
                        doc.NewPage();
                        return await Task.FromResult(true); 
                    }
                }

                // เติมบรรทัดให้เต็ม
                if (line < details.Totalline)
                {
                    for (int i = line; i < details.Totalline; i++)
                    {
                        for (int j = 0; j < details.Colwidths.Length; j++)
                        {
                            var strspace = "";
                            if (j == 4) { strspace = "-"; }
                            PdfPCell celldata = new PdfPCell(new Phrase(strspace, SpaceFont));
                            celldata.HorizontalAlignment = PdfCell.ALIGN_RIGHT;
                            celldata.VerticalAlignment = PdfCell.ALIGN_MIDDLE;
                            table.AddCell(celldata);
                        }
                    }
                }
                doc.Add(table);
                #endregion
                return await Task.FromResult(true);
            }
            catch { }
            return await Task.FromResult(false);
        }
        async Task<bool> PrintReturnFooter(Document doc,double Amount, PrintPdfData details)
        {
            #region Table Footer

            PdfPTable footertable = new PdfPTable(2);
            footertable.TotalWidth = doc.PageSize.Width - 20;
            footertable.LockedWidth = true;
            footertable.SetWidths(new float[] { 7f, 5f });
            footertable.HorizontalAlignment = AlignCenter;

            for (int i = 0; i < 3; i++)
            {
                string data = i == 1 ? Helpers.Controls.BathText(Amount) : "-";
                string data1 = i == 1 ? Amount.ToString("#,##0.00") + "      " : "-";
                PdfPCell F1 = new PdfPCell(new Phrase(data, CaptionFont));
                F1.HorizontalAlignment = PdfCell.ALIGN_CENTER;
                F1.VerticalAlignment = PdfCell.ALIGN_CENTER;
                F1.BackgroundColor = iTextSharp.text.Color.LIGHT_GRAY;
                F1.Border = 0;
                footertable.AddCell(F1);

                PdfPCell F2 = new PdfPCell(new Phrase(data1, CaptionFont));
                F2.HorizontalAlignment = PdfCell.ALIGN_RIGHT;
                F2.VerticalAlignment = PdfCell.ALIGN_CENTER;
                F2.BackgroundColor = iTextSharp.text.Color.LIGHT_GRAY;
                F2.Colspan = 4;
                F2.Border = 0;
                footertable.AddCell(F2);
            }

            #endregion

            doc.Add(footertable);

            AddLineSpace(doc);
            AddLineSpace(doc);
            AddNewItemWhithLeftAndRight(doc, "ผู้ส่งคืน.............................................................",
                                             "ผู้รับคืน.............................................................", CaptionFont, DataFont);
            AddLineSpace(doc);
            AddNewItemWhithLeftAndRight(doc, "................../......................../......................",
                                            "................../......................../......................", CaptionFont, DataFont);

            return await Task.FromResult(true);
        }

        #endregion
    }
    public class PrintPdfData
    {
        public string[] Header { get; set; } 
        public float[]  Colwidths { get; set; } 
        public int[] Colalign { get; set; } 
        public List<PdfData> Items { get; set; } = new List<PdfData>();

        public int Page = 0;

        public int index = 0;
        public string DocumentName { get; set; } = "ชื่อเอกสาร";

        public int Totalline = 20;

    }
    public class PdfData
    {
        public int Linenumber { get; set; } = 0;
        public string[] Data { get; set; } 
    }
}
