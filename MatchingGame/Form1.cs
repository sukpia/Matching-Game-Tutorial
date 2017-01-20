// This is practicing codes from Microsoft Tutorial:
// https://code.msdn.microsoft.com/windowsapps/Complete-Matching-Game-4cffddba
// Extra features added by Terri (1/20/2017):
// 1. Add second timer to tracks how long it takes for the player to win - Done
// 2. Replace icons and colors with ones you choose
// 3. Add sound when player finds a match, another sound when player uncovers two icons that don't match - Done
// 4. Make the game more difficult by making the board bigger
// 5. Make the game more challenging by hiding the first icon if the player is too slow to respond and doesn't choose the second icon before a certain amount of time.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;

namespace MatchingGame
{
    public partial class Form1 : Form
    {
        // Use this Random object to choose random icons for the squares
        Random random = new Random();

        // Each of these letters is an interesting icon in the Webdings font,
        // and each icon appears twice in this list
        List<string> icons = new List<string>()
        {
            "!", "!", "N", "N", ",", ",", "k", "k",
            "b", "b", "v", "v", "w", "w", "z", "z"
        };

        List<string> logo = new List<string>();

        // firstClicked points to the first label control
        // that the player clicks, but it will be null
        // if the player hasn't clicked a label yet
        Label firstClicked = null;

        // secondClicked points to the second Label control
        // that the player clicks
        Label secondClicked = null;

        // startTime variable, start time of the game, when the player clicks the first icon
        DateTime startTime;

        // Add sound to the game
        SoundPlayer soundNotMatch;
        SoundPlayer soundMatch;


        public Form1()
        {
            InitializeComponent();

            AssignIconsToSquares();
            // Initiate the soundPlayer
            // Since the wav file is in the Debug folder which is the program's startup location,
            // only include the wav filename.
            soundNotMatch = new SoundPlayer("chord.wav");
            soundMatch = new SoundPlayer("Alarm03.wav");
        }

        /// <summary>
        /// Assign each icon from the list of icons to a random square
        /// </summary>
        private void AssignIconsToSquares()
        {
            // The TableLayoutPanel has 16 labels,
            // and the icon list has 16 icons, so an icon is pulled at random from the list and added to each label
            foreach (Control control in tableLayoutPanel1.Controls)
            {
                Label iconLabel = control as Label;
                if (iconLabel != null)
                {
                    int randomNumber = random.Next(icons.Count);
                    iconLabel.Text = icons[randomNumber];
                    iconLabel.ForeColor = iconLabel.BackColor;
                    icons.RemoveAt(randomNumber);
                }                
            }
        }

        /// <summary>
        /// Every label's click event is handled by this event handler
        /// </summary>
        /// <param name="sender">The label that was clicked</param>
        /// <param name="e"></param>
        private void label_click(object sender, EventArgs e)
        {
            // The timer is only on after two non-matching
            // icons have been shown to the player,
            // so ignore any clicks if the timer is running
            if (timer1.Enabled == true)
                return;

            Label clickedLabel = sender as Label;
            if (clickedLabel != null)
            {
                // If clicked label is black, the player clicked
                // an icon that's already been revealed...
                // ignore the click
                if (clickedLabel.ForeColor == Color.Black)
                    return;

                // If firstClicked is null, this is the first icon
                // in the pair that the player clicked,
                // so set firstClicked to the label that the Player
                // clicked, change its color to black, and return
                if (firstClicked == null)
                {
                    if (timer2.Enabled == false)
                    {
                        startTime = DateTime.Now;
                        timer2.Start();
                    }

                    firstClicked = clickedLabel;
                    firstClicked.ForeColor = Color.Black;

                    return;
                }

                // If the Player gets this far, the timer isn't running
                // and firstClicked isn't null, so this must be the 
                // second icon the player clicked.
                // Set its color to black
                secondClicked = clickedLabel;
                secondClicked.ForeColor = Color.Black;

                // check to see if the player won
                CheckForWinnder();

                // If the player clicked two matching icons, keep them
                // black and reset firstClicked and secondClicked
                // so the player can click another icon
                if (firstClicked.Text == secondClicked.Text)
                {
                    soundMatch.Play();
                    firstClicked = null;
                    secondClicked = null;
                    return;
                }
                // If the player gets this far, the player clicked two
                // different icons, so start the timer (which will wait
                // three quarters of second, and then hide icons)
                timer1.Start();
                // Also play the Non-Match sound
                soundNotMatch.Play();
            }
        }

        /// <summary>
        /// This timer is started when the player clicks
        /// two icons that don't match,
        /// so it counts three quarters of a second
        /// and then turns itself off and hides both icons
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            // Stop the timer
            timer1.Stop();

            // Hide both icons
            firstClicked.ForeColor = firstClicked.BackColor;
            secondClicked.ForeColor = secondClicked.BackColor;

            // Reset firstClicked and secondClicked
            // so the next time a label is clicked,
            // the program knows it's the first click
            firstClicked = null;
            secondClicked = null;
        }

        /// <summary>
        /// Check every icon to see if it is matched, by
        /// comparing its foreground color to its background color.
        /// If all of the icons are matched, the player wins
        /// </summary>
        private void CheckForWinnder()
        {
            // Go through all of the labels in the TableLayoutPanel,
            // checking each one to see if its icon is matched.
            foreach(Control control in tableLayoutPanel1.Controls)
            {
                Label iconLabel = control as Label;
                if (iconLabel != null)
                {
                    if (iconLabel.ForeColor == iconLabel.BackColor)
                        return;
                }
            }

            // If the loop didn't return, it didn't find any unmatched icons
            // That means the user won. Show a message and close the form
            timer2.Stop();
            MessageBox.Show("You matched all the icons!", "Congratulations");
            Close(); // close the form
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            int hours = DateTime.Now.Hour - startTime.Hour;
            int minutes = DateTime.Now.Minute - startTime.Minute;
            int seconds = DateTime.Now.Second - startTime.Second;

            string timeSring = hours + ":" + minutes + ":" + seconds;

            lblTimer2.Text = timeSring;
        }
    }
}
