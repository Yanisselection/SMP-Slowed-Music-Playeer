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
using TagLib.Mpeg;
using NAudio.Wave;
using NAudio.Utils;
using System.Media;
using static System.Net.Mime.MediaTypeNames;

namespace _4_Music_playeer_Framework
{
    public partial class Form1 : Form
    {
        bool show_lenght = true; // Флаг для отображения длины
        int x = 30; // Начальная позиция X для трекбара
        int y = 13; // Начальная позиция Y для трекбара
        int xTextBox = 225; // Начальная позиция X для текстового поля
        int yTextBox = 572; // Начальная позиция Y для текстового поля

        public Form1()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Environment.Exit(0); // Закрывает приложение
        }

        private void btnPlayingNow_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0; // Выбирает первую вкладку
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1; // Выбирает вторую вкладку
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            guna2TrackBar1.Location = new Point(x, y); // Устанавливает начальную позицию трекбара
            string defaultPath = @"C:\FAST MONTAGE\my youtube\Footage\sound\Музыкадля блога"; // Путь по умолчанию
            string path = string.IsNullOrEmpty(guna2TextBox1.Text) ? defaultPath : guna2TextBox1.Text; // Выбирает путь
            filesInDirectory(path); // Загружает файлы из директории
            guna2TextBox1.Text = path; // Устанавливает путь в текстовое поле
            label3.Text = chooseTrack;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            TimeSpan currentPos = outputDevice.GetPositionTimeSpan(); // Получает текущую позицию воспроизведения
            string formattedTime = currentPos.ToString(@"mm\:ss"); // Форматирует время
            label6.Text = formattedTime; // Обновляет метку с текущим временем
        }

        // Спрятать прогресс бар трека
        async void hideLengthSide()
        {
            for (int i = 0; i < 20; i++)
            {
                y += 2;
                yTextBox += 2;
                guna2TextBox1.Location = new Point(xTextBox, yTextBox); // Обновляет позицию текстового поля
                guna2TrackBar1.Location = new Point(x, y); // Обновляет позицию трекбара
                await Task.Delay(5); // Задержка
            }
            show_lenght = false; // Скрывает длину
        }

        // Показать прогресс бар трека
        async void showLengthSide()
        {
            x = 29;
            y = 46;

            for (int i = 0; i < 10; i++)
            {
                y -= 3;
                yTextBox -= 4;
                guna2TextBox1.Location = new Point(xTextBox, yTextBox); // Обновляет позицию текстового поля
                guna2TrackBar1.Location = new Point(x, y); // Обновляет позицию трекбара
                await Task.Delay(5); // Задержка
            }
            show_lenght = true; // Показывает длину
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            if (tabControl1.SelectedIndex == 1 && show_lenght == true)
            {
                hideLengthSide(); // Скрывает длину при выборе второй вкладки
            }
            else
            {
                showLengthSide(); // Показывает длину при выборе первой вкладки
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
                                      .ToArray(); // Получает все музыкальные файлы в директории
                foreach (string track in musicFiles)
                {

                    TagLib.File tagFile = TagLib.File.Create(track); // Создает объект TagLib для файла
                    string fileName = Path.GetFileName(track); // Получает имя файла
                    string trackname = track; // Получает путь к файлу
                    string duration = tagFile.Properties.Duration.ToString("hh\\:mm\\:ss"); // Получает длительность трека
                    guna2DataGridView2.Rows.Add(fileName, duration); // Добавляет строку в DataGridView
                }
            }
            else
            {
                MessageBox.Show("Директория не найдена", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning); // Показывает сообщение об ошибке
            }
        }

        private void guna2TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                guna2DataGridView2.Rows.Clear(); // Очищает DataGridView
                filesInDirectory(guna2TextBox1.Text); // Загружает файлы из новой директории
            }
        }

        public AudioFileReader audioFile; // Объект для чтения аудиофайла
        public WaveOutEvent outputDevice; // Объект для воспроизведения аудио

        private bool isUserScrolling = false; // Флаг для отслеживания пользовательского скроллинга
        public void guna2TrackBar1_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.Type == ScrollEventType.ThumbTrack || e.Type == ScrollEventType.ThumbPosition)
            {
                isUserScrolling = true; // Устанавливает флаг пользовательского скроллинга
                if (audioFile != null && outputDevice != null && outputDevice.PlaybackState == PlaybackState.Playing)
                {
                    audioFile.CurrentTime = TimeSpan.FromSeconds(guna2TrackBar1.Value); // Устанавливает текущую позицию воспроизведения
                }
                isUserScrolling = false; // Сбрасывает флаг пользовательского скроллинга
            }
        }

        public string chooseTrack; // Выбранный трек
        private void guna2DataGridView2_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            chooseTrack = guna2DataGridView2[0, e.RowIndex].Value.ToString(); // Получает выбранный трек
            string filePath = Path.Combine(guna2TextBox1.Text, chooseTrack); // Получает путь к файлу
            try
            {
                outputDevice?.Stop(); // Останавливает текущее воспроизведение
                outputDevice?.Dispose(); // Освобождает ресурсы
                guna2TrackBar1.Value = 0; // Сбрасывает трекбар
                playMusic(filePath); // Воспроизводит новый трек
            }
            catch (Exception ex)
            {
                MessageBox.Show($@"Ошибка воспроезвидения файла - {guna2TextBox1.Text}\{chooseTrack}\n{ex}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); // Показывает сообщение об ошибке
            }
        }
        string PathPreviousTrack()
        {
            string currentPath = guna2DataGridView2.CurrentCell.Value.ToString();
            if (guna2DataGridView2.CurrentCell.RowIndex < guna2DataGridView2.Rows.Count - 1 && guna2DataGridView2.CurrentCell.RowIndex > 0)
            {

                int previousIndexCurrent = guna2DataGridView2.CurrentCell.RowIndex - 1;
                string chooseNextTrack = guna2DataGridView2[0, previousIndexCurrent].Value.ToString(); // Получает выбранный трек
                string chooseNextTrackPath = $"{guna2TextBox1.Text}\\{previousIndexCurrent}";
                guna2DataGridView2.CurrentCell = guna2DataGridView2[0, previousIndexCurrent];
                guna2DataGridView2.Rows[previousIndexCurrent].Selected = true;
                guna2DataGridView2.Rows[previousIndexCurrent + 1].Selected = false;
                return chooseNextTrackPath;


            }
            else
            {
                guna2DataGridView2.CurrentCell = guna2DataGridView2[0, 0];
                string firstTrackIndex = guna2DataGridView2[0, 0].Value.ToString();
                string firstTrackAllPath = $"{guna2TextBox1.Text}\\{firstTrackIndex}";
                return firstTrackAllPath;
            }

        }
        string pathNextTrack()
        {
            string currentPath = guna2DataGridView2.CurrentCell.Value.ToString();
            if (guna2DataGridView2.CurrentCell.RowIndex < guna2DataGridView2.Rows.Count - 1)
            {
                
                int nextIndexCurrent = guna2DataGridView2.CurrentCell.RowIndex + 1;
                string chooseNextTrack = guna2DataGridView2[0, nextIndexCurrent].Value.ToString(); // Получает выбранный трек
                string chooseNextTrackPath = $"{guna2TextBox1.Text}\\{chooseNextTrack}";
                guna2DataGridView2.CurrentCell = guna2DataGridView2[0, nextIndexCurrent];
                guna2DataGridView2.Rows[nextIndexCurrent].Selected = true;
                guna2DataGridView2.Rows[nextIndexCurrent-1].Selected = false;
                return chooseNextTrackPath;


            }
            else
            {
                guna2DataGridView2.CurrentCell = guna2DataGridView2[0, 0];
                string firstTrackIndex = guna2DataGridView2[0, 0].Value.ToString();
                string firstTrackAllPath = $"{guna2TextBox1.Text}\\{firstTrackIndex}";
                return firstTrackAllPath;
            }
        }


        private System.Windows.Forms.Timer timer; // Таймер для обновления трекбара

        bool playMusicNow = false;
        private void playMusic(string trackPath)
        {
            // Создаем AudioFileReader для чтения аудио файла
            audioFile = new AudioFileReader(trackPath);

            // Создаем WaveOutEvent для воспроизведения аудио
            outputDevice = new WaveOutEvent();

            // Инициализируем WaveOutEvent с AudioFileReader
            outputDevice.Init(audioFile);

            // Подписываемся на событие PlaybackStopped
            outputDevice.PlaybackStopped += OnPlaybackStopped;

            guna2TrackBar1.Maximum = (int)audioFile.TotalTime.TotalSeconds; // Устанавливает максимальное значение трекбара
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 100; // Интервал таймера
            timer.Tick += Slider_Tick; // Обработчик события таймера
           

            // Начинаем воспроизведение
            outputDevice.Play();
            timer.Start(); // Запускает таймер
            string nameTrack = Path.GetFileNameWithoutExtension(trackPath);
            label3.Text = nameTrack;


        }

        private void Slider_Tick(object sender, EventArgs e)
        {
            if (audioFile != null && outputDevice != null && outputDevice.PlaybackState == PlaybackState.Playing)
            {
                if (!isUserScrolling)
                {
                    guna2TrackBar1.Value = (int)audioFile.CurrentTime.TotalSeconds; // Обновляет значение трекбара
                }
                label6.Text = audioFile.CurrentTime.ToString(@"mm\:ss"); // Обновляет метку с текущим временем
                
                if ((int)audioFile.CurrentTime.TotalSeconds >= (int)audioFile.TotalTime.TotalSeconds)
                {
                    string nextPath = pathNextTrack();
                    playMusic(nextPath);
                    //MessageBox.Show(guna2TrackBar1.Value.ToString(), audioFile.CurrentTime.TotalSeconds.ToString());
                }
            }
        }

        private void OnPlaybackStopped(object sender, StoppedEventArgs e)
        {

            //outputDevice.Stop();
            //outputDevice.Dispose();
            //playMusic(pathNextTrack());
        }

        void stopMusic()
        {
            if (outputDevice != null)
            {
                // Останавливает воспроизведение
                outputDevice.Stop();

                // Освобождает ресурсы
                outputDevice.Dispose();
                outputDevice = null;
                playMusicNow = false;
            }
        }

        void pauseMusic()
        {
            if (outputDevice != null && outputDevice.PlaybackState == PlaybackState.Playing)
            {
                // Останавливает воспроизведение
                var pausedPosition = outputDevice.GetPositionTimeSpan();
                outputDevice.Pause();
            }
        }

        void resumeMusic()
        {
            if (outputDevice != null && outputDevice.PlaybackState == PlaybackState.Paused)
            {
                // Продолжает воспроизведение
                outputDevice.Play();
            }
        }

        private void guna2TrackBar2_Scroll(object sender, ScrollEventArgs e)
        {
            if (audioFile != null)
            {
                float volume = (float)guna2TrackBar2.Value / (float)guna2TrackBar2.Maximum; // Преобразует значение слайдера в диапазон от 0.0 до 1.0
                audioFile.Volume = volume; // Устанавливает громкость
            }
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
           
        }

        private void guna2ImageButton2_Click(object sender, EventArgs e)
        {
            if (audioFile == null && outputDevice == null)
            {
                string currentNextPath = $"{guna2TextBox1.Text}\\{guna2DataGridView2.CurrentCell.Value.ToString()}";
                label3.Text = Path.GetFileNameWithoutExtension(currentNextPath);
                playMusic(currentNextPath);
                playMusicNow = true;
                label4.Text = "Сейчас играет";
            }
            else
            {
                resumeMusic(); // Продолжает воспроизведение
            }
        }


        private void guna2TextBox1_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.Description = "Выберете папку с музыкой";
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    guna2TextBox1.Text = folderBrowserDialog.SelectedPath;
                    guna2DataGridView2.Rows.Clear(); // Очищает DataGridView
                    filesInDirectory(guna2TextBox1.Text); // Загружает файлы из новой директории
                }
            }
        }

        private void pnHeader_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2DataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnPaused_Click(object sender, EventArgs e)
        {
            pauseMusic(); // Останавливает воспроизведение
        }
        
        private void guna2ImageButton3_Click(object sender, EventArgs e)
        {
                if (playMusicNow == true)
            {
                stopMusic();
                string currentNextPath = pathNextTrack();
                playMusic(currentNextPath);
                string result = Path.GetFileNameWithoutExtension(currentNextPath);
                audioFile.Volume = guna2TrackBar2.Value / 100.0f;
                label3.Text = result;
                label4.Text = "Сейчас играет";
                playMusicNow = true;

            }


        }

        private void guna2DataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private bool isMove = false;
        private int mouseX, mouseY;



        private void pnHeader_MouseDown_1(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
            {
                isMove = true;
                mouseX = e.X;
                mouseY = e.Y;
            }
        }

        private void pnHeader_MouseMove_1(object sender, MouseEventArgs e)
        {
            {
                if (isMove)
                {
                    this.Left = e.Location.X + this.Left - mouseX;
                    this.Top = e.Location.Y + this.Top - mouseY;
                }
            }
        }

        private void guna2ImageButton1_Click(object sender, EventArgs e)
        {

            if (playMusicNow == true)
            {
                outputDevice.Stop();
                outputDevice.Dispose();
                playMusicNow = false;
                string previousTrack = PathPreviousTrack();
                //MessageBox.Show(previousTrack);
                playMusic(previousTrack);

            }

        }

        private void pnHeader_MouseUp_1(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isMove = false;
            }
        }


    }
}
    
