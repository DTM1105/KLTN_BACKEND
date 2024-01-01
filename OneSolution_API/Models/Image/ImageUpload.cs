 
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace OneSolution_API.Models
{
    public class ImageUpload
    {
        // set default size here
        public int Width { get; set; }

        public int Height { get; set; }

        // folder for the upload, you can put this in the web.config
        //   private readonly string UploadPath = "~/Images/Items/";

        public ImageResult RenameUploadFile(HttpPostedFile file, string UploadPath, Int32 counter = 0)
        {
            var fileName = Path.GetFileName(file.FileName);

            string prepend = "item_";
            string finalFileName = prepend + ((counter).ToString()) + "_" + fileName;
            if (System.IO.File.Exists
                (HttpContext.Current.Request.MapPath(UploadPath + finalFileName)))
            {
                //file exists => add country try again
                return RenameUploadFile(file, UploadPath, ++counter);
            }
            //file doesn't exist, upload item but validate first
            return UploadFile_V2(file, finalFileName, UploadPath);
        }

        public static Bitmap SmartResize(string strImageFile, Size objMaxSize, ImageFormat enuType)
        {
            Bitmap objImage = null;
            try
            {
                objImage = new Bitmap(strImageFile);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (objImage.Width > objMaxSize.Width || objImage.Height > objMaxSize.Height)
            {
                Size objSize;
                int intWidthOverrun = 0;
                int intHeightOverrun = 0;
                if (objImage.Width > objMaxSize.Width)
                    intWidthOverrun = objImage.Width - objMaxSize.Width;
                if (objImage.Height > objMaxSize.Height)
                    intHeightOverrun = objImage.Height - objMaxSize.Height;

                double dblRatio;
                double dblWidthRatio = (double)objMaxSize.Width / (double)objImage.Width;
                double dblHeightRatio = (double)objMaxSize.Height / (double)objImage.Height;
                if (dblWidthRatio < dblHeightRatio)
                    dblRatio = dblWidthRatio;
                else
                    dblRatio = dblHeightRatio;
                objSize = new Size((int)((double)objImage.Width * dblRatio), (int)((double)objImage.Height * dblRatio));

                Bitmap objNewImage = Resize(objImage, objSize, enuType);

                objImage.Dispose();
                return objNewImage;
            }
            else
            {
                return objImage;
            }
        }
        public static Bitmap Resize(Bitmap imgPhoto, Size objSize, ImageFormat enuType)
        {
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;

            int destX = 0;
            int destY = 0;
            int destWidth = objSize.Width;
            int destHeight = objSize.Height;

            Bitmap bmPhoto;
            if (enuType == ImageFormat.Png)
                bmPhoto = new Bitmap(destWidth, destHeight, PixelFormat.Format32bppArgb);
            else if (enuType == ImageFormat.Gif)
                bmPhoto = new Bitmap(destWidth, destHeight); //PixelFormat.Format8bppIndexed should be the right value for a GIF, but will throw an error with some GIF images so it's not safe to specify.
            else
                bmPhoto = new Bitmap(destWidth, destHeight, PixelFormat.Format24bppRgb);

            //For some reason the resolution properties will be 96, even when the source image is different, so this matching does not appear to be reliable.
            //bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

            //If you want to override the default 96dpi resolution do it here
            //bmPhoto.SetResolution(72, 72);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            
            grPhoto.CompositingQuality = CompositingQuality.HighQuality;
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
            grPhoto.SmoothingMode = SmoothingMode.HighQuality;


            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            ImageCodecInfo imageCodecInfo =  GetEncoderInfo(ImageFormat.Jpeg);

            // Create an Encoder object for the Quality parameter.
            Encoder encoder = Encoder.Quality;

            // Create an EncoderParameters object. 
            EncoderParameters encoderParameters = new EncoderParameters(1);

            // Save the image as a JPEG file with quality level.
            EncoderParameter encoderParameter = new EncoderParameter(encoder, 50);
            encoderParameters.Param[0] = encoderParameter;


            grPhoto.Dispose();
            return bmPhoto;
        }
        public void Save(Bitmap image, int maxWidth, int maxHeight, int quality, string filePath)
        {
            // Get the image's original width and height
            int originalWidth = image.Width;
            int originalHeight = image.Height;

            // To preserve the aspect ratio
            float ratioX = (float)maxWidth / (float)originalWidth;
            float ratioY = (float)maxHeight / (float)originalHeight;
            float ratio = Math.Min(ratioX, ratioY);

            // New width and height based on aspect ratio
            int newWidth = (int)(originalWidth * ratio);
            int newHeight = (int)(originalHeight * ratio);

            // Convert other formats (including CMYK) to RGB.
            Bitmap newImage = new Bitmap(newWidth, newHeight, PixelFormat.Format24bppRgb);

            // Draws the image in the specified size with quality mode set to HighQuality
            using (Graphics graphics = Graphics.FromImage(newImage))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            // Get an ImageCodecInfo object that represents the JPEG codec.
            ImageCodecInfo imageCodecInfo =  GetEncoderInfo(ImageFormat.Jpeg);

            // Create an Encoder object for the Quality parameter.
            Encoder encoder = Encoder.Quality;

            // Create an EncoderParameters object. 
            EncoderParameters encoderParameters = new EncoderParameters(1);

            // Save the image as a JPEG file with quality level.
            EncoderParameter encoderParameter = new EncoderParameter(encoder, quality);
            encoderParameters.Param[0] = encoderParameter;
            newImage.Save(filePath, imageCodecInfo, encoderParameters);
        }

        private static ImageCodecInfo GetEncoderInfo(ImageFormat format)
        {
            return ImageCodecInfo.GetImageDecoders().SingleOrDefault(c => c.FormatID == format.Guid);
        }
        private ImageResult UploadFile_V1(HttpPostedFileBase file, string fileName, string UploadPath)
        {
            ImageResult imageResult = new ImageResult { Success = true, ErrorMessage = null };

            var path =
          Path.Combine(HttpContext.Current.Request.MapPath(UploadPath), fileName);
            string extension = Path.GetExtension(file.FileName);

            //make sure the file is valid
            if (!ValidateExtension(extension))
            {
                imageResult.Success = false;
                imageResult.ErrorMessage = "File định dạng không hợp lệ, vui lòng chỉ upload file:.jpg,.png,.gif,.jpeg ";
                return imageResult;
            }

            try
            {
                file.SaveAs(path);
 
               // String strImageFile = Server.MapPath("/Images/ImageFile.jpg");
                //System.Drawing.Bitmap objImage = new System.Drawing.Bitmap(path);
                //System.Drawing.Size objNewSize = new System.Drawing.Size(500, 600);
                //System.Drawing.Bitmap objNewImage = Resize(objImage, objNewSize, ImageFormat.Jpeg);
                //objNewImage.Save(path, ImageFormat.Jpeg);
                //objNewImage.Dispose();

                String strImageFile = path;
                System.Drawing.Size objMaxSize = new System.Drawing.Size(500, 600);
                System.Drawing.Bitmap objNewImage = SmartResize(strImageFile, objMaxSize, ImageFormat.Png);
                objNewImage.Save(path, ImageFormat.Png);
                objNewImage.Dispose();





                imageResult.ImageName = fileName;

                return imageResult;
            }
            catch (Exception ex)
            {
                // you might NOT want to show the exception error for the user
                // this is generally logging or testing

                imageResult.Success = false;
                imageResult.ErrorMessage = ex.Message;
                return imageResult;
            }
        }

        private ImageResult UploadFile(HttpPostedFileBase file, string fileName, string UploadPath)
        {
            ImageResult imageResult = new ImageResult { Success = true, ErrorMessage = null };

            var path =
          Path.Combine(HttpContext.Current.Request.MapPath(UploadPath), fileName);
            string extension = Path.GetExtension(file.FileName);

            //make sure the file is valid
            if (!ValidateExtension(extension))
            {
                imageResult.Success = false;
                imageResult.ErrorMessage = "File định dạng không hợp lệ, vui lòng chỉ upload file:.jpg,.png,.gif,.jpeg ";
                return imageResult;
            }

            try
            {
                file.SaveAs(path);
                
                Image imgOriginal = Image.FromFile(@path);
                //testhu(imgOriginal, path);

               // Image imgOriginal = Image.FromFile(@"C:\ASP\ShopLink\ShopLink\ShopLink\Areas\SHOPLINK\Content\Image\Product\item_0_20181104_141608.jpg");
                //pass in whatever value you want
                Image imgActual = Scale(imgOriginal);
                imgOriginal.Dispose();
                imgActual.Save(path);
                imgActual.Dispose();

                imageResult.ImageName = fileName;

                return imageResult;
            }
            catch (Exception ex)
            {
                // you might NOT want to show the exception error for the user
                // this is generally logging or testing

                imageResult.Success = false;
                imageResult.ErrorMessage = ex.Message;
                return imageResult;
            }
        }
        private ImageResult UploadFile_V2(HttpPostedFile file, string fileName, string UploadPath)
        {
            ImageResult imageResult = new ImageResult { Success = true, ErrorMessage = null };

            var path =
          Path.Combine(HttpContext.Current.Request.MapPath(UploadPath), fileName);
            string extension = Path.GetExtension(file.FileName);

            //make sure the file is valid
            if (!ValidateExtension(extension))
            {
                imageResult.Success = false;
                imageResult.ErrorMessage = "File định dạng không hợp lệ, vui lòng chỉ upload file:.jpg,.png,.gif,.jpeg ";
                return imageResult;
            }

            try
            {
                file.SaveAs(path);
                clsImageResize.Resize(path,960,640);
                 
                imageResult.ImageName = fileName;

                return imageResult;
            }
            catch (Exception ex)
            {
                // you might NOT want to show the exception error for the user
                // this is generally logging or testing

                imageResult.Success = false;
                imageResult.ErrorMessage = ex.Message;
                return imageResult;
            }
        }

        private void testhu(Image image, string path)
        {
            try
            {
                float aspectRatio = (float)image.Size.Width / (float)image.Size.Height;
                int newHeight = 200;
                int newWidth = Convert.ToInt32(aspectRatio * newHeight);
                System.Drawing.Bitmap thumbBitmap = new System.Drawing.Bitmap(newWidth, newHeight);
                System.Drawing.Graphics thumbGraph = System.Drawing.Graphics.FromImage(thumbBitmap);
                thumbGraph.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                thumbGraph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                thumbGraph.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                var imageRectangle = new Rectangle(0, 0, newWidth, newHeight);
                thumbGraph.DrawImage(image, imageRectangle);
                thumbBitmap.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);
                thumbGraph.Dispose();
                thumbBitmap.Dispose();
                image.Dispose();

            }
            catch(Exception er)
            {

            }
        }

        private bool ValidateExtension(string extension)
        {
            extension = extension.ToLower();
            switch (extension)
            {
                case ".jpg":
                    return true;
                case ".png":
                    return true;
                case ".gif":
                    return true;
                case ".jpeg":
                    return true;
                default:
                    return false;
            }
        }

        private Image Scale(Image imgPhoto)
        {

            float sourceWidth = 0;
            float sourceHeight = 0;
          
            sourceWidth=imgPhoto.Size.Width;
            sourceHeight = imgPhoto.Size.Height;
            

            float destHeight = 0;
            float destWidth = 0;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            // force resize, might distort image
            if (Width != 0 && Height != 0)
            {
                destWidth = Width;
                destHeight = Height;
            }
            // change size proportially depending on width or height
            else if (Height != 0)
            {
                destWidth = (float)(Height * sourceWidth) / sourceHeight;
                destHeight = Height;
            }
            else
            {
                destWidth = Width;
                destHeight = (float)(sourceHeight * Width / sourceWidth);
            }

            Bitmap bmPhoto = new Bitmap((int)destWidth, (int)destHeight 
                                       );
            //bmPhoto.SetResolution(72, 72);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            
           
            //grPhoto.DrawImage(imgPhoto,
            //    new Rectangle(destX, destY, (int)destWidth, (int)destHeight),
            //    new Rectangle(sourceX, sourceY, (int)sourceWidth, (int)sourceHeight),
            //    GraphicsUnit.Pixel);
            grPhoto.DrawImage(imgPhoto,0,0, (int)destWidth, (int)destHeight);
             

            grPhoto.Dispose();

            return bmPhoto;
        }
    }

}