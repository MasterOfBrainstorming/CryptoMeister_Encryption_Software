using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        //Few definitions in form
        //for file in use:
        private static string file;
        //booleans for desisions, default is false.
        private bool tdes,rsa,decrypt,aes,encrypt = false;


        public Form1()
        {
            InitializeComponent();
        }

        //Clicking Open file
        private void button2_Click(object sender, EventArgs e)
        {
            //function that lets us to search from explorer our file
            OpenFileDialog openFileDialog = new OpenFileDialog();
            //filter for txt files
            openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //selected location to string
                string filename = openFileDialog.FileName;
                //user sees the file selected
                textBox1.Text = filename;
                //read all lines from selected file and it goes to our global private string
                file = File.ReadAllText(filename);
            }
        }

        //RUN button
        private void button1_Click(object sender, EventArgs e)
        {
            //FOR DEBUGGING:
                //Debug.Write("AES: "+aes.ToString());
                //Debug.Write("\nTDES: "+ tdes.ToString());
                //Debug.Write("\nRSA: "+ rsa.ToString());
                //Debug.Write("\nENCR: "+ encrypt.ToString());
                //Debug.Write("\nDECR: "+ decrypt.ToString());

            //IF nothing is selected a message box will popup
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("No file selected!", "File ERROR",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // will send the file selected and booleans to program side to do encryption/decryption
            string msg = Program.RunAlgorithm(file, aes, tdes, rsa, encrypt, decrypt);
            //Debug.WriteLine(msg);

            // new stream for the file saving dialog
            Stream stream;
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            // filter for txt files or all files
            saveFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 2;
            saveFileDialog.RestoreDirectory = true;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                if ((stream = saveFileDialog.OpenFile()) != null)
                {//saving to the file name
                    using (StreamWriter outputFile = new StreamWriter(stream))
                    {
                        //saving
                        outputFile.Write(msg);
                        //information about the process being done
                        MessageBox.Show("DONE!", "File Saved",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    stream.Close();
                }
            }
        }

        //ENCRYPT
        // Only one checkbox can be selected at time and it changes the boolean values
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            encrypt = true;
            decrypt = false;
            if (checkBox1.Checked == true)
            {
                checkBox2.Checked = false;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        //DECRYPT
        // Only one checkbox can be selected at time and it changes the boolean values
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        { 
            decrypt = true;
            encrypt = false;
            if (checkBox2.Checked == true)
            {
                checkBox1.Checked = false;
            }

        }

        //AES
        //Rule for radial buttons, changes boolean on click
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb != null)
            {
                if (rb.Checked)
                {
                    aes = true;
                    tdes =false;
                    rsa = false;
                }
            }
        }

        //TDES
        //Rule for radial buttons, changes boolean on click
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb != null)
            {
                if (rb.Checked)
                {
                    tdes = true;
                    aes = false;
                    rsa = false;
                }
            }
        }

        //RSA
        //Rule for radial buttons, changes boolean on click
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if(rb != null)
            {
                if (rb.Checked)
                {
                    rsa = true;
                    tdes = false;
                    aes = false;
                }
            }
        }
    }
}
