using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pexeso
{
    public partial class Form1 : Form
    {
        bool click = false;
        PictureBox firstGuess;
        Random rnd = new Random();
        Timer clickTimer = new Timer(); // timer po kliknuti (delay pro zmizeni nebo opetovne zakryti obrazku)
        int time = 60;
        Timer timer = new Timer { Interval = 1000 }; // hlavni timer

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }
        private PictureBox[] pictureBoxes
        {
            // prida vsechny pictureboxy do pole
            get { return Controls.OfType<PictureBox>().ToArray(); }
        }

        // snadny pristup k obrazkum pomoci IEnumerable
        private static IEnumerable<Image> images
        {
            get
            {
                return new Image[]
                {
                    Properties.Resources.img1,
                    Properties.Resources.img2,
                    Properties.Resources.img3,
                    Properties.Resources.img4,
                    Properties.Resources.img5,
                    Properties.Resources.img6,
                    Properties.Resources.img7,
                    Properties.Resources.img8,
                };
            }
        }

        // spusteni timeru
        private void startTimer()
        {
            timer.Start();
            timer.Tick += delegate
            {
                time--;
                if (time < 0)
                {
                    timer.Stop();
                    MessageBox.Show("Čas vypršel!");
                    ResetImages();
                }
                // vypis casu v labelu
                label2.Text = "00: " + time.ToString();
            };
        }

        // reset pictureboxu po dohrani hry
        private void ResetImages()
        {
            foreach(var pic in pictureBoxes)
            {
                pic.Tag = null;
                pic.Visible = true;
            }

            HideImages();
            setRandomImages();
            time = 60;
            timer.Start();
        }

        // zamaskovani obrazku
        private void HideImages()
        {
            foreach (var pic in pictureBoxes)
            {
                pic.Image = Properties.Resources.question;
            }
        }

        // zjisti, jestli picturebox je "null" nebo "EMPTY" a vrati danou hodnotu
        private PictureBox getFreeSlot()
        {
            int num;
            do
            {
                num = rnd.Next(0, pictureBoxes.Count());
            }
            while (pictureBoxes[num].Tag != null);
            return pictureBoxes[num];
        }

        // rozmsiteni paru obrazku se stejnym nazvem
        private void setRandomImages()
        {
            foreach (var image in images)
            {
                getFreeSlot().Tag = image;
                getFreeSlot().Tag = image;
            }
        }
        // znovuzakriti obrazku po odkryti
        private void clickTimerTick(object sender, EventArgs e)
        {
            HideImages();
            click = true;
            clickTimer.Stop();
        }

        // porovnavani obrazku
        private void clickImage(object sender, EventArgs e)
        {
            if (!click) return;
            var pic = (PictureBox)sender;

            if (firstGuess == null)
            {
                firstGuess = pic;
                pic.Image = (Image)pic.Tag;
                return;
            }

            pic.Image = (Image)pic.Tag;
            if (pic.Image == firstGuess.Image && pic != firstGuess)
            {
                pic.Visible = firstGuess.Visible = false;
                {
                    firstGuess = pic;
                }
                HideImages();
            }
            else
            {
                click = false;
                clickTimer.Start();
            }

            firstGuess = null;
            if (pictureBoxes.Any(p => p.Visible)) return;
            
            timer.Stop();
            
            DialogResult dialogResult = MessageBox.Show("Vyhrál jsi, chceš hrát znovu?","Výhra", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                ResetImages();
            }
            else if (dialogResult == DialogResult.No)
            {
                this.Close();
            }
           
            
        }

        // start hry po kliknuti na tlacitko start game
        private void startGame(object sender, EventArgs e)
        {
            click = true;
            setRandomImages();
            HideImages();
            startTimer();
            clickTimer.Interval = 1000;
            clickTimer.Tick += clickTimerTick;
            button1.Enabled = false;
        }

        // ukonceni hry
        private void endGame(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
