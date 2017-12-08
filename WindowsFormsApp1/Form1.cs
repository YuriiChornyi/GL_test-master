using System;
using System.IO;
using System.Windows.Forms;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        string path = "";
        BinaryFormatter formatter = new BinaryFormatter();
        public Form1()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        static long GetDirectorySize(string p)
        {
            string[] a = Directory.GetFiles(p, "*.*", SearchOption.AllDirectories);
            long b = 0;
            foreach (string name in a)
            {

                FileInfo info = new FileInfo(name);
                b += info.Length;
            }
            return b;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.RootFolder = Environment.SpecialFolder.MyComputer;
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                path = folderBrowserDialog1.SelectedPath;
                long directorySize = GetDirectorySize(path);
                if (directorySize >= 250100000)
                {
                    label1.Text = "Вибрана папка завелика";
                    MessageBox.Show("Увага", "Тільки папки з розміром <=240 MB");
                }
                else
                {
                    label1.Text = path.ToString() + "  Розмір:" + (directorySize / 1048576).ToString() + "MB";

                    button2.Enabled = true;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

            try
            {
                ZipFile.CreateFromDirectory(path, "result.zip", CompressionLevel.Optimal, true);
            }
            catch (Exception exception)
            {
                label1.Text = exception.Message.ToString();
                path = "";
                button2.Enabled = false;
                label1.Text += "\nPlease chose another folder";
                FileInfo arch = new FileInfo("result.zip");
                arch.Delete();
                return;
            }

            using (FileStream fs = new FileStream("data.bin", FileMode.OpenOrCreate))
            {

                try
                {
                    byte[] data = File.ReadAllBytes("result.zip");
                    formatter.Serialize(fs, data);
                    button3.Enabled = true;
                    MessageBox.Show("Serialized", "Successfully");
                }
                catch (Exception em)
                {
                    MessageBox.Show("Error", em.Message);
                    throw;
                }
                FileInfo arch = new FileInfo("result.zip");
                arch.Delete();

            }


        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                using (FileStream fs = new FileStream("data.bin", FileMode.Open))
                {
                    try
                    {
                        byte[] deser = (byte[])formatter.Deserialize(fs);
                        File.WriteAllBytes("new_result.zip", deser);

                        ZipFile.ExtractToDirectory("new_result.zip", folderBrowserDialog1.SelectedPath);
                        MessageBox.Show("Deserialized", "Successfully");
                        FileInfo arch = new FileInfo("new_result.zip");
                        arch.Delete();
                    }
                    catch (Exception em)
                    {
                        MessageBox.Show("Error", em.Message);
                        throw;
                    }


                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            FileInfo arch = new FileInfo("data.bin");
            arch.Delete();
        }
    }
}
