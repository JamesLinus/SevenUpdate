/*Copyright 2007-09 Robert Baker, aka Seven ALive.
This file is part of Seven Update.

    Seven Update is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Seven Update is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.*/
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;
namespace SevenUpdate.SDK
{
    public partial class Files : UserControl
    {
        #region Global Vars

        /// <summary>
        /// Gets the files of the update
        /// </summary>
        internal Collection<UpdateFile> UpdateFiles { get { return files; } }

        /// <summary>
        /// List of files
        /// </summary>
        Collection<UpdateFile> files { get; set; }

        /// <summary>
        /// Current index of file selected
        /// </summary>
        int index;

        /// <summary>
        /// The selected folder when adding files
        /// </summary>
        string selectedFolder;

        delegate void SetTextCallback(string hash);
        BackgroundWorker worker;

        #endregion

        public Files()
        {
            this.Font = System.Drawing.SystemFonts.MessageBoxFont;

            InitializeComponent();

            cbFileAction.SelectedIndex = 0;
        }

        #region Methods

        /// <summary>
        /// Adds a new file or group of files to the update
        /// </summary>
        private void AddFiles(string[] addedFiles)
        {
            Cursor = Cursors.WaitCursor;
            worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerAsync(addedFiles);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
        }

        /// <summary>
        /// Clears the UI
        /// </summary>
        /// <param name="list">indicates if to clear the collection of files</param>
        internal void ClearUI(bool clearFiles)
        {
            txtSize.Text = null;

            lblGetHash.Text = Program.RM.GetString("CalculateHashSize");

            txtArgs.Text = null;

            txtDownloadLoc.Text = null;

            txtFileLoc.Text = null;

            cbFileAction.SelectedIndex = 0;

            if (clearFiles)
                lbFiles.Items.Clear();

        }

        /// <summary>
        /// Deletes a file from the update
        /// </summary>
        private void DeleteFile()
        {
            int index = lbFiles.SelectedIndex;
            if (index < 0)
                return;

            if (files.Count > 0)
            {
                files.RemoveAt(lbFiles.SelectedIndex);

                lbFiles.Items.RemoveAt(lbFiles.SelectedIndex);
            }

            if (lbFiles.Items.Count > 0)
                if (index > 0)
                    lbFiles.SelectedIndex = index - 1;
                else
                    lbFiles.SelectedIndex = 0;
            else
            {
                ClearUI(false);
            }
            tmiAddFiles.Enabled = true;
            tmiAddFolder.Enabled = true;
        }

        /// <summary>
        /// Loads a file to the UI
        /// </summary>
        /// <param name="x">The file index</param>
        void LoadFile(int x)
        {
            if (x < 0)
                return;

            ClearUI(false);

            txtFileLoc.Text = files[x].Destination;

            txtDownloadLoc.Text = files[x].Source;

            txtArgs.Text = files[x].Arguments;
            if (files[x].Hash != null)
                lblGetHash.Text = files[x].Hash;

            switch (files[x].Action)
            {
                case FileAction.Update: cbFileAction.SelectedIndex = 0; break;
                case FileAction.UpdateAndRegister: cbFileAction.SelectedIndex = 1; break;
                case FileAction.UpdateAndExecute: cbFileAction.SelectedIndex = 2; break;
                case FileAction.UnregisterAndDelete: cbFileAction.SelectedIndex = 3; break;
                case FileAction.ExecuteAndDelete: cbFileAction.SelectedIndex = 4; break;
                case FileAction.Delete: cbFileAction.SelectedIndex = 5; break;
            }

            txtSize.Text = files[x].Size.ToString();
        }

        /// <summary>
        /// Loads the files into the UI
        /// </summary>
        internal void LoadFiles(Collection<UpdateFile> files)
        {
            this.files = new Collection<UpdateFile>();

            for (int x = 0; x < files.Count; x++)
            {
                this.files.Add(files[x]);
            }

            lbFiles.Items.Clear();

            if (this.files != null)
            {
                for (int x = 0; x < this.files.Count; x++)
                {
                    lbFiles.Items.Add(Path.GetFileName(this.files[x].Destination));
                }

                if (lbFiles.Items.Count > 0)
                {
                    lbFiles.SelectedIndex = 0;

                    index = lbFiles.SelectedIndex;

                    LoadFile(index);
                }
            }
        }

        /// <summary>
        /// Saves a file to the list
        /// </summary>
        void SaveFile()
        {
            UpdateFile file = new UpdateFile();

            switch (cbFileAction.SelectedIndex)
            {
                case 0: file.Action = FileAction.Update; break;
                case 1: file.Action = FileAction.UpdateAndRegister; break;
                case 2: file.Action = FileAction.UpdateAndExecute; break;
                case 3: file.Action = FileAction.UnregisterAndDelete; break;
                case 4: file.Action = FileAction.ExecuteAndDelete; break;
                case 5: file.Action = FileAction.Delete; break;
            }
            txtFileLoc.Text = Shared.ConvertPath(txtFileLoc.Text, false, Main.Is64Bit);
            txtFileLoc.Text = Shared.Replace(txtFileLoc.Text, AppInfo.ApplicationDirectory, "[AppDir]");

            txtDownloadLoc.Text = Shared.ConvertPath(txtDownloadLoc.Text, false, Main.Is64Bit);
            txtDownloadLoc.Text = Shared.Replace(txtDownloadLoc.Text, UpdateInfo.DownloadDirectory, "[DownloadDir]");
            
            if (txtFileLoc.Text.Length > 8)
                if (txtFileLoc.Text.Substring(0, 8).ToLower() == "[appdir]")
                    if (txtFileLoc.Text.Substring(8, 1) != "\\")
                        txtFileLoc.Text = txtFileLoc.Text.Insert(8, "\\");

            file.Arguments = txtArgs.Text;

            file.Destination = txtFileLoc.Text;

            file.Source = txtDownloadLoc.Text;

            if (lblGetHash.Enabled == true)
                file.Hash = lblGetHash.Text;
            else
                file.Hash = null;

            file.Size = Convert.ToUInt64(txtSize.Text);

            if (index > -1 && files.Count > 0)
            {
                files.RemoveAt(index);

                files.Insert(index,file);
                lbFiles.Items[index] = Path.GetFileName(file.Destination);
                lbFiles.SelectedIndex = index;
            }
            else
            {
                files.Add(file);
                lbFiles.Items.Add(Path.GetFileName(file.Destination));
                lbFiles.SelectedIndex = lbFiles.Items.Count - 1;

            }

        }

        private void SetHash(string hash)
        {
            if (this.lblGetHash.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetHash);
                this.Invoke(d, new object[] { hash });
            }
            else
            {
                this.lblGetHash.Text = hash;
            }

        }

        #endregion

        #region UI Events

        #region Button

        void btnSave_Click(object sender, EventArgs e)
        {
            if (lblGetHash.Text != Program.RM.GetString("CalculateHashSize") || cbFileAction.SelectedIndex == 3 || cbFileAction.SelectedIndex == 5)
                if (txtSize.Text.Length > 0)
                    SaveFile();
                else
                    MessageBox.Show(Program.RM.GetString("EnterFileSize"));
            else
                MessageBox.Show(Program.RM.GetString("CalculateHash"));
        }

        #endregion

        #region Context Menu Items

        private void tmiDeleteFile_Click(object sender, EventArgs e)
        {
            DeleteFile();
        }

        private void tmiAddFiles_Click(object sender, EventArgs e)
        {

            dlgOpenFile.Multiselect = true;
            dlgOpenFile.FileName = null;
            if (dlgOpenFile.ShowDialog() == DialogResult.OK)
            {
                selectedFolder = Path.GetDirectoryName(dlgOpenFile.FileName);
                ClearUI(false);
                AddFiles(dlgOpenFile.FileNames);
            }
        }

        private void tmiAddFolder_Click(object sender, EventArgs e)
        {
            if (dlgOpenFolder.ShowDialog() == DialogResult.OK)
            {
                selectedFolder = dlgOpenFolder.SelectedPath;
                ClearUI(false);
                AddFiles(Directory.GetFiles(dlgOpenFolder.SelectedPath, "*", SearchOption.AllDirectories));
            }
        }

        #endregion

        #region Labels

        void lblBrowse_Click(object sender, EventArgs e)
        {
            dlgOpenFile.Multiselect = false;
            dlgOpenFile.FileName = null;
            if (dlgOpenFile.ShowDialog() == DialogResult.OK)
            {
                txtFileLoc.Text = dlgOpenFile.FileName;
                worker = new BackgroundWorker();
                worker.RunWorkerCompleted+=new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted2);
                worker.DoWork+=new DoWorkEventHandler(worker_DoWork2);
                Cursor = Cursors.WaitCursor;
                worker.RunWorkerAsync(dlgOpenFile.FileName);
                
                txtSize.Text = new System.IO.FileInfo(txtFileLoc.Text).Length.ToString();
                SaveFile();
            }
           
        }
        
        void lblGetHash_Click(object sender, EventArgs e)
        {
            dlgOpenFile.FileName = null;

            string dir = Shared.ConvertPath(AppInfo.ApplicationDirectory, true, Main.Is64Bit) + @"\";

            if (!Directory.Exists(dir))
                dir = null;

            dlgOpenFile.FileName = dir + Path.GetFileName(txtFileLoc.Text);

            if (dlgOpenFile.ShowDialog() == DialogResult.OK)
            {
                selectedFolder = Path.GetDirectoryName(dlgOpenFile.FileName);
                worker = new BackgroundWorker();
                worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted2);
                worker.DoWork += new DoWorkEventHandler(worker_DoWork2);
                Cursor = Cursors.WaitCursor;
                worker.RunWorkerAsync(dlgOpenFile.FileName);

                System.IO.FileInfo fi = new System.IO.FileInfo(dlgOpenFile.FileName);

                txtSize.Text = fi.Length.ToString();
            }
        }

        void lbFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            index = lbFiles.SelectedIndex;

            if (index >=0)
                LoadFile(index);
        }

        void Label_MouseEnter(object sender, System.EventArgs e)
        {
            Label label = ((Label)sender);
            label.ForeColor = Color.FromArgb(51, 153, 255);
            label.Font = new Font(label.Font, FontStyle.Underline);
        }

        void Label_MouseLeave(object sender, System.EventArgs e)
        {
            Label label = ((Label)sender);
            label.ForeColor = System.Drawing.Color.FromArgb(0, 102, 204);
            label.Font = new Font(label.Font, FontStyle.Regular);
        }

        #endregion

        private void cbFileAction_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbFileAction.SelectedIndex == 3 || cbFileAction.SelectedIndex == 5)
            {
                lblGetHash.Enabled = false;
            }
            else
                lblGetHash.Enabled = true;

        }

        #region lbFiles

        private void lbFiles_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int index = lbFiles.IndexFromPoint(e.X, e.Y);
                if (index > -1)
                {
                    lbFiles.SelectedIndex = index;
                }

                if (lbFiles.SelectedIndex < 0)
                    tmiDeleteFile.Enabled = false;
                else
                    tmiDeleteFile.Enabled = true;
            
                cmsMenu.Show(Control.MousePosition);
                
            }

        }

        private void lbFiles_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Delete)
                DeleteFile();
        }

        #endregion

        #endregion

        #region Worker Events

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Cursor = Cursors.Arrow;
            lbFiles.Items.Clear();
            for (int x = 0; x < files.Count; x++)
            {
                lbFiles.Items.Add(Path.GetFileName(files[x].Destination));
            }
            lbFiles.SelectedIndex = lbFiles.Items.Count - 1;
            worker.DoWork -= worker_DoWork;
            worker.RunWorkerCompleted -= worker_RunWorkerCompleted;
        }

        void worker_RunWorkerCompleted2(object sender, RunWorkerCompletedEventArgs e)
        {
            Cursor = Cursors.Arrow;
            worker.DoWork -= worker_DoWork2;
            worker.RunWorkerCompleted -= worker_RunWorkerCompleted2;
        }

        void worker_DoWork2(object sender, DoWorkEventArgs e)
        {
            string file = ((string)e.Argument);
            SetHash(Shared.GetHash(file));
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            string[] addedFiles = ((string[])e.Argument);
            if (files == null)
                files = new Collection<UpdateFile>();

            string dir = Shared.ConvertPath(AppInfo.ApplicationDirectory, true, Main.Is64Bit) + @"\";

            if (!Directory.Exists(dir))
                dir = null;

            for (int x = 0; x < addedFiles.Length; x++)
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(addedFiles[x]);

                UpdateFile file = new UpdateFile();

                file.Hash = Shared.GetHash(addedFiles[x]);

                file.Action = FileAction.Update;

                string folder = Path.GetDirectoryName(addedFiles[x]);
                string fileName = Path.GetFileName(addedFiles[x]);
                string parent = Directory.GetParent(addedFiles[x]).Name;
                if (folder != selectedFolder)
                    file.Source = "[DownloadDir]" + "/" + parent + "/" + fileName;   
                else
                    file.Source = "[DownloadDir]" + "/" + fileName;   
               
                
               
                if (file.Source.Substring(0, 8).ToLower() == "[appdir]")
                    if (file.Source.Substring(8, 1) != "\\")
                        file.Source = file.Source.Insert(8, "\\");

                file.Arguments = null;

                file.Destination = Shared.ConvertPath(addedFiles[x], false, Main.Is64Bit);

                file.Destination = Shared.Replace(file.Destination, AppInfo.ApplicationDirectory, "[AppDir]");

                file.Size = Convert.ToUInt64(fi.Length);

                files.Add(file);
            }
        }

        #endregion

    }
}
