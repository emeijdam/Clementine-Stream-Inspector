using LiquidTechnologies.FastInfoset;
using System.IO.Compression;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Windows.Forms;
using System;


namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            tabControl3.TabPages.Remove(tabPage4);
        }



        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void OpenClementineStream()
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\temp\\";
                openFileDialog.Filter = "Modeler stream files (*.str)|*.str";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    string ModelerStreamPath = openFileDialog.FileName;

                    using (var file = File.OpenRead(ModelerStreamPath))
                    
                    using (var zip = new ZipArchive(file, ZipArchiveMode.Read))
                    {
                        this.Text = this.Text + " " + file.Name;
                        foreach (var entry in zip.Entries)
                        { 

                            treeView1.Nodes.Add(entry.FullName);
                            // iff // then parent add if not exist

                            var fiEntry = new FileInfo(entry.FullName);

                            if (fiEntry.Extension == ".xml" & fiEntry.Name != "index.xml")
                            {
                                // MessageBox.Show(entry.FullName);

                                using (var stream = entry.Open())
                                {
                                    // do whatever we want with stream
                                    XmlDocument doc = new XmlDocument();
                                    doc.PreserveWhitespace = false;
                                    // var encodingCurrentFile = stream.CurrentEncoding.EncodingName;

                                    XmlReader fiReader = XmlReader.Create(new FIReader(stream), null);
                                    doc.Load(fiReader);
                                    fiReader.Close();

                                    XDocument xDoc = XDocument.Parse(doc.OuterXml);
                                  
                                    string xDocString = xDoc.ToString(SaveOptions.None);
                                    textBox1.Text = xDocString;
                                    tabControl3.TabPages.Add(tabPage4);

                                    RenderXMLFile(doc);
                                }
                            }
                            else
                            {
                                using (var stream = entry.Open())
                                {
                                    var osr = new StreamReader(stream, Encoding.Default);
                                    TabPage nepage = new TabPage();
                                    TextBox mlT = new TextBox();
                                    mlT.Multiline = true;
                                    mlT.Text = osr.ReadToEnd();
                                    nepage.Controls.Add(mlT);
                                    nepage.Text = entry.FullName;
                                    mlT.Dock = DockStyle.Fill;
                                    tabControl3.TabPages.Add(nepage);
                                }
                            }

                        }
                    }
                }
            }
        }


        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            treeView1.Nodes.Clear();
            tabControl3.TabPages.Remove(tabPage4);
            
            OpenClementineStream();
        }

        private void RenderXMLFile(XmlDocument dom)
        {
            try
            {
                // 1. Read XML File from a local path
               // string xmlString = File.ReadAllText(filepath, Encoding.UTF8);

                // 2. Create a XML DOM Document and load the data into it.
              //  XmlDocument dom = new XmlDocument();
               // dom.LoadXml(xmlString);

                // 3. Initialize the TreeView control. treeView1 can be created dinamically
                // and attached to the form or you can just drag and drop the widget from the toolbox
                // into the Form.

                // Clear any previous content of the widget
                treeView2.Nodes.Clear();
                // Create the root tree node, on any XML file the container (first root)
                // will be the DocumentElement name as any content must be wrapped in some node first.
                treeView2.Nodes.Add(new TreeNode(dom.DocumentElement.Name));

                // 4. Create an instance of the first node in the treeview (the one that contains the DocumentElement name)
                TreeNode tNode = new TreeNode();
                tNode = treeView2.Nodes[0];

                // 5. Populate the TreeView with the DOM nodes.
                this.AddNode(dom.DocumentElement, tNode);
            }
            catch (XmlException xmlEx)
            {
                MessageBox.Show(xmlEx.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void AddNode(XmlNode inXmlNode, TreeNode inTreeNode)
        {
            XmlNode xNode;
            TreeNode tNode;
            XmlNodeList nodeList;
            int i;

            // Loop through the XML nodes until the leaf is reached.
            // Add the nodes to the TreeView during the looping process.
            if (inXmlNode.HasChildNodes)
            {
                nodeList = inXmlNode.ChildNodes;

                for (i = 0; i <= nodeList.Count - 1; i++)
                {
                    xNode = inXmlNode.ChildNodes[i];

                  //  if (!(string.IsNullOrEmpty((xNode.OuterXml).Trim())))
                  //  {
                        inTreeNode.Nodes.Add(new TreeNode(xNode.Name));
                        tNode = inTreeNode.Nodes[i];
                        this.AddNode(xNode, tNode);
                   // }
                }
            }
            else
            {
                // Here you need to pull the data from the XmlNode based on the
                // type of node, whether attribute values are required, and so forth.
                if (!(string.IsNullOrEmpty((inXmlNode.OuterXml).Trim())))
                {
                    inTreeNode.Text = (inXmlNode.OuterXml).Trim();
                }
               
            }
        }
    }
}