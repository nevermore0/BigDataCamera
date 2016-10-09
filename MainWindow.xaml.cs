using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System.IO;



namespace MyFirstApp
{

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IFaceServiceClient faceServiceClient = new FaceServiceClient("ed28eb38236647eba3128a7f9310cff0");
        private int num;
        public MainWindow()
        {
            InitializeComponent();
        }


        private async void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            string add = "d:\\snap\\temp" + (char)(num + 48) + ".jpg";
            int[] age = new int[7];
            int[] gender = new int[2];
            string filePath = add;
            while (File.Exists(filePath))
            {
            filePath = "d:\\snap\\temp" + (char)(num + 48) + ".jpg";
            Uri fileUri = new Uri(filePath);
            num++;
            BitmapImage bitmapSource = new BitmapImage();

            bitmapSource.BeginInit();
            bitmapSource.CacheOption = BitmapCacheOption.None;
            bitmapSource.UriSource = fileUri;
            bitmapSource.EndInit();

            FacePhoto.Source = bitmapSource;


            Title = "Detecting...";
            FaceAttributes[] faceRects = await UploadAndDetectFaces(filePath);
            Title = String.Format("Detection Finished. {0} face(s) detected", faceRects.Length);

            if (faceRects.Length > 0)
            {
                for (int element = 0; element< faceRects.Length; element++ )
                {
                    if (faceRects[element].Age < 13) age[0] += 1;
                    else if (faceRects[element].Age < 18) age[1] += 1;
                        else if (faceRects[element].Age < 30) age[2] += 1;
                            else if (faceRects[element].Age < 40) age[3] += 1;
                                else if (faceRects[element].Age < 50) age[4] += 1;
                                    else if (faceRects[element].Age < 60) age[5] += 1;
                                        else age[6] += 1;
                    if (faceRects[element].Gender == "female") gender[0] += 1;
                    else gender[1] += 1;
                }
                DrawingVisual visual = new DrawingVisual();
                DrawingContext drawingContext = visual.RenderOpen();
                drawingContext.DrawImage(bitmapSource,
                    new Rect(0, 0, bitmapSource.Width, bitmapSource.Height));
                double dpi = bitmapSource.DpiX;
                double resizeFactor = 96 / dpi;

                /*foreach (var faceRect in faceRects)
                {
                    drawingContext.DrawRectangle(
                        Brushes.Transparent,
                        new Pen(Brushes.Red, 2),
                        new Rect(
                            faceRect.Left * resizeFactor,
                            faceRect.Top * resizeFactor,
                            faceRect.Width * resizeFactor,
                            faceRect.Height * resizeFactor
                            )
                    );
                }*/

                drawingContext.Close();
                RenderTargetBitmap faceWithRectBitmap = new RenderTargetBitmap(
                    (int)(bitmapSource.PixelWidth * resizeFactor),
                    (int)(bitmapSource.PixelHeight * resizeFactor),
                    96,
                    96,
                    PixelFormats.Pbgra32);

                faceWithRectBitmap.Render(visual);
                FacePhoto.Source = faceWithRectBitmap;
            }
                filePath = "d:\\snap\\temp" + (char)(num + 48) + ".jpg";
            }

            System.IO.StreamWriter afile = new System.IO.StreamWriter("c:\\Users\\ASUS\\Desktop\\BigDataCamera\\t1.txt");
            for (int element=0; element<7; element++)
            {
                afile.WriteLine(age[element]);
            } 
            afile.Close();
            System.IO.StreamWriter gfile = new System.IO.StreamWriter("c:\\Users\\ASUS\\Desktop\\BigDataCamera\\t2.txt");
            for (int element=0; element<2; element++)
            {
                gfile.WriteLine(gender[element]);
            }
            gfile.Close();

            
        }
        private async Task<FaceAttributes[]> UploadAndDetectFaces(string imageFilePath)
        {
            IEnumerable<FaceAttributeType> requestFaceAttributes =
                new List<FaceAttributeType>
            {
                FaceAttributeType.Age,
                FaceAttributeType.Gender
            };

            try
            {
                using (Stream imageFileStream = File.OpenRead(imageFilePath))
                {
                    var faces = await faceServiceClient.DetectAsync(imageFileStream, true, false , requestFaceAttributes);
                    var faceRects = faces.Select(face => face.FaceAttributes);

                    return faceRects.ToArray();
                }
            }
            catch (Exception)
            {
                return new FaceAttributes[0];
            }
        }
    }
}

