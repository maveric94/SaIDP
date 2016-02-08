using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace tsosii_lab1
{
    public unsafe class UnsafeBitmap
    {
        #region Public Variables
        Bitmap bitmap;
        BitmapData bitmapData = null;
        #endregion
        
        #region Constructor/Deconstructor
        public UnsafeBitmap(Bitmap bitmap)        
        {
            this.bitmap = bitmap;
        }    
         #endregion

        #region Properties

        public Bitmap Bitmap
        {
            get
            {
                return (bitmap);
            }
        }
        #endregion

        
        #region Lock/Unlock Bitmap

        public Byte* LockBitmap()
        {

            GraphicsUnit unit = GraphicsUnit.Pixel;
            RectangleF boundsF = bitmap.GetBounds(ref unit);
            Rectangle bounds = new Rectangle((int)boundsF.X, (int)boundsF.Y, (int)boundsF.Width, (int)boundsF.Height);

            lock (this.bitmap)
            {
                bitmapData = bitmap.LockBits(bounds, ImageLockMode.ReadWrite, bitmap.PixelFormat);
            }
            return (Byte*)bitmapData.Scan0.ToPointer();
        }


        public void UnlockBitmap()
        {
            lock (this.bitmap)
            {
                bitmap.UnlockBits(bitmapData);
            }
           
            bitmapData = null;
        }
        #endregion


    }
}