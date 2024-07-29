using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TagLib;
using Bunifu.UI.WinForms.Helpers.Transitions;
using NAudio.Wave;
using TagLib.Mpeg;
using NAudio.Utils;



namespace _4_Music_playeer_Framework
{

    public partial class Form1 : Form
    {

        bool show_lenght = true;
        int x = 30;
        int y = 13;
        int xTextBox = 225;
        int yTextBox = 572;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void btnPlayingNow_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0; // Выбирает первую вкладку
        }


        private void guna2Button2_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1;
        }


      
        private void Form1_Load(object sender, EventArgs e)
        {
            guna2TrackBar1.Location = new Point(x, y);
            string defaultPath = @"C:\FAST MONTAGE\my youtube\Footage\sound\Музыкадля блога";
            string path = string.IsNullOrEmpty(guna2TextBox1.Text) ? defaultPath : guna2TextBox1.Text;
            filesInDirectory(path);
            guna2TextBox1.Text = path;


        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            TimeSpan currentPos = outputDevice.GetPositionTimeSpan();
            string formattedTime = currentPos.ToString(@"mm\:ss");
            label6.Text = formattedTime;
        }

        async void hideLengthSide()
        {
            for (int i = 0; i < 20; i++)
            {
                y += 2;
                yTextBox += 2;
                guna2TextBox1.Location = new Point(xTextBox, yTextBox);
                guna2TrackBar1.Location = new Point(x, y);
                await Task.Delay(5);
            }
            show_lenght = false;
        }

        async void showLengthSide()
        {
            x = 29;
            y = 46;

            for (int i = 0; i < 10; i++)
            {
                y -= 3;
                yTextBox -= 4;
                guna2TextBox1.Location = new Point(xTextBox, yTextBox);
                guna2TrackBar1.Location = new Point(x, y);
                await Task.Delay(5);
            }
            show_lenght = true;
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            if (tabControl1.SelectedIndex == 1 && show_lenght == true)
            {
                hideLengthSide();
            }
            else
            {
                showLengthSide();
            }
        }


        void filesInDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                string[] musicFiles = Directory.GetFiles(path, "*.mp3")
                                      .Concat(Directory.GetFiles(path, "*.wav"))
                                      .Concat(Directory.GetFiles(path, "*.flac"))
                                      .Concat(Directory.GetFiles(path, "*.ogg"))
                                      .Concat(Directory.GetFiles(path, "*.aac"))
                                      .ToArray();
                foreach (string track in musicFiles)
                {
                    TagLib.File tagFile = TagLib.File.Create(track);
                    string fileName = Path.GetFileName(track);
                    string trackname = track;
                    string duration = tagFile.Properties.Duration.ToString("hh\\:mm\\:ss");
                    guna2DataGridView2.Rows.Add(fileName, trackname, duration);

                }
            }
            else
            {
                MessageBox.Show("Директория не найдена", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void guna2TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                guna2DataGridView2.Rows.Clear();
                filesInDirectory(guna2TextBox1.Text);
            }

        }

        public AudioFileReader audioFile;
        public WaveOutEvent outputDevice;

        public void guna2TrackBar1_Scroll(object sender, ScrollEventArgs e)
        {
            var currentPosition = outputDevice.GetPositionTimeSpan();
        }


        private void guna2DataGridView2_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var chooseTrack = guna2DataGridView2[0, e.RowIndex].Value.ToString();
            string filePath = Path.Combine(guna2TextBox1.Text, chooseTrack);
            try
            {
                playMusic(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($@"Ошибка воспроезвидения файла - {guna2TextBox1.Text}\{chooseTrack}\n{ex}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }


        private void playMusic(string trackPath)
        {
            outputDevice?.Stop();
            outputDevice?.Dispose();
            guna2TrackBar1.Value = 0;
            

            // Укажите путь к вашему аудиофайлу
            string filePath = trackPath;

            // Создаем AudioFileReader для чтения аудио файла
            audioFile = new AudioFileReader(filePath);

            // Создаем WaveOutEvent для воспроизведения аудио
            outputDevice = new WaveOutEvent();

            // Инициализируем WaveOutEvent с AudioFileReader
            outputDevice.Init(audioFile);

            // Начинаем воспроизведение
            outputDevice.Play();
        }

        void stopMusic()
        {
            if (outputDevice != null)
            {
                // Останавливаем воспроизведение
                outputDevice.Stop();
                outputDevice.Stop();

                // Освобождаем ресурсы
                outputDevice.Dispose();
                outputDevice = null;

            }
        }

        private void guna2TrackBar2_Scroll(object sender, ScrollEventArgs e)
        {
            audioFile.Volume = guna2TrackBar2.Value;
        }
    }
}
    
