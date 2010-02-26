#region GNU Public License Version 3

// Copyright 2007-2010 Robert Baker, Seven Software.
// This file is part of Seven Update.
//   
//      Seven Update is free software: you can redistribute it and/or modify
//      it under the terms of the GNU General Public License as published by
//      the Free Software Foundation, either version 3 of the License, or
//      (at your option) any later version.
//  
//      Seven Update is distributed in the hope that it will be useful,
//      but WITHOUT ANY WARRANTY; without even the implied warranty of
//      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//      GNU General Public License for more details.
//   
//      You should have received a copy of the GNU General Public License
//      along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

#endregion

#region

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SevenUpdate.Base;
using SevenUpdate.Sdk.WinForms;

#endregion

namespace SevenUpdate.Sdk.Pages
{
    public sealed partial class Files : UserControl
    {
        #region Global Vars

        /// <summary>
        /// Current Index of file selected
        /// </summary>
        private int index;

        /// <summary>
        /// The selected folder when adding UpdateFiles
        /// </summary>
        private string selectedFolder;

        private BackgroundWorker worker;

        /// <summary>
        /// Gets the UpdateFiles of the update
        /// </summary>
        internal ObservableCollection<UpdateFile> UpdateFiles { get; private set; }

        private delegate void SetTextCallback(string hash);

        #endregion

        public Files()
        {
            Font = SystemFonts.MessageBoxFont;

            InitializeComponent();

            cbFileAction.SelectedIndex = 0;
        }

        #region Methods

        /// <summary>
        /// Adds a new file or group of UpdateFiles to the update
        /// </summary>
        private void AddFiles(string[] addedFiles)
        {
            Cursor = Cursors.WaitCursor;
            worker = new BackgroundWorker();
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerAsync(addedFiles);
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        }

        /// <summary>
        /// Clears the UI
        /// </summary>
        /// <param name="clearFiles">True  if to clear the collection of UpdateFiles</param>
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
            var x = lbFiles.SelectedIndex;
            if (x < 0)
                return;

            if (UpdateFiles.Count > 0)
            {
                UpdateFiles.RemoveAt(lbFiles.SelectedIndex);

                lbFiles.Items.RemoveAt(lbFiles.SelectedIndex);
            }

            if (lbFiles.Items.Count > 0)
            {
                if (x > 0)
                    lbFiles.SelectedIndex = x - 1;
                else
                    lbFiles.SelectedIndex = 0;
            }
            else
                ClearUI(false);
            tmiAddFiles.Enabled = true;
            tmiAddFolder.Enabled = true;
        }

        /// <summary>
        /// Loads a file to the UI
        /// </summary>
        /// <param name="x">The file Index</param>
        private void LoadFile(int x)
        {
            if (x < 0)
                return;

            ClearUI(false);

            txtFileLoc.Text = UpdateFiles[x].Destination;

            txtDownloadLoc.Text = UpdateFiles[x].Source;

            txtArgs.Text = UpdateFiles[x].Args;
            if (UpdateFiles[x].Hash != null)
                lblGetHash.Text = UpdateFiles[x].Hash;

            switch (UpdateFiles[x].Action)
            {
                case FileAction.Update:
                    cbFileAction.SelectedIndex = 0;
                    break;
                case FileAction.UpdateIfExist:
                    cbFileAction.SelectedIndex = 1;
                    break;
                case FileAction.UpdateThenRegister:
                    cbFileAction.SelectedIndex = 2;
                    break;
                case FileAction.UpdateThenExecute:
                    cbFileAction.SelectedIndex = 3;
                    break;
                case FileAction.CompareOnly:
                    cbFileAction.SelectedIndex = 4;
                    break;
                case FileAction.Execute:
                    cbFileAction.SelectedIndex = 5;
                    break;
                case FileAction.Delete:
                    cbFileAction.SelectedIndex = 6;
                    break;
                case FileAction.ExecuteThenDelete:
                    cbFileAction.SelectedIndex = 7;
                    break;
                case FileAction.UnregisterThenDelete:
                    cbFileAction.SelectedIndex = 8;
                    break;
            }

            txtSize.Text = UpdateFiles[x].Size.ToString();
        }

        /// <summary>
        /// Loads the fileItems into the UI
        /// </summary>
        internal void LoadFiles(ObservableCollection<UpdateFile> fileItems)
        {
            UpdateFiles = new ObservableCollection<UpdateFile>();

            for (var x = 0; x < fileItems.Count; x++)
                UpdateFiles.Add(fileItems[x]);

            lbFiles.Items.Clear();

            for (var x = 0; x < UpdateFiles.Count; x++)
                lbFiles.Items.Add(Path.GetFileName(UpdateFiles[x].Destination));

            if (lbFiles.Items.Count <= 0)
                return;
            lbFiles.SelectedIndex = 0;

            index = lbFiles.SelectedIndex;

            LoadFile(index);
        }

        /// <summary>
        /// Saves a file to the list
        /// </summary>
        private void SaveFile()
        {
            var file = new UpdateFile {Hash = lblGetHash.Enabled ? lblGetHash.Text : null};

            switch (cbFileAction.SelectedIndex)
            {
                case 0:
                    file.Action = FileAction.Update;
                    break;
                case 1:
                    file.Action = FileAction.UpdateIfExist;
                    break;
                case 2:
                    file.Action = FileAction.UpdateThenRegister;
                    break;
                case 3:
                    file.Action = FileAction.UpdateThenExecute;
                    break;
                case 4:
                    file.Action = FileAction.CompareOnly;
                    break;
                case 5:
                    file.Action = FileAction.Execute;
                    break;
                case 6:
                    file.Hash = null;
                    file.Action = FileAction.Delete;
                    break;
                case 7:
                    file.Action = FileAction.ExecuteThenDelete;
                    break;
                case 8:
                    file.Hash = null;
                    file.Action = FileAction.UnregisterThenDelete;
                    break;
            }
            txtFileLoc.Text = Base.Base.ConvertPath(txtFileLoc.Text, false, Main.Is64Bit);
            txtFileLoc.Text = Base.Base.Replace(txtFileLoc.Text, AppInfo.ApplicationDirectory, "[AppDir]");

            txtDownloadLoc.Text = Base.Base.ConvertPath(txtDownloadLoc.Text, false, Main.Is64Bit);
            txtDownloadLoc.Text = Base.Base.Replace(txtDownloadLoc.Text, UpdateInfo.DownloadDirectory, "[DownloadDir]");

            if (txtFileLoc.Text.Length > 8)
            {
                if (txtFileLoc.Text.Substring(0, 8).ToLower() == "[appdir]")
                {
                    if (txtFileLoc.Text.Substring(8, 1) != "\\")
                        txtFileLoc.Text = txtFileLoc.Text.Insert(8, "\\");
                }
            }

            file.Args = txtArgs.Text;

            file.Destination = txtFileLoc.Text;

            file.Source = txtDownloadLoc.Text;

            file.Size = Convert.ToUInt64(txtSize.Text);

            if (index > -1 && UpdateFiles.Count > 0)
            {
                UpdateFiles.RemoveAt(index);

                UpdateFiles.Insert(index, file);
                lbFiles.Items[index] = Path.GetFileName(file.Destination);
                lbFiles.SelectedIndex = index;
            }
            else
            {
                UpdateFiles.Add(file);
                lbFiles.Items.Add(Path.GetFileName(file.Destination));
                lbFiles.SelectedIndex = lbFiles.Items.Count - 1;
            }
        }

        private void SetHash(string hash)
        {
            if (lblGetHash.InvokeRequired)
            {
                var d = new SetTextCallback(SetHash);
                Invoke(d, new object[] {hash});
            }
            else
                lblGetHash.Text = hash;
        }

        #endregion

        #region UI Events

        #region Button

        private void SaveClick(object sender, EventArgs e)
        {
            if (lblGetHash.Text != Program.RM.GetString("CalculateHashSize") || cbFileAction.SelectedIndex == 4 || cbFileAction.SelectedIndex == 6 ||
                cbFileAction.SelectedIndex == 8)
            {
                if (txtSize.Text.Length > 0)
                    SaveFile();
                else
                    MessageBox.Show(Program.RM.GetString("EnterFileSize"));
            }
            else
                MessageBox.Show(Program.RM.GetString("CalculateHash"));
        }

        #endregion

        #region Context Menu Items

        private void DeleteFile_Click(object sender, EventArgs e)
        {
            DeleteFile();
        }

        private void AddFiles_Click(object sender, EventArgs e)
        {
            dlgOpenFile.Multiselect = true;
            dlgOpenFile.FileName = null;
            if (dlgOpenFile.ShowDialog() != DialogResult.OK)
                return;
            selectedFolder = Path.GetDirectoryName(dlgOpenFile.FileName);
            ClearUI(false);
            AddFiles(dlgOpenFile.FileNames);
        }

        private void AddFolder_Click(object sender, EventArgs e)
        {
            if (dlgOpenFolder.ShowDialog() != DialogResult.OK)
                return;
            selectedFolder = dlgOpenFolder.SelectedPath;
            ClearUI(false);
            AddFiles(Directory.GetFiles(dlgOpenFolder.SelectedPath, "*", SearchOption.AllDirectories));
        }

        #endregion

        #region Labels

        private void Browse_Click(object sender, EventArgs e)
        {
            dlgOpenFile.Multiselect = false;
            dlgOpenFile.FileName = null;
            if (dlgOpenFile.ShowDialog() != DialogResult.OK)
                return;
            txtFileLoc.Text = dlgOpenFile.FileName;
            worker = new BackgroundWorker();
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted2;
            worker.DoWork += Worker_DoWork2;
            Cursor = Cursors.WaitCursor;
            worker.RunWorkerAsync(dlgOpenFile.FileName);

            if (txtFileLoc != null)
                txtSize.Text = new FileInfo(txtFileLoc.Text).Length.ToString();
            SaveFile();
        }

        private void GetHash_Click(object sender, EventArgs e)
        {
            dlgOpenFile.FileName = null;

            var dir = Base.Base.ConvertPath(AppInfo.ApplicationDirectory, true, Main.Is64Bit) + @"\";

            if (!Directory.Exists(dir))
                dir = null;

            dlgOpenFile.FileName = dir + Path.GetFileName(txtFileLoc.Text);

            if (dlgOpenFile.ShowDialog() != DialogResult.OK)
                return;
            selectedFolder = Path.GetDirectoryName(dlgOpenFile.FileName);
            worker = new BackgroundWorker();
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted2;
            worker.DoWork += Worker_DoWork2;
            Cursor = Cursors.WaitCursor;
            worker.RunWorkerAsync(dlgOpenFile.FileName);

            var fi = new FileInfo(dlgOpenFile.FileName);

            txtSize.Text = fi.Length.ToString();
        }

        private void Files_SelectedIndexChanged(object sender, EventArgs e)
        {
            index = lbFiles.SelectedIndex;

            if (index >= 0)
                LoadFile(index);
        }

        private void Label_MouseEnter(object sender, EventArgs e)
        {
            var label = ((Label) sender);
            label.ForeColor = Color.FromArgb(51, 153, 255);
            label.Font = new Font(label.Font, FontStyle.Underline);
        }

        private void Label_MouseLeave(object sender, EventArgs e)
        {
            var label = ((Label) sender);
            label.ForeColor = Color.FromArgb(0, 102, 204);
            label.Font = new Font(label.Font, FontStyle.Regular);
        }

        #endregion

        private void FileAction_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbFileAction.SelectedIndex == 4 || cbFileAction.SelectedIndex == 6 || cbFileAction.SelectedIndex == 8)
            {
                lblGetHash.Enabled = cbFileAction.SelectedIndex == 4;
                txtDownloadLoc.Enabled = false;
                txtDownloadLoc.Text = null;
            }
            else
            {
                lblGetHash.Enabled = true;
                txtDownloadLoc.Enabled = true;
            }
        }

        #region lbFiles

        private void Files_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;
            var x = lbFiles.IndexFromPoint(e.X, e.Y);
            if (x > -1)
                lbFiles.SelectedIndex = x;

            tmiDeleteFile.Enabled = lbFiles.SelectedIndex >= 0;

            cmsMenu.Show(MousePosition);
        }

        private void Files_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Delete)
                DeleteFile();
        }

        #endregion

        #endregion

        #region Worker Events

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Cursor = Cursors.Arrow;
            lbFiles.Items.Clear();
            for (var x = 0; x < UpdateFiles.Count; x++)
                lbFiles.Items.Add(Path.GetFileName(UpdateFiles[x].Destination));
            lbFiles.SelectedIndex = lbFiles.Items.Count - 1;
            worker.DoWork -= Worker_DoWork;
            worker.RunWorkerCompleted -= Worker_RunWorkerCompleted;
        }

        private void Worker_RunWorkerCompleted2(object sender, RunWorkerCompletedEventArgs e)
        {
            Cursor = Cursors.Arrow;
            worker.DoWork -= Worker_DoWork2;
            worker.RunWorkerCompleted -= Worker_RunWorkerCompleted2;
        }

        private void Worker_DoWork2(object sender, DoWorkEventArgs e)
        {
            var file = ((string) e.Argument);
            SetHash(Base.Base.GetHash(file));
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var addedFiles = ((string[]) e.Argument);
            if (UpdateFiles == null)
                UpdateFiles = new ObservableCollection<UpdateFile>();

            for (var x = 0; x < addedFiles.Length; x++)
            {
                var fi = new FileInfo(addedFiles[x]);

                var file = new UpdateFile {Hash = Base.Base.GetHash(addedFiles[x]), Action = FileAction.UpdateIfExist};

                var folder = Path.GetDirectoryName(addedFiles[x]);
                var fileName = Path.GetFileName(addedFiles[x]);
                var parent = Directory.GetParent(addedFiles[x]).Name;
                if (folder != selectedFolder)
                    file.Source = "[DownloadDir]" + "/" + parent + "/" + fileName;
                else
                    file.Source = "[DownloadDir]" + "/" + fileName;


                if (file.Source.Substring(0, 8).ToLower() == "[appdir]")
                {
                    if (file.Source.Substring(8, 1) != "\\")
                        file.Source = file.Source.Insert(8, "\\");
                }

                file.Args = null;

                file.Destination = Base.Base.ConvertPath(addedFiles[x], false, Main.Is64Bit);

                file.Destination = Base.Base.Replace(file.Destination, AppInfo.ApplicationDirectory, "[AppDir]");

                file.Size = Convert.ToUInt64(fi.Length);

                UpdateFiles.Add(file);
            }
        }

        #endregion
    }
}